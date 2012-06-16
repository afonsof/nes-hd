using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper09 : IMapper
    {
        private readonly Map _map;
        public byte latch1;
        public int latch1data1, latch1data2;
        public byte latch2;
        public int latch2data1, latch2data2;

        public Mapper09(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if ((address >= 0xa000) && (address <= 0xafff))
            {
                _map.Switch8KPrgRom(data*2, 0);
            }
            else if ((address >= 0xB000) && (address <= 0xCFFF))
            {
                _map.Switch4KChrRom(data*4, 0);
            }
            else if ((address >= 0xD000) && (address <= 0xDFFF))
            {
                latch1data1 = data*4;
            }
            else if ((address >= 0xE000) && (address <= 0xEFFF))
            {
                latch1data2 = data*4;
            }
            else if ((address >= 0xF000) && (address <= 0xFFFF))
            {
                if ((data & 1) == 1)
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
            latch1 = 0xfe;
            _map.Switch32KPrgRom((_map.Cartridge.PrgPages - 1)*4 - 4);
            _map.Switch8KPrgRom(0, 0);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 9 setup done.", DebugStatus.Cool);
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