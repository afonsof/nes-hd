using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    /*IQR timer fixed at 28/3/2010 by AHD*/

    internal class Mapper16 : IMapper
    {
        private readonly Map _map;
        public short timer_irq_Latch_16;
        public short timer_irq_counter_16;
        public bool timer_irq_enabled;

        public Mapper16(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address & 0xF)
            {
                case 0:
                    _map.Switch1KChrRom(data, 0);
                    break;
                case 1:
                    _map.Switch1KChrRom(data, 1);
                    break;
                case 2:
                    _map.Switch1KChrRom(data, 2);
                    break;
                case 3:
                    _map.Switch1KChrRom(data, 3);
                    break;
                case 4:
                    _map.Switch1KChrRom(data, 4);
                    break;
                case 5:
                    _map.Switch1KChrRom(data, 5);
                    break;
                case 6:
                    _map.Switch1KChrRom(data, 6);
                    break;
                case 7:
                    _map.Switch1KChrRom(data, 7);
                    break;
                case 8:
                    _map.Switch16KPrgRom(data*4, 0);
                    break;
                case 9:
                    switch (data & 0x3)
                    {
                        case 0:
                            _map.Cartridge.Mirroring = Mirroring.Vertical;
                            break;
                        case 1:
                            _map.Cartridge.Mirroring = Mirroring.Horizontal;
                            break;
                        case 2:
                            _map.Cartridge.Mirroring = Mirroring.OneScreen;
                            _map.Cartridge.MirroringBase = 0x2000;
                            break;
                        case 3:
                            _map.Cartridge.Mirroring = Mirroring.OneScreen;
                            _map.Cartridge.MirroringBase = 0x2400;
                            break;
                    }
                    break;
                case 0xA:
                    timer_irq_enabled = ((data & 0x1) != 0);
                    timer_irq_counter_16 = timer_irq_Latch_16;
                    break;
                case 0xB:
                    timer_irq_Latch_16 = (short) ((timer_irq_Latch_16 & 0xFF00) | data);
                    break;
                case 0xC:
                    timer_irq_Latch_16 = (short) ((data << 8) | (timer_irq_Latch_16 & 0x00FF));
                    break;
                case 0xD:
                    break; //
            }
        }

        public void SetUpMapperDefaults()
        {
            timer_irq_enabled = false;
            _map.Switch16KPrgRom(0, 0);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 16 setup done.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
        }

        public void TickCycleTimer()
        {
            if (timer_irq_enabled)
            {
                if (timer_irq_counter_16 > 0)
                    timer_irq_counter_16 -= (short) (_map.Engine.Cpu.CycleCounter - 1);
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
            get { return true; }
        }

        public bool WriteUnder6000
        {
            get { return false; }
        }

        #endregion
    }
}