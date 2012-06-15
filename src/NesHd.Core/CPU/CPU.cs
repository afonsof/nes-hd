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

using System;
using System.Threading;

using NesHd.Core.Debugger;

namespace NesHd.Core.CPU
{
    /// <summary>
    /// The 6502
    /// </summary>
    public class CPU
    {
        /// <summary>
        /// The 6502
        /// </summary>
        /// <param name="Memory">The memory</param>
        /// <param name="TvFormat">The TV format</param>
        public CPU(Memory.Memory Memory, TVFORMAT TvFormat, NesEngine Nes)
        {
            this._Nes = Nes;
            this.MEM = Memory;
            this.InitializeCPU(TvFormat);
        }
        //Registers
        public byte REG_A = 0;
        public byte REG_X = 0;
        public byte REG_Y = 0;
        public byte REG_S = 0;
        public ushort REG_PC = 0;
        //Flags
        public bool Flag_N = false;
        public bool Flag_V = false;
        public bool Flag_B = false;
        public bool Flag_D = false;
        public bool Flag_I = true;
        public bool Flag_Z = false;
        public bool Flag_C = false;
        public bool IRQNextTime = false;
        //Clocks and timing stuff
        public int CycleCounter = 0;
        public int CyclesPerScanline = 0;
        //Others
        Memory.Memory MEM;
        NesEngine _Nes;
        bool _ON = false;
        bool _Pause = false;
        bool Paused = false;//To ensure the cpu is paused
        public byte OpCode = 0;
        public ushort PrevPC = 0;

        /// <summary>
        /// Run the cpu looping
        /// </summary>
        public void Run()
        {
            this.PrevPC = this.REG_PC;
            while (this._ON)
            {
                this.OpCode = this.MEM.Read(this.REG_PC);
                if (!this._Pause)
                {
                    this.Paused = false;
                    #region DO OPCODE
                    // We may not use both, but it's easier to grab them now
                    byte arg1 = this.MEM.Read((ushort)(this.REG_PC + 1));
                    byte arg2 = this.MEM.Read((ushort)(this.REG_PC + 2));
                    byte M = 0xFF;//The value holder
                    switch (this.OpCode)
                    {
                        #region ADC
                        case (0x61):
                            M = this.IndirectX(arg1);
                            this.ADC(M, 6, 2);
                            break;
                        case (0x65):
                            M = this.ZeroPage(arg1);
                            this.ADC(M, 3, 2);
                            break;
                        case (0x69):
                            M = arg1;
                            this.ADC(M, 2, 2);
                            break;
                        case (0x6D):
                            M = this.Absolute(arg1, arg2);
                            this.ADC(M, 4, 3);
                            break;
                        case (0x71):
                            M = this.IndirectY(arg1, true);
                            this.ADC(M, 5, 2);
                            break;
                        case (0x75):
                            M = this.ZeroPageX(arg1);
                            this.ADC(M, 4, 2);
                            break;
                        case (0x79):
                            M = this.AbsoluteY(arg1, arg2, true);
                            this.ADC(M, 4, 3);
                            break;
                        case (0x7D):
                            M = this.AbsoluteX(arg1, arg2, true);
                            this.ADC(M, 4, 3);
                            break;
                        #endregion
                        #region AND
                        case (0x21):
                            M = this.IndirectX(arg1);
                            this.AND(M, 6, 2);
                            break;
                        case (0x25):
                            M = this.ZeroPage(arg1);
                            this.AND(M, 3, 2);
                            break;
                        case (0x29):
                            M = arg1;
                            this.AND(M, 2, 2);
                            break;
                        case (0x2D):
                            M = this.Absolute(arg1, arg2);
                            this.AND(M, 3, 3);
                            break;
                        case (0x31):
                            M = this.IndirectY(arg1, false);
                            this.AND(M, 5, 2);
                            break;
                        case (0x35):
                            M = this.ZeroPageX(arg1);
                            this.AND(M, 4, 2);
                            break;
                        case (0x39):
                            M = this.AbsoluteY(arg1, arg2, true);
                            this.AND(M, 4, 3);
                            break;
                        case (0x3D):
                            M = this.AbsoluteX(arg1, arg2, true);
                            this.AND(M, 4, 3);
                            break;
                        #endregion
                        #region ASL
                        case (0x06):
                            M = this.ZeroPage(arg1);
                            this.ZeroPageWrite(arg1, this.ASL(M));
                            this.CycleCounter += 5;
                            this.REG_PC += 2;
                            break;
                        case (0x0A):
                            M = this.REG_A;
                            this.REG_A = this.ASL(M);
                            this.CycleCounter += 2;
                            this.REG_PC += 1;
                            break;
                        case (0x0E):
                            M = this.Absolute(arg1, arg2);
                            this.AbsoluteWrite(arg1, arg2, this.ASL(M));
                            this.CycleCounter += 6;
                            this.REG_PC += 3;
                            break;
                        case (0x16):
                            M = this.ZeroPageX(arg1);
                            this.ZeroPageXWrite(arg1, this.ASL(M));
                            this.CycleCounter += 6;
                            this.REG_PC += 2;
                            break;
                        case (0x1E):
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.AbsoluteXWrite(arg1, arg2, this.ASL(M));
                            this.CycleCounter += 7;
                            this.REG_PC += 3;
                            break;
                        #endregion
                        #region BCC
                        case (0x90): this.BCC(arg1); break;
                        #endregion
                        #region BCS
                        case (0xb0): this.BCS(arg1); break;
                        #endregion
                        #region BEQ
                        case (0xf0): this.BEQ(arg1); break;
                        #endregion
                        #region BIT
                        case (0x24):
                            M = this.ZeroPage(arg1);
                            this.BIT(M, 3, 2);
                            break;
                        case (0x2c):
                            M = this.Absolute(arg1, arg2);
                            this.BIT(M, 4, 3);
                            break;
                        #endregion
                        #region BMI
                        case (0x30): this.BMI(arg1); break;
                        #endregion
                        #region BNE
                        case (0xd0): this.BNE(arg1); break;
                        #endregion
                        #region BPL
                        case (0x10): this.BPL(arg1); break;
                        #endregion
                        #region BRK
                        case (0x00): this.BRK(); break;
                        #endregion
                        #region BVC
                        case (0x50): this.BVC(arg1); break;
                        #endregion
                        #region BVS
                        case (0x70): this.BVS(arg1); break;
                        #endregion
                        #region CLC
                        case (0x18): this.CLC(); break;
                        #endregion
                        #region CLD
                        case (0xd8): this.CLD(); break;
                        #endregion
                        #region CLI
                        case (0x58): this.CLI(); break;
                        #endregion
                        #region CLV
                        case (0xb8): this.CLV(); break;
                        #endregion
                        #region CMP
                        case (0xC1):
                            M = this.IndirectX(arg1);
                            this.CMP(M, 6, 2);
                            break;
                        case (0xC5):
                            M = this.ZeroPage(arg1);
                            this.CMP(M, 3, 2);
                            break;
                        case (0xC9):
                            M = arg1;
                            this.CMP(M, 2, 2);
                            break;
                        case (0xCD):
                            M = this.Absolute(arg1, arg2);
                            this.CMP(M, 4, 3);
                            break;
                        case (0xd1):
                            M = this.IndirectY(arg1, true);
                            this.CMP(M, 5, 2);
                            break;
                        case (0xd5):
                            M = this.ZeroPageX(arg1);
                            this.CMP(M, 4, 2);
                            break;
                        case (0xd9):
                            M = this.AbsoluteY(arg1, arg2, true);
                            this.CMP(M, 4, 3);
                            break;
                        case (0xdd):
                            M = this.AbsoluteX(arg1, arg2, true);
                            this.CMP(M, 4, 3);
                            break;
                        #endregion
                        #region CPX
                        case (0xE0):
                            M = arg1;
                            this.CPX(M, 2, 2);
                            break;
                        case (0xE4):
                            M = this.ZeroPage(arg1);
                            this.CPX(M, 3, 2);
                            break;
                        case (0xEC):
                            M = this.Absolute(arg1, arg2);
                            this.CPX(M, 4, 3);
                            break;
                        #endregion
                        #region CPY
                        case (0xc0):
                            M = arg1;
                            this.CPY(M, 2, 2);
                            break;
                        case (0xc4):
                            M = this.ZeroPage(arg1);
                            this.CPY(M, 3, 2);
                            break;
                        case (0xcc):
                            M = this.Absolute(arg1, arg2);
                            this.CPY(M, 4, 3);
                            break;
                        #endregion
                        #region DEC
                        case (0xc6):
                            M = this.ZeroPage(arg1);
                            this.ZeroPageWrite(arg1, this.DEC(M));
                            this.CycleCounter += 5;
                            this.REG_PC += 2;
                            break;
                        case (0xce):
                            M = this.Absolute(arg1, arg2);
                            this.AbsoluteWrite(arg1, arg2, this.DEC(M));
                            this.CycleCounter += 6;
                            this.REG_PC += 3;
                            break;
                        case (0xd6):
                            M = this.ZeroPageX(arg1);
                            this.ZeroPageXWrite(arg1, this.DEC(M));
                            this.CycleCounter += 6;
                            this.REG_PC += 2;
                            break;
                        case (0xde):
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.AbsoluteXWrite(arg1, arg2, this.DEC(M));
                            this.CycleCounter += 7;
                            this.REG_PC += 3;
                            break;
                        #endregion
                        #region DEX
                        case (0xca): this.DEX(); break;
                        #endregion
                        #region DEY
                        case (0x88): this.DEY(); break;
                        #endregion
                        #region EOR
                        case (0x41):
                            M = this.IndirectX(arg1);
                            this.EOR(M, 6, 2);
                            break;
                        case (0x45):
                            M = this.ZeroPage(arg1);
                            this.EOR(M, 3, 2);
                            break;
                        case (0x49):
                            M = arg1;
                            this.EOR(M, 2, 2);
                            break;
                        case (0x4d):
                            M = this.Absolute(arg1, arg2);
                            this.EOR(M, 3, 3);
                            break;
                        case (0x51):
                            M = this.IndirectY(arg1, true);
                            this.EOR(M, 5, 2);
                            break;
                        case (0x55):
                            M = this.ZeroPageX(arg1);
                            this.EOR(M, 4, 2);
                            break;
                        case (0x59):
                            M = this.AbsoluteY(arg1, arg2, true);
                            this.EOR(M, 4, 3);
                            break;
                        case (0x5d):
                            M = this.AbsoluteX(arg1, arg2, true);
                            this.EOR(M, 4, 3);
                            break;
                        #endregion
                        #region INC
                        case (0xe6):
                            M = this.ZeroPage(arg1);
                            this.ZeroPageWrite(arg1, this.INC(M));
                            this.CycleCounter += 5;
                            this.REG_PC += 2;
                            break;
                        case (0xee):
                            M = this.Absolute(arg1, arg2);
                            this.AbsoluteWrite(arg1, arg2, this.INC(M));
                            this.CycleCounter += 6;
                            this.REG_PC += 3;
                            break;
                        case (0xf6):
                            M = this.ZeroPageX(arg1);
                            this.ZeroPageXWrite(arg1, this.INC(M));
                            this.CycleCounter += 6;
                            this.REG_PC += 2;
                            break;
                        case (0xfe):
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.AbsoluteXWrite(arg1, arg2, this.INC(M));
                            this.CycleCounter += 7;
                            this.REG_PC += 3;
                            break;
                        #endregion
                        #region INX
                        case (0xe8): this.INX(); break;
                        #endregion
                        #region INY
                        case (0xc8): this.INY(); break;
                        #endregion
                        #region JMP
                        case (0x4c):
                            this.REG_PC = this.MEM.Read16((ushort)(this.REG_PC + 1));
                            this.CycleCounter += 3;
                            break;
                        case (0x6c):
                            ushort myAddress = this.MEM.Read16((ushort)(this.REG_PC + 1));
                            if ((myAddress & 0x00FF) == 0x00FF)
                            {
                                this.REG_PC = this.MEM.Read(myAddress);
                                myAddress &= 0xFF00;
                                this.REG_PC |= (ushort)((this.MEM.Read(myAddress)) << 8);
                            }
                            else
                                this.REG_PC = this.MEM.Read16(myAddress);
                            this.CycleCounter += 5;
                            break;
                        #endregion
                        #region JSR
                        case (0x20): this.JSR(arg1, arg2); break;
                        #endregion
                        #region LDA
                        case (0xa1):
                            this.REG_A = this.IndirectX(arg1);
                            this.CycleCounter += 6; this.REG_PC += 2;
                            this.LDA();
                            break;
                        case (0xa5):
                            this.REG_A = this.ZeroPage(arg1);
                            this.CycleCounter += 3; this.REG_PC += 2;
                            this.LDA();
                            break;
                        case (0xa9):
                            this.REG_A = arg1;
                            this.CycleCounter += 2; this.REG_PC += 2;
                            this.LDA(); break;
                        case (0xad):
                            this.REG_A = this.Absolute(arg1, arg2);
                            this.CycleCounter += 4; this.REG_PC += 3;
                            this.LDA(); break;
                        case (0xb1):
                            this.REG_A = this.IndirectY(arg1, true);
                            this.CycleCounter += 5; this.REG_PC += 2;
                            this.LDA();
                            break;
                        case (0xb5):
                            this.REG_A = this.ZeroPageX(arg1);
                            this.CycleCounter += 4; this.REG_PC += 2;
                            this.LDA();
                            break;
                        case (0xb9):
                            this.REG_A = this.AbsoluteY(arg1, arg2, true);
                            this.CycleCounter += 4; this.REG_PC += 3;
                            this.LDA(); break;
                        case (0xbd):
                            this.REG_A = this.AbsoluteX(arg1, arg2, true);
                            this.CycleCounter += 4; this.REG_PC += 3;
                            this.LDA(); break;
                        #endregion
                        #region LDX
                        case (0xa2):
                            this.REG_X = arg1;
                            this.CycleCounter += 2; this.REG_PC += 2;
                            this.LDX(); break;
                        case (0xa6):
                            this.REG_X = this.ZeroPage(arg1);
                            this.CycleCounter += 3; this.REG_PC += 2;
                            this.LDX(); break;
                        case (0xae):
                            this.REG_X = this.Absolute(arg1, arg2);
                            this.CycleCounter += 4; this.REG_PC += 3;
                            this.LDX(); break;
                        case (0xb6):
                            this.REG_X = this.ZeroPageY(arg1);
                            this.CycleCounter += 4; this.REG_PC += 2;
                            this.LDX(); break;
                        case (0xbe): this.REG_X = this.AbsoluteY(arg1, arg2, true);
                            this.CycleCounter += 4; this.REG_PC += 3;
                            this.LDX(); break;
                        #endregion
                        #region LDY
                        case (0xa0):
                            this.REG_Y = arg1;
                            this.CycleCounter += 2; this.REG_PC += 2;
                            this.LDY(); break;
                        case (0xa4):
                            this.REG_Y = this.ZeroPage(arg1);
                            this.CycleCounter += 3; this.REG_PC += 2;
                            this.LDY(); break;
                        case (0xac):
                            this.REG_Y = this.Absolute(arg1, arg2);
                            this.CycleCounter += 4; this.REG_PC += 3;
                            this.LDY(); break;
                        case (0xb4):
                            this.REG_Y = this.ZeroPageX(arg1);
                            this.CycleCounter += 4; this.REG_PC += 2;
                            this.LDY(); break;
                        case (0xbc):
                            this.REG_Y = this.AbsoluteX(arg1, arg2, true);
                            this.CycleCounter += 4; this.REG_PC += 3;
                            this.LDY(); break;
                        #endregion
                        #region LSR
                        case (0x46):
                            M = this.ZeroPage(arg1);
                            this.ZeroPageWrite(arg1, this.LSR(M));
                            this.CycleCounter += 5; this.REG_PC += 2;
                            break;
                        case (0x4a):
                            M = this.REG_A;
                            this.REG_A = this.LSR(M);
                            this.CycleCounter += 2; this.REG_PC += 1; break;
                        case (0x4e):
                            M = this.Absolute(arg1, arg2);
                            this.AbsoluteWrite(arg1, arg2, this.LSR(M));
                            this.CycleCounter += 6; this.REG_PC += 3;
                            break;
                        case (0x56): M = this.ZeroPageX(arg1);
                            this.ZeroPageXWrite(arg1, this.LSR(M));
                            this.CycleCounter += 6; this.REG_PC += 2; break;
                        case (0x5e):
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.AbsoluteXWrite(arg1, arg2, this.LSR(M));
                            this.CycleCounter += 7; this.REG_PC += 3;
                            break;
                        #endregion
                        #region NOP
                        case (0xEA): this.NOP(); break;
                        #endregion
                        #region ORA
                        case (0x01):
                            M = this.IndirectX(arg1);
                            this.ORA(M, 6, 2);
                            break;
                        case (0x05):
                            M = this.ZeroPage(arg1);
                            this.ORA(M, 3, 2);
                            break;
                        case (0x09):
                            M = arg1;
                            this.ORA(M, 2, 2);
                            break;
                        case (0x0d): M = this.Absolute(arg1, arg2);
                            this.ORA(M, 4, 3);
                            break;
                        case (0x11):
                            M = this.IndirectY(arg1, false);
                            this.ORA(M, 5, 2);
                            break;
                        case (0x15):
                            M = this.ZeroPageX(arg1);
                            this.ORA(M, 4, 2);
                            break;
                        case (0x19):
                            M = this.AbsoluteY(arg1, arg2, true);
                            this.ORA(M, 4, 3);
                            break;
                        case (0x1d):
                            M = this.AbsoluteX(arg1, arg2, true);
                            this.ORA(M, 4, 3);
                            break;
                        #endregion
                        #region PHA
                        case (0x48): this.PHA(); break;
                        #endregion
                        #region PHP
                        case (0x08): this.PHP(); break;
                        #endregion
                        #region PLA
                        case (0x68): this.PLA(); break;
                        #endregion
                        #region PLP
                        case (0x28): this.PLP(); break;
                        #endregion
                        #region ROL
                        case (0x26):
                            M = this.ZeroPage(arg1);
                            this.ZeroPageWrite(arg1, this.ROL(M));
                            this.CycleCounter += 5; this.REG_PC += 2;
                            break;
                        case (0x2a):
                            M = this.REG_A;
                            this.REG_A = this.ROL(M);
                            this.CycleCounter += 2; this.REG_PC += 1;
                            break;
                        case (0x2e):
                            M = this.Absolute(arg1, arg2);
                            this.AbsoluteWrite(arg1, arg2, this.ROL(M));
                            this.CycleCounter += 6; this.REG_PC += 3; break;
                        case (0x36):
                            M = this.ZeroPageX(arg1);
                            this.ZeroPageXWrite(arg1, this.ROL(M));
                            this.CycleCounter += 6; this.REG_PC += 2;
                            break;
                        case (0x3e):
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.AbsoluteXWrite(arg1, arg2, this.ROL(M));
                            this.CycleCounter += 7; this.REG_PC += 3; break;
                        #endregion
                        #region ROR
                        case (0x66):
                            M = this.ZeroPage(arg1);
                            this.ZeroPageWrite(arg1, this.ROR(M));
                            this.CycleCounter += 5; this.REG_PC += 2; break;
                        case (0x6a):
                            M = this.REG_A;
                            this.REG_A = this.ROR(M);
                            this.CycleCounter += 2; this.REG_PC += 1;
                            break;
                        case (0x6e):
                            M = this.Absolute(arg1, arg2);
                            this.AbsoluteWrite(arg1, arg2, this.ROR(M));
                            this.CycleCounter += 6; this.REG_PC += 3; break;
                        case (0x76):
                            M = this.ZeroPageX(arg1);
                            this.ZeroPageXWrite(arg1, this.ROR(M));
                            this.CycleCounter += 6; this.REG_PC += 2; break;
                        case (0x7e):
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.AbsoluteXWrite(arg1, arg2, this.ROR(M));
                            this.CycleCounter += 7; this.REG_PC += 3; break;
                        #endregion
                        #region RIT
                        case (0x40): this.RTI(); break;
                        #endregion
                        #region RTS
                        case (0x60): this.RTS(); break;
                        #endregion
                        #region SBC
                        case (0xe1):
                            M = this.IndirectX(arg1);
                            this.SBC(M, 6, 2);
                            break;
                        case (0xe5):
                            M = this.ZeroPage(arg1);
                            this.SBC(M, 3, 2);
                            break;
                        case (0xe9):
                            M = arg1;
                            this.SBC(M, 2, 2);
                            break;
                        case (0xed):
                            M = this.Absolute(arg1, arg2);
                            this.SBC(M, 4, 3);
                            break;
                        case (0xf1):
                            M = this.IndirectY(arg1, false);
                            this.SBC(M, 5, 2);
                            break;
                        case (0xf5):
                            M = this.ZeroPageX(arg1);
                            this.SBC(M, 4, 2);
                            break;
                        case (0xf9):
                            M = this.AbsoluteY(arg1, arg2, true);
                            this.SBC(M, 4, 3);
                            break;
                        case (0xfd):
                            M = this.AbsoluteX(arg1, arg2, true);
                            this.SBC(M, 4, 3);
                            break;
                        #endregion
                        #region SEC
                        case (0x38): this.SEC(); break;
                        #endregion
                        #region SED
                        case (0xf8): this.SED(); break;
                        #endregion
                        #region SEI
                        case (0x78): this.SEI(); break;
                        #endregion
                        #region STA
                        case (0x85): this.ZeroPageWrite(arg1, this.REG_A);
                            this.CycleCounter += 3; this.REG_PC += 2; break;
                        case (0x95): this.ZeroPageXWrite(arg1, this.REG_A);
                            this.CycleCounter += 4; this.REG_PC += 2; break;
                        case (0x8D): this.AbsoluteWrite(arg1, arg2, this.REG_A);
                            this.CycleCounter += 4; this.REG_PC += 3; break;
                        case (0x9D): this.AbsoluteXWrite(arg1, arg2, this.REG_A);
                            this.CycleCounter += 5; this.REG_PC += 3; break;
                        case (0x99): this.AbsoluteYWrite(arg1, arg2, this.REG_A);
                            this.CycleCounter += 5; this.REG_PC += 3; break;
                        case (0x81): this.IndirectXWrite(arg1, this.REG_A);
                            this.CycleCounter += 6; this.REG_PC += 2; break;
                        case (0x91): this.IndirectYWrite(arg1, this.REG_A);
                            this.CycleCounter += 6; this.REG_PC += 2; break;
                        #endregion
                        #region STX
                        case (0x86): this.ZeroPageWrite(arg1, this.REG_X);
                            this.CycleCounter += 3; this.REG_PC += 2; break;
                        case (0x96): this.ZeroPageYWrite(arg1, this.REG_X);
                            this.CycleCounter += 4; this.REG_PC += 2; break;
                        case (0x8E): this.AbsoluteWrite(arg1, arg2, this.REG_X);
                            this.CycleCounter += 4; this.REG_PC += 3; break;
                        #endregion
                        #region STY
                        case (0x84): this.ZeroPageWrite(arg1, this.REG_Y);
                            this.CycleCounter += 3; this.REG_PC += 2; break;
                        case (0x94): this.ZeroPageXWrite(arg1, this.REG_Y);
                            this.CycleCounter += 4; this.REG_PC += 2; break;
                        case (0x8C): this.AbsoluteWrite(arg1, arg2, this.REG_Y);
                            this.CycleCounter += 4; this.REG_PC += 3; break;
                        #endregion
                        #region TAX
                        case (0xaa): this.TAX(); break;
                        #endregion
                        #region TAY
                        case (0xa8): this.TAY(); break;
                        #endregion
                        #region TSX
                        case (0xba): this.TSX(); break;
                        #endregion
                        #region TXA
                        case (0x8a): this.TXA(); break;
                        #endregion
                        #region TXS
                        case (0x9a): this.TXS(); break;
                        #endregion
                        #region TYA
                        case (0x98): this.TYA(); break;
                        #endregion
                        /*Illegal Opcodes*/
                        #region AAC
                        case 0x0B:
                        case 0x2B:
                            M = arg1;
                            this.AAC(M, 2, 2);
                            break;
                        #endregion
                        #region AAX
                        case 0x87:
                            M = this.ZeroPage(arg1);
                            this.ZeroPageWrite(arg1, this.AAX(M));
                            this.REG_PC += 2;
                            this.CycleCounter += 3;
                            break;
                        case 0x97:
                            M = this.ZeroPageY(arg1);
                            this.ZeroPageYWrite(arg1, this.AAX(M));
                            this.REG_PC += 2;
                            this.CycleCounter += 4;
                            break;
                        case 0x83:
                            M = this.IndirectX(arg1);
                            this.IndirectXWrite(arg1, this.AAX(M));
                            this.REG_PC += 2;
                            this.CycleCounter += 6;
                            break;
                        case 0x8F:
                            M = this.Absolute(arg1, arg2);
                            this.AbsoluteWrite(arg1, arg2, this.AAX(M));
                            this.REG_PC += 3;
                            this.CycleCounter += 4;
                            break;
                        #endregion
                        #region ARR
                        case 0x6B:
                            M = arg1;
                            this.ARR(M);
                            break;
                        #endregion
                        #region ASR
                        case 0x4B:
                            M = arg1;
                            this.ASR(M);
                            break;
                        #endregion
                        #region ATX
                        case 0xAB:
                            M = arg1;
                            this.ATX(M);
                            break;
                        #endregion
                        #region AXA
                        case 0x9F:
                            M = this.AbsoluteY(arg1, arg2, false);
                            this.AbsoluteYWrite(arg1, arg2, this.AXA(M));
                            this.REG_PC += 3;
                            this.CycleCounter += 5;
                            break;
                        case 0x93:
                            M = this.IndirectY(arg1, false);
                            this.IndirectYWrite(arg1, this.AXA(M));
                            this.REG_PC += 2;
                            this.CycleCounter += 6;
                            break;
                        #endregion
                        #region AXS
                        case 0xCB: M = arg1; this.AXS(M); break;
                        #endregion
                        #region DCP
                        case 0xC7:
                            M = this.ZeroPage(arg1);
                            this.DCP(M, 5, 2);
                            break;
                        case 0xD7:
                            M = this.ZeroPageX(arg1);
                            this.DCP(M, 6, 2);
                            break;
                        case 0xCF:
                            M = this.Absolute(arg1, arg2);
                            this.DCP(M, 6, 3);
                            break;
                        case 0xDF:
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.DCP(M, 7, 3);
                            break;
                        case 0xDB:
                            M = this.AbsoluteY(arg1, arg2, false);
                            this.DCP(M, 7, 3);
                            break;
                        case 0xC3:
                            M = this.IndirectX(arg1);
                            this.DCP(M, 8, 2);
                            break;
                        case 0xD3:
                            M = this.IndirectY(arg1, false);
                            this.DCP(M, 8, 2);
                            break;
                        #endregion
                        #region DOP
                        case 0x14:
                        case 0x54:
                        case 0x74:
                        case 0xD4:
                        case 0xF4:
                        case 0x34: this.DOP(4, 2); break;
                        case 0x04:
                        case 0x64:
                        case 0x44: this.DOP(3, 2); break;
                        case 0x82:
                        case 0x89:
                        case 0xC2:
                        case 0xE2:
                        case 0x80: this.DOP(2, 2); break;
                        #endregion
                        #region ISC
                        case 0xE7:
                            M = this.ZeroPage(arg1);
                            this.ISC(M, 5, 2);
                            break;
                        case 0xF7:
                            M = this.ZeroPageX(arg1);
                            this.ISC(M, 6, 2);
                            break;
                        case 0xEF:
                            M = this.Absolute(arg1, arg2);
                            this.ISC(M, 6, 3);
                            break;
                        case 0xFF:
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.ISC(M, 7, 3);
                            break;
                        case 0xFB:
                            M = this.AbsoluteY(arg1, arg2, false);
                            this.ISC(M, 7, 3);
                            break;
                        case 0xE3:
                            M = this.IndirectX(arg1);
                            this.ISC(M, 8, 2);
                            break;
                        case 0xF3:
                            M = this.IndirectY(arg1, false);
                            this.ISC(M, 8, 2);
                            break;
                        #endregion
                        #region KIL
                        case 0x02:
                        case 0x12:
                        case 0x22:
                        case 0x32:
                        case 0x42:
                        case 0x52:
                        case 0x62:
                        case 0x72:
                        case 0x92:
                        case 0xB2:
                        case 0xD2:
                        case 0xF2: this.KIL(); break;
                        #endregion
                        #region LAR
                        case 0xBB:
                            M = this.AbsoluteY(arg1, arg2, true);
                            this.LAR(M, 4, 2);
                            break;
                        #endregion
                        #region LAX
                        case 0xA7:
                            M = this.ZeroPage(arg1);
                            this.LAX(M, 3, 2);
                            break;
                        case 0xB7:
                            M = this.ZeroPageY(arg1);
                            this.LAX(M, 4, 2);
                            break;
                        case 0xAF:
                            M = this.Absolute(arg1, arg2);
                            this.LAX(M, 4, 3);
                            break;
                        case 0xBF:
                            M = this.AbsoluteY(arg1, arg2, true);
                            this.LAX(M, 4, 3);
                            break;
                        case 0xA3:
                            M = this.IndirectX(arg1);
                            this.LAX(M, 6, 2);
                            break;
                        case 0xB3:
                            M = this.IndirectY(arg1, true);
                            this.LAX(M, 5, 2);
                            break;
                        #endregion
                        #region NOP
                        case 0x1A:
                        case 0x3A:
                        case 0x5A:
                        case 0x7A:
                        case 0xDA:
                        case 0xFA: this.NOP(); break;
                        #endregion
                        #region RLA
                        case 0x27:
                            M = this.ZeroPage(arg1);
                            this.RLA(M, 5, 2);
                            break;
                        case 0x37:
                            M = this.ZeroPageX(arg1);
                            this.RLA(M, 6, 2);
                            break;
                        case 0x2F:
                            M = this.Absolute(arg1, arg2);
                            this.RLA(M, 6, 3);
                            break;
                        case 0x3F:
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.RLA(M, 7, 3);
                            break;
                        case 0x3B:
                            M = this.AbsoluteY(arg1, arg2, false);
                            this.RLA(M, 7, 3);
                            break;
                        case 0x23:
                            M = this.IndirectX(arg1);
                            this.RLA(M, 8, 2);
                            break;
                        case 0x33:
                            M = this.IndirectY(arg1, false);
                            this.RLA(M, 8, 2);
                            break;
                        #endregion
                        #region RRA
                        case 0x67:
                            M = this.ZeroPage(arg1);
                            this.RRA(M, 5, 2);
                            break;
                        case 0x77:
                            M = this.ZeroPageX(arg1);
                            this.RRA(M, 6, 2);
                            break;
                        case 0x6F:
                            M = this.Absolute(arg1, arg2);
                            this.RRA(M, 6, 3);
                            break;
                        case 0x7F:
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.RRA(M, 7, 3);
                            break;
                        case 0x7B:
                            M = this.AbsoluteY(arg1, arg2, false);
                            this.RRA(M, 7, 3);
                            break;
                        case 0x63:
                            M = this.IndirectX(arg1);
                            this.RRA(M, 8, 2);
                            break;
                        case 0x73:
                            M = this.IndirectY(arg1, false);
                            this.RRA(M, 8, 2);
                            break;
                        #endregion
                        #region SBC
                        case 0xEB: M = arg1; this.SBC(M, 2, 2); break;
                        #endregion
                        #region SLO
                        case 0x07:
                            M = this.ZeroPage(arg1);
                            this.SLO(M, 5, 2);
                            break;
                        case 0x17:
                            M = this.ZeroPageX(arg1);
                            this.SLO(M, 6, 2);
                            break;
                        case 0x0F:
                            M = this.Absolute(arg1, arg2);
                            this.SLO(M, 6, 3);
                            break;
                        case 0x1F:
                            M = this.AbsoluteX(arg1, arg2, false);
                            this.SLO(M, 7, 3);
                            break;
                        case 0x1B:
                            M = this.AbsoluteY(arg1, arg2, false);
                            this.SLO(M, 7, 3);
                            break;
                        case 0x03:
                            M = this.IndirectX(arg1);
                            this.SLO(M, 8, 2);
                            break;
                        case 0x13:
                            M = this.IndirectY(arg1, false);
                            this.SLO(M, 8, 2);
                            break;
                        #endregion
                        #region SRE
                        case 0x47: this.SRE(this.ZeroPage(arg1), 5, 2); break;
                        case 0x57: this.SRE(this.ZeroPageX(arg1), 6, 2); break;
                        case 0x4F: this.SRE(this.Absolute(arg1, arg2), 6, 3); break;
                        case 0x5F: this.SRE(this.AbsoluteX(arg1, arg2, false), 7, 3); break;
                        case 0x5B: this.SRE(this.AbsoluteY(arg1, arg2, false), 7, 3); break;
                        case 0x43: this.SRE(this.IndirectX(arg1), 8, 2); break;
                        case 0x53: this.SRE(this.IndirectY(arg1, false), 8, 2); break;
                        #endregion
                        #region SXA
                        case 0x9E:
                            this.AbsoluteYWrite(arg1, arg2, this.SXA(arg1));
                            break;
                        #endregion
                        #region SYA
                        case 0x9C:
                            this.AbsoluteXWrite(arg1, arg2, this.SYA(arg1));
                            break;
                        #endregion
                        #region TOP
                        case 0x0C: this.TOP(4, 3); break;
                        case 0x1C:
                        case 0x3C:
                        case 0x5C:
                        case 0x7C:
                        case 0xDC:
                        case 0xFC: this.AbsoluteX(arg1, arg2, true); this.TOP(4, 3); break;
                        #endregion
                        #region XAA
                        case 0x8B: this.XAA(2, 2); break;
                        #endregion
                        #region XAS
                        case 0x9B:
                            M = this.AbsoluteY(arg1, arg2, false);
                            this.AbsoluteYWrite(arg1, arg2, this.XAS(M, arg1));
                            this.CycleCounter += 5;
                            this.REG_PC += 3;
                            break;
                        #endregion
                        default://Should not reach here, if it, shutdown the system
                            Debug.WriteLine(this, "<UNKOWN OPCODE> 0x" +
                                string.Format("{0:X}", this.OpCode), DebugStatus.Error);
                            this._Nes.ShutDown();
                            break;
                    }
                    #endregion
                    this._Nes.APU.Play();//If the sound is paused
                }
                else
                {
                    Thread.Sleep(100);
                    this._Nes.APU.Pause();
                    this.Paused = true;
                }
                if (this.CycleCounter >= this.CyclesPerScanline)
                {
                    if (this._Nes.PPU.DoScanline())/*NMI*/
                    {
                        this.Flag_B = false;
                        this.Push16(this.REG_PC);
                        this.PushStatus();
                        this.Flag_I = true;
                        this.REG_PC = this.MEM.Read16(0xFFFA);
                    }
                    if (this.IRQNextTime)/*IRQ*/
                    {
                        this.IRQ();
                        this.IRQNextTime = false;
                    }
                    //CycleCounter -= CyclesPerScanline;
                    this.CycleCounter = 0;//?!! Just don't know what to do with the rest cycles !!
                }
            }
        }
        public void Reset()
        {
            this.REG_S = 0xFF;
            this.REG_A = 0;
            this.REG_X = 0;
            this.REG_Y = 0;
            this.Flag_N = false;
            this.Flag_V = false;
            this.Flag_B = false;
            this.Flag_D = false;
            this.Flag_I = true;
            this.Flag_Z = false;
            this.Flag_C = false;
            this.REG_PC = this.MEM.Read16(0xFFFC);
        }
        public void SoftReset()
        {
            this.Pause = true;
            this.REG_S -= 3;
            this.Flag_I = true;
            this.REG_PC = this.MEM.Read16(0xFFFC);
            this._Nes.MEMORY.MAP.CurrentMapper.SoftReset();
            this.Pause = false;
        }
        /// <summary>
        /// Pause the cpu
        /// </summary>
        public void TogglePause()
        {
            this._Pause = !this._Pause;
            //Wait until the cpu pauses if the user resume the cpu loop
            if (this._Pause)
                while (!this.Paused)
                { }
            //Rise the event
            if (this.PauseToggle != null)
                this.PauseToggle(this, null);
        }
        /// <summary>
        /// Initialize and set the timing of the cpu
        /// </summary>
        /// <param name="FORMAT">The tv format to set the cpu emulation for</param>
        void InitializeCPU(TVFORMAT FORMAT)
        {
            switch (FORMAT)
            {
                case TVFORMAT.NTSC:
                    this.CyclesPerScanline = 113;
                    break;
                case TVFORMAT.PAL:
                    this.CyclesPerScanline = 106;
                    break;
            }
            this.REG_S = 0xFF;
            this.REG_A = 0;
            this.REG_X = 0;
            this.REG_Y = 0;
            Debug.WriteLine(this, "CPU initialized ok.", DebugStatus.Cool);
        }
        /// <summary>
        /// get two bytes into a correct address
        /// </summary>
        /// <param name="c">Byte A</param>
        /// <param name="d">Byte B</param>
        /// <returns>ushort A & B togather</returns>
        ushort MakeAddress(byte c, byte d)
        {
            ushort New = (ushort)((d << 8) | c);
            return New;
        }
        public void SetTVFormat(TVFORMAT FORMAT)
        {
            if (!this._Pause)
                return;
            switch (FORMAT)
            {
                case TVFORMAT.NTSC:
                    this.CyclesPerScanline = 113;
                    break;
                case TVFORMAT.PAL:
                    this.CyclesPerScanline = 106;
                    break;
            }
        }
        //Properties
        public bool ON
        { get { return this._ON; } set { this._ON = value; } }

        public bool Pause
        {
            get { return this._Pause; }
            set
            {
                this._Pause = value;
                //Wait until the cpu pauses if the user resume the cpu loop
                if (this._Pause)
                    while (!this.Paused)
                    { }
            }
        }
        #region Addressing Modes
        byte ZeroPage(ushort A)
        { return this.MEM.Read(A); }
        byte ZeroPageX(ushort A)
        { return this.MEM.Read((ushort)(0xFF & (A + this.REG_X))); }
        byte ZeroPageY(ushort A)
        { return this.MEM.Read((ushort)(0xFF & (A + this.REG_Y))); }
        byte Absolute(byte A, byte B)
        { return this.MEM.Read(this.MakeAddress(A, B)); }
        byte AbsoluteX(byte A, byte B, bool CheckPage)
        {
            if (CheckPage)
            {
                if ((this.MakeAddress(A, B) & 0xFF00) !=
                   ((this.MakeAddress(A, B) + this.REG_X) & 0xFF00))
                {
                    this.CycleCounter += 1;
                };
            }
            return this.MEM.Read((ushort)(this.MakeAddress(A, B) + this.REG_X));
        }
        byte AbsoluteY(byte A, byte B, bool CheckPage)
        {
            if (CheckPage)
            {
                if ((this.MakeAddress(A, B) & 0xFF00) !=
                   ((this.MakeAddress(A, B) + this.REG_Y) & 0xFF00))
                {
                    this.CycleCounter += 1;
                };
            }
            return this.MEM.Read((ushort)(this.MakeAddress(A, B) + this.REG_Y));
        }
        byte IndirectX(byte A)
        {
            return this.MEM.Read((ushort)this.MEM.Read16((ushort)(0xff & (A + (ushort)this.REG_X))));
        }
        byte IndirectY(byte A, bool CheckPage)
        {
            if (CheckPage)
            {
                if ((this.MEM.Read16(A) & 0xFF00) !=
                    ((this.MEM.Read16(A) + this.REG_Y) & 0xFF00))
                {
                    this.CycleCounter += 1;
                };
            }
            return this.MEM.Read((ushort)(this.MEM.Read16(A) + (ushort)this.REG_Y));
        }

        byte ZeroPageWrite(ushort A, byte data)
        {
            return this.MEM.Write(A, data);
        }
        byte ZeroPageXWrite(ushort A, byte data)
        {
            return this.MEM.Write((ushort)(0xff & (A + this.REG_X)), data);
        }
        byte ZeroPageYWrite(ushort A, byte data)
        {
            return this.MEM.Write((ushort)(0xff & (A + this.REG_Y)), data);
        }
        byte AbsoluteWrite(byte A, byte B, byte data)
        {
            return this.MEM.Write(this.MakeAddress(A, B), data);
        }
        byte AbsoluteXWrite(byte A, byte B, byte data)
        {
            return this.MEM.Write((ushort)(this.MakeAddress(A, B) + this.REG_X), data);
        }
        byte AbsoluteYWrite(byte A, byte B, byte data)
        {
            return this.MEM.Write((ushort)(this.MakeAddress(A, B) + this.REG_Y), data);
        }
        byte IndirectXWrite(byte A, byte data)
        {
            return this.MEM.Write((ushort)this.MEM.Read16((ushort)(0xff & (A + (short)this.REG_X))), data);
        }
        byte IndirectYWrite(byte A, byte data)
        {
            return this.MEM.Write((ushort)(this.MEM.Read16(A) + (ushort)this.REG_Y), data);
        }
        #endregion
        #region OPCODES
        void ADC(byte M, int count, ushort bytes)
        {
            int carry_flag = this.Flag_C ? 1 : 0;
            uint valueholder32 = (uint)(this.REG_A + M + carry_flag);
            //Set flags
            this.Flag_V = (((valueholder32 ^ this.REG_A) & (valueholder32 ^ M)) & 0x80) != 0;
            this.Flag_C = ((valueholder32 >> 8) != 0);
            this.Flag_Z = ((valueholder32 & 0xFF) == 0);
            this.Flag_N = ((valueholder32 & 0x80) == 0x80);
            this.REG_A = (byte)(valueholder32 & 0xff);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void AND(byte M, int count, ushort bytes)
        {
            this.REG_A = (byte)(this.REG_A & M);
            //Flags
            this.Flag_Z = (this.REG_A == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        byte ASL(byte M)
        {
            //Flags
            this.Flag_C = ((M & 0x80) == 0x80);
            M <<= 1;
            this.Flag_Z = ((M & 0xff) == 0x0);
            this.Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void BCC(byte arg1)
        {
            this.REG_PC += 2;
            if (!this.Flag_C)
            {
                if ((this.REG_PC & 0xFF00) != ((this.REG_PC + (sbyte)arg1 + 2) & 0xFF00))
                {
                    this.CycleCounter++;
                }
                this.REG_PC = (ushort)(this.REG_PC + (sbyte)arg1);
                this.CycleCounter++;
            }
            this.CycleCounter += 2;
        }
        void BCS(byte arg1)
        {
            if (this.Flag_C)
            {
                this.REG_PC += 2;
                if ((this.REG_PC & 0xFF00) != ((this.REG_PC + (sbyte)arg1 + 2) & 0xFF00))
                {
                    this.CycleCounter += 1;
                }
                this.REG_PC = (ushort)(this.REG_PC + (sbyte)arg1);
                this.CycleCounter += 1;
            }
            else
            {
                this.REG_PC += 2;
            }
            this.CycleCounter += 2;
        }
        void BEQ(byte arg1)
        {
            if (this.Flag_Z)
            {
                this.REG_PC += 2;
                if ((this.REG_PC & 0xFF00) != ((this.REG_PC + (sbyte)arg1 + 2) & 0xFF00))
                {
                    this.CycleCounter += 1;
                }
                this.REG_PC = (ushort)(this.REG_PC + (sbyte)arg1);
                this.CycleCounter += 1;
            }
            else
            {
                this.REG_PC += 2;
            }
            this.CycleCounter += 2;
        }
        void BIT(byte M, int count, ushort bytes)
        {
            this.Flag_Z = ((this.REG_A & M) == 0x0);
            this.Flag_N = ((M & 0x80) == 0x80);
            this.Flag_V = ((M & 0x40) == 0x40);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void BMI(byte arg1)
        {
            if (this.Flag_N)
            {
                this.REG_PC += 2;
                if ((this.REG_PC & 0xFF00) != ((this.REG_PC + (sbyte)arg1 + 2) & 0xFF00))
                {
                    this.CycleCounter += 1;
                }
                this.REG_PC = (ushort)(this.REG_PC + (sbyte)arg1);
                this.CycleCounter += 1;
            }
            else
            {
                this.REG_PC += 2;
            }
            this.CycleCounter += 2;
        }
        void BNE(byte arg1)
        {
            if (!this.Flag_Z)
            {
                this.REG_PC += 2;
                if ((this.REG_PC & 0xFF00) != ((this.REG_PC + (sbyte)arg1 + 2) & 0xFF00))
                {
                    this.CycleCounter += 1;
                }
                this.REG_PC = (ushort)(this.REG_PC + (sbyte)arg1);
                this.CycleCounter += 1;
            }
            else
            {
                this.REG_PC += 2;
            }
            this.CycleCounter += 2;
        }
        void BPL(byte arg1)
        {
            if (!this.Flag_N)
            {
                this.REG_PC += 2;
                if ((this.REG_PC & 0xFF00) != ((this.REG_PC + (sbyte)arg1 + 2) & 0xFF00))
                {
                    this.CycleCounter += 1;
                }
                this.REG_PC = (ushort)(this.REG_PC + (sbyte)arg1);
                this.CycleCounter += 1;
            }
            else
            {
                this.REG_PC += 2;
            }
            this.CycleCounter += 2;
        }
        void BRK()
        {
            this.REG_PC++;
            this.Push16((ushort)(this.REG_PC + 1));
            this.Flag_B = true;
            this.PushStatus();
            this.Flag_I = true;
            this.REG_PC = this.MEM.Read16(0xFFFE);
            this.CycleCounter += 7;
        }
        void BVC(byte arg1)
        {
            if (!this.Flag_V)
            {
                this.REG_PC += 2;
                if ((this.REG_PC & 0xFF00) != ((this.REG_PC + (sbyte)arg1 + 2) & 0xFF00))
                {
                    this.CycleCounter += 1;
                }
                this.REG_PC = (ushort)(this.REG_PC + (sbyte)arg1);
                this.CycleCounter += 1;
            }
            else
            {
                this.REG_PC += 2;
            }
            this.CycleCounter += 2;
        }
        void BVS(byte arg1)
        {
            if (this.Flag_V)
            {
                this.REG_PC += 2;
                if ((this.REG_PC & 0xFF00) != ((this.REG_PC + (sbyte)arg1 + 2) & 0xFF00))
                {
                    this.CycleCounter += 1;
                }
                this.REG_PC = (ushort)(this.REG_PC + (sbyte)arg1);
                this.CycleCounter += 1;
            }
            else
            {
                this.REG_PC += 2;
            }
            this.CycleCounter += 2;
        }
        void CLC()
        {
            this.Flag_C = false;
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void CLD()
        {
            this.Flag_D = false;
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void CLI()
        {
            this.Flag_I = false;
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void CLV()
        {
            this.Flag_V = false;
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void CMP(byte M, int count, ushort bytes)
        {
            //Flags
            this.Flag_C = (this.REG_A >= M);
            this.Flag_Z = (this.REG_A == M);
            M = (byte)(this.REG_A - M);
            this.Flag_N = ((M & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void CPX(byte M, int count, ushort bytes)
        {
            this.Flag_C = (this.REG_X >= M);
            this.Flag_Z = (this.REG_X == M);
            M = (byte)(this.REG_X - M);
            this.Flag_N = ((M & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void CPY(byte M, int count, ushort bytes)
        {
            this.Flag_C = (this.REG_Y >= M);
            this.Flag_Z = (this.REG_Y == M);
            M = (byte)(this.REG_Y - M);
            this.Flag_N = ((M & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        byte DEC(byte M)
        {
            M--;
            this.Flag_Z = (M == 0x0);
            this.Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void DEX()
        {
            this.REG_X--;
            this.Flag_Z = (this.REG_X == 0x0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
            this.REG_PC++;
            this.CycleCounter += 2;
        }
        void DEY()
        {
            this.REG_Y--;
            this.Flag_Z = (this.REG_Y == 0x0);
            this.Flag_N = ((this.REG_Y & 0x80) == 0x80);
            this.REG_PC++;
            this.CycleCounter += 2;
        }
        void EOR(byte M, int count, ushort bytes)
        {
            this.REG_A = (byte)(this.REG_A ^ M);
            this.Flag_Z = (this.REG_A == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        byte INC(byte M)
        {
            M++;
            this.Flag_Z = ((M & 0xff) == 0x0);
            this.Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void INX()
        {
            this.REG_X++;
            this.Flag_Z = ((this.REG_X & 0xff) == 0x0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
            this.REG_PC++;
            this.CycleCounter += 2;
        }
        void INY()
        {
            this.REG_Y++;
            this.Flag_Z = ((this.REG_Y & 0xff) == 0x0);
            this.Flag_N = ((this.REG_Y & 0x80) == 0x80);
            this.REG_PC++;
            this.CycleCounter += 2;
        }
        void IRQ()
        {
            if (!this.Flag_I)
            {
                this.Flag_B = false;
                this.Push16(this.REG_PC);
                this.PushStatus();
                this.Flag_I = true;
                this.REG_PC = this.MEM.Read16(0xFFFE);
            }
        }
        void JSR(byte arg1, byte arg2)
        {
            this.Push16((ushort)(this.REG_PC + 2));
            this.REG_PC = this.MakeAddress(arg1, arg2);
            this.CycleCounter += 6;
        }
        void LDA()
        {
            this.Flag_Z = (this.REG_A == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
        }
        void LDX()
        {
            this.Flag_Z = (this.REG_X == 0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
        }
        void LDY()
        {
            this.Flag_Z = (this.REG_Y == 0);
            this.Flag_N = ((this.REG_Y & 0x80) == 0x80);
        }
        byte LSR(byte M)
        {
            this.Flag_C = ((M & 0x1) == 0x1);
            M = (byte)(M >> 1);
            this.Flag_Z = ((M & 0xff) == 0x0);
            this.Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void NOP()
        {
            this.REG_PC++;
            this.CycleCounter += 2;
        }
        void ORA(byte M, int count, ushort bytes)
        {
            this.REG_A = (byte)(this.REG_A | M);
            this.Flag_Z = ((this.REG_A & 0xff) == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void PHA()
        {
            this.Push8(this.REG_A);
            this.REG_PC += 1;
            this.CycleCounter += 3;
        }
        void PHP()
        {
            this.Flag_B = true;
            this.PushStatus();
            this.REG_PC++;
            this.CycleCounter += 3;
        }
        void PLA()
        {
            this.REG_A = this.Pull8();
            this.Flag_Z = (this.REG_A == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            this.REG_PC += 1;
            this.CycleCounter += 4;
        }
        void PLP()
        {
            this.PullStatus();
            this.REG_PC += 1;
            this.CycleCounter += 4;
        }
        byte ROL(byte M)
        {
            byte bitholder = 0;
            if ((M & 0x80) == 0x80)
                bitholder = 1;
            else
                bitholder = 0;
            byte carry_flag = (byte)(this.Flag_C ? 1 : 0);
            M = (byte)(M << 1);
            M = (byte)(M | carry_flag);
            carry_flag = bitholder;
            this.Flag_C = (bitholder == 1);
            this.Flag_Z = ((M & 0xff) == 0x0);
            this.Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        byte ROR(byte M)
        {
            byte bitholder = 0;
            byte carry_flag = (byte)(this.Flag_C ? 1 : 0);
            if ((M & 0x1) == 0x1)
                bitholder = 1;
            else
                bitholder = 0;
            M = (byte)(M >> 1);

            if (carry_flag == 1)
                M = (byte)(M | 0x80);
            this.Flag_C = (bitholder == 1);
            this.Flag_Z = ((M & 0xff) == 0x0);
            this.Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void RTI()
        {
            this.PullStatus();
            this.REG_PC = this.Pull16();
            this.CycleCounter += 6;
        }
        void RTS()
        {
            this.REG_PC = this.Pull16();
            this.CycleCounter += 6;
            this.REG_PC += 1;
        }
        void SBC(byte M, int count, ushort bytes)
        {
            uint valueholder32;
            valueholder32 = (uint)(this.REG_A - M);
            if (!this.Flag_C)
                valueholder32 = valueholder32 - 1;
            if (valueholder32 > 255)
                this.Flag_C = false;
            else
                this.Flag_C = true;
            this.Flag_Z = ((valueholder32 & 0xff) == 0);
            this.Flag_V = ((this.REG_A ^ M) & (this.REG_A ^ valueholder32) & 0x80) != 0;
            this.Flag_N = ((valueholder32 & 0x80) == 0x80);
            this.REG_A = (byte)(valueholder32 & 0xff);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void SEC()
        {
            this.Flag_C = true;
            this.CycleCounter += 2;
            this.REG_PC += 1;
        }
        void SED()
        {
            this.Flag_D = true;
            this.CycleCounter += 2;
            this.REG_PC += 1;
        }
        void SEI()
        {
            this.Flag_I = true;
            this.CycleCounter += 2;
            this.REG_PC += 1;
        }
        void TAX()
        {
            this.REG_X = this.REG_A;
            this.Flag_Z = (this.REG_X == 0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void TAY()
        {
            this.REG_Y = this.REG_A;
            this.Flag_Z = (this.REG_Y == 0);
            this.Flag_N = ((this.REG_Y & 0x80) == 0x80);
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void TSX()
        {
            this.REG_X = this.REG_S;
            this.Flag_Z = (this.REG_X == 0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void TXA()
        {
            this.REG_A = this.REG_X;
            this.Flag_Z = (this.REG_A == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void TXS()
        {
            this.REG_S = this.REG_X;
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        void TYA()
        {
            this.REG_A = this.REG_Y;
            this.Flag_Z = (this.REG_A == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            this.REG_PC += 1;
            this.CycleCounter += 2;
        }
        /*Illegall Opcodes, not sure if they work*/
        void AAC(byte M, int count, ushort bytes)
        {
            this.REG_A &= M;
            this.Flag_C = ((this.REG_A >> 8) != 0);
            this.Flag_Z = ((this.REG_A & 0xFF) == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        byte AAX(byte M)
        {
            byte temp = (byte)((this.REG_X & this.REG_A) - M);
            this.Flag_Z = (temp == 0);
            this.Flag_N = ((temp & 0x80) == 0x80);
            return temp;
        }
        void ARR(byte M)
        {
            this.REG_A &= M;
            this.REG_A = this.ROR(this.REG_A);
            //Advance
            this.REG_PC += 2;
            this.CycleCounter += 2;
        }
        void ASR(byte M)
        {
            this.REG_A &= M;
            this.LSR(this.REG_A);
            //Advance
            this.REG_PC += 2;
            this.CycleCounter += 2;
        }
        void ATX(byte M)
        {
            this.REG_A &= M;
            this.REG_X = this.REG_A;
            this.Flag_Z = (this.REG_X == 0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
            //Advance
            this.REG_PC += 2;
            this.CycleCounter += 2;
        }
        byte AXA(byte M)
        {
            M = (byte)((this.REG_A & this.REG_X) & 7);
            return M;
        }
        void AXS(byte M)
        {
            this.REG_X = (byte)((this.REG_X & this.REG_A) - M);
            this.Flag_C = ((this.REG_X >> 8) != 0);
            this.Flag_Z = ((this.REG_X & 0xFF) == 0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
            //Advance
            this.REG_PC += 2;
            this.CycleCounter += 2;
        }
        void DCP(byte M, int count, ushort bytes)
        {
            M--;
            this.Flag_C = ((M >> 8) != 0);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void DOP(int count, ushort bytes)
        {
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void ISC(byte M, int count, ushort bytes)
        {
            M++;
            this.SBC(M, count, bytes);
        }
        void KIL()
        {
            this.REG_PC++;
        }
        void LAR(byte M, int count, ushort bytes)
        {
            this.REG_X = this.REG_A = (this.REG_S &= M);
            this.Flag_Z = ((this.REG_X & 0xFF) == 0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void LAX(byte M, int count, ushort bytes)
        {
            this.REG_X = this.REG_A = M;
            this.Flag_Z = ((this.REG_X & 0xFF) == 0);
            this.Flag_N = ((this.REG_X & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void RLA(byte M, int count, ushort bytes)
        {
            M <<= 1;
            if (this.Flag_C)
                M |= 1;
            this.Flag_C = (M & 0x80) != 0;
            this.REG_A &= M;
            this.Flag_Z = ((this.REG_A & 0xFF) == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void RRA(byte M, int count, ushort bytes)
        {
            M >>= 1;
            if (this.Flag_C)
                M |= 0x80;
            this.Flag_C = (M & 1) != 0;
            this.ADC(M, count, bytes);
        }
        void SLO(byte M, int count, ushort bytes)
        {
            this.Flag_C = (M & 0x80) != 0;
            M <<= 1;
            this.REG_A |= M;
            this.Flag_Z = ((this.REG_A & 0xFF) == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void SRE(byte M, int count, ushort bytes)
        {
            this.Flag_C = (M & 0x80) != 0;
            M >>= 1;
            this.REG_A ^= M;
            this.Flag_Z = ((this.REG_A & 0xFF) == 0);
            this.Flag_N = ((this.REG_A & 0x80) == 0x80);
            //Advance
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        byte SXA(byte M)
        {
            byte tmp = (byte)(this.REG_X & (M + 1));
            //Advance
            this.CycleCounter += 5;
            this.REG_PC += 3;
            return tmp;
        }
        byte SYA(byte M)
        {
            byte tmp = (byte)(this.REG_Y & (M + 1));
            //Advance
            this.CycleCounter += 5;
            this.REG_PC += 3;
            return tmp;
        }
        void TOP(int count, ushort bytes)
        {
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        void XAA(int count, ushort bytes)
        {
            this.CycleCounter += count;
            this.REG_PC += bytes;
        }
        byte XAS(byte M, byte arg1)
        {
            this.REG_S = (byte)(this.REG_X & this.REG_A);
            return (byte)(this.REG_S & (arg1 + 1));
        }
        #endregion
        #region Operations (pull, Push ...)
        void Push8(byte data)
        {
            this.MEM.Write((ushort)(0x100 + this.REG_S), data);
            this.REG_S--;
        }
        public void Push16(ushort data)
        {
            this.Push8((byte)((data & 0xFF00) >> 8));
            this.Push8((byte)(data & 0xFF));
        }
        public void PushStatus()
        {
            byte statusdata = 0;

            if (this.Flag_N)
                statusdata = (byte)(statusdata | 0x80);

            if (this.Flag_V)
                statusdata = (byte)(statusdata | 0x40);

            statusdata = (byte)(statusdata | 0x20);

            if (this.Flag_B)
                statusdata = (byte)(statusdata | 0x10);

            if (this.Flag_D)
                statusdata = (byte)(statusdata | 0x08);

            if (this.Flag_I)
                statusdata = (byte)(statusdata | 0x04);

            if (this.Flag_Z)
                statusdata = (byte)(statusdata | 0x02);

            if (this.Flag_C)
                statusdata = (byte)(statusdata | 0x01);
            this.Push8(statusdata);
        }
        public byte StatusRegister()
        {
            byte statusdata = 0;

            if (this.Flag_N)
                statusdata = (byte)(statusdata | 0x80);

            if (this.Flag_V)
                statusdata = (byte)(statusdata | 0x40);

            statusdata = (byte)(statusdata | 0x20);

            if (this.Flag_B)
                statusdata = (byte)(statusdata | 0x10);

            if (this.Flag_D)
                statusdata = (byte)(statusdata | 0x08);

            if (this.Flag_I)
                statusdata = (byte)(statusdata | 0x04);

            if (this.Flag_Z)
                statusdata = (byte)(statusdata | 0x02);

            if (this.Flag_C)
                statusdata = (byte)(statusdata | 0x01);

            return statusdata;
        }
        byte Pull8()
        {
            this.REG_S++;
            return this.MEM.Read((ushort)(0x100 + this.REG_S));
        }
        ushort Pull16()
        {
            byte data1 = this.Pull8();
            byte data2 = this.Pull8();
            return this.MakeAddress(data1, data2);
        }
        void PullStatus()
        {
            byte statusdata = this.Pull8();
            this.Flag_N = ((statusdata & 0x80) == 0x80);
            this.Flag_V = ((statusdata & 0x40) == 0x40);
            this.Flag_B = ((statusdata & 0x10) == 0x10);
            this.Flag_D = ((statusdata & 0x08) == 0x08);
            this.Flag_I = ((statusdata & 0x04) == 0x04);
            this.Flag_Z = ((statusdata & 0x02) == 0x02);
            this.Flag_C = ((statusdata & 0x01) == 0x01);
        }
        #endregion
        /// <summary>
        /// Event rised when the cpu is paused or rusumed
        /// </summary>
        public event EventHandler<EventArgs> PauseToggle;
    }
}