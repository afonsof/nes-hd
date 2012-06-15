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
    /*
     * I'm not sure about this one so it's disabled....
     */
    class Mapper05 : IMapper
    {
        MAP Map;
        public byte mapper5_prgBankSize;
        public byte mapper5_chrBankSize;
        public int mapper5_scanlineSplit;
        public bool mapper5_splitIrqEnabled;
        public int graphic_mode = 0;
        public bool IQREnabled = false;
        public byte IRQCounter = 0;
        public Mapper05(MAP map)
        { this.Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x5100)
            {
                this.mapper5_prgBankSize = (byte)(data & 0x3);
            }
            else if (address == 0x5101)
            {
                this.mapper5_chrBankSize = (byte)(data & 0x3);
            }
            else if (address == 0x5102)
            {
                this.Map.NES.MEMORY.WriteOnRam = (data & 0x3) == 0x02;
            }
            else if (address == 0x5103)
            {
                this.Map.NES.MEMORY.WriteOnRam = (data & 0x3) == 0x01;
            }
            else if (address == 0x5104)
            {
                this.graphic_mode = (data & 0x3);
            }
            else if (address == 0x5105)
            {
                this.Map.SwitchVRAMToVRAM((data & 0x3), 0);
                this.Map.SwitchVRAMToVRAM(((data & 0xC) >> 2), 1);
                this.Map.SwitchVRAMToVRAM(((data & 0x30) >> 4), 2);
                this.Map.SwitchVRAMToVRAM(((data & 0xC0) >> 6), 3);
            }
            else if (address == 0x5106)
            {

            }
            else if (address == 0x5107)
            {

            }
            else if (address == 0x5114)
            {
                if (this.mapper5_prgBankSize == 3)
                {
                    this.Map.Switch8kPrgRom((data & 0x7f) * 2, 0);
                }
            }
            else if (address == 0x5115)
            {
                if (this.mapper5_prgBankSize == 1)
                {
                    this.Map.Switch16kPrgRom((data & 0x7e) * 2, 0);
                }
                else if (this.mapper5_prgBankSize == 2)
                {
                    this.Map.Switch16kPrgRom((data & 0x7e) * 2, 0);
                }
                else if (this.mapper5_prgBankSize == 3)
                {
                    this.Map.Switch8kPrgRom((data & 0x7f) * 2, 1);
                }
            }
            else if (address == 0x5116)
            {
                if (this.mapper5_prgBankSize == 2)
                {
                    this.Map.Switch8kPrgRom((data & 0x7f) * 2, 2);
                }
                else if (this.mapper5_prgBankSize == 3)
                {
                    this.Map.Switch8kPrgRom((data & 0x7f) * 2, 2);
                }
            }
            else if (address == 0x5117)
            {
                if (this.mapper5_prgBankSize == 0)
                {
                    this.Map.Switch32kPrgRom((data & 0x7c) * 2);
                }
                else if (this.mapper5_prgBankSize == 1)
                {
                    this.Map.Switch16kPrgRom((data & 0x7e) * 2, 1);
                }
                else if (this.mapper5_prgBankSize == 2)
                {
                    this.Map.Switch8kPrgRom((data & 0x7f) * 2, 3);
                }
                else if (this.mapper5_prgBankSize == 3)
                {
                    this.Map.Switch8kPrgRom((data & 0x7f) * 2, 3);
                }

            }
            else if (address == 0x5120)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 0);
                }
            }
            else if (address == 0x5121)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 1);
                }
                else if (this.mapper5_chrBankSize == 2)
                {
                    this.Map.Switch2kChrRom(data, 0);
                }
            }
            else if (address == 0x5122)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 2);
                }
            }
            else if (address == 0x5123)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 3);
                }
                else if (this.mapper5_chrBankSize == 2)
                {
                    this.Map.Switch2kChrRom(data, 1);
                }
                else if (this.mapper5_chrBankSize == 1)
                {
                    this.Map.Switch4kChrRom(data, 0);
                }
            }
            else if (address == 0x5124)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 4);
                }
            }
            else if (address == 0x5125)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 5);
                }
                else if (this.mapper5_chrBankSize == 2)
                {
                    this.Map.Switch2kChrRom(data, 2);
                }
            }
            else if (address == 0x5126)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 6);
                }
            }
            else if (address == 0x5127)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 7);
                }
                else if (this.mapper5_chrBankSize == 2)
                {
                    this.Map.Switch2kChrRom(data, 3);
                }
                else if (this.mapper5_chrBankSize == 1)
                {
                    this.Map.Switch4kChrRom(data, 1);
                }
                else if (this.mapper5_chrBankSize == 0)
                {
                    this.Map.Switch8kChrRom(data);
                }
            }
            else if (address == 0x5128)
            {
                this.Map.Switch1kChrRom(data, 0);
                this.Map.Switch1kChrRom(data, 4);
            }
            else if (address == 0x5129)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 1);
                    this.Map.Switch1kChrRom(data, 5);
                }
                else if (this.mapper5_chrBankSize == 2)
                {
                    this.Map.Switch2kChrRom(data, 0);
                    this.Map.Switch2kChrRom(data, 2);
                }
            }
            else if (address == 0x512a)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 2);
                    this.Map.Switch1kChrRom(data, 6);
                }
            }
            else if (address == 0x512b)
            {
                if (this.mapper5_chrBankSize == 3)
                {
                    this.Map.Switch1kChrRom(data, 3);
                    this.Map.Switch1kChrRom(data, 7);
                }
                else if (this.mapper5_chrBankSize == 2)
                {
                    this.Map.Switch2kChrRom(data, 1);
                    this.Map.Switch2kChrRom(data, 3);
                }
                else if (this.mapper5_chrBankSize == 1)
                {
                    this.Map.Switch4kChrRom(data, 0);
                    this.Map.Switch4kChrRom(data, 1);
                }
                else if (this.mapper5_chrBankSize == 0)
                {
                    this.Map.Switch8kChrRom(data);
                }
            }
            else if (address == 0x5203)
            {
                this.IRQCounter = data;
            }
            else if (address == 0x5204)
            {
                this.IQREnabled = (data & 0x80) == 0x80;
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch8kPrgRom((this.Map.Cartridge.PRG_PAGES * 4) - 2, 0);
            this.Map.Switch8kPrgRom((this.Map.Cartridge.PRG_PAGES * 4) - 2, 1);
            this.Map.Switch8kPrgRom((this.Map.Cartridge.PRG_PAGES * 4) - 2, 2);
            this.Map.Switch8kPrgRom((this.Map.Cartridge.PRG_PAGES * 4) - 2, 3);
            this.mapper5_splitIrqEnabled = false;
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 5 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
            if (this.IQREnabled)
            {
                this.IRQCounter--;
                if (this.IRQCounter <= 0)
                {
                    this.Map.NES.CPU.IRQNextTime = true;
                    this.IQREnabled = false;
                }
            }
        }
        public void TickCycleTimer()
        {
        }
        public void SoftReset()
        { }

        public bool WriteUnder8000
        { get { return true; } }
        public bool WriteUnder6000
        { get { return true; } }
    }
}
