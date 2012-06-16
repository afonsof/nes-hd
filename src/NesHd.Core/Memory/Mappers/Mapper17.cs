using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper17 : IMapper
    {
        private readonly Map _map;
        public bool IRQEnabled;
        public int irq_counter;

        public Mapper17(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x42FC:
                case 0x42FE:
                case 0x42FF:
                    switch (address & 0x1)
                    {
                        case 0:
                            _map.Cartridge.Mirroring = Mirroring.OneScreen;
                            if ((data & 0x10) != 0)
                                _map.Cartridge.MirroringBase = 0x2400;
                            else
                                _map.Cartridge.MirroringBase = 0x2000;
                            break;
                        case 1:
                            if ((data & 0x10) != 0)
                                _map.Cartridge.Mirroring = Mirroring.Horizontal;
                            else
                                _map.Cartridge.Mirroring = Mirroring.Vertical;
                            break;
                    }
                    break;
                case 0x4501:
                    IRQEnabled = false;
                    break;
                case 0x4502:
                    irq_counter = (short) ((irq_counter & 0xFF00) | data);
                    break;
                case 0x4503:
                    irq_counter = (short) ((data << 8) | (irq_counter & 0x00FF));
                    IRQEnabled = true;
                    break;
                case 0x4504:
                    _map.Switch8KPrgRom(data*2, 0);
                    break;
                case 0x4505:
                    _map.Switch8KPrgRom(data*2, 1);
                    break;
                case 0x4506:
                    _map.Switch8KPrgRom(data*2, 2);
                    break;
                case 0x4507:
                    _map.Switch8KPrgRom(data*2, 3);
                    break;
                case 0x4510:
                    _map.Switch1KChrRom(data, 0);
                    break;
                case 0x4511:
                    _map.Switch1KChrRom(data, 1);
                    break;
                case 0x4512:
                    _map.Switch1KChrRom(data, 2);
                    break;
                case 0x4513:
                    _map.Switch1KChrRom(data, 3);
                    break;
                case 0x4514:
                    _map.Switch1KChrRom(data, 4);
                    break;
                case 0x4515:
                    _map.Switch1KChrRom(data, 5);
                    break;
                case 0x4516:
                    _map.Switch1KChrRom(data, 6);
                    break;
                case 0x4517:
                    _map.Switch1KChrRom(data, 7);
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom(0, 0);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 6 setup done.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
        }

        public void TickCycleTimer()
        {
            if (IRQEnabled)
            {
                irq_counter += (short) (_map.Engine.Cpu.CycleCounter);
                if (irq_counter >= 0xFFFF)
                {
                    _map.Engine.Cpu.IRQNextTime = true;
                    irq_counter = 0;
                }
            }
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