using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper80 : IMapper
    {
        private readonly Map _map;

        public Mapper80(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address == 0x7EF0)
            {
                _map.Switch2KChrRom(((data >> 1) & 0x3F)*2, 0);
            }
            else if (address == 0x7EF1)
            {
                _map.Switch2KChrRom(((data >> 1) & 0x3F)*2, 1);
            }
            else if (address == 0x7EF2)
                _map.Switch1KChrRom(data, 4);
            else if (address == 0x7EF3)
                _map.Switch1KChrRom(data, 5);
            else if (address == 0x7EF4)
                _map.Switch1KChrRom(data, 6);
            else if (address == 0x7EF5)
                _map.Switch1KChrRom(data, 7);
            else if (address == 0x7EF6)
            {
                if ((address & 0x1) == 0)
                    _map.Cartridge.Mirroring = Mirroring.Vertical;
                else
                    _map.Cartridge.Mirroring = Mirroring.Horizontal;
            }
            else if (address == 0x7EF8)
            {
                if (data == 0xA3)
                    _map.IsSRamReadOnly = true;
                else if (data == 0xFF)
                    _map.IsSRamReadOnly = false;
            }
            else if (address == 0x7EFA)
                _map.Switch8KPrgRom(data*2, 0);
            else if (address == 0x7EFC)
                _map.Switch8KPrgRom(data*2, 1);
            else if (address == 0x7EFE)
                _map.Switch8KPrgRom(data*2, 2);
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 80 setup done.", DebugStatus.Cool);
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