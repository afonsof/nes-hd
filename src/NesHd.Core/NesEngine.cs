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
using System.IO;

using NesHd.Core.Debugger;
using NesHd.Core.Input;
using NesHd.Core.Output.Audio;
using NesHd.Core.Output.Video;
using NesHd.Core.PPU;

namespace NesHd.Core
{
    /// <summary>
    /// The main class of nes machine we emulate
    /// </summary>
    public class NesEngine
    {
        Memory.Memory _MEMORY;
        CPU.CPU _CPU;
        PPU.PPU _PPU;
        APU.APU _APU;
        TVFORMAT _TvFormat = TVFORMAT.NTSC;
        bool _SoundEnabled;
        bool _AutoSaveSRAM = true;
        public NesEngine(TVFORMAT TvFormat, PaletteFormat PlFormat)
        {
            Debug.WriteLine(this, "Initializeing the nes emulation engine...", DebugStatus.None);
            this._TvFormat = TvFormat;
            //Initialize Engine
            this._MEMORY = new Memory.Memory(this);
            this._CPU = new CPU.CPU(this._MEMORY, TvFormat, this);
            this._CPU.PauseToggle += new EventHandler<EventArgs>(this._CPU_PauseToggle);
            this._PPU = new PPU.PPU(TvFormat, PlFormat, this);
            Debug.WriteLine(this, "Nes initialized ok.", DebugStatus.Cool);
        }
        void _CPU_PauseToggle(object sender, EventArgs e)
        {
            //Rise the pause event
            if (this.PauseToggle != null)
                this.PauseToggle(this, null);
        }
        /// <summary>
        /// Load a rom into the memory
        /// </summary>
        /// <param name="RomPath">The INES rom path</param>
        public bool LoadRom(string RomPath)
        {
            return this._MEMORY.LoadCart(RomPath);
        }
        /// <summary>
        /// Run the Nes
        /// </summary>
        public void Run()
        {
            this._CPU.Reset();
            this._CPU.ON = true;
            this._CPU.Run();
            this._CPU.ON = false;
        }
        /// <summary>
        /// Toggle pause the nes
        /// </summary>
        public void TogglePause()
        {
            this._CPU.TogglePause();
        }
        public void Pause()
        {
            this._CPU.Pause = true;
            //Rise the pause event
            if (this.PauseToggle != null)
                this.PauseToggle(this, null);
        }
        public void Resume()
        {
            this._CPU.Pause = false;
            //Rise the pause event
            if (this.PauseToggle != null)
                this.PauseToggle(this, null);
        }
        public void ShutDown()
        {
            while (this._APU.IsRendering)
            { }
            this._APU.Pause();
            this._APU.Shutdown();
            if (this._APU.RECODER.IsRecording)
                this._APU.RECODER.Stop();
            if (this._MEMORY.MAP.Cartridge.IsSaveRam & this._AutoSaveSRAM)
                this.SaveSRAM(this._MEMORY.MAP.Cartridge.saveFilename);
            this._CPU.ON = false;
        }
        public void SoftReset()
        {
            this._CPU.SoftReset();
        }
        public void SaveSRAM(string FilePath)
        {
            //If we have save RAM, try to save it
            try
            {
                using (FileStream writer = File.OpenWrite(FilePath))
                {
                    writer.Write(this._MEMORY.SRAM, 0, 0x2000);
                    Debug.WriteLine(this, "SRAM saved !!", DebugStatus.Cool);
                }
            }
            catch
            {
                Debug.WriteLine(this, "Could not save S-RAM.", DebugStatus.Error);
            }
        }
        public void SetupInput(InputManager Manager, Joypad Joy1, Joypad Joy2)
        {
            this._MEMORY.InputManager = Manager;
            this._MEMORY.Joypad1 = Joy1;
            this._MEMORY.Joypad2 = Joy2;
        }
        public void SetupOutput(IGraphicDevice VideoDevice, IAudioDevice AudioDevice)
        {
            this._PPU.OutputDevice = VideoDevice;
            this._APU = new APU.APU(this, AudioDevice);
        }
        public void SetupTV(TVFORMAT TvFormat, PaletteFormat PLFormat)
        {
            this.Pause();
            this._CPU.SetTVFormat(TvFormat);
            this._PPU.SetTVFormat(TvFormat, PLFormat);
        }
        //Properties
        public TVFORMAT TvFormat
        { get { return this._TvFormat; } }
        public CPU.CPU CPU
        { get { return this._CPU; } }
        public Memory.Memory MEMORY
        { get { return this._MEMORY; } }
        public PPU.PPU PPU
        { get { return this._PPU; } set { this._PPU = value; } }
        public APU.APU APU
        { get { return this._APU; } set { this._APU = value; } }
        public bool SoundEnabled
        { get { return this._SoundEnabled; } set { this._SoundEnabled = value; } }
        public event EventHandler<EventArgs> PauseToggle;
        public bool AutoSaveSRAM
        { get { return this._AutoSaveSRAM; } set { this._AutoSaveSRAM = value; } }
    }
    public enum TVFORMAT
    { NTSC, PAL }
}
