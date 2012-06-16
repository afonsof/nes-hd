using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    internal class Mapper01 : IMapper
    {
        private readonly Map _map;
        public byte Mapper1MirroringFlag;
        public byte Mapper1OnePageMirroring;
        public byte Mapper1PRGSwitchingArea;
        public byte Mapper1PRGSwitchingSize;
        public int Mapper1Register8000BitPosition;
        public int Mapper1Register8000Value;
        public int Mapper1RegisterA000BitPosition;
        public int Mapper1RegisterA000Value;
        public int Mapper1RegisterC000BitPosition;
        public int Mapper1RegisterC000Value;
        public int Mapper1RegisterE000BitPosition;
        public int Mapper1RegisterE000Value;
        public byte Mapper1VromSwitchingSize;

        public Mapper01(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            //Using Mapper #1
            if ((address >= 0x8000) && (address <= 0x9FFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    Mapper1Register8000BitPosition = 0;
                    Mapper1Register8000Value = 0;
                    Mapper1MirroringFlag = 0;
                    Mapper1OnePageMirroring = 1;
                    Mapper1PRGSwitchingArea = 1;
                    Mapper1PRGSwitchingSize = 1;
                    Mapper1VromSwitchingSize = 0;
                }
                else
                {
                    Mapper1Register8000Value += (data & 0x1) << Mapper1Register8000BitPosition;
                    Mapper1Register8000BitPosition++;
                    if (Mapper1Register8000BitPosition == 5)
                    {
                        Mapper1MirroringFlag = (byte) (Mapper1Register8000Value & 0x1);
                        _map.Cartridge.Mirroring = Mapper1MirroringFlag == 0 ? Mirroring.Vertical : Mirroring.Horizontal;
                        Mapper1OnePageMirroring = (byte) ((Mapper1Register8000Value >> 1) & 0x1);
                        if (Mapper1OnePageMirroring == 0)
                        {
                            _map.Cartridge.Mirroring = Mirroring.OneScreen;
                            _map.Cartridge.MirroringBase = 0x2000;
                        }
                        Mapper1PRGSwitchingArea = (byte) ((Mapper1Register8000Value >> 2) & 0x1);
                        Mapper1PRGSwitchingSize = (byte) ((Mapper1Register8000Value >> 3) & 0x1);
                        Mapper1VromSwitchingSize = (byte) ((Mapper1Register8000Value >> 4) & 0x1);
                        Mapper1Register8000BitPosition = 0;
                        Mapper1Register8000Value = 0;
                        Mapper1RegisterA000BitPosition = 0;
                        Mapper1RegisterA000Value = 0;
                        Mapper1RegisterC000BitPosition = 0;
                        Mapper1RegisterC000Value = 0;
                        Mapper1RegisterE000BitPosition = 0;
                        Mapper1RegisterE000Value = 0;
                    }
                }
            }
            else if ((address >= 0xA000) && (address <= 0xBFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    Mapper1RegisterA000BitPosition = 0;
                    Mapper1RegisterA000Value = 0;
                }
                else
                {
                    Mapper1RegisterA000Value += (data & 0x1) << Mapper1RegisterA000BitPosition;
                    Mapper1RegisterA000BitPosition++;
                    if (Mapper1RegisterA000BitPosition == 5)
                    {
                        if (_map.Cartridge.ChrPages > 0)
                        {
                            if (Mapper1VromSwitchingSize == 1)
                            {
                                _map.Switch4KChrRom(Mapper1RegisterA000Value*4, 0);
                            }
                            else
                            {
                                _map.Switch8KChrRom((Mapper1RegisterA000Value >> 1)*8);
                            }
                        }
                        Mapper1RegisterA000BitPosition = 0;
                        Mapper1RegisterA000Value = 0;
                    }
                }
            }
            else if ((address >= 0xC000) && (address <= 0xDFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    Mapper1RegisterC000BitPosition = 0;
                    Mapper1RegisterC000Value = 0;
                }
                else
                {
                    Mapper1RegisterC000Value += (data & 0x1) << Mapper1RegisterC000BitPosition;
                    Mapper1RegisterC000BitPosition++;
                    if (Mapper1RegisterC000BitPosition == 5)
                    {
                        if (_map.Cartridge.ChrPages > 0)
                        {
                            if (Mapper1VromSwitchingSize == 1)
                            {
                                _map.Switch4KChrRom(Mapper1RegisterC000Value*4, 1);
                            }
                        }
                        Mapper1RegisterC000BitPosition = 0;
                        Mapper1RegisterC000Value = 0;
                    }
                }
            }
            else if ((address >= 0xE000) && (address <= 0xFFFF))
            {
                if ((data & 0x80) == 0x80)
                {
                    //Reset
                    Mapper1RegisterE000BitPosition = 0;
                    Mapper1RegisterE000Value = 0;
                    Mapper1RegisterA000BitPosition = 0;
                    Mapper1RegisterA000Value = 0;
                    Mapper1RegisterC000BitPosition = 0;
                    Mapper1RegisterC000Value = 0;
                    Mapper1Register8000BitPosition = 0;
                    Mapper1Register8000Value = 0;
                }
                else
                {
                    Mapper1RegisterE000Value += (data & 0x1) << Mapper1RegisterE000BitPosition;
                    Mapper1RegisterE000BitPosition++;
                    if (Mapper1RegisterE000BitPosition == 5)
                    {
                        if (Mapper1PRGSwitchingSize == 1)
                        {
                            if (Mapper1PRGSwitchingArea == 1)
                            {
                                // Switch bank at 0x8000 and reset 0xC000
                                _map.Switch16KPrgRom(Mapper1RegisterE000Value*4, 0);
                                _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
                            }
                            else
                            {
                                // Switch bank at 0xC000 and reset 0x8000
                                _map.Switch16KPrgRom(Mapper1RegisterE000Value*4, 1);
                                _map.Switch16KPrgRom(0, 0);
                            }
                        }
                        else
                        {
                            //Full 32k switch
                            _map.Switch32KPrgRom((Mapper1RegisterE000Value >> 1)*8);
                        }
                        Mapper1RegisterE000BitPosition = 0;
                        Mapper1RegisterE000Value = 0;
                    }
                }
            }
        }

        public void SetUpMapperDefaults()
        {
            Mapper1Register8000BitPosition = 0;
            Mapper1Register8000Value = 0;
            Mapper1MirroringFlag = 0;
            Mapper1OnePageMirroring = 1;
            Mapper1PRGSwitchingArea = 1;
            Mapper1PRGSwitchingSize = 1;
            Mapper1VromSwitchingSize = 0;
            _map.Switch16KPrgRom((_map.Cartridge.PrgPages - 1)*4, 1);
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 1 setup done.", DebugStatus.Cool);
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