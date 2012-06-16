using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NesHd.Ui.WinForms
{
    public partial class ArchivesForm : Form
    {
        public ArchivesForm(IEnumerable<string> files)
        {
            InitializeComponent();
            foreach (string t in files)
            {
                var extension = Path.GetExtension(t);
                if (extension != null && extension.ToLower() == ".nes")
                {
                    listBox1.Items.Add(t);
                }
            }
            listBox1.SelectedIndex = (listBox1.Items.Count > 0) ? 0 : -1;
        }

        public bool Ok { get; private set; }

        public string SelectedRom
        {
            get { return listBox1.SelectedItem.ToString(); }
        }

        private void ListBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = listBox1.SelectedIndex >= 0;
        }

        private void Button1Click(object sender, EventArgs e)
        {
            Ok = true;
            Close();
        }

        private void Button2Click(object sender, EventArgs e)
        {
            Ok = false;
            Close();
        }

        private void ListBox1MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            Ok = true;
            Close();
        }
    }
}