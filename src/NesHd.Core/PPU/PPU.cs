/*
This file is part of My Nes
A Nintendo Entertainment System Emulator.

 Copyright © 2009 - 2010 Ala Hadid (AHD)

My Nes is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

My Nes is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using NesHd.Core.Debugger;
using NesHd.Core.Input;
using NesHd.Core.Memory;
using NesHd.Core.Misc;
using NesHd.Core.Output.Video;
using NesHd.Core.Output.Video.Devices;

namespace NesHd.Core.PPU
{
    public class PPU
    {
        public byte[] SPRRAM;
        public byte[] VRAM;
        public int[] PALETTE = new int[64];
        TIMER _Timer = new TIMER();
        NesEngine _Nes;
        double _currentFrameTime = 0;
        double _lastFrameTime = 0;
        double FramePeriod = 0;
        //I HATE FIXES !!
        public bool FIX_scroll;
        public bool FIX_scroll2;
        public bool FIX_scroll3;
        /*PPU*/
        public int CurrentScanLine = 0;
        public ushort VRAMAddress = 0;
        public bool Sprite0Hit = false;
        public int SpriteCrossed = 0;
        public int ScanlinesPerFrame = 0;
        public int ScanlineOfVBLANK = 0;
        public int _FPS = 0;
        public bool VBLANK = false;
        IGraphicDevice _Video;
        public byte VRAMReadBuffer = 0;
        public bool _NoLimiter = false;
        /*2000*/
        public bool ExecuteNMIonVBlank = false;
        public bool SpriteSize = false;//true=8x16, false=8x8
        public int PatternTableAddressBackground = 0;
        public int PatternTableAddress8x8Sprites = 0;
        public int VRAMAddressIncrement = 1;
        public byte ReloadBits2000 = 0;
        /*2001*/
        public ushort ColorEmphasis = 0;
        public bool SpriteVisibility = false;
        public bool BackgroundVisibility = false;
        public bool SpriteClipping = false;
        public bool BackgroundClipping = false;
        public bool MonochromeMode = false;
        /*2003*/
        public byte SpriteRamAddress = 0;
        /*2005,2006*/
        public bool PPUToggle = true;
        public ushort VRAMTemp = 0;
        /*Draw stuff*/
        public byte HScroll = 0;
        public int VScroll = 0;
        public int VBits = 0;
        public int TileY = 0;
        /*Zapper*/
        public int ZapperX = 0;
        public int ZapperY = 0;
        public bool CheckZapperHit = false;
        public int ZapperFrame = 0;
        /*VRAM Control*/
        public bool IsMapperMirroring = false;
        /// <summary>
        /// The picture unit
        /// </summary>
        /// <param name="TV">The tv format</param>
        /// <param name="Nes">The nes engine</param>
        /// <param name="VideoDevice">The output video device</param>
        public PPU(TVFORMAT TV, PaletteFormat PlFormat, NesEngine Nes)
        {
            this._Nes = Nes;
            this.VRAM = new byte[0x2000];
            this.SPRRAM = new byte[0x100];
            this.SetTVFormat(TV, PlFormat);
            Debug.WriteLine(this, "PPU initialized ok.", DebugStatus.Cool);
        }
        /// <summary>
        /// True when NMI is needed
        /// </summary>
        /// <returns>NMI</returns>
        public bool DoScanline()
        {
            //Start a new frame
            if (this.CurrentScanLine == 0)
            {
                this._Video.Begin();
            }
            if (this.CurrentScanLine < 240)
            {
                //Clean up the line from before
                byte B = this.VRAM[0x1F00];
                if (B >= 63)
                    B = 63;
                for (int i = 0; i < 256; i++)
                {
                    this._Video.DrawPixel(i, this.CurrentScanLine, this.PALETTE[B]);
                }
                //first we should reset the sprite crossed for reg $2002
                this.SpriteCrossed = 0;
                //Render the sprites and background
                if (this.SpriteVisibility)
                    this.RenderSprites(0x20);
                if (this.BackgroundVisibility)
                    this.RenderBackground();
                if (this.SpriteVisibility)
                    this.RenderSprites(0);
                //Do clipping
                if (!this.BackgroundClipping)
                    for (int i = 0; i < 8; i++)
                        this._Video.DrawPixel(i, this.CurrentScanLine, 0);
                //(clock each scanline, paused during VBlank)
                if (this.SpriteVisibility | this.BackgroundVisibility)
                    this._Nes.MEMORY.MAP.TickScanlineTimer();
            }
            //It should clock each cpu cycle, but here will clock in each 
            //scanline to make the emu faster.
            //The IRQ counter of the mapper will be decremented by
            //the cycles that done by CPU at scanline.
            this._Nes.MEMORY.MAP.TickCycleTimer();
            //Set the status of the VBlank
            if (this.CurrentScanLine == 240)
            {
                this.VBLANK = true;
                if (this.CheckZapperHit)
                    this.ZapperFrame++;
            }
            //Render the sound
            if (this.CurrentScanLine >= 240 & this.CurrentScanLine < 242)
                if (this._Nes.SoundEnabled)
                    this._Nes.APU.RenderFrame();
            //Draw the frame into the screen
            if (this.CurrentScanLine == this.ScanlineOfVBLANK + 1)
                this._Video.RenderFrame();
            //Advance the scanline
            this.CurrentScanLine++;
            //End of the frame
            if (this.CurrentScanLine == this.ScanlinesPerFrame)
            {
                //Handle the speed
                if (!this._NoLimiter)
                {
                    double currentTime = this._Timer.GetCurrentTime();
                    this._currentFrameTime = currentTime - this._lastFrameTime;
                    if ((this._currentFrameTime < this.FramePeriod))
                    {
                        while (true)
                        {
                            if ((this._Timer.GetCurrentTime() - this._lastFrameTime) >= this.FramePeriod)
                            {
                                break;
                            }
                        }
                    }
                }
                this._lastFrameTime = this._Timer.GetCurrentTime();
                //Do routine
                this._FPS++;
                this.CurrentScanLine = 0;
                this.Sprite0Hit = false;
            }
            //Return if the cpu should hit the NMI
            return ((this.CurrentScanLine == this.ScanlineOfVBLANK) & this.ExecuteNMIonVBlank);
        }
        void RenderSprites(int Behind)
        {
            int _SpriteSize = this.SpriteSize ? 16 : 8;
            int _LineToDraw = 0;
            //1: loop through SPR-RAM
            for (int i = 252; i >= 0; i = i - 4)
            {
                int PixelColor = 0;
                byte YCoordinate = (byte)(this.SPRRAM[i] + 1);
                //2: if the sprite falls on the current scanline, draw it
                if (((this.SPRRAM[i + 2] & 0x20) == Behind) &&
                    (YCoordinate <= this.CurrentScanLine) &&
                    ((YCoordinate + _SpriteSize) > this.CurrentScanLine))
                {
                    this.SpriteCrossed++;
                    //3: Draw the sprites differently if they are 8x8 or 8x16
                    if (!this.SpriteSize)//8x8
                    {
                        //4: calculate which line of the sprite is currently being drawn
                        //Line to draw is: currentScanline - Y coord + 1
                        if ((this.SPRRAM[i + 2] & 0x80) != 0x80)
                            _LineToDraw = this.CurrentScanLine - YCoordinate;
                        else
                            _LineToDraw = YCoordinate + 7 - this.CurrentScanLine;
                        //5: calculate the offset to the sprite's data in
                        //our chr rom data 
                        int SpriteOffset = this.PatternTableAddress8x8Sprites + this.SPRRAM[i + 1] * 16;
                        //6: extract our tile data
                        byte TileData1 = this._Nes.MEMORY.MAP.ReadCHR((ushort)(SpriteOffset + _LineToDraw));
                        byte TileData2 = this._Nes.MEMORY.MAP.ReadCHR((ushort)(SpriteOffset + _LineToDraw + 8));
                        //7: get the palette attribute data
                        byte PaletteUpperBits = (byte)((this.SPRRAM[i + 2] & 0x3) << 2);
                        //8: render the line inside the tile into the screen direcly
                        for (int j = 0; j < 8; j++)
                        {
                            if ((this.SPRRAM[i + 2] & 0x40) == 0x40)
                            {
                                PixelColor = PaletteUpperBits + (((TileData2 & (1 << (j))) >> (j)) << 1) +
                                    ((TileData1 & (1 << (j))) >> (j));
                            }
                            else
                            {
                                PixelColor = PaletteUpperBits + (((TileData2 & (1 << (7 - j))) >> (7 - j)) << 1) +
                                    ((TileData1 & (1 << (7 - j))) >> (7 - j));
                            }
                            //Hi Res stages
                            if (this._Video.Name == "Windows GDI Hi Res")
                            {
                                //store which chr page is being used
                                uint tilepage = this._Nes.MEMORY.MAP.ReadCHRPageNo((ushort)(SpriteOffset));
                                if ((this.SPRRAM[i + 3] + j) < 256)
                                {
                                    ((VideoGdiHiRes)this._Video).DrawPixelHiRes((this.SPRRAM[i + 3]) + j, this.CurrentScanLine,
                                        this.PALETTE[(0x3f & this.VRAM[0x1F10 + PixelColor])], tilepage, SpriteOffset & 0x0003ff, ((this.SPRRAM[i + 2] & 0x40) == 0x40) ? j : 7 - j, _LineToDraw, (this.SPRRAM[i + 2] & 0x40) != 0x40, (this.SPRRAM[i + 2] & 0x80) == 0x80, this.VRAM[0x1F10 + PixelColor - (PixelColor % 4) + 1], this.VRAM[0x1F10 + PixelColor - (PixelColor % 4) + 2], this.VRAM[0x1F10 + PixelColor - (PixelColor % 4) + 3], PixelColor);
                                    if (((PixelColor % 4) != 0) && (i == 0))
                                    {
                                        this.Sprite0Hit = true;
                                    }
                                }


                            }
                            else
                            {
                                if ((PixelColor % 4) != 0)
                                {
                                    if ((this.SPRRAM[i + 3] + j) < 256)
                                    {
                                        this._Video.DrawPixel((this.SPRRAM[i + 3]) + j, this.CurrentScanLine,
                                            this.PALETTE[(0x3f & this.VRAM[0x1F10 + PixelColor])]);
                                        if (i == 0)
                                        {
                                            this.Sprite0Hit = true;
                                        }
                                    }
                                }
                            }
                        }
                        //Check for the zapper
                        if (this.CheckZapperHit & this.ZapperFrame > 2)
                        {
                            if ((this.ZapperX >= (this.SPRRAM[i + 3]) & this.ZapperX <= (this.SPRRAM[i + 3] + 8))
                                & (this.ZapperY >= YCoordinate & this.ZapperY <= YCoordinate + 8))
                            {
                                this._Nes.MEMORY.ZAPPER.SetDetect(0);
                                this.CheckZapperHit = false;
                            }
                        }
                    }
                    else//8x16
                    {
                        //4: get the sprite id
                        byte SpriteId = this.SPRRAM[i + 1];
                        if ((this.SPRRAM[i + 2] & 0x80) != 0x80)
                            _LineToDraw = this.CurrentScanLine - YCoordinate;
                        else
                            _LineToDraw = YCoordinate + 15 - this.CurrentScanLine;
                        //5: We draw the sprite like two halves, so getting past the 
                        //first 8 puts us into the next tile
                        //If the ID is even, the tile is in 0x0000, odd 0x1000
                        int SpriteOffset = 0;
                        if (_LineToDraw < 8)
                        {
                            //Draw the top tile
                            if ((SpriteId % 2) == 0)
                                SpriteOffset = 0x0000 + (SpriteId) * 16;
                            else
                                SpriteOffset = 0x1000 + (SpriteId - 1) * 16;
                        }
                        else
                        {
                            //Draw the bottom tile
                            _LineToDraw -= 8;
                            if ((SpriteId % 2) == 0)
                                SpriteOffset = 0x0000 + (SpriteId + 1) * 16;
                            else
                                SpriteOffset = 0x1000 + (SpriteId) * 16;
                        }
                        //6: extract our tile data
                        byte TileData1 = this._Nes.MEMORY.MAP.ReadCHR((ushort)(SpriteOffset + _LineToDraw));
                        byte TileData2 = this._Nes.MEMORY.MAP.ReadCHR((ushort)(SpriteOffset + _LineToDraw + 8));
                        //7: get the palette attribute data
                        byte PaletteUpperBits = (byte)((this.SPRRAM[i + 2] & 0x3) << 2);
                        //8, render the line inside the tile to the screen
                        for (int j = 0; j < 8; j++)
                        {
                            if ((this.SPRRAM[i + 2] & 0x40) == 0x40)
                            {
                                PixelColor = PaletteUpperBits + (((TileData2 & (1 << (j))) >> (j)) << 1) +
                                    ((TileData1 & (1 << (j))) >> (j));
                            }
                            else
                            {
                                PixelColor = PaletteUpperBits + (((TileData2 & (1 << (7 - j))) >> (7 - j)) << 1) +
                                    ((TileData1 & (1 << (7 - j))) >> (7 - j));
                            }

                            if (this._Video.Name == "Windows GDI Hi Res")
                            {
                                //store which chr page is being used
                                uint tilepage = this._Nes.MEMORY.MAP.ReadCHRPageNo((ushort)(SpriteOffset));
                                if ((this.SPRRAM[i + 3] + j) < 256)
                                {
                                    ((VideoGdiHiRes)this._Video).DrawPixelHiRes((this.SPRRAM[i + 3]) + j, this.CurrentScanLine,
                                        this.PALETTE[(0x3f & this.VRAM[0x1F10 + PixelColor])], tilepage, SpriteOffset & 0x0003ff, ((this.SPRRAM[i + 2] & 0x40) == 0x40) ? j : 7 - j, _LineToDraw, (this.SPRRAM[i + 2] & 0x40) == 0x40, (this.SPRRAM[i + 2] & 0x80) == 0x80, this.VRAM[0x1F10 + PixelColor - (PixelColor % 4) + 1], this.VRAM[0x1F10 + PixelColor - (PixelColor % 4) + 2], this.VRAM[0x1F10 + PixelColor - (PixelColor % 4) + 3], PixelColor);
                                    if (((PixelColor % 4) != 0) && (i == 0))
                                    {
                                        this.Sprite0Hit = true;
                                    }
                                }
                            }
                            else
                            {
                                if ((PixelColor % 4) != 0)
                                {
                                    if ((this.SPRRAM[i + 3] + j) < 256)
                                    {
                                        this._Video.DrawPixel((this.SPRRAM[i + 3]) + j,
                                            this.CurrentScanLine,
                                            this.PALETTE[(0x3f & this.VRAM[0x1F10 + PixelColor])]);
                                        if (i == 0)
                                        {
                                            this.Sprite0Hit = true;
                                        }
                                    }
                                }
                            }
                        }
                        if (this.CheckZapperHit & this.ZapperFrame > 2)
                        {
                            if ((this.ZapperX >= (this.SPRRAM[i + 3]) & this.ZapperX <= (this.SPRRAM[i + 3] + 8))
                                & (this.ZapperY >= YCoordinate & this.ZapperY <= YCoordinate + 16)
                                & this.PALETTE[(0x3f & this.VRAM[0x1F10 + PixelColor])] >= 0xFF)
                            {
                                this._Nes.MEMORY.ZAPPER.SetDetect(0);
                                this.CheckZapperHit = false;
                            }
                        }
                    }
                }
                else
                {
                    this.SpriteRamAddress = 0;
                }
            }
        }
        void RenderBackground()
        {
            int nameTableAddress = 0;
            if (this.ReloadBits2000 == 0)
                nameTableAddress = 0x2000;
            else if (this.ReloadBits2000 == 1)
                nameTableAddress = 0x2400;
            else if (this.ReloadBits2000 == 2)
                nameTableAddress = 0x2800;
            else if (this.ReloadBits2000 == 3)
                nameTableAddress = 0x2C00;
            for (int vScrollSide = 0; vScrollSide < 2; vScrollSide++)
            {
                int virtualScanline = this.CurrentScanLine + this.VScroll;
                if (virtualScanline < 0)
                    virtualScanline = 0;
                int nameTableBase = nameTableAddress;
                int startColumn = 0;
                int endColumn = 0;
                if (vScrollSide == 0)
                {
                    if (virtualScanline >= 240)
                    {
                        if (nameTableAddress == 0x2000)
                            nameTableBase = 0x2800;
                        else if (nameTableAddress == 0x2400)
                            nameTableBase = 0x2C00;
                        else if (nameTableAddress == 0x2800)
                            nameTableBase = 0x2000;
                        else if (nameTableAddress == 0x2C00)
                            nameTableBase = 0x2400;
                        virtualScanline -= 240;
                    }
                    startColumn = this.HScroll / 8;
                    endColumn = 32;
                }
                else
                {
                    if (virtualScanline >= 240)
                    {
                        if (nameTableAddress == 0x2000)
                            nameTableBase = 0x2C00;
                        else if (nameTableAddress == 0x2400)
                            nameTableBase = 0x2800;
                        else if (nameTableAddress == 0x2800)
                            nameTableBase = 0x2400;
                        else if (nameTableAddress == 0x2C00)
                            nameTableBase = 0x2000;
                        virtualScanline -= 240;
                    }
                    else
                    {
                        if (nameTableAddress == 0x2000)
                            nameTableBase = 0x2400;
                        else if (nameTableAddress == 0x2400)
                            nameTableBase = 0x2000;
                        else if (nameTableAddress == 0x2800)
                            nameTableBase = 0x2C00;
                        else if (nameTableAddress == 0x2C00)
                            nameTableBase = 0x2800;
                    }
                    startColumn = 0;
                    endColumn = (this.HScroll / 8) + 1;
                }
                //Next Try: Forcing two page only: 0x2000 and 0x2400				
                if (this._Nes.MEMORY.MAP.Cartridge.Mirroring == MIRRORING.HORIZONTAL)
                {
                    if (nameTableBase == 0x2400)
                        nameTableBase = 0x2000;
                    else if (nameTableBase == 0x2800)
                        nameTableBase = 0x2400;
                    else if (nameTableBase == 0x2C00)
                        nameTableBase = 0x2400;
                }
                else if (this._Nes.MEMORY.MAP.Cartridge.Mirroring == MIRRORING.VERTICAL)
                {
                    if (nameTableBase == 0x2800)
                        nameTableBase = 0x2000;
                    else if (nameTableBase == 0x2C00)
                        nameTableBase = 0x2400;
                }
                else if (this._Nes.MEMORY.MAP.Cartridge.Mirroring == MIRRORING.ONE_SCREEN)
                {
                    nameTableBase = (int)this._Nes.MEMORY.MAP.Cartridge.MirroringBase;
                }
                for (int currentTileColumn = startColumn; currentTileColumn < endColumn;
                    currentTileColumn++)
                {
                    //Starting tile row is currentScanline / 8
                    //The offset in the tile is currentScanline % 8

                    //Step #1, get the tile number
                    int tileNumber = this.VRAM[((nameTableBase - 0x2000) + ((virtualScanline / 8) * 32) + currentTileColumn)];

                    //Step #2, get the offset for the tile in the tile data
                    int tileDataOffset = this.PatternTableAddressBackground + (tileNumber * 16);

                    //Step #3, get the tile data from chr rom
                    int tiledata1 = this._Nes.MEMORY.MAP.ReadCHR((ushort)(tileDataOffset + (virtualScanline % 8)));
                    int tiledata2 = this._Nes.MEMORY.MAP.ReadCHR((ushort)(tileDataOffset + (virtualScanline % 8) + 8));
                    
                    //Step #4, get the attribute byte for the block of tiles we're in
                    //this will put us in the correct section in the palette table
                    int paletteHighBits = this.VRAM[((nameTableBase - 0x2000 +
                        0x3c0 + (((virtualScanline / 8) / 4) * 8) + (currentTileColumn / 4)))];
                    paletteHighBits = (byte)(paletteHighBits >> ((4 * (((virtualScanline / 8) % 4) / 2)) +
                        (2 * ((currentTileColumn % 4) / 2))));
                    paletteHighBits = (byte)((paletteHighBits & 0x3) << 2);

                    //Step #5, render the line inside the tile to the offscreen buffer
                    int startTilePixel = 0;
                    int endTilePixel = 0;
                    if (vScrollSide == 0)
                    {
                        if (currentTileColumn == startColumn)
                        {
                            startTilePixel = this.HScroll % 8;
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
                            endTilePixel = this.HScroll % 8;
                        }
                        else
                        {
                            startTilePixel = 0;
                            endTilePixel = 8;
                        }
                    }

                    for (int i = startTilePixel; i < endTilePixel; i++)
                    {
                        int pixelColor = paletteHighBits + (((tiledata2 & (1 << (7 - i))) >> (7 - i)) << 1) +
                            ((tiledata1 & (1 << (7 - i))) >> (7 - i));

                        //Hi Res stages
                        if (this._Video.Name == "Windows GDI Hi Res")
                        {
                            //store which chr page is being used
                            uint tilepage = this._Nes.MEMORY.MAP.ReadCHRPageNo((ushort)(tileDataOffset));

                            if (vScrollSide == 0)
                            {
                                ((VideoGdiHiRes)this._Video).DrawPixelHiRes((8 * currentTileColumn) - this.HScroll + i, this.CurrentScanLine, this.PALETTE[(0x3f & this.VRAM[0x1f00 + pixelColor])], tilepage, tileDataOffset & 0x0003ff, 7 - i, virtualScanline % 8, true, false, this.VRAM[0x1f00 + pixelColor - (pixelColor % 4) + 1], this.VRAM[0x1f00 + pixelColor - (pixelColor % 4) + 2], this.VRAM[0x1f00 + pixelColor - (pixelColor % 4) + 3], pixelColor);
                            }
                            else
                            {
                                if (((8 * currentTileColumn) + (256 - this.HScroll) + i) < 256)
                                {
                                    ((VideoGdiHiRes)this._Video).DrawPixelHiRes((8 * currentTileColumn) + (256 - this.HScroll) + i, this.CurrentScanLine, this.PALETTE[(0x3f & this.VRAM[0x1f00 + pixelColor])], tilepage, tileDataOffset & 0x0003ff, 7 - i, virtualScanline % 8, true, false, this.VRAM[0x1f00 + pixelColor - (pixelColor % 4) + 1], this.VRAM[0x1f00 + pixelColor - (pixelColor % 4) + 2], this.VRAM[0x1f00 + pixelColor - (pixelColor % 4) + 3], pixelColor);
                                }
                            }

                        }
                        else {
                            if ((pixelColor % 4) != 0)
                            {
                                if (vScrollSide == 0)
                                {
                                    this._Video.DrawPixel((8 * currentTileColumn) - this.HScroll + i, this.CurrentScanLine, this.PALETTE[(0x3f & this.VRAM[0x1f00 + pixelColor])]);
                                }
                                else
                                {
                                    if (((8 * currentTileColumn) + (256 - this.HScroll) + i) < 256)
                                    {
                                        this._Video.DrawPixel((8 * currentTileColumn) + (256 - this.HScroll) + i, this.CurrentScanLine, this.PALETTE[(0x3f & this.VRAM[0x1f00 + pixelColor])]);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
        public void SetTVFormat(TVFORMAT FORMAT, PaletteFormat PlFormat)
        {
            switch (FORMAT)
            {
                case TVFORMAT.NTSC:
                    this.ScanlinesPerFrame = 261;
                    this.FramePeriod = 0.01667;//60 FPS
                    //FramePeriod = 2;
                    this.ScanlineOfVBLANK = 244;
                    if (PlFormat.UseInternalPalette)
                    {
                        switch (PlFormat.UseInternalPaletteMode)
                        {
                            case UseInternalPaletteMode.Auto:
                                this.PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.NTSC:
                                this.PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.PAL:
                                this.PALETTE = Paletter.PALPalette;
                                break;
                        }

                    }
                    else
                    {
                        if (Paletter.LoadPalette(PlFormat.ExternalPalettePath) != null)
                        {
                            this.PALETTE = Paletter.LoadPalette(PlFormat.ExternalPalettePath);
                        }
                        else
                        {
                            this.PALETTE = Paletter.NTSCPalette;
                            Debug.WriteLine(this, "Could not find the external palette file, uses the defualt palette for NTSC.", DebugStatus.Error);
                        }
                    }
                    break;
                case TVFORMAT.PAL:
                    this.ScanlinesPerFrame = 311;
                    this.FramePeriod = 0.020;//50 FPS
                    this.ScanlineOfVBLANK = 290;
                    if (PlFormat.UseInternalPalette)
                    {
                        switch (PlFormat.UseInternalPaletteMode)
                        {
                            case UseInternalPaletteMode.Auto:
                                this.PALETTE = Paletter.PALPalette;
                                break;
                            case UseInternalPaletteMode.NTSC:
                                this.PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.PAL:
                                this.PALETTE = Paletter.PALPalette;
                                break;
                        }

                    }
                    else
                    {
                        if (Paletter.LoadPalette(PlFormat.ExternalPalettePath) != null)
                        {
                            this.PALETTE = Paletter.LoadPalette(PlFormat.ExternalPalettePath);
                        }
                        else
                        {
                            this.PALETTE = Paletter.PALPalette;
                            Debug.WriteLine(this, "Could not find the external palette file, uses the defualt palette for PAL.", DebugStatus.Error);
                        }
                    }
                    break;
            }

        }
        #region Registers
        public void Write2000(byte Value)
        {
            this.ExecuteNMIonVBlank = (Value & 0x80) == 0x80;//Bit 7
            this.SpriteSize = (Value & 0x20) == 0x20;//Bit 5
            this.PatternTableAddressBackground = ((Value & 0x10) == 0x10) ? 0x1000 : 0;//Bit 4
            this.PatternTableAddress8x8Sprites = ((Value & 0x8) == 0x8) ? 0x1000 : 0;//Bit 3
            this.VRAMAddressIncrement = ((Value & 0x04) != 0) ? 32 : 1;//Bit 2
            this.ReloadBits2000 = (byte)(Value & 0x3);//Bit 0 - 1
            this.VRAMTemp = (ushort)((this.VRAMTemp & 0xF3FF) | ((Value & 0x3) << 10));
        }
        public void Write2001(byte Value)
        {
            this.ColorEmphasis = (ushort)((Value & 0xE0) << 1);//Bit 5 - 7
            this.SpriteVisibility = (Value & 0x10) != 0;//Bit 4
            this.BackgroundVisibility = (Value & 0x8) != 0;//Bit 3
            this.SpriteClipping = (Value & 0x04) != 0;//Bit 2
            this.BackgroundClipping = (Value & 0x02) != 0;//Bit 1
            this.MonochromeMode = (Value & 0x01) != 0;//Bit 0
        }
        public byte Read2002()
        {
            byte returnedValue = 0;
            // VBlank
            if (this.VBLANK)
                returnedValue = (byte)(returnedValue | 0x80);
            //Sprite 0 Hit
            if (this.Sprite0Hit & (this.BackgroundVisibility & this.SpriteVisibility))
                returnedValue = (byte)(returnedValue | 0x40);
            //More than 8 sprites in 1 scanline
            if ((this.SpriteCrossed > 8) & (this.BackgroundVisibility & this.SpriteVisibility))
                returnedValue = (byte)(returnedValue | 0x20);
            //If it should ignore any write into 2007
            if ((this.CurrentScanLine >= 240) | !(this.BackgroundVisibility & this.SpriteVisibility))
                returnedValue = (byte)(returnedValue | 0x10);
            this.VBLANK = false;
            this.PPUToggle = true;
            return returnedValue;
        }
        public void Write2003(byte Value)
        {
            this.SpriteRamAddress = Value;
        }
        public void Write2004(byte Value)
        {
            if (this.CurrentScanLine >= 240)
                this.SPRRAM[this.SpriteRamAddress] = Value;
            this.SpriteRamAddress++;
        }
        public byte Read2004()
        {
            if (this.CurrentScanLine >= 240)
                return this.SPRRAM[this.SpriteRamAddress];
            else
                return 0;
        }
        public void Write2005(byte Value)
        {
            if (this.PPUToggle)
            {
                this.HScroll = Value;
                this.VRAMTemp = (ushort)(this.VRAMTemp | ((Value & 0xF8) >> 3));
            }
            else
            {
                this.VRAMTemp = (ushort)(this.VRAMTemp | ((Value & 0xF8) << 2));
                this.VRAMTemp = (ushort)(this.VRAMTemp | ((Value & 0x3) << 12));
                if (this.CurrentScanLine >= 240)//All ...
                {
                    this.VScroll = Value;
                    if (this.VScroll > 239)
                        this.VScroll = 0;
                }
                else
                {
                    if (this.FIX_scroll)//Karnov, Mapper 225/255...
                    {
                        this.VScroll = Value;
                        if (this.VScroll > 239)
                            this.VScroll = 0;
                    }
                    else if (this.FIX_scroll3)//SMB 3
                        this.VScroll = 238;
                }
            }
            this.PPUToggle = !this.PPUToggle;
        }
        public void Write2006(byte Value)
        {
            if (this.PPUToggle)
            {
                this.VRAMTemp = (ushort)((this.VRAMTemp & 0x00FF) | ((Value & 0x3F) << 8));
            }
            else
            {
                this.VRAMTemp = (ushort)((this.VRAMTemp & 0x7F00) | Value);
                this.VRAMAddress = this.VRAMTemp;
            }
            //Update the reload bits
            this.TileY = ((this.VRAMTemp & 0x7000) >> 12);
            this.HScroll = (byte)(((Value & 0x1F) << 3));
            if (this.CurrentScanLine < 240)
            {
                if (this.FIX_scroll2)//special for ZELDAAAAAAAAAAAAAA !!!! why ?
                {
                    if (this.VRAMTemp <= 0x2400)//Zelda needs this check
                    {
                        this.VScroll = ((this.VRAMTemp & 0x03E0) >> 5);
                        this.VScroll = ((this.VScroll * 8) - this.CurrentScanLine);
                        this.VScroll += this.TileY + 1;
                    }
                }
                else//All .....
                {
                    //All games will use this
                    this.VScroll = ((this.VRAMTemp & 0x03E0) >> 5);
                    this.VScroll = ((this.VScroll * 8) - this.CurrentScanLine);
                    this.VScroll += (this.TileY + 1);
                }
            }
            else
            {
                this.VScroll = 0;// ??!! don't remove this otherwise some games 
                //will crach like Power Rangers 2.
            }
            if (!this.FIX_scroll2)
                this.ReloadBits2000 = (byte)((this.VRAMTemp & 0x0C00) >> 10);
            else if (this.CurrentScanLine >= 240)
                this.ReloadBits2000 = (byte)((this.VRAMTemp & 0x0C00) >> 10);
            this.PPUToggle = !this.PPUToggle;
        }
        public void Write2007(byte Value)
        {
            int ADD = this.VRAMAddress;
            if (ADD >= 0x4000)
                ADD -= 0x4000;
            else if (ADD >= 0x3F20 & ADD < 0x4000)
                ADD -= 0x20;
            if (ADD < 0x2000)
            {
                this._Nes.MEMORY.MAP.WriteCHR((ushort)ADD, Value);
            }
            else if ((ADD >= 0x2000) && (ADD < 0x3F00))
            {
                if (ADD >= 0x3000)
                    ADD -= 0x1000;
                int vr = (ADD & 0x2C00);
                if (!this.IsMapperMirroring)
                {
                    if (this._Nes.MEMORY.MAP.Cartridge.Mirroring == MIRRORING.HORIZONTAL)
                    {
                        if (vr == 0x2000)
                            this.VRAM[ADD - 0x2000] = Value;
                        else if (vr == 0x2400)
                            this.VRAM[(ADD - 0x400) - 0x2000] = Value;
                        else if (vr == 0x2800)
                        {
                            this.VRAM[ADD - 0x400 - 0x2000] = Value;
                            this.VRAM[ADD - 0x2000] = Value;
                        }
                        else if (vr == 0x2C00)
                        {
                            this.VRAM[(ADD - 0x800) - 0x2000] = Value;
                            this.VRAM[ADD - 0x2000] = Value;
                        }
                    }
                    else if (this._Nes.MEMORY.MAP.Cartridge.Mirroring == MIRRORING.VERTICAL)
                    {
                        if (vr == 0x2000)
                            this.VRAM[ADD - 0x2000] = Value;
                        else if (vr == 0x2400)
                            this.VRAM[ADD - 0x2000] = Value;
                        else if (vr == 0x2800)
                        {
                            this.VRAM[ADD - 0x800 - 0x2000] = Value;
                            this.VRAM[ADD - 0x2000] = Value;
                        }
                        else if (vr == 0x2C00)
                        {
                            this.VRAM[(ADD - 0x800) - 0x2000] = Value;
                            this.VRAM[ADD - 0x2000] = Value;
                        }
                    }
                    else if (this._Nes.MEMORY.MAP.Cartridge.Mirroring == MIRRORING.ONE_SCREEN)
                    {
                        if (this._Nes.MEMORY.MAP.Cartridge.MirroringBase == 0x2000)
                        {
                            if (vr == 0x2000)
                                this.VRAM[ADD - 0x2000] = Value;
                            else if (vr == 0x2400)
                                this.VRAM[ADD - 0x400 - 0x2000] = Value;
                            else if (vr == 0x2800)
                                this.VRAM[ADD - 0x800 - 0x2000] = Value;
                            else if (vr == 0x2C00)
                                this.VRAM[ADD - 0xC00 - 0x2000] = Value;
                        }
                        else if (this._Nes.MEMORY.MAP.Cartridge.MirroringBase == 0x2400)
                        {
                            if (vr == 0x2000)
                                this.VRAM[ADD + 0x400 - 0x2000] = Value;
                            else if (vr == 0x2400)
                                this.VRAM[ADD - 0x2000] = Value;
                            else if (vr == 0x2800)
                                this.VRAM[ADD - 0x400 - 0x2000] = Value;
                            else if (vr == 0x2C00)
                                this.VRAM[ADD - 0x800 - 0x2000] = Value;
                        }
                    }
                    else
                    {
                        this.VRAM[ADD - 0x2000] = Value;
                    }
                }
                else
                { this.VRAM[ADD - 0x2000] = Value; }
            }
            else if ((ADD >= 0x3F00) && (ADD < 0x3F20))
            {
                this.VRAM[ADD - 0x2000] = Value;
                if ((ADD & 0x7) == 0)
                {
                    this.VRAM[(ADD - 0x2000) ^ 0x10] = (byte)(Value & 0x3F);
                }
            }
            this.VRAMAddress += (ushort)this.VRAMAddressIncrement;
        }
        public byte Read2007()
        {
            byte returnedValue = 0;
            int ADD = this.VRAMAddress;
            if (ADD >= 0x4000)
                ADD -= 0x4000;
            else if (ADD >= 0x3F20 & ADD < 0x4000)
                ADD -= 0x20;
            if (ADD < 0x3f00)
            {
                returnedValue = this.VRAMReadBuffer;
                if (ADD >= 0x2000)
                {
                    if (ADD >= 0x3000)
                        ADD -= 0x1000;
                    this.VRAMReadBuffer = this.VRAM[ADD - 0x2000];
                }
                else
                {
                    this.VRAMReadBuffer = this._Nes.MEMORY.MAP.ReadCHR((ushort)(ADD));
                }
            }
            else
            {
                returnedValue = this.VRAM[ADD - 0x2000];
            }
            this.VRAMAddress += (ushort)this.VRAMAddressIncrement;
            return returnedValue;
        }
        public void Write4014(byte Value)
        {
            if (this.CurrentScanLine >= 240)
            {
                for (int i = 0; i < 0x100; i++)
                {
                    this.SPRRAM[i] = this._Nes.MEMORY.Read((ushort)((Value * 0x100) + i));
                    this._Nes.CPU.CycleCounter += 2;
                }
            }
        }
        #endregion
        //Properties
        public IGraphicDevice OutputDevice
        { get { return this._Video; } set { this._Video = value; } }
        public int FPS
        { get { return this._FPS; } set { this._FPS = value; } }
        public bool NoLimiter
        { get { return this._NoLimiter; } set { this._NoLimiter = value; } }
    }
}