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
    /*IQR timer fixed at 28/3/2010 by AHD*/
    class Mapper16 : IMapper
    {
        MAP Map;
        public short timer_irq_counter_16 = 0;
        public short timer_irq_Latch_16 = 0;
        public bool timer_irq_enabled;
        public Mapper16(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF)
            {
                case 0: this.Map.Switch1kChrRom(data, 0); break;
                case 1: this.Map.Switch1kChrRom(data, 1); break;
                case 2: this.Map.Switch1kChrRom(data, 2); break;
                case 3: this.Map.Switch1kChrRom(data, 3); break;
                case 4: this.Map.Switch1kChrRom(data, 4); break;
                case 5: this.Map.Switch1kChrRom(data, 5); break;
                case 6: this.Map.Switch1kChrRom(data, 6); break;
                case 7: this.Map.Switch1kChrRom(data, 7); break;
                case 8: this.Map.Switch16kPrgRom(data * 4, 0); break;
                case 9: switch (data & 0x3)
                    {
                        case 0:
                            this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                            break;
                        case 1:
                            this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;
                            break;
                        case 2:
                            this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                            this.Map.Cartridge.MirroringBase = 0x2000;
                            break;
                        case 3:
                            this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                            this.Map.Cartridge.MirroringBase = 0x2400;
                            break;
                    } break;
                case 0xA:
                    this.timer_irq_enabled = ((data & 0x1) != 0);
                    this.timer_irq_counter_16 = this.timer_irq_Latch_16;
                    break;
                case 0xB:
                    this.timer_irq_Latch_16 = (short)((this.timer_irq_Latch_16 & 0xFF00) | data);
                    break;
                case 0xC:
                    this.timer_irq_Latch_16 = (short)((data << 8) | (this.timer_irq_Latch_16 & 0x00FF));
                    break;
                case 0xD: break;//
            }
        }
        public void SetUpMapperDefaults()
        {
            this.timer_irq_enabled = false;
            this.Map.Switch16kPrgRom(0, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 16 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer()
        {
            if (this.timer_irq_enabled)
            {
                if (this.timer_irq_counter_16 > 0)
                    this.timer_irq_counter_16 -= (short)(this.Map.NES.CPU.CycleCounter - 1);
                else
                {
                    this.Map.NES.CPU.IRQNextTime = true;
                    this.timer_irq_enabled = false;
                }
            }
        }
        public void SoftReset()
        { }
        public bool WriteUnder8000
        { get { return true; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
