using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NesHd.Core.Debugger;

namespace NesHd.Core.Output.Video.Devices
{
    public unsafe class VideoGdi : IGraphicDevice
    {
        private readonly int[] _buffer;
        private readonly int _scanlinesToCut;
        private readonly int _scanlines;
        private readonly Control _surface;
        private readonly Bitmap _bitmap;
        private Graphics _graphics;
        private int Screen_H;
        private int Screen_W;
        private int Screen_X;
        private int Screen_Y;

        private int _textApperance = 200;
        private string _textToRender = "";
        private bool _canRender = true;
        private bool _isRendering;
        private BitmapData _bitmapData;
        private int* _numPtr;

        public VideoGdi(TVFORMAT tvFormat, Control surface)
        {
            if (surface == null)
                return;
            Debug.WriteLine(this, "Initializeing GDI ...", DebugStatus.None);
            switch (tvFormat)
            {
                case TVFORMAT.NTSC:
                    _scanlinesToCut = 8;
                    _scanlines = 224;
                    _bitmap = new Bitmap(256, 224);
                    break;
                case TVFORMAT.PAL:
                    _scanlinesToCut = 0;
                    _scanlines = 240;
                    _bitmap = new Bitmap(256, 240);
                    break;
            }
            _buffer = new int[256*_scanlines];
            _surface = surface;
            UpdateSize(0, 0, _surface.Width + 1, _surface.Height + 1);
            Debug.WriteLine(this, "Video device " + @"""" + "GDI" + @"""" + " Ok !!", DebugStatus.Cool);
        }

        #region IGraphicDevice Members

        public string Name
        {
            get { return "Windows GDI"; }
        }

        public string Description
        {
            get
            {
                return
                    "Render the video using System.Drawing\nYou can not switch to fullscreen with this mode. This mode is perfect (best quality) and accurate but needs pc power and not recommanded for old pcs.";
            }
        }

        public void Begin()
        {
            if (_canRender)
            {
                _isRendering = true;
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
                        _buffer[liner + i] = scanlineBuffer[i];
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
                    var liner = ((y - _scanlinesToCut)*256) + x;
                    _buffer[liner] = color;
                }
            }
        }

        public void RenderFrame()
        {
            if (_canRender & _isRendering)
            {
                _bitmapData = _bitmap.LockBits(new Rectangle(0, 0, 256, _scanlines),
                                       ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                _numPtr = (int*) _bitmapData.Scan0;
                for (var i = 0; i < _buffer.Length; i++)
                {
                    _numPtr[i] = _buffer[i];
                }
                _bitmap.UnlockBits(_bitmapData);
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
        }

        public void TakeSnapshot(string snapPath, string format)
        {
            switch (format)
            {
                case ".bmp":
                    _bitmap.Save(snapPath, ImageFormat.Bmp);
                    _canRender = true;
                    break;
                case ".gif":
                    _bitmap.Save(snapPath, ImageFormat.Gif);
                    _canRender = true;
                    break;
                case ".jpg":
                    _bitmap.Save(snapPath, ImageFormat.Jpeg);
                    _canRender = true;
                    break;
                case ".png":
                    _bitmap.Save(snapPath, ImageFormat.Png);
                    _canRender = true;
                    break;
                case ".tiff":
                    _bitmap.Save(snapPath, ImageFormat.Tiff);
                    _canRender = true;
                    break;
            }
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
            Screen_W = w;
            Screen_H = h;
            Screen_X = x;
            Screen_Y = y;
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
    }
}