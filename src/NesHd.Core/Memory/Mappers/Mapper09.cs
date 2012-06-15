﻿/*
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
    class Mapper09 : IMapper
    {
        MAP Map;
        public byte latch1, latch2;
        public int latch1data1, latch1data2;
        public int latch2data1, latch2data2;
        public Mapper09(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if ((address >= 0xa000) && (address <= 0xafff))
            {
                this.Map.Switch8kPrgRom(data * 2, 0);
            }
            else if ((address >= 0xB000) && (address <= 0xCFFF))
            {
                this.Map.Switch4kChrRom(data * 4, 0);
            }
            else if ((address >= 0xD000) && (address <= 0xDFFF))
            {
                this.latch1data1 = data * 4;
            }
            else if ((address >= 0xE000) && (address <= 0xEFFF))
            {
                this.latch1data2 = data * 4;
            }
            else if ((address >= 0xF000) && (address <= 0xFFFF))
            {
                if ((data & 1) == 1)
                {
                    this.Map.Cartridge.Mirroring= MIRRORING.HORIZONTAL;
                }
                else
                {
                    this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            this.latch1 = 0xfe;
            this.Map.Switch32kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4 - 4);
            this.Map.Switch8kPrgRom(0, 0);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this,"Mapper 9 setup done.", DebugStatus.Cool);
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