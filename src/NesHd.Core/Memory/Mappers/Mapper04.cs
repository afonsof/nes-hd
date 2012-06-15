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
    class Mapper04 : IMapper
    {
        MAP _Map;
        public int mapper4_commandNumber;
        public int mapper4_prgAddressSelect;
        public int mapper4_chrAddressSelect;
        public bool timer_irq_enabled;
        public uint timer_irq_count, timer_irq_reload;
        public Mapper04(MAP map)
        { this._Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                this.mapper4_commandNumber = data & 0x7;
                this.mapper4_prgAddressSelect = data & 0x40;
                this.mapper4_chrAddressSelect = data & 0x80;
            }
            else if (address == 0x8001)
            {
                if (this.mapper4_commandNumber == 0)
                {

                    data = (byte)(data - (data % 2));
                    if (this.mapper4_chrAddressSelect == 0)
                        this._Map.Switch2kChrRom(data, 0);
                    else
                        this._Map.Switch2kChrRom(data, 2);
                }
                else if (this.mapper4_commandNumber == 1)
                {

                    data = (byte)(data - (data % 2));
                    if (this.mapper4_chrAddressSelect == 0)
                    {
                        this._Map.Switch2kChrRom(data, 1);
                    }
                    else
                    {
                        this._Map.Switch2kChrRom(data, 3);
                    }
                }
                else if (this.mapper4_commandNumber == 2)
                {

                    data = (byte)(data & (this._Map.Cartridge.CHR_PAGES * 8 - 1));
                    if (this.mapper4_chrAddressSelect == 0)
                    {
                        this._Map.Switch1kChrRom(data, 4);
                    }
                    else
                    {
                        this._Map.Switch1kChrRom(data, 0);
                    }
                }
                else if (this.mapper4_commandNumber == 3)
                {

                    if (this.mapper4_chrAddressSelect == 0)
                    {
                        this._Map.Switch1kChrRom(data, 5);
                    }
                    else
                    {
                        this._Map.Switch1kChrRom(data, 1);
                    }
                }
                else if (this.mapper4_commandNumber == 4)
                {

                    if (this.mapper4_chrAddressSelect == 0)
                    {
                        this._Map.Switch1kChrRom(data, 6);
                    }
                    else
                    {
                        this._Map.Switch1kChrRom(data, 2);
                    }
                }
                else if (this.mapper4_commandNumber == 5)
                {

                    if (this.mapper4_chrAddressSelect == 0)
                    {
                        this._Map.Switch1kChrRom(data, 7);
                    }
                    else
                    {
                        this._Map.Switch1kChrRom(data, 3);
                    }
                }
                else if (this.mapper4_commandNumber == 6)
                {
                    if (this.mapper4_prgAddressSelect == 0)
                    {
                        this._Map.Switch8kPrgRom(data * 2, 0);
                    }
                    else
                    {
                        this._Map.Switch8kPrgRom(data * 2, 2);
                    }
                }
                else if (this.mapper4_commandNumber == 7)
                {

                    this._Map.Switch8kPrgRom(data * 2, 1);
                }

                if (this.mapper4_prgAddressSelect == 0)
                { this._Map.Switch8kPrgRom(((this._Map.Cartridge.PRG_PAGES * 4) - 2) * 2, 2); }
                else
                { this._Map.Switch8kPrgRom(((this._Map.Cartridge.PRG_PAGES * 4) - 2) * 2, 0); }
                this._Map.Switch8kPrgRom(((this._Map.Cartridge.PRG_PAGES * 4) - 1) * 2, 3);
            }
            else if (address == 0xA000)
            {
                if ((data & 0x1) == 0)
                {
                    this._Map.Cartridge.Mirroring = MIRRORING.VERTICAL;
                }
                else
                {
                    this._Map.Cartridge.Mirroring = MIRRORING.HORIZONTAL;
                }
            }
            else if (address == 0xA001)
            {
                if ((data & 0x80) == 0)
                    this._Map.IsSRAMReadOnly = true;
                else
                    this._Map.IsSRAMReadOnly = false;
            }
            else if (address == 0xC000)
            { this.timer_irq_reload = data; }
            else if (address == 0xC001)
            { this.timer_irq_count = data; }
            else if (address == 0xE000)
            {
                this.timer_irq_enabled = false;
                this.timer_irq_reload = this.timer_irq_count;
            }
            else if (address == 0xE001)
            { this.timer_irq_enabled = true; }
        }
        public void SetUpMapperDefaults()
        {
            this.timer_irq_count = this.timer_irq_reload = 0xff;
            this.mapper4_prgAddressSelect = 0;
            this.mapper4_chrAddressSelect = 0;
            this._Map.Switch16kPrgRom((this._Map.Cartridge.PRG_PAGES - 1) * 4, 1);
            this._Map.Switch8kChrRom(0);
            Debug.WriteLine(this, "Mapper 4 setup done.", DebugStatus.Cool);
        }
        public void TickScanlineTimer()
        {
            if (this.timer_irq_reload == 0)
            {
                if (this.timer_irq_enabled)
                {
                    this._Map.NES.CPU.IRQNextTime = true;
                    this.timer_irq_enabled = false;
                }
                this.timer_irq_reload = this.timer_irq_count;
            }
            this.timer_irq_reload -= 1;
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
