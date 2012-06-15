/*
This file is part of My Nes
A Nintendo Entertainment System Emulator.

 Copyright © 2009 - 2010 Ala Hadid (AHD)

My Nes is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

My Nes is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

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
        public Memory(NesEngine Engine)
        {
            this._Nes = Engine;
            this.InitializeMemory();
        }
        byte[] _RAM;
        byte[] _SRAM;
        MAP _MAP;
        NesEngine _Nes;
        public int JoyData1 = 0;
        public int JoyData2 = 0;
        public byte JoyStrobe = 0;
        InputManager _inputManager;
        Joypad _joypad1;
        Joypad _joypad2;
        public Zapper ZAPPER;
        public bool WriteOnRam = true;
        #region Memory Read Write
        /// <summary>
        /// Write a value into the memory at the specific address
        /// </summary>
        /// <param name="Address">The address to write into</param>
        /// <param name="Value">The value to write</param>
        /// <returns></returns>
        public byte Write(ushort Address, byte Value)
        {
            if (Address < 0x2000)
            {
                if (this.WriteOnRam)
                    this._RAM[Address & 0x07FF] = Value;
            }
            else if (Address < 0x4000)/*IO, 0x2xxx*/
            {
                if (Address == 0x2000)
                    this._Nes.PPU.Write2000(Value);
                else if (Address == 0x2001)
                    this._Nes.PPU.Write2001(Value);
                else if (Address == 0x2003)
                    this._Nes.PPU.Write2003(Value);
                else if (Address == 0x2004)
                    this._Nes.PPU.Write2004(Value);
                else if (Address == 0x2005)
                    this._Nes.PPU.Write2005(Value);
                else if (Address == 0x2006)
                    this._Nes.PPU.Write2006(Value);
                else if (Address == 0x2007)
                    this._Nes.PPU.Write2007(Value);
            }
            else if (Address < 0x6000)/*IO, 0x4xxx*/
            {
                if (Address == 0x4000)
                    this._Nes.APU.Write_4000(Value);
                else if (Address == 0x4001)
                    this._Nes.APU.Write_4001(Value);
                else if (Address == 0x4002)
                    this._Nes.APU.Write_4002(Value);
                else if (Address == 0x4003)
                    this._Nes.APU.Write_4003(Value);
                else if (Address == 0x4004)
                    this._Nes.APU.Write_4004(Value);
                else if (Address == 0x4005)
                    this._Nes.APU.Write_4005(Value);
                else if (Address == 0x4006)
                    this._Nes.APU.Write_4006(Value);
                else if (Address == 0x4007)
                    this._Nes.APU.Write_4007(Value);
                else if (Address == 0x4008)
                    this._Nes.APU.Write_4008(Value);
                else if (Address == 0x400A)
                    this._Nes.APU.Write_400A(Value);
                else if (Address == 0x400B)
                    this._Nes.APU.Write_400B(Value);
                else if (Address == 0x400C)
                    this._Nes.APU.Write_400C(Value);
                else if (Address == 0x400E)
                    this._Nes.APU.Write_400E(Value);
                else if (Address == 0x400F)
                    this._Nes.APU.Write_400F(Value);
                else if (Address == 0x4010)
                    this._Nes.APU.Write_4010(Value);
                else if (Address == 0x4011)
                    this._Nes.APU.Write_4011(Value);
                else if (Address == 0x4012)
                    this._Nes.APU.Write_4012(Value);
                else if (Address == 0x4013)
                    this._Nes.APU.Write_4013(Value);
                else if (Address == 0x4014)
                    this._Nes.PPU.Write4014(Value);
                else if (Address == 0x4015)
                    this._Nes.APU.Write_4015(Value);
                else if (Address == 0x4016)
                {
                    if ((this.JoyStrobe == 1) && ((Value & 1) == 0))
                    {
                        this._inputManager.Update();
                        this.JoyData1 = this._joypad1.GetJoyData();
                        this.JoyData2 = this._joypad2.GetJoyData();
                    }
                    this.JoyStrobe = (byte)(Value & 1);
                }
                else if (Address == 0x4017)
                    this._Nes.APU.Write_4017(Value);

                if (this._MAP.CurrentMapper.WriteUnder6000)
                    this._MAP.WritePRG(Address, Value);
            }
            else if (Address < 0x8000)/*SRAM, Trainer*/
            {
                if (!this._MAP.IsSRAMReadOnly)
                    this._SRAM[Address - 0x6000] = Value;
                if (this._MAP.CurrentMapper.WriteUnder8000)
                    this._MAP.WritePRG(Address, Value);
            }
            else/*PRG*/
            {
                this._MAP.WritePRG(Address, Value);
            }
            return 1;
        }
        /// <summary>
        /// Read a value from the memory at a specific address
        /// </summary>
        /// <param name="Address">The address to read from</param>
        /// <returns>Byte The value</returns>
        public byte Read(ushort Address)
        {
            if (Address < 0x2000)/*RAM*/
            {
                return this._RAM[Address & 0x07FF];
            }
            else if (Address < 0x6000)/*IO*/
            {
                if (Address == 0x2002)
                    return this._Nes.PPU.Read2002();
                else if (Address == 0x2004)
                    return this._Nes.PPU.Read2004();
                else if (Address == 0x2007)
                    return this._Nes.PPU.Read2007();
                else if (Address == 0x4015)
                    return this._Nes.APU.Read_4015();
                else if (Address == 0x4016)
                {
                    byte v = (byte)(0x40 | (this.JoyData1 & 1));
                    this.JoyData1 = (this.JoyData1 >> 1) | 0x80;
                    return v;
                }
                else if (Address == 0x4017)
                {
                    byte v = (byte)(0x40 | this.ZAPPER.GetData() | (this.JoyData2 & 1));
                    this.JoyData2 = (this.JoyData2 >> 1) | 0x80;
                    return v;
                }
                else if (this._MAP.Cartridge.MapperNo == 225 | this._MAP.Cartridge.MapperNo == 255)
                {
                    Mapper225_255 map225 = (Mapper225_255)this._MAP.CurrentMapper;
                    return map225.ReadRegisters(Address);
                }
            }
            else if (Address < 0x8000)/*SRAM*/
            {
                if (this._MAP.Cartridge.MapperNo == 5)
                    return 1;
                return this._SRAM[Address - 0x6000];
            }
            else/*PRG*/
            {
                return this._MAP.ReadPRG(Address);
            }
            return 0;
        }
        /// <summary>
        /// Read an address from the memory
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public ushort Read16(ushort Address)
        {
            byte data_1 = 0;
            byte data_2 = 0;
            if (Address < 0x2000)
            {
                data_1 = this._RAM[(Address & 0x07FF)];
                data_2 = this._RAM[(Address & 0x07FF) + 1];
            }
            else if (Address < 0x8000)
            {
                data_1 = this._SRAM[Address - 0x6000];
                data_2 = this._SRAM[(Address - 0x6000) + 1];
            }
            else
            {
                data_1 = this._MAP.ReadPRG(Address);
                data_2 = this._MAP.ReadPRG((ushort)(Address + 1));
            }
            return (ushort)((data_2 << 8) | data_1);
        }
        #endregion
        /// <summary>
        /// Initialize and clear the memory
        /// </summary>
        void InitializeMemory()
        {
            this._RAM = new byte[0x800];
            this._MAP = new MAP(this, this._Nes);
            this._SRAM = new byte[0x2000];
            this.ZAPPER = new Zapper(this._Nes);
            Debug.WriteLine(this, "Memory initialized ok.", DebugStatus.Cool);
        }
        public bool LoadCart(string RomPath)
        {
            bool Done = this._MAP.Cartridge.LoadCart(RomPath);
            if (Done)
                this._MAP.InitializeMapper();
            return Done;
        }
        //Properties
        /// <summary>
        /// Get or set the RAM pages, addresses 0x0000 -> 0x2000
        /// </summary>
        public byte[] RAM
        { get { return this._RAM; } set { this._RAM = value; } }
        /// <summary>
        /// Get or set the Save Ram, addresses 0x6000 -> 0x8000
        /// </summary>
        public byte[] SRAM
        { get { return this._SRAM; } set { this._SRAM = value; } }
        public MAP MAP
        { get { return this._MAP; } }
        public Joypad Joypad1
        { get { return this._joypad1; } set { this._joypad1 = value; } }
        public Joypad Joypad2
        { get { return this._joypad2; } set { this._joypad2 = value; } }
        public InputManager InputManager
        { get { return this._inputManager; } set { this._inputManager = value; } }
    }
}