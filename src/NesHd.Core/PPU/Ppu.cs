using System.Drawing;
using NesHd.Core.Debugger;
using NesHd.Core.Input;
using NesHd.Core.Memory;
using NesHd.Core.Misc;
using NesHd.Core.Output.Video;
using NesHd.Core.Output.Video.Devices;

namespace NesHd.Core.PPU
{
    public class Ppu
    {
        private readonly NesEngine _engine;
        private readonly Timer _timer = new Timer();
        public bool BackgroundClipping;
        public bool BackgroundVisibility;
        public bool CheckZapperHit;
        public ushort ColorEmphasis;
        public int CurrentScanLine;
        public bool ExecuteNMIonVBlank;

        //I HATE FIXES !!
        public bool FixScroll;
        public bool FixScroll2;
        public bool FixScroll3;
        private double _framePeriod;
        public byte HScroll;
        public bool IsMapperMirroring;
        public bool MonochromeMode;
        public int[] Palette = new int[64];
        public bool PpuToggle = true;

        /*PPU*/
        public int PatternTableAddress8X8Sprites;
        public int PatternTableAddressBackground;
        public byte ReloadBits2000;
        public byte[] SprRam;
        public int ScanlineOfVblank;
        public int ScanlinesPerFrame;
        public bool Sprite0Hit;

        /*2001*/
        public bool SpriteClipping;
        public int SpriteCrossed;

        /*2003*/
        public byte SpriteRamAddress;
        public bool SpriteSize; //true=8x16, false=8x8
        public bool SpriteVisibility;
        public int TileY;
        public bool VBlank;
        public int VBits;
        public byte[] VRam;
        public ushort VRamAddress;
        public int VRamAddressIncrement = 1;
        public byte VRamReadBuffer;

        /*2005,2006*/
        public ushort VRamTemp;

        /*Draw stuff*/
        public int VScroll;
        public int ZapperFrame;
        /*Zapper*/
        public int ZapperX;
        public int ZapperY;
        private double _currentFrameTime;
        private double _lastFrameTime;

        private readonly int _transparentColor = Color.FromArgb(255, 166, 202, 240).ToArgb();
        private readonly Cartridge _cartridge;
        public int[][] Bitmap { get; set; }
        public int BitmapWidth { get; set; }
        public int BitmapOffset { get; set; }

        /// <summary>
        /// The picture unit
        /// </summary>
        /// <param name="tv">The tv format</param>
        /// <param name="paletteFormat">Palette Format</param>
        /// <param name="engine">Engine NES</param>
        public Ppu(TvFormat tv, PaletteFormat paletteFormat, NesEngine engine)
        {
            _engine = engine;
            _cartridge = _engine.Memory.Map.Cartridge;

            VRam = new byte[0x2000];
            SprRam = new byte[0x100];
            SetTvFormat(tv, paletteFormat);
            Debug.WriteLine(this, "PPU initialized ok.", DebugStatus.Cool);
        }

        public IGraphicDevice OutputDevice { get; set; }
        public int Fps { get; set; }
        public bool NoLimiter { get; set; }

        /// <summary>
        /// True when NMI is needed
        /// </summary>
        /// <returns>NMI</returns>
        public bool DoScanline()
        {
            //Start a new frame
            if (CurrentScanLine == 0)
            {
                OutputDevice.Begin();
            }
            if (CurrentScanLine < 240)
            {
                //Clean up the line from before
                var theByte = VRam[0x1F00];
                if (theByte >= 63)
                {
                    theByte = 63;
                }
                var theY = CurrentScanLine * _cartridge.Multi;
                for (var x = 0; x < 256 * _cartridge.Multi; x += _cartridge.Multi)
                {
                    for (var x1 = 0; x1 < _cartridge.Multi; x1++)
                    {
                        for (var y1 = 0; y1 < _cartridge.Multi; y1++)
                        {
                            OutputDevice.DrawAbsolutePixel(x + x1, theY + y1, Palette[theByte]);
                        }
                    }
                }
                //first we should reset the sprite crossed for reg $2002
                SpriteCrossed = 0;
                //Render the sprites and background
                if (SpriteVisibility)
                    RenderSprites(0x20);
                if (BackgroundVisibility)
                    RenderBackground();
                if (SpriteVisibility)
                    RenderSprites(0);
                //Do clipping
                if (!BackgroundClipping)
                    for (var i = 0; i < 8; i++)
                    {
                        var x = i * _cartridge.Multi;
                        var y = CurrentScanLine * _cartridge.Multi;
                        for (var x1 = 0; x1 < _cartridge.Multi; x1++)
                        {
                            for (var y1 = 0; y1 < _cartridge.Multi; y1++)
                            {
                                OutputDevice.DrawAbsolutePixel(x + x1, y + y1, Palette[theByte]);
                            }
                        }
                    }
                //(clock each scanline, paused during VBlank)
                if (SpriteVisibility | BackgroundVisibility)
                    _engine.Memory.Map.TickScanlineTimer();
            }
            //It should clock each cpu cycle, but here will clock in each 
            //scanline to make the emu faster.
            //The IRQ counter of the mapper will be decremented by
            //the cycles that done by CPU at scanline.
            _engine.Memory.Map.TickCycleTimer();
            //Set the status of the VBlank
            if (CurrentScanLine == 240)
            {
                VBlank = true;
                if (CheckZapperHit)
                    ZapperFrame++;
            }
            //Render the sound
            if (CurrentScanLine >= 240 & CurrentScanLine < 242)
                if (_engine.SoundEnabled)
                    _engine.Apu.RenderFrame();
            //Draw the frame into the screen
            if (CurrentScanLine == ScanlineOfVblank + 1)
                OutputDevice.RenderFrame();
            //Advance the scanline
            CurrentScanLine++;
            //End of the frame
            if (CurrentScanLine == ScanlinesPerFrame)
            {
                //Handle the speed
                if (!NoLimiter)
                {
                    var currentTime = _timer.GetCurrentTime();
                    _currentFrameTime = currentTime - _lastFrameTime;
                    if ((_currentFrameTime < _framePeriod))
                    {
                        while (true)
                        {
                            if ((_timer.GetCurrentTime() - _lastFrameTime) >= _framePeriod)
                            {
                                break;
                            }
                        }
                    }
                }
                _lastFrameTime = _timer.GetCurrentTime();
                //Do routine
                Fps++;
                CurrentScanLine = 0;
                Sprite0Hit = false;
            }
            //Return if the cpu should hit the NMI
            return ((CurrentScanLine == ScanlineOfVblank) & ExecuteNMIonVBlank);
        }

        private void RenderSprites(int behind)
        {
            var spriteSize = SpriteSize ? 16 : 8;
            //1: loop through SPR-RAM
            for (var i = 252; i >= 0; i = i - 4)
            {
                var yCoordinate = (byte)(SprRam[i] + 1);
                //2: if the sprite falls on the current scanline, draw it
                if (((SprRam[i + 2] & 0x20) == behind) && (yCoordinate <= CurrentScanLine) && ((yCoordinate + spriteSize) > CurrentScanLine))
                {
                    SpriteCrossed++;
                    //3: Draw the sprites differently if they are 8x8 or 8x16
                    if (!SpriteSize) //8x8
                    {
                        Draw8X8Sprite(i, yCoordinate);
                    }
                    else //8x16
                    {
                        Draw8X16Sprite(i, yCoordinate);
                    }
                }
                else
                {
                    SpriteRamAddress = 0;
                }
            }
        }

        private void Draw8X16Sprite(int i, byte yCoordinate)
        {
            //4: get the sprite id
            var spriteId = SprRam[i + 1];
            int lineToDraw;
            if ((SprRam[i + 2] & 0x80) != 0x80)
                lineToDraw = CurrentScanLine - yCoordinate;
            else
                lineToDraw = yCoordinate + 15 - CurrentScanLine;
            //5: We draw the sprite like two halves, so getting past the 
            //first 8 puts us into the next tile
            //If the ID is even, the tile is in 0x0000, odd 0x1000
            int spriteOffset;
            if (lineToDraw < 8)
            {
                //Draw the top tile
                if ((spriteId % 2) == 0)
                    spriteOffset = 0x0000 + (spriteId) * 16;
                else
                    spriteOffset = 0x1000 + (spriteId - 1) * 16;
            }
            else
            {
                //Draw the bottom tile
                lineToDraw -= 8;
                if ((spriteId % 2) == 0)
                    spriteOffset = 0x0000 + (spriteId + 1) * 16;
                else
                    spriteOffset = 0x1000 + (spriteId) * 16;
            }
            //6: extract our tile data
            var tileData1 = _engine.Memory.Map.ReadChr((ushort)(spriteOffset + lineToDraw));
            var tileData2 = _engine.Memory.Map.ReadChr((ushort)(spriteOffset + lineToDraw + 8));
            //7: get the palette attribute data
            var paletteUpperBits = (byte)((SprRam[i + 2] & 0x3) << 2);
            //8, render the line inside the tile to the screen
            int pixelColor = 0;
            for (var j = 0; j < 8; j++)
            {
                if ((SprRam[i + 2] & 0x40) == 0x40)
                {
                    pixelColor = paletteUpperBits + (((tileData2 & (1 << (j))) >> (j)) << 1) +
                                 ((tileData1 & (1 << (j))) >> (j));
                }
                else
                {
                    pixelColor = paletteUpperBits + (((tileData2 & (1 << (7 - j))) >> (7 - j)) << 1) +
                                 ((tileData1 & (1 << (7 - j))) >> (7 - j));
                }

                if (OutputDevice.Name == "Windows GDI Hi Res")
                {
                    //store which chr page is being used
                    var tilepage = _engine.Memory.Map.ReadChrPageNo((ushort)(spriteOffset));
                    if ((SprRam[i + 3] + j) < 256)
                    {
                        ((VideoGdiHiRes)OutputDevice).DrawPixelHiRes((SprRam[i + 3]) + j, CurrentScanLine,
                                                                      Palette[(0x3f & VRam[0x1F10 + pixelColor])],
                                                                      tilepage, spriteOffset & 0x0003ff,
                                                                      ((SprRam[i + 2] & 0x40) == 0x40) ? j : 7 - j,
                                                                      lineToDraw, (SprRam[i + 2] & 0x40) == 0x40,
                                                                      (SprRam[i + 2] & 0x80) == 0x80,
                                                                      VRam[
                                                                          0x1F10 + pixelColor - (pixelColor % 4) + 1
                                                                          ],
                                                                      VRam[
                                                                          0x1F10 + pixelColor - (pixelColor % 4) + 2
                                                                          ],
                                                                      VRam[
                                                                          0x1F10 + pixelColor - (pixelColor % 4) + 3
                                                                          ], pixelColor);
                        if (((pixelColor % 4) != 0) && (i == 0))
                        {
                            Sprite0Hit = true;
                        }
                    }
                }
                else
                {
                    if ((pixelColor % 4) != 0)
                    {
                        if ((SprRam[i + 3] + j) < 256)
                        {
                            OutputDevice.DrawPixel((SprRam[i + 3]) + j,
                                                   CurrentScanLine,
                                                   Palette[(0x3f & VRam[0x1F10 + pixelColor])]);
                            if (i == 0)
                            {
                                Sprite0Hit = true;
                            }
                        }
                    }
                }
            }
            if (CheckZapperHit & ZapperFrame > 2)
            {
                if ((ZapperX >= (SprRam[i + 3]) & ZapperX <= (SprRam[i + 3] + 8))
                    & (ZapperY >= yCoordinate & ZapperY <= yCoordinate + 16)
                    & Palette[(0x3f & VRam[0x1F10 + pixelColor])] >= 0xFF)
                {
                    _engine.Memory.Zapper.SetDetect(0);
                    CheckZapperHit = false;
                }
            }
        }

        private void Draw8X8Sprite(int i, byte yCoordinate)
        {
            int lineToDraw;
            //4: calculate which line of the sprite is currently being drawn
            //Line to draw is: currentScanline - Y coord + 1
            if ((SprRam[i + 2] & 0x80) != 0x80)
            {
                lineToDraw = CurrentScanLine - yCoordinate;
            }
            else
            {
                lineToDraw = yCoordinate + 7 - CurrentScanLine;
            }
            //5: calculate the offset to the sprite's data in
            //our chr rom data 
            var spriteOffset = PatternTableAddress8X8Sprites + SprRam[i + 1] * 16;
            //6: extract our tile data
            var tileData1 = _engine.Memory.Map.ReadChr((ushort)(spriteOffset + lineToDraw));
            var tileData2 = _engine.Memory.Map.ReadChr((ushort)(spriteOffset + lineToDraw + 8));
            //7: get the palette attribute data
            var paletteUpperBits = (byte)((SprRam[i + 2] & 0x3) << 2);


            if (DrawHdTile(i, spriteOffset, lineToDraw))
            {
                //return;
            }


            //8: render the line inside the tile into the screen direcly
            for (var j = 0; j < 8; j++)
            {
                int pixelColor;
                if ((SprRam[i + 2] & 0x40) == 0x40)
                {
                    pixelColor = paletteUpperBits + (((tileData2 & (1 << (j))) >> (j)) << 1) +
                                 ((tileData1 & (1 << (j))) >> (j));
                }
                else
                {
                    pixelColor = paletteUpperBits + (((tileData2 & (1 << (7 - j))) >> (7 - j)) << 1) +
                                 ((tileData1 & (1 << (7 - j))) >> (7 - j));
                }
                //Hi Res stages
                if (OutputDevice.Name == "Windows GDI Hi Res")
                {
                    //store which chr page is being used
                    var tilepage = _engine.Memory.Map.ReadChrPageNo((ushort)(spriteOffset));
                    if ((SprRam[i + 3] + j) < 256)
                    {
                        ((VideoGdiHiRes)OutputDevice).DrawPixelHiRes((SprRam[i + 3]) + j, CurrentScanLine,
                                                                      Palette[(0x3f & VRam[0x1F10 + pixelColor])],
                                                                      tilepage, spriteOffset & 0x0003ff,
                                                                      ((SprRam[i + 2] & 0x40) == 0x40) ? j : 7 - j,
                                                                      lineToDraw, (SprRam[i + 2] & 0x40) != 0x40,
                                                                      (SprRam[i + 2] & 0x80) == 0x80,
                                                                      VRam[
                                                                          0x1F10 + pixelColor - (pixelColor % 4) + 1
                                                                          ],
                                                                      VRam[
                                                                          0x1F10 + pixelColor - (pixelColor % 4) + 2
                                                                          ],
                                                                      VRam[
                                                                          0x1F10 + pixelColor - (pixelColor % 4) + 3
                                                                          ], pixelColor);
                        if (((pixelColor % 4) != 0) && (i == 0))
                        {
                            Sprite0Hit = true;
                        }
                    }
                }
                else
                {
                    if ((pixelColor % 4) != 0)
                    {
                        if ((SprRam[i + 3] + j) < 256)
                        {
                            OutputDevice.DrawAbsolutePixel((SprRam[i + 3]) + j, CurrentScanLine,
                                                   Palette[(0x3f & VRam[0x1F10 + pixelColor])]);
                            if (i == 0)
                            {
                                Sprite0Hit = true;
                            }
                        }
                    }
                }
            }
            //Check for the zapper
            if (CheckZapperHit & ZapperFrame > 2)
            {
                if ((ZapperX >= (SprRam[i + 3]) & ZapperX <= (SprRam[i + 3] + 8))
                    & (ZapperY >= yCoordinate & ZapperY <= yCoordinate + 8))
                {
                    _engine.Memory.Zapper.SetDetect(0);
                    CheckZapperHit = false;
                }
            }
        }

        private bool DrawHdTile(int i, int spriteOffset, int lineToDraw)
        {
            var start = (ushort)(spriteOffset + lineToDraw);
            var mustInvert = (SprRam[i + 2] & 0x40) == 0x40;

            var absPos = start + BitmapOffset;
            if (absPos >= BitmapOffset && absPos < BitmapOffset + 4000)
            {
                var offset = absPos - BitmapOffset;

                for (var j = 0; j < 8; j++)
                {
                    var x = GetX(offset, (mustInvert ? 7 - j : j), _cartridge.Multi);
                    var y = GetY(offset, BitmapWidth, _cartridge.Multi);

                    var x1 = (SprRam[i + 3] + j) * _cartridge.Multi;
                    var y1 = CurrentScanLine * _cartridge.Multi;

                    for (var x2 = 0; x2 < _cartridge.Multi; x2++)
                    {
                        for (var y2 = 0; y2 < _cartridge.Multi; y2++)
                        {
                            var pixelColor = Bitmap[x + x2][y + y2];
                            if (pixelColor != _transparentColor)
                            {
                                OutputDevice.DrawAbsolutePixel(x1 + x2, y1 + y2, pixelColor);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public int GetX(int offset, int desl, int multi)
        {
            return (desl + ((offset / 16) * 8) % 128) * multi;
        }

        public int GetY(int offset, int w, int multi)
        {
            return ((offset % 16) + (offset / ((w / multi) * 2) * 8)) * multi;
        }

        private void RenderBackground()
        {
            var nameTableAddress = 0;
            switch (ReloadBits2000)
            {
                case 0:
                    nameTableAddress = 0x2000;
                    break;
                case 1:
                    nameTableAddress = 0x2400;
                    break;
                case 2:
                    nameTableAddress = 0x2800;
                    break;
                case 3:
                    nameTableAddress = 0x2C00;
                    break;
            }
            for (var vScrollSide = 0; vScrollSide < 2; vScrollSide++)
            {
                var virtualScanline = CurrentScanLine + VScroll;
                if (virtualScanline < 0)
                    virtualScanline = 0;
                var nameTableBase = nameTableAddress;
                int startColumn;
                int endColumn;
                if (vScrollSide == 0)
                {
                    if (virtualScanline >= 240)
                    {
                        switch (nameTableAddress)
                        {
                            case 0x2000:
                                nameTableBase = 0x2800;
                                break;
                            case 0x2400:
                                nameTableBase = 0x2C00;
                                break;
                            case 0x2800:
                                nameTableBase = 0x2000;
                                break;
                            case 0x2C00:
                                nameTableBase = 0x2400;
                                break;
                        }
                        virtualScanline -= 240;
                    }
                    startColumn = HScroll / 8;
                    endColumn = 32;
                }
                else
                {
                    if (virtualScanline >= 240)
                    {
                        switch (nameTableAddress)
                        {
                            case 0x2000:
                                nameTableBase = 0x2C00;
                                break;
                            case 0x2400:
                                nameTableBase = 0x2800;
                                break;
                            case 0x2800:
                                nameTableBase = 0x2400;
                                break;
                            case 0x2C00:
                                nameTableBase = 0x2000;
                                break;
                        }
                        virtualScanline -= 240;
                    }
                    else
                    {
                        switch (nameTableAddress)
                        {
                            case 0x2000:
                                nameTableBase = 0x2400;
                                break;
                            case 0x2400:
                                nameTableBase = 0x2000;
                                break;
                            case 0x2800:
                                nameTableBase = 0x2C00;
                                break;
                            case 0x2C00:
                                nameTableBase = 0x2800;
                                break;
                        }
                    }
                    startColumn = 0;
                    endColumn = (HScroll / 8) + 1;
                }
                //Next Try: Forcing two page only: 0x2000 and 0x2400				
                switch (_cartridge.Mirroring)
                {
                    case Mirroring.Horizontal:
                        switch (nameTableBase)
                        {
                            case 0x2400:
                                nameTableBase = 0x2000;
                                break;
                            case 0x2800:
                                nameTableBase = 0x2400;
                                break;
                            case 0x2C00:
                                nameTableBase = 0x2400;
                                break;
                        }
                        break;
                    case Mirroring.Vertical:
                        switch (nameTableBase)
                        {
                            case 0x2800:
                                nameTableBase = 0x2000;
                                break;
                            case 0x2C00:
                                nameTableBase = 0x2400;
                                break;
                        }
                        break;
                    case Mirroring.OneScreen:
                        nameTableBase = (int)_cartridge.MirroringBase;
                        break;
                }
                for (var currentTileColumn = startColumn; currentTileColumn < endColumn; currentTileColumn++)
                {
                    //Starting tile row is currentScanline / 8
                    //The offset in the tile is currentScanline % 8

                    //Step #1, get the tile number
                    int tileNumber = VRam[((nameTableBase - 0x2000) + ((virtualScanline / 8) * 32) + currentTileColumn)];

                    //Step #2, get the offset for the tile in the tile data
                    var tileDataOffset = PatternTableAddressBackground + (tileNumber * 16);

                    //Step #3, get the tile data from chr rom
                    int tiledata1 = _engine.Memory.Map.ReadChr((ushort)(tileDataOffset + (virtualScanline % 8)));
                    int tiledata2 = _engine.Memory.Map.ReadChr((ushort)(tileDataOffset + (virtualScanline % 8) + 8));

                    //Step #4, get the attribute byte for the block of tiles we're in
                    //this will put us in the correct section in the palette table
                    int paletteHighBits = VRam[((nameTableBase - 0x2000 + 0x3c0 + (((virtualScanline / 8) / 4) * 8) + (currentTileColumn / 4)))];
                    paletteHighBits = (byte)(paletteHighBits >> ((4 * (((virtualScanline / 8) % 4) / 2)) + (2 * ((currentTileColumn % 4) / 2))));
                    paletteHighBits = (byte)((paletteHighBits & 0x3) << 2);

                    //Step #5, render the line inside the tile to the offscreen buffer
                    int startTilePixel;
                    int endTilePixel;
                    if (vScrollSide == 0)
                    {
                        if (currentTileColumn == startColumn)
                        {
                            startTilePixel = HScroll % 8;
                            endTilePixel = 8;
                        }
                        else
                        {
                            startTilePixel = 0;
                            endTilePixel = 8;
                        }
                    }
                    else
                    {
                        if (currentTileColumn == endColumn)
                        {
                            startTilePixel = 0;
                            endTilePixel = HScroll % 8;
                        }
                        else
                        {
                            startTilePixel = 0;
                            endTilePixel = 8;
                        }
                    }

                    var start = (ushort)(tileDataOffset + (virtualScanline % 8));
                    var absPos = start + BitmapOffset;

                    if (absPos >= BitmapOffset && absPos < BitmapOffset + 8000)
                    {
                        var offset = absPos - BitmapOffset;

                        for (var i = startTilePixel; i < endTilePixel; i++)
                        {
                            var x = GetX(offset, i, _cartridge.Multi);
                            var y = GetY(offset, BitmapWidth, _cartridge.Multi);

                            var x1 = 0;
                            var y1 = 0;

                            if (vScrollSide == 0)
                            {
                                x1 = ((8 * currentTileColumn) - HScroll + i) * _cartridge.Multi;
                                y1 = (CurrentScanLine) * _cartridge.Multi;
                            }
                            else if (((8 * currentTileColumn) + (256 - HScroll) + i) < 256)
                            {
                                x1 = ((8 * currentTileColumn) + (256 - HScroll) + i) * _cartridge.Multi;
                                y1 = (CurrentScanLine) * _cartridge.Multi;
                            }

                            for (var x2 = 0; x2 < _cartridge.Multi; x2++)
                            {
                                for (var y2 = 0; y2 < _cartridge.Multi; y2++)
                                {
                                    try
                                    {
                                        var pixelColor = Bitmap[x + x2][y + y2];
                                        if (pixelColor == _transparentColor)
                                        {
                                            continue;
                                        }
                                        OutputDevice.DrawAbsolutePixel(x1 + x2, y1 + y2, pixelColor);
                                    }
                                    catch
                                    {

                                    }

                                }
                            }
                        }
                        //continue;
                    }

                    for (var i = startTilePixel; i < endTilePixel; i++)
                    {
                        var pixelColor = paletteHighBits + (((tiledata2 & (1 << (7 - i))) >> (7 - i)) << 1) +
                                         ((tiledata1 & (1 << (7 - i))) >> (7 - i));

                        //Hi Res stages
                        if (OutputDevice.Name == "Windows GDI Hi Res")
                        {
                            //store which chr page is being used
                            var tilepage = _engine.Memory.Map.ReadChrPageNo((ushort)(tileDataOffset));

                            if (vScrollSide == 0)
                            {
                                ((VideoGdiHiRes)OutputDevice).DrawPixelHiRes((8 * currentTileColumn) - HScroll + i,
                                                                        CurrentScanLine,
                                                                        Palette[(0x3f & VRam[0x1f00 + pixelColor])],
                                                                        tilepage, tileDataOffset & 0x0003ff, 7 - i,
                                                                        virtualScanline % 8, true, false,
                                                                        VRam[0x1f00 + pixelColor - (pixelColor % 4) + 1],
                                                                        VRam[0x1f00 + pixelColor - (pixelColor % 4) + 2],
                                                                        VRam[0x1f00 + pixelColor - (pixelColor % 4) + 3],
                                                                        pixelColor);
                            }
                            else
                            {
                                if (((8 * currentTileColumn) + (256 - HScroll) + i) < 256)
                                {
                                    ((VideoGdiHiRes)OutputDevice).DrawPixelHiRes(
                                        (8 * currentTileColumn) + (256 - HScroll) + i, CurrentScanLine,
                                        Palette[(0x3f & VRam[0x1f00 + pixelColor])], tilepage, tileDataOffset & 0x0003ff,
                                        7 - i, virtualScanline % 8, true, false,
                                        VRam[0x1f00 + pixelColor - (pixelColor % 4) + 1],
                                        VRam[0x1f00 + pixelColor - (pixelColor % 4) + 2],
                                        VRam[0x1f00 + pixelColor - (pixelColor % 4) + 3], pixelColor);
                                }
                            }
                        }
                        else
                        {
                            if ((pixelColor % 4) != 0)
                            {
                                if (vScrollSide == 0)
                                {
                                    OutputDevice.DrawAbsolutePixel((8 * currentTileColumn) - HScroll + i, CurrentScanLine,
                                                     Palette[(0x3f & VRam[0x1f00 + pixelColor])]);
                                }
                                else
                                {
                                    if (((8 * currentTileColumn) + (256 - HScroll) + i) < 256)
                                    {
                                        OutputDevice.DrawAbsolutePixel((8 * currentTileColumn) + (256 - HScroll) + i, CurrentScanLine,
                                                         Palette[(0x3f & VRam[0x1f00 + pixelColor])]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetTvFormat(TvFormat format, PaletteFormat plFormat)
        {
            switch (format)
            {
                case TvFormat.Ntsc:
                    ScanlinesPerFrame = 261;
                    _framePeriod = 0.01667; //60 FPS
                    //FramePeriod = 2;
                    ScanlineOfVblank = 244;
                    if (plFormat.UseInternalPalette)
                    {
                        switch (plFormat.UseInternalPaletteMode)
                        {
                            case UseInternalPaletteMode.Auto:
                                Palette = Paletter.NtscPalette;
                                break;
                            case UseInternalPaletteMode.Ntsc:
                                Palette = Paletter.NtscPalette;
                                break;
                            case UseInternalPaletteMode.Pal:
                                Palette = Paletter.PalPalette;
                                break;
                        }
                    }
                    else
                    {
                        if (Paletter.LoadPalette(plFormat.ExternalPalettePath) != null)
                        {
                            Palette = Paletter.LoadPalette(plFormat.ExternalPalettePath);
                        }
                        else
                        {
                            Palette = Paletter.NtscPalette;
                            Debug.WriteLine(this,
                                            "Could not find the external palette file, uses the defualt palette for NTSC.",
                                            DebugStatus.Error);
                        }
                    }
                    break;
                case TvFormat.Pal:
                    ScanlinesPerFrame = 311;
                    _framePeriod = 0.020; //50 FPS
                    ScanlineOfVblank = 290;
                    if (plFormat.UseInternalPalette)
                    {
                        switch (plFormat.UseInternalPaletteMode)
                        {
                            case UseInternalPaletteMode.Auto:
                                Palette = Paletter.PalPalette;
                                break;
                            case UseInternalPaletteMode.Ntsc:
                                Palette = Paletter.NtscPalette;
                                break;
                            case UseInternalPaletteMode.Pal:
                                Palette = Paletter.PalPalette;
                                break;
                        }
                    }
                    else
                    {
                        if (Paletter.LoadPalette(plFormat.ExternalPalettePath) != null)
                        {
                            Palette = Paletter.LoadPalette(plFormat.ExternalPalettePath);
                        }
                        else
                        {
                            Palette = Paletter.PalPalette;
                            Debug.WriteLine(this,
                                            "Could not find the external palette file, uses the defualt palette for PAL.",
                                            DebugStatus.Error);
                        }
                    }
                    break;
            }
        }

        #region Registers

        public void Write2000(byte value)
        {
            ExecuteNMIonVBlank = (value & 0x80) == 0x80; //Bit 7
            SpriteSize = (value & 0x20) == 0x20; //Bit 5
            PatternTableAddressBackground = ((value & 0x10) == 0x10) ? 0x1000 : 0; //Bit 4
            PatternTableAddress8X8Sprites = ((value & 0x8) == 0x8) ? 0x1000 : 0; //Bit 3
            VRamAddressIncrement = ((value & 0x04) != 0) ? 32 : 1; //Bit 2
            ReloadBits2000 = (byte)(value & 0x3); //Bit 0 - 1
            VRamTemp = (ushort)((VRamTemp & 0xF3FF) | ((value & 0x3) << 10));
        }

        public void Write2001(byte value)
        {
            ColorEmphasis = (ushort)((value & 0xE0) << 1); //Bit 5 - 7
            SpriteVisibility = (value & 0x10) != 0; //Bit 4
            BackgroundVisibility = (value & 0x8) != 0; //Bit 3
            SpriteClipping = (value & 0x04) != 0; //Bit 2
            BackgroundClipping = (value & 0x02) != 0; //Bit 1
            MonochromeMode = (value & 0x01) != 0; //Bit 0
        }

        public byte Read2002()
        {
            byte returnedValue = 0;
            // VBlank
            if (VBlank)
                returnedValue = (byte)(returnedValue | 0x80);
            //Sprite 0 Hit
            if (Sprite0Hit & (BackgroundVisibility & SpriteVisibility))
                returnedValue = (byte)(returnedValue | 0x40);
            //More than 8 sprites in 1 scanline
            if ((SpriteCrossed > 8) & (BackgroundVisibility & SpriteVisibility))
                returnedValue = (byte)(returnedValue | 0x20);
            //If it should ignore any write into 2007
            if ((CurrentScanLine >= 240) | !(BackgroundVisibility & SpriteVisibility))
                returnedValue = (byte)(returnedValue | 0x10);
            VBlank = false;
            PpuToggle = true;
            return returnedValue;
        }

        public void Write2003(byte value)
        {
            SpriteRamAddress = value;
        }

        public void Write2004(byte value)
        {
            if (CurrentScanLine >= 240)
                SprRam[SpriteRamAddress] = value;
            SpriteRamAddress++;
        }

        public byte Read2004()
        {
            return CurrentScanLine >= 240 ? SprRam[SpriteRamAddress] : (byte)0;
        }

        public void Write2005(byte value)
        {
            if (PpuToggle)
            {
                HScroll = value;
                VRamTemp = (ushort)(VRamTemp | ((value & 0xF8) >> 3));
            }
            else
            {
                VRamTemp = (ushort)(VRamTemp | ((value & 0xF8) << 2));
                VRamTemp = (ushort)(VRamTemp | ((value & 0x3) << 12));
                if (CurrentScanLine >= 240) //All ...
                {
                    VScroll = value;
                    if (VScroll > 239)
                        VScroll = 0;
                }
                else
                {
                    if (FixScroll) //Karnov, Mapper 225/255...
                    {
                        VScroll = value;
                        if (VScroll > 239)
                            VScroll = 0;
                    }
                    else if (FixScroll3) //SMB 3
                        VScroll = 238;
                }
            }
            PpuToggle = !PpuToggle;
        }

        public void Write2006(byte value)
        {
            if (PpuToggle)
            {
                VRamTemp = (ushort)((VRamTemp & 0x00FF) | ((value & 0x3F) << 8));
            }
            else
            {
                VRamTemp = (ushort)((VRamTemp & 0x7F00) | value);
                VRamAddress = VRamTemp;
            }
            //Update the reload bits
            TileY = ((VRamTemp & 0x7000) >> 12);
            HScroll = (byte)(((value & 0x1F) << 3));
            if (CurrentScanLine < 240)
            {
                if (FixScroll2) //special for ZELDAAAAAAAAAAAAAA !!!! why ?
                {
                    if (VRamTemp <= 0x2400) //Zelda needs this check
                    {
                        VScroll = ((VRamTemp & 0x03E0) >> 5);
                        VScroll = ((VScroll * 8) - CurrentScanLine);
                        VScroll += TileY + 1;
                    }
                }
                else //All .....
                {
                    //All games will use this
                    VScroll = ((VRamTemp & 0x03E0) >> 5);
                    VScroll = ((VScroll * 8) - CurrentScanLine);
                    VScroll += (TileY + 1);
                }
            }
            else
            {
                VScroll = 0; // ??!! don't remove this otherwise some games 
                //will crach like Power Rangers 2.
            }
            if (!FixScroll2)
                ReloadBits2000 = (byte)((VRamTemp & 0x0C00) >> 10);
            else if (CurrentScanLine >= 240)
                ReloadBits2000 = (byte)((VRamTemp & 0x0C00) >> 10);
            PpuToggle = !PpuToggle;
        }

        public void Write2007(byte value)
        {
            int add = VRamAddress;
            if (add >= 0x4000)
                add -= 0x4000;
            else if (add >= 0x3F20 & add < 0x4000)
                add -= 0x20;
            if (add < 0x2000)
            {
                _engine.Memory.Map.WriteChr((ushort)add, value);
            }
            else if ((add >= 0x2000) && (add < 0x3F00))
            {
                if (add >= 0x3000)
                    add -= 0x1000;
                var vr = (add & 0x2C00);
                if (!IsMapperMirroring)
                {
                    if (_cartridge.Mirroring == Mirroring.Horizontal)
                    {
                        switch (vr)
                        {
                            case 0x2000:
                                VRam[add - 0x2000] = value;
                                break;
                            case 0x2400:
                                VRam[(add - 0x400) - 0x2000] = value;
                                break;
                            case 0x2800:
                                VRam[add - 0x400 - 0x2000] = value;
                                VRam[add - 0x2000] = value;
                                break;
                            case 0x2C00:
                                VRam[(add - 0x800) - 0x2000] = value;
                                VRam[add - 0x2000] = value;
                                break;
                        }
                    }
                    else if (_cartridge.Mirroring == Mirroring.Vertical)
                    {
                        switch (vr)
                        {
                            case 0x2000:
                                VRam[add - 0x2000] = value;
                                break;
                            case 0x2400:
                                VRam[add - 0x2000] = value;
                                break;
                            case 0x2800:
                                VRam[add - 0x800 - 0x2000] = value;
                                VRam[add - 0x2000] = value;
                                break;
                            case 0x2C00:
                                VRam[(add - 0x800) - 0x2000] = value;
                                VRam[add - 0x2000] = value;
                                break;
                        }
                    }
                    else if (_cartridge.Mirroring == Mirroring.OneScreen)
                    {
                        switch (_cartridge.MirroringBase)
                        {
                            case 0x2000:
                                switch (vr)
                                {
                                    case 0x2000:
                                        VRam[add - 0x2000] = value;
                                        break;
                                    case 0x2400:
                                        VRam[add - 0x400 - 0x2000] = value;
                                        break;
                                    case 0x2800:
                                        VRam[add - 0x800 - 0x2000] = value;
                                        break;
                                    case 0x2C00:
                                        VRam[add - 0xC00 - 0x2000] = value;
                                        break;
                                }
                                break;
                            case 0x2400:
                                switch (vr)
                                {
                                    case 0x2000:
                                        VRam[add + 0x400 - 0x2000] = value;
                                        break;
                                    case 0x2400:
                                        VRam[add - 0x2000] = value;
                                        break;
                                    case 0x2800:
                                        VRam[add - 0x400 - 0x2000] = value;
                                        break;
                                    case 0x2C00:
                                        VRam[add - 0x800 - 0x2000] = value;
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        VRam[add - 0x2000] = value;
                    }
                }
                else
                {
                    VRam[add - 0x2000] = value;
                }
            }
            else if ((add >= 0x3F00) && (add < 0x3F20))
            {
                VRam[add - 0x2000] = value;
                if ((add & 0x7) == 0)
                {
                    VRam[(add - 0x2000) ^ 0x10] = (byte)(value & 0x3F);
                }
            }
            VRamAddress += (ushort)VRamAddressIncrement;
        }

        public byte Read2007()
        {
            byte returnedValue;
            int add = VRamAddress;
            if (add >= 0x4000)
                add -= 0x4000;
            else if (add >= 0x3F20 & add < 0x4000)
                add -= 0x20;
            if (add < 0x3f00)
            {
                returnedValue = VRamReadBuffer;
                if (add >= 0x2000)
                {
                    if (add >= 0x3000)
                        add -= 0x1000;
                    VRamReadBuffer = VRam[add - 0x2000];
                }
                else
                {
                    VRamReadBuffer = _engine.Memory.Map.ReadChr((ushort)(add));
                }
            }
            else
            {
                returnedValue = VRam[add - 0x2000];
            }
            VRamAddress += (ushort)VRamAddressIncrement;
            return returnedValue;
        }

        public void Write4014(byte value)
        {
            if (CurrentScanLine < 240)
            {
                return;
            }
            for (var i = 0; i < 0x100; i++)
            {
                SprRam[i] = _engine.Memory.Read((ushort)((value * 0x100) + i));
                _engine.Cpu.CycleCounter += 2;
            }
        }

        #endregion

        //Properties
    }
}