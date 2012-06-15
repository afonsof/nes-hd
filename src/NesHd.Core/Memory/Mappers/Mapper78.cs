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
    class Mapper78 : IMapper
    {
        MAP _Map;
        public Mapper78(MAP Map)
        {
            this._Map = Map;
        }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                this._Map.Switch16kPrgRom((data & 0x7) * 4, 0);
                this._Map.Switch8kChrRom(((data & 0xF0) >> 4) * 8);
                this._Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                if ((address & 0xFE00) != 0xFE00)
                {
                    if ((data & 0x8) != 0)
                        this._Map.Cartridge.MirroringBase = 0x2000;
                    else
                        this._Map.Cartridge.MirroringBase = 0x2400;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            this._Map.Switch16kPrgRom(0, 0);
            this._Map.Switch16kPrgRom((this._Map.Cartridge.CHR_PAGES - 1) * 4, 1);
            this._Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 78 setup OK", DebugStatus.Cool);
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
