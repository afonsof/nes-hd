using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper11 : IMapper
    {
        private readonly Map _map;

        public Mapper11(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0xFFFF))
            {
                _map.Switch32KPrgRom((data & 0xf)*8);
                _map.Switch8KChrRom((data >> 4)*8);
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch32KPrgRom(0);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 11 setup done.", DebugStatus.Cool);
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