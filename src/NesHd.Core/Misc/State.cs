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
using System.Runtime.Serialization.Formatters.Binary;

using NesHd.Core.Memory;
using NesHd.Core.Memory.Mappers;

namespace NesHd.Core.Misc
{
    public class State
    {
        NesEngine _Nes;
        public StateHolder Holder = new StateHolder();
        /// <summary>
        /// The state saver / loader
        /// </summary>
        /// <param name="NesEmu">The current system you want to save / load state from / into</param>
        public State(NesEngine NesEmu)
        {
            this._Nes = NesEmu;
        }
        public bool SaveState(string FilePath)
        {
            try
            {
                this._Nes.Pause();
                this.Holder.LoadNesData(this._Nes);
                FileStream fs = new FileStream(FilePath, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, this.Holder);
                fs.Close();
                this._Nes.Resume();
                return true;
            }
            catch { return false; }
        }
        public bool LoadState(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    this._Nes.Pause();
                    FileStream fs = new FileStream(FilePath, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    this.Holder = (StateHolder)formatter.Deserialize(fs);
                    fs.Close();
                    this.Holder.ApplyDataToNes(this._Nes);
                    this._Nes.Resume();
                    return true;
                }
                return false;
            }
            catch { return false; };
        }
    }
    [Serializable()]
    public class StateHolder
    {
        #region CPU
        byte REG_A = 0;
        byte REG_X = 0;
        byte REG_Y = 0;
        byte REG_S = 0;
        ushort REG_PC = 0;
        bool Flag_N = false;
        bool Flag_V = false;
        bool Flag_B = false;
        bool Flag_D = false;
        bool Flag_I = true;
        bool Flag_Z = false;
        bool Flag_C = false;
        public int CycleCounter = 0;
        int CyclesPerScanline = 0;
        byte OpCode = 0;
        ushort PrevPC = 0;
        #endregion
        #region MEMORY
        byte[] _RAM;
        byte[] _SRAM;
        int JoyData1 = 0;
        int JoyData2 = 0;
        byte JoyStrobe = 0;
        #endregion
        #region CART
        byte[][] _CHR;
        MIRRORING _MIRRORING;
        bool _save_ram_present;
        bool _is_vram;
        uint _mirroringBase;
        #endregion
        #region MAP
        public uint[] current_prg_rom_page;
        public uint[] current_chr_rom_page;
        #endregion
        #region PPU
        byte[] SPRRAM;
        byte[] VRAM;
        int CurrentScanLine = 0;
        ushort VRAMAddress = 0;
        bool Sprite0Hit = false;
        int SpriteCrossed = 0;
        int ScanlinesPerFrame = 0;
        int ScanlineOfVBLANK = 0;
        int _FPS = 0;
        bool VBLANK = false;
        byte VRAMReadBuffer = 0;
        bool _NoLimiter = false;
        /*2000*/
        bool ExecuteNMIonVBlank = false;
        bool SpriteSize = false;//true=8x16, false=8x8
        int PatternTableAddressBackground = 0;
        int PatternTableAddress8x8Sprites = 0;
        int VRAMAddressIncrement = 1;
        byte ReloadBits2000 = 0;
        /*2001*/
        ushort ColorEmphasis = 0;
        bool SpriteVisibility = false;
        bool BackgroundVisibility = false;
        bool SpriteClipping = false;
        bool BackgroundClipping = false;
        bool MonochromeMode = false;
        /*2003*/
        byte SpriteRamAddress = 0;
        /*2005,2006*/
        bool PPUToggle = false;
        ushort VRAMTemp = 0;
        /*Draw stuff*/
        byte HScroll = 0;
        int VScroll = 0;
        int VBits = 0;
        int TileY = 0;
        #endregion
        #region APU
        int _FrameCounter = 0;
        bool _PAL;
        bool DMCIRQPending = false;
        bool FrameIRQEnabled = false;
        bool FrameIRQPending = false;
        //DMC
        public double DMC_Frequency = 0;
        public double DMC_RenderedLength = 0;
        public double DMC_SampleCount = 0;
        public bool DMCDMCIRQEnabled = false;
        public bool DMC_Enabled = false;
        public bool DMC_Loop = false;
        public double DMC_FreqTimer = 0;
        public byte DMCDAC = 0;
        public ushort DMCDMAStartAddress = 0;
        public ushort DMCDMAAddress = 0;
        public ushort DMCDMALength = 0;
        public ushort DMCDMALengthCounter = 0;
        public byte DMCDMCBIT = 0;
        public byte DMCDMCBYTE = 0;
        //NOIZE
        public bool NOIZE_Enabled;
        public byte NOIZE_Volume = 0;
        public byte NOIZE_Envelope = 0;
        public double NOIZE_Frequency = 0;
        public double NOIZE_SampleCount = 0;
        public double NOIZE_RenderedLength = 0;
        public double NOIZE_FreqTimer = 0;
        public byte NOIZE_LengthCount = 0;
        public ushort NOIZE_ShiftReg = 1;
        public byte NOIZE_DecayCount = 0;
        public byte NOIZE_DecayTimer = 0;
        public bool NOIZE_DecayDiable;
        public int NOIZE_NoiseMode;
        public bool NOIZE_DecayLoopEnable;
        public bool NOIZE_DecayReset = false;
        public short NOIZEOUT = 0;
        //Rectangle1
        public bool Rectangle1_Enabled;
        public byte Rectangle1_Volume = 0;
        public byte Rectangle1_Envelope = 0;
        public double Rectangle1_Frequency = 0;
        public double Rectangle1_SampleCount = 0;
        public double Rectangle1_RenderedLength = 0;
        public int Rectangle1_DutyCycle = 0;
        public int Rectangle1_FreqTimer = 0;
        public byte Rectangle1_DecayCount = 0;
        public byte Rectangle1_DecayTimer = 0;
        public bool Rectangle1_DecayDiable;
        public bool Rectangle1_DecayReset;
        public bool Rectangle1_DecayLoopEnable;
        public byte Rectangle1_LengthCount = 0;
        public byte Rectangle1_SweepShift = 0;
        public bool Rectangle1_SweepDirection;
        public byte Rectangle1_SweepRate = 0;
        public bool Rectangle1_SweepEnable;
        public byte Rectangle1_SweepCount = 0;
        public bool Rectangle1_SweepReset;
        public bool Rectangle1_SweepForceSilence;
        public double Rectangle1DutyPercentage = 0;
        public bool Rectangle1WaveStatus;
        //Rectangle2
        public bool Rectangle2_Enabled;
        public byte Rectangle2_Volume = 0;
        public byte Rectangle2_Envelope = 0;
        public double Rectangle2_Frequency = 0;
        public double Rectangle2_SampleCount = 0;
        public double Rectangle2_RenderedLength = 0;
        public int Rectangle2_DutyCycle = 0;
        public int Rectangle2_FreqTimer = 0;
        public byte Rectangle2_DecayCount = 0;
        public byte Rectangle2_DecayTimer = 0;
        public bool Rectangle2_DecayDiable;
        public bool Rectangle2_DecayReset;
        public bool Rectangle2_DecayLoopEnable;
        public byte Rectangle2_LengthCount = 0;
        public byte Rectangle2_SweepShift = 0;
        public bool Rectangle2_SweepDirection;
        public byte Rectangle2_SweepRate = 0;
        public bool Rectangle2_SweepEnable;
        public byte Rectangle2_SweepCount = 0;
        public bool Rectangle2_SweepReset;
        public bool Rectangle2_SweepForceSilence;
        public double Rectangle2DutyPercentage = 0;
        public bool Rectangle2WaveStatus;
        //Triangle
        public double Triangle_Frequency = 0;
        public double Triangle_SampleCount = 0;
        public double Triangle_RenderedLength = 0;
        public int Triangle_FreqTimer = 0;
        public byte Triangle_LengthCount = 0;
        public int Triangle_LinearCounter = 0;
        public int Triangle_LinearCounterLoad = 0;
        public bool Triangle_LinearControl;
        public bool Triangle_LengthEnabled;
        public int Triangle_Sequence = 0;
        public bool TriangleHALT;
        public bool Triangle_Enabled;
        public short TriangleOUT = 0;
        //VRC6 Pulse 1
        public byte VRC6Pulse1_Volume = 0;
        public double VRC6Pulse1DutyPercentage = 0;
        public int VRC6Pulse1_DutyCycle = 0;
        public int VRC6Pulse1_FreqTimer = 0;
        public bool VRC6Pulse1_Enabled = false;
        public double VRC6Pulse1_Frequency = 0;
        public double VRC6Pulse1_SampleCount = 0;
        public double VRC6Pulse1_RenderedLength = 0;
        public bool VRC6Pulse1WaveStatus = false;
        public short VRC6Pulse1OUT = 0;
        //VRC6 Pulse 2
        public byte VRC6Pulse2_Volume = 0;
        public double VRC6Pulse2DutyPercentage = 0;
        public int VRC6Pulse2_DutyCycle = 0;
        public int VRC6Pulse2_FreqTimer = 0;
        public bool VRC6Pulse2_Enabled = false;
        public double VRC6Pulse2_Frequency = 0;
        public double VRC6Pulse2_SampleCount = 0;
        public double VRC6Pulse2_RenderedLength = 0;
        public bool VRC6Pulse2WaveStatus = false;
        public short VRC6Pulse2OUT = 0;
        //VRC6 Sawtooth
        public byte VRC6SawtoothAccumRate = 0;
        public byte VRC6SawtoothAccumStep = 0;
        public byte VRC6SawtoothAccum = 0;
        public int VRC6Sawtooth_FreqTimer = 0;
        public bool VRC6Sawtooth_Enabled = false;
        public double VRC6Sawtooth_Frequency = 0;
        public double VRC6Sawtooth_SampleCount = 0;
        public double VRC6Sawtooth_RenderedLength = 0;
        public short VRC6SawtoothOUT = 0;
        #endregion
        #region MAPPERS
        //MAPPER 1
        int mapper1_register8000BitPosition;
        int mapper1_registerA000BitPosition;
        int mapper1_registerC000BitPosition;
        int mapper1_registerE000BitPosition;
        int mapper1_register8000Value;
        int mapper1_registerA000Value;
        int mapper1_registerC000Value;
        int mapper1_registerE000Value;
        byte mapper1_mirroringFlag;
        byte mapper1_onePageMirroring;
        byte mapper1_prgSwitchingArea;
        byte mapper1_prgSwitchingSize;
        byte mapper1_vromSwitchingSize;
        //MAPPER 4
        int mapper4_commandNumber;
        int mapper4_prgAddressSelect;
        int mapper4_chrAddressSelect;
        bool mapper4_timer_irq_enabled;
        uint mapper4_timer_irq_count;
        uint mapper4_timer_irq_reload;
        //MAPPER 5
        byte mapper5_prgBankSize;
        byte mapper5_chrBankSize;
        int mapper5_scanlineSplit;
        bool mapper5_splitIrqEnabled;
        //MAPPER 6
        bool mapper6_IRQEnabled = false;
        int mapper6_irq_counter = 0;
        //MAPPER 8
        bool mapper8_IRQEnabled = false;
        int mapper8_irq_counter = 0;
        //MAPPER 9
        byte mapper9_latch1;
        byte mapper9_latch2;
        int mapper9_latch1data1;
        int mapper9_latch1data2;
        int mapper9_latch2data1;
        int mapper9_latch2data2;
        //MAPPER 10
        byte mapper10_latch1;
        byte mapper10_latch2;
        int mapper10_latch1data1;
        int mapper10_latch1data2;
        int mapper10_latch2data1;
        int mapper10_latch2data2;
        //MAPPER 16
        short timer_irq_counter_16 = 0;
        short timer_irq_Latch_16 = 0;
        bool timer_irq_enabled;
        //MAPPER 17
        bool mapper17_IRQEnabled = false;
        int mapper17_irq_counter = 0;
        //MAPPER 18
        short Mapper18_Timer = 0;
        short Mapper18_latch = 0;
        byte mapper18_control = 0;
        int Mapper18_IRQWidth = 0;
        bool Mapper18_timer_irq_enabled;
        byte[] Mapper18_x = new byte[22];
        //MAPPER 19
        bool Mapper19_VROMRAMfor0000 = false;
        bool Mapper19_VROMRAMfor1000 = false;
        short Mapper19_irq_counter = 0;
        bool Mapper19_IRQEnabled = false;
        //MAPPER 21
        bool Mapper21_PRGMode = true;
        byte[] Mapper21_REG = new byte[8];
        int Mapper21_irq_latch = 0;
        int Mapper21_irq_enable = 0;
        int Mapper21_irq_counter = 0;
        int Mapper21_irq_clock = 0;
        //MAPPER 23
        bool Mapper23_PRGMode = true;
        byte[] Mapper23_REG = new byte[8];
        int Mapper23_irq_latch = 0;
        int Mapper23_irq_enable = 0;
        int Mapper23_irq_counter = 0;
        int Mapper23_irq_clock = 0;
        //MAPPER 24
        int Mapper24_irq_latch = 0;
        bool Mapper24_irq_enable = false;
        int Mapper24_irq_counter = 0;
        int Mapper24_irq_clock = 0;
        //MAPPER 225
        byte Mapper225_reg0 = 0xF;
        byte Mapper225_reg1 = 0xF;
        byte Mapper225_reg2 = 0xF;
        byte Mapper225_reg3 = 0xF;
        //MAPPER 32
        int mapper32SwitchingMode = 0;
        //MAPPER 33
        bool mapper33_type1 = true;
        byte mapper33_IRQCounter = 0;
        bool mapper33_IRQEabled;
        //MAPPER 41
        byte Mapper41_CHR_Low = 0;
        byte Mapper41_CHR_High = 0;
        //MAPPER 64
        byte mapper64_commandNumber;
        byte mapper64_prgAddressSelect;
        byte mapper64_chrAddressSelect;
        //MAPPER 65
        short mapper65_timer_irq_counter_65 = 0;
        short mapper65_timer_irq_Latch_65 = 0;
        bool mapper65_timer_irq_enabled;
        //MAPPER 69
        ushort mapper69_reg = 0;
        short mapper69_timer_irq_counter_69 = 0;
        bool mapper69_timer_irq_enabled;
        //MAPPER 91
        bool mapper91_IRQEnabled;
        int mapper91_IRQCount = 0;
        #endregion
        public void LoadNesData(NesEngine _Nes)
        {
            #region CPU
            this.REG_A = _Nes.CPU.REG_A;
            this.REG_X = _Nes.CPU.REG_X;
            this.REG_Y = _Nes.CPU.REG_Y;
            this.REG_S = _Nes.CPU.REG_S;
            this.REG_PC = _Nes.CPU.REG_PC;
            this.Flag_N = _Nes.CPU.Flag_N;
            this.Flag_V = _Nes.CPU.Flag_V;
            this.Flag_B = _Nes.CPU.Flag_B;
            this.Flag_D = _Nes.CPU.Flag_D;
            this.Flag_I = _Nes.CPU.Flag_I;
            this.Flag_Z = _Nes.CPU.Flag_Z;
            this.Flag_C = _Nes.CPU.Flag_C;
            this.CycleCounter = _Nes.CPU.CycleCounter;
            this.CyclesPerScanline = _Nes.CPU.CyclesPerScanline;
            this.OpCode = _Nes.CPU.OpCode;
            this.PrevPC = _Nes.CPU.PrevPC;
            #endregion
            #region MEMORY
            this._RAM = _Nes.MEMORY.RAM;
            this._SRAM = _Nes.MEMORY.SRAM;
            this.JoyData1 = _Nes.MEMORY.JoyData1;
            this.JoyData2 = _Nes.MEMORY.JoyData2;
            this.JoyStrobe = _Nes.MEMORY.JoyStrobe;
            #endregion
            #region CART
            if (_Nes.MEMORY.MAP.Cartridge.CHR_PAGES == 0)
                this._CHR = _Nes.MEMORY.MAP.Cartridge.CHR;
            this._MIRRORING = _Nes.MEMORY.MAP.Cartridge.Mirroring;
            this._save_ram_present = _Nes.MEMORY.MAP.Cartridge.IsSaveRam;
            this._is_vram = _Nes.MEMORY.MAP.Cartridge.IsVRAM;
            this._mirroringBase = _Nes.MEMORY.MAP.Cartridge.MirroringBase;
            #endregion
            #region MAP
            this.current_prg_rom_page = _Nes.MEMORY.MAP.current_prg_rom_page;
            this.current_chr_rom_page = _Nes.MEMORY.MAP.current_chr_rom_page;
            #endregion
            #region PPU
            this.SPRRAM = _Nes.PPU.SPRRAM;
            this.VRAM = _Nes.PPU.VRAM;
            this.CurrentScanLine = _Nes.PPU.CurrentScanLine;
            this.VRAMAddress = _Nes.PPU.VRAMAddress;
            this.Sprite0Hit = _Nes.PPU.Sprite0Hit;
            this.SpriteCrossed = _Nes.PPU.SpriteCrossed;
            this.ScanlinesPerFrame = _Nes.PPU.ScanlinesPerFrame;
            this.ScanlineOfVBLANK = _Nes.PPU.ScanlineOfVBLANK;
            this._FPS = _Nes.PPU._FPS;
            this.VBLANK = _Nes.PPU.VBLANK;
            this.VRAMReadBuffer = _Nes.PPU.VRAMReadBuffer;
            this._NoLimiter = _Nes.PPU._NoLimiter;
            /*2000*/
            this.ExecuteNMIonVBlank = _Nes.PPU.ExecuteNMIonVBlank;
            this.SpriteSize = _Nes.PPU.SpriteSize;
            this.PatternTableAddressBackground = _Nes.PPU.PatternTableAddressBackground;
            this.PatternTableAddress8x8Sprites = _Nes.PPU.PatternTableAddress8x8Sprites;
            this.VRAMAddressIncrement = _Nes.PPU.VRAMAddressIncrement;
            this.ReloadBits2000 = _Nes.PPU.ReloadBits2000;
            /*2001*/
            this.ColorEmphasis = _Nes.PPU.ColorEmphasis;
            this.SpriteVisibility = _Nes.PPU.SpriteVisibility;
            this.BackgroundVisibility = _Nes.PPU.BackgroundVisibility;
            this.SpriteClipping = _Nes.PPU.SpriteClipping;
            this.BackgroundClipping = _Nes.PPU.BackgroundClipping;
            this.MonochromeMode = _Nes.PPU.MonochromeMode;
            /*2003*/
            this.SpriteRamAddress = _Nes.PPU.SpriteRamAddress;
            /*2005,2006*/
            this.PPUToggle = _Nes.PPU.PPUToggle;
            this.VRAMTemp = _Nes.PPU.VRAMTemp;
            /*Draw stuff*/
            this.HScroll = _Nes.PPU.HScroll;
            this.VScroll = _Nes.PPU.VScroll;
            this.VBits = _Nes.PPU.VBits;
            this.TileY = _Nes.PPU.TileY;
            #endregion
            #region APU
            this._FrameCounter = _Nes.APU._FrameCounter;
            this._PAL = _Nes.APU._PAL;
            this.DMCIRQPending = _Nes.APU.DMCIRQPending;
            this.FrameIRQEnabled = _Nes.APU.FrameIRQEnabled;
            this.FrameIRQPending = _Nes.APU.FrameIRQPending;
            _Nes.APU.DMC.SaveState(this);
            _Nes.APU.NOIZE.SaveState(this);
            _Nes.APU.RECT1.SaveState(this);
            _Nes.APU.RECT2.SaveState(this);
            _Nes.APU.TRIANGLE.SaveState(this);
            _Nes.APU.VRC6PULSE1.SaveState(this);
            _Nes.APU.VRC6PULSE2.SaveState(this);
            _Nes.APU.VRC6SAWTOOTH.SaveState(this);
            #endregion
            #region Mappers
            //MAPPER 1
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 1)
            {
                Mapper01 map1 = (Mapper01)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper1_register8000BitPosition = map1.mapper1_register8000BitPosition;
                this.mapper1_registerA000BitPosition = map1.mapper1_registerA000BitPosition;
                this.mapper1_registerC000BitPosition = map1.mapper1_registerC000BitPosition;
                this.mapper1_registerE000BitPosition = map1.mapper1_registerE000BitPosition;
                this.mapper1_register8000Value = map1.mapper1_register8000Value;
                this.mapper1_registerA000Value = map1.mapper1_registerA000Value;
                this.mapper1_registerC000Value = map1.mapper1_registerC000Value;
                this.mapper1_registerE000Value = map1.mapper1_registerE000Value;
                this.mapper1_mirroringFlag = map1.mapper1_mirroringFlag;
                this.mapper1_onePageMirroring = map1.mapper1_onePageMirroring;
                this.mapper1_prgSwitchingArea = map1.mapper1_prgSwitchingArea;
                this.mapper1_prgSwitchingSize = map1.mapper1_prgSwitchingSize;
                this.mapper1_vromSwitchingSize = map1.mapper1_vromSwitchingSize;
            }
            //MAPPER 4
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 4)
            {
                Mapper04 map4 = (Mapper04)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper4_commandNumber = map4.mapper4_commandNumber;
                this.mapper4_prgAddressSelect = map4.mapper4_prgAddressSelect;
                this.mapper4_chrAddressSelect = map4.mapper4_chrAddressSelect;
                this.mapper4_timer_irq_enabled = map4.timer_irq_enabled;
                this.mapper4_timer_irq_count = map4.timer_irq_count;
                this.mapper4_timer_irq_reload = map4.timer_irq_reload;
            }
            //MAPPER 5
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 5)
            {
                Mapper05 map5 = (Mapper05)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper5_prgBankSize = map5.mapper5_prgBankSize;
                this.mapper5_chrBankSize = map5.mapper5_chrBankSize;
                this.mapper5_scanlineSplit = map5.mapper5_scanlineSplit;
                this.mapper5_splitIrqEnabled = map5.mapper5_splitIrqEnabled;
            }
            //MAPPER 6
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 6)
            {
                Mapper06 map6 = (Mapper06)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper6_IRQEnabled = map6.IRQEnabled;
                this.mapper6_irq_counter = map6.irq_counter;
            }
            //MAPPER 8
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 8)
            {
                Mapper08 map8 = (Mapper08)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper8_IRQEnabled = map8.IRQEnabled;
                this.mapper8_irq_counter = map8.irq_counter;
            }
            //MAPPER 9
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 9)
            {
                Mapper09 map9 = (Mapper09)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper9_latch1 = map9.latch1;
                this.mapper9_latch2 = map9.latch2;
                this.mapper9_latch1data1 = map9.latch1data1;
                this.mapper9_latch1data2 = map9.latch1data2;
                this.mapper9_latch2data1 = map9.latch2data1;
                this.mapper9_latch2data2 = map9.latch2data2;
            }
            //MAPPER 10
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 10)
            {
                Mapper10 map10 = (Mapper10)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper10_latch1 = map10.latch1;
                this.mapper10_latch2 = map10.latch2;
                this.mapper10_latch1data1 = map10.latch1data1;
                this.mapper10_latch1data2 = map10.latch1data2;
                this.mapper10_latch2data1 = map10.latch2data1;
                this.mapper10_latch2data2 = map10.latch2data2;
            }
            //MAPPER 16
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 16)
            {
                Mapper16 map16 = (Mapper16)_Nes.MEMORY.MAP.CurrentMapper;
                this.timer_irq_counter_16 = map16.timer_irq_counter_16;
                this.timer_irq_Latch_16 = map16.timer_irq_Latch_16;
                this.timer_irq_enabled = map16.timer_irq_enabled;
            }
            //MAPPER 17
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 17)
            {
                Mapper17 map17 = (Mapper17)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper17_IRQEnabled = map17.IRQEnabled;
                this.mapper17_irq_counter = map17.irq_counter;
            }
            //MAPPER 18
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 18)
            {
                Mapper18 map18 = (Mapper18)_Nes.MEMORY.MAP.CurrentMapper;
                this.Mapper18_Timer = map18.Mapper18_Timer;
                this.Mapper18_latch = map18.Mapper18_latch;
                this.mapper18_control = map18.mapper18_control;
                this.Mapper18_IRQWidth = map18.Mapper18_IRQWidth;
                this.Mapper18_timer_irq_enabled = map18.timer_irq_enabled;
                this.Mapper18_x = map18.x;
            }
            //MAPPER 19
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 19)
            {
                Mapper19 map19 = (Mapper19)_Nes.MEMORY.MAP.CurrentMapper;
                this.Mapper19_VROMRAMfor0000 = map19.VROMRAMfor0000;
                this.Mapper19_VROMRAMfor1000 = map19.VROMRAMfor1000;
                this.Mapper19_irq_counter = map19.irq_counter;
                this.Mapper19_IRQEnabled = map19.IRQEnabled;
            }
            //MAPPER 21
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 21)
            {
                Mapper21 map21 = (Mapper21)_Nes.MEMORY.MAP.CurrentMapper;
                this.Mapper21_PRGMode = map21.PRGMode;
                this.Mapper21_REG = map21.REG;
                this.Mapper21_irq_latch = map21.irq_latch;
                this.Mapper21_irq_enable = map21.irq_enable;
                this.Mapper21_irq_counter = map21.irq_counter;
                this.Mapper21_irq_clock = map21.irq_clock;
            }
            //MAPPER 23
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 23)
            {
                Mapper23 map23 = (Mapper23)_Nes.MEMORY.MAP.CurrentMapper;
                this.Mapper23_PRGMode = map23.PRGMode;
                this.Mapper23_REG = map23.REG;
                this.Mapper23_irq_latch = map23.irq_latch;
                this.Mapper23_irq_enable = map23.irq_enable;
                this.Mapper23_irq_counter = map23.irq_counter;
                this.Mapper23_irq_clock = map23.irq_clock;
            }
            //MAPPER 24
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 24)
            {
                Mapper24 map24 = (Mapper24)_Nes.MEMORY.MAP.CurrentMapper;
                this.Mapper24_irq_latch = map24.irq_latch;
                this.Mapper24_irq_enable = map24.irq_enable;
                this.Mapper24_irq_counter = map24.irq_counter;
                this.Mapper24_irq_clock = map24.irq_clock;
            }
            //MAPPER 225
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 225)
            {
                Mapper225_255 map225 = (Mapper225_255)_Nes.MEMORY.MAP.CurrentMapper;
                this.Mapper225_reg0 = map225.Mapper225_reg0;
                this.Mapper225_reg1 = map225.Mapper225_reg1;
                this.Mapper225_reg2 = map225.Mapper225_reg2;
                this.Mapper225_reg3 = map225.Mapper225_reg3;
            }
            //MAPPER 32
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 32)
            {
                Mapper32 map32 = (Mapper32)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper32SwitchingMode = map32.mapper32SwitchingMode;
            }
            //MAPPER 33
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 33)
            {
                Mapper33 map33 = (Mapper33)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper33_type1 = map33.type1;
                this.mapper33_IRQCounter = map33.IRQCounter;
                this.mapper33_IRQEabled = map33.IRQEabled;
            }
            //MAPPER 41
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 41)
            {
                Mapper41 map41 = (Mapper41)_Nes.MEMORY.MAP.CurrentMapper;
                this.Mapper41_CHR_Low = map41.Mapper41_CHR_Low;
                this.Mapper41_CHR_High = map41.Mapper41_CHR_High;
            }
            //MAPPER 64
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 64)
            {
                Mapper64 map64 = (Mapper64)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper64_commandNumber = map64.mapper64_commandNumber;
                this.mapper64_prgAddressSelect = map64.mapper64_prgAddressSelect;
                this.mapper64_chrAddressSelect = map64.mapper64_chrAddressSelect;
            }
            //MAPPER 65
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 65)
            {
                Mapper65 map65 = (Mapper65)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper65_timer_irq_counter_65 = map65.timer_irq_counter_65;
                this.mapper65_timer_irq_Latch_65 = map65.timer_irq_Latch_65;
                this.mapper65_timer_irq_enabled = map65.timer_irq_enabled;
            }
            //MAPPER 69
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 69)
            {
                Mapper69 map69 = (Mapper69)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper69_reg = map69.reg;
                this.mapper69_timer_irq_counter_69 = map69.timer_irq_counter_69;
                this.mapper69_timer_irq_enabled = map69.timer_irq_enabled;
            }
            //MAPPER 91
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 91)
            {
                Mapper91 map91 = (Mapper91)_Nes.MEMORY.MAP.CurrentMapper;
                this.mapper91_IRQEnabled = map91.IRQEnabled;
                this.mapper91_IRQCount = map91.IRQCount;
            }
            #endregion
        }
        public void ApplyDataToNes(NesEngine _Nes)
        {
            #region CPU
            _Nes.CPU.REG_A = this.REG_A;
            _Nes.CPU.REG_X = this.REG_X;
            _Nes.CPU.REG_Y = this.REG_Y;
            _Nes.CPU.REG_S = this.REG_S;
            _Nes.CPU.REG_PC = this.REG_PC;
            _Nes.CPU.Flag_N = this.Flag_N;
            _Nes.CPU.Flag_V = this.Flag_V;
            _Nes.CPU.Flag_B = this.Flag_B;
            _Nes.CPU.Flag_D = this.Flag_D;
            _Nes.CPU.Flag_I = this.Flag_I;
            _Nes.CPU.Flag_Z = this.Flag_Z;
            _Nes.CPU.Flag_C = this.Flag_C;
            _Nes.CPU.CycleCounter = this.CycleCounter;
            _Nes.CPU.CyclesPerScanline = this.CyclesPerScanline;
            _Nes.CPU.OpCode = this.OpCode;
            _Nes.CPU.PrevPC = this.PrevPC;
            #endregion
            #region MEMORY
            _Nes.MEMORY.RAM = this._RAM;
            _Nes.MEMORY.SRAM = this._SRAM;
            _Nes.MEMORY.JoyData1 = this.JoyData1;
            _Nes.MEMORY.JoyData2 = this.JoyData2;
            _Nes.MEMORY.JoyStrobe = this.JoyStrobe;
            #endregion
            #region CART
            if (_Nes.MEMORY.MAP.Cartridge.CHR_PAGES == 0)
                _Nes.MEMORY.MAP.Cartridge.CHR = this._CHR;
            _Nes.MEMORY.MAP.Cartridge.Mirroring = this._MIRRORING;
            _Nes.MEMORY.MAP.Cartridge.IsSaveRam = this._save_ram_present;
            _Nes.MEMORY.MAP.Cartridge.IsVRAM = this._is_vram;
            _Nes.MEMORY.MAP.Cartridge.MirroringBase = this._mirroringBase;
            #endregion
            #region MAP
            _Nes.MEMORY.MAP.current_prg_rom_page = this.current_prg_rom_page;
            _Nes.MEMORY.MAP.current_chr_rom_page = this.current_chr_rom_page;
            #endregion
            #region PPU
            _Nes.PPU.SPRRAM = this.SPRRAM;
            _Nes.PPU.VRAM = this.VRAM;
            _Nes.PPU.CurrentScanLine = this.CurrentScanLine;
            _Nes.PPU.VRAMAddress = this.VRAMAddress;
            _Nes.PPU.Sprite0Hit = this.Sprite0Hit;
            _Nes.PPU.SpriteCrossed = this.SpriteCrossed;
            _Nes.PPU.ScanlinesPerFrame = this.ScanlinesPerFrame;
            _Nes.PPU.ScanlineOfVBLANK = this.ScanlineOfVBLANK;
            _Nes.PPU._FPS = this._FPS;
            _Nes.PPU.VBLANK = this.VBLANK;
            _Nes.PPU.VRAMReadBuffer = this.VRAMReadBuffer;
            _Nes.PPU._NoLimiter = this._NoLimiter;
            /*2000*/
            _Nes.PPU.ExecuteNMIonVBlank = this.ExecuteNMIonVBlank;
            _Nes.PPU.SpriteSize = this.SpriteSize;
            _Nes.PPU.PatternTableAddressBackground = this.PatternTableAddressBackground;
            _Nes.PPU.PatternTableAddress8x8Sprites = this.PatternTableAddress8x8Sprites;
            _Nes.PPU.VRAMAddressIncrement = this.VRAMAddressIncrement;
            _Nes.PPU.ReloadBits2000 = this.ReloadBits2000;
            /*2001*/
            _Nes.PPU.ColorEmphasis = this.ColorEmphasis;
            _Nes.PPU.SpriteVisibility = this.SpriteVisibility;
            _Nes.PPU.BackgroundVisibility = this.BackgroundVisibility;
            _Nes.PPU.SpriteClipping = this.SpriteClipping;
            _Nes.PPU.BackgroundClipping = this.BackgroundClipping;
            _Nes.PPU.MonochromeMode = this.MonochromeMode;
            /*2003*/
            _Nes.PPU.SpriteRamAddress = this.SpriteRamAddress;
            /*2005,2006*/
            _Nes.PPU.PPUToggle = this.PPUToggle;
            _Nes.PPU.VRAMTemp = this.VRAMTemp;
            /*Draw stuff*/
            _Nes.PPU.HScroll = this.HScroll;
            _Nes.PPU.VScroll = this.VScroll;
            _Nes.PPU.VBits = this.VBits;
            _Nes.PPU.TileY = this.TileY;
            #endregion
            #region APU
            _Nes.APU._FrameCounter = this._FrameCounter;
            _Nes.APU._PAL = this._PAL;
            _Nes.APU.DMCIRQPending = this.DMCIRQPending;
            _Nes.APU.FrameIRQEnabled = this.FrameIRQEnabled;
            _Nes.APU.FrameIRQPending = this.FrameIRQPending;
            _Nes.APU.DMC.LoadState(this);
            _Nes.APU.NOIZE.LoadState(this);
            _Nes.APU.RECT1.LoadState(this);
            _Nes.APU.RECT2.LoadState(this);
            _Nes.APU.TRIANGLE.LoadState(this);
            _Nes.APU.VRC6PULSE1.LoadState(this);
            _Nes.APU.VRC6PULSE2.LoadState(this);
            _Nes.APU.VRC6SAWTOOTH.LoadState(this);
            #endregion
            #region MAPPERS
            //MAPPER 1
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 1)
            {
                Mapper01 map1 = (Mapper01)_Nes.MEMORY.MAP.CurrentMapper;
                map1.mapper1_register8000BitPosition = this.mapper1_register8000BitPosition;
                map1.mapper1_registerA000BitPosition = this.mapper1_registerA000BitPosition;
                map1.mapper1_registerC000BitPosition = this.mapper1_registerC000BitPosition;
                map1.mapper1_registerE000BitPosition = this.mapper1_registerE000BitPosition;
                map1.mapper1_register8000Value = this.mapper1_register8000Value;
                map1.mapper1_registerA000Value = this.mapper1_registerA000Value;
                map1.mapper1_registerC000Value = this.mapper1_registerC000Value;
                map1.mapper1_registerE000Value = this.mapper1_registerE000Value;
                map1.mapper1_mirroringFlag = this.mapper1_mirroringFlag;
                map1.mapper1_onePageMirroring = this.mapper1_onePageMirroring;
                map1.mapper1_prgSwitchingArea = this.mapper1_prgSwitchingArea;
                map1.mapper1_prgSwitchingSize = this.mapper1_prgSwitchingSize;
                map1.mapper1_vromSwitchingSize = this.mapper1_vromSwitchingSize;
            }
            //MAPPER 4
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 4)
            {
                Mapper04 map4 = (Mapper04)_Nes.MEMORY.MAP.CurrentMapper;
                map4.mapper4_commandNumber = this.mapper4_commandNumber;
                map4.mapper4_prgAddressSelect = this.mapper4_prgAddressSelect;
                map4.mapper4_chrAddressSelect = this.mapper4_chrAddressSelect;
                map4.timer_irq_enabled = this.mapper4_timer_irq_enabled;
                map4.timer_irq_count = this.mapper4_timer_irq_count;
                map4.timer_irq_reload = this.mapper4_timer_irq_reload;
            }
            //MAPPER 5
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 5)
            {
                Mapper05 map5 = (Mapper05)_Nes.MEMORY.MAP.CurrentMapper;
                map5.mapper5_prgBankSize = this.mapper5_prgBankSize;
                map5.mapper5_chrBankSize = this.mapper5_chrBankSize;
                map5.mapper5_scanlineSplit = this.mapper5_scanlineSplit;
                map5.mapper5_splitIrqEnabled = this.mapper5_splitIrqEnabled;
            }
            //MAPPER 6
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 6)
            {
                Mapper06 map6 = (Mapper06)_Nes.MEMORY.MAP.CurrentMapper;
                map6.IRQEnabled = this.mapper6_IRQEnabled;
                map6.irq_counter = this.mapper6_irq_counter;
            }
            //MAPPER 8
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 8)
            {
                Mapper08 map8 = (Mapper08)_Nes.MEMORY.MAP.CurrentMapper;
                map8.IRQEnabled = this.mapper8_IRQEnabled;
                map8.irq_counter = this.mapper8_irq_counter;
            }
            //MAPPER 9
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 9)
            {
                Mapper09 map9 = (Mapper09)_Nes.MEMORY.MAP.CurrentMapper;
                map9.latch1 = this.mapper9_latch1;
                map9.latch2 = this.mapper9_latch2;
                map9.latch1data1 = this.mapper9_latch1data1;
                map9.latch1data2 = this.mapper9_latch1data2;
                map9.latch2data1 = this.mapper9_latch2data1;
                map9.latch2data2 = this.mapper9_latch2data2;
            }
            //MAPPER 10
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 10)
            {
                Mapper10 map10 = (Mapper10)_Nes.MEMORY.MAP.CurrentMapper;
                map10.latch1 = this.mapper10_latch1;
                map10.latch2 = this.mapper10_latch2;
                map10.latch1data1 = this.mapper10_latch1data1;
                map10.latch1data2 = this.mapper10_latch1data2;
                map10.latch2data1 = this.mapper10_latch2data1;
                map10.latch2data2 = this.mapper10_latch2data2;
            }
            //MAPPER 16
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 16)
            {
                Mapper16 map16 = (Mapper16)_Nes.MEMORY.MAP.CurrentMapper;
                map16.timer_irq_counter_16 = this.timer_irq_counter_16;
                map16.timer_irq_Latch_16 = this.timer_irq_Latch_16;
                map16.timer_irq_enabled = this.timer_irq_enabled;
            }
            //MAPPER 17
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 17)
            {
                Mapper17 map17 = (Mapper17)_Nes.MEMORY.MAP.CurrentMapper;
                map17.IRQEnabled = this.mapper17_IRQEnabled;
                map17.irq_counter = this.mapper17_irq_counter;
            }
            //MAPPER 18
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 18)
            {
                Mapper18 map18 = (Mapper18)_Nes.MEMORY.MAP.CurrentMapper;
                map18.Mapper18_Timer = this.Mapper18_Timer;
                map18.Mapper18_latch = this.Mapper18_latch;
                map18.mapper18_control = this.mapper18_control;
                map18.Mapper18_IRQWidth = this.Mapper18_IRQWidth;
                map18.timer_irq_enabled = this.Mapper18_timer_irq_enabled;
                map18.x = this.Mapper18_x;
            }
            //MAPPER 19
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 19)
            {
                Mapper19 map19 = (Mapper19)_Nes.MEMORY.MAP.CurrentMapper;
                map19.VROMRAMfor0000 = this.Mapper19_VROMRAMfor0000;
                map19.VROMRAMfor1000 = this.Mapper19_VROMRAMfor1000;
                map19.irq_counter = this.Mapper19_irq_counter;
                map19.IRQEnabled = this.Mapper19_IRQEnabled;
            }
            //MAPPER 21
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 21)
            {
                Mapper21 map21 = (Mapper21)_Nes.MEMORY.MAP.CurrentMapper;
                map21.PRGMode = this.Mapper21_PRGMode;
                map21.REG = this.Mapper21_REG;
                map21.irq_latch = this.Mapper21_irq_latch;
                map21.irq_enable = this.Mapper21_irq_enable;
                map21.irq_counter = this.Mapper21_irq_counter;
                map21.irq_clock = this.Mapper21_irq_clock;
            }
            //MAPPER 23
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 23)
            {
                Mapper23 map23 = (Mapper23)_Nes.MEMORY.MAP.CurrentMapper;
                map23.PRGMode = this.Mapper23_PRGMode;
                map23.REG = this.Mapper23_REG;
                map23.irq_latch = this.Mapper23_irq_latch;
                map23.irq_enable = this.Mapper23_irq_enable;
                map23.irq_counter = this.Mapper23_irq_counter;
                map23.irq_clock = this.Mapper23_irq_clock;
            }
            //MAPPER 24
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 24)
            {
                Mapper24 map24 = (Mapper24)_Nes.MEMORY.MAP.CurrentMapper;
                map24.irq_latch = this.Mapper24_irq_latch;
                map24.irq_enable = this.Mapper24_irq_enable;
                map24.irq_counter = this.Mapper24_irq_counter;
                map24.irq_clock = this.Mapper24_irq_clock;
            }
            //MAPPER 225
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 225)
            {
                Mapper225_255 map225 = (Mapper225_255)_Nes.MEMORY.MAP.CurrentMapper;
                map225.Mapper225_reg0 = this.Mapper225_reg0;
                map225.Mapper225_reg1 = this.Mapper225_reg1;
                map225.Mapper225_reg2 = this.Mapper225_reg2;
                map225.Mapper225_reg3 = this.Mapper225_reg3;
            }
            //MAPPER 32
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 32)
            {
                Mapper32 map32 = (Mapper32)_Nes.MEMORY.MAP.CurrentMapper;
                map32.mapper32SwitchingMode = this.mapper32SwitchingMode;
            }
            //MAPPER 33
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 33)
            {
                Mapper33 map33 = (Mapper33)_Nes.MEMORY.MAP.CurrentMapper;
                map33.type1 = this.mapper33_type1;
                map33.IRQCounter = this.mapper33_IRQCounter;
                map33.IRQEabled = this.mapper33_IRQEabled;
            }
            //MAPPER 41
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 41)
            {
                Mapper41 map41 = (Mapper41)_Nes.MEMORY.MAP.CurrentMapper;
                map41.Mapper41_CHR_Low = this.Mapper41_CHR_Low;
                map41.Mapper41_CHR_High = this.Mapper41_CHR_High;
            }
            //MAPPER 64
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 64)
            {
                Mapper64 map64 = (Mapper64)_Nes.MEMORY.MAP.CurrentMapper;
                map64.mapper64_commandNumber = this.mapper64_commandNumber;
                map64.mapper64_prgAddressSelect = this.mapper64_prgAddressSelect;
                map64.mapper64_chrAddressSelect = this.mapper64_chrAddressSelect;
            }
            //MAPPER 65
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 65)
            {
                Mapper65 map65 = (Mapper65)_Nes.MEMORY.MAP.CurrentMapper;
                map65.timer_irq_counter_65 = this.mapper65_timer_irq_counter_65;
                map65.timer_irq_Latch_65 = this.mapper65_timer_irq_Latch_65;
                map65.timer_irq_enabled = this.mapper65_timer_irq_enabled;
            }
            //MAPPER 69
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 69)
            {
                Mapper69 map69 = (Mapper69)_Nes.MEMORY.MAP.CurrentMapper;
                map69.reg = this.mapper69_reg;
                map69.timer_irq_counter_69 = this.mapper69_timer_irq_counter_69;
                map69.timer_irq_enabled = this.mapper69_timer_irq_enabled;
            }
            //MAPPER 91
            if (_Nes.MEMORY.MAP.Cartridge.MapperNo == 91)
            {
                Mapper91 map91 = (Mapper91)_Nes.MEMORY.MAP.CurrentMapper;
                map91.IRQEnabled = this.mapper91_IRQEnabled;
                map91.IRQCount = this.mapper91_IRQCount;
            }
            #endregion
        }
    }
}