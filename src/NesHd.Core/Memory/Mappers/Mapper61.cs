using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper61 : IMapper
    {
        private readonly Map _map;

        public Mapper61(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                if ((address & 0x10) == 0)
                    _map.Switch32KPrgRom((address & 0xF)*8);
                else
                {
                    _map.Switch16KPrgRom((((address & 0xF) << 1) | (((address & 0x20) >> 5)))*4, 0);
                    _map.Switch16KPrgRom((((address & 0xF) << 1) | (((address & 0x20) >> 5)))*4, 1);
                }

                _map.Cartridge.Mirroring = ((address & 0x80) != 0) ? Mirroring.Horizontal : Mirroring.Vertical;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch32KPrgRom(0);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 61 setup OK", DebugStatus.Cool);
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