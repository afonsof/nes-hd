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
    public partial class Frm_GeneralOptions : Form
    {
        public Frm_GeneralOptions()
        {
            this.InitializeComponent();
            switch (Program.Settings.SnapshotFormat)
            {
                case ".bmp":
                    this.radioButton1.Checked = true;
                    break;
                case ".jpg":
                    this.radioButton2.Checked = true;
                    break;
                case ".gif":
                    this.radioButton3.Checked = true;
                    break;
                case ".png":
                    this.radioButton4.Checked = true;
                    break;
                case ".tiff":
                    this.radioButton5.Checked = true;
                    break;
            }
            this.checkBox1_sramsave.Checked = Program.Settings.AutoSaveSRAM;
            this.checkBox1_pause.Checked = Program.Settings.PauseWhenFocusLost;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.radioButton1.Checked = true;
            this.checkBox1_pause.Checked = true;
            this.checkBox1_sramsave.Checked = true;
        }
        //save
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            { Program.Settings.SnapshotFormat = ".bmp"; }
            if (this.radioButton2.Checked)
            { Program.Settings.SnapshotFormat = ".jpg"; }
            if (this.radioButton3.Checked)
            { Program.Settings.SnapshotFormat = ".gif"; }
            if (this.radioButton4.Checked)
            { Program.Settings.SnapshotFormat = ".png"; }
            if (this.radioButton5.Checked)
            { Program.Settings.SnapshotFormat = ".tiff"; }
            Program.Settings.AutoSaveSRAM = this.checkBox1_sramsave.Checked;
            Program.Settings.PauseWhenFocusLost = this.checkBox1_pause.Checked;
            Program.Settings.Save();
            this.Close();
        }
    }
}
