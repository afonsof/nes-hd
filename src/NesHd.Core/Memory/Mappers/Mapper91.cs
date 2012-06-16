using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper91 : IMapper
    {
        private readonly Map _map;
        public int IRQCount;
        public bool IRQEnabled;

        public Mapper91(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address == 0x6000)
                _map.Switch2KChrRom(data*2, 0);
            else if (address == 0x6001)
                _map.Switch2KChrRom(data*2, 1);
            else if (address == 0x6002)
                _map.Switch2KChrRom(data*2, 2);
            else if (address == 0x6003)
                _map.Switch2KChrRom(data*2, 3);
            else if (address == 0x7000)
                _map.Switch8KPrgRom(data*2, 0);
            else if (address == 0x7001)
                _map.Switch8KPrgRom(data*2, 1);
            else if (address == 0x7006)
            {
                IRQEnabled = false;
                IRQCount = 0;
            }
            else if (address == 0x7007)
                IRQEnabled = true;
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 91 setup OK.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
            if (IRQCount < 8 & IRQEnabled)
            {
                IRQCount++;
                if (IRQCount >= 8)
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
            get { return true; }
        }

        public bool WriteUnder6000
        {
            get { return false; }
        }

        #endregion
    }
}