using NesHd.Core.Memory.Mappers;

namespace NesHd.Core.Memory
{
    public class Map
    {
        private Mapper10 _mapper10;
        private Mapper09 _mapper09;
        public uint[] CurrentChrRomPage = new uint[8];
        public uint[] CurrentPrgRomPage = new uint[8];

        public Map(Memory memory, NesEngine engine)
        {
            Engine = engine;
            Cartridge = new Cartridge(memory);
        }

        /// <summary>
        /// Get the current cart
        /// </summary>
        public Cartridge Cartridge { get; private set; }
        public IMapper CurrentMapper { get; set; }
        public bool IsSRamReadOnly { get; set; }
        public NesEngine Engine { get; private set; }


        public void WritePrg(ushort address, byte value)
        {
            CurrentMapper.Write(address, value);
        }

        public void WriteChr(ushort address, byte value)
        {
            if (Cartridge.IsVram)
            {
                if (address < 0x400)
                {
                    Cartridge.Chr[CurrentChrRomPage[0]][address] = value;
                }
                else if (address < 0x800)
                {
                    Cartridge.Chr[CurrentChrRomPage[1]][address - 0x400] = value;
                }
                else if (address < 0xC00)
                {
                    Cartridge.Chr[CurrentChrRomPage[2]][address - 0x800] = value;
                }
                else if (address < 0x1000)
                {
                    Cartridge.Chr[CurrentChrRomPage[3]][address - 0xC00] = value;
                }
                else if (address < 0x1400)
                {
                    Cartridge.Chr[CurrentChrRomPage[4]][address - 0x1000] = value;
                }
                else if (address < 0x1800)
                {
                    Cartridge.Chr[CurrentChrRomPage[5]][address - 0x1400] = value;
                }
                else if (address < 0x1C00)
                {
                    Cartridge.Chr[CurrentChrRomPage[6]][address - 0x1800] = value;
                }
                else
                {
                    Cartridge.Chr[CurrentChrRomPage[7]][address - 0x1C00] = value;
                }
            }
        }

        public byte ReadPrg(ushort address)
        {
            byte returnvalue;
            if (address < 0x9000)
            {
                returnvalue = Cartridge.Prg[CurrentPrgRomPage[0]][address - 0x8000];
            }
            else if (address < 0xA000)
            {
                returnvalue = Cartridge.Prg[CurrentPrgRomPage[1]][address - 0x9000];
            }
            else if (address < 0xB000)
            {
                returnvalue = Cartridge.Prg[CurrentPrgRomPage[2]][address - 0xA000];
            }
            else if (address < 0xC000)
            {
                returnvalue = Cartridge.Prg[CurrentPrgRomPage[3]][address - 0xB000];
            }
            else if (address < 0xD000)
            {
                returnvalue = Cartridge.Prg[CurrentPrgRomPage[4]][address - 0xC000];
            }
            else if (address < 0xE000)
            {
                returnvalue = Cartridge.Prg[CurrentPrgRomPage[5]][address - 0xD000];
            }
            else if (address < 0xF000)
            {
                returnvalue = Cartridge.Prg[CurrentPrgRomPage[6]][address - 0xE000];
            }
            else
            {
                returnvalue = Cartridge.Prg[CurrentPrgRomPage[7]][address - 0xF000];
            }
            return returnvalue;
        }

        public byte ReadChr(ushort address)
        {
            byte returnvalue;

            if (address < 0x400)
            {
                returnvalue = Cartridge.Chr[CurrentChrRomPage[0]][address];
            }
            else if (address < 0x800)
            {
                returnvalue = Cartridge.Chr[CurrentChrRomPage[1]][address - 0x400];
            }
            else if (address < 0xC00)
            {
                returnvalue = Cartridge.Chr[CurrentChrRomPage[2]][address - 0x800];
            }
            else if (address < 0x1000)
            {
                if (Cartridge.MapperNo == 9)
                {
                    if ((address >= 0xfd0) && (address <= 0xfdf))
                    {
                        _mapper09.latch1 = 0xfd;
                        Switch4KChrRom(_mapper09.latch1data1, 1);
                    }
                    else if ((address >= 0xfe0) && (address <= 0xfef))
                    {
                        _mapper09.latch1 = 0xfe;
                        Switch4KChrRom(_mapper09.latch1data2, 1);
                    }
                }
                else if (Cartridge.MapperNo == 10)
                {
                    if ((address >= 0xfd0) && (address <= 0xfdf))
                    {
                        _mapper10.Latch1 = 0xfd;
                        Switch4KChrRom(_mapper10.Latch1Data1, 0);
                    }
                    else if ((address >= 0xfe0) && (address <= 0xfef))
                    {
                        _mapper10.Latch1 = 0xfe;
                        Switch4KChrRom(_mapper10.Latch1Data2, 0);
                    }
                }
                returnvalue = Cartridge.Chr[CurrentChrRomPage[3]][address - 0xC00];
            }
            else if (address < 0x1400)
            {
                returnvalue = Cartridge.Chr[CurrentChrRomPage[4]][address - 0x1000];
            }
            else if (address < 0x1800)
            {
                returnvalue = Cartridge.Chr[CurrentChrRomPage[5]][address - 0x1400];
            }
            else if (address < 0x1C00)
            {
                returnvalue = Cartridge.Chr[CurrentChrRomPage[6]][address - 0x1800];
            }
            else
            {
                if (Cartridge.MapperNo == 9)
                {
                    if ((address >= 0x1fd0) && (address <= 0x1fdf))
                    {
                        _mapper09.latch1 = 0xfd;
                        Switch4KChrRom(_mapper09.latch1data1, 1);
                    }
                    else if ((address >= 0x1fe0) && (address <= 0x1fef))
                    {
                        _mapper09.latch1 = 0xfe;
                        Switch4KChrRom(_mapper09.latch1data2, 1);
                    }
                }
                else if (Cartridge.MapperNo == 10)
                {
                    if ((address >= 0x1fd0) && (address <= 0x1fdf))
                    {
                        _mapper10.Latch2 = 0xfd;
                        Switch4KChrRom(_mapper10.Latch2Data1, 1);
                    }
                    else if ((address >= 0x1fe0) && (address <= 0x1fef))
                    {
                        _mapper10.Latch2 = 0xfe;
                        Switch4KChrRom(_mapper10.Latch2Data2, 1);
                    }
                }
                returnvalue = Cartridge.Chr[CurrentChrRomPage[7]][address - 0x1C00];
            }
            return returnvalue;
        }

        //get which chr rom page is in use
        public uint ReadChrPageNo(ushort address)
        {
            uint returnvalue;

            if (address < 0x400)
            {
                returnvalue = CurrentChrRomPage[0];
            }
            else if (address < 0x800)
            {
                returnvalue = CurrentChrRomPage[1];
            }
            else if (address < 0xC00)
            {
                returnvalue = CurrentChrRomPage[2];
            }
            else if (address < 0x1000)
            {
                if (Cartridge.MapperNo == 9)
                {
                    if ((address >= 0xfd0) && (address <= 0xfdf))
                    {
                        _mapper09.latch1 = 0xfd;
                        Switch4KChrRom(_mapper09.latch1data1, 1);
                    }
                    else if ((address >= 0xfe0) && (address <= 0xfef))
                    {
                        _mapper09.latch1 = 0xfe;
                        Switch4KChrRom(_mapper09.latch1data2, 1);
                    }
                }
                else if (Cartridge.MapperNo == 10)
                {
                    if ((address >= 0xfd0) && (address <= 0xfdf))
                    {
                        _mapper10.Latch1 = 0xfd;
                        Switch4KChrRom(_mapper10.Latch1Data1, 0);
                    }
                    else if ((address >= 0xfe0) && (address <= 0xfef))
                    {
                        _mapper10.Latch1 = 0xfe;
                        Switch4KChrRom(_mapper10.Latch1Data2, 0);
                    }
                }
                returnvalue = CurrentChrRomPage[3];
            }
            else if (address < 0x1400)
            {
                returnvalue = CurrentChrRomPage[4];
            }
            else if (address < 0x1800)
            {
                returnvalue = CurrentChrRomPage[5];
            }
            else if (address < 0x1C00)
            {
                returnvalue = CurrentChrRomPage[6];
            }
            else
            {
                if (Cartridge.MapperNo == 9)
                {
                    if ((address >= 0x1fd0) && (address <= 0x1fdf))
                    {
                        _mapper09.latch1 = 0xfd;
                        Switch4KChrRom(_mapper09.latch1data1, 1);
                    }
                    else if ((address >= 0x1fe0) && (address <= 0x1fef))
                    {
                        _mapper09.latch1 = 0xfe;
                        Switch4KChrRom(_mapper09.latch1data2, 1);
                    }
                }
                else if (Cartridge.MapperNo == 10)
                {
                    if ((address >= 0x1fd0) && (address <= 0x1fdf))
                    {
                        _mapper10.Latch2 = 0xfd;
                        Switch4KChrRom(_mapper10.Latch2Data1, 1);
                    }
                    else if ((address >= 0x1fe0) && (address <= 0x1fef))
                    {
                        _mapper10.Latch2 = 0xfe;
                        Switch4KChrRom(_mapper10.Latch2Data2, 1);
                    }
                }
                returnvalue = CurrentChrRomPage[7];
            }
            return returnvalue;
        }

        public void TickScanlineTimer()
        {
            CurrentMapper.TickScanlineTimer();
        }

        public void TickCycleTimer()
        {
            CurrentMapper.TickCycleTimer();
        }

        //* 8
        public void Switch32KPrgRom(int start)
        {
            int i;
            switch (Cartridge.PrgPages)
            {
                case (2):
                    start = (start & 0x7);
                    break;
                case (4):
                    start = (start & 0xf);
                    break;
                case (8):
                    start = (start & 0x1f);
                    break;
                case (16):
                    start = (start & 0x3f);
                    break;
                case (32):
                    start = (start & 0x7f);
                    break;
                case (64):
                    start = (start & 0xff);
                    break;
                case (128):
                    start = (start & 0x1ff);
                    break;
            }
            for (i = 0; i < 8; i++)
            {
                CurrentPrgRomPage[i] = (uint) (start + i);
            }
        }

        //* 4
        //area 0,1
        public void Switch16KPrgRom(int start, int area)
        {
            int i;
            switch (Cartridge.PrgPages)
            {
                case (2):
                    start = (start & 0x7);
                    break;
                case (4):
                    start = (start & 0xf);
                    break;
                case (8):
                    start = (start & 0x1f);
                    break;
                case (16):
                    start = (start & 0x3f);
                    break;
                case (31):
                    start = (start & 0x7f);
                    break;
                case (32):
                    start = (start & 0x7f);
                    break;
                case (64):
                    start = (start & 0xff);
                    break;
                case (128):
                    start = (start & 0x1ff);
                    break;
            }
            for (i = 0; i < 4; i++)
            {
                CurrentPrgRomPage[4*area + i] = (uint) (start + i);
            }
        }

        //* 2
        //area 0,1,2,3
        public void Switch8KPrgRom(int start, int area)
        {
            int i;
            switch (Cartridge.PrgPages)
            {
                case (2):
                    start = (start & 0x7);
                    break;
                case (4):
                    start = (start & 0xf);
                    break;
                case (8):
                    start = (start & 0x1f);
                    break;
                case (16):
                    start = (start & 0x3f);
                    break;
                case (32):
                    start = (start & 0x7f);
                    break;
                case (64):
                    start = (start & 0xff);
                    break;
                case (128):
                    start = (start & 0x1ff);
                    break;
            }
            for (i = 0; i < 2; i++)
            {
                CurrentPrgRomPage[2*area + i] = (uint) (start + i);
            }
        }

        //* 8
        public void Switch8KChrRom(int start)
        {
            int i;
            switch (Cartridge.ChrPages)
            {
                case (2):
                    start = (start & 0xf);
                    break;
                case (4):
                    start = (start & 0x1f);
                    break;
                case (8):
                    start = (start & 0x3f);
                    break;
                case (16):
                    start = (start & 0x7f);
                    break;
                case (32):
                    start = (start & 0xff);
                    break;
                case (64):
                    start = (start & 0x1ff);
                    break;
            }
            for (i = 0; i < 8; i++)
            {
                CurrentChrRomPage[i] = (uint) (start + i);
            }
        }

        //* 4
        //area 0,1
        public void Switch4KChrRom(int start, int area)
        {
            int i;
            switch (Cartridge.ChrPages)
            {
                case (2):
                    start = (start & 0xf);
                    break;
                case (4):
                    start = (start & 0x1f);
                    break;
                case (8):
                    start = (start & 0x3f);
                    break;
                case (16):
                    start = (start & 0x7f);
                    break;
                case (32):
                    start = (start & 0xff);
                    break;
                case (64):
                    start = (start & 0x1ff);
                    break;
            }
            for (i = 0; i < 4; i++)
            {
                CurrentChrRomPage[4*area + i] = (uint) (start + i);
            }
        }

        //* 2 
        //area 0,1,2,3
        public void Switch2KChrRom(int start, int area)
        {
            int i;
            switch (Cartridge.ChrPages)
            {
                case (2):
                    start = (start & 0xf);
                    break;
                case (4):
                    start = (start & 0x1f);
                    break;
                case (8):
                    start = (start & 0x3f);
                    break;
                case (16):
                    start = (start & 0x7f);
                    break;
                case (32):
                    start = (start & 0xff);
                    break;
                case (64):
                    start = (start & 0x1ff);
                    break;
            }
            for (i = 0; i < 2; i++)
            {
                CurrentChrRomPage[2*area + i] = (uint) (start + i);
            }
        }

        //area 0,1,2,3,4,5,6,7
        public void Switch1KChrRom(int start, int area)
        {
            switch (Cartridge.ChrPages)
            {
                case (2):
                    start = (start & 0xf);
                    break;
                case (4):
                    start = (start & 0x1f);
                    break;
                case (8):
                    start = (start & 0x3f);
                    break;
                case (16):
                    start = (start & 0x7f);
                    break;
                case (32):
                    start = (start & 0xff);
                    break;
                case (64):
                    start = (start & 0x1ff);
                    break;
            }
            CurrentChrRomPage[area] = (uint) (start);
        }

        //area 0,1,2,3
        public void Switch1kVRomToVRam(int start, int area)
        {
            switch (Cartridge.ChrPages)
            {
                case (2):
                    start = (start & 0xf);
                    break;
                case (4):
                    start = (start & 0x1f);
                    break;
                case (8):
                    start = (start & 0x3f);
                    break;
                case (16):
                    start = (start & 0x7f);
                    break;
                case (32):
                    start = (start & 0xff);
                    break;
                case (64):
                    start = (start & 0x1ff);
                    break;
            }
            //CurrentChrRomPage[area] = (uint)(start);
            switch (area)
            {
                case 0:
                    for (var i = 0; i < 0x400; i++)
                    {
                        Engine.Ppu.VRam[i] = Cartridge.Chr[start][i];
                    }
                    break;
                case 1:
                    for (var i = 0x400; i < 0x800; i++)
                    {
                        Engine.Ppu.VRam[i] = Cartridge.Chr[start][i - 0x400];
                    }
                    break;
                case 2:
                    for (var i = 0x800; i < 0xC00; i++)
                    {
                        Engine.Ppu.VRam[i] = Cartridge.Chr[start][i - 0x800];
                    }
                    break;
                case 3:
                    for (var i = 0xC00; i < 0x1000; i++)
                    {
                        Engine.Ppu.VRam[i] = Cartridge.Chr[start][i - 0xC00];
                    }
                    break;
            }
        }

        //area 0,1,2,3
        public void SwitchVRamToVRam(int start, int area)
        {
            //CurrentChrRomPage[area] = (uint)(start);
            switch (start)
            {
                case 0:
                    start = 0x000;
                    break;
                case 1:
                    start = 0x400;
                    break;
                case 2:
                    start = 0x800;
                    break;
                case 3:
                    start = 0xC00;
                    break;
            }
            switch (area)
            {
                case 0:
                    for (var i = 0; i < 0x400; i++)
                    {
                        Engine.Ppu.VRam[i] = Engine.Ppu.VRam[i + start];
                    }
                    break;
                case 1:
                    for (var i = 0x400; i < 0x800; i++)
                    {
                        Engine.Ppu.VRam[i] = Engine.Ppu.VRam[i - 0x400 + start];
                    }
                    break;
                case 2:
                    for (var i = 0x800; i < 0xC00; i++)
                    {
                        Engine.Ppu.VRam[i] = Engine.Ppu.VRam[i - 0x800 + start];
                    }
                    break;
                case 3:
                    for (var i = 0xC00; i < 0x1000; i++)
                    {
                        Engine.Ppu.VRam[i] = Engine.Ppu.VRam[i - 0xC00 + start];
                    }
                    break;
            }
        }

        /*WARNING*/
        /*INITIALIZE YOUR NEW MAPPER HERE*/

        public void InitializeMapper()
        {
            switch (Cartridge.MapperNo)
            {
                case 0:
                    CurrentMapper = new Mapper00(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 1:
                    CurrentMapper = new Mapper01(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 2:
                    CurrentMapper = new Mapper02(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 3:
                    CurrentMapper = new Mapper03(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 4:
                    CurrentMapper = new Mapper04(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 5:
                    CurrentMapper = new Mapper05(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 6:
                    CurrentMapper = new Mapper06(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 7:
                    CurrentMapper = new Mapper07(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 8:
                    CurrentMapper = new Mapper08(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 9:
                    CurrentMapper = new Mapper09(this);
                    _mapper09 = (Mapper09) CurrentMapper;
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 10:
                    CurrentMapper = new Mapper10(this);
                    _mapper10 = (Mapper10) CurrentMapper;
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 11:
                    CurrentMapper = new Mapper11(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 13:
                    CurrentMapper = new Mapper13(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 15:
                    CurrentMapper = new Mapper15(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 16:
                    CurrentMapper = new Mapper16(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 17:
                    CurrentMapper = new Mapper17(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 18:
                    CurrentMapper = new Mapper18(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 19:
                    CurrentMapper = new Mapper19(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 21:
                    CurrentMapper = new Mapper21(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 22:
                    CurrentMapper = new Mapper22(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 23:
                    CurrentMapper = new Mapper23(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 24:
                    CurrentMapper = new Mapper24(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 32:
                    CurrentMapper = new Mapper32(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 33:
                    CurrentMapper = new Mapper33(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 34:
                    CurrentMapper = new Mapper34(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 41:
                    CurrentMapper = new Mapper41(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 48:
                    CurrentMapper = new Mapper33(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 61:
                    CurrentMapper = new Mapper61(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 64:
                    CurrentMapper = new Mapper64(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 65:
                    CurrentMapper = new Mapper65(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 66:
                    CurrentMapper = new Mapper66(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 69:
                    CurrentMapper = new Mapper69(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 71:
                    CurrentMapper = new Mapper71(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 78:
                    CurrentMapper = new Mapper71(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 79:
                    CurrentMapper = new Mapper79(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 80:
                    CurrentMapper = new Mapper80(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 81:
                    CurrentMapper = new Mapper113(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 82:
                    CurrentMapper = new Mapper82(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 91:
                    CurrentMapper = new Mapper91(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 113:
                    CurrentMapper = new Mapper113(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 225:
                    CurrentMapper = new Mapper225_255(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
                case 255:
                    CurrentMapper = new Mapper225_255(this);
                    CurrentMapper.SetUpMapperDefaults();
                    break;
            }
        }

        //Properties
    }
}