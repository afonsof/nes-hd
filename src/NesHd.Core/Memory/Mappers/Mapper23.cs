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
    class Mapper23:IMapper
    {
        MAP Map;
        public bool PRGMode = true;
        public byte[] REG = new byte[8];
        public int irq_latch = 0;
        public int irq_enable = 0;
        public int irq_counter = 0;
        public int irq_clock = 0;
        public Mapper23(MAP MAP)
        { this.Map = MAP; }       
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                case 0x8004:
                case 0x8008:
                case 0x800C:
                    if (this.PRGMode)
                        this.Map.Switch8kPrgRom(data * 2, 0);
                    else
                        this.Map.Switch8kPrgRom(data * 2, 2);
                    break;
                case 0x9000:
                    switch (data & 0x3)
                    {
                        case 0: this.Map.Cartridge.Mirroring = MIRRORING.VERTICAL; break;
                        case 1: this.Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL; break;
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
                case 0x9008:
                    this.Map.IsSRAMReadOnly = (data & 0x1) == 0;
                    this.PRGMode = (data & 0x2) == 0x2;
                    break;
                case 0xA000:
                case 0xA004:
                case 0xA008:
                case 0xA00C:
                    this.Map.Switch8kPrgRom(data * 2, 1);
                    break;

                case 0xB000:
                    this.REG[0] = (byte)((this.REG[0] & 0xF0) | (data & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[0], 0);
                    break;
                case 0xB001:
                case 0xB004:
                    this.REG[0] = (byte)(((data & 0x0F) << 4) | (this.REG[0] & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[0], 0);
                    break;

                case 0xB002:
                case 0xB008:
                    this.REG[1] = (byte)((this.REG[1] & 0xF0) | (data & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[1], 1);
                    break;
                case 0xB003:
                case 0xB00C:
                    this.REG[1] = (byte)(((data & 0x0F) << 4) | (this.REG[1] & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[1], 1);
                    break;

                case 0xC000:
                    this.REG[2] = (byte)((this.REG[2] & 0xF0) | (data & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[2], 2);
                    break;
                case 0xC001:
                case 0xC004:
                    this.REG[2] = (byte)(((data & 0x0F) << 4) | (this.REG[2] & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[2], 2);
                    break;

                case 0xC002:
                case 0xC008:
                    this.REG[3] = (byte)((this.REG[3] & 0xF0) | (data & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[3], 3);
                    break;
                case 0xC003:
                case 0xC00C:
                    this.REG[3] = (byte)(((data & 0x0F) << 4) | (this.REG[3] & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[3], 3);
                    break;

                case 0xD000:
                    this.REG[4] = (byte)((this.REG[4] & 0xF0) | (data & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[4], 4);
                    break;
                case 0xD001:
                case 0xD004:
                    this.REG[4] = (byte)(((data & 0x0F) << 4) | (this.REG[4] & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[4], 4);
                    break;

                case 0xD002:
                case 0xD008:
                    this.REG[5] = (byte)((this.REG[5] & 0xF0) | (data & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[5], 5);
                    break;
                case 0xD003:
                case 0xD00C:
                    this.REG[5] = (byte)(((data & 0x0F) << 4) | (this.REG[5] & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[5], 5);
                    break;

                case 0xE000:
                    this.REG[6] = (byte)((this.REG[6] & 0xF0) | (data & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[6], 6);
                    break;
                case 0xE001:
                case 0xE004:
                    this.REG[6] = (byte)(((data & 0x0F) << 4) | (this.REG[6] & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[6], 6);
                    break;

                case 0xE002:
                case 0xE008:
                    this.REG[7] = (byte)((this.REG[7] & 0xF0) | (data & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[7], 7);
                    break;
                case 0xE003:
                case 0xE00C:
                    this.REG[7] = (byte)(((data & 0x0F) << 4) | (this.REG[7] & 0x0F));
                    this.Map.Switch1kChrRom(this.REG[7], 7);
                    break;

                case 0xF000:
                    this.irq_latch = (this.irq_latch & 0xF0) | (data & 0x0F);
                    this.Map.NES.CPU.IRQNextTime = true;
                    break;
                case 0xF004:
                    this.irq_latch = (this.irq_latch & 0x0F) | ((data & 0x0F) << 4);
                    this.Map.NES.CPU.IRQNextTime = true;
                    break;
                case 0xF008:
                    this.irq_enable = data & 0x03; 
                    this.irq_counter = this.irq_latch;
                    this.irq_clock = 0; 
                    this.Map.NES.CPU.IRQNextTime = true;
                    break;
                case 0xF00C:
                    this.irq_enable = (this.irq_enable & 0x01) * 3;
                    this.Map.NES.CPU.IRQNextTime = true;
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch16kPrgRom(0, 0);
            this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            //Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 23 setup OK.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer()
        {
            if ((this.irq_enable & 0x02)!=0)
            {
                this.irq_clock += this.Map.NES.CPU.CycleCounter * 3;
                while (this.irq_clock >= 341)
                {
                    this.irq_clock -= 341;
                    this.irq_counter++;
                    if (this.irq_counter == 0)
                    {
                        this.irq_counter = this.irq_latch;
                        this.Map.NES.CPU.IRQNextTime = true;
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
