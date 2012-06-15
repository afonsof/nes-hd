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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using NesHd.Core;
using NesHd.Core.Debugger;
using NesHd.Core.Input;
using NesHd.Core.Memory;
using NesHd.Core.Misc;
using NesHd.Core.Output.Audio.Devices;
using NesHd.Core.Output.Video;
using NesHd.Core.Output.Video.Devices;
using NesHd.Core.Output.Video.Devices.SlimDX;
using NesHd.Core.PPU;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_Main : Form
    {
        NesEngine _Nes;
        Thread gameThread;
        ThreadStart myThreadCreator;
        Frm_Debugger Debugger;
        int StateIndex = 0;
        public Frm_Main(string[] Args)
        {
            InitializeComponent();
            Debug.WriteLine(this, "Main window Initialized OK.", DebugStatus.Cool);
            this.DoCommandLines(Args);
            this.Activated += new EventHandler(this.Frm_Main_Activated);
        }
        public NesEngine NES
        { get { return this._Nes; } }
        bool StartActions = true;
        SevenZip.SevenZipExtractor EXTRACTOR;
        #region Functions
        /// <summary>
        /// Load the settings (applay them)
        /// </summary>
        public void LoadSettings()
        {
            Debug.WriteLine(null, "Loading settings ...", DebugStatus.None);
            this.Location = new Point(Program.Settings.Win_X, Program.Settings.Win_Y);
            this.Size = new Size(Program.Settings.Win_W, Program.Settings.Win_H);
            runDebuggerAtStartupToolStripMenuItem.Checked = Program.Settings.ShowDebugger;
            noLimiterToolStripMenuItem.Checked = Program.Settings.NoLimiter;
            showMenuAndStatusStripToolStripMenuItem.Checked = Program.Settings.ShowMenuAndStatus;
            this.showMenuAndStatusStripToolStripMenuItem_CheckedChanged(this, null);
            this.RefreshRecents();
            Debug.WriteLine(this, "Selected TV Format : " + Program.Settings.TV.ToString(), DebugStatus.None);
            Debug.WriteLine(this, "Selected GFX : " + Program.Settings.GFXDevice.ToString(), DebugStatus.None);
            Debug.WriteLine(this, "Settings OK !", DebugStatus.Cool);
            Debug.WriteSeparateLine(this, DebugStatus.None);
        }
        public void SaveSettings()
        {
            Program.Settings.Win_X = this.Location.X;
            Program.Settings.Win_Y = this.Location.Y;
            Program.Settings.Win_W = this.Width;
            Program.Settings.Win_H = this.Height;
            Program.Settings.ShowDebugger = runDebuggerAtStartupToolStripMenuItem.Checked;
            Program.Settings.ShowMenuAndStatus = showMenuAndStatusStripToolStripMenuItem.Checked;
            Program.Settings.Save();
        }
        void AddRecent(string FilePath)
        {
            if (Program.Settings.Recents == null)
                Program.Settings.Recents = new System.Collections.Specialized.StringCollection();
            for (int i = 0; i < Program.Settings.Recents.Count; i++)
            {
                if (FilePath == Program.Settings.Recents[i])
                { Program.Settings.Recents.Remove(FilePath); }
            }
            Program.Settings.Recents.Insert(0, FilePath);
            //limit to 9 elements
            if (Program.Settings.Recents.Count > 9)
                Program.Settings.Recents.RemoveAt(9);
        }
        void RefreshRecents()
        {
            if (Program.Settings.Recents == null)
                Program.Settings.Recents = new System.Collections.Specialized.StringCollection();
            recentToolStripMenuItem.DropDownItems.Clear();
            int i = 1;
            foreach (string Recc in Program.Settings.Recents)
            {
                ToolStripMenuItem IT = new ToolStripMenuItem();
                IT.Text = Path.GetFileNameWithoutExtension(Recc);
                switch (i)//This for the recent item shortcut key
                //So that user can press CTRL + item No
                {
                    case 1:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D1)));
                        break;
                    case 2:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D2)));
                        break;
                    case 3:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D3)));
                        break;
                    case 4:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D4)));
                        break;
                    case 5:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D5)));
                        break;
                    case 6:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D6)));
                        break;
                    case 7:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D7)));
                        break;
                    case 8:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D8)));
                        break;
                    case 9:
                        IT.ShortcutKeys = ((System.Windows.Forms.Keys)
                            ((System.Windows.Forms.Keys.Control |
                            System.Windows.Forms.Keys.D9)));
                        break;
                }
                recentToolStripMenuItem.DropDownItems.Add(IT);
                i++;
            }
        }
        public void OpenRom(string RomPath)
        {
            if (File.Exists(RomPath))
            {
                #region Check if archive
                if (Path.GetExtension(RomPath).ToLower() != ".nes")
                {
                    try
                    {
                        this.EXTRACTOR = new SevenZip.SevenZipExtractor(RomPath);
                    }
                    catch
                    {

                    }
                    if (this.EXTRACTOR.ArchiveFileData.Count == 1)
                    {
                        if (this.EXTRACTOR.ArchiveFileData[0].FileName.Substring(this.EXTRACTOR.ArchiveFileData[0].FileName.Length - 4, 4).ToLower() == ".nes")
                        {
                            this.EXTRACTOR.ExtractArchive(Path.GetTempPath());
                            RomPath = Path.GetTempPath() + this.EXTRACTOR.ArchiveFileData[0].FileName;
                        }
                    }
                    else
                    {
                        List<string> filenames = new List<string>();
                        foreach (SevenZip.ArchiveFileInfo file in this.EXTRACTOR.ArchiveFileData)
                        {
                            filenames.Add(file.FileName);
                        }
                        Frm_Archives ar = new Frm_Archives(filenames.ToArray());
                        ar.ShowDialog(this);
                        if (ar.OK)
                        {
                            string[] fil = { ar.SelectedRom };
                            this.EXTRACTOR.ExtractFiles(Path.GetTempPath(), fil);
                            RomPath = Path.GetTempPath() + ar.SelectedRom;
                        }
                        else
                        { return; }
                    }
                }
                #endregion
                #region Check the rom
                INESHeaderReader Header = new INESHeaderReader(RomPath);
                if (Header.validRom)
                {
                    if (!Header.SupportedMapper())
                    {
                        MessageBox.Show("Can't load rom:\n" + RomPath +
                         "\n\nUNSUPPORTED MAPPER # " +
                         Header.MemoryMapper);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Can't load rom:\n" + RomPath +
                        "\n\nRom is damaged or not an INES format file");
                    return;
                }
                #endregion
                //Exit current thread
                if (this._Nes != null)
                {
                    this._Nes.ShutDown();
                    this._Nes = null;
                }
                if (this.gameThread != null)
                { this.gameThread.Abort(); }
                //Start new nes !!
                if (Program.Settings.PaletteFormat == null)
                    Program.Settings.PaletteFormat = new PaletteFormat();
                this._Nes = new NesEngine(Program.Settings.TV, Program.Settings.PaletteFormat);
                this._Nes.PauseToggle += new EventHandler<EventArgs>(this._Nes_PauseToggle);
                if (this._Nes.LoadRom(RomPath))
                {
                    #region Setup input
                    InputManager Manager = new InputManager(this.Handle);
                    Joypad Joy1 = new Joypad(Manager);
                    Joy1.A.Input = Program.Settings.CurrentControlProfile.Player1_A;
                    Joy1.B.Input = Program.Settings.CurrentControlProfile.Player1_B;
                    Joy1.Up.Input = Program.Settings.CurrentControlProfile.Player1_Up;
                    Joy1.Down.Input = Program.Settings.CurrentControlProfile.Player1_Down;
                    Joy1.Left.Input = Program.Settings.CurrentControlProfile.Player1_Left;
                    Joy1.Right.Input = Program.Settings.CurrentControlProfile.Player1_Right;
                    Joy1.Select.Input = Program.Settings.CurrentControlProfile.Player1_Select;
                    Joy1.Start.Input = Program.Settings.CurrentControlProfile.Player1_Start;
                    Joypad Joy2 = new Joypad(Manager);
                    Joy2.A.Input = Program.Settings.CurrentControlProfile.Player2_A;
                    Joy2.B.Input = Program.Settings.CurrentControlProfile.Player2_B;
                    Joy2.Up.Input = Program.Settings.CurrentControlProfile.Player2_Up;
                    Joy2.Down.Input = Program.Settings.CurrentControlProfile.Player2_Down;
                    Joy2.Left.Input = Program.Settings.CurrentControlProfile.Player2_Left;
                    Joy2.Right.Input = Program.Settings.CurrentControlProfile.Player2_Right;
                    Joy2.Select.Input = Program.Settings.CurrentControlProfile.Player2_Select;
                    Joy2.Start.Input = Program.Settings.CurrentControlProfile.Player2_Start;
                    this._Nes.SetupInput(Manager, Joy1, Joy2);
                    #endregion
                    #region Output
                    //Set the size
                    switch (Program.Settings.Size.ToLower())
                    {
                        case "x1":
                            this.Size = new Size(265, 305);
                            this.FormBorderStyle = FormBorderStyle.FixedDialog;
                            statusStrip1.SizingGrip = false;
                            break;
                        case "x2":
                            this.Size = new Size(521, 529);
                            this.FormBorderStyle = FormBorderStyle.FixedDialog;
                            statusStrip1.SizingGrip = false;
                            break;
                        case "stretch":
                            this.FormBorderStyle = FormBorderStyle.Sizable;
                            statusStrip1.SizingGrip = true;
                            break;
                    }
                    //The output devices
                    SoundDeviceGeneral16 mon = new SoundDeviceGeneral16(statusStrip1);
                    mon.Stereo = Program.Settings.Stereo;
                    switch (Program.Settings.GFXDevice)
                    {
                        case GraphicDevices.Gdi:
                            VideoGdi gdi = new VideoGdi(Program.Settings.TV, panel1);
                            this._Nes.SetupOutput(gdi, mon);
                            break;
                        case GraphicDevices.GdiHiRes:
                            VideoGdiHiRes gdih = new VideoGdiHiRes(Program.Settings.TV, panel1, this._Nes.MEMORY.MAP.Cartridge.RomPath, this._Nes.MEMORY.MAP.Cartridge.CHR_PAGES);
                            this._Nes.SetupOutput(gdih, mon);
                            break;
                        case GraphicDevices.SlimDx:
                            VideoSlimDx SL = new VideoSlimDx(Program.Settings.TV, panel1);
                            this._Nes.SetupOutput(SL, mon);
                            break;
                        default:
                            Program.Settings.GFXDevice = GraphicDevices.SlimDx;
                            VideoSlimDx SL1 = new VideoSlimDx(Program.Settings.TV, panel1);
                            this._Nes.SetupOutput(SL1, mon);
                            break;
                    }
                    if (this._Nes.PPU.OutputDevice.SupportFullScreen)
                        this._Nes.PPU.OutputDevice.FullScreen = Program.Settings.Fullscreen;
                    //Audio
                    this._Nes.SoundEnabled = Program.Settings.SoundEnabled;
                    this._Nes.APU.Square1Enabled = Program.Settings.Square1;
                    this._Nes.APU.Square2Enabled = Program.Settings.Square2;
                    this._Nes.APU.DMCEnabled = Program.Settings.DMC;
                    this._Nes.APU.TriangleEnabled = Program.Settings.Triangle;
                    this._Nes.APU.NoiseEnabled = Program.Settings.Noize;
                    this._Nes.APU.VRC6P1Enabled = Program.Settings.VRC6Pulse1;
                    this._Nes.APU.VRC6P2Enabled = Program.Settings.VRC6Pulse2;
                    this._Nes.APU.VRC6SawToothEnabled = Program.Settings.VRC6Sawtooth;
                    this._Nes.APU.SetVolume(Program.Settings.Volume);
                    #endregion
                    #region Misc
                    this._Nes.PPU.NoLimiter = Program.Settings.NoLimiter;
                    this._Nes.AutoSaveSRAM = Program.Settings.AutoSaveSRAM;
                    #endregion
                    //Launch
                    this.myThreadCreator = new ThreadStart(this._Nes.Run);
                    this.gameThread = new Thread(this.myThreadCreator);
                    this.gameThread.Start();
                    timer_FPS.Start();
                    StatusLabel4_status.BackColor = Color.Green;
                    StatusLabel4_status.Text = "ON";
                    //Add to the recent
                    this.AddRecent(RomPath);
                    this.RefreshRecents();
                    //Set the name
                    this.Text = "My Nes - " + Path.GetFileNameWithoutExtension(RomPath);
                }
                else
                {
                    MessageBox.Show("Can't load rom:\n" + RomPath +
                        "\n\nRom is damaged or not an INES format file !!");
                    if (this._Nes != null)
                    {
                        this._Nes.ShutDown();
                        this._Nes = null;
                    }
                    if (this.gameThread != null)
                    { this.gameThread.Abort(); }
                    return;
                }
            }
        }
        void TakeSnapshot()
        {
            if (this._Nes == null)
                return;
            //If there's no rom, get out !!
            if (!File.Exists(this._Nes.MEMORY.MAP.Cartridge.RomPath))
            { return; }
            this._Nes.Pause();
            Directory.CreateDirectory(Path.GetFullPath(Program.Settings.SnapshotsFolder));
            int i = 1;
            while (i != 0)
            {
                if (!File.Exists(Path.GetFullPath(Program.Settings.SnapshotsFolder) + "\\"
                    + Path.GetFileNameWithoutExtension(this._Nes.MEMORY.MAP.Cartridge.RomPath) + "_" +
                    i.ToString() + Program.Settings.SnapshotFormat))
                {
                    this._Nes.PPU.OutputDevice.TakeSnapshot(Path.GetFullPath(Program.Settings.SnapshotsFolder) + "\\"
                        + Path.GetFileNameWithoutExtension(this._Nes.MEMORY.MAP.Cartridge.RomPath) + "_" +
                        i.ToString() + Program.Settings.SnapshotFormat,
                       Program.Settings.SnapshotFormat);
                    break;
                }
                i++;
            }
            this._Nes.Resume();
        }
        void SaveState()
        {
            if (this._Nes == null)
                return;
            Directory.CreateDirectory(Path.GetFullPath(Program.Settings.StateFloder));
            State ST = new State(this._Nes);
            if (ST.SaveState(Path.GetFullPath(Program.Settings.StateFloder)
                 + "\\" + Path.GetFileNameWithoutExtension(this._Nes.MEMORY.MAP.Cartridge.RomPath) + "_" + this.StateIndex.ToString() + ".st"))
            {
                this.WriteStatus("STATE SAVED !!");
            }
            else
            {
                this.WriteStatus("CAN'T SAVE !!!!!??");
            }
        }
        void LoadState()
        {
            if (this._Nes == null)
                return;
            State ST = new State(this._Nes);
            if (ST.LoadState(Path.GetFullPath(Program.Settings.StateFloder)
                 + "\\" + Path.GetFileNameWithoutExtension(this._Nes.MEMORY.MAP.Cartridge.RomPath) + "_" + this.StateIndex.ToString() + ".st"))
            {
                this.WriteStatus("STATE LOADED !!");
            }
            else
            {
                this.WriteStatus("NO STATE FOUND IN SLOT " + this.StateIndex.ToString());
            }
        }
        void WriteStatus(string TEXT)
        {
            StatusLabel.Text = TEXT;
            timer1.Start();
        }
        public void ShowDialogs()
        {
            if (!this.StartActions)
                return;
            if (Program.Settings.ShowDebugger)
            {
                this.Debugger = new Frm_Debugger();
                this.Debugger.Show();
            }
            if (Program.Settings.ShowBrowser)
            {
                Frm_Browser BRO = new Frm_Browser(this);
                BRO.Show();
            }
        }
        void DoCommandLines(string[] Args)
        {
            //Get the command lines
            if (Args != null)
            {
                if (Args.Length > 0)
                {
                    this.StartActions = false;
                    //First one must be the rom path, so :
                    try
                    {
                        if (File.Exists(Args[0]))
                        { this.OpenRom(Args[0]); }
                    }
                    catch { MessageBox.Show("Rom path expacted !!"); return; }
                    //Let's see what the user wants My Nes to do :
                    for (int i = 1/*start from 1 'cause we've already used the args[0]*/;
                        i < Args.Length; i++)
                    {
                        switch (Args[i].ToLower())
                        {
                            case "-cm":
                                MessageBox.Show("List of the available command lines:\n" +
                                    "====================================\n" +
                                    "\n" +
                                    "* -cm : show this list !!\n" +
                                    "* -ls x : load the state from slot number x (x = slot number from 0 to 9)\n" +
                                    "* -ss x : select the state slot number x (x = slot number from 0 to 9)\n" +
                                    "* -pal : select the PAL region\n" +
                                    "* -ntsc : select the NTSC region\n" +
                                    "* -st x : Enable / Disable no limiter (x=0 disable, x=1 enable)\n" +
                                    "* -s x : switch the size (x=x1 use size X1 '256x240', x=x2 use size X2 '512x480', x=str use Stretch)\n" +
                                    "* -r <WavePath> : record the audio wave into <WavePath>\n" +
                                    "* -p <PaletteFilePath> : load the palette <PaletteFilePath> and use it\n" +
                                    "* -w_size w h : resize the window (W=width, h=height)\n" +
                                    "* -w_pos x y : move the window into a specific location (x=X coordinate, y=Y coordinate)\n" +
                                    "* -w_max : maximize the window\n" +
                                    "* -w_min : minimize the window");
                                break;
                            case "-w_min":
                                this.WindowState = FormWindowState.Minimized;
                                break;
                            case "-w_max":
                                this.WindowState = FormWindowState.Maximized;
                                break;
                            case "-w_size":
                                try
                                {
                                    //We expact the next "arg" must be the size
                                    i++;
                                    int W = Convert.ToInt32(Args[i]);
                                    i++;
                                    int H = Convert.ToInt32(Args[i]);
                                    if (this.FormBorderStyle == FormBorderStyle.Sizable)
                                        this.Size = new Size(W, H);
                                }
                                catch
                                { return; }
                                break;
                            case "-w_pos":
                                try
                                {
                                    //We expact the next "arg" must be the coordinates
                                    i++;
                                    int X = Convert.ToInt32(Args[i]);
                                    i++;
                                    int Y = Convert.ToInt32(Args[i]);
                                    this.Location = new Point(X, Y);
                                }
                                catch
                                { return; }
                                break;
                            case "-p":
                                try
                                {
                                    //We expact the next "arg" must be the palette path
                                    i++;
                                    if (File.Exists(Args[i]))
                                        if (Paletter.LoadPalette(Args[i]) != null)
                                            this._Nes.PPU.PALETTE = Paletter.LoadPalette(Args[i]);
                                }
                                catch { return; }
                                break;
                            case "-r":
                                try
                                {
                                    //We expact the next "arg" must be the wav path
                                    i++;
                                    if (this._Nes != null)
                                    {
                                        this._Nes.Pause();
                                        this._Nes.APU.RECODER.Record(Path.GetFullPath(Args[i]), Program.Settings.Stereo);
                                        recordAudioToolStripMenuItem.Text = "&Stop recording";
                                        this._Nes.Resume();
                                    }
                                }
                                catch { return; }
                                break;
                            case "-s":
                                try
                                {
                                    //We expact the next "arg" must be the size mode
                                    i++;
                                    if (Args[i] == "x1")
                                    {
                                        this._Nes.Pause();
                                        this.Size = new Size(265, 305);
                                        this.FormBorderStyle = FormBorderStyle.FixedDialog;
                                        statusStrip1.SizingGrip = false;
                                        this._Nes.Resume();
                                    }
                                    if (Args[i] == "x2")
                                    {
                                        this._Nes.Pause();
                                        this.Size = new Size(265, 305);
                                        this.FormBorderStyle = FormBorderStyle.FixedDialog;
                                        statusStrip1.SizingGrip = false;
                                        this._Nes.Resume();
                                    }
                                    if (Args[i] == "str")
                                    {
                                        this.FormBorderStyle = FormBorderStyle.Sizable;
                                        statusStrip1.SizingGrip = true;
                                    }
                                }
                                catch { return; }
                                break;
                            case "-st":
                                try
                                {
                                    //We expact the next "arg" must be 0 or 1
                                    i++;
                                    if (Args[i] == "0")
                                    {
                                        if (this._Nes != null)
                                            this._Nes.PPU.NoLimiter = false;
                                        this.WriteStatus("No limiter disabled !!");
                                    }
                                    else if (Args[i] == "1")
                                    {
                                        if (this._Nes != null)
                                            this._Nes.PPU.NoLimiter = true;
                                        this.WriteStatus("No limiter enabled !!");
                                    }
                                    else
                                    { return; }

                                }
                                catch { return; }
                                break;
                            case "-ntsc":
                                if (this._Nes != null)
                                {
                                    this._Nes.SetupTV(TVFORMAT.NTSC, Program.Settings.PaletteFormat);
                                }
                                break;
                            case "-pal":
                                if (this._Nes != null)
                                {
                                    this._Nes.SetupTV(TVFORMAT.PAL, Program.Settings.PaletteFormat);
                                }
                                break;
                            case "-ls":
                                try
                                {
                                    //We expact the next "arg" must be the state number
                                    i++;
                                    if (Args[i] == "0")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot0ToolStripMenuItem));
                                    if (Args[i] == "1")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot1ToolStripMenuItem));
                                    if (Args[i] == "2")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot2ToolStripMenuItem));
                                    if (Args[i] == "3")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot3ToolStripMenuItem));
                                    if (Args[i] == "4")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot4ToolStripMenuItem));
                                    if (Args[i] == "5")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot5ToolStripMenuItem));
                                    if (Args[i] == "6")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot6ToolStripMenuItem));
                                    if (Args[i] == "7")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot7ToolStripMenuItem));
                                    if (Args[i] == "8")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot8ToolStripMenuItem));
                                    if (Args[i] == "9")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot9ToolStripMenuItem));
                                    this.LoadState();
                                }
                                catch
                                {
                                    MessageBox.Show("State # expacted !!"); return;
                                }
                                break;
                            case "-ss":
                                try
                                {
                                    //We expact the next "arg" must be the state number
                                    i++;
                                    if (Args[i] == "0")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot0ToolStripMenuItem));
                                    if (Args[i] == "1")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot1ToolStripMenuItem));
                                    if (Args[i] == "2")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot2ToolStripMenuItem));
                                    if (Args[i] == "3")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot3ToolStripMenuItem));
                                    if (Args[i] == "4")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot4ToolStripMenuItem));
                                    if (Args[i] == "5")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot5ToolStripMenuItem));
                                    if (Args[i] == "6")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot6ToolStripMenuItem));
                                    if (Args[i] == "7")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot7ToolStripMenuItem));
                                    if (Args[i] == "8")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot8ToolStripMenuItem));
                                    if (Args[i] == "9")
                                        this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot9ToolStripMenuItem));
                                }
                                catch
                                {
                                    MessageBox.Show("State # expacted !!"); return;
                                }
                                break;
                        }
                    }
                }
            }
        }
        void ToggleFullscreen()
        {
            if (this._Nes != null)
            {
                this._Nes.Pause();
                Program.Settings.Fullscreen = !Program.Settings.Fullscreen;
                if (this._Nes.PPU.OutputDevice.SupportFullScreen)
                {
                    this._Nes.PPU.OutputDevice.FullScreen = Program.Settings.Fullscreen;
                    if (!Program.Settings.Fullscreen)
                    {
                        //restore the old status after the back
                        //from Fullscreen.
                        base.Size = this.MaximumSize;
                        this.Location = new Point(Program.Settings.Win_X, Program.Settings.Win_Y);
                        this.Size = new Size(Program.Settings.Win_W, Program.Settings.Win_H);
                    }
                    else
                    {
                        //save the window status to restore when
                        //we back from Fullscreen.
                        Program.Settings.Win_X = this.Location.X;
                        Program.Settings.Win_Y = this.Location.Y;
                        Program.Settings.Win_W = this.Width;
                        Program.Settings.Win_H = this.Height;
                    }
                }
                this._Nes.Resume();
            }
        }
        #endregion
        void _Nes_PauseToggle(object sender, EventArgs e)
        {
            if (this._Nes != null)
            {
                if (this._Nes.CPU.ON)
                {
                    if (this._Nes.CPU.Pause)
                    {
                        StatusLabel4_status.BackColor = Color.Orange;
                        StatusLabel4_status.Text = "PAUSED";
                    }
                    else
                    {
                        StatusLabel4_status.BackColor = Color.Green;
                        StatusLabel4_status.Text = "ON";
                    }
                }
                else
                {
                    StatusLabel4_status.BackColor = Color.Red;
                    StatusLabel4_status.Text = "OFF";
                }
            }
        }
        private void Frm_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer_FPS.Stop();
            if (this._Nes != null)
            {
                this._Nes.ShutDown();
            }
            if (this.gameThread != null)
            {
                this.gameThread.Abort();
                this._Nes = null;
            }
            if (this.WindowState == FormWindowState.Maximized | this.WindowState == FormWindowState.Minimized)
            { this.WindowState = FormWindowState.Normal; }
            this.SaveSettings();
            if (this.Debugger != null)
                this.Debugger.SaveSettings();
        }
        private void openRomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Open INES rom";
            op.Filter = "All Supported Files |*.nes;*.NES;*.7z;*.7Z;*.rar;*.RAR;*.zip;*.ZIP|INES rom (*.nes)|*.nes;*.NES|Archives (*.7z *.rar *.zip)|*.7z;*.7Z;*.rar;*.RAR;*.zip;*.ZIP";
            op.Multiselect = false;
            if (op.ShowDialog(this) == DialogResult.OK)
            {
                this.OpenRom(op.FileName);
            }
            if (this._Nes != null)
                this._Nes.Resume();
        }
        private void runDebuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Debugger == null)
            {
                this.Debugger = new Frm_Debugger();
                this.Debugger.Show(); return;
            }
            if (!this.Debugger.Visible)
            {
                this.Debugger = new Frm_Debugger();
                this.Debugger.Show();
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //the fps
        private void timer_FPS_Tick(object sender, EventArgs e)
        {
            if (this._Nes != null)
            {
                StatusLabel1.Text = "FPS : " + this._Nes.PPU.FPS.ToString();
                this._Nes.PPU.FPS = 0;
                if (this._Nes.APU.RECODER.IsRecording)
                    StatusLabel.Text = "Recording audio [" + TimeSpan.FromSeconds(this._Nes.APU.RECODER.Time).ToString() + "]";
            }
            else
            {
                StatusLabel4_status.BackColor = Color.Red;
                StatusLabel4_status.Text = "OFF";
            }
        }
        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            Frm_ControlsSettings cnt = new Frm_ControlsSettings();
            cnt.ShowDialog(this);
            if (this._Nes != null)
            {
                if (cnt.OK)
                {
                    InputManager Manager = new InputManager(this.Handle);
                    Joypad Joy1 = new Joypad(Manager);
                    Joy1.A.Input = Program.Settings.CurrentControlProfile.Player1_A;
                    Joy1.B.Input = Program.Settings.CurrentControlProfile.Player1_B;
                    Joy1.Up.Input = Program.Settings.CurrentControlProfile.Player1_Up;
                    Joy1.Down.Input = Program.Settings.CurrentControlProfile.Player1_Down;
                    Joy1.Left.Input = Program.Settings.CurrentControlProfile.Player1_Left;
                    Joy1.Right.Input = Program.Settings.CurrentControlProfile.Player1_Right;
                    Joy1.Select.Input = Program.Settings.CurrentControlProfile.Player1_Select;
                    Joy1.Start.Input = Program.Settings.CurrentControlProfile.Player1_Start;
                    Joypad Joy2 = new Joypad(Manager);
                    Joy2.A.Input = Program.Settings.CurrentControlProfile.Player2_A;
                    Joy2.B.Input = Program.Settings.CurrentControlProfile.Player2_B;
                    Joy2.Up.Input = Program.Settings.CurrentControlProfile.Player2_Up;
                    Joy2.Down.Input = Program.Settings.CurrentControlProfile.Player2_Down;
                    Joy2.Left.Input = Program.Settings.CurrentControlProfile.Player2_Left;
                    Joy2.Right.Input = Program.Settings.CurrentControlProfile.Player2_Right;
                    Joy2.Select.Input = Program.Settings.CurrentControlProfile.Player2_Select;
                    Joy2.Start.Input = Program.Settings.CurrentControlProfile.Player2_Start;
                    this._Nes.SetupInput(Manager, Joy1, Joy2);
                }
                this._Nes.Resume();
            }
        }
        private void togglePauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.TogglePause();
        }
        private void hardResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //The nes will be destroied and will reintialized with the same rom
            if (this._Nes != null)
            {
                Debug.WriteSeparateLine(this, DebugStatus.None);
                Debug.WriteLine(this, "HARD RESET !!", DebugStatus.Error);
                this.OpenRom(this._Nes.MEMORY.MAP.Cartridge.RomPath);
            }
        }
        private void recentToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            //I know this is stuped :|
            int index = 0;
            for (int i = 0; i < recentToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (e.ClickedItem == recentToolStripMenuItem.DropDownItems[i])
                { index = i; break; }
            }
            if (File.Exists(Program.Settings.Recents[index]) == true)
            { this.OpenRom(Program.Settings.Recents[index]); }
            else
            {
                MessageBox.Show("This game is missing !!");
                Program.Settings.Recents.RemoveAt(index);
                this.RefreshRecents();
            }
        }
        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Pause
            if (this._Nes != null)
                this._Nes.Pause();
            //Show the dialog
            Frm_VideoOption OP = new Frm_VideoOption();
            OP.ShowDialog(this);
            //Applay the options if the Nes is null
            if (this._Nes != null)
            {
                this._Nes.SetupTV(Program.Settings.TV, Program.Settings.PaletteFormat);
                //Set the size
                switch (Program.Settings.Size.ToLower())
                {
                    case "x1":
                        this.Size = new Size(265, 305);
                        this.FormBorderStyle = FormBorderStyle.FixedDialog;
                        statusStrip1.SizingGrip = false;
                        break;
                    case "x2":
                        this.Size = new Size(521, 529);
                        this.FormBorderStyle = FormBorderStyle.FixedDialog;
                        statusStrip1.SizingGrip = false;
                        break;
                    case "stretch":
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                        statusStrip1.SizingGrip = true;
                        break;
                }
                switch (Program.Settings.GFXDevice)
                {
                    case GraphicDevices.Gdi:
                        VideoGdi gdi = new VideoGdi(Program.Settings.TV, panel1);
                        this._Nes.PPU.OutputDevice = gdi;
                        break;
                    case GraphicDevices.GdiHiRes:
                        VideoGdiHiRes gdih = new VideoGdiHiRes(Program.Settings.TV, panel1, "", 0);
                        this._Nes.PPU.OutputDevice = gdih;
                        break;
                    case GraphicDevices.SlimDx:
                        VideoSlimDx sli = new VideoSlimDx(Program.Settings.TV, panel1);
                        this._Nes.PPU.OutputDevice = sli;
                        break;
                }
                //Set fullscreen if available
                if (this._Nes.PPU.OutputDevice.SupportFullScreen)
                    this._Nes.PPU.OutputDevice.FullScreen = Program.Settings.Fullscreen;
                this._Nes.Resume();
            }
        }
        private void Frm_Main_ResizeBegin(object sender, EventArgs e)
        {
            if (this._Nes != null)
            {
                this._Nes.Pause();
            }
        }
        private void Frm_Main_ResizeEnd(object sender, EventArgs e)
        {
            if (this._Nes != null)
            {
                this._Nes.PPU.OutputDevice.UpdateSize(0, 0, panel1.Width, panel1.Height);
                this._Nes.PPU.OutputDevice.CanRender = true;
                this._Nes.Resume();
            }
        }
        private void noLimiterToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.NoLimiter = noLimiterToolStripMenuItem.Checked;
            if (this._Nes != null)
                this._Nes.PPU.NoLimiter = Program.Settings.NoLimiter;
            if (Program.Settings.NoLimiter)
                this.WriteStatus("No limiter enabled !!");
            else
                this.WriteStatus("No limiter disabled !!");
        }
        private void Frm_Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                this.TakeSnapshot();
            if (e.KeyCode == Keys.F6)
                this.SaveState();
            if (e.KeyCode == Keys.F9)
                this.LoadState();
            if (e.KeyCode == Keys.D0)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot0ToolStripMenuItem));
            if (e.KeyCode == Keys.D1)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot1ToolStripMenuItem));
            if (e.KeyCode == Keys.D2)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot2ToolStripMenuItem));
            if (e.KeyCode == Keys.D3)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot3ToolStripMenuItem));
            if (e.KeyCode == Keys.D4)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot4ToolStripMenuItem));
            if (e.KeyCode == Keys.D5)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot5ToolStripMenuItem));
            if (e.KeyCode == Keys.D6)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot6ToolStripMenuItem));
            if (e.KeyCode == Keys.D7)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot7ToolStripMenuItem));
            if (e.KeyCode == Keys.D8)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot8ToolStripMenuItem));
            if (e.KeyCode == Keys.D9)
                this.stateSlotToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(slot9ToolStripMenuItem));
            if (e.KeyCode == Keys.F12)
                this.ToggleFullscreen();
        }
        private void stateSlotToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //Uncheck all
            foreach (ToolStripMenuItem IT in stateSlotToolStripMenuItem.DropDownItems)
            { IT.Checked = false; }
            //Check selected
            ToolStripMenuItem SIT = (ToolStripMenuItem)e.ClickedItem;
            SIT.Checked = true;
            //Get state index
            this.StateIndex = Convert.ToInt32(SIT.Text.Substring(SIT.Text.Length - 1, 1));
            StatusLabel2_slot.Text = "State Slot [" + this.StateIndex.ToString() + "]";
        }
        private void pathsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            Frm_PathsOptions frm = new Frm_PathsOptions();
            frm.ShowDialog(this);
            if (this._Nes != null)
                this._Nes.Resume();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            timer1.Stop();
        }
        private void loadSateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LoadState();
        }
        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveState();
        }
        private void recordAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
            {
                if (!this._Nes.APU.RECODER.IsRecording)
                {
                    this._Nes.Pause();
                    SaveFileDialog SAV = new SaveFileDialog();
                    SAV.Title = "Save wave file";
                    SAV.Filter = "WAV PCM (*.wav)|*.wav";
                    if (SAV.ShowDialog(this) == DialogResult.OK)
                    {
                        this._Nes.APU.RECODER.Record(SAV.FileName, Program.Settings.Stereo);
                        recordAudioToolStripMenuItem.Text = "&Stop recording";
                    }
                    this._Nes.Resume();
                }
                else
                {
                    this._Nes.APU.RECODER.Stop();
                    recordAudioToolStripMenuItem.Text = "&Record audio";
                    this.WriteStatus("WAV SAVED");
                }
            }
        }
        private void paletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            Frm_Palette pl = new Frm_Palette();
            pl.ShowDialog(this);
            if (this._Nes != null)
            {
                if (pl.OK)
                {
                    this._Nes.PPU.SetTVFormat(Program.Settings.TV,
                        Program.Settings.PaletteFormat);
                }
                this._Nes.Resume();
            }
        }
        private void audioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            Frm_AudioOptions au = new Frm_AudioOptions();
            au.ShowDialog(this);
            if (this._Nes != null)
            {
                if (au.OK)
                {
                    this._Nes.SoundEnabled = Program.Settings.SoundEnabled;
                    this._Nes.APU.Square1Enabled = Program.Settings.Square1;
                    this._Nes.APU.Square2Enabled = Program.Settings.Square2;
                    this._Nes.APU.TriangleEnabled = Program.Settings.Triangle;
                    this._Nes.APU.NoiseEnabled = Program.Settings.Noize;
                    this._Nes.APU.DMCEnabled = Program.Settings.DMC;
                    this._Nes.APU.VRC6P1Enabled = Program.Settings.VRC6Pulse1;
                    this._Nes.APU.VRC6P2Enabled = Program.Settings.VRC6Pulse2;
                    this._Nes.APU.VRC6SawToothEnabled = Program.Settings.VRC6Sawtooth;
                    this._Nes.APU.SetVolume(Program.Settings.Volume);
                    if (!this._Nes.SoundEnabled)
                    {
                        while (this._Nes.APU.IsRendering)
                        { }
                        this._Nes.APU.Pause();
                    }
                }
                this._Nes.Resume();
            }
        }
        private void browserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            Frm_Browser BR = new Frm_Browser(this);
            BR.Show();
            if (this._Nes != null)
                this._Nes.Resume();
        }
        private void commandLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Debugger == null)
            {
                this.Debugger = new Frm_Debugger();
                this.Debugger.Show();
            }
            else if (!this.Debugger.Visible)
            {
                this.Debugger = new Frm_Debugger();
                this.Debugger.Show();
            }
            this.Debugger.WriteLine("LIST OF COMMAND LINES CURRENTLY AVAILABLE :", DebugStatus.Cool);
            this.Debugger.WriteLine("* -cm : show this list !!", DebugStatus.None);
            this.Debugger.WriteLine("* -ls x : load the state from slot number x (x = slot number from 0 to 9)", DebugStatus.None);
            this.Debugger.WriteLine("* -ss x : select the state slot number x (x = slot number from 0 to 9)", DebugStatus.None);
            this.Debugger.WriteLine("* -pal : select the PAL region", DebugStatus.None);
            this.Debugger.WriteLine("* -ntsc : select the NTSC region", DebugStatus.None);
            this.Debugger.WriteLine("* -st x : Enable / Disable no limiter (x=0 disable, x=1 enable)", DebugStatus.None);
            this.Debugger.WriteLine("* -s x : switch the size (x=x1 use size X1 '256x240', x=x2 use size X2 '512x480', x=str use Stretch)", DebugStatus.None);
            this.Debugger.WriteLine("* -r <WavePath> : record the audio wave into <WavePath>", DebugStatus.None);
            this.Debugger.WriteLine("* -p <PaletteFilePath> : load the palette <PaletteFilePath> and use it", DebugStatus.None);
            this.Debugger.WriteLine("* -w_size w h : resize the window (W=width, h=height)", DebugStatus.None);
            this.Debugger.WriteLine("* -w_pos x y : move the window into a specific location (x=X coordinate, y=Y coordinate)", DebugStatus.None);
            this.Debugger.WriteLine("* -w_max : maximize the window", DebugStatus.None);
            this.Debugger.WriteLine("* -w_min : minimize the window", DebugStatus.None);
        }
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, ".\\Help.chm", HelpNavigator.TableOfContents);
        }
        private void romInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
            {
                this._Nes.Pause();
                Frm_RomInfo RR = new Frm_RomInfo(this._Nes.MEMORY.MAP.Cartridge.RomPath);
                RR.ShowDialog(this);
                this._Nes.Resume();
            }
        }
        private void UiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            Frm_About ab = new Frm_About(Program.AssemblyVersion);
            ab.ShowDialog(this);
            if (this._Nes != null)
                this._Nes.Resume();
        }
        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            Frm_GeneralOptions ge = new Frm_GeneralOptions();
            ge.ShowDialog(this);
            if (this._Nes != null)
            {
                this._Nes.AutoSaveSRAM = Program.Settings.AutoSaveSRAM;
                this._Nes.Resume();
            }
        }
        private void softResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
            {
                Debug.WriteLine(this, "SOFT RESET !!", DebugStatus.Warning);
                this._Nes.SoftReset();
            }
        }
        private void indexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, ".\\Help.chm", HelpNavigator.Index);
        }
        private void saveSRAMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Nes != null)
                this._Nes.Pause();
            else
                return;
            if (!this._Nes.MEMORY.MAP.Cartridge.IsSaveRam)
            { MessageBox.Show("Not a S-RAM !!"); return; }
            SaveFileDialog sav = new SaveFileDialog();
            sav.Title = "Save S-RAM";
            sav.Filter = "Save file (*.sav)|*.sav";
            if (sav.ShowDialog(this) == DialogResult.OK)
                this._Nes.SaveSRAM(sav.FileName);
            if (this._Nes != null)
                this._Nes.Resume();
        }
        private void Frm_Main_Deactivate(object sender, EventArgs e)
        {
            if (this._Nes != null & Program.Settings.PauseWhenFocusLost)
                this._Nes.Pause();
        }
        void Frm_Main_Activated(object sender, EventArgs e)
        {
            if (this._Nes != null & Program.Settings.PauseWhenFocusLost)
                this._Nes.Resume();
        }
        //Zapper Trigger
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this._Nes != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int X = (e.X * 256) / panel1.Width;
                    int Y = (e.Y * ((this._Nes.TvFormat == TVFORMAT.NTSC) ? 224 : 240)) / panel1.Height;
                    this._Nes.MEMORY.ZAPPER.PullTrigger(true, X, Y);
                }
            }
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._Nes != null)
                this._Nes.MEMORY.ZAPPER.PullTrigger(false, 0, 0);
        }
        private void showMenuAndStatusStripToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            menuStrip1.Visible = showMenuAndStatusStripToolStripMenuItem.Checked;
            statusStrip1.Visible = showMenuAndStatusStripToolStripMenuItem.Checked;
        }

        private void supportedMappersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Debugger == null)
            {
                this.Debugger = new Frm_Debugger();
                this.Debugger.Show();
            }
            else if (!this.Debugger.Visible)
            {
                this.Debugger = new Frm_Debugger();
                this.Debugger.Show();
            }
            this.Debugger.WriteLine("SUPPORTED MAPPERS :", DebugStatus.Cool);
            foreach (int map in INESHeaderReader.SupportedMappersNo)
            {
                this.Debugger.WriteLine("M # " + map.ToString() + " " + @"""" +
                    INESHeaderReader.GetMapperName(map) + @"""", DebugStatus.None);
            }
        }
    }
}