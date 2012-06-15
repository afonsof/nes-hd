using System;
using System.Windows.Forms;

using SlimDX.Direct3D9;

namespace NesHd.Core.Output.Video.Devices.SlimDX
{
    public partial class Frm_SlimDXOptions : Form
    {
        VideoSlimDx _SLIM;
        VideoModeSettings sett = new VideoModeSettings();
        public Frm_SlimDXOptions(VideoSlimDx SLIM)
        {
            this._SLIM = SLIM;
            this.InitializeComponent();
            this.sett.Reload();
            this.comboBox1.Items.Clear();
            for (int i = 0; i < this._SLIM.d3d.Adapters[0].GetDisplayModes(Format.X8R8G8B8).Count; i++)
            {
                this.comboBox1.Items.Add(this._SLIM.d3d.Adapters[0].GetDisplayModes(Format.X8R8G8B8)[i].Width + " x " + this._SLIM.d3d.Adapters[0].GetDisplayModes(Format.X8R8G8B8)[i].Height + " " + this._SLIM.d3d.Adapters[0].GetDisplayModes(Format.X8R8G8B8)[i].RefreshRate + " Hz");
            }
            this.comboBox1.SelectedIndex = this.sett.SlimDX_ResMode;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.sett.SlimDX_ResMode = this.comboBox1.SelectedIndex;
            this.sett.Save();
            this._SLIM.ApplaySettings(this.sett);
            this.Close();
        }
    }
}
