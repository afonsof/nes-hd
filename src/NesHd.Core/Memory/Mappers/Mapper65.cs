using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper65 : IMapper
    {
        private readonly Map _map;
        public short timer_irq_Latch_65;
        public short timer_irq_counter_65;
        public bool timer_irq_enabled;

        public Mapper65(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
                _map.Switch8KPrgRom(data*2, 0);
            else if (address == 0xA000)
                _map.Switch8KPrgRom(data*2, 1);
            else if (address == 0xC000)
                _map.Switch8KPrgRom(data*2, 2);
            else if (address == 0x9003)
                timer_irq_enabled = ((data & 0x80) != 0);
            else if (address == 0x9004)
                timer_irq_counter_65 = timer_irq_Latch_65;
            else if (address == 0x9005)
                timer_irq_Latch_65 = (short) ((timer_irq_Latch_65 & 0x00FF) | (data << 8));
            else if (address == 0x9006)
                timer_irq_Latch_65 = (short) ((timer_irq_Latch_65 & 0xFF00) | (data));
            else if (address == 0xB000)
                _map.Switch1KChrRom(data, 0);
            else if (address == 0xB001)
                _map.Switch1KChrRom(data, 1);
            else if (address == 0xB002)
                _map.Switch1KChrRom(data, 2);
            else if (address == 0xB003)
                _map.Switch1KChrRom(data, 3);
            else if (address == 0xB004)
                _map.Switch1KChrRom(data, 4);
            else if (address == 0xB005)
                _map.Switch1KChrRom(data, 5);
            else if (address == 0xB006)
                _map.Switch1KChrRom(data, 6);
            else if (address == 0xB007)
                _map.Switch1KChrRom(data, 7);
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom(0, 0);
            _map.Switch16KPrgRom((_map.Cartridge.ChrPages - 1)*4, 1);
            Debug.WriteLine(this, "Mapper 65 setup done", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
        }

        public void TickCycleTimer()
        {
            if (timer_irq_enabled)
            {
                timer_irq_counter_65 -= (short) (_map.Engine.Cpu.CycleCounter);
                if (timer_irq_counter_65 <= 0)
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