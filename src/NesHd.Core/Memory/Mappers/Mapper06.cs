using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper06 : IMapper
    {
        private readonly Map _map;
        public bool IRQEnabled;
        public int irq_counter;

        public Mapper06(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                _map.Switch16KPrgRom(((data & 0x3C) >> 2)*4, 0);
                _map.Switch8KChrRom((data & 0x3)*8);
            }
            else
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
                }
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom(7*4, 1);
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