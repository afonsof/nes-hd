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
    class Mapper24 : IMapper
    {
        MAP Map;
        public int irq_latch = 0;
        public bool irq_enable = false;
        public int irq_counter = 0;
        public int irq_clock = 0;
        public Mapper24(MAP MAP)
        { 
            this.Map = MAP;
        }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000: this.Map.Switch16kPrgRom(data * 4, 0); break;
                case 0xB003:
                    switch ((data & 0xC) >> 2)
                    {
                        case 0: this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL; break;
                        case 1: this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL; break;
                        case 2:
                            this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                            this.Map.Cartridge.MirroringBase = 0x2000;
                            break;
                        case 3:
                            this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                            this.Map.Cartridge.MirroringBase = 0x2400;
                            break;
                    }
                    break;

                case 0xC000: this.Map.Switch8kPrgRom(data * 2, 2); break;
                case 0xD000: this.Map.Switch1kChrRom(data, 0); break;
                case 0xD001: this.Map.Switch1kChrRom(data, 1); break;
                case 0xD002: this.Map.Switch1kChrRom(data, 2); break;
                case 0xD003: this.Map.Switch1kChrRom(data, 3); break;
                case 0xE000: this.Map.Switch1kChrRom(data, 4); break;
                case 0xE001: this.Map.Switch1kChrRom(data, 5); break;
                case 0xE002: this.Map.Switch1kChrRom(data, 6); break;
                case 0xE003: this.Map.Switch1kChrRom(data, 7); break;

                case 0xF000:
                    this.irq_latch = data;
                    break;
                case 0xF001:
                    this.irq_enable = (data & 0x01) != 0;
                    break;
                case 0xF002:
                    this.irq_counter = this.irq_latch;
                    break;

                //Sound
                //Pulse 1
                case 0x9000:
                    this.Map.NES.APU.VRC6PULSE1.Write9000(data);
                    break;
                case 0x9001:
                    this.Map.NES.APU.VRC6PULSE1.Write9001(data);
                    break;
                case 0x9002:
                    this.Map.NES.APU.VRC6PULSE1.Write9002(data);
                    break;
                //Pulse 2
                case 0xA000:
                    this.Map.NES.APU.VRC6PULSE2.WriteA000(data);
                    break;
                case 0xA001:
                    this.Map.NES.APU.VRC6PULSE2.WriteA001(data);
                    break;
                case 0xA002:
                    this.Map.NES.APU.VRC6PULSE2.WriteA002(data);
                    break;
                //Sawtooth
                case 0xB000:
                    this.Map.NES.APU.VRC6SAWTOOTH.WriteB000(data);
                    break;
                case 0xB001:
                    this.Map.NES.APU.VRC6SAWTOOTH.WriteB001(data);
                    break;
                case 0xB002:
                    this.Map.NES.APU.VRC6SAWTOOTH.WriteB002(data);
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom(0, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            //Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 24 setup OK.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer()
        {
            if (this.irq_enable)
            {
                if ((this.irq_clock += this.Map.NES.CPU.CycleCounter) >= 0x72)
                {
                    this.irq_clock -= 0x72;
                    if (this.irq_counter == 0xFF)
                    {
                        this.irq_counter = this.irq_latch;
                        this.Map.NES.CPU.IRQNextTime = true;
                    }
                    else
                    {
                        this.irq_counter++;
                    }
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
