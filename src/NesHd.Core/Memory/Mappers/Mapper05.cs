using NesHd.Core.Debugger;

namespace NesHd.Core.Memory.Mappers
{
    /*
     * I'm not sure about this one so it's disabled....
     */

    internal class Mapper05 : IMapper
    {
        private readonly Map _map;
        public bool IrqEnabled;
        public byte IrqCounter;
        public int GraphicMode;
        public byte Mapper5ChrBankSize;
        public byte Mapper5PRGBankSize;
        public int Mapper5ScanlineSplit;
        public bool Mapper5SplitIrqEnabled;

        public Mapper05(Map map)
        {
            _map = map;
        }

        #region IMapper Members

        public void Write(ushort address, byte data)
        {
            if (address == 0x5100)
            {
                Mapper5PRGBankSize = (byte) (data & 0x3);
            }
            else if (address == 0x5101)
            {
                Mapper5ChrBankSize = (byte) (data & 0x3);
            }
            else if (address == 0x5102)
            {
                _map.Engine.Memory.WriteOnRam = (data & 0x3) == 0x02;
            }
            else if (address == 0x5103)
            {
                _map.Engine.Memory.WriteOnRam = (data & 0x3) == 0x01;
            }
            else if (address == 0x5104)
            {
                GraphicMode = (data & 0x3);
            }
            else if (address == 0x5105)
            {
                _map.SwitchVRamToVRam((data & 0x3), 0);
                _map.SwitchVRamToVRam(((data & 0xC) >> 2), 1);
                _map.SwitchVRamToVRam(((data & 0x30) >> 4), 2);
                _map.SwitchVRamToVRam(((data & 0xC0) >> 6), 3);
            }
            else if (address == 0x5106)
            {
            }
            else if (address == 0x5107)
            {
            }
            else if (address == 0x5114)
            {
                if (Mapper5PRGBankSize == 3)
                {
                    _map.Switch8KPrgRom((data & 0x7f)*2, 0);
                }
            }
            else if (address == 0x5115)
            {
                if (Mapper5PRGBankSize == 1)
                {
                    _map.Switch16KPrgRom((data & 0x7e)*2, 0);
                }
                else if (Mapper5PRGBankSize == 2)
                {
                    _map.Switch16KPrgRom((data & 0x7e)*2, 0);
                }
                else if (Mapper5PRGBankSize == 3)
                {
                    _map.Switch8KPrgRom((data & 0x7f)*2, 1);
                }
            }
            else if (address == 0x5116)
            {
                if (Mapper5PRGBankSize == 2)
                {
                    _map.Switch8KPrgRom((data & 0x7f)*2, 2);
                }
                else if (Mapper5PRGBankSize == 3)
                {
                    _map.Switch8KPrgRom((data & 0x7f)*2, 2);
                }
            }
            else if (address == 0x5117)
            {
                if (Mapper5PRGBankSize == 0)
                {
                    _map.Switch32KPrgRom((data & 0x7c)*2);
                }
                else if (Mapper5PRGBankSize == 1)
                {
                    _map.Switch16KPrgRom((data & 0x7e)*2, 1);
                }
                else if (Mapper5PRGBankSize == 2)
                {
                    _map.Switch8KPrgRom((data & 0x7f)*2, 3);
                }
                else if (Mapper5PRGBankSize == 3)
                {
                    _map.Switch8KPrgRom((data & 0x7f)*2, 3);
                }
            }
            else if (address == 0x5120)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 0);
                }
            }
            else if (address == 0x5121)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 1);
                }
                else if (Mapper5ChrBankSize == 2)
                {
                    _map.Switch2KChrRom(data, 0);
                }
            }
            else if (address == 0x5122)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 2);
                }
            }
            else if (address == 0x5123)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 3);
                }
                else if (Mapper5ChrBankSize == 2)
                {
                    _map.Switch2KChrRom(data, 1);
                }
                else if (Mapper5ChrBankSize == 1)
                {
                    _map.Switch4KChrRom(data, 0);
                }
            }
            else if (address == 0x5124)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 4);
                }
            }
            else if (address == 0x5125)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 5);
                }
                else if (Mapper5ChrBankSize == 2)
                {
                    _map.Switch2KChrRom(data, 2);
                }
            }
            else if (address == 0x5126)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 6);
                }
            }
            else if (address == 0x5127)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 7);
                }
                else if (Mapper5ChrBankSize == 2)
                {
                    _map.Switch2KChrRom(data, 3);
                }
                else if (Mapper5ChrBankSize == 1)
                {
                    _map.Switch4KChrRom(data, 1);
                }
                else if (Mapper5ChrBankSize == 0)
                {
                    _map.Switch8KChrRom(data);
                }
            }
            else if (address == 0x5128)
            {
                _map.Switch1KChrRom(data, 0);
                _map.Switch1KChrRom(data, 4);
            }
            else if (address == 0x5129)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 1);
                    _map.Switch1KChrRom(data, 5);
                }
                else if (Mapper5ChrBankSize == 2)
                {
                    _map.Switch2KChrRom(data, 0);
                    _map.Switch2KChrRom(data, 2);
                }
            }
            else if (address == 0x512a)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 2);
                    _map.Switch1KChrRom(data, 6);
                }
            }
            else if (address == 0x512b)
            {
                if (Mapper5ChrBankSize == 3)
                {
                    _map.Switch1KChrRom(data, 3);
                    _map.Switch1KChrRom(data, 7);
                }
                else if (Mapper5ChrBankSize == 2)
                {
                    _map.Switch2KChrRom(data, 1);
                    _map.Switch2KChrRom(data, 3);
                }
                else if (Mapper5ChrBankSize == 1)
                {
                    _map.Switch4KChrRom(data, 0);
                    _map.Switch4KChrRom(data, 1);
                }
                else if (Mapper5ChrBankSize == 0)
                {
                    _map.Switch8KChrRom(data);
                }
            }
            else if (address == 0x5203)
            {
                IrqCounter = data;
            }
            else if (address == 0x5204)
            {
                IrqEnabled = (data & 0x80) == 0x80;
            }
        }

        public void SetUpMapperDefaults()
        {
            _map.Switch8KPrgRom((_map.Cartridge.PrgPages*4) - 2, 0);
            _map.Switch8KPrgRom((_map.Cartridge.PrgPages*4) - 2, 1);
            _map.Switch8KPrgRom((_map.Cartridge.PrgPages*4) - 2, 2);
            _map.Switch8KPrgRom((_map.Cartridge.PrgPages*4) - 2, 3);
            Mapper5SplitIrqEnabled = false;
            _map.Switch8KChrRom(0);
            Debug.WriteLine(this, "Mapper 5 setup done.", DebugStatus.Cool);
        }

        public void TickScanlineTimer()
        {
            if (IrqEnabled)
            {
                IrqCounter--;
                if (IrqCounter <= 0)
                {
                    _map.Engine.Cpu.IRQNextTime = true;
                    IrqEnabled = false;
                }
            }
        }

        public void TickCycleTimer()
        {
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