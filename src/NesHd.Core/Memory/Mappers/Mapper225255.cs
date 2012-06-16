using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper225255 : IMapper
    {
        private readonly Map _Map;
        public byte Mapper225_reg0 = 0xF;
        public byte Mapper225_reg1 = 0xF;
        public byte Mapper225_reg2 = 0xF;
        public byte Mapper225_reg3 = 0xF;

        public Mapper225255(Map map)
        {
            _Map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0xFFFF))
            {
                var banks = ((address & 0x4000) != 0) ? 1 : 0;
                var wher = (ushort) ((address & 0x0Fc0) >> 7);
                _Map.Switch8KChrRom(((address & 0x003F) + (banks << 6))*8);
                if ((address & 0x1000) != 0) //A12
                {
                    if ((address & 0x0040) != 0) //A6
                    {
                        _Map.Switch16KPrgRom((((wher + (banks << 5)) << 1) + 1)*4, 0);
                        _Map.Switch16KPrgRom((((wher + (banks << 5)) << 1) + 1)*4, 1);
                    }
                    else
                    {
                        _Map.Switch16KPrgRom(((wher + (banks << 5)) << 1)*4, 0);
                        _Map.Switch16KPrgRom(((wher + (banks << 5)) << 1)*4, 1);
                    }
                }
                else
                {
                    _Map.Switch32KPrgRom((wher + (banks << 5))*8); //ignore A6
                }
                _Map.Cartridge.Mirroring = ((address >> 13) & 1) == 0 ? Mirroring.Vertical : Mirroring.Horizontal;
            }
            else if ((address >= 0x5800) && (address <= 0x5FFF))
            {
                switch (address & 0x3)
                {
                    case 0:
                        Mapper225_reg0 = (byte) (data & 0xf);
                        break;
                    case 1:
                        Mapper225_reg1 = (byte) (data & 0xf);
                        break;
                    case 2:
                        Mapper225_reg2 = (byte) (data & 0xf);
                        break;
                    case 3:
                        Mapper225_reg3 = (byte) (data & 0xf);
                        break;
                }
            }
        }

        public void SetUpMapperDefaults()
        {
            _Map.Switch32KPrgRom(0);
            _Map.Cartridge.Mirroring = Mirroring.Vertical;
            _Map.Switch8KChrRom(0);
            Mapper225_reg0 = 0xF;
            Mapper225_reg1 = 0xF;
            Mapper225_reg2 = 0xF;
            Mapper225_reg3 = 0xF;
            Debug.WriteLine(this, "Mapper 225 / 255 setup done", DebugStatus.Cool);
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

        public byte ReadRegisters(ushort address)
        {
            if ((address >= 0x5800) && (address <= 0x5FFF))
            {
                switch (address & 0x3)
                {
                    case 0:
                        return Mapper225_reg0;
                    case 1:
                        return Mapper225_reg1;
                    case 2:
                        return Mapper225_reg2;
                    case 3:
                        return Mapper225_reg3;
                }
            }
            return 0xF;
        }
    }
}