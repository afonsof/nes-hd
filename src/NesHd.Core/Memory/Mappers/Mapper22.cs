using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper22 : IMapper
    {
        private readonly Map _map;

        public Mapper22(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                _map.Switch8KPrgRom(data*2, 0);
            }
            else if (address == 0x9000)
            {
                switch (data & 0x3)
                {
                    case (0):
                        _map.Cartridge.Mirroring = Mirroring.Vertical;
                        break;
                    case (1):
                        _map.Cartridge.Mirroring = Mirroring.Vertical;
                        break;
                    case (2):
                        _map.Cartridge.Mirroring = Mirroring.OneScreen;
                        _map.Cartridge.MirroringBase = 0x2400;
                        break;
                    case (3):
                        _map.Cartridge.Mirroring = Mirroring.OneScreen;
                        _map.Cartridge.MirroringBase = 0x2000;
                        break;
                }
            }
            else if (address == 0xA000)
            {
                _map.Switch8KPrgRom(data*2, 1);
            }
            else if (address == 0xB000)
            {
                _map.Switch1KChrRom((data >> 1), 0);
            }
            else if (address == 0xB001)
            {
                _map.Switch1KChrRom((data >> 1), 1);
            }
            else if (address == 0xC000)
            {
                _map.Switch1KChrRom((data >> 1), 2);
            }
            else if (address == 0xC001)
            {
                _map.Switch1KChrRom((data >> 1), 3);
            }
            else if (address == 0xD000)
            {
                _map.Switch1KChrRom((data >> 1), 4);
            }
            else if (address == 0xD001)
            {
                _map.Switch1KChrRom((data >> 1), 5);
            }
            else if (address == 0xE000)
            {
                _map.Switch1KChrRom((data >> 1), 6);
            }
            else if (address == 0xE001)
            {
                _map.Switch1KChrRom((data >> 1), 7);
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 22 setup done", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
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