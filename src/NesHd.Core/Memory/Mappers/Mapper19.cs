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
    class Mapper19 : IMapper
    {
        MAP Map;
        public bool VROMRAMfor0000 = false;
        public bool VROMRAMfor1000 = false;
        public short irq_counter = 0;
        public bool IRQEnabled = false;
        public Mapper19(MAP map)
        { this.Map = map; }
        public void Write(ushort address, byte data)
        {
            /*Pattern Table Control*/
            if (address >= 0x8000 & address <= 0x87FF)
            {
                if (this.VROMRAMfor0000)
                { this.Map.Switch1kChrRom(data, 0); }
                else
                {
                    if (data <= 0xDF)
                    { this.Map.Switch1kChrRom(data, 0); }
                }
            }
            else if (address >= 0x8800 & address <= 0x8FFF)
                this.Map.Switch1kChrRom(data, 1);
            else if (address >= 0x9000 & address <= 0x97FF)
                this.Map.Switch1kChrRom(data, 2);
            else if (address >= 0x9800 & address <= 0x9FFF)
                this.Map.Switch1kChrRom(data, 3);
            else if (address >= 0xA000 & address <= 0xA7FF)
            {
                if (this.VROMRAMfor1000)
                {
                    this.Map.Switch1kChrRom(data, 4);
                }
                else
                {
                    if (data <= 0xDF)
                    { this.Map.Switch1kChrRom(data, 4); }
                }
            }
            else if (address >= 0xA800 & address <= 0xAFFF)
                this.Map.Switch1kChrRom(data, 5);
            else if (address >= 0xB000 & address <= 0xB7FF)
                this.Map.Switch1kChrRom(data, 6);
            else if (address >= 0xB800 & address <= 0xBFFF)
                this.Map.Switch1kChrRom(data, 7);
            else if (address >= 0xB800 & address <= 0xBFFF)
                this.Map.Switch1kChrRom(data, 7);
            /*Name Table Control*/
            else if (address >= 0xC000 & address <= 0xC7FF)
            {
                if (data <= 0xDF)
                {
                    this.Map.Switch1kVROMToVRAM(data, 0);
                }
            }
            else if (address >= 0xC800 & address <= 0xCFFF)
            {
                if (data <= 0xDF)
                {
                    this.Map.Switch1kVROMToVRAM(data, 1);
                }
            }
            else if (address >= 0xD000 & address <= 0xD7FF)
            {
                if (data <= 0xDF)
                {
                    this.Map.Switch1kVROMToVRAM(data, 2);
                }
            }
            else if (address >= 0xD800 & address <= 0xDFFF)
            {
                if (data <= 0xDF)
                {
                    this.Map.Switch1kVROMToVRAM(data, 3);
                }
            }
            /*CPU Memory Control*/
            else if (address >= 0xE000 & address <= 0xE7FF)
            {
                this.Map.Switch8kPrgRom((data & 0x3F) * 2, 0);
            }
            else if (address >= 0xE800 & address <= 0xEFFF)
            {
                this.Map.Switch8kPrgRom((data & 0x3F) * 2, 1);
                this.VROMRAMfor0000 = (data & 0x40) == 0x40;
                this.VROMRAMfor1000 = (data & 0x80) == 0x80;
            }
            else if (address >= 0xF000 & address <= 0xF7FF)
            {
                this.Map.Switch8kPrgRom((data & 0x3F) * 2, 2);
            }
            /*IRQ Control*/
            else if (address >= 0x5000 & address <= 0x57FF)
            {
                this.irq_counter = (short)((this.irq_counter & 0xFF00) | data);
            }
            else if (address >= 0x5800 & address <= 0x5FFF)
            {
                this.irq_counter = (short)(((data & 0x7F) << 8) | (this.irq_counter & 0x00FF));
                this.IRQEnabled = (data & 0x80) == 0x80;
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom(0, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 19 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer()
        {
            if (this.IRQEnabled)
            {
                this.irq_counter += (short)(this.Map.NES.CPU.CycleCounter);
                if (this.irq_counter >= 0x7FFF)
                {
                    this.Map.NES.CPU.IRQNextTime = true;
                    this.irq_counter = 0;
                }
            }
        }
        public void SoftReset()
        {

        }
        public bool WriteUnder8000
        {
            get { return true; }
        }
        public bool WriteUnder6000
        {
            get { return true; }
        }
    }
}
