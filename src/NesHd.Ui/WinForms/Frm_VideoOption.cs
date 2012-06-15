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

using NesHd.Core;
using NesHd.Core.Output.Video;
using NesHd.Core.Output.Video.Devices;
using NesHd.Core.Output.Video.Devices.SlimDX;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_VideoOption : Form
    {
        bool _Ok = false;
        public bool OK
        { get { return this._Ok; } }
        public Frm_VideoOption()
        {
            this.InitializeComponent();
            //Load the settings
            this.comboBox1_Tv.SelectedItem = Program.Settings.TV.ToString();
            this.comboBox1_Size.SelectedItem = Program.Settings.Size;
            this.comboBox1_VideoMode.SelectedItem = Program.Settings.GFXDevice.ToString();
            this.checkBox1.Checked = Program.Settings.Fullscreen;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //save 
        private void button1_Click(object sender, EventArgs e)
        {
            //TV format
            switch (this.comboBox1_Tv.SelectedItem.ToString())
            {
                case "NTSC":
                    Program.Settings.TV = TVFORMAT.NTSC;
                    break;
                case "PAL":
                    Program.Settings.TV = TVFORMAT.PAL;
                    break;
            }
            //Size
            Program.Settings.Size = this.comboBox1_Size.SelectedItem.ToString();
            //Output device
            switch (this.comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    Program.Settings.GFXDevice = GraphicDevices.SlimDx;
                    break;
                case "GDI":
                    Program.Settings.GFXDevice = GraphicDevices.Gdi;
                    break;
                case "GDIHiRes":
                    Program.Settings.GFXDevice = GraphicDevices.GdiHiRes;
                    break;
            }
            //Fullscreen
            Program.Settings.Fullscreen = this.checkBox1.Checked;
            //SAVE
            Program.Settings.Save();
            this.Close();
            this._Ok = true;
        }

        private void comboBox1_VideoMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    VideoSlimDx sl = new VideoSlimDx(TVFORMAT.NTSC, Program.Form_Main.panel1);
                    this.richTextBox1_DrawerDescription.Text = sl.Description;
                    break;
                case "GDI":
                    VideoGdi gd = new VideoGdi(TVFORMAT.NTSC, Program.Form_Main.panel1);
                    this.richTextBox1_DrawerDescription.Text = gd.Description;
                    break;
                case "GDIHiRes":
                    VideoGdiHiRes gdh = new VideoGdiHiRes(TVFORMAT.NTSC, Program.Form_Main.panel1, "", 0);
                    this.richTextBox1_DrawerDescription.Text = gdh.Description;
                    break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            switch (this.comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    VideoSlimDx sl = new VideoSlimDx(TVFORMAT.NTSC, Program.Form_Main.panel1);
                    sl.ChangeSettings();
                    break;
                case "GDI":
                    VideoGdi gd = new VideoGdi(TVFORMAT.NTSC, Program.Form_Main.panel1);
                    gd.ChangeSettings();
                    break;
                case "GDIHiRes":
                    VideoGdi gdh = new VideoGdi(TVFORMAT.NTSC, Program.Form_Main.panel1);
                    gdh.ChangeSettings();
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //TV format
            this.comboBox1_Tv.SelectedItem = "NTSC";
            //Size
            this.comboBox1_Size.SelectedItem = "Stretch"; ;
            //Output device
            this.comboBox1_VideoMode.SelectedItem = "SlimDX";
            //Fullscreen
            this.checkBox1.Checked = true;
        }
    }
}