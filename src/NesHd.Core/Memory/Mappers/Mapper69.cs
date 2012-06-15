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
    class Mapper69 : IMapper
    {
        MAP Map;
        public ushort reg = 0;
        public short timer_irq_counter_69 = 0;
        public bool timer_irq_enabled;
        public Mapper69(MAP MAP)
        { this.Map = MAP; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xE000)
            {
                case 0x8000:
                    this.reg = data;
                    break;
                case 0xA000:
                    switch (this.reg & 0x0F)
                    {
                        case 0x00: this.Map.Switch1kChrRom(data, 0); break;
                        case 0x01: this.Map.Switch1kChrRom(data, 1); break;
                        case 0x02: this.Map.Switch1kChrRom(data, 2); break;
                        case 0x03: this.Map.Switch1kChrRom(data, 3); break;
                        case 0x04: this.Map.Switch1kChrRom(data, 4); break;
                        case 0x05: this.Map.Switch1kChrRom(data, 5); break;
                        case 0x06: this.Map.Switch1kChrRom(data, 6); break;
                        case 0x07: this.Map.Switch1kChrRom(data, 7); break;
                        case 0x08:
                            if ((data & 0x40) == 0)
                            {
                                this.Map.Switch8kPrgRom((data & 0x3F) * 2, 0);
                            }
                            break;
                        case 0x09:
                            this.Map.Switch8kPrgRom(data * 2, 0);
                            break;
                        case 0x0A:
                            this.Map.Switch8kPrgRom(data * 2, 1);
                            break;
                        case 0x0B:
                            this.Map.Switch8kPrgRom(data * 2, 2);
                            break;

                        case 0x0C:
                            data &= 0x03;
                            if (data == 0) this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                            if (data == 1) this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;
                            if (data == 2)
                            {
                                this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                                this.Map.Cartridge.MirroringBase = 0x2000;
                            }
                            if (data == 3)
                            {
                                this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                                this.Map.Cartridge.MirroringBase = 0x2400;
                            }
                            break;

                        case 0x0D:
                            if (data == 0)
                                this.timer_irq_enabled = false;
                            if (data == 0x81)
                                this.timer_irq_enabled = true;
                            break;

                        case 0x0E:
                            this.timer_irq_counter_69 = (short)((this.timer_irq_counter_69 & 0xFF00) | data);

                            break;

                        case 0x0F:
                            this.timer_irq_counter_69 = (short)((this.timer_irq_counter_69 & 0x00FF) | (data << 8));
                            break;
                    }
                    break;

                case 0xC000:
                case 0xE000:
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {

            this.Map.Switch8kPrgRom((this.Map.Cartridge.PRG_PAGES * 4) - 2, 0);
            this.Map.Switch8kPrgRom((this.Map.Cartridge.PRG_PAGES * 4) - 2, 1);
            this.Map.Switch8kPrgRom((this.Map.Cartridge.PRG_PAGES * 4) - 2, 2);
            this.Map.Switch8kPrgRom((this.Map.Cartridge.PRG_PAGES * 4) - 2, 3);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 69 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer()
        {
            if (this.timer_irq_enabled)
            {
                if (this.timer_irq_counter_69 > 0)
                    this.timer_irq_counter_69 -= (short)(this.Map.NES.CPU.CycleCounter - 1);
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
        { get { return false; } }
        public bool WriteUnder6000
        { get { return false; } }
    }
}
