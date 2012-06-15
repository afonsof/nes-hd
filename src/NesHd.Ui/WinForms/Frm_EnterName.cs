using System;
using System.Windows.Forms;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_EnterName : Form
    {
        bool _Ok = false;
        /// <summary>
        /// Get if the user pressed the Ok button
        /// </summary>
        public bool OK
        { get { return this._Ok; } }
        /// <summary>
        /// Get the name entered by the user
        /// </summary>
        public string NameEntered
        { get { return this.textBox1.Text; } }
        public Frm_EnterName()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this._Ok = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this._Ok = false;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.button1.Enabled = this.textBox1.Text.Length > 0;
        }
    }
}
