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
    class Mapper08 : IMapper
    {
        MAP Map;
        public bool IRQEnabled = false;
        public int irq_counter = 0;
        public Mapper08(MAP map)
        { this.Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                this.Map.Switch32kPrgRom(((data & 0x30) >> 4) * 8);
                this.Map.Switch8kChrRom((data & 0x3) * 8);
            }
            else
            {
                switch (address)
                {
                    case 0x42FC:
                    case 0x42FE:
                    case 0x42FF:
                        switch (address & 0x1)
                        {
                            case 0:
                                this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                                if ((data & 0x10) != 0)
                                    this.Map.Cartridge.MirroringBase = 0x2400;
                                else
                                    this.Map.Cartridge.MirroringBase = 0x2000;
                                break;
                            case 1:
                                if ((data & 0x10) != 0)
                                    this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                                else
                                    this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;
                                break;
                        }
                        break;
                    case 0x4501: this.IRQEnabled = false; break;
                    case 0x4502: this.irq_counter = (short)((this.irq_counter & 0xFF00) | data); break;
                    case 0x4503: this.irq_counter = (short)((data << 8) | (this.irq_counter & 0x00FF)); this.IRQEnabled = true; break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            //Map.Switch16kPrgRom(7 * 4, 1);
            this.Map.Switch32kPrgRom(0);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 8 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer()
        {
            if (this.IRQEnabled)
            {
                this.irq_counter += (short)(this.Map.NES.CPU.CycleCounter);
                if (this.irq_counter >= 0xFFFF)
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
