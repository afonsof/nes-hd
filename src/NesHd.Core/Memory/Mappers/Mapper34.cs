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
    class Mapper34 : IMapper
    {
        MAP Map;
        public Mapper34(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x7ffd)
            {
                this.Map.Switch32kPrgRom(data * 8);
            }
            else if (address == 0x7ffe)
            {
                this.Map.Switch4kChrRom(data * 4, 0);
            }
            else if (address == 0x7fff)
            {
                this.Map.Switch4kChrRom(data * 4, 1);
            }
            else if (address >= 0x8000)
            {
                this.Map.Switch32kPrgRom(data * 8);
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch32kPrgRom(0);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this,"Mapper 34 setup done", DebugStatus.Cool);
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
        { get { return true; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
