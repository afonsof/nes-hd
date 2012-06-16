using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper64 : IMapper
    {
        private readonly Map _map;
        public byte Mapper64ChrAddressSelect;
        public byte Mapper64CommandNumber;
        public byte Mapper64PrgAddressSelect;

        public Mapper64(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                    Mapper64CommandNumber = data;
                    Mapper64PrgAddressSelect = (byte) (data & 0x40);
                    Mapper64ChrAddressSelect = (byte) (data & 0x80);
                    break;
                case 0x8001:
                    switch ((Mapper64CommandNumber & 0xf))
                    {
                        case 0:
                            data = (byte) (data - (data%2));
                            _map.Switch2KChrRom(data, Mapper64ChrAddressSelect == 0 ? 0 : 2);
                            break;
                        case 1:
                            data = (byte) (data - (data%2));
                            _map.Switch2KChrRom(data, Mapper64ChrAddressSelect == 0 ? 1 : 3);
                            break;
                        case 2:
                            _map.Switch1KChrRom(data, Mapper64ChrAddressSelect == 0 ? 4 : 0);
                            break;
                        case 3:
                            _map.Switch1KChrRom(data, Mapper64ChrAddressSelect == 0 ? 5 : 1);
                            break;
                        case 4:
                            _map.Switch1KChrRom(data, Mapper64ChrAddressSelect == 0 ? 6 : 2);
                            break;
                        case 5:
                            _map.Switch1KChrRom(data, Mapper64ChrAddressSelect == 0 ? 7 : 3);
                            break;
                        case 6:
                            _map.Switch8KPrgRom(data*2, Mapper64PrgAddressSelect == 0 ? 0 : 1);
                            break;
                        case 7:
                            _map.Switch8KPrgRom(data*2, Mapper64PrgAddressSelect == 0 ? 1 : 2);
                            break;
                        case 8:
                            _map.Switch1KChrRom(data, 1);
                            break;
                        case 9:
                            _map.Switch1KChrRom(data, 3);
                            break;
                        case 0xf:
                            _map.Switch8KPrgRom(data*2, Mapper64PrgAddressSelect == 0 ? 2 : 0);
                            break;
                    }
                    break;
                case 0xA000:
                    _map.Cartridge.Mirroring = (data & 1) == 1 ? Mirroring.Vertical : Mirroring.Horizontal;
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch8KPrgRom((_map.Cartridge.PrgPages*4) - 2, 0);
            _map.Switch8KPrgRom((_map.Cartridge.PrgPages*4) - 2, 1);
            _map.Switch8KPrgRom((_map.Cartridge.PrgPages*4) - 2, 2);
            _map.Switch8KPrgRom((_map.Cartridge.PrgPages*4) - 2, 3);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 64 setup done", DebugStatus.Cool);
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