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
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

using NesHd.Core.Debugger;

namespace NesHd.Core.Output.Video.Devices
{
    public unsafe class VideoGdiHiRes : IGraphicDevice
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

        bool hasHiResPack = false;
        List<Bitmap> bitmaps;
        List<TileData> tdata;
        int[][] packData;
        StreamWriter log;
        bool highResSnap = false;
        bool takingSnap = false;
        string snapfile;
        string snapformat;

        public VideoGdiHiRes(TVFORMAT TvFormat, Control Surface, string pPath, int chrPages)
        {
            if (Surface == null)
                return;
            Debug.WriteLine(this, "Initializeing GDI ...", DebugStatus.None);
            switch (TvFormat)
            {
                case TVFORMAT.NTSC:
                    this.ScanlinesToCut = 8;
                    this._Scanlines = 224;
                    this.bmp = new Bitmap(512, 448);
                    break;
                case TVFORMAT.PAL:
                    this.ScanlinesToCut = 0;
                    this._Scanlines = 240;
                    this.bmp = new Bitmap(512, 480);
                    break;
            }
            this._Surface = Surface;
            this.UpdateSize(0, 0, this._Surface.Width + 1, this._Surface.Height + 1);

            
            if (pPath != "") this.ReadHiResPack(Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(pPath), chrPages);
            
            Debug.WriteLine(this, "Video device " + @"""" + "GDI Hi Res" + @"""" + " Ok !!", DebugStatus.Cool);
        }

        void ReadHiResPack(string pPath, int chrPages)
        {
            if (Directory.Exists(pPath))
            {
                if (File.Exists(pPath + Path.DirectorySeparatorChar.ToString() + "hires.txt"))
                {

                    this.packData = new int[chrPages * 8][];
                    for (int i = 0; i < chrPages * 8; i++)
                    {
                        this.packData[i] = new int[1024];
                        for (int j = 0; j < 1024; j++)
                        {
                            this.packData[i][j] = -1;
                        }
                    }
                    this.bitmaps = new List<Bitmap>();
                    this.tdata = new List<TileData>();

                    StreamReader r = new StreamReader(pPath + Path.DirectorySeparatorChar.ToString() + "hires.txt");
                    while (!r.EndOfStream)
                    {
                        String s = r.ReadLine();
                        if (s.StartsWith("<img>"))
                        {
                            this.bitmaps.Add(new Bitmap(pPath + Path.DirectorySeparatorChar.ToString() + s.Substring(5)));
                        }
                        else if (s.StartsWith("<tile>"))
                        {
                            String[] d = s.Substring(6).Split(',');
                            if (d.Count() == 9)
                            {
                                if (this.packData[int.Parse(d[0])][int.Parse(d[1])] == -1)
                                {
                                    this.packData[int.Parse(d[0])][int.Parse(d[1])] = this.tdata.Count;
                                    this.tdata.Add(new TileData());
                                    this.tdata.Last().BitmapP = new List<BitmapF>();
                                    this.tdata.Last().DefaultId = -1;
                                }
                                BitmapF b = new BitmapF();
                                b.BitmapId = int.Parse(d[2]);
                                b.Color1 = int.Parse(d[3]);
                                b.Color2 = int.Parse(d[4]);
                                b.Color3 = int.Parse(d[5]);
                                b.X = int.Parse(d[6]);
                                b.Y = int.Parse(d[7]);
                                if (d[8] == "Y") this.tdata[this.packData[int.Parse(d[0])][int.Parse(d[1])]].DefaultId = this.tdata[this.packData[int.Parse(d[0])][int.Parse(d[1])]].BitmapP.Count();
                                this.tdata[this.packData[int.Parse(d[0])][int.Parse(d[1])]].BitmapP.Add(b);
                            }
                        }
                    }
                    r.Close();
                    if (chrPages > 0) this.hasHiResPack = true;
                }
            }
        }

        public string Name
        {
            get { return "Windows GDI Hi Res"; }
        }
        public string Description
        {
            get { return "Same as GDI with double resolution and uses hi res graphics packs."; }
        }
        public unsafe void Begin()
        {
            if (this._CanRender)
            {
                this._IsRendering = true;
                this.bmpData = this.bmp.LockBits(new Rectangle(0, 0, 512, this._Scanlines * 2),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                this.numPtr = (int*)this.bmpData.Scan0;
                if (this.highResSnap)
                {
                    this.log = new StreamWriter(this.snapfile + "log.txt");
                    this.takingSnap = true;
                }
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
                        this.numPtr[liner + i] = ScanlineBuffer[i];
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
                    int liner = ((Y - this.ScanlinesToCut) * 1024) + (X * 2);
                    int liner2 = liner + 512;
                    this.numPtr[liner] = Color;
                    liner++;
                    this.numPtr[liner] = Color;
                    this.numPtr[liner2] = Color;
                    liner2++;
                    this.numPtr[liner2] = Color;
                }
            }

        }

        public void DrawPixelHiRes(int X, int Y, int Color, uint tilePage, int tileOffSet, int tileX, int tileY, bool hFlip, bool vFlip, byte color1, byte color2, byte color3, int renColor)
        {
            if (this._CanRender & this._IsRendering)
            {
                //Check if we should cut this line 
                if (Y >= this.ScanlinesToCut & Y < (this._Scanlines + this.ScanlinesToCut))
                {
                    int liner = ((Y - this.ScanlinesToCut) * 1024) + (X * 2);
                    int liner2 = liner + 512;

                    bool usePack = false;

                    if (this.takingSnap)
                    {
                        if (tileX == 0 && tileY == 0)
                        {
                            this.log.WriteLine("Page = " + tilePage.ToString() + ", Offset = " + tileOffSet.ToString() + ", X = " + X.ToString() + ", Y = " + Y.ToString() + ", Color1 = " + color1.ToString() + ", Color2 = " + color2.ToString() + ", Color3 = " + color3.ToString());
                        }
                    }
                    if (this.hasHiResPack)
                    {
                        if (this.packData[tilePage][tileOffSet] != -1)
                        {
                            usePack = true;
                        }
                    }
                    unsafe{
                        int packID = -1;
                        TileData t;
                        if (usePack)
                        {
                            t = this.tdata[this.packData[tilePage][tileOffSet]];
                            for(int idx = 0; idx < t.BitmapP.Count; idx++){
                                if (t.BitmapP[idx].Color1 == color1 && t.BitmapP[idx].Color2 == color2 && t.BitmapP[idx].Color3 == color3) packID = idx;
                            }
                            if (packID == -1) packID = t.DefaultId;

                            if (packID != -1)
                            {
                                BitmapData b = this.bitmaps[t.BitmapP[packID].BitmapId].LockBits(new Rectangle(0, 0, this.bitmaps[t.BitmapP[packID].BitmapId].Width, this.bitmaps[t.BitmapP[packID].BitmapId].Height),
                                            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                                int* tpt = (int*)(b.Scan0) + (t.BitmapP[packID].Y + (tileY * 2)) * b.Width + t.BitmapP[packID].X + 14 - (tileX * 2);

                                if ((*tpt & 0xFF000000) != 0)
                                {
                                    if (hFlip)
                                    {
                                        if (vFlip) this.numPtr[liner2] = *tpt;
                                        else this.numPtr[liner] = *tpt;
                                    }
                                    else
                                    {
                                        if (vFlip) this.numPtr[liner2 + 1] = *tpt;
                                        else this.numPtr[liner + 1] = *tpt;
                                    }
                                }
                                tpt++;
                                if ((*tpt & 0xFF000000) != 0)
                                {
                                    if (hFlip)
                                    {
                                        if (vFlip) this.numPtr[liner2 + 1] = *tpt;
                                        else this.numPtr[liner + 1] = *tpt;
                                    }
                                    else
                                    {
                                        if (vFlip) this.numPtr[liner2] = *tpt;
                                        else this.numPtr[liner] = *tpt;
                                    }
                                }
                                tpt += b.Width;
                                if ((*tpt & 0xFF000000) != 0)
                                {
                                    if (hFlip)
                                    {
                                        if (vFlip) this.numPtr[liner + 1] = *tpt;
                                        else this.numPtr[liner2 + 1] = *tpt;
                                    }
                                    else
                                    {
                                        if (vFlip) this.numPtr[liner] = *tpt;
                                        else this.numPtr[liner2] = *tpt;
                                    }
                                }
                                tpt--;
                                if ((*tpt & 0xFF000000) != 0)
                                {
                                    if (hFlip)
                                    {
                                        if (vFlip) this.numPtr[liner] = *tpt;
                                        else this.numPtr[liner2] = *tpt;
                                    }
                                    else
                                    {
                                        if (vFlip) this.numPtr[liner + 1] = *tpt;
                                        else this.numPtr[liner2 + 1] = *tpt;
                                    }
                                }
                                this.bitmaps[t.BitmapP[packID].BitmapId].UnlockBits(b);
                            }
                        }
                        if (packID == -1)
                        {
                            if ((renColor % 4) != 0)
                            {
                                this.numPtr[liner] = Color;
                                liner++;
                                this.numPtr[liner] = Color;
                                this.numPtr[liner2] = Color;
                                liner2++;
                                this.numPtr[liner2] = Color;
                            }
                        }

                    }
                }
            }
        }

        public void RenderFrame()
        {
            if (this._CanRender & this._IsRendering)
            {
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
            if (this.highResSnap && this.takingSnap)
            {
                this.log.Flush();
                this.log.Close();
                this.saveSnap();
                this.highResSnap = false;
                this.takingSnap = false;
            }
        }
        public void TakeSnapshot(string SnapPath, string Format)
        {
            this.highResSnap = true;
            this.takingSnap = false;
            this.snapfile = SnapPath;
            this.snapformat = Format;
        }

        void saveSnap() {
            switch (this.snapformat)
            {
                case ".bmp":
                    this.bmp.Save(this.snapfile, ImageFormat.Bmp);
                    this._CanRender = true;
                    break;
                case ".gif":
                    this.bmp.Save(this.snapfile, ImageFormat.Gif);
                    this._CanRender = true;
                    break;
                case ".jpg":
                    this.bmp.Save(this.snapfile, ImageFormat.Jpeg);
                    this._CanRender = true;
                    break;
                case ".png":
                    this.bmp.Save(this.snapfile, ImageFormat.Png);
                    this._CanRender = true;
                    break;
                case ".tiff":
                    this.bmp.Save(this.snapfile, ImageFormat.Tiff);
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
        public void Dispose()
        {
            if (this.hasHiResPack) {
                for (int i = 0; i < this.bitmaps.Count; i++){
                    this.bitmaps[i].Dispose();
                }
            }
        }
    }
}
