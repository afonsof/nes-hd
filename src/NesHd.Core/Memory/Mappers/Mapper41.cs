using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper41 : IMapper
    {
        private readonly Map _map;
        public byte Mapper41_CHR_High;
        public byte Mapper41_CHR_Low;
        public ushort temp;

        public Mapper41(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address >= 0x6000 & address <= 0xFFFF)
            {
                _map.Switch32KPrgRom((address & 0x7)*8);
                _map.Cartridge.Mirroring = ((address & 0x20) == 0) ? Mirroring.Vertical : Mirroring.Horizontal;
                Mapper41_CHR_High = (byte) (data & 0x18);
                if ((address & 0x4) == 0)
                {
                    Mapper41_CHR_Low = (byte) (data & 0x3);
                }
                temp = (ushort) (Mapper41_CHR_High | Mapper41_CHR_Low);
                _map.Switch8KChrRom((temp)*8);
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch32KPrgRom(0);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 41 setup done", DebugStatus.Cool);
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