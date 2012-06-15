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
using System.IO;
using System.Windows.Forms;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_Archives : Form
    {
        bool _OK;
        public bool OK
        { get { return this._OK; } }
        public string SelectedRom
        { get { return this.listBox1.SelectedItem.ToString(); } }
        public Frm_Archives(string[] FILES)
        {
            this.InitializeComponent();
            for (int i = 0; i < FILES.Length; i++)
            {
                if (Path.GetExtension(FILES[i]).ToLower() == ".nes")
                {
                    this.listBox1.Items.Add(FILES[i]);
                }
            }
            this.listBox1.SelectedIndex = (this.listBox1.Items.Count > 0) ? 0 : -1;
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.button1.Enabled = this.listBox1.SelectedIndex >= 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this._OK = true;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this._OK = false;
            this.Close();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listBox1.SelectedIndex < 0)
                return;
            this._OK = true;
            this.Close();
        }
    }
    public class Rom
    {
        string _Name = "";
        string _Path = "";
        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }
        public string Path
        {
            get { return this._Path; }
            set { this._Path = value; }
        }
    }
}
