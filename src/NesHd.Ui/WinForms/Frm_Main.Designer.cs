/*
This file is part of My Nes
A Nintendo Entertainment System Emulator.

 Copyright © 2009 - 2010 Ala Hadid (AHD)

My Nes is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

My Nes is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/
namespace NesHd.Ui.WinForms
{
    partial class Frm_Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSRAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.romInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadSateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stateSlotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot6ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot7ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot9ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.recordAudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.machineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.togglePauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.softResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hardResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.noLimiterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runDebuggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runDebuggerAtStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.showMenuAndStatusStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.commandLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutNesHdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel4_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel2_slot = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timer_FPS = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.supportedMappersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.machineToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(463, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openRomToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.browserToolStripMenuItem,
            this.toolStripSeparator5,
            this.saveSRAMToolStripMenuItem,
            this.toolStripSeparator11,
            this.romInfoToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadSateToolStripMenuItem,
            this.saveStateToolStripMenuItem,
            this.stateSlotToolStripMenuItem,
            this.toolStripSeparator8,
            this.recordAudioToolStripMenuItem,
            this.toolStripSeparator9,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openRomToolStripMenuItem
            // 
            this.openRomToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openRomToolStripMenuItem.Image")));
            this.openRomToolStripMenuItem.Name = "openRomToolStripMenuItem";
            this.openRomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openRomToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.openRomToolStripMenuItem.Text = "&Open rom";
            this.openRomToolStripMenuItem.Click += new System.EventHandler(this.openRomToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.recentToolStripMenuItem.Text = "Recent";
            this.recentToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recentToolStripMenuItem_DropDownItemClicked);
            // 
            // browserToolStripMenuItem
            // 
            this.browserToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("browserToolStripMenuItem.Image")));
            this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
            this.browserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.browserToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.browserToolStripMenuItem.Text = "&Browser";
            this.browserToolStripMenuItem.Click += new System.EventHandler(this.browserToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(170, 6);
            // 
            // saveSRAMToolStripMenuItem
            // 
            this.saveSRAMToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveSRAMToolStripMenuItem.Image")));
            this.saveSRAMToolStripMenuItem.Name = "saveSRAMToolStripMenuItem";
            this.saveSRAMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveSRAMToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.saveSRAMToolStripMenuItem.Text = "&Save SRAM";
            this.saveSRAMToolStripMenuItem.Click += new System.EventHandler(this.saveSRAMToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(170, 6);
            // 
            // romInfoToolStripMenuItem
            // 
            this.romInfoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("romInfoToolStripMenuItem.Image")));
            this.romInfoToolStripMenuItem.Name = "romInfoToolStripMenuItem";
            this.romInfoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.romInfoToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.romInfoToolStripMenuItem.Text = "&Rom info";
            this.romInfoToolStripMenuItem.Click += new System.EventHandler(this.romInfoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(170, 6);
            // 
            // loadSateToolStripMenuItem
            // 
            this.loadSateToolStripMenuItem.Name = "loadSateToolStripMenuItem";
            this.loadSateToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.loadSateToolStripMenuItem.Text = "&Load sate      F9";
            this.loadSateToolStripMenuItem.Click += new System.EventHandler(this.loadSateToolStripMenuItem_Click);
            // 
            // saveStateToolStripMenuItem
            // 
            this.saveStateToolStripMenuItem.Name = "saveStateToolStripMenuItem";
            this.saveStateToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.saveStateToolStripMenuItem.Text = "&Save state     F6";
            this.saveStateToolStripMenuItem.Click += new System.EventHandler(this.saveStateToolStripMenuItem_Click);
            // 
            // stateSlotToolStripMenuItem
            // 
            this.stateSlotToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slot0ToolStripMenuItem,
            this.slot1ToolStripMenuItem,
            this.slot2ToolStripMenuItem,
            this.slot3ToolStripMenuItem,
            this.slot4ToolStripMenuItem,
            this.slot5ToolStripMenuItem,
            this.slot6ToolStripMenuItem,
            this.slot7ToolStripMenuItem,
            this.slot8ToolStripMenuItem,
            this.slot9ToolStripMenuItem});
            this.stateSlotToolStripMenuItem.Name = "stateSlotToolStripMenuItem";
            this.stateSlotToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.stateSlotToolStripMenuItem.Text = "State slot";
            this.stateSlotToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.stateSlotToolStripMenuItem_DropDownItemClicked);
            // 
            // slot0ToolStripMenuItem
            // 
            this.slot0ToolStripMenuItem.Checked = true;
            this.slot0ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.slot0ToolStripMenuItem.Name = "slot0ToolStripMenuItem";
            this.slot0ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot0ToolStripMenuItem.Text = "Slot 0";
            // 
            // slot1ToolStripMenuItem
            // 
            this.slot1ToolStripMenuItem.Name = "slot1ToolStripMenuItem";
            this.slot1ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot1ToolStripMenuItem.Text = "Slot 1";
            // 
            // slot2ToolStripMenuItem
            // 
            this.slot2ToolStripMenuItem.Name = "slot2ToolStripMenuItem";
            this.slot2ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot2ToolStripMenuItem.Text = "Slot 2";
            // 
            // slot3ToolStripMenuItem
            // 
            this.slot3ToolStripMenuItem.Name = "slot3ToolStripMenuItem";
            this.slot3ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot3ToolStripMenuItem.Text = "Slot 3";
            // 
            // slot4ToolStripMenuItem
            // 
            this.slot4ToolStripMenuItem.Name = "slot4ToolStripMenuItem";
            this.slot4ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot4ToolStripMenuItem.Text = "Slot 4";
            // 
            // slot5ToolStripMenuItem
            // 
            this.slot5ToolStripMenuItem.Name = "slot5ToolStripMenuItem";
            this.slot5ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot5ToolStripMenuItem.Text = "Slot 5";
            // 
            // slot6ToolStripMenuItem
            // 
            this.slot6ToolStripMenuItem.Name = "slot6ToolStripMenuItem";
            this.slot6ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot6ToolStripMenuItem.Text = "Slot 6";
            // 
            // slot7ToolStripMenuItem
            // 
            this.slot7ToolStripMenuItem.Name = "slot7ToolStripMenuItem";
            this.slot7ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot7ToolStripMenuItem.Text = "Slot 7";
            // 
            // slot8ToolStripMenuItem
            // 
            this.slot8ToolStripMenuItem.Name = "slot8ToolStripMenuItem";
            this.slot8ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot8ToolStripMenuItem.Text = "Slot 8";
            // 
            // slot9ToolStripMenuItem
            // 
            this.slot9ToolStripMenuItem.Name = "slot9ToolStripMenuItem";
            this.slot9ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.slot9ToolStripMenuItem.Text = "Slot 9";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(170, 6);
            // 
            // recordAudioToolStripMenuItem
            // 
            this.recordAudioToolStripMenuItem.Image = global::NesHd.Ui.Properties.Resources.Record;
            this.recordAudioToolStripMenuItem.Name = "recordAudioToolStripMenuItem";
            this.recordAudioToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.recordAudioToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.recordAudioToolStripMenuItem.Text = "&Record audio";
            this.recordAudioToolStripMenuItem.Click += new System.EventHandler(this.recordAudioToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(170, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // machineToolStripMenuItem
            // 
            this.machineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.togglePauseToolStripMenuItem,
            this.toolStripSeparator3,
            this.softResetToolStripMenuItem,
            this.hardResetToolStripMenuItem,
            this.toolStripSeparator4,
            this.noLimiterToolStripMenuItem});
            this.machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            this.machineToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.machineToolStripMenuItem.Text = "&Machine";
            // 
            // togglePauseToolStripMenuItem
            // 
            this.togglePauseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("togglePauseToolStripMenuItem.Image")));
            this.togglePauseToolStripMenuItem.Name = "togglePauseToolStripMenuItem";
            this.togglePauseToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.togglePauseToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.togglePauseToolStripMenuItem.Text = "&Toggle pause";
            this.togglePauseToolStripMenuItem.Click += new System.EventHandler(this.togglePauseToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(161, 6);
            // 
            // softResetToolStripMenuItem
            // 
            this.softResetToolStripMenuItem.Name = "softResetToolStripMenuItem";
            this.softResetToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.softResetToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.softResetToolStripMenuItem.Text = "&Soft reset";
            this.softResetToolStripMenuItem.Click += new System.EventHandler(this.softResetToolStripMenuItem_Click);
            // 
            // hardResetToolStripMenuItem
            // 
            this.hardResetToolStripMenuItem.Name = "hardResetToolStripMenuItem";
            this.hardResetToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.hardResetToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.hardResetToolStripMenuItem.Text = "&Hard reset";
            this.hardResetToolStripMenuItem.Click += new System.EventHandler(this.hardResetToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(161, 6);
            // 
            // noLimiterToolStripMenuItem
            // 
            this.noLimiterToolStripMenuItem.CheckOnClick = true;
            this.noLimiterToolStripMenuItem.Name = "noLimiterToolStripMenuItem";
            this.noLimiterToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.noLimiterToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.noLimiterToolStripMenuItem.Text = "&No limiter";
            this.noLimiterToolStripMenuItem.CheckedChanged += new System.EventHandler(this.noLimiterToolStripMenuItem_CheckedChanged);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runDebuggerToolStripMenuItem,
            this.runDebuggerAtStartupToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "&Debug";
            // 
            // runDebuggerToolStripMenuItem
            // 
            this.runDebuggerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("runDebuggerToolStripMenuItem.Image")));
            this.runDebuggerToolStripMenuItem.Name = "runDebuggerToolStripMenuItem";
            this.runDebuggerToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.runDebuggerToolStripMenuItem.Text = "&Run console";
            this.runDebuggerToolStripMenuItem.Click += new System.EventHandler(this.runDebuggerToolStripMenuItem_Click);
            // 
            // runDebuggerAtStartupToolStripMenuItem
            // 
            this.runDebuggerAtStartupToolStripMenuItem.CheckOnClick = true;
            this.runDebuggerAtStartupToolStripMenuItem.Name = "runDebuggerAtStartupToolStripMenuItem";
            this.runDebuggerAtStartupToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.runDebuggerAtStartupToolStripMenuItem.Text = "&Run console at startup";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripMenuItem,
            this.pathsToolStripMenuItem,
            this.videoToolStripMenuItem,
            this.paletteToolStripMenuItem,
            this.audioToolStripMenuItem,
            this.controlsToolStripMenuItem,
            this.toolStripSeparator6,
            this.showMenuAndStatusStripToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("generalToolStripMenuItem.Image")));
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.generalToolStripMenuItem.Text = "&General";
            this.generalToolStripMenuItem.Click += new System.EventHandler(this.generalToolStripMenuItem_Click);
            // 
            // pathsToolStripMenuItem
            // 
            this.pathsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pathsToolStripMenuItem.Image")));
            this.pathsToolStripMenuItem.Name = "pathsToolStripMenuItem";
            this.pathsToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.pathsToolStripMenuItem.Text = "&Paths";
            this.pathsToolStripMenuItem.Click += new System.EventHandler(this.pathsToolStripMenuItem_Click);
            // 
            // videoToolStripMenuItem
            // 
            this.videoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("videoToolStripMenuItem.Image")));
            this.videoToolStripMenuItem.Name = "videoToolStripMenuItem";
            this.videoToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.videoToolStripMenuItem.Text = "&Video";
            this.videoToolStripMenuItem.Click += new System.EventHandler(this.videoToolStripMenuItem_Click);
            // 
            // paletteToolStripMenuItem
            // 
            this.paletteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("paletteToolStripMenuItem.Image")));
            this.paletteToolStripMenuItem.Name = "paletteToolStripMenuItem";
            this.paletteToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.paletteToolStripMenuItem.Text = "&Palette";
            this.paletteToolStripMenuItem.Click += new System.EventHandler(this.paletteToolStripMenuItem_Click);
            // 
            // audioToolStripMenuItem
            // 
            this.audioToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("audioToolStripMenuItem.Image")));
            this.audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            this.audioToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.audioToolStripMenuItem.Text = "&Audio";
            this.audioToolStripMenuItem.Click += new System.EventHandler(this.audioToolStripMenuItem_Click);
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("controlsToolStripMenuItem.Image")));
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.controlsToolStripMenuItem.Text = "&Controls";
            this.controlsToolStripMenuItem.Click += new System.EventHandler(this.controlsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(242, 6);
            // 
            // showMenuAndStatusStripToolStripMenuItem
            // 
            this.showMenuAndStatusStripToolStripMenuItem.CheckOnClick = true;
            this.showMenuAndStatusStripToolStripMenuItem.Name = "showMenuAndStatusStripToolStripMenuItem";
            this.showMenuAndStatusStripToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.showMenuAndStatusStripToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.showMenuAndStatusStripToolStripMenuItem.Text = "Show menu and status strip";
            this.showMenuAndStatusStripToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showMenuAndStatusStripToolStripMenuItem_CheckedChanged);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.indexToolStripMenuItem,
            this.toolStripSeparator10,
            this.commandLinesToolStripMenuItem,
            this.supportedMappersToolStripMenuItem,
            this.toolStripSeparator2,
            this.aboutNesHdToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripMenuItem1.Image")));
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(178, 22);
            this.helpToolStripMenuItem1.Text = "&Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("indexToolStripMenuItem.Image")));
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            this.indexToolStripMenuItem.Click += new System.EventHandler(this.indexToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(175, 6);
            // 
            // commandLinesToolStripMenuItem
            // 
            this.commandLinesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("commandLinesToolStripMenuItem.Image")));
            this.commandLinesToolStripMenuItem.Name = "commandLinesToolStripMenuItem";
            this.commandLinesToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.commandLinesToolStripMenuItem.Text = "Command lines";
            this.commandLinesToolStripMenuItem.Click += new System.EventHandler(this.commandLinesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // aboutNesHdToolStripMenuItem
            // 
            this.aboutNesHdToolStripMenuItem.Image = global::NesHd.Ui.Properties.Resources.NESDoc;
            this.aboutNesHdToolStripMenuItem.Name = "aboutNesHdToolStripMenuItem";
            this.aboutNesHdToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.aboutNesHdToolStripMenuItem.Text = "&About My Nes";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel1,
            this.toolStripStatusLabel1,
            this.StatusLabel4_status,
            this.toolStripStatusLabel3,
            this.StatusLabel2_slot,
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 384);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(463, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "FPS : ";
            // 
            // StatusLabel1
            // 
            this.StatusLabel1.AutoSize = false;
            this.StatusLabel1.Name = "StatusLabel1";
            this.StatusLabel1.Size = new System.Drawing.Size(55, 17);
            this.StatusLabel1.Text = "FPS : 0";
            this.StatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.StatusLabel1.ToolTipText = "Frames per second";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Enabled = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel1.Text = "|";
            // 
            // StatusLabel4_status
            // 
            this.StatusLabel4_status.AutoSize = false;
            this.StatusLabel4_status.AutoToolTip = true;
            this.StatusLabel4_status.BackColor = System.Drawing.Color.Red;
            this.StatusLabel4_status.Name = "StatusLabel4_status";
            this.StatusLabel4_status.Size = new System.Drawing.Size(50, 17);
            this.StatusLabel4_status.Text = "  OFF  ";
            this.StatusLabel4_status.ToolTipText = "The status of the NES (ON, OFF, PAUSED)";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Enabled = false;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel3.Text = "|";
            // 
            // StatusLabel2_slot
            // 
            this.StatusLabel2_slot.Name = "StatusLabel2_slot";
            this.StatusLabel2_slot.Size = new System.Drawing.Size(73, 17);
            this.StatusLabel2_slot.Text = "State Slot [0]";
            this.StatusLabel2_slot.ToolTipText = "Currently selected state sloet";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(463, 360);
            this.panel1.TabIndex = 2;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // timer_FPS
            // 
            this.timer_FPS.Interval = 1000;
            this.timer_FPS.Tick += new System.EventHandler(this.timer_FPS_Tick);
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // supportedMappersToolStripMenuItem
            // 
            this.supportedMappersToolStripMenuItem.Name = "supportedMappersToolStripMenuItem";
            this.supportedMappersToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.supportedMappersToolStripMenuItem.Text = "Supported mappers";
            this.supportedMappersToolStripMenuItem.Click += new System.EventHandler(this.supportedMappersToolStripMenuItem_Click);
            // 
            // Frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 406);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "My Nes";
            this.Deactivate += new System.EventHandler(this.Frm_Main_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Frm_Main_FormClosed);
            this.ResizeBegin += new System.EventHandler(this.Frm_Main_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Frm_Main_ResizeEnd);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Frm_Main_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRomToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem aboutNesHdToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runDebuggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runDebuggerAtStartupToolStripMenuItem;
        private System.Windows.Forms.Timer timer_FPS;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem audioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem machineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem togglePauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem hardResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem softResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noLimiterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pathsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stateSlotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot0ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot6ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot7ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot8ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot9ToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel2_slot;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel4_status;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recordAudioToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commandLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem romInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem saveSRAMToolStripMenuItem;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem showMenuAndStatusStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supportedMappersToolStripMenuItem;
    }
}

