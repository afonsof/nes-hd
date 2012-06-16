using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NesHd.Core.Debugger;

namespace NesHd.Core.Output.Video.Devices
{
    public unsafe class VideoGdiHiRes : IGraphicDevice
    {
        private readonly int _scanlinesToCut;
        private readonly int _scanlines;
        private readonly Control _surface;

        private Graphics _graphics;
        private int _textApperance = 200;
        private string _textToRender = "";
        private bool _canRender = true;
        private bool _isRendering;

        private readonly Bitmap _bitmap;
        private List<Bitmap> _bitmaps;
        private BitmapData _bmpData;
        
        private bool _hasHiResPack;
        private bool _highResSnap;
        private StreamWriter _log;
        private int* _numPtr;
        private int[][] _packData;
        private string _snapFile;
        private string _snapFormat;
        private bool _takingSnap;
        private List<TileData> _tileDatas;

        private int _screenW;
        private int _screenH;
        private int _screenX;
        private int _screenY;

        public VideoGdiHiRes(TVFORMAT tvFormat, Control surface, string pPath, int chrPages)
        {
            if (surface == null)
                return;
            Debug.WriteLine(this, "Initializeing GDI ...", DebugStatus.None);
            switch (tvFormat)
            {
                case TVFORMAT.NTSC:
                    _scanlinesToCut = 8;
                    _scanlines = 224;
                    _bitmap = new Bitmap(512, 448);
                    break;
                case TVFORMAT.PAL:
                    _scanlinesToCut = 0;
                    _scanlines = 240;
                    _bitmap = new Bitmap(512, 480);
                    break;
            }
            _surface = surface;
            UpdateSize(0, 0, _surface.Width + 1, _surface.Height + 1);


            if (pPath != "")
                ReadHiResPack(
                    Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(pPath),
                    chrPages);

            Debug.WriteLine(this, "Video device " + @"""" + "GDI Hi Res" + @"""" + " Ok !!", DebugStatus.Cool);
        }

        #region IGraphicDevice Members

        public string Name
        {
            get { return "Windows GDI Hi Res"; }
        }

        public string Description
        {
            get { return "Same as GDI with double resolution and uses hi res graphics packs."; }
        }

        public void Begin()
        {
            if (_canRender)
            {
                _isRendering = true;
                _bmpData = _bitmap.LockBits(new Rectangle(0, 0, 512, _scanlines*2),
                                       ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                _numPtr = (int*) _bmpData.Scan0;
                if (_highResSnap)
                {
                    _log = new StreamWriter(_snapFile + "log.txt");
                    _takingSnap = true;
                }
            }
        }

        public void AddScanline(int line, int[] scanlineBuffer)
        {
            if (_canRender & _isRendering)
            {
                //Check if we should cut this line 
                if (line > _scanlinesToCut & line < (_scanlines + _scanlinesToCut))
                {
                    var liner = ((line - _scanlinesToCut)*256);
                    //Set the scanline into the buffer
                    for (var i = 0; i < 256; i++)
                        _numPtr[liner + i] = scanlineBuffer[i];
                }
            }
        }

        public void DrawPixel(int x, int y, int color)
        {
            if (_canRender & _isRendering)
            {
                //Check if we should cut this line 
                if (y >= _scanlinesToCut & y < (_scanlines + _scanlinesToCut))
                {
                    var liner = ((y - _scanlinesToCut)*1024) + (x*2);
                    var liner2 = liner + 512;
                    _numPtr[liner] = color;
                    liner++;
                    _numPtr[liner] = color;
                    _numPtr[liner2] = color;
                    liner2++;
                    _numPtr[liner2] = color;
                }
            }
        }

        public void RenderFrame()
        {
            if (_canRender & _isRendering)
            {
                _bitmap.UnlockBits(_bmpData);
                //Draw it !!
                _graphics.DrawImage(_bitmap, 0, 0, _surface.Size.Width, _surface.Size.Height);
                //Draw the text
                if (_textApperance > 0)
                {
                    _graphics.DrawString(_textToRender, new Font("Tohama", 16, FontStyle.Bold),
                                  new SolidBrush(Color.White), new PointF(30, _surface.Height - 50));
                    _textApperance--;
                }
            }
            _isRendering = false;
            if (_highResSnap && _takingSnap)
            {
                _log.Flush();
                _log.Close();
                SaveSnap();
                _highResSnap = false;
                _takingSnap = false;
            }
        }

        public void TakeSnapshot(string snapPath, string format)
        {
            _highResSnap = true;
            _takingSnap = false;
            _snapFile = snapPath;
            _snapFormat = format;
        }

        public void DrawText(string text, int frames)
        {
            _textToRender = text;
            _textApperance = frames;
            //This draw is useful when the nes is paused.
            if (_surface != null & !_isRendering)
            {
                if (_textApperance > 0)
                {
                    var graphics = _surface.CreateGraphics();
                    graphics.DrawString(_textToRender, new Font("Tohama", 16, FontStyle.Bold),
                                  new SolidBrush(Color.White), new PointF(30, _surface.Height - 50));
                    _textApperance--;
                }
            }
        }

        public void ChangeSettings()
        {
            MessageBox.Show("Currently there's no settings for this mode.");
        }

        public void Clear()
        {
            _graphics.Clear(Color.Black);
        }

        public void UpdateSize(int x, int y, int w, int h)
        {
            _screenW = w;
            _screenH = h;
            _screenX = x;
            _screenY = y;
            _graphics = _surface.CreateGraphics();
            _graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            _graphics.Clear(Color.Black);
        }

        public bool IsSizable
        {
            get { return true; }
        }

        public bool IsRendering
        {
            get { return _isRendering; }
        }

        public bool CanRender
        {
            get { return _canRender; }
            set { _canRender = value; }
        }

        public bool FullScreen
        {
            get { return false; }
            set { }
        }

        public bool SupportFullScreen
        {
            get { return false; }
        }

        #endregion

        private void ReadHiResPack(string pPath, int chrPages)
        {
            if (Directory.Exists(pPath))
            {
                if (File.Exists(pPath + Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture) + "hires.txt"))
                {
                    _packData = new int[chrPages*8][];
                    for (var i = 0; i < chrPages*8; i++)
                    {
                        _packData[i] = new int[1024];
                        for (var j = 0; j < 1024; j++)
                        {
                            _packData[i][j] = -1;
                        }
                    }
                    _bitmaps = new List<Bitmap>();
                    _tileDatas = new List<TileData>();

                    using(var reader = new StreamReader(pPath + Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture) + "hires.txt"))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line != null && line.StartsWith("<img>"))
                            {
                                _bitmaps.Add(
                                    new Bitmap(pPath +
                                               Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture) +
                                               line.Substring(5)));
                            }
                            else if (line != null && line.StartsWith("<tile>"))
                            {
                                var d = line.Substring(6).Split(',');
                                if (d.Count() == 9)
                                {
                                    if (_packData[int.Parse(d[0])][int.Parse(d[1])] == -1)
                                    {
                                        _packData[int.Parse(d[0])][int.Parse(d[1])] = _tileDatas.Count;
                                        _tileDatas.Add(new TileData());
                                        _tileDatas.Last().BitmapP = new List<BitmapF>();
                                        _tileDatas.Last().DefaultId = -1;
                                    }
                                    var b = new BitmapF
                                                {
                                                    BitmapId = int.Parse(d[2]),
                                                    Color1 = int.Parse(d[3]),
                                                    Color2 = int.Parse(d[4]),
                                                    Color3 = int.Parse(d[5]),
                                                    X = int.Parse(d[6]),
                                                    Y = int.Parse(d[7])
                                                };
                                    if (d[8] == "Y")
                                        _tileDatas[_packData[int.Parse(d[0])][int.Parse(d[1])]].DefaultId =
                                            _tileDatas[_packData[int.Parse(d[0])][int.Parse(d[1])]].BitmapP.Count();
                                    _tileDatas[_packData[int.Parse(d[0])][int.Parse(d[1])]].BitmapP.Add(b);
                                }
                            }
                        }
                    }
                    if (chrPages > 0)
                    {
                        _hasHiResPack = true;
                    }
                }
            }
        }

        public void DrawPixelHiRes(int x, int y, int color, uint tilePage, int tileOffSet, int tileX, int tileY,
                                   bool hFlip, bool vFlip, byte color1, byte color2, byte color3, int renColor)
        {
            if (_canRender & _isRendering)
            {
                //Check if we should cut this line 
                if (y >= _scanlinesToCut & y < (_scanlines + _scanlinesToCut))
                {
                    var liner = ((y - _scanlinesToCut)*1024) + (x*2);
                    var liner2 = liner + 512;

                    var usePack = false;

                    if (_takingSnap)
                    {
                        if (tileX == 0 && tileY == 0)
                        {
                            _log.WriteLine("Page = " + tilePage.ToString(CultureInfo.InvariantCulture) + ", Offset = " + tileOffSet.ToString(CultureInfo.InvariantCulture) +
                                          ", X = " + x.ToString(CultureInfo.InvariantCulture) + ", Y = " + y.ToString(CultureInfo.InvariantCulture) + ", Color1 = " +
                                          color1.ToString(CultureInfo.InvariantCulture) + ", Color2 = " + color2.ToString(CultureInfo.InvariantCulture) + ", Color3 = " +
                                          color3.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    if (_hasHiResPack)
                    {
                        if (_packData[tilePage][tileOffSet] != -1)
                        {
                            usePack = true;
                        }
                    }
                    var packId = -1;
                    if (usePack)
                    {
                        TileData tileData = _tileDatas[_packData[tilePage][tileOffSet]];
                        for (var idx = 0; idx < tileData.BitmapP.Count; idx++)
                        {
                            if (tileData.BitmapP[idx].Color1 == color1 && tileData.BitmapP[idx].Color2 == color2 &&
                                tileData.BitmapP[idx].Color3 == color3) packId = idx;
                        }
                        if (packId == -1) packId = tileData.DefaultId;

                        if (packId != -1)
                        {
                            var b =
                                _bitmaps[tileData.BitmapP[packId].BitmapId].LockBits(
                                    new Rectangle(0, 0, _bitmaps[tileData.BitmapP[packId].BitmapId].Width,
                                                  _bitmaps[tileData.BitmapP[packId].BitmapId].Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                            var tpt = (int*) (b.Scan0) + (tileData.BitmapP[packId].Y + (tileY*2))*b.Width + tileData.BitmapP[packId].X +
                                      14 - (tileX*2);

                            if ((*tpt & 0xFF000000) != 0)
                            {
                                if (hFlip)
                                {
                                    if (vFlip) _numPtr[liner2] = *tpt;
                                    else _numPtr[liner] = *tpt;
                                }
                                else
                                {
                                    if (vFlip) _numPtr[liner2 + 1] = *tpt;
                                    else _numPtr[liner + 1] = *tpt;
                                }
                            }
                            tpt++;
                            if ((*tpt & 0xFF000000) != 0)
                            {
                                if (hFlip)
                                {
                                    if (vFlip) _numPtr[liner2 + 1] = *tpt;
                                    else _numPtr[liner + 1] = *tpt;
                                }
                                else
                                {
                                    if (vFlip) _numPtr[liner2] = *tpt;
                                    else _numPtr[liner] = *tpt;
                                }
                            }
                            tpt += b.Width;
                            if ((*tpt & 0xFF000000) != 0)
                            {
                                if (hFlip)
                                {
                                    if (vFlip) _numPtr[liner + 1] = *tpt;
                                    else _numPtr[liner2 + 1] = *tpt;
                                }
                                else
                                {
                                    if (vFlip) _numPtr[liner] = *tpt;
                                    else _numPtr[liner2] = *tpt;
                                }
                            }
                            tpt--;
                            if ((*tpt & 0xFF000000) != 0)
                            {
                                if (hFlip)
                                {
                                    if (vFlip) _numPtr[liner] = *tpt;
                                    else _numPtr[liner2] = *tpt;
                                }
                                else
                                {
                                    if (vFlip) _numPtr[liner + 1] = *tpt;
                                    else _numPtr[liner2 + 1] = *tpt;
                                }
                            }
                            _bitmaps[tileData.BitmapP[packId].BitmapId].UnlockBits(b);
                        }
                    }
                    if (packId == -1)
                    {
                        if ((renColor%4) != 0)
                        {
                            _numPtr[liner] = color;
                            liner++;
                            _numPtr[liner] = color;
                            _numPtr[liner2] = color;
                            liner2++;
                            _numPtr[liner2] = color;
                        }
                    }
                }
            }
        }

        private void SaveSnap()
        {
            switch (_snapFormat)
            {
                case ".bmp":
                    _bitmap.Save(_snapFile, ImageFormat.Bmp);
                    _canRender = true;
                    break;
                case ".gif":
                    _bitmap.Save(_snapFile, ImageFormat.Gif);
                    _canRender = true;
                    break;
                case ".jpg":
                    _bitmap.Save(_snapFile, ImageFormat.Jpeg);
                    _canRender = true;
                    break;
                case ".png":
                    _bitmap.Save(_snapFile, ImageFormat.Png);
                    _canRender = true;
                    break;
                case ".tiff":
                    _bitmap.Save(_snapFile, ImageFormat.Tiff);
                    _canRender = true;
                    break;
            }
        }

        public void Dispose()
        {
            if (!_hasHiResPack)
            {
                return;
            }
            foreach (var t in _bitmaps)
            {
                t.Dispose();
            }
        }
    }
}