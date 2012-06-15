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
    class Mapper18 : IMapper
    {
        MAP Map;
        public short Mapper18_Timer = 0;
        public short Mapper18_latch = 0;
        public byte mapper18_control = 0;
        public int Mapper18_IRQWidth = 0;
        public bool timer_irq_enabled;
        public byte[] x = new byte[22];
        public Mapper18(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000: this.x[0] = 0; this.x[0] = (byte)((data & 0x0f)); break;
                case 0x8001: this.x[1] = 0; this.x[1] = (byte)(((data & 0x0f) << 4) | this.x[0]); this.Map.Switch8kPrgRom((this.x[1]) * 2, 0); break;
                case 0x8002: this.x[2] = 0; this.x[2] = (byte)((data & 0x0f)); break;
                case 0x8003: this.x[3] = 0; this.x[3] = (byte)(((data & 0x0f) << 4) | this.x[2]); this.Map.Switch8kPrgRom((this.x[3]) * 2, 1); break;
                case 0x9000: this.x[4] = 0; this.x[4] = (byte)((data & 0x0f)); break;
                case 0x9001: this.x[5] = 0; this.x[5] = (byte)(((data & 0x0f) << 4) | this.x[4]); this.Map.Switch8kPrgRom((this.x[5]) * 2, 2); break;
                case 0x9002:
                    this.Map.Cartridge.IsSaveRam = ((data & 0x1) != 0);
                    this.Map.IsSRAMReadOnly = !this.Map.Cartridge.IsSaveRam;
                    break;
                case 0xA000: this.x[3] &= 0xf0; this.x[3] |= (byte)((data & 0x0f)); break;
                case 0xA001: this.x[3] &= 0x0f; this.x[3] |= (byte)((data & 0x0f) << 4); this.Map.Switch1kChrRom((this.x[3]), 0); break;
                case 0xA002: this.x[4] &= 0xf0; this.x[4] |= (byte)((data & 0x0f)); break;
                case 0xA003: this.x[4] &= 0x0f; this.x[4] |= (byte)((data & 0x0f) << 4); this.Map.Switch1kChrRom((this.x[4]), 1); break;
                case 0xB000: this.x[5] &= 0xf0; this.x[5] |= (byte)((data & 0x0f)); break;
                case 0xB001: this.x[5] &= 0x0f; this.x[5] |= (byte)((data & 0x0f) << 4); this.Map.Switch1kChrRom((this.x[5]), 2); break;
                case 0xB002: this.x[6] &= 0xf0; this.x[6] |= (byte)((data & 0x0f)); break;
                case 0xB003: this.x[6] &= 0x0f; this.x[6] |= (byte)((data & 0x0f) << 4); this.Map.Switch1kChrRom((this.x[6]), 3); break;
                case 0xC000: this.x[7] &= 0xf0; this.x[7] |= (byte)((data & 0x0f)); break;
                case 0xC001: this.x[7] &= 0x0f; this.x[7] |= (byte)((data & 0x0f) << 4); this.Map.Switch1kChrRom((this.x[7]), 4); break;
                case 0xC002: this.x[8] &= 0xf0; this.x[8] |= (byte)((data & 0x0f)); break;
                case 0xC003: this.x[8] &= 0x0f; this.x[8] |= (byte)((data & 0x0f) << 4); this.Map.Switch1kChrRom((this.x[8]), 5); break;
                case 0xD000: this.x[9] &= 0xf0; this.x[9] |= (byte)((data & 0x0f)); break;
                case 0xD001: this.x[9] &= 0x0f; this.x[9] |= (byte)((data & 0x0f) << 4); this.Map.Switch1kChrRom((this.x[9]), 6); break;
                case 0xD002: this.x[10] &= 0xf0; this.x[10] |= (byte)((data & 0x0f)); break;
                case 0xD003: this.x[10] &= 0x0f; this.x[10] |= (byte)((data & 0x0f) << 4); this.Map.Switch1kChrRom((this.x[10]), 7); /*x[10] = 0xff;*/ break;
                case 0xE000:
                    this.Mapper18_latch = (short)((this.Mapper18_latch & 0xFFF0) | (data & 0x0f));
                    break;
                case 0xE001:
                    this.Mapper18_latch = (short)((this.Mapper18_latch & 0xFF0F) | ((data & 0x0f) << 4));
                    break;
                case 0xE002:
                    this.Mapper18_latch = (short)((this.Mapper18_latch & 0xF0FF) | ((data & 0x0f) << 8));
                    break;
                case 0xE003:
                    this.Mapper18_latch = (short)((this.Mapper18_latch & 0x0FFF) | ((data & 0x0f) << 12));
                    break;
                case 0xF000:
                    this.timer_irq_enabled = ((data & 0x01) != 0);
                    break;
                case 0xF001:
                    this.timer_irq_enabled = ((data & 0x01) != 0);
                    this.Mapper18_Timer = this.Mapper18_latch;
                    this.Mapper18_IRQWidth = (data & 0x0E);
                    break;
                case 0xF002:
                    int BankMode = (address & 0x3);
                    switch (BankMode)
                    {
                        case 0:
                            this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;
                            break;
                        case 1:
                            this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                            break;
                        case 2:
                        case 3:
                            this.Map.Cartridge.Mirroring = MIRRORING.ONE_SCREEN;
                            this.Map.Cartridge.MirroringBase = 0x2000;
                            break;
                    } break;
            }
            switch (this.Mapper18_IRQWidth)
            {
                case 0://16 bit
                    break;
                case 1://12 bit
                    this.Mapper18_Timer &= 0x0FFF;
                    break;
                case 2:
                case 3://8 bit
                    this.Mapper18_Timer &= 0x00FF;
                    break;
                case 4:
                case 5:
                case 6:
                case 7://4 bit
                    this.Mapper18_Timer &= 0x000F;
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            this.timer_irq_enabled = false;
            this.Map.Switch16kPrgRom(0, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 18 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer()
        {
            if (this.timer_irq_enabled)
            {
                if (this.Mapper18_Timer > 0)
                    this.Mapper18_Timer -= (short)(this.Map.NES.CPU.CycleCounter);
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
