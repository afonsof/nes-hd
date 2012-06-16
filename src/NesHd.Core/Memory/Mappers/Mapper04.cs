using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper04 : IMapper
    {
        private readonly Map _map;
        public int Mapper4ChrAddressSelect;
        public int Mapper4CommandNumber;
        public int Mapper4PRGAddressSelect;
        public uint TimerIrqCount;
        public bool TimerIrqEnabled;
        public uint TimerIrqReload;

        public Mapper04(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                Mapper4CommandNumber = data & 0x7;
                Mapper4PRGAddressSelect = data & 0x40;
                Mapper4ChrAddressSelect = data & 0x80;
            }
            else if (address == 0x8001)
            {
                if (Mapper4CommandNumber == 0)
                {
                    data = (byte) (data - (data%2));
                    if (Mapper4ChrAddressSelect == 0)
                        _map.Switch2KChrRom(data, 0);
                    else
                        _map.Switch2KChrRom(data, 2);
                }
                else if (Mapper4CommandNumber == 1)
                {
                    data = (byte) (data - (data%2));
                    if (Mapper4ChrAddressSelect == 0)
                    {
                        _map.Switch2KChrRom(data, 1);
                    }
                    else
                    {
                        _map.Switch2KChrRom(data, 3);
                    }
                }
                else if (Mapper4CommandNumber == 2)
                {
                    data = (byte) (data & (_map.Cartridge.ChrPages*8 - 1));
                    if (Mapper4ChrAddressSelect == 0)
                    {
                        _map.Switch1KChrRom(data, 4);
                    }
                    else
                    {
                        _map.Switch1KChrRom(data, 0);
                    }
                }
                else if (Mapper4CommandNumber == 3)
                {
                    if (Mapper4ChrAddressSelect == 0)
                    {
                        _map.Switch1KChrRom(data, 5);
                    }
                    else
                    {
                        _map.Switch1KChrRom(data, 1);
                    }
                }
                else if (Mapper4CommandNumber == 4)
                {
                    if (Mapper4ChrAddressSelect == 0)
                    {
                        _map.Switch1KChrRom(data, 6);
                    }
                    else
                    {
                        _map.Switch1KChrRom(data, 2);
                    }
                }
                else if (Mapper4CommandNumber == 5)
                {
                    if (Mapper4ChrAddressSelect == 0)
                    {
                        _map.Switch1KChrRom(data, 7);
                    }
                    else
                    {
                        _map.Switch1KChrRom(data, 3);
                    }
                }
                else if (Mapper4CommandNumber == 6)
                {
                    if (Mapper4PRGAddressSelect == 0)
                    {
                        _map.Switch8KPrgRom(data*2, 0);
                    }
                    else
                    {
                        _map.Switch8KPrgRom(data*2, 2);
                    }
                }
                else if (Mapper4CommandNumber == 7)
                {
                    _map.Switch8KPrgRom(data*2, 1);
                }

                if (Mapper4PRGAddressSelect == 0)
                {
                    _map.Switch8KPrgRom(((_map.Cartridge.PrgPages*4) - 2)*2, 2);
                }
                else
                {
                    _map.Switch8KPrgRom(((_map.Cartridge.PrgPages*4) - 2)*2, 0);
                }
                _map.Switch8KPrgRom(((_map.Cartridge.PrgPages*4) - 1)*2, 3);
            }
            else if (address == 0xA000)
            {
                if ((data & 0x1) == 0)
                {
                    _map.Cartridge.Mirroring = Mirroring.Vertical;
                }
                else
                {
                    _map.Cartridge.Mirroring = Mirroring.Horizontal;
                }
            }
            else if (address == 0xA001)
            {
                if ((data & 0x80) == 0)
                    _map.IsSRamReadOnly = true;
                else
                    _map.IsSRamReadOnly = false;
            }
            else if (address == 0xC000)
            {
                TimerIrqReload = data;
            }
            else if (address == 0xC001)
            {
                TimerIrqCount = data;
            }
            else if (address == 0xE000)
            {
                TimerIrqEnabled = false;
                TimerIrqReload = TimerIrqCount;
            }
            else if (address == 0xE001)
            {
                TimerIrqEnabled = true;
            }
        }

        public void SetUpMapperDefaults()
        {
            TimerIrqCount = TimerIrqReload = 0xff;
            Mapper4PRGAddressSelect = 0;
            Mapper4ChrAddressSelect = 0;
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 4 setup done.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
            if (TimerIrqReload == 0)
            {
                if (TimerIrqEnabled)
                {
                    _map.Engine.Cpu.IRQNextTime = true;
                    TimerIrqEnabled = false;
                }
                TimerIrqReload = TimerIrqCount;
            }
            TimerIrqReload -= 1;
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