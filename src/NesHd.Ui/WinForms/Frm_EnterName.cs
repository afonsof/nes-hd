using System;
using System.Windows.Forms;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_EnterName : Form
    {
        private bool _Ok;

        public Frm_EnterName()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Get if the user pressed the Ok button
        /// </summary>
        public bool OK
        {
            get { return _Ok; }
        }

        /// <summary>
        /// Get the name entered by the user
        /// </summary>
        public string NameEntered
        {
            get { return textBox1.Text; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _Ok = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _Ok = false;
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = textBox1.Text.Length > 0;
        }
    }
}