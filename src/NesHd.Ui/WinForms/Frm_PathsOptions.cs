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
    public partial class Frm_PathsOptions : Form
    {
        public Frm_PathsOptions()
        {
            this.InitializeComponent();
            //Load the settings
            //snapshots folder
            this.textBox1_Snapshots.Text = Program.Settings.SnapshotsFolder;
            //State saves ..
            this.textBox1_States.Text = Program.Settings.StateFloder;
        }
        private void button1_Snapshots_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fol = new FolderBrowserDialog();
            fol.Description = "Snapshots folder";
            fol.ShowNewFolderButton = true;
            fol.SelectedPath = Application.StartupPath;
            if (fol.ShowDialog(this) == DialogResult.OK)
            {
                this.textBox1_Snapshots.Text = fol.SelectedPath;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Save
        private void button1_Click(object sender, EventArgs e)
        {
            Program.Settings.SnapshotsFolder = this.textBox1_Snapshots.Text;
            Program.Settings.StateFloder = this.textBox1_States.Text;
            Program.Settings.Save();
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox1_Snapshots.Text = ".\\Snapshots\\";
            this.textBox1_States.Text = ".\\StateSaves\\";
        }
        private void button4_States_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fol = new FolderBrowserDialog();
            fol.Description = "State saves folder";
            fol.ShowNewFolderButton = true;
            fol.SelectedPath = Application.StartupPath;
            if (fol.ShowDialog(this) == DialogResult.OK)
            {
               this.textBox1_States.Text = fol.SelectedPath;
            }
        }
    }
}
