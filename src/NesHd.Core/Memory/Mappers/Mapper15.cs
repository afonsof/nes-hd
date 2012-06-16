using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper15 : IMapper
    {
        private readonly Map _map;

        public Mapper15(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0xFFFF))
            {
                var X = (byte) (data & 0x3F);
                _map.Cartridge.Mirroring = ((data & 0x40) == 0) ? Mirroring.Vertical : Mirroring.Horizontal;
                var Y = (byte) (data & 0x80);
                Y >>= 7;
                switch (address & 0x3)
                {
                    case 0: //0=32K
                        _map.Switch16KPrgRom(X*4, 0);
                        _map.Switch16KPrgRom((X + 1)*4, 1);
                        break;
                    case 1: //1=128K
                        _map.Switch16KPrgRom(X*4, 0);
                        _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
                        break;
                    case 2: //2=8K
                        _map.Switch8KPrgRom(((X*2) + Y)*2, 0);
                        _map.Switch8KPrgRom(((X*2) + Y)*2, 1);
                        _map.Switch8KPrgRom(((X*2) + Y)*2, 2);
                        _map.Switch8KPrgRom(((X*2) + Y)*2, 3);
                        break;
                    case 3: //3=16K
                        _map.Switch16KPrgRom(X*4, 0);
                        _map.Switch16KPrgRom(X*4, 1);
                        break;
                }
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch32KPrgRom(0);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 15 setup done.", DebugStatus.Cool);
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