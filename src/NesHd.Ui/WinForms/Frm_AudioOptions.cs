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
using System.Windows.Forms;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_AudioOptions : Form
    {
        bool _Ok;
        public bool OK
        { get { return this._Ok; } }
        public Frm_AudioOptions()
        {
            this.InitializeComponent();
            this.checkBox1.Checked = Program.Settings.SoundEnabled;
            this.checkBox2.Checked = Program.Settings.Square1;
            this.checkBox3.Checked = Program.Settings.Square2;
            this.checkBox4.Checked = Program.Settings.Noize;
            this.checkBox5.Checked = Program.Settings.Triangle;
            this.checkBox6.Checked = Program.Settings.DMC;
            this.checkBox7.Checked = Program.Settings.VRC6Pulse1;
            this.checkBox8.Checked = Program.Settings.VRC6Pulse2;
            this.checkBox9.Checked = Program.Settings.VRC6Sawtooth;
            this.trackBar1.Value = Program.Settings.Volume;

            this.label1.Text = ((((100 * (3000 - this.trackBar1.Value)) / 3000) - 200) * -1).ToString() + " %";
            this.radioButton_Stereo.Checked = Program.Settings.Stereo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.Settings.SoundEnabled = this.checkBox1.Checked;
            Program.Settings.Square1 = this.checkBox2.Checked;
            Program.Settings.Square2 = this.checkBox3.Checked;
            Program.Settings.Noize = this.checkBox4.Checked;
            Program.Settings.Triangle = this.checkBox5.Checked;
            Program.Settings.DMC = this.checkBox6.Checked;
            Program.Settings.VRC6Pulse1 = this.checkBox7.Checked;
            Program.Settings.VRC6Pulse2 = this.checkBox8.Checked;
            Program.Settings.VRC6Sawtooth = this.checkBox9.Checked;
            Program.Settings.Volume = this.trackBar1.Value;
            Program.Settings.Stereo = this.radioButton_Stereo.Checked;
            Program.Settings.Save();
            this._Ok = true;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this._Ok = false;
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.checkBox2.Checked = true;
            this.checkBox3.Checked = true;
            this.checkBox4.Checked = true;
            this.checkBox5.Checked = true;
            this.checkBox6.Checked = true;
            this.checkBox7.Checked = true;
            this.checkBox8.Checked = true;
            this.checkBox9.Checked = true;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;
            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;
            this.checkBox6.Checked = false;
            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;
            this.checkBox9.Checked = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.label1.Text = ((((100 * (3000 - this.trackBar1.Value)) / 3000) - 200) * -1).ToString() + " %";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox1.Enabled = this.checkBox1.Checked;
        }
    }
}
