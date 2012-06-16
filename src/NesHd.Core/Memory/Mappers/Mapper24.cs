using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper24 : IMapper
    {
        private readonly Map _map;
        public int irq_clock;
        public int irq_counter;
        public bool irq_enable;
        public int irq_latch;

        public Mapper24(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                    _map.Switch16KPrgRom(data*4, 0);
                    break;
                case 0xB003:
                    switch ((data & 0xC) >> 2)
                    {
                        case 0:
                            _map.Cartridge.Mirroring = Mirroring.Horizontal;
                            break;
                        case 1:
                            _map.Cartridge.Mirroring = Mirroring.Vertical;
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

                case 0xC000:
                    _map.Switch8KPrgRom(data*2, 2);
                    break;
                case 0xD000:
                    _map.Switch1KChrRom(data, 0);
                    break;
                case 0xD001:
                    _map.Switch1KChrRom(data, 1);
                    break;
                case 0xD002:
                    _map.Switch1KChrRom(data, 2);
                    break;
                case 0xD003:
                    _map.Switch1KChrRom(data, 3);
                    break;
                case 0xE000:
                    _map.Switch1KChrRom(data, 4);
                    break;
                case 0xE001:
                    _map.Switch1KChrRom(data, 5);
                    break;
                case 0xE002:
                    _map.Switch1KChrRom(data, 6);
                    break;
                case 0xE003:
                    _map.Switch1KChrRom(data, 7);
                    break;

                case 0xF000:
                    irq_latch = data;
                    break;
                case 0xF001:
                    irq_enable = (data & 0x01) != 0;
                    break;
                case 0xF002:
                    irq_counter = irq_latch;
                    break;

                    //Sound
                    //Pulse 1
                case 0x9000:
                    _map.Engine.Apu.VRC6PULSE1.Write9000(data);
                    break;
                case 0x9001:
                    _map.Engine.Apu.VRC6PULSE1.Write9001(data);
                    break;
                case 0x9002:
                    _map.Engine.Apu.VRC6PULSE1.Write9002(data);
                    break;
                    //Pulse 2
                case 0xA000:
                    _map.Engine.Apu.VRC6PULSE2.WriteA000(data);
                    break;
                case 0xA001:
                    _map.Engine.Apu.VRC6PULSE2.WriteA001(data);
                    break;
                case 0xA002:
                    _map.Engine.Apu.VRC6PULSE2.WriteA002(data);
                    break;
                    //Sawtooth
                case 0xB000:
                    _map.Engine.Apu.VRC6SAWTOOTH.WriteB000(data);
                    break;
                case 0xB001:
                    _map.Engine.Apu.VRC6SAWTOOTH.WriteB001(data);
                    break;
                case 0xB002:
                    _map.Engine.Apu.VRC6SAWTOOTH.WriteB002(data);
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom(0, 0);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            //_map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 24 setup OK.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
        }

        public void TickCycleTimer()
        {
            if (irq_enable)
            {
                if ((irq_clock += _map.Engine.Cpu.CycleCounter) >= 0x72)
                {
                    irq_clock -= 0x72;
                    if (irq_counter == 0xFF)
                    {
                        irq_counter = irq_latch;
                        _map.Engine.Cpu.IRQNextTime = true;
                    }
                    else
                    {
                        irq_counter++;
                    }
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