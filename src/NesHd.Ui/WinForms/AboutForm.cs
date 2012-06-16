using System;
using System.Diagnostics;
using System.Windows.Forms;
using NesHd.Ui.Properties;

namespace NesHd.Ui.WinForms
{
    public partial class AboutForm : Form
    {
        public AboutForm(string version)
        {
            InitializeComponent();
            label2_ver.Text = string.Format("{0} {1}", Resources.Version, version);
        }

        private void Button1Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:ahdsoftwares@hotmail.com");
        }

        private void LinkLabel2LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://sourceforge.net/projects/NesHd.Ui/");
        }
    }
}