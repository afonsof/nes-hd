using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper69 : IMapper
    {
        private readonly Map _map;
        public ushort reg;
        public short timer_irq_counter_69;
        public bool timer_irq_enabled;

        public Mapper69(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address & 0xE000)
            {
                case 0x8000:
                    reg = data;
                    break;
                case 0xA000:
                    switch (reg & 0x0F)
                    {
                        case 0x00:
                            _map.Switch1KChrRom(data, 0);
                            break;
                        case 0x01:
                            _map.Switch1KChrRom(data, 1);
                            break;
                        case 0x02:
                            _map.Switch1KChrRom(data, 2);
                            break;
                        case 0x03:
                            _map.Switch1KChrRom(data, 3);
                            break;
                        case 0x04:
                            _map.Switch1KChrRom(data, 4);
                            break;
                        case 0x05:
                            _map.Switch1KChrRom(data, 5);
                            break;
                        case 0x06:
                            _map.Switch1KChrRom(data, 6);
                            break;
                        case 0x07:
                            _map.Switch1KChrRom(data, 7);
                            break;
                        case 0x08:
                            if ((data & 0x40) == 0)
                            {
                                _map.Switch8KPrgRom((data & 0x3F)*2, 0);
                            }
                            break;
                        case 0x09:
                            _map.Switch8KPrgRom(data*2, 0);
                            break;
                        case 0x0A:
                            _map.Switch8KPrgRom(data*2, 1);
                            break;
                        case 0x0B:
                            _map.Switch8KPrgRom(data*2, 2);
                            break;

                        case 0x0C:
                            data &= 0x03;
                            if (data == 0) _map.Cartridge.Mirroring = Mirroring.Vertical;
                            if (data == 1) _map.Cartridge.Mirroring = Mirroring.Horizontal;
                            if (data == 2)
                            {
                                _map.Cartridge.Mirroring = Mirroring.OneScreen;
                                _map.Cartridge.MirroringBase = 0x2000;
                            }
                            if (data == 3)
                            {
                                _map.Cartridge.Mirroring = Mirroring.OneScreen;
                                _map.Cartridge.MirroringBase = 0x2400;
                            }
                            break;

                        case 0x0D:
                            if (data == 0)
                                timer_irq_enabled = false;
                            if (data == 0x81)
                                timer_irq_enabled = true;
                            break;

                        case 0x0E:
                            timer_irq_counter_69 = (short) ((timer_irq_counter_69 & 0xFF00) | data);

                            break;

                        case 0x0F:
                            timer_irq_counter_69 = (short) ((timer_irq_counter_69 & 0x00FF) | (data << 8));
                            break;
                    }
                    break;

                case 0xC000:
                case 0xE000:
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
            Debug.WriteLine(this, "Mapper 69 setup done.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
        }

        public void TickCycleTimer()
        {
            if (timer_irq_enabled)
            {
                if (timer_irq_counter_69 > 0)
                    timer_irq_counter_69 -= (short) (_map.Engine.Cpu.CycleCounter - 1);
                else
                {
                    _map.Engine.Cpu.IRQNextTime = true;
                    timer_irq_enabled = false;
                }
            }
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