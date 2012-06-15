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
    class Mapper33 : IMapper
    {
        MAP Map;
        public bool type1 = true;
        public byte IRQCounter = 0;
        public bool IRQEabled;
        public Mapper33(MAP Maps)
        {
            this.Map = Maps;
        }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                this.Map.Switch8kPrgRom((data & 0x1F) * 2, 0);
                if (this.type1)
                {
                    if ((data & 0x40) == 0x40)
                    {
                        this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;

                    }
                    else
                    {
                        this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                    }
                }
            }
            else if (address == 0x8001)
            {
                this.Map.Switch8kPrgRom(data * 2, 1);
            }
            else if (address == 0x8002)
            {
                this.Map.Switch2kChrRom(data * 2, 0);
            }
            else if (address == 0x8003)
            {
                this.Map.Switch2kChrRom(data * 2, 1);
            }
            else if (address == 0xA000)
            {
                this.Map.Switch1kChrRom(data, 4);
            }
            else if (address == 0xA001)
            {
                this.Map.Switch1kChrRom(data, 5);
            }
            else if (address == 0xA002)
            {
                this.Map.Switch1kChrRom(data, 6);
            }
            else if (address == 0xA003)
            {
                this.Map.Switch1kChrRom(data, 7);
            }
            //Type 2 registers
            else if (address == 0xC000)
            {
                this.type1 = false;
                this.IRQCounter = data;
            }
            else if (address == 0xC001)
            {
                this.type1 = false;
                this.IRQCounter = data;
            }
            else if (address == 0xC002)
            {
                this.type1 = false;
                this.IRQEabled = true;
            }
            else if (address == 0xC003)
            {
                this.type1 = false;
                this.IRQEabled = false;
            }
            else if (address == 0xE000)
            {
                this.type1 = false;
                if ((data & 0x40) == 0x40)
                {
                    this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;

                }
                else
                {
                    this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom(0, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            Debug.WriteLine(this, "Mapper 33 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
            if (this.IRQEabled)
            {
                this.IRQCounter++;
                if (this.IRQCounter == 0xFF)
                {
                    this.Map.NES.CPU.IRQNextTime = true;
                }
            }
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
