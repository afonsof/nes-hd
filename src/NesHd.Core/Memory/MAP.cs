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

using NesHd.Core.Memory.Mappers;

namespace NesHd.Core.Memory
{
    public class MAP
    {
        public MAP(Memory Mem, NesEngine Nes)
        {
            this._Cartridge = new Cart(Mem);
            this._Nes = Nes;
        }
        Cart _Cartridge;
        IMapper _CurrentMapper;
        Mapper09 Map9;
        Mapper10 Map10;
        NesEngine _Nes;
        bool _IsSRAMReadOnly = false;
        public uint[] current_prg_rom_page = new uint[8];
        public uint[] current_chr_rom_page = new uint[8];

        public void WritePRG(ushort Address, byte Value)
        { this._CurrentMapper.Write(Address, Value); }
        public void WriteCHR(ushort Address, byte Value)
        {
            if (this._Cartridge.IsVRAM)
            {
                if (Address < 0x400)
                {
                    this._Cartridge.CHR[this.current_chr_rom_page[0]][Address] = Value;
                }
                else if (Address < 0x800)
                {
                    this._Cartridge.CHR[this.current_chr_rom_page[1]][Address - 0x400] = Value;
                }
                else if (Address < 0xC00)
                {
                    this._Cartridge.CHR[this.current_chr_rom_page[2]][Address - 0x800] = Value;
                }
                else if (Address < 0x1000)
                {
                    this._Cartridge.CHR[this.current_chr_rom_page[3]][Address - 0xC00] = Value;
                }
                else if (Address < 0x1400)
                {
                    this._Cartridge.CHR[this.current_chr_rom_page[4]][Address - 0x1000] = Value;
                }
                else if (Address < 0x1800)
                {
                    this._Cartridge.CHR[this.current_chr_rom_page[5]][Address - 0x1400] = Value;
                }
                else if (Address < 0x1C00)
                {
                    this._Cartridge.CHR[this.current_chr_rom_page[6]][Address - 0x1800] = Value;
                }
                else
                {
                    this._Cartridge.CHR[this.current_chr_rom_page[7]][Address - 0x1C00] = Value;
                }
            }
        }
        public byte ReadPRG(ushort Address)
        {
            byte returnvalue = 0xff;
            if (Address < 0x9000)
            {
                returnvalue = this._Cartridge.PRG[this.current_prg_rom_page[0]][Address - 0x8000];
            }
            else if (Address < 0xA000)
            {
                returnvalue = this._Cartridge.PRG[this.current_prg_rom_page[1]][Address - 0x9000];
            }
            else if (Address < 0xB000)
            {
                returnvalue = this._Cartridge.PRG[this.current_prg_rom_page[2]][Address - 0xA000];
            }
            else if (Address < 0xC000)
            {
                returnvalue = this._Cartridge.PRG[this.current_prg_rom_page[3]][Address - 0xB000];
            }
            else if (Address < 0xD000)
            {
                returnvalue = this._Cartridge.PRG[this.current_prg_rom_page[4]][Address - 0xC000];
            }
            else if (Address < 0xE000)
            {
                returnvalue = this._Cartridge.PRG[this.current_prg_rom_page[5]][Address - 0xD000];
            }
            else if (Address < 0xF000)
            {
                returnvalue = this._Cartridge.PRG[this.current_prg_rom_page[6]][Address - 0xE000];
            }
            else
            {
                returnvalue = this._Cartridge.PRG[this.current_prg_rom_page[7]][Address - 0xF000];
            }
            return returnvalue;
        }
        public byte ReadCHR(ushort Address)
        {
            byte returnvalue = 0xff;

            if (Address < 0x400)
            {
                returnvalue = this._Cartridge.CHR[this.current_chr_rom_page[0]][Address];
            }
            else if (Address < 0x800)
            {
                returnvalue = this._Cartridge.CHR[this.current_chr_rom_page[1]][Address - 0x400];
            }
            else if (Address < 0xC00)
            {
                returnvalue = this._Cartridge.CHR[this.current_chr_rom_page[2]][Address - 0x800];
            }
            else if (Address < 0x1000)
            {
                if (this._Cartridge.MapperNo == 9)
                {
                    if ((Address >= 0xfd0) && (Address <= 0xfdf))
                    {
                        this.Map9.latch1 = 0xfd;
                        this.Switch4kChrRom(this.Map9.latch1data1, 1);
                    }
                    else if ((Address >= 0xfe0) && (Address <= 0xfef))
                    {
                        this.Map9.latch1 = 0xfe;
                        this.Switch4kChrRom(this.Map9.latch1data2, 1);
                    }
                }
                else if (this._Cartridge.MapperNo == 10)
                {
                    if ((Address >= 0xfd0) && (Address <= 0xfdf))
                    {
                        this.Map10.latch1 = 0xfd;
                        this.Switch4kChrRom(this.Map10.latch1data1, 0);
                    }
                    else if ((Address >= 0xfe0) && (Address <= 0xfef))
                    {
                        this.Map10.latch1 = 0xfe;
                        this.Switch4kChrRom(this.Map10.latch1data2, 0);
                    }
                }
                returnvalue = this._Cartridge.CHR[this.current_chr_rom_page[3]][Address - 0xC00];
            }
            else if (Address < 0x1400)
            {
                returnvalue = this._Cartridge.CHR[this.current_chr_rom_page[4]][Address - 0x1000];
            }
            else if (Address < 0x1800)
            {
                returnvalue = this._Cartridge.CHR[this.current_chr_rom_page[5]][Address - 0x1400];
            }
            else if (Address < 0x1C00)
            {
                returnvalue = this._Cartridge.CHR[this.current_chr_rom_page[6]][Address - 0x1800];
            }
            else
            {
                if (this._Cartridge.MapperNo == 9)
                {
                    if ((Address >= 0x1fd0) && (Address <= 0x1fdf))
                    {
                        this.Map9.latch1 = 0xfd;
                        this.Switch4kChrRom(this.Map9.latch1data1, 1);
                    }
                    else if ((Address >= 0x1fe0) && (Address <= 0x1fef))
                    {
                        this.Map9.latch1 = 0xfe;
                        this.Switch4kChrRom(this.Map9.latch1data2, 1);
                    }
                }
                else if (this._Cartridge.MapperNo == 10)
                {
                    if ((Address >= 0x1fd0) && (Address <= 0x1fdf))
                    {
                        this.Map10.latch2 = 0xfd;
                        this.Switch4kChrRom(this.Map10.latch2data1, 1);
                    }
                    else if ((Address >= 0x1fe0) && (Address <= 0x1fef))
                    {
                        this.Map10.latch2 = 0xfe;
                        this.Switch4kChrRom(this.Map10.latch2data2, 1);
                    }
                }
                returnvalue = this._Cartridge.CHR[this.current_chr_rom_page[7]][Address - 0x1C00];
            }
            return returnvalue;
        }

        //get which chr rom page is in use
        public uint ReadCHRPageNo(ushort Address)
        {
            uint returnvalue = UInt32.MaxValue;

            if (Address < 0x400)
            {
                returnvalue = this.current_chr_rom_page[0];
            }
            else if (Address < 0x800)
            {
                returnvalue = this.current_chr_rom_page[1];
            }
            else if (Address < 0xC00)
            {
                returnvalue = this.current_chr_rom_page[2];
            }
            else if (Address < 0x1000)
            {
                if (this._Cartridge.MapperNo == 9)
                {
                    if ((Address >= 0xfd0) && (Address <= 0xfdf))
                    {
                        this.Map9.latch1 = 0xfd;
                        this.Switch4kChrRom(this.Map9.latch1data1, 1);
                    }
                    else if ((Address >= 0xfe0) && (Address <= 0xfef))
                    {
                        this.Map9.latch1 = 0xfe;
                        this.Switch4kChrRom(this.Map9.latch1data2, 1);
                    }
                }
                else if (this._Cartridge.MapperNo == 10)
                {
                    if ((Address >= 0xfd0) && (Address <= 0xfdf))
                    {
                        this.Map10.latch1 = 0xfd;
                        this.Switch4kChrRom(this.Map10.latch1data1, 0);
                    }
                    else if ((Address >= 0xfe0) && (Address <= 0xfef))
                    {
                        this.Map10.latch1 = 0xfe;
                        this.Switch4kChrRom(this.Map10.latch1data2, 0);
                    }
                }
                returnvalue = this.current_chr_rom_page[3];
            }
            else if (Address < 0x1400)
            {
                returnvalue = this.current_chr_rom_page[4];
            }
            else if (Address < 0x1800)
            {
                returnvalue = this.current_chr_rom_page[5];
            }
            else if (Address < 0x1C00)
            {
                returnvalue = this.current_chr_rom_page[6];
            }
            else
            {
                if (this._Cartridge.MapperNo == 9)
                {
                    if ((Address >= 0x1fd0) && (Address <= 0x1fdf))
                    {
                        this.Map9.latch1 = 0xfd;
                        this.Switch4kChrRom(this.Map9.latch1data1, 1);
                    }
                    else if ((Address >= 0x1fe0) && (Address <= 0x1fef))
                    {
                        this.Map9.latch1 = 0xfe;
                        this.Switch4kChrRom(this.Map9.latch1data2, 1);
                    }
                }
                else if (this._Cartridge.MapperNo == 10)
                {
                    if ((Address >= 0x1fd0) && (Address <= 0x1fdf))
                    {
                        this.Map10.latch2 = 0xfd;
                        this.Switch4kChrRom(this.Map10.latch2data1, 1);
                    }
                    else if ((Address >= 0x1fe0) && (Address <= 0x1fef))
                    {
                        this.Map10.latch2 = 0xfe;
                        this.Switch4kChrRom(this.Map10.latch2data2, 1);
                    }
                }
                returnvalue = this.current_chr_rom_page[7];
            }
            return returnvalue;
        }

        public void TickScanlineTimer()
        { this._CurrentMapper.TickScanlineTimer(); }
        public void TickCycleTimer()
        { this._CurrentMapper.TickCycleTimer(); }
        //* 8
        public void Switch32kPrgRom(int start)
        {
            int i;
            switch (this._Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
                case (128): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 8; i++)
            {
                this.current_prg_rom_page[i] = (uint)(start + i);
            }
        }
        //* 4
        //area 0,1
        public void Switch16kPrgRom(int start, int area)
        {
            int i;
            switch (this._Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (31): start = (start & 0x7f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
                case (128): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 4; i++)
            {
                this.current_prg_rom_page[4 * area + i] = (uint)(start + i);
            }
        }
        //* 2
        //area 0,1,2,3
        public void Switch8kPrgRom(int start, int area)
        {
            int i;
            switch (this._Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
                case (128): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 2; i++)
            {
                this.current_prg_rom_page[2 * area + i] = (uint)(start + i);
            }
        }
        //* 8
        public void Switch8kChrRom(int start)
        {
            int i;
            switch (this._Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 8; i++)
            {
                this.current_chr_rom_page[i] = (uint)(start + i);
            }
        }
        //* 4
        //area 0,1
        public void Switch4kChrRom(int start, int area)
        {
            int i;
            switch (this._Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 4; i++)
            {
                this.current_chr_rom_page[4 * area + i] = (uint)(start + i);
            }
        }
        //* 2 
        //area 0,1,2,3
        public void Switch2kChrRom(int start, int area)
        {
            int i;
            switch (this._Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 2; i++)
            {
                this.current_chr_rom_page[2 * area + i] = (uint)(start + i);
            }
        }
        //area 0,1,2,3,4,5,6,7
        public void Switch1kChrRom(int start, int area)
        {
            switch (this._Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
            }
            this.current_chr_rom_page[area] = (uint)(start);
        }
        //area 0,1,2,3
        public void Switch1kVROMToVRAM(int start, int area)
        {
            switch (this._Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
            }
            //current_chr_rom_page[area] = (uint)(start);
            switch (area)
            {
                case 0:
                    for (int i = 0; i < 0x400; i++)
                    {
                        this._Nes.PPU.VRAM[i] = this._Cartridge.CHR[start][i];
                    }
                    break;
                case 1:
                    for (int i = 0x400; i < 0x800; i++)
                    {
                        this._Nes.PPU.VRAM[i] = this._Cartridge.CHR[start][i - 0x400];
                    }
                    break;
                case 2:
                    for (int i = 0x800; i < 0xC00; i++)
                    {
                        this._Nes.PPU.VRAM[i] = this._Cartridge.CHR[start][i - 0x800];
                    }
                    break;
                case 3:
                    for (int i = 0xC00; i < 0x1000; i++)
                    {
                        this._Nes.PPU.VRAM[i] = this._Cartridge.CHR[start][i - 0xC00];
                    }
                    break;
            }
        }
        //area 0,1,2,3
        public void SwitchVRAMToVRAM(int start, int area)
        {
            //current_chr_rom_page[area] = (uint)(start);
            switch (start)
            {
                case 0: start = 0x000; break;
                case 1: start = 0x400; break;
                case 2: start = 0x800; break;
                case 3: start = 0xC00; break;
            }
            switch (area)
            {
                case 0:
                    for (int i = 0; i < 0x400; i++)
                    {
                        this._Nes.PPU.VRAM[i] = this._Nes.PPU.VRAM[i + start];
                    }
                    break;
                case 1:
                    for (int i = 0x400; i < 0x800; i++)
                    {
                        this._Nes.PPU.VRAM[i] = this._Nes.PPU.VRAM[i - 0x400 + start];
                    }
                    break;
                case 2:
                    for (int i = 0x800; i < 0xC00; i++)
                    {
                        this._Nes.PPU.VRAM[i] = this._Nes.PPU.VRAM[i - 0x800 + start];
                    }
                    break;
                case 3:
                    for (int i = 0xC00; i < 0x1000; i++)
                    {
                        this._Nes.PPU.VRAM[i] = this._Nes.PPU.VRAM[i - 0xC00 + start];
                    }
                    break;
            }
        }

        /*WARNING*/
        /*INITIALIZE YOUR NEW MAPPER HERE*/
        public void InitializeMapper()
        {
            switch (this._Cartridge.MapperNo)
            {
                case 0:
                    this._CurrentMapper = new Mapper00(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 1:
                    this._CurrentMapper = new Mapper01(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 2:
                    this._CurrentMapper = new Mapper02(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 3:
                    this._CurrentMapper = new Mapper03(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 4:
                    this._CurrentMapper = new Mapper04(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 5:
                    this._CurrentMapper = new Mapper05(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 6:
                    this._CurrentMapper = new Mapper06(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 7:
                    this._CurrentMapper = new Mapper07(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 8:
                    this._CurrentMapper = new Mapper08(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 9:
                    this._CurrentMapper = new Mapper09(this);
                    this.Map9 = (Mapper09)this._CurrentMapper;
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 10:
                    this._CurrentMapper = new Mapper10(this);
                    this.Map10 = (Mapper10)this._CurrentMapper;
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 11:
                    this._CurrentMapper = new Mapper11(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 13:
                    this._CurrentMapper = new Mapper13(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 15:
                    this._CurrentMapper = new Mapper15(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 16:
                    this._CurrentMapper = new Mapper16(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 17:
                    this._CurrentMapper = new Mapper17(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 18:
                    this._CurrentMapper = new Mapper18(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 19:
                    this._CurrentMapper = new Mapper19(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 21:
                    this._CurrentMapper = new Mapper21(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 22:
                    this._CurrentMapper = new Mapper22(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 23:
                    this._CurrentMapper = new Mapper23(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 24:
                    this._CurrentMapper = new Mapper24(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 32:
                    this._CurrentMapper = new Mapper32(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 33:
                    this._CurrentMapper = new Mapper33(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 34:
                    this._CurrentMapper = new Mapper34(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 41:
                    this._CurrentMapper = new Mapper41(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 48:
                    this._CurrentMapper = new Mapper33(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 61:
                    this._CurrentMapper = new Mapper61(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 64:
                    this._CurrentMapper = new Mapper64(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 65:
                    this._CurrentMapper = new Mapper65(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 66:
                    this._CurrentMapper = new Mapper66(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 69:
                    this._CurrentMapper = new Mapper69(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 71:
                    this._CurrentMapper = new Mapper71(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 78:
                    this._CurrentMapper = new Mapper71(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 79:
                    this._CurrentMapper = new Mapper79(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 80:
                    this._CurrentMapper = new Mapper80(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 81:
                    this._CurrentMapper = new Mapper113(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 82:
                    this._CurrentMapper = new Mapper82(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 91:
                    this._CurrentMapper = new Mapper91(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 113:
                    this._CurrentMapper = new Mapper113(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 225:
                    this._CurrentMapper = new Mapper225_255(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
                case 255:
                    this._CurrentMapper = new Mapper225_255(this);
                    this._CurrentMapper.SetUpMapperDefaults();
                    break;
            }
        }

        //Properties
        /// <summary>
        /// Get the current cart
        /// </summary>
        public Cart Cartridge
        { get { return this._Cartridge; } }
        public IMapper CurrentMapper
        { get { return this._CurrentMapper; } set { this._CurrentMapper = value; } }
        public bool IsSRAMReadOnly
        { get { return this._IsSRAMReadOnly; } set { this._IsSRAMReadOnly = value; } }
        public NesEngine NES
        { get { return this._Nes; } }
    }
}
