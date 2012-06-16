using System;
using NesHd.Core.Memory;
using NesHd.Core.Memory.Mappers;

namespace NesHd.Core.Misc
{
    [Serializable]
    public class StateHolder
    {
        #region CPU

        public int CycleCounter;
        private int _cyclesPerScanline;
        private bool _flagB;
        private bool _flagC;
        private bool _flagD;
        private bool _flagI = true;
        private bool _flagN;
        private bool _flagV;
        private bool _flagZ;
        private byte _opCode;
        private ushort _prevPc;
        private byte _regA;
        private ushort _regPc;
        private byte _regS;
        private byte _regX;
        private byte _regY;

        #endregion

        #region MEMORY

        private int _joyData1;
        private int _joyData2;
        private byte _joyStrobe;
        private byte[] _ram;
        private byte[] _sram;

        #endregion

        #region CART

        private byte[][] _chr;
        private Mirroring _mirroring;
        private bool _isVram;
        private uint _mirroringBase;
        private bool _saveRAMPresent;

        #endregion

        #region _map

        public uint[] CurrentChrRomPage;
        public uint[] CurrentPRGRomPage;

        #endregion

        #region PPU

        private bool _backgroundClipping;
        private bool _backgroundVisibility;
        private ushort _colorEmphasis;
        private int _currentScanLine;
        /*2000*/
        private bool _executeNMIonVBlank;
        private byte _hScroll;
        private bool _monochromeMode;
        private bool _ppuToggle;
        private int _patternTableAddress8X8Sprites;
        private int _patternTableAddressBackground;
        private byte _reloadBits2000;
        private byte[] _sprram;
        private int _scanlineOfVblank;
        private int _scanlinesPerFrame;
        private bool _sprite0Hit;
        /*2001*/
        private bool _spriteClipping;
        private int _spriteCrossed;
        /*2003*/
        private byte _spriteRamAddress;
        private bool _spriteSize; //true=8x16, false=8x8
        private bool _spriteVisibility;
        private int _tileY;
        private bool _vblank;
        private int _vBits;
        private byte[] _vram;
        private ushort _vramAddress;
        private int _vramAddressIncrement = 1;
        private byte _vramReadBuffer;
        /*2005,2006*/
        private ushort _vramTemp;
        /*Draw stuff*/
        private int _vScroll;
        private int _fps;
        private bool _noLimiter;

        #endregion

        #region APU

        public byte DMCDAC;
        public ushort DMCDMAAddress;
        public ushort DMCDMALength;
        public ushort DMCDMALengthCounter;
        public ushort DMCDMAStartAddress;
        public byte DMCDMCBIT;
        public byte DMCDMCBYTE;
        public bool DMCDMCIRQEnabled;
        private bool DMCIRQPending;
        public bool DMC_Enabled;
        public double DMC_FreqTimer;
        public double DMC_Frequency;
        public bool DMC_Loop;
        public double DMC_RenderedLength;
        public double DMC_SampleCount;
        private bool FrameIRQEnabled;
        private bool FrameIRQPending;
        public short NOIZEOUT;
        public byte NOIZE_DecayCount;
        public bool NOIZE_DecayDiable;
        public bool NOIZE_DecayLoopEnable;
        public bool NOIZE_DecayReset;
        public byte NOIZE_DecayTimer;
        //NOIZE
        public bool NOIZE_Enabled;
        public byte NOIZE_Envelope;
        public double NOIZE_FreqTimer;
        public double NOIZE_Frequency;
        public byte NOIZE_LengthCount;
        public int NOIZE_NoiseMode;
        public double NOIZE_RenderedLength;
        public double NOIZE_SampleCount;
        public ushort NOIZE_ShiftReg = 1;
        public byte NOIZE_Volume;
        public double Rectangle1DutyPercentage;
        public bool Rectangle1WaveStatus;
        public byte Rectangle1_DecayCount;
        public bool Rectangle1_DecayDiable;
        public bool Rectangle1_DecayLoopEnable;
        public bool Rectangle1_DecayReset;
        public byte Rectangle1_DecayTimer;
        public int Rectangle1_DutyCycle;
        public bool Rectangle1_Enabled;
        public byte Rectangle1_Envelope;
        public int Rectangle1_FreqTimer;
        public double Rectangle1_Frequency;
        public byte Rectangle1_LengthCount;
        public double Rectangle1_RenderedLength;
        public double Rectangle1_SampleCount;
        public byte Rectangle1_SweepCount;
        public bool Rectangle1_SweepDirection;
        public bool Rectangle1_SweepEnable;
        public bool Rectangle1_SweepForceSilence;
        public byte Rectangle1_SweepRate;
        public bool Rectangle1_SweepReset;
        public byte Rectangle1_SweepShift;
        public byte Rectangle1_Volume;
        public double Rectangle2DutyPercentage;
        public bool Rectangle2WaveStatus;
        public byte Rectangle2_DecayCount;
        public bool Rectangle2_DecayDiable;
        public bool Rectangle2_DecayLoopEnable;
        public bool Rectangle2_DecayReset;
        public byte Rectangle2_DecayTimer;
        public int Rectangle2_DutyCycle;
        public bool Rectangle2_Enabled;
        public byte Rectangle2_Envelope;
        public int Rectangle2_FreqTimer;
        public double Rectangle2_Frequency;
        public byte Rectangle2_LengthCount;
        public double Rectangle2_RenderedLength;
        public double Rectangle2_SampleCount;
        public byte Rectangle2_SweepCount;
        public bool Rectangle2_SweepDirection;
        public bool Rectangle2_SweepEnable;
        public bool Rectangle2_SweepForceSilence;
        public byte Rectangle2_SweepRate;
        public bool Rectangle2_SweepReset;
        public byte Rectangle2_SweepShift;
        public byte Rectangle2_Volume;
        public bool TriangleHALT;
        public short TriangleOUT;
        public bool Triangle_Enabled;
        public int Triangle_FreqTimer;
        public double Triangle_Frequency;
        public byte Triangle_LengthCount;
        public bool Triangle_LengthEnabled;
        public bool Triangle_LinearControl;
        public int Triangle_LinearCounter;
        public int Triangle_LinearCounterLoad;
        public double Triangle_RenderedLength;
        public double Triangle_SampleCount;
        public int Triangle_Sequence;
        public double VRC6Pulse1DutyPercentage;
        public short VRC6Pulse1OUT;
        public bool VRC6Pulse1WaveStatus;
        public int VRC6Pulse1_DutyCycle;
        public bool VRC6Pulse1_Enabled;
        public int VRC6Pulse1_FreqTimer;
        public double VRC6Pulse1_Frequency;
        public double VRC6Pulse1_RenderedLength;
        public double VRC6Pulse1_SampleCount;
        public byte VRC6Pulse1_Volume;
        public double VRC6Pulse2DutyPercentage;
        public short VRC6Pulse2OUT;
        public bool VRC6Pulse2WaveStatus;
        public int VRC6Pulse2_DutyCycle;
        public bool VRC6Pulse2_Enabled;
        public int VRC6Pulse2_FreqTimer;
        public double VRC6Pulse2_Frequency;
        public double VRC6Pulse2_RenderedLength;
        public double VRC6Pulse2_SampleCount;
        public byte VRC6Pulse2_Volume;
        public byte VRC6SawtoothAccum;
        //VRC6 Sawtooth
        public byte VRC6SawtoothAccumRate;
        public byte VRC6SawtoothAccumStep;
        public short VRC6SawtoothOUT;
        public bool VRC6Sawtooth_Enabled;
        public int VRC6Sawtooth_FreqTimer;
        public double VRC6Sawtooth_Frequency;
        public double VRC6Sawtooth_RenderedLength;
        public double VRC6Sawtooth_SampleCount;
        private int _FrameCounter;
        private bool _PAL;

        #endregion

        #region MAPPERS

        //MAPPER 1
        //MAPPER 18
        private int Mapper18_IRQWidth;
        private short Mapper18_Timer;
        private short Mapper18_latch;
        private bool Mapper18_timer_irq_enabled;
        private byte[] Mapper18_x = new byte[22];
        private bool Mapper19_IRQEnabled;
        //MAPPER 19
        private bool Mapper19_VROMRAMfor0000;
        private bool Mapper19_VROMRAMfor1000;
        private short Mapper19_irq_counter;
        //MAPPER 21
        private bool Mapper21_PRGMode = true;
        private byte[] Mapper21_REG = new byte[8];
        private int Mapper21_irq_clock;
        private int Mapper21_irq_counter;
        private int Mapper21_irq_enable;
        private int Mapper21_irq_latch;
        //MAPPER 23
        //MAPPER 225
        private byte Mapper225_reg0 = 0xF;
        private byte Mapper225_reg1 = 0xF;
        private byte Mapper225_reg2 = 0xF;
        private byte Mapper225_reg3 = 0xF;
        private bool Mapper23_PRGMode = true;
        private byte[] Mapper23_REG = new byte[8];
        private int Mapper23_irq_clock;
        private int Mapper23_irq_counter;
        private int Mapper23_irq_enable;
        private int Mapper23_irq_latch;
        private int Mapper24_irq_clock;
        private int Mapper24_irq_counter;
        private bool Mapper24_irq_enable;
        private int Mapper24_irq_latch;
        private byte Mapper41_CHR_High;
        private byte Mapper41_CHR_Low;
        private byte mapper10_latch1;
        private int mapper10_latch1data1;
        private int mapper10_latch1data2;
        private byte mapper10_latch2;
        private int mapper10_latch2data1;
        private int mapper10_latch2data2;
        private bool mapper17_IRQEnabled;
        private int mapper17_irq_counter;
        private byte mapper18_control;
        private byte mapper1_mirroringFlag;
        private byte mapper1_onePageMirroring;
        private byte mapper1_prgSwitchingArea;
        private byte mapper1_prgSwitchingSize;
        private int mapper1_register8000BitPosition;
        private int mapper1_register8000Value;
        private int mapper1_registerA000BitPosition;
        private int mapper1_registerA000Value;
        private int mapper1_registerC000BitPosition;
        private int mapper1_registerC000Value;
        private int mapper1_registerE000BitPosition;
        private int mapper1_registerE000Value;
        private byte mapper1_vromSwitchingSize;
        //MAPPER 32
        private int mapper32SwitchingMode;
        //MAPPER 33
        private byte mapper33_IRQCounter;
        private bool mapper33_IRQEabled;
        private bool mapper33_type1 = true;
        private int mapper4_chrAddressSelect;
        private int mapper4_commandNumber;
        private int mapper4_prgAddressSelect;
        private uint mapper4_timer_irq_count;
        private bool mapper4_timer_irq_enabled;
        private uint mapper4_timer_irq_reload;
        private byte mapper5_chrBankSize;
        private byte mapper5_prgBankSize;
        private int mapper5_scanlineSplit;
        private bool mapper5_splitIrqEnabled;
        private byte mapper64_chrAddressSelect;
        //MAPPER 41
        //MAPPER 64
        private byte mapper64_commandNumber;
        private byte mapper64_prgAddressSelect;
        private short mapper65_timer_irq_Latch_65;
        private short mapper65_timer_irq_counter_65;
        private bool mapper65_timer_irq_enabled;
        //MAPPER 69
        private ushort mapper69_reg;
        private short mapper69_timer_irq_counter_69;
        private bool mapper69_timer_irq_enabled;
        private bool mapper6_IRQEnabled;
        private int mapper6_irq_counter;
        //MAPPER 8
        private bool mapper8_IRQEnabled;
        private int mapper8_irq_counter;
        //MAPPER 91
        private int mapper91_IRQCount;
        private bool mapper91_IRQEnabled;
        private byte mapper9_latch1;
        private int mapper9_latch1data1;
        private int mapper9_latch1data2;
        private byte mapper9_latch2;
        private int mapper9_latch2data1;
        private int mapper9_latch2data2;
        private short timer_irq_Latch_16;
        private short timer_irq_counter_16;
        private bool timer_irq_enabled;

        #endregion

        public void LoadNesData(NesEngine _engine)
        {
            #region CPU

            _regA = _engine.Cpu.REG_A;
            _regX = _engine.Cpu.REG_X;
            _regY = _engine.Cpu.REG_Y;
            _regS = _engine.Cpu.REG_S;
            _regPc = _engine.Cpu.REG_PC;
            _flagN = _engine.Cpu.Flag_N;
            _flagV = _engine.Cpu.Flag_V;
            _flagB = _engine.Cpu.Flag_B;
            _flagD = _engine.Cpu.Flag_D;
            _flagI = _engine.Cpu.Flag_I;
            _flagZ = _engine.Cpu.Flag_Z;
            _flagC = _engine.Cpu.Flag_C;
            CycleCounter = _engine.Cpu.CycleCounter;
            _cyclesPerScanline = _engine.Cpu.CyclesPerScanline;
            _opCode = _engine.Cpu.OpCode;
            _prevPc = _engine.Cpu.PrevPC;

            #endregion

            #region MEMORY

            _ram = _engine.Memory.Ram;
            _sram = _engine.Memory.SRam;
            _joyData1 = _engine.Memory.JoyData1;
            _joyData2 = _engine.Memory.JoyData2;
            _joyStrobe = _engine.Memory.JoyStrobe;

            #endregion

            #region CART

            if (_engine.Memory.Map.Cartridge.ChrPages == 0)
                _chr = _engine.Memory.Map.Cartridge.Chr;
            _mirroring = _engine.Memory.Map.Cartridge.Mirroring;
            _saveRAMPresent = _engine.Memory.Map.Cartridge.IsSaveRam;
            _isVram = _engine.Memory.Map.Cartridge.IsVram;
            _mirroringBase = _engine.Memory.Map.Cartridge.MirroringBase;

            #endregion

            #region _map

            CurrentPRGRomPage = _engine.Memory.Map.CurrentPrgRomPage;
            CurrentChrRomPage = _engine.Memory.Map.CurrentChrRomPage;

            #endregion

            #region PPU

            _sprram = _engine.Ppu.SprRam;
            _vram = _engine.Ppu.VRam;
            _currentScanLine = _engine.Ppu.CurrentScanLine;
            _vramAddress = _engine.Ppu.VRamAddress;
            _sprite0Hit = _engine.Ppu.Sprite0Hit;
            _spriteCrossed = _engine.Ppu.SpriteCrossed;
            _scanlinesPerFrame = _engine.Ppu.ScanlinesPerFrame;
            _scanlineOfVblank = _engine.Ppu.ScanlineOfVblank;
            _fps = _engine.Ppu.Fps;
            _vblank = _engine.Ppu.VBlank;
            _vramReadBuffer = _engine.Ppu.VRamReadBuffer;
            _noLimiter = _engine.Ppu.NoLimiter;
            /*2000*/
            _executeNMIonVBlank = _engine.Ppu.ExecuteNMIonVBlank;
            _spriteSize = _engine.Ppu.SpriteSize;
            _patternTableAddressBackground = _engine.Ppu.PatternTableAddressBackground;
            _patternTableAddress8X8Sprites = _engine.Ppu.PatternTableAddress8X8Sprites;
            _vramAddressIncrement = _engine.Ppu.VRamAddressIncrement;
            _reloadBits2000 = _engine.Ppu.ReloadBits2000;
            /*2001*/
            _colorEmphasis = _engine.Ppu.ColorEmphasis;
            _spriteVisibility = _engine.Ppu.SpriteVisibility;
            _backgroundVisibility = _engine.Ppu.BackgroundVisibility;
            _spriteClipping = _engine.Ppu.SpriteClipping;
            _backgroundClipping = _engine.Ppu.BackgroundClipping;
            _monochromeMode = _engine.Ppu.MonochromeMode;
            /*2003*/
            _spriteRamAddress = _engine.Ppu.SpriteRamAddress;
            /*2005,2006*/
            _ppuToggle = _engine.Ppu.PpuToggle;
            _vramTemp = _engine.Ppu.VRamTemp;
            /*Draw stuff*/
            _hScroll = _engine.Ppu.HScroll;
            _vScroll = _engine.Ppu.VScroll;
            _vBits = _engine.Ppu.VBits;
            _tileY = _engine.Ppu.TileY;

            #endregion

            #region APU

            _FrameCounter = _engine.Apu._FrameCounter;
            _PAL = _engine.Apu._PAL;
            DMCIRQPending = _engine.Apu.DMCIRQPending;
            FrameIRQEnabled = _engine.Apu.FrameIRQEnabled;
            FrameIRQPending = _engine.Apu.FrameIRQPending;
            _engine.Apu.DMC.SaveState(this);
            _engine.Apu.NOIZE.SaveState(this);
            _engine.Apu.RECT1.SaveState(this);
            _engine.Apu.RECT2.SaveState(this);
            _engine.Apu.TRIANGLE.SaveState(this);
            _engine.Apu.VRC6PULSE1.SaveState(this);
            _engine.Apu.VRC6PULSE2.SaveState(this);
            _engine.Apu.VRC6SAWTOOTH.SaveState(this);

            #endregion

            #region Mappers

            //MAPPER 1
            if (_engine.Memory.Map.Cartridge.MapperNo == 1)
            {
                var map1 = (Mapper01) _engine.Memory.Map.CurrentMapper;
                mapper1_register8000BitPosition = map1.Mapper1Register8000BitPosition;
                mapper1_registerA000BitPosition = map1.Mapper1RegisterA000BitPosition;
                mapper1_registerC000BitPosition = map1.Mapper1RegisterC000BitPosition;
                mapper1_registerE000BitPosition = map1.Mapper1RegisterE000BitPosition;
                mapper1_register8000Value = map1.Mapper1Register8000Value;
                mapper1_registerA000Value = map1.Mapper1RegisterA000Value;
                mapper1_registerC000Value = map1.Mapper1RegisterC000Value;
                mapper1_registerE000Value = map1.Mapper1RegisterE000Value;
                mapper1_mirroringFlag = map1.Mapper1MirroringFlag;
                mapper1_onePageMirroring = map1.Mapper1OnePageMirroring;
                mapper1_prgSwitchingArea = map1.Mapper1PRGSwitchingArea;
                mapper1_prgSwitchingSize = map1.Mapper1PRGSwitchingSize;
                mapper1_vromSwitchingSize = map1.Mapper1VromSwitchingSize;
            }
            //MAPPER 4
            if (_engine.Memory.Map.Cartridge.MapperNo == 4)
            {
                var map4 = (Mapper04) _engine.Memory.Map.CurrentMapper;
                mapper4_commandNumber = map4.Mapper4CommandNumber;
                mapper4_prgAddressSelect = map4.Mapper4PRGAddressSelect;
                mapper4_chrAddressSelect = map4.Mapper4ChrAddressSelect;
                mapper4_timer_irq_enabled = map4.TimerIrqEnabled;
                mapper4_timer_irq_count = map4.TimerIrqCount;
                mapper4_timer_irq_reload = map4.TimerIrqReload;
            }
            //MAPPER 5
            if (_engine.Memory.Map.Cartridge.MapperNo == 5)
            {
                var map5 = (Mapper05) _engine.Memory.Map.CurrentMapper;
                mapper5_prgBankSize = map5.Mapper5PRGBankSize;
                mapper5_chrBankSize = map5.Mapper5ChrBankSize;
                mapper5_scanlineSplit = map5.Mapper5ScanlineSplit;
                mapper5_splitIrqEnabled = map5.Mapper5SplitIrqEnabled;
            }
            //MAPPER 6
            if (_engine.Memory.Map.Cartridge.MapperNo == 6)
            {
                var map6 = (Mapper06) _engine.Memory.Map.CurrentMapper;
                mapper6_IRQEnabled = map6.IRQEnabled;
                mapper6_irq_counter = map6.irq_counter;
            }
            //MAPPER 8
            if (_engine.Memory.Map.Cartridge.MapperNo == 8)
            {
                var map8 = (Mapper08) _engine.Memory.Map.CurrentMapper;
                mapper8_IRQEnabled = map8.IRQEnabled;
                mapper8_irq_counter = map8.irq_counter;
            }
            //MAPPER 9
            if (_engine.Memory.Map.Cartridge.MapperNo == 9)
            {
                var map9 = (Mapper09) _engine.Memory.Map.CurrentMapper;
                mapper9_latch1 = map9.latch1;
                mapper9_latch2 = map9.latch2;
                mapper9_latch1data1 = map9.latch1data1;
                mapper9_latch1data2 = map9.latch1data2;
                mapper9_latch2data1 = map9.latch2data1;
                mapper9_latch2data2 = map9.latch2data2;
            }
            //MAPPER 10
            if (_engine.Memory.Map.Cartridge.MapperNo == 10)
            {
                var map10 = (Mapper10) _engine.Memory.Map.CurrentMapper;
                mapper10_latch1 = map10.Latch1;
                mapper10_latch2 = map10.Latch2;
                mapper10_latch1data1 = map10.Latch1Data1;
                mapper10_latch1data2 = map10.Latch1Data2;
                mapper10_latch2data1 = map10.Latch2Data1;
                mapper10_latch2data2 = map10.Latch2Data2;
            }
            //MAPPER 16
            if (_engine.Memory.Map.Cartridge.MapperNo == 16)
            {
                var map16 = (Mapper16) _engine.Memory.Map.CurrentMapper;
                timer_irq_counter_16 = map16.timer_irq_counter_16;
                timer_irq_Latch_16 = map16.timer_irq_Latch_16;
                timer_irq_enabled = map16.timer_irq_enabled;
            }
            //MAPPER 17
            if (_engine.Memory.Map.Cartridge.MapperNo == 17)
            {
                var map17 = (Mapper17) _engine.Memory.Map.CurrentMapper;
                mapper17_IRQEnabled = map17.IRQEnabled;
                mapper17_irq_counter = map17.irq_counter;
            }
            //MAPPER 18
            if (_engine.Memory.Map.Cartridge.MapperNo == 18)
            {
                var map18 = (Mapper18) _engine.Memory.Map.CurrentMapper;
                Mapper18_Timer = map18.Mapper18_Timer;
                Mapper18_latch = map18.Mapper18_latch;
                mapper18_control = map18.mapper18_control;
                Mapper18_IRQWidth = map18.Mapper18_IRQWidth;
                Mapper18_timer_irq_enabled = map18.timer_irq_enabled;
                Mapper18_x = map18.x;
            }
            //MAPPER 19
            if (_engine.Memory.Map.Cartridge.MapperNo == 19)
            {
                var map19 = (Mapper19) _engine.Memory.Map.CurrentMapper;
                Mapper19_VROMRAMfor0000 = map19.VROMRAMfor0000;
                Mapper19_VROMRAMfor1000 = map19.VROMRAMfor1000;
                Mapper19_irq_counter = map19.irq_counter;
                Mapper19_IRQEnabled = map19.IRQEnabled;
            }
            //MAPPER 21
            if (_engine.Memory.Map.Cartridge.MapperNo == 21)
            {
                var map21 = (Mapper21) _engine.Memory.Map.CurrentMapper;
                Mapper21_PRGMode = map21.PRGMode;
                Mapper21_REG = map21.REG;
                Mapper21_irq_latch = map21.IrqLatch;
                Mapper21_irq_enable = map21.IrqEnable;
                Mapper21_irq_counter = map21.IrqCounter;
                Mapper21_irq_clock = map21.IrqClock;
            }
            //MAPPER 23
            if (_engine.Memory.Map.Cartridge.MapperNo == 23)
            {
                var map23 = (Mapper23) _engine.Memory.Map.CurrentMapper;
                Mapper23_PRGMode = map23.PRGMode;
                Mapper23_REG = map23.REG;
                Mapper23_irq_latch = map23.IrqLatch;
                Mapper23_irq_enable = map23.IrqEnable;
                Mapper23_irq_counter = map23.IrqCounter;
                Mapper23_irq_clock = map23.IrqClock;
            }
            //MAPPER 24
            if (_engine.Memory.Map.Cartridge.MapperNo == 24)
            {
                var map24 = (Mapper24) _engine.Memory.Map.CurrentMapper;
                Mapper24_irq_latch = map24.irq_latch;
                Mapper24_irq_enable = map24.irq_enable;
                Mapper24_irq_counter = map24.irq_counter;
                Mapper24_irq_clock = map24.irq_clock;
            }
            //MAPPER 225
            if (_engine.Memory.Map.Cartridge.MapperNo == 225)
            {
                var map225 = (Mapper225_255) _engine.Memory.Map.CurrentMapper;
                Mapper225_reg0 = map225.Mapper225_reg0;
                Mapper225_reg1 = map225.Mapper225_reg1;
                Mapper225_reg2 = map225.Mapper225_reg2;
                Mapper225_reg3 = map225.Mapper225_reg3;
            }
            //MAPPER 32
            if (_engine.Memory.Map.Cartridge.MapperNo == 32)
            {
                var map32 = (Mapper32) _engine.Memory.Map.CurrentMapper;
                mapper32SwitchingMode = map32.mapper32SwitchingMode;
            }
            //MAPPER 33
            if (_engine.Memory.Map.Cartridge.MapperNo == 33)
            {
                var map33 = (Mapper33) _engine.Memory.Map.CurrentMapper;
                mapper33_type1 = map33.type1;
                mapper33_IRQCounter = map33.IRQCounter;
                mapper33_IRQEabled = map33.IRQEabled;
            }
            //MAPPER 41
            if (_engine.Memory.Map.Cartridge.MapperNo == 41)
            {
                var map41 = (Mapper41) _engine.Memory.Map.CurrentMapper;
                Mapper41_CHR_Low = map41.Mapper41_CHR_Low;
                Mapper41_CHR_High = map41.Mapper41_CHR_High;
            }
            //MAPPER 64
            if (_engine.Memory.Map.Cartridge.MapperNo == 64)
            {
                var map64 = (Mapper64) _engine.Memory.Map.CurrentMapper;
                mapper64_commandNumber = map64.Mapper64CommandNumber;
                mapper64_prgAddressSelect = map64.Mapper64PrgAddressSelect;
                mapper64_chrAddressSelect = map64.Mapper64ChrAddressSelect;
            }
            //MAPPER 65
            if (_engine.Memory.Map.Cartridge.MapperNo == 65)
            {
                var map65 = (Mapper65) _engine.Memory.Map.CurrentMapper;
                mapper65_timer_irq_counter_65 = map65.timer_irq_counter_65;
                mapper65_timer_irq_Latch_65 = map65.timer_irq_Latch_65;
                mapper65_timer_irq_enabled = map65.timer_irq_enabled;
            }
            //MAPPER 69
            if (_engine.Memory.Map.Cartridge.MapperNo == 69)
            {
                var map69 = (Mapper69) _engine.Memory.Map.CurrentMapper;
                mapper69_reg = map69.reg;
                mapper69_timer_irq_counter_69 = map69.timer_irq_counter_69;
                mapper69_timer_irq_enabled = map69.timer_irq_enabled;
            }
            //MAPPER 91
            if (_engine.Memory.Map.Cartridge.MapperNo == 91)
            {
                var map91 = (Mapper91) _engine.Memory.Map.CurrentMapper;
                mapper91_IRQEnabled = map91.IRQEnabled;
                mapper91_IRQCount = map91.IRQCount;
            }

            #endregion
        }

        public void ApplyDataToNes(NesEngine _engine)
        {
            #region CPU

            _engine.Cpu.REG_A = _regA;
            _engine.Cpu.REG_X = _regX;
            _engine.Cpu.REG_Y = _regY;
            _engine.Cpu.REG_S = _regS;
            _engine.Cpu.REG_PC = _regPc;
            _engine.Cpu.Flag_N = _flagN;
            _engine.Cpu.Flag_V = _flagV;
            _engine.Cpu.Flag_B = _flagB;
            _engine.Cpu.Flag_D = _flagD;
            _engine.Cpu.Flag_I = _flagI;
            _engine.Cpu.Flag_Z = _flagZ;
            _engine.Cpu.Flag_C = _flagC;
            _engine.Cpu.CycleCounter = CycleCounter;
            _engine.Cpu.CyclesPerScanline = _cyclesPerScanline;
            _engine.Cpu.OpCode = _opCode;
            _engine.Cpu.PrevPC = _prevPc;

            #endregion

            #region MEMORY

            _engine.Memory.Ram = _ram;
            _engine.Memory.SRam = _sram;
            _engine.Memory.JoyData1 = _joyData1;
            _engine.Memory.JoyData2 = _joyData2;
            _engine.Memory.JoyStrobe = _joyStrobe;

            #endregion

            #region CART

            if (_engine.Memory.Map.Cartridge.ChrPages == 0)
                _engine.Memory.Map.Cartridge.Chr = _chr;
            _engine.Memory.Map.Cartridge.Mirroring = _mirroring;
            _engine.Memory.Map.Cartridge.IsSaveRam = _saveRAMPresent;
            _engine.Memory.Map.Cartridge.IsVram = _isVram;
            _engine.Memory.Map.Cartridge.MirroringBase = _mirroringBase;

            #endregion

            #region _map

            _engine.Memory.Map.CurrentPrgRomPage = CurrentPRGRomPage;
            _engine.Memory.Map.CurrentChrRomPage = CurrentChrRomPage;

            #endregion

            #region PPU

            _engine.Ppu.SprRam = _sprram;
            _engine.Ppu.VRam = _vram;
            _engine.Ppu.CurrentScanLine = _currentScanLine;
            _engine.Ppu.VRamAddress = _vramAddress;
            _engine.Ppu.Sprite0Hit = _sprite0Hit;
            _engine.Ppu.SpriteCrossed = _spriteCrossed;
            _engine.Ppu.ScanlinesPerFrame = _scanlinesPerFrame;
            _engine.Ppu.ScanlineOfVblank = _scanlineOfVblank;
            _engine.Ppu.Fps = _fps;
            _engine.Ppu.VBlank = _vblank;
            _engine.Ppu.VRamReadBuffer = _vramReadBuffer;
            _engine.Ppu.NoLimiter = _noLimiter;
            /*2000*/
            _engine.Ppu.ExecuteNMIonVBlank = _executeNMIonVBlank;
            _engine.Ppu.SpriteSize = _spriteSize;
            _engine.Ppu.PatternTableAddressBackground = _patternTableAddressBackground;
            _engine.Ppu.PatternTableAddress8X8Sprites = _patternTableAddress8X8Sprites;
            _engine.Ppu.VRamAddressIncrement = _vramAddressIncrement;
            _engine.Ppu.ReloadBits2000 = _reloadBits2000;
            /*2001*/
            _engine.Ppu.ColorEmphasis = _colorEmphasis;
            _engine.Ppu.SpriteVisibility = _spriteVisibility;
            _engine.Ppu.BackgroundVisibility = _backgroundVisibility;
            _engine.Ppu.SpriteClipping = _spriteClipping;
            _engine.Ppu.BackgroundClipping = _backgroundClipping;
            _engine.Ppu.MonochromeMode = _monochromeMode;
            /*2003*/
            _engine.Ppu.SpriteRamAddress = _spriteRamAddress;
            /*2005,2006*/
            _engine.Ppu.PpuToggle = _ppuToggle;
            _engine.Ppu.VRamTemp = _vramTemp;
            /*Draw stuff*/
            _engine.Ppu.HScroll = _hScroll;
            _engine.Ppu.VScroll = _vScroll;
            _engine.Ppu.VBits = _vBits;
            _engine.Ppu.TileY = _tileY;

            #endregion

            #region APU

            _engine.Apu._FrameCounter = _FrameCounter;
            _engine.Apu._PAL = _PAL;
            _engine.Apu.DMCIRQPending = DMCIRQPending;
            _engine.Apu.FrameIRQEnabled = FrameIRQEnabled;
            _engine.Apu.FrameIRQPending = FrameIRQPending;
            _engine.Apu.DMC.LoadState(this);
            _engine.Apu.NOIZE.LoadState(this);
            _engine.Apu.RECT1.LoadState(this);
            _engine.Apu.RECT2.LoadState(this);
            _engine.Apu.TRIANGLE.LoadState(this);
            _engine.Apu.VRC6PULSE1.LoadState(this);
            _engine.Apu.VRC6PULSE2.LoadState(this);
            _engine.Apu.VRC6SAWTOOTH.LoadState(this);

            #endregion

            #region MAPPERS

            //MAPPER 1
            if (_engine.Memory.Map.Cartridge.MapperNo == 1)
            {
                var map1 = (Mapper01) _engine.Memory.Map.CurrentMapper;
                map1.Mapper1Register8000BitPosition = mapper1_register8000BitPosition;
                map1.Mapper1RegisterA000BitPosition = mapper1_registerA000BitPosition;
                map1.Mapper1RegisterC000BitPosition = mapper1_registerC000BitPosition;
                map1.Mapper1RegisterE000BitPosition = mapper1_registerE000BitPosition;
                map1.Mapper1Register8000Value = mapper1_register8000Value;
                map1.Mapper1RegisterA000Value = mapper1_registerA000Value;
                map1.Mapper1RegisterC000Value = mapper1_registerC000Value;
                map1.Mapper1RegisterE000Value = mapper1_registerE000Value;
                map1.Mapper1MirroringFlag = mapper1_mirroringFlag;
                map1.Mapper1OnePageMirroring = mapper1_onePageMirroring;
                map1.Mapper1PRGSwitchingArea = mapper1_prgSwitchingArea;
                map1.Mapper1PRGSwitchingSize = mapper1_prgSwitchingSize;
                map1.Mapper1VromSwitchingSize = mapper1_vromSwitchingSize;
            }
            //MAPPER 4
            if (_engine.Memory.Map.Cartridge.MapperNo == 4)
            {
                var map4 = (Mapper04) _engine.Memory.Map.CurrentMapper;
                map4.Mapper4CommandNumber = mapper4_commandNumber;
                map4.Mapper4PRGAddressSelect = mapper4_prgAddressSelect;
                map4.Mapper4ChrAddressSelect = mapper4_chrAddressSelect;
                map4.TimerIrqEnabled = mapper4_timer_irq_enabled;
                map4.TimerIrqCount = mapper4_timer_irq_count;
                map4.TimerIrqReload = mapper4_timer_irq_reload;
            }
            //MAPPER 5
            if (_engine.Memory.Map.Cartridge.MapperNo == 5)
            {
                var map5 = (Mapper05) _engine.Memory.Map.CurrentMapper;
                map5.Mapper5PRGBankSize = mapper5_prgBankSize;
                map5.Mapper5ChrBankSize = mapper5_chrBankSize;
                map5.Mapper5ScanlineSplit = mapper5_scanlineSplit;
                map5.Mapper5SplitIrqEnabled = mapper5_splitIrqEnabled;
            }
            //MAPPER 6
            if (_engine.Memory.Map.Cartridge.MapperNo == 6)
            {
                var map6 = (Mapper06) _engine.Memory.Map.CurrentMapper;
                map6.IRQEnabled = mapper6_IRQEnabled;
                map6.irq_counter = mapper6_irq_counter;
            }
            //MAPPER 8
            if (_engine.Memory.Map.Cartridge.MapperNo == 8)
            {
                var map8 = (Mapper08) _engine.Memory.Map.CurrentMapper;
                map8.IRQEnabled = mapper8_IRQEnabled;
                map8.irq_counter = mapper8_irq_counter;
            }
            //MAPPER 9
            if (_engine.Memory.Map.Cartridge.MapperNo == 9)
            {
                var map9 = (Mapper09) _engine.Memory.Map.CurrentMapper;
                map9.latch1 = mapper9_latch1;
                map9.latch2 = mapper9_latch2;
                map9.latch1data1 = mapper9_latch1data1;
                map9.latch1data2 = mapper9_latch1data2;
                map9.latch2data1 = mapper9_latch2data1;
                map9.latch2data2 = mapper9_latch2data2;
            }
            //MAPPER 10
            if (_engine.Memory.Map.Cartridge.MapperNo == 10)
            {
                var map10 = (Mapper10) _engine.Memory.Map.CurrentMapper;
                map10.Latch1 = mapper10_latch1;
                map10.Latch2 = mapper10_latch2;
                map10.Latch1Data1 = mapper10_latch1data1;
                map10.Latch1Data2 = mapper10_latch1data2;
                map10.Latch2Data1 = mapper10_latch2data1;
                map10.Latch2Data2 = mapper10_latch2data2;
            }
            //MAPPER 16
            if (_engine.Memory.Map.Cartridge.MapperNo == 16)
            {
                var map16 = (Mapper16) _engine.Memory.Map.CurrentMapper;
                map16.timer_irq_counter_16 = timer_irq_counter_16;
                map16.timer_irq_Latch_16 = timer_irq_Latch_16;
                map16.timer_irq_enabled = timer_irq_enabled;
            }
            //MAPPER 17
            if (_engine.Memory.Map.Cartridge.MapperNo == 17)
            {
                var map17 = (Mapper17) _engine.Memory.Map.CurrentMapper;
                map17.IRQEnabled = mapper17_IRQEnabled;
                map17.irq_counter = mapper17_irq_counter;
            }
            //MAPPER 18
            if (_engine.Memory.Map.Cartridge.MapperNo == 18)
            {
                var map18 = (Mapper18) _engine.Memory.Map.CurrentMapper;
                map18.Mapper18_Timer = Mapper18_Timer;
                map18.Mapper18_latch = Mapper18_latch;
                map18.mapper18_control = mapper18_control;
                map18.Mapper18_IRQWidth = Mapper18_IRQWidth;
                map18.timer_irq_enabled = Mapper18_timer_irq_enabled;
                map18.x = Mapper18_x;
            }
            //MAPPER 19
            if (_engine.Memory.Map.Cartridge.MapperNo == 19)
            {
                var map19 = (Mapper19) _engine.Memory.Map.CurrentMapper;
                map19.VROMRAMfor0000 = Mapper19_VROMRAMfor0000;
                map19.VROMRAMfor1000 = Mapper19_VROMRAMfor1000;
                map19.irq_counter = Mapper19_irq_counter;
                map19.IRQEnabled = Mapper19_IRQEnabled;
            }
            //MAPPER 21
            if (_engine.Memory.Map.Cartridge.MapperNo == 21)
            {
                var map21 = (Mapper21) _engine.Memory.Map.CurrentMapper;
                map21.PRGMode = Mapper21_PRGMode;
                map21.REG = Mapper21_REG;
                map21.IrqLatch = Mapper21_irq_latch;
                map21.IrqEnable = Mapper21_irq_enable;
                map21.IrqCounter = Mapper21_irq_counter;
                map21.IrqClock = Mapper21_irq_clock;
            }
            //MAPPER 23
            if (_engine.Memory.Map.Cartridge.MapperNo == 23)
            {
                var map23 = (Mapper23) _engine.Memory.Map.CurrentMapper;
                map23.PRGMode = Mapper23_PRGMode;
                map23.REG = Mapper23_REG;
                map23.IrqLatch = Mapper23_irq_latch;
                map23.IrqEnable = Mapper23_irq_enable;
                map23.IrqCounter = Mapper23_irq_counter;
                map23.IrqClock = Mapper23_irq_clock;
            }
            //MAPPER 24
            if (_engine.Memory.Map.Cartridge.MapperNo == 24)
            {
                var map24 = (Mapper24) _engine.Memory.Map.CurrentMapper;
                map24.irq_latch = Mapper24_irq_latch;
                map24.irq_enable = Mapper24_irq_enable;
                map24.irq_counter = Mapper24_irq_counter;
                map24.irq_clock = Mapper24_irq_clock;
            }
            //MAPPER 225
            if (_engine.Memory.Map.Cartridge.MapperNo == 225)
            {
                var map225 = (Mapper225_255) _engine.Memory.Map.CurrentMapper;
                map225.Mapper225_reg0 = Mapper225_reg0;
                map225.Mapper225_reg1 = Mapper225_reg1;
                map225.Mapper225_reg2 = Mapper225_reg2;
                map225.Mapper225_reg3 = Mapper225_reg3;
            }
            //MAPPER 32
            if (_engine.Memory.Map.Cartridge.MapperNo == 32)
            {
                var map32 = (Mapper32) _engine.Memory.Map.CurrentMapper;
                map32.mapper32SwitchingMode = mapper32SwitchingMode;
            }
            //MAPPER 33
            if (_engine.Memory.Map.Cartridge.MapperNo == 33)
            {
                var map33 = (Mapper33) _engine.Memory.Map.CurrentMapper;
                map33.type1 = mapper33_type1;
                map33.IRQCounter = mapper33_IRQCounter;
                map33.IRQEabled = mapper33_IRQEabled;
            }
            //MAPPER 41
            if (_engine.Memory.Map.Cartridge.MapperNo == 41)
            {
                var map41 = (Mapper41) _engine.Memory.Map.CurrentMapper;
                map41.Mapper41_CHR_Low = Mapper41_CHR_Low;
                map41.Mapper41_CHR_High = Mapper41_CHR_High;
            }
            //MAPPER 64
            if (_engine.Memory.Map.Cartridge.MapperNo == 64)
            {
                var map64 = (Mapper64) _engine.Memory.Map.CurrentMapper;
                map64.Mapper64CommandNumber = mapper64_commandNumber;
                map64.Mapper64PrgAddressSelect = mapper64_prgAddressSelect;
                map64.Mapper64ChrAddressSelect = mapper64_chrAddressSelect;
            }
            //MAPPER 65
            if (_engine.Memory.Map.Cartridge.MapperNo == 65)
            {
                var map65 = (Mapper65) _engine.Memory.Map.CurrentMapper;
                map65.timer_irq_counter_65 = mapper65_timer_irq_counter_65;
                map65.timer_irq_Latch_65 = mapper65_timer_irq_Latch_65;
                map65.timer_irq_enabled = mapper65_timer_irq_enabled;
            }
            //MAPPER 69
            if (_engine.Memory.Map.Cartridge.MapperNo == 69)
            {
                var map69 = (Mapper69) _engine.Memory.Map.CurrentMapper;
                map69.reg = mapper69_reg;
                map69.timer_irq_counter_69 = mapper69_timer_irq_counter_69;
                map69.timer_irq_enabled = mapper69_timer_irq_enabled;
            }
            //MAPPER 91
            if (_engine.Memory.Map.Cartridge.MapperNo == 91)
            {
                var map91 = (Mapper91) _engine.Memory.Map.CurrentMapper;
                map91.IRQEnabled = mapper91_IRQEnabled;
                map91.IRQCount = mapper91_IRQCount;
            }

            #endregion
        }
    }
}