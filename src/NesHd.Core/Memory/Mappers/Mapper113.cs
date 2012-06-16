using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper113 : IMapper
    {
        private readonly Map _map;

        public Mapper113(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address >= 0x4100 & address <= 0x41FF)
            {
                _map.Switch8KChrRom((data & 0x3)*8);
                _map.Switch32KPrgRom(((data & 0x18) >> 3)*8);
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch8KChrRom(0);
            _map.Switch32KPrgRom(0);
            Debug.WriteLine(this, "Mapper 113 setup OK", DebugStatus.Cool);
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
            get { return true; }
        }

        #endregion
    }
}