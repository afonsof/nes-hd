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
            InitializeComponent();
            var header = new NesHeaderReader(RomPath);
            if (header.ValidRom)
            {
                textBox1_Name.Text = Path.GetFileNameWithoutExtension(RomPath);
                textBox1_prgs.Text = header.PrgRomPageCount.ToString();
                textBox2_Mapper.Text = header.MemoryMapper.ToString() + ", " + header.GetMapperName();
                textBox2_chr.Text = header.ChrRomPageCount.ToString();
                textBox3_mirroring.Text = header.VerticalMirroring ? "Vertical" : "Horizontal";
                checkBox1_four.Checked = header.FourScreenVRamLayout;
                checkBox1_saveram.Checked = header.SRamEnabled;
                checkBox2_trainer.Checked = header.TrainerPresent512;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}