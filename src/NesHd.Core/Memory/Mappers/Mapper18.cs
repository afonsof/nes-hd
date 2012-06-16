using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper18 : IMapper
    {
        private readonly Map _map;
        public int Mapper18_IRQWidth;
        public short Mapper18_Timer;
        public short Mapper18_latch;
        public byte mapper18_control;
        public bool timer_irq_enabled;
        public byte[] x = new byte[22];

        public Mapper18(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                    x[0] = 0;
                    x[0] = (byte) ((data & 0x0f));
                    break;
                case 0x8001:
                    x[1] = 0;
                    x[1] = (byte) (((data & 0x0f) << 4) | x[0]);
                    _map.Switch8KPrgRom((x[1])*2, 0);
                    break;
                case 0x8002:
                    x[2] = 0;
                    x[2] = (byte) ((data & 0x0f));
                    break;
                case 0x8003:
                    x[3] = 0;
                    x[3] = (byte) (((data & 0x0f) << 4) | x[2]);
                    _map.Switch8KPrgRom((x[3])*2, 1);
                    break;
                case 0x9000:
                    x[4] = 0;
                    x[4] = (byte) ((data & 0x0f));
                    break;
                case 0x9001:
                    x[5] = 0;
                    x[5] = (byte) (((data & 0x0f) << 4) | x[4]);
                    _map.Switch8KPrgRom((x[5])*2, 2);
                    break;
                case 0x9002:
                    _map.Cartridge.IsSaveRam = ((data & 0x1) != 0);
                    _map.IsSRamReadOnly = !_map.Cartridge.IsSaveRam;
                    break;
                case 0xA000:
                    x[3] &= 0xf0;
                    x[3] |= (byte) ((data & 0x0f));
                    break;
                case 0xA001:
                    x[3] &= 0x0f;
                    x[3] |= (byte) ((data & 0x0f) << 4);
                    _map.Switch1KChrRom((x[3]), 0);
                    break;
                case 0xA002:
                    x[4] &= 0xf0;
                    x[4] |= (byte) ((data & 0x0f));
                    break;
                case 0xA003:
                    x[4] &= 0x0f;
                    x[4] |= (byte) ((data & 0x0f) << 4);
                    _map.Switch1KChrRom((x[4]), 1);
                    break;
                case 0xB000:
                    x[5] &= 0xf0;
                    x[5] |= (byte) ((data & 0x0f));
                    break;
                case 0xB001:
                    x[5] &= 0x0f;
                    x[5] |= (byte) ((data & 0x0f) << 4);
                    _map.Switch1KChrRom((x[5]), 2);
                    break;
                case 0xB002:
                    x[6] &= 0xf0;
                    x[6] |= (byte) ((data & 0x0f));
                    break;
                case 0xB003:
                    x[6] &= 0x0f;
                    x[6] |= (byte) ((data & 0x0f) << 4);
                    _map.Switch1KChrRom((x[6]), 3);
                    break;
                case 0xC000:
                    x[7] &= 0xf0;
                    x[7] |= (byte) ((data & 0x0f));
                    break;
                case 0xC001:
                    x[7] &= 0x0f;
                    x[7] |= (byte) ((data & 0x0f) << 4);
                    _map.Switch1KChrRom((x[7]), 4);
                    break;
                case 0xC002:
                    x[8] &= 0xf0;
                    x[8] |= (byte) ((data & 0x0f));
                    break;
                case 0xC003:
                    x[8] &= 0x0f;
                    x[8] |= (byte) ((data & 0x0f) << 4);
                    _map.Switch1KChrRom((x[8]), 5);
                    break;
                case 0xD000:
                    x[9] &= 0xf0;
                    x[9] |= (byte) ((data & 0x0f));
                    break;
                case 0xD001:
                    x[9] &= 0x0f;
                    x[9] |= (byte) ((data & 0x0f) << 4);
                    _map.Switch1KChrRom((x[9]), 6);
                    break;
                case 0xD002:
                    x[10] &= 0xf0;
                    x[10] |= (byte) ((data & 0x0f));
                    break;
                case 0xD003:
                    x[10] &= 0x0f;
                    x[10] |= (byte) ((data & 0x0f) << 4);
                    _map.Switch1KChrRom((x[10]), 7); /*x[10] = 0xff;*/
                    break;
                case 0xE000:
                    Mapper18_latch = (short) ((Mapper18_latch & 0xFFF0) | (data & 0x0f));
                    break;
                case 0xE001:
                    Mapper18_latch = (short) ((Mapper18_latch & 0xFF0F) | ((data & 0x0f) << 4));
                    break;
                case 0xE002:
                    Mapper18_latch = (short) ((Mapper18_latch & 0xF0FF) | ((data & 0x0f) << 8));
                    break;
                case 0xE003:
                    Mapper18_latch = (short) ((Mapper18_latch & 0x0FFF) | ((data & 0x0f) << 12));
                    break;
                case 0xF000:
                    timer_irq_enabled = ((data & 0x01) != 0);
                    break;
                case 0xF001:
                    timer_irq_enabled = ((data & 0x01) != 0);
                    Mapper18_Timer = Mapper18_latch;
                    Mapper18_IRQWidth = (data & 0x0E);
                    break;
                case 0xF002:
                    var BankMode = (address & 0x3);
                    switch (BankMode)
                    {
                        case 0:
                            _map.Cartridge.Mirroring = Mirroring.Horizontal;
                            break;
                        case 1:
                            _map.Cartridge.Mirroring = Mirroring.Vertical;
                            break;
                        case 2:
                        case 3:
                            _map.Cartridge.Mirroring = Mirroring.OneScreen;
                            _map.Cartridge.MirroringBase = 0x2000;
                            break;
                    }
                    break;
            }
            switch (Mapper18_IRQWidth)
            {
                case 0: //16 bit
                    break;
                case 1: //12 bit
                    Mapper18_Timer &= 0x0FFF;
                    break;
                case 2:
                case 3: //8 bit
                    Mapper18_Timer &= 0x00FF;
                    break;
                case 4:
                case 5:
                case 6:
                case 7: //4 bit
                    Mapper18_Timer &= 0x000F;
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            timer_irq_enabled = false;
            _map.Switch16KPrgRom(0, 0);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 18 setup done.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
        }

        public void TickCycleTimer()
        {
            if (timer_irq_enabled)
            {
                if (Mapper18_Timer > 0)
                    Mapper18_Timer -= (short) (_map.Engine.Cpu.CycleCounter);
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