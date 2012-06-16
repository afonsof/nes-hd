using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper23 : IMapper
    {
        private readonly Map _map;
        public bool PRGMode = true;
        public byte[] REG = new byte[8];
        public int IrqClock;
        public int IrqCounter;
        public int IrqEnable;
        public int IrqLatch;

        public Mapper23(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                case 0x8004:
                case 0x8008:
                case 0x800C:
                    if (PRGMode)
                        _map.Switch8KPrgRom(data*2, 0);
                    else
                        _map.Switch8KPrgRom(data*2, 2);
                    break;
                case 0x9000:
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
                case 0x9008:
                    _map.IsSRamReadOnly = (data & 0x1) == 0;
                    PRGMode = (data & 0x2) == 0x2;
                    break;
                case 0xA000:
                case 0xA004:
                case 0xA008:
                case 0xA00C:
                    _map.Switch8KPrgRom(data*2, 1);
                    break;

                case 0xB000:
                    REG[0] = (byte) ((REG[0] & 0xF0) | (data & 0x0F));
                    _map.Switch1KChrRom(REG[0], 0);
                    break;
                case 0xB001:
                case 0xB004:
                    REG[0] = (byte) (((data & 0x0F) << 4) | (REG[0] & 0x0F));
                    _map.Switch1KChrRom(REG[0], 0);
                    break;

                case 0xB002:
                case 0xB008:
                    REG[1] = (byte) ((REG[1] & 0xF0) | (data & 0x0F));
                    _map.Switch1KChrRom(REG[1], 1);
                    break;
                case 0xB003:
                case 0xB00C:
                    REG[1] = (byte) (((data & 0x0F) << 4) | (REG[1] & 0x0F));
                    _map.Switch1KChrRom(REG[1], 1);
                    break;

                case 0xC000:
                    REG[2] = (byte) ((REG[2] & 0xF0) | (data & 0x0F));
                    _map.Switch1KChrRom(REG[2], 2);
                    break;
                case 0xC001:
                case 0xC004:
                    REG[2] = (byte) (((data & 0x0F) << 4) | (REG[2] & 0x0F));
                    _map.Switch1KChrRom(REG[2], 2);
                    break;

                case 0xC002:
                case 0xC008:
                    REG[3] = (byte) ((REG[3] & 0xF0) | (data & 0x0F));
                    _map.Switch1KChrRom(REG[3], 3);
                    break;
                case 0xC003:
                case 0xC00C:
                    REG[3] = (byte) (((data & 0x0F) << 4) | (REG[3] & 0x0F));
                    _map.Switch1KChrRom(REG[3], 3);
                    break;

                case 0xD000:
                    REG[4] = (byte) ((REG[4] & 0xF0) | (data & 0x0F));
                    _map.Switch1KChrRom(REG[4], 4);
                    break;
                case 0xD001:
                case 0xD004:
                    REG[4] = (byte) (((data & 0x0F) << 4) | (REG[4] & 0x0F));
                    _map.Switch1KChrRom(REG[4], 4);
                    break;

                case 0xD002:
                case 0xD008:
                    REG[5] = (byte) ((REG[5] & 0xF0) | (data & 0x0F));
                    _map.Switch1KChrRom(REG[5], 5);
                    break;
                case 0xD003:
                case 0xD00C:
                    REG[5] = (byte) (((data & 0x0F) << 4) | (REG[5] & 0x0F));
                    _map.Switch1KChrRom(REG[5], 5);
                    break;

                case 0xE000:
                    REG[6] = (byte) ((REG[6] & 0xF0) | (data & 0x0F));
                    _map.Switch1KChrRom(REG[6], 6);
                    break;
                case 0xE001:
                case 0xE004:
                    REG[6] = (byte) (((data & 0x0F) << 4) | (REG[6] & 0x0F));
                    _map.Switch1KChrRom(REG[6], 6);
                    break;

                case 0xE002:
                case 0xE008:
                    REG[7] = (byte) ((REG[7] & 0xF0) | (data & 0x0F));
                    _map.Switch1KChrRom(REG[7], 7);
                    break;
                case 0xE003:
                case 0xE00C:
                    REG[7] = (byte) (((data & 0x0F) << 4) | (REG[7] & 0x0F));
                    _map.Switch1KChrRom(REG[7], 7);
                    break;

                case 0xF000:
                    IrqLatch = (IrqLatch & 0xF0) | (data & 0x0F);
                    _map.Engine.Cpu.IRQNextTime = true;
                    break;
                case 0xF004:
                    IrqLatch = (IrqLatch & 0x0F) | ((data & 0x0F) << 4);
                    _map.Engine.Cpu.IRQNextTime = true;
                    break;
                case 0xF008:
                    IrqEnable = data & 0x03;
                    IrqCounter = IrqLatch;
                    IrqClock = 0;
                    _map.Engine.Cpu.IRQNextTime = true;
                    break;
                case 0xF00C:
                    IrqEnable = (IrqEnable & 0x01)*3;
                    _map.Engine.Cpu.IRQNextTime = true;
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch16KPrgRom(0, 0);
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            //_map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 23 setup OK.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
        }

        public void TickCycleTimer()
        {
            if ((IrqEnable & 0x02) != 0)
            {
                IrqClock += _map.Engine.Cpu.CycleCounter*3;
                while (IrqClock >= 341)
                {
                    IrqClock -= 341;
                    IrqCounter++;
                    if (IrqCounter == 0)
                    {
                        IrqCounter = IrqLatch;
                        _map.Engine.Cpu.IRQNextTime = true;
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