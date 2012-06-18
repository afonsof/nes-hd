using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
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
using SevenZip;

namespace NesHd.Ui.WinForms
{
    public partial class MainForm : Form
    {
        private DebuggerForm Debugger;
        private NesEngine _engine;
        private SevenZipExtractor _extractor;
        private Thread _gameThread;
        private ThreadStart _myThreadCreator;
        private bool _startActions = true;
        private int _stateIndex;

        #region Functions

        /// <summary>
        /// Load the settings (applay them)
        /// </summary>
        public void LoadSettings()
        {
            Debug.WriteLine(null, "Loading settings ...", DebugStatus.None);
            Location = new Point(Program.Settings.Win_X, Program.Settings.Win_Y);
            Size = new Size(Program.Settings.Win_W, Program.Settings.Win_H);
            runDebuggerAtStartupToolStripMenuItem.Checked = Program.Settings.ShowDebugger;
            noLimiterToolStripMenuItem.Checked = Program.Settings.NoLimiter;
            showMenuAndStatusStripToolStripMenuItem.Checked = Program.Settings.ShowMenuAndStatus;
            ShowMenuAndStatusStripToolStripMenuItemCheckedChanged(this, null);
            RefreshRecents();
            Debug.WriteLine(this, "Selected TV Format : " + Program.Settings.TV.ToString(), DebugStatus.None);
            Debug.WriteLine(this, "Selected GFX : " + Program.Settings.GFXDevice.ToString(), DebugStatus.None);
            Debug.WriteLine(this, "Settings OK !", DebugStatus.Cool);
            Debug.WriteSeparateLine(this, DebugStatus.None);
        }

        public void SaveSettings()
        {
            Program.Settings.Win_X = Location.X;
            Program.Settings.Win_Y = Location.Y;
            Program.Settings.Win_W = Width;
            Program.Settings.Win_H = Height;
            Program.Settings.ShowDebugger = runDebuggerAtStartupToolStripMenuItem.Checked;
            Program.Settings.ShowMenuAndStatus = showMenuAndStatusStripToolStripMenuItem.Checked;
            Program.Settings.Save();
        }

        private void AddRecent(string filePath)
        {
            if (Program.Settings.Recents == null)
                Program.Settings.Recents = new StringCollection();
            for (var i = 0; i < Program.Settings.Recents.Count; i++)
            {
                if (filePath == Program.Settings.Recents[i])
                {
                    Program.Settings.Recents.Remove(filePath);
                }
            }
            Program.Settings.Recents.Insert(0, filePath);
            //limit to 9 elements
            if (Program.Settings.Recents.Count > 9)
                Program.Settings.Recents.RemoveAt(9);
        }

        private void RefreshRecents()
        {
            if (Program.Settings.Recents == null)
                Program.Settings.Recents = new StringCollection();
            recentToolStripMenuItem.DropDownItems.Clear();
            var i = 1;
            foreach (var recc in Program.Settings.Recents)
            {
                var it = new ToolStripMenuItem {Text = Path.GetFileNameWithoutExtension(recc)};
                switch (i) //This for the recent item shortcut key
                    //So that user can press CTRL + item No
                {
                    case 1:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D1)));
                        break;
                    case 2:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D2)));
                        break;
                    case 3:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D3)));
                        break;
                    case 4:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D4)));
                        break;
                    case 5:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D5)));
                        break;
                    case 6:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D6)));
                        break;
                    case 7:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D7)));
                        break;
                    case 8:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D8)));
                        break;
                    case 9:
                        it.ShortcutKeys = (((Keys.Control |
                                             Keys.D9)));
                        break;
                }
                recentToolStripMenuItem.DropDownItems.Add(it);
                i++;
            }
        }

        public void OpenRom(string romPath)
        {
            if (File.Exists(romPath))
            {
                #region Check if archive

                var extension = Path.GetExtension(romPath);
                if (extension != null && extension.ToLower() != ".nes")
                {
                    try
                    {
                        _extractor = new SevenZipExtractor(romPath);
                    }
                    catch
                    {
                    }
                    if (_extractor.ArchiveFileData.Count == 1)
                    {
                        if (
                            _extractor.ArchiveFileData[0].FileName.Substring(
                                _extractor.ArchiveFileData[0].FileName.Length - 4, 4).ToLower() == ".nes")
                        {
                            _extractor.ExtractArchive(Path.GetTempPath());
                            romPath = Path.GetTempPath() + _extractor.ArchiveFileData[0].FileName;
                        }
                    }
                    else
                    {
                        var ar = new ArchivesForm(_extractor.ArchiveFileData.Select(file => file.FileName).ToArray());
                        ar.ShowDialog(this);
                        if (ar.Ok)
                        {
                            string[] fil = {ar.SelectedRom};
                            _extractor.ExtractFiles(Path.GetTempPath(), fil);
                            romPath = Path.GetTempPath() + ar.SelectedRom;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                #endregion

                #region Check the rom

                var header = new NesHeaderReader(romPath);
                if (header.ValidRom)
                {
                    if (!header.SupportedMapper())
                    {
                        MessageBox.Show("Can't load rom:\n" + romPath +
                                        "\n\nUNSUPPORTED MAPPER # " +
                                        header.MemoryMapper);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Can't load rom:\n" + romPath +
                                    "\n\nRom is damaged or not an INES format file");
                    return;
                }

                #endregion

                //Exit current thread
                if (_engine != null)
                {
                    _engine.ShutDown();
                    _engine = null;
                }
                if (_gameThread != null)
                {
                    _gameThread.Abort();
                }
                //Start new nes !!
                if (Program.Settings.PaletteFormat == null)
                    Program.Settings.PaletteFormat = new PaletteFormat();
                _engine = new NesEngine(Program.Settings.TV, Program.Settings.PaletteFormat);
                _engine.PauseToggle += NesPauseToggle;
                if (_engine.LoadRom(romPath))
                {
                    #region Setup input

                    var manager = new InputManager(Handle);
                    var joy1 = new Joypad(manager);
                    joy1.A.Input = Program.Settings.CurrentControlProfile.Player1A;
                    joy1.B.Input = Program.Settings.CurrentControlProfile.Player1B;
                    joy1.Up.Input = Program.Settings.CurrentControlProfile.Player1Up;
                    joy1.Down.Input = Program.Settings.CurrentControlProfile.Player1Down;
                    joy1.Left.Input = Program.Settings.CurrentControlProfile.Player1Left;
                    joy1.Right.Input = Program.Settings.CurrentControlProfile.Player1Right;
                    joy1.Select.Input = Program.Settings.CurrentControlProfile.Player1Select;
                    joy1.Start.Input = Program.Settings.CurrentControlProfile.Player1Start;
                    var joy2 = new Joypad(manager);
                    joy2.A.Input = Program.Settings.CurrentControlProfile.Player2A;
                    joy2.B.Input = Program.Settings.CurrentControlProfile.Player2B;
                    joy2.Up.Input = Program.Settings.CurrentControlProfile.Player2Up;
                    joy2.Down.Input = Program.Settings.CurrentControlProfile.Player2Down;
                    joy2.Left.Input = Program.Settings.CurrentControlProfile.Player2Left;
                    joy2.Right.Input = Program.Settings.CurrentControlProfile.Player2Right;
                    joy2.Select.Input = Program.Settings.CurrentControlProfile.Player2Select;
                    joy2.Start.Input = Program.Settings.CurrentControlProfile.Player2Start;
                    _engine.SetupInput(manager, joy1, joy2);

                    #endregion

                    #region Output

                    //Set the size
                    switch (Program.Settings.Size.ToLower())
                    {
                        case "x1":
                            Size = new Size(265, 305);
                            FormBorderStyle = FormBorderStyle.FixedDialog;
                            statusStrip1.SizingGrip = false;
                            break;
                        case "x2":
                            Size = new Size(521, 529);
                            FormBorderStyle = FormBorderStyle.FixedDialog;
                            statusStrip1.SizingGrip = false;
                            break;
                        case "stretch":
                            FormBorderStyle = FormBorderStyle.Sizable;
                            statusStrip1.SizingGrip = true;
                            break;
                    }
                    //The output devices
                    var mon = new SoundDeviceGeneral16(statusStrip1) {Stereo = Program.Settings.Stereo};
                    switch (Program.Settings.GFXDevice)
                    {
                        case GraphicDevices.Gdi:
                            var gdi = new VideoGdi(Program.Settings.TV, panel1);
                            _engine.SetupOutput(gdi, mon);
                            break;
                        case GraphicDevices.GdiHiRes:
                            var gdih = new VideoGdiHiRes(Program.Settings.TV, panel1,
                                                         _engine.Memory.Map.Cartridge.RomPath,
                                                         _engine.Memory.Map.Cartridge.ChrPages);
                            _engine.SetupOutput(gdih, mon);
                            break;
                        case GraphicDevices.SlimDx:
                            var sl = new VideoSlimDx(Program.Settings.TV, panel1, _engine.Memory.Map.Cartridge.Multi);
                            _engine.SetupOutput(sl, mon);
                            break;
                        default:
                            Program.Settings.GFXDevice = GraphicDevices.SlimDx;
                            var sl1 = new VideoSlimDx(Program.Settings.TV, panel1, _engine.Memory.Map.Cartridge.Multi);
                            _engine.SetupOutput(sl1, mon);
                            break;
                    }
                    if (_engine.Ppu.OutputDevice.SupportFullScreen)
                        _engine.Ppu.OutputDevice.FullScreen = Program.Settings.Fullscreen;
                    //Audio
                    _engine.SoundEnabled = Program.Settings.SoundEnabled;
                    _engine.Apu.Square1Enabled = Program.Settings.Square1;
                    _engine.Apu.Square2Enabled = Program.Settings.Square2;
                    _engine.Apu.DMCEnabled = Program.Settings.DMC;
                    _engine.Apu.TriangleEnabled = Program.Settings.Triangle;
                    _engine.Apu.NoiseEnabled = Program.Settings.Noize;
                    _engine.Apu.VRC6P1Enabled = Program.Settings.VRC6Pulse1;
                    _engine.Apu.VRC6P2Enabled = Program.Settings.VRC6Pulse2;
                    _engine.Apu.VRC6SawToothEnabled = Program.Settings.VRC6Sawtooth;
                    _engine.Apu.SetVolume(Program.Settings.Volume);

                    #endregion

                    #region Misc

                    _engine.Ppu.NoLimiter = Program.Settings.NoLimiter;
                    _engine.AutoSaveSram = Program.Settings.AutoSaveSRAM;

                    #endregion

                    //Launch
                    _myThreadCreator = _engine.Run;
                    _gameThread = new Thread(_myThreadCreator);
                    _gameThread.Start();
                    timer_FPS.Start();
                    StatusLabel4_status.BackColor = Color.Green;
                    StatusLabel4_status.Text = "ON";
                    //Add to the recent
                    AddRecent(romPath);
                    RefreshRecents();
                    //Set the name
                    Text = "My Nes - " + Path.GetFileNameWithoutExtension(romPath);
                }
                else
                {
                    MessageBox.Show("Can't load rom:\n" + romPath +
                                    "\n\nRom is damaged or not an INES format file !!");
                    if (_engine != null)
                    {
                        _engine.ShutDown();
                        _engine = null;
                    }
                    if (_gameThread != null)
                    {
                        _gameThread.Abort();
                    }
                    return;
                }
            }
        }

        private void TakeSnapshot()
        {
            if (_engine == null)
                return;
            //If there's no rom, get out !!
            if (!File.Exists(_engine.Memory.Map.Cartridge.RomPath))
            {
                return;
            }
            _engine.Pause();
            Directory.CreateDirectory(Path.GetFullPath(Program.Settings.SnapshotsFolder));
            var i = 1;
            while (i != 0)
            {
                if (!File.Exists(Path.GetFullPath(Program.Settings.SnapshotsFolder) + "\\"
                                 + Path.GetFileNameWithoutExtension(_engine.Memory.Map.Cartridge.RomPath) + "_" +
                                 i.ToString() + Program.Settings.SnapshotFormat))
                {
                    _engine.Ppu.OutputDevice.TakeSnapshot(Path.GetFullPath(Program.Settings.SnapshotsFolder) + "\\"
                                                       +
                                                       Path.GetFileNameWithoutExtension(
                                                           _engine.Memory.Map.Cartridge.RomPath) + "_" +
                                                       i.ToString() + Program.Settings.SnapshotFormat,
                                                       Program.Settings.SnapshotFormat);
                    break;
                }
                i++;
            }
            _engine.Resume();
        }

        private void SaveState()
        {
            if (_engine == null)
                return;
            Directory.CreateDirectory(Path.GetFullPath(Program.Settings.StateFloder));
            var ST = new State(_engine);
            if (ST.SaveState(Path.GetFullPath(Program.Settings.StateFloder)
                             + "\\" + Path.GetFileNameWithoutExtension(_engine.Memory.Map.Cartridge.RomPath) + "_" +
                             _stateIndex.ToString() + ".st"))
            {
                WriteStatus("STATE SAVED !!");
            }
            else
            {
                WriteStatus("CAN'T SAVE !!!!!??");
            }
        }

        private void LoadState()
        {
            if (_engine == null)
                return;
            var ST = new State(_engine);
            if (ST.LoadState(Path.GetFullPath(Program.Settings.StateFloder)
                             + "\\" + Path.GetFileNameWithoutExtension(_engine.Memory.Map.Cartridge.RomPath) + "_" +
                             _stateIndex.ToString() + ".st"))
            {
                WriteStatus("STATE LOADED !!");
            }
            else
            {
                WriteStatus("NO STATE FOUND IN SLOT " + _stateIndex.ToString());
            }
        }

        private void WriteStatus(string TEXT)
        {
            StatusLabel.Text = TEXT;
            timer1.Start();
        }

        public void ShowDialogs()
        {
            if (!_startActions)
                return;
            if (Program.Settings.ShowDebugger)
            {
                Debugger = new DebuggerForm();
                Debugger.Show();
            }
            if (Program.Settings.ShowBrowser)
            {
                var BRO = new BrowserForm(this);
                BRO.Show();
            }
        }

        private void DoCommandLines(string[] Args)
        {
            //Get the command lines
            if (Args != null)
            {
                if (Args.Length > 0)
                {
                    _startActions = false;
                    //First one must be the rom path, so :
                    try
                    {
                        if (File.Exists(Args[0]))
                        {
                            OpenRom(Args[0]);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Rom path expacted !!");
                        return;
                    }
                    //Let's see what the user wants My Nes to do :
                    for (var i = 1 /*start from 1 'cause we've already used the args[0]*/;
                         i < Args.Length;
                         i++)
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
                                WindowState = FormWindowState.Minimized;
                                break;
                            case "-w_max":
                                WindowState = FormWindowState.Maximized;
                                break;
                            case "-w_size":
                                try
                                {
                                    //We expact the next "arg" must be the size
                                    i++;
                                    var W = Convert.ToInt32(Args[i]);
                                    i++;
                                    var H = Convert.ToInt32(Args[i]);
                                    if (FormBorderStyle == FormBorderStyle.Sizable)
                                        Size = new Size(W, H);
                                }
                                catch
                                {
                                    return;
                                }
                                break;
                            case "-w_pos":
                                try
                                {
                                    //We expact the next "arg" must be the coordinates
                                    i++;
                                    var X = Convert.ToInt32(Args[i]);
                                    i++;
                                    var Y = Convert.ToInt32(Args[i]);
                                    Location = new Point(X, Y);
                                }
                                catch
                                {
                                    return;
                                }
                                break;
                            case "-p":
                                try
                                {
                                    //We expact the next "arg" must be the palette path
                                    i++;
                                    if (File.Exists(Args[i]))
                                        if (Paletter.LoadPalette(Args[i]) != null)
                                            _engine.Ppu.Palette = Paletter.LoadPalette(Args[i]);
                                }
                                catch
                                {
                                    return;
                                }
                                break;
                            case "-r":
                                try
                                {
                                    //We expact the next "arg" must be the wav path
                                    i++;
                                    if (_engine != null)
                                    {
                                        _engine.Pause();
                                        _engine.Apu.RECODER.Record(Path.GetFullPath(Args[i]), Program.Settings.Stereo);
                                        recordAudioToolStripMenuItem.Text = "&Stop recording";
                                        _engine.Resume();
                                    }
                                }
                                catch
                                {
                                    return;
                                }
                                break;
                            case "-s":
                                try
                                {
                                    //We expact the next "arg" must be the size mode
                                    i++;
                                    if (Args[i] == "x1")
                                    {
                                        _engine.Pause();
                                        Size = new Size(265, 305);
                                        FormBorderStyle = FormBorderStyle.FixedDialog;
                                        statusStrip1.SizingGrip = false;
                                        _engine.Resume();
                                    }
                                    if (Args[i] == "x2")
                                    {
                                        _engine.Pause();
                                        Size = new Size(265, 305);
                                        FormBorderStyle = FormBorderStyle.FixedDialog;
                                        statusStrip1.SizingGrip = false;
                                        _engine.Resume();
                                    }
                                    if (Args[i] == "str")
                                    {
                                        FormBorderStyle = FormBorderStyle.Sizable;
                                        statusStrip1.SizingGrip = true;
                                    }
                                }
                                catch
                                {
                                    return;
                                }
                                break;
                            case "-st":
                                try
                                {
                                    //We expact the next "arg" must be 0 or 1
                                    i++;
                                    if (Args[i] == "0")
                                    {
                                        if (_engine != null)
                                            _engine.Ppu.NoLimiter = false;
                                        WriteStatus("No limiter disabled !!");
                                    }
                                    else if (Args[i] == "1")
                                    {
                                        if (_engine != null)
                                            _engine.Ppu.NoLimiter = true;
                                        WriteStatus("No limiter enabled !!");
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                                catch
                                {
                                    return;
                                }
                                break;
                            case "-ntsc":
                                if (_engine != null)
                                {
                                    _engine.SetupTv(TvFormat.Ntsc, Program.Settings.PaletteFormat);
                                }
                                break;
                            case "-pal":
                                if (_engine != null)
                                {
                                    _engine.SetupTv(TvFormat.Pal, Program.Settings.PaletteFormat);
                                }
                                break;
                            case "-ls":
                                try
                                {
                                    //We expact the next "arg" must be the state number
                                    i++;
                                    if (Args[i] == "0")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot0ToolStripMenuItem));
                                    if (Args[i] == "1")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot1ToolStripMenuItem));
                                    if (Args[i] == "2")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot2ToolStripMenuItem));
                                    if (Args[i] == "3")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot3ToolStripMenuItem));
                                    if (Args[i] == "4")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot4ToolStripMenuItem));
                                    if (Args[i] == "5")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot5ToolStripMenuItem));
                                    if (Args[i] == "6")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot6ToolStripMenuItem));
                                    if (Args[i] == "7")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot7ToolStripMenuItem));
                                    if (Args[i] == "8")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot8ToolStripMenuItem));
                                    if (Args[i] == "9")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot9ToolStripMenuItem));
                                    LoadState();
                                }
                                catch
                                {
                                    MessageBox.Show("State # expacted !!");
                                    return;
                                }
                                break;
                            case "-ss":
                                try
                                {
                                    //We expact the next "arg" must be the state number
                                    i++;
                                    if (Args[i] == "0")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot0ToolStripMenuItem));
                                    if (Args[i] == "1")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot1ToolStripMenuItem));
                                    if (Args[i] == "2")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot2ToolStripMenuItem));
                                    if (Args[i] == "3")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot3ToolStripMenuItem));
                                    if (Args[i] == "4")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot4ToolStripMenuItem));
                                    if (Args[i] == "5")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot5ToolStripMenuItem));
                                    if (Args[i] == "6")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot6ToolStripMenuItem));
                                    if (Args[i] == "7")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot7ToolStripMenuItem));
                                    if (Args[i] == "8")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot8ToolStripMenuItem));
                                    if (Args[i] == "9")
                                        stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                                                       new ToolStripItemClickedEventArgs
                                                                                           (slot9ToolStripMenuItem));
                                }
                                catch
                                {
                                    MessageBox.Show("State # expacted !!");
                                    return;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void ToggleFullscreen()
        {
            if (_engine == null)
            {
                return;
            }
            _engine.Pause();
            Program.Settings.Fullscreen = !Program.Settings.Fullscreen;
            if (_engine.Ppu.OutputDevice.SupportFullScreen)
            {
                _engine.Ppu.OutputDevice.FullScreen = Program.Settings.Fullscreen;
                if (!Program.Settings.Fullscreen)
                {
                    //restore the old status after the back
                    //from Fullscreen.
                    Size = MaximumSize;
                    Location = new Point(Program.Settings.Win_X, Program.Settings.Win_Y);
                    Size = new Size(Program.Settings.Win_W, Program.Settings.Win_H);
                }
                else
                {
                    //save the window status to restore when
                    //we back from Fullscreen.
                    Program.Settings.Win_X = Location.X;
                    Program.Settings.Win_Y = Location.Y;
                    Program.Settings.Win_W = Width;
                    Program.Settings.Win_H = Height;
                }
            }
            _engine.Resume();
        }

        #endregion

        public MainForm(string[] args)
        {
            InitializeComponent();
            Debug.WriteLine(this, "Main window Initialized OK.", DebugStatus.Cool);
            DoCommandLines(args);
            Activated += FrmMainActivated;
        }

        public NesEngine Engine { get; private set; }

        private void NesPauseToggle(object sender, EventArgs e)
        {
            if (_engine != null)
            {
                if (_engine.Cpu.ON)
                {
                    if (_engine.Cpu.Pause)
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

        private void FrmMainFormClosed(object sender, FormClosedEventArgs e)
        {
            timer_FPS.Stop();
            if (_engine != null)
            {
                _engine.ShutDown();
            }
            if (_gameThread != null)
            {
                _gameThread.Abort();
                _engine = null;
            }
            if (WindowState == FormWindowState.Maximized | WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            SaveSettings();
            if (Debugger != null)
                Debugger.SaveSettings();
        }

        private void OpenRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            var op = new OpenFileDialog();
            op.Title = "Open INES rom";
            op.Filter =
                "All Supported Files |*.nes;*.NES;*.7z;*.7Z;*.rar;*.RAR;*.zip;*.ZIP|INES rom (*.nes)|*.nes;*.NES|Archives (*.7z *.rar *.zip)|*.7z;*.7Z;*.rar;*.RAR;*.zip;*.ZIP";
            op.Multiselect = false;
            if (op.ShowDialog(this) == DialogResult.OK)
            {
                OpenRom(op.FileName);
            }
            if (_engine != null)
                _engine.Resume();
        }

        private void RunDebuggerToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Debugger == null)
            {
                Debugger = new DebuggerForm();
                Debugger.Show();
                return;
            }
            if (!Debugger.Visible)
            {
                Debugger = new DebuggerForm();
                Debugger.Show();
            }
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        //the fps
        private void TimerFpsTick(object sender, EventArgs e)
        {
            if (_engine != null)
            {
                StatusLabel1.Text = "FPS : " + _engine.Ppu.Fps.ToString();
                _engine.Ppu.Fps = 0;
                if (_engine.Apu.RECODER.IsRecording)
                    StatusLabel.Text = "Recording audio [" + TimeSpan.FromSeconds(_engine.Apu.RECODER.Time).ToString() +
                                       "]";
            }
            else
            {
                StatusLabel4_status.BackColor = Color.Red;
                StatusLabel4_status.Text = "OFF";
            }
        }

        private void ControlsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            var cnt = new Frm_ControlsSettings();
            cnt.ShowDialog(this);
            if (_engine != null)
            {
                if (cnt.OK)
                {
                    var manager = new InputManager(Handle);
                    var joy1 = new Joypad(manager);
                    joy1.A.Input = Program.Settings.CurrentControlProfile.Player1A;
                    joy1.B.Input = Program.Settings.CurrentControlProfile.Player1B;
                    joy1.Up.Input = Program.Settings.CurrentControlProfile.Player1Up;
                    joy1.Down.Input = Program.Settings.CurrentControlProfile.Player1Down;
                    joy1.Left.Input = Program.Settings.CurrentControlProfile.Player1Left;
                    joy1.Right.Input = Program.Settings.CurrentControlProfile.Player1Right;
                    joy1.Select.Input = Program.Settings.CurrentControlProfile.Player1Select;
                    joy1.Start.Input = Program.Settings.CurrentControlProfile.Player1Start;
                    var joy2 = new Joypad(manager);
                    joy2.A.Input = Program.Settings.CurrentControlProfile.Player2A;
                    joy2.B.Input = Program.Settings.CurrentControlProfile.Player2B;
                    joy2.Up.Input = Program.Settings.CurrentControlProfile.Player2Up;
                    joy2.Down.Input = Program.Settings.CurrentControlProfile.Player2Down;
                    joy2.Left.Input = Program.Settings.CurrentControlProfile.Player2Left;
                    joy2.Right.Input = Program.Settings.CurrentControlProfile.Player2Right;
                    joy2.Select.Input = Program.Settings.CurrentControlProfile.Player2Select;
                    joy2.Start.Input = Program.Settings.CurrentControlProfile.Player2Start;
                    _engine.SetupInput(manager, joy1, joy2);
                }
                _engine.Resume();
            }
        }

        private void TogglePauseToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.TogglePause();
        }

        private void HardResetToolStripMenuItemClick(object sender, EventArgs e)
        {
            //The nes will be destroied and will reintialized with the same rom
            if (_engine != null)
            {
                Debug.WriteSeparateLine(this, DebugStatus.None);
                Debug.WriteLine(this, "HARD RESET !!", DebugStatus.Error);
                OpenRom(_engine.Memory.Map.Cartridge.RomPath);
            }
        }

        private void RecentToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            //I know this is stuped :|
            var index = 0;
            for (var i = 0; i < recentToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (e.ClickedItem == recentToolStripMenuItem.DropDownItems[i])
                {
                    index = i;
                    break;
                }
            }
            if (File.Exists(Program.Settings.Recents[index]))
            {
                OpenRom(Program.Settings.Recents[index]);
            }
            else
            {
                MessageBox.Show("This game is missing !!");
                Program.Settings.Recents.RemoveAt(index);
                RefreshRecents();
            }
        }

        private void VideoToolStripMenuItemClick(object sender, EventArgs e)
        {
            //Pause
            if (_engine != null)
                _engine.Pause();
            //Show the dialog
            var optionForm = new VideoOptionForm(_engine.Memory.Map.Cartridge.Multi);
            optionForm.ShowDialog(this);
            //Applay the options if the Nes is null
            if (_engine != null)
            {
                _engine.SetupTv(Program.Settings.TV, Program.Settings.PaletteFormat);
                //Set the size
                switch (Program.Settings.Size.ToLower())
                {
                    case "x1":
                        Size = new Size(265, 305);
                        FormBorderStyle = FormBorderStyle.FixedDialog;
                        statusStrip1.SizingGrip = false;
                        break;
                    case "x2":
                        Size = new Size(521, 529);
                        FormBorderStyle = FormBorderStyle.FixedDialog;
                        statusStrip1.SizingGrip = false;
                        break;
                    case "stretch":
                        FormBorderStyle = FormBorderStyle.Sizable;
                        statusStrip1.SizingGrip = true;
                        break;
                }
                switch (Program.Settings.GFXDevice)
                {
                    case GraphicDevices.Gdi:
                        var gdi = new VideoGdi(Program.Settings.TV, panel1);
                        _engine.Ppu.OutputDevice = gdi;
                        break;
                    case GraphicDevices.GdiHiRes:
                        var gdih = new VideoGdiHiRes(Program.Settings.TV, panel1, "", 0);
                        _engine.Ppu.OutputDevice = gdih;
                        break;
                    case GraphicDevices.SlimDx:
                        var sli = new VideoSlimDx(Program.Settings.TV, panel1, _engine.Memory.Map.Cartridge.Multi);
                        _engine.Ppu.OutputDevice = sli;
                        break;
                }
                //Set fullscreen if available
                if (_engine.Ppu.OutputDevice.SupportFullScreen)
                    _engine.Ppu.OutputDevice.FullScreen = Program.Settings.Fullscreen;
                _engine.Resume();
            }
        }

        private void FrmMainResizeBegin(object sender, EventArgs e)
        {
            if (_engine != null)
            {
                _engine.Pause();
            }
        }

        private void FrmMainResizeEnd(object sender, EventArgs e)
        {
            if (_engine != null)
            {
                _engine.Ppu.OutputDevice.UpdateSize(0, 0, panel1.Width, panel1.Height);
                _engine.Ppu.OutputDevice.CanRender = true;
                _engine.Resume();
            }
        }

        private void NoLimiterToolStripMenuItemCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.NoLimiter = noLimiterToolStripMenuItem.Checked;
            if (_engine != null)
                _engine.Ppu.NoLimiter = Program.Settings.NoLimiter;
            if (Program.Settings.NoLimiter)
                WriteStatus("No limiter enabled !!");
            else
                WriteStatus("No limiter disabled !!");
        }

        private void FrmMainKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                TakeSnapshot();
            if (e.KeyCode == Keys.F6)
                SaveState();
            if (e.KeyCode == Keys.F9)
                LoadState();
            if (e.KeyCode == Keys.D0)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot0ToolStripMenuItem));
            if (e.KeyCode == Keys.D1)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot1ToolStripMenuItem));
            if (e.KeyCode == Keys.D2)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot2ToolStripMenuItem));
            if (e.KeyCode == Keys.D3)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot3ToolStripMenuItem));
            if (e.KeyCode == Keys.D4)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot4ToolStripMenuItem));
            if (e.KeyCode == Keys.D5)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot5ToolStripMenuItem));
            if (e.KeyCode == Keys.D6)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot6ToolStripMenuItem));
            if (e.KeyCode == Keys.D7)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot7ToolStripMenuItem));
            if (e.KeyCode == Keys.D8)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot8ToolStripMenuItem));
            if (e.KeyCode == Keys.D9)
                stateSlotToolStripMenuItem_DropDownItemClicked(this,
                                                               new ToolStripItemClickedEventArgs(
                                                                   slot9ToolStripMenuItem));
            if (e.KeyCode == Keys.F12)
                ToggleFullscreen();
        }

        private void stateSlotToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //Uncheck all
            foreach (ToolStripMenuItem IT in stateSlotToolStripMenuItem.DropDownItems)
            {
                IT.Checked = false;
            }
            //Check selected
            var SIT = (ToolStripMenuItem) e.ClickedItem;
            SIT.Checked = true;
            //Get state index
            _stateIndex = Convert.ToInt32(SIT.Text.Substring(SIT.Text.Length - 1, 1));
            StatusLabel2_slot.Text = "State Slot [" + _stateIndex.ToString() + "]";
        }

        private void PathsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            var frm = new Frm_PathsOptions();
            frm.ShowDialog(this);
            if (_engine != null)
                _engine.Resume();
        }

        private void Timer1Tick(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            timer1.Stop();
        }

        private void LoadSateToolStripMenuItemClick(object sender, EventArgs e)
        {
            LoadState();
        }

        private void SaveStateToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveState();
        }

        private void RecordAudioToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
            {
                if (!_engine.Apu.RECODER.IsRecording)
                {
                    _engine.Pause();
                    var SAV = new SaveFileDialog();
                    SAV.Title = "Save wave file";
                    SAV.Filter = "WAV PCM (*.wav)|*.wav";
                    if (SAV.ShowDialog(this) == DialogResult.OK)
                    {
                        _engine.Apu.RECODER.Record(SAV.FileName, Program.Settings.Stereo);
                        recordAudioToolStripMenuItem.Text = "&Stop recording";
                    }
                    _engine.Resume();
                }
                else
                {
                    _engine.Apu.RECODER.Stop();
                    recordAudioToolStripMenuItem.Text = "&Record audio";
                    WriteStatus("WAV SAVED");
                }
            }
        }

        private void PaletteToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            var pl = new Frm_Palette();
            pl.ShowDialog(this);
            if (_engine != null)
            {
                if (pl.OK)
                {
                    _engine.Ppu.SetTvFormat(Program.Settings.TV,
                                         Program.Settings.PaletteFormat);
                }
                _engine.Resume();
            }
        }

        private void AudioToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            var au = new AudioOptionsForm();
            au.ShowDialog(this);
            if (_engine != null)
            {
                if (au.Ok)
                {
                    _engine.SoundEnabled = Program.Settings.SoundEnabled;
                    _engine.Apu.Square1Enabled = Program.Settings.Square1;
                    _engine.Apu.Square2Enabled = Program.Settings.Square2;
                    _engine.Apu.TriangleEnabled = Program.Settings.Triangle;
                    _engine.Apu.NoiseEnabled = Program.Settings.Noize;
                    _engine.Apu.DMCEnabled = Program.Settings.DMC;
                    _engine.Apu.VRC6P1Enabled = Program.Settings.VRC6Pulse1;
                    _engine.Apu.VRC6P2Enabled = Program.Settings.VRC6Pulse2;
                    _engine.Apu.VRC6SawToothEnabled = Program.Settings.VRC6Sawtooth;
                    _engine.Apu.SetVolume(Program.Settings.Volume);
                    if (!_engine.SoundEnabled)
                    {
                        while (_engine.Apu.IsRendering)
                        {
                        }
                        _engine.Apu.Pause();
                    }
                }
                _engine.Resume();
            }
        }

        private void BrowserToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            var BR = new BrowserForm(this);
            BR.Show();
            if (_engine != null)
                _engine.Resume();
        }

        private void CommandLinesToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Debugger == null)
            {
                Debugger = new DebuggerForm();
                Debugger.Show();
            }
            else if (!Debugger.Visible)
            {
                Debugger = new DebuggerForm();
                Debugger.Show();
            }
            Debugger.WriteLine("LIST OF COMMAND LINES CURRENTLY AVAILABLE :", DebugStatus.Cool);
            Debugger.WriteLine("* -cm : show this list !!", DebugStatus.None);
            Debugger.WriteLine("* -ls x : load the state from slot number x (x = slot number from 0 to 9)",
                               DebugStatus.None);
            Debugger.WriteLine("* -ss x : select the state slot number x (x = slot number from 0 to 9)",
                               DebugStatus.None);
            Debugger.WriteLine("* -pal : select the PAL region", DebugStatus.None);
            Debugger.WriteLine("* -ntsc : select the NTSC region", DebugStatus.None);
            Debugger.WriteLine("* -st x : Enable / Disable no limiter (x=0 disable, x=1 enable)", DebugStatus.None);
            Debugger.WriteLine(
                "* -s x : switch the size (x=x1 use size X1 '256x240', x=x2 use size X2 '512x480', x=str use Stretch)",
                DebugStatus.None);
            Debugger.WriteLine("* -r <WavePath> : record the audio wave into <WavePath>", DebugStatus.None);
            Debugger.WriteLine("* -p <PaletteFilePath> : load the palette <PaletteFilePath> and use it",
                               DebugStatus.None);
            Debugger.WriteLine("* -w_size w h : resize the window (W=width, h=height)", DebugStatus.None);
            Debugger.WriteLine(
                "* -w_pos x y : move the window into a specific location (x=X coordinate, y=Y coordinate)",
                DebugStatus.None);
            Debugger.WriteLine("* -w_max : maximize the window", DebugStatus.None);
            Debugger.WriteLine("* -w_min : minimize the window", DebugStatus.None);
        }

        private void HelpToolStripMenuItem1Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, ".\\Help.chm", HelpNavigator.TableOfContents);
        }

        private void RomInfoToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
            {
                _engine.Pause();
                var RR = new Frm_RomInfo(_engine.Memory.Map.Cartridge.RomPath);
                RR.ShowDialog(this);
                _engine.Resume();
            }
        }

        private void UiToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            var ab = new AboutForm(Program.AssemblyVersion);
            ab.ShowDialog(this);
            if (_engine != null)
                _engine.Resume();
        }

        private void GeneralToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            var ge = new Frm_GeneralOptions();
            ge.ShowDialog(this);
            if (_engine != null)
            {
                _engine.AutoSaveSram = Program.Settings.AutoSaveSRAM;
                _engine.Resume();
            }
        }

        private void SoftResetToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
            {
                Debug.WriteLine(this, "SOFT RESET !!", DebugStatus.Warning);
                _engine.SoftReset();
            }
        }

        private void IndexToolStripMenuItemClick(object sender, EventArgs e)
        {
            Help.ShowHelp(this, ".\\Help.chm", HelpNavigator.Index);
        }

        private void SaveSramToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_engine != null)
                _engine.Pause();
            else
                return;
            if (!_engine.Memory.Map.Cartridge.IsSaveRam)
            {
                MessageBox.Show("Not a S-RAM !!");
                return;
            }
            var sav = new SaveFileDialog {Title = "Save S-RAM", Filter = "Save file (*.sav)|*.sav"};
            if (sav.ShowDialog(this) == DialogResult.OK)
                _engine.SaveSram(sav.FileName);
            if (_engine != null)
                _engine.Resume();
        }

        private void FrmMainDeactivate(object sender, EventArgs e)
        {
            if (_engine != null & Program.Settings.PauseWhenFocusLost)
                _engine.Pause();
        }

        private void FrmMainActivated(object sender, EventArgs e)
        {
            if (_engine != null & Program.Settings.PauseWhenFocusLost)
                _engine.Resume();
        }

        //Zapper Trigger
        private void Panel1MouseDown(object sender, MouseEventArgs e)
        {
            if (_engine != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    var x = (e.X*256)/panel1.Width;
                    var y = (e.Y*((_engine.TvFormat == TvFormat.Ntsc) ? 224 : 240))/panel1.Height;
                    _engine.Memory.Zapper.PullTrigger(true, x, y);
                }
            }
        }

        private void Panel1MouseUp(object sender, MouseEventArgs e)
        {
            if (_engine != null)
                _engine.Memory.Zapper.PullTrigger(false, 0, 0);
        }

        private void ShowMenuAndStatusStripToolStripMenuItemCheckedChanged(object sender, EventArgs e)
        {
            menuStrip1.Visible = showMenuAndStatusStripToolStripMenuItem.Checked;
            statusStrip1.Visible = showMenuAndStatusStripToolStripMenuItem.Checked;
        }

        private void SupportedMappersToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Debugger == null)
            {
                Debugger = new DebuggerForm();
                Debugger.Show();
            }
            else if (!Debugger.Visible)
            {
                Debugger = new DebuggerForm();
                Debugger.Show();
            }
            Debugger.WriteLine("SUPPORTED MAPPERS :", DebugStatus.Cool);
            foreach (var map in NesHeaderReader.SupportedMappersNo)
            {
                Debugger.WriteLine("M # " + map.ToString(CultureInfo.InvariantCulture) + " " + @"""" +
                                   NesHeaderReader.GetMapperName(map) + @"""", DebugStatus.None);
            }
        }
    }
}