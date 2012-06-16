using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper34 : IMapper
    {
        private readonly Map _map;

        public Mapper34(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address == 0x7ffd)
            {
                _map.Switch32KPrgRom(data*8);
            }
            else if (address == 0x7ffe)
            {
                _map.Switch4KChrRom(data*4, 0);
            }
            else if (address == 0x7fff)
            {
                _map.Switch4KChrRom(data*4, 1);
            }
            else if (address >= 0x8000)
            {
                _map.Switch32KPrgRom(data*8);
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch32KPrgRom(0);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 34 setup done", DebugStatus.Cool);
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