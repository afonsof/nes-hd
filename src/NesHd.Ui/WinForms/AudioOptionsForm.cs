using System;
using System.Windows.Forms;

namespace NesHd.Ui.WinForms
{
    public partial class AudioOptionsForm : Form
    {
        public AudioOptionsForm()
        {
            InitializeComponent();
            checkBox1.Checked = Program.Settings.SoundEnabled;
            checkBox2.Checked = Program.Settings.Square1;
            checkBox3.Checked = Program.Settings.Square2;
            checkBox4.Checked = Program.Settings.Noize;
            checkBox5.Checked = Program.Settings.Triangle;
            checkBox6.Checked = Program.Settings.DMC;
            checkBox7.Checked = Program.Settings.VRC6Pulse1;
            checkBox8.Checked = Program.Settings.VRC6Pulse2;
            checkBox9.Checked = Program.Settings.VRC6Sawtooth;
            trackBar1.Value = Program.Settings.Volume;

            label1.Text = string.Format("{0} %", ((((100*(3000 - trackBar1.Value))/3000) - 200)*-1));
            radioButton_Stereo.Checked = Program.Settings.Stereo;
        }

        public bool Ok { get; private set; }

        private void Button1Click(object sender, EventArgs e)
        {
            Program.Settings.SoundEnabled = checkBox1.Checked;
            Program.Settings.Square1 = checkBox2.Checked;
            Program.Settings.Square2 = checkBox3.Checked;
            Program.Settings.Noize = checkBox4.Checked;
            Program.Settings.Triangle = checkBox5.Checked;
            Program.Settings.DMC = checkBox6.Checked;
            Program.Settings.VRC6Pulse1 = checkBox7.Checked;
            Program.Settings.VRC6Pulse2 = checkBox8.Checked;
            Program.Settings.VRC6Sawtooth = checkBox9.Checked;
            Program.Settings.Volume = trackBar1.Value;
            Program.Settings.Stereo = radioButton_Stereo.Checked;
            Program.Settings.Save();
            Ok = true;
            Close();
        }

        private void Button2Click(object sender, EventArgs e)
        {
            Ok = false;
            Close();
        }

        private void Button3Click(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
            checkBox3.Checked = true;
            checkBox4.Checked = true;
            checkBox5.Checked = true;
            checkBox6.Checked = true;
            checkBox7.Checked = true;
            checkBox8.Checked = true;
            checkBox9.Checked = true;
        }

        private void Button4Click(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
            checkBox7.Checked = false;
            checkBox8.Checked = false;
            checkBox9.Checked = false;
        }

        private void TrackBar1Scroll(object sender, EventArgs e)
        {
            label1.Text = string.Format("{0} %", ((((100*(3000 - trackBar1.Value))/3000) - 200)*-1));
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = checkBox1.Checked;
        }
    }
}