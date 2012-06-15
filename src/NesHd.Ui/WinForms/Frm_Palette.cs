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

using NesHd.Core.Misc;
using NesHd.Core.PPU;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_Palette : Form
    {
        bool _Ok;
        public bool OK
        { get { return this._Ok; } }
        unsafe void ShowPalette(int[] PALETTE)
        {
            Graphics GR = this.panel1.CreateGraphics();
            GR.Clear(Color.Black);
            GR.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            int y = 0;
            int x = 0;
            int w = 18;
            int H = 18;
            for (int j = 0; j < PALETTE.Length; j++)
            {
                y = (j / 16) * H;
                x = (j * w) - (y * 16);
                Bitmap bmp = new Bitmap(w, H);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, H), 
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                int* numPtr = (int*)bmpData.Scan0;
                for (int i = 0; i < w * H; i++)
                {
                    numPtr[i] = PALETTE[j];
                }
                bmp.UnlockBits(bmpData);
                GR.DrawImage(bmp, x, y, w, H);
            }
        }
        public Frm_Palette()
        {
            this.InitializeComponent();
            //Load the settings
            if (Program.Settings.PaletteFormat == null)
                Program.Settings.PaletteFormat = new PaletteFormat();
            this.radioButton1_useInternal.Checked = Program.Settings.PaletteFormat.UseInternalPalette;
            this.radioButton2_useExternal.Checked = !Program.Settings.PaletteFormat.UseInternalPalette;
            this.groupBox2.Enabled = Program.Settings.PaletteFormat.UseInternalPalette;
            this.button1.Enabled = !Program.Settings.PaletteFormat.UseInternalPalette;
            this.textBox1.Enabled = !Program.Settings.PaletteFormat.UseInternalPalette;
            switch (Program.Settings.PaletteFormat.UseInternalPaletteMode)
            {
                case UseInternalPaletteMode.Auto:
                    this.radioButton3_auto.Checked = true;
                    this.radioButton4_pal.Checked = false;
                    this.radioButton5_ntsc.Checked = false;
                    break;
                case UseInternalPaletteMode.NTSC:
                    this.radioButton3_auto.Checked = false;
                    this.radioButton4_pal.Checked = false;
                    this.radioButton5_ntsc.Checked = true;
                    break;
                case UseInternalPaletteMode.PAL:
                    this.radioButton3_auto.Checked = false;
                    this.radioButton4_pal.Checked = true;
                    this.radioButton5_ntsc.Checked = false;
                    break;
            }
            this.textBox1.Text = Program.Settings.PaletteFormat.ExternalPalettePath;
        }
        private void radioButton1_useInternal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton3_auto.Checked)
                this.ShowPalette(new int[64]);
            this.groupBox2.Enabled = this.radioButton1_useInternal.Checked;
            this.button1.Enabled = !this.radioButton1_useInternal.Checked;
            this.textBox1.Enabled = !this.radioButton1_useInternal.Checked;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Open palette file";
            op.Filter = "Palette file (*.pal)|*.pal;*.PAL";
            if (op.ShowDialog(this) == DialogResult.OK)
            {
                if (Paletter.LoadPalette(op.FileName) != null)
                {
                    this.textBox1.Text = op.FileName;
                    this.ShowPalette(Paletter.LoadPalette(op.FileName));
                }
                else
                {
                    MessageBox.Show("Can't load this palette file !!");
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this._Ok = false;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Program.Settings.PaletteFormat.UseInternalPalette = this.radioButton1_useInternal.Checked;
            Program.Settings.PaletteFormat.ExternalPalettePath = this.textBox1.Text;
            if (this.radioButton3_auto.Checked)
                Program.Settings.PaletteFormat.UseInternalPaletteMode = UseInternalPaletteMode.Auto;
            else if (this.radioButton4_pal.Checked)
                Program.Settings.PaletteFormat.UseInternalPaletteMode = UseInternalPaletteMode.PAL;
            else if (this.radioButton5_ntsc.Checked)
                Program.Settings.PaletteFormat.UseInternalPaletteMode = UseInternalPaletteMode.NTSC;
            Program.Settings.Save();
            this._Ok = true;
            this.Close();
        }

        private void radioButton4_pal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton4_pal.Checked)
                this.ShowPalette(Paletter.PALPalette);
        }
        private void radioButton5_ntsc_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton5_ntsc.Checked)
                this.ShowPalette(Paletter.NTSCPalette);
        }
        private void radioButton2_useExternal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton2_useExternal.Checked)
                if (Paletter.LoadPalette(this.textBox1.Text) != null)
                    this.ShowPalette(Paletter.LoadPalette(this.textBox1.Text));
        }
        private void radioButton3_auto_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton3_auto.Checked)
                this.ShowPalette(new int[64]);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Program.Settings.PaletteFormat = new PaletteFormat();
            //RELoad the settings
            this.radioButton1_useInternal.Checked = Program.Settings.PaletteFormat.UseInternalPalette;
            this.radioButton2_useExternal.Checked = !Program.Settings.PaletteFormat.UseInternalPalette;
            this.groupBox2.Enabled = Program.Settings.PaletteFormat.UseInternalPalette;
            this.button1.Enabled = !Program.Settings.PaletteFormat.UseInternalPalette;
            this.textBox1.Enabled = !Program.Settings.PaletteFormat.UseInternalPalette;
            switch (Program.Settings.PaletteFormat.UseInternalPaletteMode)
            {
                case UseInternalPaletteMode.Auto:
                    this.radioButton3_auto.Checked = true;
                    this.radioButton4_pal.Checked = false;
                    this.radioButton5_ntsc.Checked = false;
                    break;
                case UseInternalPaletteMode.NTSC:
                    this.radioButton3_auto.Checked = false;
                    this.radioButton4_pal.Checked = false;
                    this.radioButton5_ntsc.Checked = true;
                    break;
                case UseInternalPaletteMode.PAL:
                    this.radioButton3_auto.Checked = false;
                    this.radioButton4_pal.Checked = true;
                    this.radioButton5_ntsc.Checked = false;
                    break;
            }
            this.textBox1.Text = Program.Settings.PaletteFormat.ExternalPalettePath;
        }
    }
}
