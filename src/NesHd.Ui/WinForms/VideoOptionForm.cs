﻿using System;
using System.Windows.Forms;
using NesHd.Core;
using NesHd.Core.Output.Video;
using NesHd.Core.Output.Video.Devices;
using NesHd.Core.Output.Video.Devices.SlimDX;

namespace NesHd.Ui.WinForms
{
    public partial class VideoOptionForm : Form
    {
        private byte _multi;

        public VideoOptionForm(byte multi)
        {
            InitializeComponent();
            //Load the settings
            comboBox1_Tv.SelectedItem = Program.Settings.TV.ToString();
            comboBox1_Size.SelectedItem = Program.Settings.Size;
            comboBox1_VideoMode.SelectedItem = Program.Settings.GFXDevice.ToString();
            checkBox1.Checked = Program.Settings.Fullscreen;
            _multi = multi;
        }

        public bool Ok { get; private set; }

        private void Button2Click(object sender, EventArgs e)
        {
            Close();
        }

        //save 
        private void Button1Click(object sender, EventArgs e)
        {
            //TV format
            switch (comboBox1_Tv.SelectedItem.ToString())
            {
                case "NTSC":
                    Program.Settings.TV = TvFormat.Ntsc;
                    break;
                case "PAL":
                    Program.Settings.TV = TvFormat.Pal;
                    break;
            }
            //Size
            Program.Settings.Size = comboBox1_Size.SelectedItem.ToString();
            //Output device
            switch (comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    Program.Settings.GFXDevice = GraphicDevices.SlimDx;
                    break;
                case "GDI":
                    Program.Settings.GFXDevice = GraphicDevices.Gdi;
                    break;
                case "GDIHiRes":
                    Program.Settings.GFXDevice = GraphicDevices.GdiHiRes;
                    break;
            }
            //Fullscreen
            Program.Settings.Fullscreen = checkBox1.Checked;
            //SAVE
            Program.Settings.Save();
            Close();
            Ok = true;
        }

        private void ComboBox1VideoModeSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    var sl = new VideoSlimDx(TvFormat.Ntsc, Program.MainForm.panel1, _multi);
                    richTextBox1_DrawerDescription.Text = sl.Description;
                    break;
                case "GDI":
                    var gd = new VideoGdi(TvFormat.Ntsc, Program.MainForm.panel1);
                    richTextBox1_DrawerDescription.Text = gd.Description;
                    break;
                case "GDIHiRes":
                    var gdh = new VideoGdiHiRes(TvFormat.Ntsc, Program.MainForm.panel1, "", 0);
                    richTextBox1_DrawerDescription.Text = gdh.Description;
                    break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            switch (comboBox1_VideoMode.SelectedItem.ToString())
            {
                case "SlimDX":
                    var sl = new VideoSlimDx(TvFormat.Ntsc, Program.MainForm.panel1, _multi);
                    sl.ChangeSettings();
                    break;
                case "GDI":
                    var gd = new VideoGdi(TvFormat.Ntsc, Program.MainForm.panel1);
                    gd.ChangeSettings();
                    break;
                case "GDIHiRes":
                    var gdh = new VideoGdi(TvFormat.Ntsc, Program.MainForm.panel1);
                    gdh.ChangeSettings();
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //TV format
            comboBox1_Tv.SelectedItem = "NTSC";
            //Size
            comboBox1_Size.SelectedItem = "Stretch";
            ;
            //Output device
            comboBox1_VideoMode.SelectedItem = "SlimDX";
            //Fullscreen
            checkBox1.Checked = true;
        }
    }
}