using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper10 : IMapper
    {
        private readonly Map _map;
        public byte Latch1;
        public int Latch1Data1, Latch1Data2;
        public byte Latch2;
        public int Latch2Data1, Latch2Data2;

        public Mapper10(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if ((address >= 0xa000) && (address <= 0xafff))
            {
                _map.Switch16KPrgRom(data*4, 0);
            }
            else if ((address >= 0xB000) && (address <= 0xBFFF))
            {
                if (Latch1 == 0xfd)
                {
                    _map.Switch4KChrRom(data*4, 0);
                }

                Latch1Data1 = data*4;
            }
            else if ((address >= 0xC000) && (address <= 0xCFFF))
            {
                if (Latch1 == 0xfe)
                {
                    _map.Switch4KChrRom(data*4, 0);
                }

                Latch1Data2 = data*4;
            }
            else if ((address >= 0xD000) && (address <= 0xDFFF))
            {
                if (Latch2 == 0xfd)
                {
                    _map.Switch4KChrRom(data*4, 1);
                }

                Latch2Data1 = data*4;
            }
            else if ((address >= 0xE000) && (address <= 0xEFFF))
            {
                if (Latch2 == 0xfe)
                {
                    _map.Switch4KChrRom(data*4, 1);
                }

                Latch2Data2 = data*4;
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
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            Latch1 = 0xfe;
            Latch2 = 0xfe;
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 10 setup done.", DebugStatus.Cool);
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