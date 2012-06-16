using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper71 : IMapper
    {
        private readonly Map _map;

        public Mapper71(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0xF000:
                case 0xE000:
                case 0xD000:
                case 0xC000:
                    _map.Switch16KPrgRom(data*4, 0);
                    break;
                case 0x9000:
                    _map.Cartridge.Mirroring = Mirroring.OneScreen;
                    if ((data & 0x10) != 0)
                    {
                        _map.Cartridge.MirroringBase = 0x2000;
                    }
                    else
                    {
                        _map.Cartridge.MirroringBase = 0x2400;
                    }
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 71 setup done", DebugStatus.Cool);
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