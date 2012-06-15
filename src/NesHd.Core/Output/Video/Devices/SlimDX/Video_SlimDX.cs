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
        bool _disposed;
        bool _IsRendering = false;
        bool _CanRender = true;
        int _ScanLines = 240;
        int _FirstLinesTCut = 0;
        Control _Surface;
        Texture _backBuffer;
        bool _deviceLost;
        byte[] _displayData = new byte[245760];
        int BuffSize = 229376;
        Device _displayDevice;
        Format _displayFormat;
        Rectangle _displayRect;
        Sprite _displaySprite;
        Texture _nesDisplay;
        public Direct3D d3d = new Direct3D();
        PresentParameters _presentParams;
        bool Initialized = false;
        bool _FullScreen = false;
        Vector3 _Position;
        int ModeIndex = 0;
        DisplayMode mode;

        public VideoSlimDx(TVFORMAT TvFormat, Control Surface)
        {
            if (Surface == null)
                return;
            this._Surface = Surface;
            switch (TvFormat)
            {
                case TVFORMAT.NTSC:
                    this._ScanLines = 224;
                    this._FirstLinesTCut = 8;
                    break;
                case TVFORMAT.PAL:
                    this._ScanLines = 240;
                    this._FirstLinesTCut = 0;
                    break;
            }
            VideoModeSettings sett = new VideoModeSettings();
            sett.Reload();
            this.ApplaySettings(sett);
            //InitializeDirect3D();
        }
        void InitializeDirect3D()
        {
            try
            {
                if (!this._FullScreen)
                {
                    this.d3d = new Direct3D();
                    this._presentParams = new PresentParameters();
                    this._presentParams.BackBufferWidth = 256;
                    this._presentParams.BackBufferHeight = this._ScanLines;
                    this._presentParams.Windowed = true;
                    this._presentParams.SwapEffect = SwapEffect.Discard;
                    this._presentParams.Multisample = MultisampleType.None;
                    this._presentParams.PresentationInterval = PresentInterval.Immediate;
                    this._displayFormat = this.d3d.Adapters[0].CurrentDisplayMode.Format;
                    this._displayDevice = new Device(this.d3d, 0, DeviceType.Hardware, this._Surface.Handle, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { this._presentParams });
                    this._displayRect = new Rectangle(0, 0, 256, this._ScanLines);
                    this.BuffSize = (256 * this._ScanLines) * 4;
                    this._displayData = new byte[this.BuffSize];
                    this.CreateDisplayObjects(this._displayDevice);
                    this.Initialized = true;
                    this._disposed = false;
                    Debug.WriteLine(this, "SlimDX video mode (Windowed) Initialized ok.", DebugStatus.Cool);
                }
                else
                {
                    this.d3d = new Direct3D();
                    this.mode = this.FindSupportedMode();
                    this._presentParams = new PresentParameters();
                    this._presentParams.BackBufferFormat = this.mode.Format;
                    this._presentParams.BackBufferCount = 1;
                    this._presentParams.BackBufferWidth = this.mode.Width;
                    this._presentParams.BackBufferHeight = this.mode.Height;
                    this._presentParams.Windowed = false;
                    this._presentParams.FullScreenRefreshRateInHertz = this.mode.RefreshRate;
                    this._presentParams.SwapEffect = SwapEffect.Discard;
                    this._presentParams.Multisample = MultisampleType.None;
                    this._presentParams.PresentationInterval = PresentInterval.Immediate;
                    this._displayFormat = this.mode.Format;
                    this._displayDevice = new Device(this.d3d, 0, DeviceType.Hardware, this._Surface.Parent.Handle, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { this._presentParams });
                    this._displayDevice.ShowCursor = false;
                    this._displayDevice.SetRenderState(RenderState.PointScaleEnable, true);
                    this._displayDevice.SetRenderState(RenderState.PointSpriteEnable, true);
                    this._displayRect = new Rectangle(0, 0, 256, this._ScanLines);
                    this._Position = new Vector3((this.mode.Width - 256) / 2, (this.mode.Height - this._ScanLines) / 2, 0);
                    this.BuffSize = (256 * this._ScanLines) * 4;
                    this._displayData = new byte[this.BuffSize];
                    this.CreateDisplayObjects(this._displayDevice);
                    this.Initialized = true;
                    this._disposed = false;

                    Debug.WriteLine(this, "SlimDX video mode (Fullscreen) Initialized ok.", DebugStatus.Cool);
                }
            }
            catch (Exception EX)
            { Debug.WriteLine(this, "Could not Initialize SlimDX mode because of : \n" + EX.Message, DebugStatus.Error); this.Initialized = false; }
        }
        DisplayMode FindSupportedMode()
        {
            DisplayModeCollection supportedDisplayModes = this.d3d.Adapters[0].GetDisplayModes(Format.X8R8G8B8);
            DisplayMode mode = supportedDisplayModes[this.ModeIndex];
            return mode;
        }
        private void CreateDisplayObjects(Device device)
        {
            this._displaySprite = new Sprite(device);
            this._backBuffer = new Texture(device, 256, this._ScanLines, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.SystemMemory);
            this._nesDisplay = new Texture(device, 256, this._ScanLines, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
        }
        public string Name
        {
            get { return "SlimDX"; }
        }
        public string Description
        {
            get { return "Render the video using SlimDX library.\nYou can switch into fullscreen using this mode with the resolution you set in the settings of this mode. This mode is accurate and fast."; }
        }
        public void Begin()
        {
            if (this._CanRender)
            {
                this._IsRendering = true;
            }
        }
        public void AddScanline(int Line, int[] ScanlineBuffer)
        {
            if (this._CanRender & this._IsRendering)
            {
                //Check if we should cut this line 
                if (Line >= this._FirstLinesTCut & Line < (this._ScanLines + this._FirstLinesTCut))
                {
                    for (int i = 0; i < 256; i++)
                    {
                        //Extract the colors of each pixel
                        //To arrange the B,G,R,A
                        int liner = ((Line - this._FirstLinesTCut) * 256) + i;
                        int BuffPos = liner * 4;
                        this._displayData[BuffPos] = (byte)(ScanlineBuffer[i] & 0xFF); BuffPos++;//Blue
                        this._displayData[BuffPos] = (byte)(((ScanlineBuffer[i] & 0xFF00) >> 8)); BuffPos++;//Green
                        this._displayData[BuffPos] = (byte)(((ScanlineBuffer[i] & 0xFF0000) >> 0x10)); BuffPos++;//Red
                        this._displayData[BuffPos] = 0xFF; BuffPos++;//Alpha
                    }
                }
            }
        }
        public void DrawPixel(int X, int Y, int Color)
        {
            if (this._CanRender & this._IsRendering)
            {
                //Check if we should cut this line 
                if (Y >= this._FirstLinesTCut & Y < (this._ScanLines + this._FirstLinesTCut))
                {
                    int liner = ((Y - this._FirstLinesTCut) * 256) + X;
                    int BuffPos = liner * 4;
                    this._displayData[BuffPos] = (byte)(Color & 0xFF); BuffPos++;//Blue
                    this._displayData[BuffPos] = (byte)((Color & 0xFF00) >> 8); BuffPos++;//Green
                    this._displayData[BuffPos] = (byte)(((Color & 0xFF0000) >> 0x10)); BuffPos++;//Red
                    this._displayData[BuffPos] = 0xFF; //Alpha
                }
            }
        }
        public void RenderFrame()
        {
            if (this._Surface != null & this._CanRender & !this._disposed & this.Initialized)
            {
                Result result = this._displayDevice.TestCooperativeLevel();
                switch (result.Code)
                {
                    case -2005530520:
                        this._deviceLost = true;
                        break;

                    case -2005530519:
                        this.ResetDirect3D();
                        this._deviceLost = false;
                        break;
                }
                if (!this._deviceLost)
                {
                    this.UpdateDisplayTexture();
                    this._displayDevice.BeginScene();
                    this._displayDevice.Clear(ClearFlags.Target, Color.Black, 0f, 0);
                    this._displaySprite.Begin(SpriteFlags.None);
                    if (this._FullScreen)
                    {
                        this._displaySprite.Draw(this._nesDisplay, this._displayRect, new Vector3(), this._Position, Color.White);
                    }
                    else
                    {
                        this._displaySprite.Draw(this._nesDisplay, this._displayRect, Color.White);
                    }
                    this._displaySprite.End();
                    this._displayDevice.EndScene();
                    this._displayDevice.Present();
                }
                this._IsRendering = false;
            }
        }
        void UpdateDisplayTexture()
        {
            if ((this._displayData != null) && !this._deviceLost)
            {
                DataRectangle rect = this._backBuffer.LockRectangle(0, LockFlags.DoNotWait);
                rect.Data.Write(this._displayData, 0, this.BuffSize);
                rect.Data.Close();
                this._backBuffer.UnlockRectangle(0);
                if (this._CanRender & !this._disposed)
                    this._displayDevice.UpdateTexture(this._backBuffer, this._nesDisplay);
            }
        }
        void ResetDirect3D()
        {
            if (!this.Initialized)
                return;
            this.DisposeDisplayObjects();
            try
            {
                this._displayDevice.Reset(this._presentParams);
                this.CreateDisplayObjects(this._displayDevice);
            }
            catch
            {
                this._displayDevice.Dispose();
                this.InitializeDirect3D();
            }
        }
        void DisposeDisplayObjects()
        {
            if (this._displaySprite != null)
            {
                this._displaySprite.Dispose();
            }
            if (this._nesDisplay != null)
            {
                this._nesDisplay.Dispose();
            }
            if (this._backBuffer != null)
            {
                this._backBuffer.Dispose();
            }
        }
        public void Dispose()
        {
            if (this._displayDevice != null)
            {
                this._displayDevice.Dispose();
            }
            this.DisposeDisplayObjects();
            this._disposed = true;
        }
        public unsafe void TakeSnapshot(string SnapPath, string Format)
        {
            Bitmap bmp = new Bitmap(256, this._ScanLines);
            BitmapData bmpData32 = bmp.LockBits(new Rectangle(0, 0, 256, this._ScanLines), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            int* numPtr32 = (int*)bmpData32.Scan0;
            int j = 0;
            for (int i = (this._FirstLinesTCut * 256); i < 61440 - (this._FirstLinesTCut * 256); i++)
            {
                byte VALBLUE = (byte)(this._displayData[j]);
                j++;
                byte VALGREEN = (byte)(this._displayData[j]);
                j++;
                byte VALRED = (byte)(this._displayData[j]);
                j++;
                byte VALALHA = (byte)(this._displayData[j]);
                j++;
                numPtr32[i - (this._FirstLinesTCut * 256)] =
                   (VALALHA << 24 |//Alpha
                   VALRED << 16 |//Red
                  VALGREEN << 8 |//Green
                  VALBLUE)//Blue
                             ;
            }
            bmp.UnlockBits(bmpData32);
            switch (Format)
            {
                case ".bmp":
                    bmp.Save(SnapPath, ImageFormat.Bmp);
                    this._CanRender = true;
                    break;
                case ".gif":
                    bmp.Save(SnapPath, ImageFormat.Gif);
                    this._CanRender = true;
                    break;
                case ".jpg":
                    bmp.Save(SnapPath, ImageFormat.Jpeg);
                    this._CanRender = true;
                    break;
                case ".png":
                    bmp.Save(SnapPath, ImageFormat.Png);
                    this._CanRender = true;
                    break;
                case ".tiff":
                    bmp.Save(SnapPath, ImageFormat.Tiff);
                    this._CanRender = true;
                    break;
            }
        }
        public void DrawText(string Text, int Frames)
        {

        }
        public void ChangeSettings()
        {
            Frm_SlimDXOptions Op = new Frm_SlimDXOptions(this);
            Op.ShowDialog(this._Surface.Parent);
        }
        public void Clear()
        {
            //??!!
        }
        public void UpdateSize(int X, int Y, int W, int H)
        {

        }
        public bool IsSizable
        {
            get { return false; }
        }
        public bool IsRendering
        {
            get { return this._IsRendering; }
        }
        public bool CanRender
        {
            get
            {
                return this._CanRender;
            }
            set
            {
                this._CanRender = value;
            }
        }
        public bool FullScreen
        {
            get
            {
                return this._FullScreen;
            }
            set
            {
                this._FullScreen = value;
                this.Dispose();
                this.InitializeDirect3D();
            }
        }
        public bool SupportFullScreen
        {
            get { return true; }
        }
        public void ApplaySettings(VideoModeSettings newSettings)
        {
            this.ModeIndex = newSettings.SlimDX_ResMode;
        }
    }
}
