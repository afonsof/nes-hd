using NesHd.Core.Debugger;
using NesHd.Core.Input;
using NesHd.Core.Memory.Mappers;

namespace NesHd.Core.Memory
{
    /// <summary>
    /// The main class for memory including RAM, IO, SRAM and PRG
    /// </summary>
    public class Memory
    {
        /*Memory shared by CPU & PPU*/

        private readonly NesEngine _engine;
        public int JoyData1;
        public int JoyData2;
        public byte JoyStrobe;
        public bool WriteOnRam = true;
        public Zapper Zapper;

        public Memory(NesEngine engine)
        {
            _engine = engine;
            InitializeMemory();
        }

        //Properties
        /// <summary>
        /// Get or set the RAM pages, addresses 0x0000 -> 0x2000
        /// </summary>
        public byte[] Ram { get; set; }

        /// <summary>
        /// Get or set the Save Ram, addresses 0x6000 -> 0x8000
        /// </summary>
        public byte[] SRam { get; set; }

        public Map Map { get; private set; }

        public Joypad Joypad1 { get; set; }

        public Joypad Joypad2 { get; set; }

        public InputManager InputManager { get; set; }

        #region Memory Read Write

        /// <summary>
        /// Write a value into the memory at the specific address
        /// </summary>
        /// <param name="address"> </param>
        /// <param name="value"> </param>
        /// <returns></returns>
        public byte Write(ushort address, byte value)
        {
            if (address < 0x2000)
            {
                if (WriteOnRam)
                    Ram[address & 0x07FF] = value;
            }
            else if (address < 0x4000) /*IO, 0x2xxx*/
            {
                switch (address)
                {
                    case 0x2000:
                        _engine.Ppu.Write2000(value);
                        break;
                    case 0x2001:
                        _engine.Ppu.Write2001(value);
                        break;
                    case 0x2003:
                        _engine.Ppu.Write2003(value);
                        break;
                    case 0x2004:
                        _engine.Ppu.Write2004(value);
                        break;
                    case 0x2005:
                        _engine.Ppu.Write2005(value);
                        break;
                    case 0x2006:
                        _engine.Ppu.Write2006(value);
                        break;
                    case 0x2007:
                        _engine.Ppu.Write2007(value);
                        break;
                }
            }
            else if (address < 0x6000) /*IO, 0x4xxx*/
            {
                switch (address)
                {
                    case 0x4000:
                        _engine.Apu.Write_4000(value);
                        break;
                    case 0x4001:
                        _engine.Apu.Write_4001(value);
                        break;
                    case 0x4002:
                        _engine.Apu.Write_4002(value);
                        break;
                    case 0x4003:
                        _engine.Apu.Write_4003(value);
                        break;
                    case 0x4004:
                        _engine.Apu.Write_4004(value);
                        break;
                    case 0x4005:
                        _engine.Apu.Write_4005(value);
                        break;
                    case 0x4006:
                        _engine.Apu.Write_4006(value);
                        break;
                    case 0x4007:
                        _engine.Apu.Write_4007(value);
                        break;
                    case 0x4008:
                        _engine.Apu.Write_4008(value);
                        break;
                    case 0x400A:
                        _engine.Apu.Write_400A(value);
                        break;
                    case 0x400B:
                        _engine.Apu.Write_400B(value);
                        break;
                    case 0x400C:
                        _engine.Apu.Write_400C(value);
                        break;
                    case 0x400E:
                        _engine.Apu.Write_400E(value);
                        break;
                    case 0x400F:
                        _engine.Apu.Write_400F(value);
                        break;
                    case 0x4010:
                        _engine.Apu.Write_4010(value);
                        break;
                    case 0x4011:
                        _engine.Apu.Write_4011(value);
                        break;
                    case 0x4012:
                        _engine.Apu.Write_4012(value);
                        break;
                    case 0x4013:
                        _engine.Apu.Write_4013(value);
                        break;
                    case 0x4014:
                        _engine.Ppu.Write4014(value);
                        break;
                    case 0x4015:
                        _engine.Apu.Write_4015(value);
                        break;
                    case 0x4016:
                        if ((JoyStrobe == 1) && ((value & 1) == 0))
                        {
                            InputManager.Update();
                            JoyData1 = Joypad1.GetJoyData();
                            JoyData2 = Joypad2.GetJoyData();
                        }
                        JoyStrobe = (byte)(value & 1);
                        break;
                    case 0x4017:
                        _engine.Apu.Write_4017(value);
                        break;
                }

                if (Map.CurrentMapper.WriteUnder6000)
                    Map.WritePrg(address, value);
            }
            else if (address < 0x8000) /*SRAM, Trainer*/
            {
                if (!Map.IsSRamReadOnly)
                    SRam[address - 0x6000] = value;
                if (Map.CurrentMapper.WriteUnder8000)
                    Map.WritePrg(address, value);
            }
            else /*PRG*/
            {
                Map.WritePrg(address, value);
            }
            return 1;
        }

        /// <summary>
        /// Read a value from the memory at a specific address
        /// </summary>
        /// <param name="address">The address to read from</param>
        /// <returns>Byte The value</returns>
        public byte Read(ushort address)
        {
            if (address < 0x2000) /*RAM*/
            {
                return Ram[address & 0x07FF];
            }
            if (address < 0x6000) /*IO*/
            {
                switch (address)
                {
                    case 0x2002:
                        return _engine.Ppu.Read2002();
                    case 0x2004:
                        return _engine.Ppu.Read2004();
                    case 0x2007:
                        return _engine.Ppu.Read2007();
                    case 0x4015:
                        return _engine.Apu.Read_4015();
                    case 0x4016:
                        {
                            var v = (byte) (0x40 | (JoyData1 & 1));
                            JoyData1 = (JoyData1 >> 1) | 0x80;
                            return v;
                        }
                    case 0x4017:
                        {
                            var v = (byte) (0x40 | Zapper.GetData() | (JoyData2 & 1));
                            JoyData2 = (JoyData2 >> 1) | 0x80;
                            return v;
                        }
                    default:
                        if (Map.Cartridge.MapperNo == 225 | Map.Cartridge.MapperNo == 255)
                        {
                            var map225 = (Mapper225255)Map.CurrentMapper;
                            return map225.ReadRegisters(address);
                        }
                        break;
                }
            }
            else if (address < 0x8000) /*SRAM*/
            {
                if (Map.Cartridge.MapperNo == 5)
                    return 1;
                return SRam[address - 0x6000];
            }
            else /*PRG*/
            {
                return Map.ReadPrg(address);
            }
            return 0;
        }

        /// <summary>
        /// Read an address from the memory
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public ushort Read16(ushort address)
        {
            byte data1;
            byte data2;
            if (address < 0x2000)
            {
                data1 = Ram[(address & 0x07FF)];
                data2 = Ram[(address & 0x07FF) + 1];
            }
            else if (address < 0x8000)
            {
                data1 = SRam[address - 0x6000];
                data2 = SRam[(address - 0x6000) + 1];
            }
            else
            {
                data1 = Map.ReadPrg(address);
                data2 = Map.ReadPrg((ushort)(address + 1));
            }
            return (ushort) ((data2 << 8) | data1);
        }

        #endregion

        /// <summary>
        /// Initialize and clear the memory
        /// </summary>
        private void InitializeMemory()
        {
            Ram = new byte[0x800];
            Map = new Map(this, _engine);
            SRam = new byte[0x2000];
            Zapper = new Zapper(_engine);
            Debug.WriteLine(this, "Memory initialized ok.", DebugStatus.Cool);
        }

        public bool LoadCart(string romPath)
        {
            var done = Map.Cartridge.LoadCart(romPath);
            if (done)
                Map.InitializeMapper();
            return done;
        }
    }
}