using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper00 : IMapper
    {
        private readonly Map _map;

        public Mapper00(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte value)
        {
            Debug.WriteLine(this, "Attempt to write into PRG, mapper 0.", DebugStatus.Error);
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch32KPrgRom(0);
            _map.Switch8KChrRom(0);
            if (_map.Cartridge.PrgPages == 1)
            {
                _map.Switch16KPrgRom(0, 1);
            }
            Debug.WriteLine(this, "Mapper 0 setup OK", DebugStatus.Cool);
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