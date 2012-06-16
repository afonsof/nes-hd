using System;
using System.Windows.Forms;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_GeneralOptions : Form
    {
        public Frm_GeneralOptions()
        {
            InitializeComponent();
            switch (Program.Settings.SnapshotFormat)
            {
                case ".bmp":
                    radioButton1.Checked = true;
                    break;
                case ".jpg":
                    radioButton2.Checked = true;
                    break;
                case ".gif":
                    radioButton3.Checked = true;
                    break;
                case ".png":
                    radioButton4.Checked = true;
                    break;
                case ".tiff":
                    radioButton5.Checked = true;
                    break;
            }
            checkBox1_sramsave.Checked = Program.Settings.AutoSaveSRAM;
            checkBox1_pause.Checked = Program.Settings.PauseWhenFocusLost;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            checkBox1_pause.Checked = true;
            checkBox1_sramsave.Checked = true;
        }

        //save
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Program.Settings.SnapshotFormat = ".bmp";
            }
            if (radioButton2.Checked)
            {
                Program.Settings.SnapshotFormat = ".jpg";
            }
            if (radioButton3.Checked)
            {
                Program.Settings.SnapshotFormat = ".gif";
            }
            if (radioButton4.Checked)
            {
                Program.Settings.SnapshotFormat = ".png";
            }
            if (radioButton5.Checked)
            {
                Program.Settings.SnapshotFormat = ".tiff";
            }
            Program.Settings.AutoSaveSRAM = checkBox1_sramsave.Checked;
            Program.Settings.PauseWhenFocusLost = checkBox1_pause.Checked;
            Program.Settings.Save();
            Close();
        }
    }
}