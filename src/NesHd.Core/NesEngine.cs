using System;
using System.IO;
using NesHd.Core.APU;
using NesHd.Core.CPU;
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
        public NesEngine(TVFORMAT TvFormat, PaletteFormat PlFormat)
        {
            AutoSaveSRAM = true;
            Debug.WriteLine(this, "Initializeing the nes emulation engine...", DebugStatus.None);
            this.TvFormat = TvFormat;
            //Initialize Engine
            Memory = new Memory.Memory(this);
            Cpu = new Cpu(Memory, TvFormat, this);
            Cpu.PauseToggle += _CPU_PauseToggle;
            Ppu = new Ppu(TvFormat, PlFormat, this);
            Debug.WriteLine(this, "Nes initialized ok.", DebugStatus.Cool);
        }

        public TVFORMAT TvFormat { get; private set; }

        public Cpu Cpu { get; private set; }

        public Memory.Memory Memory { get; private set; }

        public Ppu Ppu { get; set; }

        public Apu Apu { get; set; }

        public bool SoundEnabled { get; set; }

        public bool AutoSaveSRAM { get; set; }

        private void _CPU_PauseToggle(object sender, EventArgs e)
        {
            //Rise the pause event
            if (PauseToggle != null)
                PauseToggle(this, null);
        }

        /// <summary>
        /// Load a rom into the memory
        /// </summary>
        /// <param name="romPath">The INES rom path</param>
        public bool LoadRom(string romPath)
        {
            return Memory.LoadCart(romPath);
        }

        /// <summary>
        /// Run the Nes
        /// </summary>
        public void Run()
        {
            Cpu.Reset();
            Cpu.ON = true;
            Cpu.Run();
            Cpu.ON = false;
        }

        /// <summary>
        /// Toggle pause the nes
        /// </summary>
        public void TogglePause()
        {
            Cpu.TogglePause();
        }

        public void Pause()
        {
            Cpu.Pause = true;
            //Rise the pause event
            if (PauseToggle != null)
                PauseToggle(this, null);
        }

        public void Resume()
        {
            Cpu.Pause = false;
            //Rise the pause event
            if (PauseToggle != null)
                PauseToggle(this, null);
        }

        public void ShutDown()
        {
            while (Apu.IsRendering)
            {
            }
            Apu.Pause();
            Apu.Shutdown();
            if (Apu.RECODER.IsRecording)
                Apu.RECODER.Stop();
            if (Memory.Map.Cartridge.IsSaveRam & AutoSaveSRAM)
                SaveSRAM(Memory.Map.Cartridge.SaveFilename);
            Cpu.ON = false;
        }

        public void SoftReset()
        {
            Cpu.SoftReset();
        }

        public void SaveSRAM(string FilePath)
        {
            //If we have save RAM, try to save it
            try
            {
                using (var writer = File.OpenWrite(FilePath))
                {
                    writer.Write(Memory.SRam, 0, 0x2000);
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
            Memory.InputManager = Manager;
            Memory.Joypad1 = Joy1;
            Memory.Joypad2 = Joy2;
        }

        public void SetupOutput(IGraphicDevice VideoDevice, IAudioDevice AudioDevice)
        {
            Ppu.OutputDevice = VideoDevice;
            Apu = new Apu(this, AudioDevice);
        }

        public void SetupTv(TVFORMAT tvFormat, PaletteFormat palleteFormat)
        {
            Pause();
            Cpu.SetTVFormat(tvFormat);
            Ppu.SetTvFormat(tvFormat, palleteFormat);
        }

        //Properties

        public event EventHandler<EventArgs> PauseToggle;
    }

    public enum TVFORMAT
    {
        NTSC,
        PAL
    }
}