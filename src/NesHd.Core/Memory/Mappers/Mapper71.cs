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
    class Mapper71 : IMapper
    {
        MAP Map;
        public Mapper71(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0xF000:
                case 0xE000:
                case 0xD000:
                case 0xC000: this.Map.Switch16kPrgRom(data * 4, 0); break;
                case 0x9000:
                    this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                    if ((data & 0x10) != 0)
                    {
                        this.Map.Cartridge.MirroringBase = 0x2000;
                    }
                    else
                    {
                        this.Map.Cartridge.MirroringBase = 0x2400;
                    }
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this,"Mapper 71 setup done", DebugStatus.Cool);
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
