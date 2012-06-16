using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper82 : IMapper
    {
        private readonly Map _map;
        private bool _swapped;

        public Mapper82(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x7EF0:
                    _map.Switch2KChrRom((data >> 1)*2, _swapped ? 2 : 0);
                    break;
                case 0x7EF1:
                    _map.Switch2KChrRom((data >> 1)*2, _swapped ? 3 : 1);
                    break;
                case 0x7EF2:
                    _map.Switch1KChrRom(data, _swapped ? 0 : 4);
                    break;
                case 0x7EF3:
                    _map.Switch1KChrRom(data, _swapped ? 1 : 5);
                    break;
                case 0x7EF4:
                    _map.Switch1KChrRom(data, _swapped ? 2 : 6);
                    break;
                case 0x7EF5:
                    _map.Switch1KChrRom(data, _swapped ? 3 : 7);
                    break;
                case 0x7EF6:
                    _swapped = ((data & 0x2) >> 1) != 0;
                    _map.Cartridge.Mirroring = (data & 0x01) == 0 ? Mirroring.Horizontal : Mirroring.Vertical;
                    break;
                case 0x7EFA:
                    _map.Switch8KPrgRom((data >> 2) * 2, 0);
                    break;
                case 0x7EFB:
                    _map.Switch8KPrgRom((data >> 2) * 2, 1);
                    break;
                case 0x7EFC:
                    _map.Switch8KPrgRom((data >> 2) * 2, 2);
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1) * 4, 1);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 2) * 4, 0);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 82 setup done.", DebugStatus.Cool);
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
            get { return true; }
        }

        public bool WriteUnder6000
        {
            get { return false; }
        }

        #endregion
    }
}