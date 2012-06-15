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
    class Mapper32 : IMapper
    {
        MAP Map;
        public int mapper32SwitchingMode = 0;
        public Mapper32(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x9FFF: this.Map.Cartridge.Mirroring = ((data & 0x01) == 0) ? MIRRORING.VERTICAL : MIRRORING.HORIZONTAL;
                    this.mapper32SwitchingMode = ((data & 0x02) == 0) ? 0 : 1;
                    break;
                case 0x8FFF:
                    if (this.mapper32SwitchingMode == 0)
                    {
                        this.Map.Switch8kPrgRom(data * 2, 0);
                    }
                    else
                    {
                        this.Map.Switch8kPrgRom(data * 2, 2);
                    }
                    break;
                case 0xAFFF: this.Map.Switch8kPrgRom(data * 2, 1); break;
                case 0xBFF0: this.Map.Switch1kChrRom(data, 0); break;
                case 0xBFF1: this.Map.Switch1kChrRom(data, 1); break;
                case 0xBFF2: this.Map.Switch1kChrRom(data, 2); break;
                case 0xBFF3: this.Map.Switch1kChrRom(data, 3); break;
                case 0xBFF4: this.Map.Switch1kChrRom(data, 4); break;
                case 0xBFF5: this.Map.Switch1kChrRom(data, 5); break;
                case 0xBFF6: this.Map.Switch1kChrRom(data, 6); break;
                case 0xBFF7: this.Map.Switch1kChrRom(data, 7); break;
            }

        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom(1 * 4, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 32 setup done.", DebugStatus.Cool);
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
