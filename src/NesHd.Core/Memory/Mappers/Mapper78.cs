using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper78 : IMapper
    {
        private readonly Map _Map;

        public Mapper78(Map map)
        {
            _Map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                _Map.Switch16KPrgRom((data & 0x7)*4, 0);
                _Map.Switch8KChrRom(((data & 0xF0) >> 4)*8);
                _Map.Cartridge.Mirroring = Mirroring.OneScreen;
                if ((address & 0xFE00) != 0xFE00)
                {
                    if ((data & 0x8) != 0)
                        _Map.Cartridge.MirroringBase = 0x2000;
                    else
                        _Map.Cartridge.MirroringBase = 0x2400;
                }
            }
        }

        public void SetUpMapperDefaults()
        {
            _Map.Switch16KPrgRom(0, 0);
            _Map.Switch16KPrgRom((_Map.Cartridge.ChrPages - 1)*4, 1);
            _Map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 78 setup OK", DebugStatus.Cool);
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