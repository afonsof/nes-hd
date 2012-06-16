using System;
using System.Windows.Forms;
using SlimDX.Direct3D9;

namespace NesHd.Core.Output.Video.Devices.SlimDX
{
    public partial class Frm_SlimDXOptions : Form
    {
        private readonly VideoSlimDx _SLIM;
        private readonly VideoModeSettings sett = new VideoModeSettings();

        public Frm_SlimDXOptions(VideoSlimDx SLIM)
        {
            _SLIM = SLIM;
            InitializeComponent();
            sett.Reload();
            comboBox1.Items.Clear();
            for (var i = 0; i < _SLIM.D3D.Adapters[0].GetDisplayModes(Format.X8R8G8B8).Count; i++)
            {
                comboBox1.Items.Add(_SLIM.D3D.Adapters[0].GetDisplayModes(Format.X8R8G8B8)[i].Width + " x " +
                                    _SLIM.D3D.Adapters[0].GetDisplayModes(Format.X8R8G8B8)[i].Height + " " +
                                    _SLIM.D3D.Adapters[0].GetDisplayModes(Format.X8R8G8B8)[i].RefreshRate + " Hz");
            }
            comboBox1.SelectedIndex = sett.SlimDX_ResMode;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sett.SlimDX_ResMode = comboBox1.SelectedIndex;
            sett.Save();
            _SLIM.ApplaySettings(sett);
            Close();
        }
    }
}