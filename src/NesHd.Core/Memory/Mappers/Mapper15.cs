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
    class Mapper15 : IMapper
    {
        MAP Map;
        public Mapper15(MAP Maps)
        { this.Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0xFFFF))
            {
                byte X = (byte)(data & 0x3F);
                this.Map.Cartridge.Mirroring = ((data & 0x40) == 0) ? MIRRORING.VERTICAL : MIRRORING.HORIZONTAL;
                byte Y = (byte)(data & 0x80);
                Y >>= 7;
                switch (address & 0x3)
                {
                    case 0://0=32K
                        this.Map.Switch16kPrgRom(X * 4, 0);
                        this.Map.Switch16kPrgRom((X + 1) * 4, 1);
                        break;
                    case 1://1=128K
                        this.Map.Switch16kPrgRom(X * 4, 0);
                        this.Map.Switch16kPrgRom((this.Map.Cartridge.PRG_PAGES - 1) * 4, 1);
                        break;
                    case 2://2=8K
                        this.Map.Switch8kPrgRom(((X * 2) + Y) * 2, 0);
                        this.Map.Switch8kPrgRom(((X * 2) + Y) * 2, 1);
                        this.Map.Switch8kPrgRom(((X * 2) + Y) * 2, 2);
                        this.Map.Switch8kPrgRom(((X * 2) + Y) * 2, 3);
                        break;
                    case 3://3=16K
                        this.Map.Switch16kPrgRom(X * 4, 0);
                        this.Map.Switch16kPrgRom(X * 4, 1);
                        break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            this.Map.Switch32kPrgRom(0);
            this.Map.Switch8kChrRom(0);
            Debug.WriteLine(this,"Mapper 15 setup done.", DebugStatus.Cool);
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
