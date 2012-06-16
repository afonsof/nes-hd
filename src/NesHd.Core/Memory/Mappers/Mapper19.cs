using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper19 : IMapper
    {
        private readonly Map _map;
        public bool IRQEnabled;
        public bool VROMRAMfor0000;
        public bool VROMRAMfor1000;
        public short irq_counter;

        public Mapper19(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            /*Pattern Table Control*/
            if (address >= 0x8000 & address <= 0x87FF)
            {
                if (VROMRAMfor0000)
                {
                    _map.Switch1KChrRom(data, 0);
                }
                else
                {
                    if (data <= 0xDF)
                    {
                        _map.Switch1KChrRom(data, 0);
                    }
                }
            }
            else if (address >= 0x8800 & address <= 0x8FFF)
                _map.Switch1KChrRom(data, 1);
            else if (address >= 0x9000 & address <= 0x97FF)
                _map.Switch1KChrRom(data, 2);
            else if (address >= 0x9800 & address <= 0x9FFF)
                _map.Switch1KChrRom(data, 3);
            else if (address >= 0xA000 & address <= 0xA7FF)
            {
                if (VROMRAMfor1000)
                {
                    _map.Switch1KChrRom(data, 4);
                }
                else
                {
                    if (data <= 0xDF)
                    {
                        _map.Switch1KChrRom(data, 4);
                    }
                }
            }
            else if (address >= 0xA800 & address <= 0xAFFF)
                _map.Switch1KChrRom(data, 5);
            else if (address >= 0xB000 & address <= 0xB7FF)
                _map.Switch1KChrRom(data, 6);
            else if (address >= 0xB800 & address <= 0xBFFF)
                _map.Switch1KChrRom(data, 7);
            else if (address >= 0xB800 & address <= 0xBFFF)
                _map.Switch1KChrRom(data, 7);
                /*Name Table Control*/
            else if (address >= 0xC000 & address <= 0xC7FF)
            {
                if (data <= 0xDF)
                {
                    _map.Switch1kVRomToVRam(data, 0);
                }
            }
            else if (address >= 0xC800 & address <= 0xCFFF)
            {
                if (data <= 0xDF)
                {
                    _map.Switch1kVRomToVRam(data, 1);
                }
            }
            else if (address >= 0xD000 & address <= 0xD7FF)
            {
                if (data <= 0xDF)
                {
                    _map.Switch1kVRomToVRam(data, 2);
                }
            }
            else if (address >= 0xD800 & address <= 0xDFFF)
            {
                if (data <= 0xDF)
                {
                    _map.Switch1kVRomToVRam(data, 3);
                }
            }
                /*CPU Memory Control*/
            else if (address >= 0xE000 & address <= 0xE7FF)
            {
                _map.Switch8KPrgRom((data & 0x3F)*2, 0);
            }
            else if (address >= 0xE800 & address <= 0xEFFF)
            {
                _map.Switch8KPrgRom((data & 0x3F)*2, 1);
                VROMRAMfor0000 = (data & 0x40) == 0x40;
                VROMRAMfor1000 = (data & 0x80) == 0x80;
            }
            else if (address >= 0xF000 & address <= 0xF7FF)
            {
                _map.Switch8KPrgRom((data & 0x3F)*2, 2);
            }
                /*IRQ Control*/
            else if (address >= 0x5000 & address <= 0x57FF)
            {
                irq_counter = (short) ((irq_counter & 0xFF00) | data);
            }
            else if (address >= 0x5800 & address <= 0x5FFF)
            {
                irq_counter = (short) (((data & 0x7F) << 8) | (irq_counter & 0x00FF));
                IRQEnabled = (data & 0x80) == 0x80;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom(0, 0);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 19 setup done.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
        }

        public void TickCycleTimer()
        {
            if (IRQEnabled)
            {
                irq_counter += (short) (_map.Engine.Cpu.CycleCounter);
                if (irq_counter >= 0x7FFF)
                {
                    _map.Engine.Cpu.IRQNextTime = true;
                    irq_counter = 0;
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

        #endregion
    }
}