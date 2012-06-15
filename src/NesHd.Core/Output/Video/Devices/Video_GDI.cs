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

using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

using NesHd.Core.Debugger;

namespace NesHd.Core.Output.Video.Devices
{
    public unsafe class VideoGdi : IGraphicDevice
    {
        bool _IsRendering = false;
        bool _CanRender = true;
        Bitmap bmp;
        Control _Surface;
        Graphics GR;
        BitmapData bmpData;
        int Screen_W = 0;
        int Screen_H = 0;
        int Screen_X = 0;
        int Screen_Y = 0;
        string TextToRender = "";
        int TextApperance = 200;
        int ScanlinesToCut = 0;
        int _Scanlines = 0;
        int* numPtr;
        int[] Buffer;

        public VideoGdi(TVFORMAT TvFormat, Control Surface)
        {
            if (Surface == null)
                return;
            Debug.WriteLine(this, "Initializeing GDI ...", DebugStatus.None);
            switch (TvFormat)
            {
                case TVFORMAT.NTSC:
                    this.ScanlinesToCut = 8;
                    this._Scanlines = 224;
                    this.bmp = new Bitmap(256, 224);
                    break;
                case TVFORMAT.PAL:
                    this.ScanlinesToCut = 0;
                    this._Scanlines = 240;
                    this.bmp = new Bitmap(256, 240);
                    break;
            }
            this.Buffer = new int[256 * this._Scanlines];
            this._Surface = Surface;
            this.UpdateSize(0, 0, this._Surface.Width + 1, this._Surface.Height + 1);
            Debug.WriteLine(this, "Video device " + @"""" + "GDI" + @"""" + " Ok !!", DebugStatus.Cool);
        }
        public string Name
        {
            get { return "Windows GDI"; }
        }
        public string Description
        {
            get { return "Render the video using System.Drawing\nYou can not switch to fullscreen with this mode. This mode is perfect (best quality) and accurate but needs pc power and not recommanded for old pcs."; }
        }
        public unsafe void Begin()
        {
            if (this._CanRender)
            {
                this._IsRendering = true;
            }
        }
        public unsafe void AddScanline(int Line, int[] ScanlineBuffer)
        {
            if (this._CanRender & this._IsRendering)
            {
                //Check if we should cut this line 
                if (Line > this.ScanlinesToCut & Line < (this._Scanlines + this.ScanlinesToCut))
                {
                    int liner = ((Line - this.ScanlinesToCut) * 256);
                    //Set the scanline into the buffer
                    for (int i = 0; i < 256; i++)
                        this.Buffer[liner + i] = ScanlineBuffer[i];
                }
            }
        }
        public unsafe void DrawPixel(int X, int Y, int Color)
        {
            if (this._CanRender & this._IsRendering)
            {
                //Check if we should cut this line 
                if (Y >= this.ScanlinesToCut & Y < (this._Scanlines + this.ScanlinesToCut))
                {
                    int liner = ((Y - this.ScanlinesToCut) * 256) + X;
                    this.Buffer[liner] = Color;
                    liner++;
                }
            }
        }
        public void RenderFrame()
        {
            if (this._CanRender & this._IsRendering)
            {
                this.bmpData = this.bmp.LockBits(new Rectangle(0, 0, 256, this._Scanlines),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                this.numPtr = (int*)this.bmpData.Scan0;
                for (int i = 0; i < this.Buffer.Length; i++)
                {
                    this.numPtr[i] = this.Buffer[i];
                }
                this.bmp.UnlockBits(this.bmpData);
                //Draw it !!
                this.GR.DrawImage(this.bmp, 0, 0, this._Surface.Size.Width, this._Surface.Size.Height);
                //Draw the text
                if (this.TextApperance > 0)
                {
                    this.GR.DrawString(this.TextToRender, new System.Drawing.Font("Tohama", 16, FontStyle.Bold),
                       new SolidBrush(Color.White), new PointF(30, this._Surface.Height - 50));
                    this.TextApperance--;
                }
            }
            this._IsRendering = false;
        }
        public void TakeSnapshot(string SnapPath, string Format)
        {
            switch (Format)
            {
                case ".bmp":
                    this.bmp.Save(SnapPath, ImageFormat.Bmp);
                    this._CanRender = true;
                    break;
                case ".gif":
                    this.bmp.Save(SnapPath, ImageFormat.Gif);
                    this._CanRender = true;
                    break;
                case ".jpg":
                    this.bmp.Save(SnapPath, ImageFormat.Jpeg);
                    this._CanRender = true;
                    break;
                case ".png":
                    this.bmp.Save(SnapPath, ImageFormat.Png);
                    this._CanRender = true;
                    break;
                case ".tiff":
                    this.bmp.Save(SnapPath, ImageFormat.Tiff);
                    this._CanRender = true;
                    break;
            }
        }
        public void DrawText(string Text, int Frames)
        {
            this.TextToRender = Text;
            this.TextApperance = Frames;
            //This draw is useful when the nes is paused.
            if (this._Surface != null & !this._IsRendering)
            {
                if (this.TextApperance > 0)
                {
                    Graphics GR = this._Surface.CreateGraphics();
                    GR.DrawString(this.TextToRender, new System.Drawing.Font("Tohama", 16, FontStyle.Bold),
                        new SolidBrush(Color.White), new PointF(30, this._Surface.Height - 50));
                    this.TextApperance--;
                }
            }
        }
        public void ChangeSettings()
        {
            MessageBox.Show("Currently there's no settings for this mode.");
        }
        public void Clear()
        {
            this.GR.Clear(Color.Black);
        }
        public void UpdateSize(int X, int Y, int W, int H)
        {
            this.Screen_W = W;
            this.Screen_H = H;
            this.Screen_X = X;
            this.Screen_Y = Y;
            this.GR = this._Surface.CreateGraphics();
            this.GR.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.GR.Clear(Color.Black);
        }
        public bool IsSizable
        { get { return true; } }
        public bool IsRendering
        {
            get { return this._IsRendering; }
        }
        public bool CanRender
        {
            get { return this._CanRender; }
            set { this._CanRender = value; }
        }
        public bool FullScreen
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
        public bool SupportFullScreen
        {
            get { return false; }
        }
    }
}
