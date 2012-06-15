﻿/*
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
using System.Diagnostics;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_About : Form
    {
        public Frm_About(string Version)
        {
            this.InitializeComponent();
            this.label2_ver.Text = "Version "+ Version;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:ahdsoftwares@hotmail.com");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://sourceforge.net/projects/NesHd.Ui/");
        }
    }
}
