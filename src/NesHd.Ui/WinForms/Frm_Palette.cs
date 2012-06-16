using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NesHd.Core.Misc;
using NesHd.Core.PPU;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_Palette : Form
    {
        private bool _Ok;

        public Frm_Palette()
        {
            InitializeComponent();
            //Load the settings
            if (Program.Settings.PaletteFormat == null)
                Program.Settings.PaletteFormat = new PaletteFormat();
            radioButton1_useInternal.Checked = Program.Settings.PaletteFormat.UseInternalPalette;
            radioButton2_useExternal.Checked = !Program.Settings.PaletteFormat.UseInternalPalette;
            groupBox2.Enabled = Program.Settings.PaletteFormat.UseInternalPalette;
            button1.Enabled = !Program.Settings.PaletteFormat.UseInternalPalette;
            textBox1.Enabled = !Program.Settings.PaletteFormat.UseInternalPalette;
            switch (Program.Settings.PaletteFormat.UseInternalPaletteMode)
            {
                case UseInternalPaletteMode.Auto:
                    radioButton3_auto.Checked = true;
                    radioButton4_pal.Checked = false;
                    radioButton5_ntsc.Checked = false;
                    break;
                case UseInternalPaletteMode.Ntsc:
                    radioButton3_auto.Checked = false;
                    radioButton4_pal.Checked = false;
                    radioButton5_ntsc.Checked = true;
                    break;
                case UseInternalPaletteMode.Pal:
                    radioButton3_auto.Checked = false;
                    radioButton4_pal.Checked = true;
                    radioButton5_ntsc.Checked = false;
                    break;
            }
            textBox1.Text = Program.Settings.PaletteFormat.ExternalPalettePath;
        }

        public bool OK
        {
            get { return _Ok; }
        }

        private unsafe void ShowPalette(int[] PALETTE)
        {
            var GR = panel1.CreateGraphics();
            GR.Clear(Color.Black);
            GR.InterpolationMode = InterpolationMode.NearestNeighbor;
            var y = 0;
            var x = 0;
            var w = 18;
            var H = 18;
            for (var j = 0; j < PALETTE.Length; j++)
            {
                y = (j/16)*H;
                x = (j*w) - (y*16);
                var bmp = new Bitmap(w, H);
                var bmpData = bmp.LockBits(new Rectangle(0, 0, w, H),
                                           ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                var numPtr = (int*) bmpData.Scan0;
                for (var i = 0; i < w*H; i++)
                {
                    numPtr[i] = PALETTE[j];
                }
                bmp.UnlockBits(bmpData);
                GR.DrawImage(bmp, x, y, w, H);
            }
        }

        private void radioButton1_useInternal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3_auto.Checked)
                ShowPalette(new int[64]);
            groupBox2.Enabled = radioButton1_useInternal.Checked;
            button1.Enabled = !radioButton1_useInternal.Checked;
            textBox1.Enabled = !radioButton1_useInternal.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Open palette file";
            op.Filter = "Palette file (*.pal)|*.pal;*.PAL";
            if (op.ShowDialog(this) == DialogResult.OK)
            {
                if (Paletter.LoadPalette(op.FileName) != null)
                {
                    textBox1.Text = op.FileName;
                    ShowPalette(Paletter.LoadPalette(op.FileName));
                }
                else
                {
                    MessageBox.Show("Can't load this palette file !!");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _Ok = false;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.Settings.PaletteFormat.UseInternalPalette = radioButton1_useInternal.Checked;
            Program.Settings.PaletteFormat.ExternalPalettePath = textBox1.Text;
            if (radioButton3_auto.Checked)
                Program.Settings.PaletteFormat.UseInternalPaletteMode = UseInternalPaletteMode.Auto;
            else if (radioButton4_pal.Checked)
                Program.Settings.PaletteFormat.UseInternalPaletteMode = UseInternalPaletteMode.Pal;
            else if (radioButton5_ntsc.Checked)
                Program.Settings.PaletteFormat.UseInternalPaletteMode = UseInternalPaletteMode.Ntsc;
            Program.Settings.Save();
            _Ok = true;
            Close();
        }

        private void radioButton4_pal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4_pal.Checked)
                ShowPalette(Paletter.PalPalette);
        }

        private void radioButton5_ntsc_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5_ntsc.Checked)
                ShowPalette(Paletter.NtscPalette);
        }

        private void radioButton2_useExternal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2_useExternal.Checked)
                if (Paletter.LoadPalette(textBox1.Text) != null)
                    ShowPalette(Paletter.LoadPalette(textBox1.Text));
        }

        private void radioButton3_auto_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3_auto.Checked)
                ShowPalette(new int[64]);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Program.Settings.PaletteFormat = new PaletteFormat();
            //RELoad the settings
            radioButton1_useInternal.Checked = Program.Settings.PaletteFormat.UseInternalPalette;
            radioButton2_useExternal.Checked = !Program.Settings.PaletteFormat.UseInternalPalette;
            groupBox2.Enabled = Program.Settings.PaletteFormat.UseInternalPalette;
            button1.Enabled = !Program.Settings.PaletteFormat.UseInternalPalette;
            textBox1.Enabled = !Program.Settings.PaletteFormat.UseInternalPalette;
            switch (Program.Settings.PaletteFormat.UseInternalPaletteMode)
            {
                case UseInternalPaletteMode.Auto:
                    radioButton3_auto.Checked = true;
                    radioButton4_pal.Checked = false;
                    radioButton5_ntsc.Checked = false;
                    break;
                case UseInternalPaletteMode.Ntsc:
                    radioButton3_auto.Checked = false;
                    radioButton4_pal.Checked = false;
                    radioButton5_ntsc.Checked = true;
                    break;
                case UseInternalPaletteMode.Pal:
                    radioButton3_auto.Checked = false;
                    radioButton4_pal.Checked = true;
                    radioButton5_ntsc.Checked = false;
                    break;
            }
            textBox1.Text = Program.Settings.PaletteFormat.ExternalPalettePath;
        }
    }
}