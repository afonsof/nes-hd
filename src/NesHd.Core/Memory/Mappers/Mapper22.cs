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
    class Mapper22 : IMapper
    {
        MAP Map;
        public Mapper22(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                this.Map.Switch8kPrgRom(data * 2, 0);
            }
            else if (address == 0x9000)
            {
                switch (data & 0x3)
                {
                    case (0): this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                        break;
                    case (1): this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                        break;
                    case (2): this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                        this.Map.Cartridge.MirroringBase = 0x2400; break;
                    case (3): this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                        this.Map.Cartridge.MirroringBase = 0x2000; break;
                }
            }
            else if (address == 0xA000)
            {
                this.Map.Switch8kPrgRom(data * 2, 1);
            }
            else if (address == 0xB000)
            {
                this.Map.Switch1kChrRom((data >> 1), 0);
            }
            else if (address == 0xB001)
            {
                this.Map.Switch1kChrRom((data >> 1), 1);
            }
            else if (address == 0xC000)
            {
                this.Map.Switch1kChrRom((data >> 1), 2);
            }
            else if (address == 0xC001)
            {
                this.Map.Switch1kChrRom((data >> 1), 3);
            }
            else if (address == 0xD000)
            {
                this.Map.Switch1kChrRom((data >> 1), 4);
            }
            else if (address == 0xD001)
            {
                this.Map.Switch1kChrRom((data >> 1), 5);
            }
            else if (address == 0xE000)
            {
                this.Map.Switch1kChrRom((data >> 1), 6);
            }
            else if (address == 0xE001)
            {
                this.Map.Switch1kChrRom((data >> 1), 7);
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this,"Mapper 22 setup done", DebugStatus.Cool);
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
