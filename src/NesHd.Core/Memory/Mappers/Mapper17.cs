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
    class Mapper17 : IMapper
    {
        MAP Map;
        public bool IRQEnabled = false;
        public int irq_counter = 0;
        public Mapper17(MAP map)
        { this.Map = map; }
        public void Write(ushort address, byte data)
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
                                this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;
                            else
                                this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                            break;
                    }
                    break;
                case 0x4501: this.IRQEnabled = false; break;
                case 0x4502: this.irq_counter = (short)((this.irq_counter & 0xFF00) | data); break;
                case 0x4503: this.irq_counter = (short)((data << 8) | (this.irq_counter & 0x00FF)); this.IRQEnabled = true; break;
                case 0x4504: this.Map.Switch8kPrgRom(data * 2, 0); break;
                case 0x4505: this.Map.Switch8kPrgRom(data * 2, 1); break;
                case 0x4506: this.Map.Switch8kPrgRom(data * 2, 2); break;
                case 0x4507: this.Map.Switch8kPrgRom(data * 2, 3); break;
                case 0x4510: this.Map.Switch1kChrRom(data, 0); break;
                case 0x4511: this.Map.Switch1kChrRom(data, 1); break;
                case 0x4512: this.Map.Switch1kChrRom(data, 2); break;
                case 0x4513: this.Map.Switch1kChrRom(data, 3); break;
                case 0x4514: this.Map.Switch1kChrRom(data, 4); break;
                case 0x4515: this.Map.Switch1kChrRom(data, 5); break;
                case 0x4516: this.Map.Switch1kChrRom(data, 6); break;
                case 0x4517: this.Map.Switch1kChrRom(data, 7); break;
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom(0, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 6 setup done.", DebugStatus.Cool);
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
