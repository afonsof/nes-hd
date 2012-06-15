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
    class Mapper91 : IMapper
    {
        MAP Map;
        public bool IRQEnabled;
        public int IRQCount = 0;
        public Mapper91(MAP map)
        { this.Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x6000)
                this.Map.Switch2kChrRom(data * 2, 0);
            else if (address == 0x6001)
                this.Map.Switch2kChrRom(data * 2, 1);
            else if (address == 0x6002)
                this.Map.Switch2kChrRom(data * 2, 2);
            else if (address == 0x6003)
                this.Map.Switch2kChrRom(data * 2, 3);
            else if (address == 0x7000)
                this.Map.Switch8kPrgRom(data * 2, 0);
            else if (address == 0x7001)
                this.Map.Switch8kPrgRom(data * 2, 1);
            else if (address == 0x7006)
            {
                this.IRQEnabled = false;
                this.IRQCount = 0;
            }
            else if (address == 0x7007)
                this.IRQEnabled = true;
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 91 setup OK.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
            if (this.IRQCount < 8 & this.IRQEnabled)
            {
                this.IRQCount++;
                if (this.IRQCount >= 8)
                {
                    this.Map.NES.CPU.IRQNextTime = true;
                }
            }
        }
        public void TickCycleTimer()
        {

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
            get { return false; }
        }
    }
}
