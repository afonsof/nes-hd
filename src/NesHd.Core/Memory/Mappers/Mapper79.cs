using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper79 : IMapper
    {
        private readonly Map _map;

        public Mapper79(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if ((address & 0x0100) != 0)
            {
                _map.Switch32KPrgRom(((data >> 3) & 0x01)*8);
                _map.Switch8KChrRom((data & 0x07)*8);
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch32KPrgRom(0);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 79 setup done.", DebugStatus.Cool);
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

        public bool WriteUnder6000
        {
            get { return true; }
        }

        public bool WriteUnder8000
        {
            get { return false; }
        }

        #endregion
    }
}