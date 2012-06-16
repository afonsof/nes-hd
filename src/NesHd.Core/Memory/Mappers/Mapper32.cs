using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper32 : IMapper
    {
        private readonly Map _map;
        public int mapper32SwitchingMode;

        public Mapper32(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x9FFF:
                    _map.Cartridge.Mirroring = ((data & 0x01) == 0) ? Mirroring.Vertical : Mirroring.Horizontal;
                    mapper32SwitchingMode = ((data & 0x02) == 0) ? 0 : 1;
                    break;
                case 0x8FFF:
                    if (mapper32SwitchingMode == 0)
                    {
                        _map.Switch8KPrgRom(data*2, 0);
                    }
                    else
                    {
                        _map.Switch8KPrgRom(data*2, 2);
                    }
                    break;
                case 0xAFFF:
                    _map.Switch8KPrgRom(data*2, 1);
                    break;
                case 0xBFF0:
                    _map.Switch1KChrRom(data, 0);
                    break;
                case 0xBFF1:
                    _map.Switch1KChrRom(data, 1);
                    break;
                case 0xBFF2:
                    _map.Switch1KChrRom(data, 2);
                    break;
                case 0xBFF3:
                    _map.Switch1KChrRom(data, 3);
                    break;
                case 0xBFF4:
                    _map.Switch1KChrRom(data, 4);
                    break;
                case 0xBFF5:
                    _map.Switch1KChrRom(data, 5);
                    break;
                case 0xBFF6:
                    _map.Switch1KChrRom(data, 6);
                    break;
                case 0xBFF7:
                    _map.Switch1KChrRom(data, 7);
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom(1*4, 0);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 32 setup done.", DebugStatus.Cool);
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