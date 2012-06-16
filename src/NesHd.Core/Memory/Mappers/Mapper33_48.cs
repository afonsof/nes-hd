using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper33 : IMapper
    {
        private readonly Map _map;
        public byte IRQCounter;
        public bool IRQEabled;
        public bool type1 = true;

        public Mapper33(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                _map.Switch8KPrgRom((data & 0x1F)*2, 0);
                if (type1)
                {
                    if ((data & 0x40) == 0x40)
                    {
                        _map.Cartridge.Mirroring = Mirroring.Horizontal;
                    }
                    else
                    {
                        _map.Cartridge.Mirroring = Mirroring.Vertical;
                    }
                }
            }
            else if (address == 0x8001)
            {
                _map.Switch8KPrgRom(data*2, 1);
            }
            else if (address == 0x8002)
            {
                _map.Switch2KChrRom(data*2, 0);
            }
            else if (address == 0x8003)
            {
                _map.Switch2KChrRom(data*2, 1);
            }
            else if (address == 0xA000)
            {
                _map.Switch1KChrRom(data, 4);
            }
            else if (address == 0xA001)
            {
                _map.Switch1KChrRom(data, 5);
            }
            else if (address == 0xA002)
            {
                _map.Switch1KChrRom(data, 6);
            }
            else if (address == 0xA003)
            {
                _map.Switch1KChrRom(data, 7);
            }
                //Type 2 registers
            else if (address == 0xC000)
            {
                type1 = false;
                IRQCounter = data;
            }
            else if (address == 0xC001)
            {
                type1 = false;
                IRQCounter = data;
            }
            else if (address == 0xC002)
            {
                type1 = false;
                IRQEabled = true;
            }
            else if (address == 0xC003)
            {
                type1 = false;
                IRQEabled = false;
            }
            else if (address == 0xE000)
            {
                type1 = false;
                if ((data & 0x40) == 0x40)
                {
                    _map.Cartridge.Mirroring = Mirroring.Horizontal;
                }
                else
                {
                    _map.Cartridge.Mirroring = Mirroring.Vertical;
                }
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom(0, 0);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            Debug.WriteLine(this, "Mapper 33 setup done.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
            if (IRQEabled)
            {
                IRQCounter++;
                if (IRQCounter == 0xFF)
                {
                    _map.Engine.Cpu.IRQNextTime = true;
                }
            }
        }

        public void TickCycleTimer()
        {
        }

        public void SoftReset()
        {
        }

        public bool WriteUnder8000
        {
            get { return false; }
        }

        public bool WriteUnder6000
        {
            get { return false; }
        }

        #endregion
    }
}