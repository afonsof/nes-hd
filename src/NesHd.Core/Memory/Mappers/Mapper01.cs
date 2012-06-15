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

using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    class Mapper01 : IMapper
    {
        MAP Map;
        public int mapper1_register8000BitPosition;
        public int mapper1_registerA000BitPosition;
        public int mapper1_registerC000BitPosition;
        public int mapper1_registerE000BitPosition;
        public int mapper1_register8000Value;
        public int mapper1_registerA000Value;
        public int mapper1_registerC000Value;
        public int mapper1_registerE000Value;
        public byte mapper1_mirroringFlag;
        public byte mapper1_onePageMirroring;
        public byte mapper1_prgSwitchingArea;
        public byte mapper1_prgSwitchingSize;
        public byte mapper1_vromSwitchingSize;
        public Mapper01(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            //Using Mapper #1
            if ((address >= 0x8000) && (address <= 0x9FFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    this.mapper1_register8000BitPosition = 0;
                    this.mapper1_register8000Value = 0;
                    this.mapper1_mirroringFlag = 0;
                    this.mapper1_onePageMirroring = 1;
                    this.mapper1_prgSwitchingArea = 1;
                    this.mapper1_prgSwitchingSize = 1;
                    this.mapper1_vromSwitchingSize = 0;
                }
                else
                {
                    this.mapper1_register8000Value += (data & 0x1) << this.mapper1_register8000BitPosition;
                    this.mapper1_register8000BitPosition++;
                    if (this.mapper1_register8000BitPosition == 5)
                    {
                        this.mapper1_mirroringFlag = (byte)(this.mapper1_register8000Value & 0x1);
                        if (this.mapper1_mirroringFlag == 0)
                        {
                            this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                        }
                        else
                        {
                            this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;
                        }
                        this.mapper1_onePageMirroring = (byte)((this.mapper1_register8000Value >> 1) & 0x1);
                        if (this.mapper1_onePageMirroring == 0)
                        {
                            this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                            this.Map.Cartridge.MirroringBase = 0x2000;
                        }
                        this.mapper1_prgSwitchingArea = (byte)((this.mapper1_register8000Value >> 2) & 0x1);
                        this.mapper1_prgSwitchingSize = (byte)((this.mapper1_register8000Value >> 3) & 0x1);
                        this.mapper1_vromSwitchingSize = (byte)((this.mapper1_register8000Value >> 4) & 0x1);
                        this.mapper1_register8000BitPosition = 0;
                        this.mapper1_register8000Value = 0;
                        this.mapper1_registerA000BitPosition = 0;
                        this.mapper1_registerA000Value = 0;
                        this.mapper1_registerC000BitPosition = 0;
                        this.mapper1_registerC000Value = 0;
                        this.mapper1_registerE000BitPosition = 0;
                        this.mapper1_registerE000Value = 0;
                    }
                }
            }
            else if ((address >= 0xA000) && (address <= 0xBFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    this.mapper1_registerA000BitPosition = 0;
                    this.mapper1_registerA000Value = 0;
                }
                else
                {
                    this.mapper1_registerA000Value += (data & 0x1) << this.mapper1_registerA000BitPosition;
                    this.mapper1_registerA000BitPosition++;
                    if (this.mapper1_registerA000BitPosition == 5)
                    {
                        if (this.Map.Cartridge.CHR_PAGES > 0)
                        {
                            if (this.mapper1_vromSwitchingSize == 1)
                            {
                                this.Map.Switch4kChrRom(this.mapper1_registerA000Value * 4, 0);
                            }
                            else
                            {
                                this.Map.Switch8kChrRom((this.mapper1_registerA000Value >> 1) * 8);
                            }
                        }
                        this.mapper1_registerA000BitPosition = 0;
                        this.mapper1_registerA000Value = 0;
                    }
                }
            }
            else if ((address >= 0xC000) && (address <= 0xDFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    this.mapper1_registerC000BitPosition = 0;
                    this.mapper1_registerC000Value = 0;
                }
                else
                {
                    this.mapper1_registerC000Value += (data & 0x1) << this.mapper1_registerC000BitPosition;
                    this.mapper1_registerC000BitPosition++;
                    if (this.mapper1_registerC000BitPosition == 5)
                    {
                        if (this.Map.Cartridge.CHR_PAGES > 0)
                        {
                            if (this.mapper1_vromSwitchingSize == 1)
                            {
                                this.Map.Switch4kChrRom(this.mapper1_registerC000Value * 4, 1);
                            }
                        }
                        this.mapper1_registerC000BitPosition = 0;
                        this.mapper1_registerC000Value = 0;
                    }
                }
            }
            else if ((address >= 0xE000) && (address <= 0xFFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    this.mapper1_registerE000BitPosition = 0;
                    this.mapper1_registerE000Value = 0;
                    this.mapper1_registerA000BitPosition = 0;
                    this.mapper1_registerA000Value = 0;
                    this.mapper1_registerC000BitPosition = 0;
                    this.mapper1_registerC000Value = 0;
                    this.mapper1_register8000BitPosition = 0;
                    this.mapper1_register8000Value = 0;
                }
                else
                {
                    this.mapper1_registerE000Value += (data & 0x1) << this.mapper1_registerE000BitPosition;
                    this.mapper1_registerE000BitPosition++;
                    if (this.mapper1_registerE000BitPosition == 5)
                    {
                        if (this.mapper1_prgSwitchingSize == 1)
                        {
                            if (this.mapper1_prgSwitchingArea == 1)
                            {
                                // Switch bank at 0x8000 and reset 0xC000
                                this.Map.Switch16kPrgRom(this.mapper1_registerE000Value * 4, 0);
                                this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
                            }
                            else
                            {
                                // Switch bank at 0xC000 and reset 0x8000
                                this.Map.Switch16kPrgRom(this.mapper1_registerE000Value * 4, 1);
                                this.Map.Switch16kPrgRom(0, 0);
                            }
                        }
                        else
                        {
                            //Full 32k switch
                            this.Map.Switch32kPrgRom((this.mapper1_registerE000Value >> 1) * 8);
                        }
                        this.mapper1_registerE000BitPosition = 0;
                        this.mapper1_registerE000Value = 0;
                    }
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            this.mapper1_register8000BitPosition = 0;
            this.mapper1_register8000Value = 0;
            this.mapper1_mirroringFlag = 0;
            this.mapper1_onePageMirroring = 1;
            this.mapper1_prgSwitchingArea = 1;
            this.mapper1_prgSwitchingSize = 1;
            this.mapper1_vromSwitchingSize = 0;
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 1 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer()
        {
        }
        public void SoftReset()
        { }
        public bool WriteUnder8000
        { get { return false; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
