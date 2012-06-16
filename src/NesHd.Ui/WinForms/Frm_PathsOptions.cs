using System;
using System.Windows.Forms;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_PathsOptions : Form
    {
        public Frm_PathsOptions()
        {
            InitializeComponent();
            //Load the settings
            //snapshots folder
            textBox1_Snapshots.Text = Program.Settings.SnapshotsFolder;
            //State saves ..
            textBox1_States.Text = Program.Settings.StateFloder;
        }

        private void button1_Snapshots_Click(object sender, EventArgs e)
        {
            var fol = new FolderBrowserDialog();
            fol.Description = "Snapshots folder";
            fol.ShowNewFolderButton = true;
            fol.SelectedPath = Application.StartupPath;
            if (fol.ShowDialog(this) == DialogResult.OK)
            {
                textBox1_Snapshots.Text = fol.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Save
        private void button1_Click(object sender, EventArgs e)
        {
            Program.Settings.SnapshotsFolder = textBox1_Snapshots.Text;
            Program.Settings.StateFloder = textBox1_States.Text;
            Program.Settings.Save();
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1_Snapshots.Text = ".\\Snapshots\\";
            textBox1_States.Text = ".\\StateSaves\\";
        }

        private void button4_States_Click(object sender, EventArgs e)
        {
            var fol = new FolderBrowserDialog();
            fol.Description = "State saves folder";
            fol.ShowNewFolderButton = true;
            fol.SelectedPath = Application.StartupPath;
            if (fol.ShowDialog(this) == DialogResult.OK)
            {
                textBox1_States.Text = fol.SelectedPath;
            }
        }
    }
}