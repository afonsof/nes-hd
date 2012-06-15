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
    class Mapper80 : IMapper
    {
        MAP Map;
        public Mapper80(MAP map)
        { this.Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x7EF0)
            {
                this.Map.Switch2kChrRom(((data >> 1) & 0x3F) * 2, 0);
            }
            else if (address == 0x7EF1)
            {
                this.Map.Switch2kChrRom(((data >> 1) & 0x3F) * 2, 1);
            }
            else if (address == 0x7EF2)
                this.Map.Switch1kChrRom(data, 4);
            else if (address == 0x7EF3)
                this.Map.Switch1kChrRom(data, 5);
            else if (address == 0x7EF4)
                this.Map.Switch1kChrRom(data, 6);
            else if (address == 0x7EF5)
                this.Map.Switch1kChrRom(data, 7);
            else if (address == 0x7EF6)
            {
                if ((address & 0x1) == 0)
                    this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                else
                    this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;
            }
            else if (address == 0x7EF8)
            {
                if (data == 0xA3)
                    this.Map.IsSRAMReadOnly = true;
                else if (data == 0xFF)
                    this.Map.IsSRAMReadOnly = false;
            }
            else if (address == 0x7EFA)
                this.Map.Switch8kPrgRom(data * 2, 0);
            else if (address == 0x7EFC)
                this.Map.Switch8kPrgRom(data * 2, 1);
            else if (address == 0x7EFE)
                this.Map.Switch8kPrgRom(data * 2, 2);
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 80 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer()
        {
        }
        public void SoftReset()
        {
        }
        public bool WriteUnder8000
        { get { return true; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
