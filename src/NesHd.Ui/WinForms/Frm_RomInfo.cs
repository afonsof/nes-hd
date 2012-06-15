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

using NesHd.Core.Memory;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_RomInfo : Form
    {
        public Frm_RomInfo(string RomPath)
        {
            this.InitializeComponent();
            INESHeaderReader header=new INESHeaderReader (RomPath);
            if (header.validRom)
            {
                this.textBox1_Name.Text = Path.GetFileNameWithoutExtension(RomPath);
                this.textBox1_prgs.Text = header.PrgRomPageCount.ToString();
                this.textBox2_Mapper.Text = header.MemoryMapper.ToString() + ", " + header.GetMapperName();
                this.textBox2_chr.Text = header.ChrRomPageCount.ToString();
                this.textBox3_mirroring.Text = header.VerticalMirroring ? "Vertical" : "Horizontal";
                this.checkBox1_four.Checked = header.FourScreenVRAMLayout;
                this.checkBox1_saveram.Checked = header.SRamEnabled;
                this.checkBox2_trainer.Checked = header.TrainerPresent512;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
