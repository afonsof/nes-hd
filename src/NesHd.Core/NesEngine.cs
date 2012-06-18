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
        public NesEngine(TvFormat tvFormat, PaletteFormat plFormat)
        {
            AutoSaveSram = true;
            Debug.WriteLine(this, "Initializeing the nes emulation engine...", DebugStatus.None);
            TvFormat = tvFormat;
            //Initialize Engine
            Memory = new Memory.Memory(this);
            Cpu = new Cpu(Memory, tvFormat, this);
            Cpu.PauseToggle += CpuPauseToggle;
            Ppu = new Ppu(tvFormat, plFormat, this);
            Debug.WriteLine(this, "Nes initialized ok.", DebugStatus.Cool);
        }

        public TvFormat TvFormat { get; private set; }

        public Cpu Cpu { get; private set; }

        public Memory.Memory Memory { get; private set; }

        public Ppu Ppu { get; set; }

        public Apu Apu { get; set; }

        public bool SoundEnabled { get; set; }

        public bool AutoSaveSram { get; set; }

        private void CpuPauseToggle(object sender, EventArgs e)
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
            if (Memory.LoadCart(romPath))
            {
                Ppu.Bitmap = Memory.Map.Cartridge.Bitmap;
                Ppu.BitmapOffset = Memory.Map.Cartridge.BitmapOffset;
                Ppu.BitmapWidth = Memory.Map.Cartridge.BitmapWidth;
                return true;
            }
            return false;
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
            if (Memory.Map.Cartridge.IsSaveRam & AutoSaveSram)
                SaveSram(Memory.Map.Cartridge.SaveFilename);
            Cpu.ON = false;
        }

        public void SoftReset()
        {
            Cpu.SoftReset();
        }

        public void SaveSram(string filePath)
        {
            //If we have save RAM, try to save it
            try
            {
                using (var writer = File.OpenWrite(filePath))
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

        public void SetupInput(InputManager manager, Joypad joy1, Joypad joy2)
        {
            Memory.InputManager = manager;
            Memory.Joypad1 = joy1;
            Memory.Joypad2 = joy2;
        }

        public void SetupOutput(IGraphicDevice videoDevice, IAudioDevice audioDevice)
        {
            Ppu.OutputDevice = videoDevice;
            Apu = new Apu(this, audioDevice);
        }

        public void SetupTv(TvFormat tvFormat, PaletteFormat palleteFormat)
        {
            Pause();
            Cpu.SetTvFormat(tvFormat);
            Ppu.SetTvFormat(tvFormat, palleteFormat);
        }

        //Properties

        public event EventHandler<EventArgs> PauseToggle;
    }

    public enum TvFormat
    {
        Ntsc,
        Pal
    }
}