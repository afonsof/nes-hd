using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NesHd.Core.Debugger;
using SlimDX;
using SlimDX.Direct3D9;

namespace NesHd.Core.Output.Video.Devices.SlimDX
{
    public class VideoSlimDx : IGraphicDevice, IDisposable
    {
        private readonly int _firstLinesTCut;

        private readonly int _multi;
        private readonly int _height;
        private readonly int _width;

        private readonly Control _surface;
        private int _buffSize = 229376;
        private bool _initialized;
        private int _modeIndex;
        private bool _canRender = true;
        private bool _fullScreen;
        private bool _isRendering;
        private Vector3 _position;
        private Texture _backBuffer;
        private bool _deviceLost;
        private byte[] _displayData = new byte[245760];
        private Device _displayDevice;
        private Rectangle _displayRect;
        private Sprite _displaySprite;
        private bool _disposed;
        private Texture _nesDisplay;
        private PresentParameters _presentParams;
        public Direct3D D3D = new Direct3D();
        private DisplayMode _mode;

        public VideoSlimDx(TvFormat tvFormat, Control surface, byte multi)
        {
            _multi = multi;
            _width = 256 * multi;

            if (surface == null)
                return;
            _surface = surface;
            switch (tvFormat)
            {
                case TvFormat.Ntsc:
                    _height = 224 * _multi;
                    _firstLinesTCut = 8;
                    break;
                case TvFormat.Pal:
                    _height = 240 * _multi;
                    _firstLinesTCut = 0;
                    break;
            }
            var sett = new VideoModeSettings();
            sett.Reload();
            ApplaySettings(sett);
            //InitializeDirect3D();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_displayDevice != null)
            {
                _displayDevice.Dispose();
            }
            DisposeDisplayObjects();
            _disposed = true;
        }

        #endregion

        #region IGraphicDevice Members

        public string Name
        {
            get { return "SlimDX"; }
        }

        public string Description
        {
            get
            {
                return
                    "Render the video using SlimDX library.\nYou can switch into fullscreen using this mode with the resolution you set in the settings of this mode. This mode is accurate and fast.";
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
                if (line >= _firstLinesTCut & line < (_height + _firstLinesTCut))
                {
                    for (var i = 0; i < 256; i++)
                    {
                        //Extract the colors of each pixel
                        //To arrange the B,G,R,A
                        var liner = ((line - _firstLinesTCut) * 256) + i;
                        var buffPos = liner * 4;
                        _displayData[buffPos] = (byte)(scanlineBuffer[i] & 0xFF);
                        buffPos++; //Blue
                        _displayData[buffPos] = (byte)(((scanlineBuffer[i] & 0xFF00) >> 8));
                        buffPos++; //Green
                        _displayData[buffPos] = (byte)(((scanlineBuffer[i] & 0xFF0000) >> 0x10));
                        buffPos++; //Red
                        _displayData[buffPos] = 0xFF; //Alpha
                    }
                }
            }
        }

        public void DrawPixel(int x, int y, int color)
        {
            x *= _multi;
            y *= _multi;
            DrawAbsolutePixel(x, y, color);
        }

        public void DrawAbsolutePixel(int x, int y, int color)
        {
            if (!(_canRender & _isRendering))
            {
                return;
            }
            //Check if we should cut this line 
            if (!(y >= _firstLinesTCut & y < (_height + _firstLinesTCut)))
            {
                return;
            }
            var liner = ((y - _firstLinesTCut) * _width) + x;
            var buffPos = liner * 4;
            _displayData[buffPos] = (byte)(color & 0xFF);
            buffPos++; //Blue
            _displayData[buffPos] = (byte)((color & 0xFF00) >> 8);
            buffPos++; //Green
            _displayData[buffPos] = (byte)(((color & 0xFF0000) >> 0x10));
            buffPos++; //Red
            _displayData[buffPos] = 0xFF; //Alpha*/
        }

        public void RenderFrame()
        {
            if (!(_surface != null & _canRender & !_disposed & _initialized))
            {
                return;
            }
            var result = _displayDevice.TestCooperativeLevel();
            switch (result.Code)
            {
                case -2005530520:
                    _deviceLost = true;
                    break;

                case -2005530519:
                    ResetDirect3D();
                    _deviceLost = false;
                    break;
            }
            if (!_deviceLost)
            {
                UpdateDisplayTexture();
                _displayDevice.BeginScene();
                _displayDevice.Clear(ClearFlags.Target, Color.Black, 0f, 0);
                _displaySprite.Begin(SpriteFlags.None);
                if (_fullScreen)
                {
                    _displaySprite.Draw(_nesDisplay, _displayRect, new Vector3(), _position, Color.White);
                }
                else
                {
                    _displaySprite.Draw(_nesDisplay, _displayRect, Color.White);
                }
                _displaySprite.End();
                _displayDevice.EndScene();
                _displayDevice.Present();
            }
            _isRendering = false;
        }

        public unsafe void TakeSnapshot(string snapPath, string format)
        {
            var bmp = new Bitmap(_width, _height);
            var bmpData32 = bmp.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly,
                                         PixelFormat.Format32bppRgb);
            var numPtr32 = (int*)bmpData32.Scan0;
            var j = 0;
            for (var i = (_firstLinesTCut * _width); i < 61440 - (_firstLinesTCut * _width); i++)
            {
                var valBlue = (_displayData[j]);
                j++;
                var valGreen = (_displayData[j]);
                j++;
                var valRed = (_displayData[j]);
                j++;
                var valAlpha = (_displayData[j]);
                j++;
                numPtr32[i - (_firstLinesTCut * _width)] =
                    (valAlpha << 24 | //Alpha
                     valRed << 16 | //Red
                     valGreen << 8 | //Green
                     valBlue) //Blue
                    ;
            }
            bmp.UnlockBits(bmpData32);
            switch (format)
            {
                case ".bmp":
                    bmp.Save(snapPath, ImageFormat.Bmp);
                    _canRender = true;
                    break;
                case ".gif":
                    bmp.Save(snapPath, ImageFormat.Gif);
                    _canRender = true;
                    break;
                case ".jpg":
                    bmp.Save(snapPath, ImageFormat.Jpeg);
                    _canRender = true;
                    break;
                case ".png":
                    bmp.Save(snapPath, ImageFormat.Png);
                    _canRender = true;
                    break;
                case ".tiff":
                    bmp.Save(snapPath, ImageFormat.Tiff);
                    _canRender = true;
                    break;
            }
        }

        public void DrawText(string text, int frames)
        {
        }

        public void ChangeSettings()
        {
            var options = new Frm_SlimDXOptions(this);
            options.ShowDialog(_surface.Parent);
        }

        public void Clear()
        {
            //??!!
        }

        public void UpdateSize(int x, int y, int w, int h)
        {
        }

        public bool IsSizable
        {
            get { return false; }
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
            get { return _fullScreen; }
            set
            {
                _fullScreen = value;
                Dispose();
                InitializeDirect3D();
            }
        }

        public bool SupportFullScreen
        {
            get { return true; }
        }

        #endregion

        private void InitializeDirect3D()
        {
            try
            {
                if (!_fullScreen)
                {
                    D3D = new Direct3D();
                    _presentParams = new PresentParameters
                    {
                        BackBufferWidth = _width,
                        BackBufferHeight = _height,
                        Windowed = true,
                        SwapEffect = SwapEffect.Discard,
                        Multisample = MultisampleType.None,
                        PresentationInterval = PresentInterval.Immediate
                    };
                    _displayDevice = new Device(D3D, 0, DeviceType.Hardware, _surface.Handle,
                                                CreateFlags.SoftwareVertexProcessing, new[] { _presentParams });
                    _displayRect = new Rectangle(0, 0, _width, _height);
                    _buffSize = (_width * _height) * 4;
                    _displayData = new byte[_buffSize];
                    CreateDisplayObjects(_displayDevice);
                    _initialized = true;
                    _disposed = false;
                    Debug.WriteLine(this, "SlimDX video mode (Windowed) Initialized ok.", DebugStatus.Cool);
                }
                else
                {
                    D3D = new Direct3D();
                    _mode = FindSupportedMode();
                    _presentParams = new PresentParameters
                    {
                        BackBufferFormat = _mode.Format,
                        BackBufferCount = 1,
                        BackBufferWidth = _mode.Width,
                        BackBufferHeight = _mode.Height,
                        Windowed = false,
                        FullScreenRefreshRateInHertz = _mode.RefreshRate,
                        SwapEffect = SwapEffect.Discard,
                        Multisample = MultisampleType.None,
                        PresentationInterval = PresentInterval.Immediate
                    };
                    _displayDevice = new Device(D3D, 0, DeviceType.Hardware, _surface.Parent.Handle,
                                                CreateFlags.SoftwareVertexProcessing, new[] { _presentParams })
                    {
                        ShowCursor = false,
                    };
                    _displayDevice.SetRenderState(RenderState.PointScaleEnable, true);
                    _displayDevice.SetRenderState(RenderState.PointSpriteEnable, true);
                    _displayRect = new Rectangle(0, 0, _width, _height);
                    _position = new Vector3((_mode.Width - _width) / 2, (_mode.Height - _height) / 2, 0);
                    _buffSize = (_width * _height) * 4;
                    _displayData = new byte[_buffSize];
                    CreateDisplayObjects(_displayDevice);
                    _initialized = true;
                    _disposed = false;

                    Debug.WriteLine(this, "SlimDX video mode (Fullscreen) Initialized ok.", DebugStatus.Cool);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, "Could not Initialize SlimDX mode because of : \n" + ex.Message, DebugStatus.Error);
                _initialized = false;
            }
        }

        private DisplayMode FindSupportedMode()
        {
            var supportedDisplayModes = D3D.Adapters[0].GetDisplayModes(Format.X8R8G8B8);
            var mode = supportedDisplayModes[_modeIndex];
            return mode;
        }

        private void CreateDisplayObjects(Device device)
        {
            _displaySprite = new Sprite(device);
            _backBuffer = new Texture(device, _width, _height, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.SystemMemory);
            _nesDisplay = new Texture(device, _width, _height, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
        }

        private void UpdateDisplayTexture()
        {
            if ((_displayData != null) && !_deviceLost)
            {
                var rect = _backBuffer.LockRectangle(0, LockFlags.DoNotWait);
                rect.Data.Write(_displayData, 0, _buffSize);
                rect.Data.Close();
                _backBuffer.UnlockRectangle(0);
                if (_canRender & !_disposed)
                    _displayDevice.UpdateTexture(_backBuffer, _nesDisplay);
            }
        }

        private void ResetDirect3D()
        {
            if (!_initialized)
                return;
            DisposeDisplayObjects();
            try
            {
                _displayDevice.Reset(_presentParams);
                CreateDisplayObjects(_displayDevice);
            }
            catch
            {
                _displayDevice.Dispose();
                InitializeDirect3D();
            }
        }

        private void DisposeDisplayObjects()
        {
            if (_displaySprite != null)
            {
                _displaySprite.Dispose();
            }
            if (_nesDisplay != null)
            {
                _nesDisplay.Dispose();
            }
            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
            }
        }

        public void ApplaySettings(VideoModeSettings newSettings)
        {
            _modeIndex = newSettings.SlimDX_ResMode;
        }
    }
}