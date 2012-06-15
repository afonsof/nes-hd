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
    class Mapper65 : IMapper
    {
        MAP Map;
        public short timer_irq_counter_65 = 0;
        public short timer_irq_Latch_65 = 0;
        public bool timer_irq_enabled;

        public Mapper65(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
                this.Map.Switch8kPrgRom(data * 2, 0);
            else if (address == 0xA000)
                this.Map.Switch8kPrgRom(data * 2, 1);
            else if (address == 0xC000)
                this.Map.Switch8kPrgRom(data * 2, 2);
            else if (address == 0x9003)
                this.timer_irq_enabled = ((data & 0x80) != 0);
            else if (address == 0x9004)
                this.timer_irq_counter_65 = this.timer_irq_Latch_65;
            else if (address == 0x9005)
                this.timer_irq_Latch_65 = (short)((this.timer_irq_Latch_65 & 0x00FF) | (data << 8));
            else if (address == 0x9006)
                this.timer_irq_Latch_65 = (short)((this.timer_irq_Latch_65 & 0xFF00) | (data));
            else if (address == 0xB000)
                this.Map.Switch1kChrRom(data, 0);
            else if (address == 0xB001)
                this.Map.Switch1kChrRom(data, 1);
            else if (address == 0xB002)
                this.Map.Switch1kChrRom(data, 2);
            else if (address == 0xB003)
                this.Map.Switch1kChrRom(data, 3);
            else if (address == 0xB004)
                this.Map.Switch1kChrRom(data, 4);
            else if (address == 0xB005)
                this.Map.Switch1kChrRom(data, 5);
            else if (address == 0xB006)
                this.Map.Switch1kChrRom(data, 6);
            else if (address == 0xB007)
                this.Map.Switch1kChrRom(data, 7);
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom(0, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.CHR_PAGES - 1) * 4, 1);
            Debug.WriteLine(this, "Mapper 65 setup done", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer()
        {
            if (this.timer_irq_enabled)
            {
                this.timer_irq_counter_65 -= (short)(this.Map.NES.CPU.CycleCounter);
                if (this.timer_irq_counter_65 <= 0)
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
