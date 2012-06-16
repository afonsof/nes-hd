using System;
using System.Threading;
using NesHd.Core.Debugger;

namespace NesHd.Core.CPU
{
    /// <summary>
    /// The 6502
    /// </summary>
    public class Cpu
    {
        private readonly Memory.Memory MEM;
        private readonly NesEngine _engine;
        public int CycleCounter;
        public int CyclesPerScanline;
        public bool Flag_B;
        public bool Flag_C;
        public bool Flag_D;
        public bool Flag_I = true;
        public bool Flag_N;
        public bool Flag_V;
        public bool Flag_Z;
        public bool IRQNextTime;
        //Clocks and timing stuff
        public byte OpCode;
        private bool Paused; //To ensure the cpu is paused
        public ushort PrevPC;
        public byte REG_A;
        public ushort REG_PC;
        public byte REG_S;
        public byte REG_X;
        public byte REG_Y;
        private bool _ON;
        private bool _Pause;

        /// <summary>
        /// The 6502
        /// </summary>
        /// <param name="Memory">The memory</param>
        /// <param name="TvFormat">The TV format</param>
        public Cpu(Memory.Memory Memory, TVFORMAT TvFormat, NesEngine Nes)
        {
            _engine = Nes;
            MEM = Memory;
            InitializeCPU(TvFormat);
        }

        public bool ON
        {
            get { return _ON; }
            set { _ON = value; }
        }

        public bool Pause
        {
            get { return _Pause; }
            set
            {
                _Pause = value;
                //Wait until the cpu pauses if the user resume the cpu loop
                if (_Pause)
                    while (!Paused)
                    {
                    }
            }
        }

        #region Addressing Modes

        private byte ZeroPage(ushort A)
        {
            return MEM.Read(A);
        }

        private byte ZeroPageX(ushort A)
        {
            return MEM.Read((ushort) (0xFF & (A + REG_X)));
        }

        private byte ZeroPageY(ushort A)
        {
            return MEM.Read((ushort) (0xFF & (A + REG_Y)));
        }

        private byte Absolute(byte A, byte B)
        {
            return MEM.Read(MakeAddress(A, B));
        }

        private byte AbsoluteX(byte A, byte B, bool CheckPage)
        {
            if (CheckPage)
            {
                if ((MakeAddress(A, B) & 0xFF00) !=
                    ((MakeAddress(A, B) + REG_X) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                ;
            }
            return MEM.Read((ushort) (MakeAddress(A, B) + REG_X));
        }

        private byte AbsoluteY(byte A, byte B, bool CheckPage)
        {
            if (CheckPage)
            {
                if ((MakeAddress(A, B) & 0xFF00) !=
                    ((MakeAddress(A, B) + REG_Y) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                ;
            }
            return MEM.Read((ushort) (MakeAddress(A, B) + REG_Y));
        }

        private byte IndirectX(byte A)
        {
            return MEM.Read(MEM.Read16((ushort) (0xff & (A + REG_X))));
        }

        private byte IndirectY(byte A, bool CheckPage)
        {
            if (CheckPage)
            {
                if ((MEM.Read16(A) & 0xFF00) !=
                    ((MEM.Read16(A) + REG_Y) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                ;
            }
            return MEM.Read((ushort) (MEM.Read16(A) + REG_Y));
        }

        private byte ZeroPageWrite(ushort A, byte data)
        {
            return MEM.Write(A, data);
        }

        private byte ZeroPageXWrite(ushort A, byte data)
        {
            return MEM.Write((ushort) (0xff & (A + REG_X)), data);
        }

        private byte ZeroPageYWrite(ushort A, byte data)
        {
            return MEM.Write((ushort) (0xff & (A + REG_Y)), data);
        }

        private byte AbsoluteWrite(byte A, byte B, byte data)
        {
            return MEM.Write(MakeAddress(A, B), data);
        }

        private byte AbsoluteXWrite(byte A, byte B, byte data)
        {
            return MEM.Write((ushort) (MakeAddress(A, B) + REG_X), data);
        }

        private byte AbsoluteYWrite(byte A, byte B, byte data)
        {
            return MEM.Write((ushort) (MakeAddress(A, B) + REG_Y), data);
        }

        private byte IndirectXWrite(byte A, byte data)
        {
            return MEM.Write(MEM.Read16((ushort) (0xff & (A + REG_X))), data);
        }

        private byte IndirectYWrite(byte A, byte data)
        {
            return MEM.Write((ushort) (MEM.Read16(A) + REG_Y), data);
        }

        #endregion

        #region OPCODES

        private void ADC(byte M, int count, ushort bytes)
        {
            var carry_flag = Flag_C ? 1 : 0;
            var valueholder32 = (uint) (REG_A + M + carry_flag);
            //Set flags
            Flag_V = (((valueholder32 ^ REG_A) & (valueholder32 ^ M)) & 0x80) != 0;
            Flag_C = ((valueholder32 >> 8) != 0);
            Flag_Z = ((valueholder32 & 0xFF) == 0);
            Flag_N = ((valueholder32 & 0x80) == 0x80);
            REG_A = (byte) (valueholder32 & 0xff);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void AND(byte M, int count, ushort bytes)
        {
            REG_A = (byte) (REG_A & M);
            //Flags
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private byte ASL(byte M)
        {
            //Flags
            Flag_C = ((M & 0x80) == 0x80);
            M <<= 1;
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }

        private void BCC(byte arg1)
        {
            REG_PC += 2;
            if (!Flag_C)
            {
                if ((REG_PC & 0xFF00) != ((REG_PC + (sbyte) arg1 + 2) & 0xFF00))
                {
                    CycleCounter++;
                }
                REG_PC = (ushort) (REG_PC + (sbyte) arg1);
                CycleCounter++;
            }
            CycleCounter += 2;
        }

        private void BCS(byte arg1)
        {
            if (Flag_C)
            {
                REG_PC += 2;
                if ((REG_PC & 0xFF00) != ((REG_PC + (sbyte) arg1 + 2) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                REG_PC = (ushort) (REG_PC + (sbyte) arg1);
                CycleCounter += 1;
            }
            else
            {
                REG_PC += 2;
            }
            CycleCounter += 2;
        }

        private void BEQ(byte arg1)
        {
            if (Flag_Z)
            {
                REG_PC += 2;
                if ((REG_PC & 0xFF00) != ((REG_PC + (sbyte) arg1 + 2) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                REG_PC = (ushort) (REG_PC + (sbyte) arg1);
                CycleCounter += 1;
            }
            else
            {
                REG_PC += 2;
            }
            CycleCounter += 2;
        }

        private void BIT(byte M, int count, ushort bytes)
        {
            Flag_Z = ((REG_A & M) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            Flag_V = ((M & 0x40) == 0x40);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void BMI(byte arg1)
        {
            if (Flag_N)
            {
                REG_PC += 2;
                if ((REG_PC & 0xFF00) != ((REG_PC + (sbyte) arg1 + 2) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                REG_PC = (ushort) (REG_PC + (sbyte) arg1);
                CycleCounter += 1;
            }
            else
            {
                REG_PC += 2;
            }
            CycleCounter += 2;
        }

        private void BNE(byte arg1)
        {
            if (!Flag_Z)
            {
                REG_PC += 2;
                if ((REG_PC & 0xFF00) != ((REG_PC + (sbyte) arg1 + 2) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                REG_PC = (ushort) (REG_PC + (sbyte) arg1);
                CycleCounter += 1;
            }
            else
            {
                REG_PC += 2;
            }
            CycleCounter += 2;
        }

        private void BPL(byte arg1)
        {
            if (!Flag_N)
            {
                REG_PC += 2;
                if ((REG_PC & 0xFF00) != ((REG_PC + (sbyte) arg1 + 2) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                REG_PC = (ushort) (REG_PC + (sbyte) arg1);
                CycleCounter += 1;
            }
            else
            {
                REG_PC += 2;
            }
            CycleCounter += 2;
        }

        private void BRK()
        {
            REG_PC++;
            Push16((ushort) (REG_PC + 1));
            Flag_B = true;
            PushStatus();
            Flag_I = true;
            REG_PC = MEM.Read16(0xFFFE);
            CycleCounter += 7;
        }

        private void BVC(byte arg1)
        {
            if (!Flag_V)
            {
                REG_PC += 2;
                if ((REG_PC & 0xFF00) != ((REG_PC + (sbyte) arg1 + 2) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                REG_PC = (ushort) (REG_PC + (sbyte) arg1);
                CycleCounter += 1;
            }
            else
            {
                REG_PC += 2;
            }
            CycleCounter += 2;
        }

        private void BVS(byte arg1)
        {
            if (Flag_V)
            {
                REG_PC += 2;
                if ((REG_PC & 0xFF00) != ((REG_PC + (sbyte) arg1 + 2) & 0xFF00))
                {
                    CycleCounter += 1;
                }
                REG_PC = (ushort) (REG_PC + (sbyte) arg1);
                CycleCounter += 1;
            }
            else
            {
                REG_PC += 2;
            }
            CycleCounter += 2;
        }

        private void CLC()
        {
            Flag_C = false;
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void CLD()
        {
            Flag_D = false;
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void CLI()
        {
            Flag_I = false;
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void CLV()
        {
            Flag_V = false;
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void CMP(byte M, int count, ushort bytes)
        {
            //Flags
            Flag_C = (REG_A >= M);
            Flag_Z = (REG_A == M);
            M = (byte) (REG_A - M);
            Flag_N = ((M & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void CPX(byte M, int count, ushort bytes)
        {
            Flag_C = (REG_X >= M);
            Flag_Z = (REG_X == M);
            M = (byte) (REG_X - M);
            Flag_N = ((M & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void CPY(byte M, int count, ushort bytes)
        {
            Flag_C = (REG_Y >= M);
            Flag_Z = (REG_Y == M);
            M = (byte) (REG_Y - M);
            Flag_N = ((M & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private byte DEC(byte M)
        {
            M--;
            Flag_Z = (M == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }

        private void DEX()
        {
            REG_X--;
            Flag_Z = (REG_X == 0x0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            REG_PC++;
            CycleCounter += 2;
        }

        private void DEY()
        {
            REG_Y--;
            Flag_Z = (REG_Y == 0x0);
            Flag_N = ((REG_Y & 0x80) == 0x80);
            REG_PC++;
            CycleCounter += 2;
        }

        private void EOR(byte M, int count, ushort bytes)
        {
            REG_A = (byte) (REG_A ^ M);
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private byte INC(byte M)
        {
            M++;
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }

        private void INX()
        {
            REG_X++;
            Flag_Z = ((REG_X & 0xff) == 0x0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            REG_PC++;
            CycleCounter += 2;
        }

        private void INY()
        {
            REG_Y++;
            Flag_Z = ((REG_Y & 0xff) == 0x0);
            Flag_N = ((REG_Y & 0x80) == 0x80);
            REG_PC++;
            CycleCounter += 2;
        }

        private void IRQ()
        {
            if (!Flag_I)
            {
                Flag_B = false;
                Push16(REG_PC);
                PushStatus();
                Flag_I = true;
                REG_PC = MEM.Read16(0xFFFE);
            }
        }

        private void JSR(byte arg1, byte arg2)
        {
            Push16((ushort) (REG_PC + 2));
            REG_PC = MakeAddress(arg1, arg2);
            CycleCounter += 6;
        }

        private void LDA()
        {
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
        }

        private void LDX()
        {
            Flag_Z = (REG_X == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
        }

        private void LDY()
        {
            Flag_Z = (REG_Y == 0);
            Flag_N = ((REG_Y & 0x80) == 0x80);
        }

        private byte LSR(byte M)
        {
            Flag_C = ((M & 0x1) == 0x1);
            M = (byte) (M >> 1);
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }

        private void NOP()
        {
            REG_PC++;
            CycleCounter += 2;
        }

        private void ORA(byte M, int count, ushort bytes)
        {
            REG_A = (byte) (REG_A | M);
            Flag_Z = ((REG_A & 0xff) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void PHA()
        {
            Push8(REG_A);
            REG_PC += 1;
            CycleCounter += 3;
        }

        private void PHP()
        {
            Flag_B = true;
            PushStatus();
            REG_PC++;
            CycleCounter += 3;
        }

        private void PLA()
        {
            REG_A = Pull8();
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 4;
        }

        private void PLP()
        {
            PullStatus();
            REG_PC += 1;
            CycleCounter += 4;
        }

        private byte ROL(byte M)
        {
            byte bitholder = 0;
            if ((M & 0x80) == 0x80)
                bitholder = 1;
            else
                bitholder = 0;
            var carry_flag = (byte) (Flag_C ? 1 : 0);
            M = (byte) (M << 1);
            M = (byte) (M | carry_flag);
            carry_flag = bitholder;
            Flag_C = (bitholder == 1);
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }

        private byte ROR(byte M)
        {
            byte bitholder = 0;
            var carry_flag = (byte) (Flag_C ? 1 : 0);
            if ((M & 0x1) == 0x1)
                bitholder = 1;
            else
                bitholder = 0;
            M = (byte) (M >> 1);

            if (carry_flag == 1)
                M = (byte) (M | 0x80);
            Flag_C = (bitholder == 1);
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }

        private void RTI()
        {
            PullStatus();
            REG_PC = Pull16();
            CycleCounter += 6;
        }

        private void RTS()
        {
            REG_PC = Pull16();
            CycleCounter += 6;
            REG_PC += 1;
        }

        private void SBC(byte M, int count, ushort bytes)
        {
            uint valueholder32;
            valueholder32 = (uint) (REG_A - M);
            if (!Flag_C)
                valueholder32 = valueholder32 - 1;
            if (valueholder32 > 255)
                Flag_C = false;
            else
                Flag_C = true;
            Flag_Z = ((valueholder32 & 0xff) == 0);
            Flag_V = ((REG_A ^ M) & (REG_A ^ valueholder32) & 0x80) != 0;
            Flag_N = ((valueholder32 & 0x80) == 0x80);
            REG_A = (byte) (valueholder32 & 0xff);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void SEC()
        {
            Flag_C = true;
            CycleCounter += 2;
            REG_PC += 1;
        }

        private void SED()
        {
            Flag_D = true;
            CycleCounter += 2;
            REG_PC += 1;
        }

        private void SEI()
        {
            Flag_I = true;
            CycleCounter += 2;
            REG_PC += 1;
        }

        private void TAX()
        {
            REG_X = REG_A;
            Flag_Z = (REG_X == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void TAY()
        {
            REG_Y = REG_A;
            Flag_Z = (REG_Y == 0);
            Flag_N = ((REG_Y & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void TSX()
        {
            REG_X = REG_S;
            Flag_Z = (REG_X == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void TXA()
        {
            REG_A = REG_X;
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void TXS()
        {
            REG_S = REG_X;
            REG_PC += 1;
            CycleCounter += 2;
        }

        private void TYA()
        {
            REG_A = REG_Y;
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }

        /*Illegall Opcodes, not sure if they work*/

        private void AAC(byte M, int count, ushort bytes)
        {
            REG_A &= M;
            Flag_C = ((REG_A >> 8) != 0);
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private byte AAX(byte M)
        {
            var temp = (byte) ((REG_X & REG_A) - M);
            Flag_Z = (temp == 0);
            Flag_N = ((temp & 0x80) == 0x80);
            return temp;
        }

        private void ARR(byte M)
        {
            REG_A &= M;
            REG_A = ROR(REG_A);
            //Advance
            REG_PC += 2;
            CycleCounter += 2;
        }

        private void ASR(byte M)
        {
            REG_A &= M;
            LSR(REG_A);
            //Advance
            REG_PC += 2;
            CycleCounter += 2;
        }

        private void ATX(byte M)
        {
            REG_A &= M;
            REG_X = REG_A;
            Flag_Z = (REG_X == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            //Advance
            REG_PC += 2;
            CycleCounter += 2;
        }

        private byte AXA(byte M)
        {
            M = (byte) ((REG_A & REG_X) & 7);
            return M;
        }

        private void AXS(byte M)
        {
            REG_X = (byte) ((REG_X & REG_A) - M);
            Flag_C = ((REG_X >> 8) != 0);
            Flag_Z = ((REG_X & 0xFF) == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            //Advance
            REG_PC += 2;
            CycleCounter += 2;
        }

        private void DCP(byte M, int count, ushort bytes)
        {
            M--;
            Flag_C = ((M >> 8) != 0);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void DOP(int count, ushort bytes)
        {
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void ISC(byte M, int count, ushort bytes)
        {
            M++;
            SBC(M, count, bytes);
        }

        private void KIL()
        {
            REG_PC++;
        }

        private void LAR(byte M, int count, ushort bytes)
        {
            REG_X = REG_A = (REG_S &= M);
            Flag_Z = ((REG_X & 0xFF) == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void LAX(byte M, int count, ushort bytes)
        {
            REG_X = REG_A = M;
            Flag_Z = ((REG_X & 0xFF) == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void RLA(byte M, int count, ushort bytes)
        {
            M <<= 1;
            if (Flag_C)
                M |= 1;
            Flag_C = (M & 0x80) != 0;
            REG_A &= M;
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void RRA(byte M, int count, ushort bytes)
        {
            M >>= 1;
            if (Flag_C)
                M |= 0x80;
            Flag_C = (M & 1) != 0;
            ADC(M, count, bytes);
        }

        private void SLO(byte M, int count, ushort bytes)
        {
            Flag_C = (M & 0x80) != 0;
            M <<= 1;
            REG_A |= M;
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void SRE(byte M, int count, ushort bytes)
        {
            Flag_C = (M & 0x80) != 0;
            M >>= 1;
            REG_A ^= M;
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }

        private byte SXA(byte M)
        {
            var tmp = (byte) (REG_X & (M + 1));
            //Advance
            CycleCounter += 5;
            REG_PC += 3;
            return tmp;
        }

        private byte SYA(byte M)
        {
            var tmp = (byte) (REG_Y & (M + 1));
            //Advance
            CycleCounter += 5;
            REG_PC += 3;
            return tmp;
        }

        private void TOP(int count, ushort bytes)
        {
            CycleCounter += count;
            REG_PC += bytes;
        }

        private void XAA(int count, ushort bytes)
        {
            CycleCounter += count;
            REG_PC += bytes;
        }

        private byte XAS(byte M, byte arg1)
        {
            REG_S = (byte) (REG_X & REG_A);
            return (byte) (REG_S & (arg1 + 1));
        }

        #endregion

        #region Operations (pull, Push ...)

        private void Push8(byte data)
        {
            MEM.Write((ushort) (0x100 + REG_S), data);
            REG_S--;
        }

        public void Push16(ushort data)
        {
            Push8((byte) ((data & 0xFF00) >> 8));
            Push8((byte) (data & 0xFF));
        }

        public void PushStatus()
        {
            byte statusdata = 0;

            if (Flag_N)
                statusdata = (byte) (statusdata | 0x80);

            if (Flag_V)
                statusdata = (byte) (statusdata | 0x40);

            statusdata = (byte) (statusdata | 0x20);

            if (Flag_B)
                statusdata = (byte) (statusdata | 0x10);

            if (Flag_D)
                statusdata = (byte) (statusdata | 0x08);

            if (Flag_I)
                statusdata = (byte) (statusdata | 0x04);

            if (Flag_Z)
                statusdata = (byte) (statusdata | 0x02);

            if (Flag_C)
                statusdata = (byte) (statusdata | 0x01);
            Push8(statusdata);
        }

        public byte StatusRegister()
        {
            byte statusdata = 0;

            if (Flag_N)
                statusdata = (byte) (statusdata | 0x80);

            if (Flag_V)
                statusdata = (byte) (statusdata | 0x40);

            statusdata = (byte) (statusdata | 0x20);

            if (Flag_B)
                statusdata = (byte) (statusdata | 0x10);

            if (Flag_D)
                statusdata = (byte) (statusdata | 0x08);

            if (Flag_I)
                statusdata = (byte) (statusdata | 0x04);

            if (Flag_Z)
                statusdata = (byte) (statusdata | 0x02);

            if (Flag_C)
                statusdata = (byte) (statusdata | 0x01);

            return statusdata;
        }

        private byte Pull8()
        {
            REG_S++;
            return MEM.Read((ushort) (0x100 + REG_S));
        }

        private ushort Pull16()
        {
            var data1 = Pull8();
            var data2 = Pull8();
            return MakeAddress(data1, data2);
        }

        private void PullStatus()
        {
            var statusdata = Pull8();
            Flag_N = ((statusdata & 0x80) == 0x80);
            Flag_V = ((statusdata & 0x40) == 0x40);
            Flag_B = ((statusdata & 0x10) == 0x10);
            Flag_D = ((statusdata & 0x08) == 0x08);
            Flag_I = ((statusdata & 0x04) == 0x04);
            Flag_Z = ((statusdata & 0x02) == 0x02);
            Flag_C = ((statusdata & 0x01) == 0x01);
        }

        #endregion

        /// <summary>
        /// Run the cpu looping
        /// </summary>
        public void Run()
        {
            PrevPC = REG_PC;
            while (_ON)
            {
                OpCode = MEM.Read(REG_PC);
                if (!_Pause)
                {
                    Paused = false;

                    #region DO OPCODE

                    // We may not use both, but it's easier to grab them now
                    var arg1 = MEM.Read((ushort) (REG_PC + 1));
                    var arg2 = MEM.Read((ushort) (REG_PC + 2));
                    byte M = 0xFF; //The value holder
                    switch (OpCode)
                    {
                            #region ADC

                        case (0x61):
                            M = IndirectX(arg1);
                            ADC(M, 6, 2);
                            break;
                        case (0x65):
                            M = ZeroPage(arg1);
                            ADC(M, 3, 2);
                            break;
                        case (0x69):
                            M = arg1;
                            ADC(M, 2, 2);
                            break;
                        case (0x6D):
                            M = Absolute(arg1, arg2);
                            ADC(M, 4, 3);
                            break;
                        case (0x71):
                            M = IndirectY(arg1, true);
                            ADC(M, 5, 2);
                            break;
                        case (0x75):
                            M = ZeroPageX(arg1);
                            ADC(M, 4, 2);
                            break;
                        case (0x79):
                            M = AbsoluteY(arg1, arg2, true);
                            ADC(M, 4, 3);
                            break;
                        case (0x7D):
                            M = AbsoluteX(arg1, arg2, true);
                            ADC(M, 4, 3);
                            break;

                            #endregion

                            #region AND

                        case (0x21):
                            M = IndirectX(arg1);
                            AND(M, 6, 2);
                            break;
                        case (0x25):
                            M = ZeroPage(arg1);
                            AND(M, 3, 2);
                            break;
                        case (0x29):
                            M = arg1;
                            AND(M, 2, 2);
                            break;
                        case (0x2D):
                            M = Absolute(arg1, arg2);
                            AND(M, 3, 3);
                            break;
                        case (0x31):
                            M = IndirectY(arg1, false);
                            AND(M, 5, 2);
                            break;
                        case (0x35):
                            M = ZeroPageX(arg1);
                            AND(M, 4, 2);
                            break;
                        case (0x39):
                            M = AbsoluteY(arg1, arg2, true);
                            AND(M, 4, 3);
                            break;
                        case (0x3D):
                            M = AbsoluteX(arg1, arg2, true);
                            AND(M, 4, 3);
                            break;

                            #endregion

                            #region ASL

                        case (0x06):
                            M = ZeroPage(arg1);
                            ZeroPageWrite(arg1, ASL(M));
                            CycleCounter += 5;
                            REG_PC += 2;
                            break;
                        case (0x0A):
                            M = REG_A;
                            REG_A = ASL(M);
                            CycleCounter += 2;
                            REG_PC += 1;
                            break;
                        case (0x0E):
                            M = Absolute(arg1, arg2);
                            AbsoluteWrite(arg1, arg2, ASL(M));
                            CycleCounter += 6;
                            REG_PC += 3;
                            break;
                        case (0x16):
                            M = ZeroPageX(arg1);
                            ZeroPageXWrite(arg1, ASL(M));
                            CycleCounter += 6;
                            REG_PC += 2;
                            break;
                        case (0x1E):
                            M = AbsoluteX(arg1, arg2, false);
                            AbsoluteXWrite(arg1, arg2, ASL(M));
                            CycleCounter += 7;
                            REG_PC += 3;
                            break;

                            #endregion

                            #region BCC

                        case (0x90):
                            BCC(arg1);
                            break;

                            #endregion

                            #region BCS

                        case (0xb0):
                            BCS(arg1);
                            break;

                            #endregion

                            #region BEQ

                        case (0xf0):
                            BEQ(arg1);
                            break;

                            #endregion

                            #region BIT

                        case (0x24):
                            M = ZeroPage(arg1);
                            BIT(M, 3, 2);
                            break;
                        case (0x2c):
                            M = Absolute(arg1, arg2);
                            BIT(M, 4, 3);
                            break;

                            #endregion

                            #region BMI

                        case (0x30):
                            BMI(arg1);
                            break;

                            #endregion

                            #region BNE

                        case (0xd0):
                            BNE(arg1);
                            break;

                            #endregion

                            #region BPL

                        case (0x10):
                            BPL(arg1);
                            break;

                            #endregion

                            #region BRK

                        case (0x00):
                            BRK();
                            break;

                            #endregion

                            #region BVC

                        case (0x50):
                            BVC(arg1);
                            break;

                            #endregion

                            #region BVS

                        case (0x70):
                            BVS(arg1);
                            break;

                            #endregion

                            #region CLC

                        case (0x18):
                            CLC();
                            break;

                            #endregion

                            #region CLD

                        case (0xd8):
                            CLD();
                            break;

                            #endregion

                            #region CLI

                        case (0x58):
                            CLI();
                            break;

                            #endregion

                            #region CLV

                        case (0xb8):
                            CLV();
                            break;

                            #endregion

                            #region CMP

                        case (0xC1):
                            M = IndirectX(arg1);
                            CMP(M, 6, 2);
                            break;
                        case (0xC5):
                            M = ZeroPage(arg1);
                            CMP(M, 3, 2);
                            break;
                        case (0xC9):
                            M = arg1;
                            CMP(M, 2, 2);
                            break;
                        case (0xCD):
                            M = Absolute(arg1, arg2);
                            CMP(M, 4, 3);
                            break;
                        case (0xd1):
                            M = IndirectY(arg1, true);
                            CMP(M, 5, 2);
                            break;
                        case (0xd5):
                            M = ZeroPageX(arg1);
                            CMP(M, 4, 2);
                            break;
                        case (0xd9):
                            M = AbsoluteY(arg1, arg2, true);
                            CMP(M, 4, 3);
                            break;
                        case (0xdd):
                            M = AbsoluteX(arg1, arg2, true);
                            CMP(M, 4, 3);
                            break;

                            #endregion

                            #region CPX

                        case (0xE0):
                            M = arg1;
                            CPX(M, 2, 2);
                            break;
                        case (0xE4):
                            M = ZeroPage(arg1);
                            CPX(M, 3, 2);
                            break;
                        case (0xEC):
                            M = Absolute(arg1, arg2);
                            CPX(M, 4, 3);
                            break;

                            #endregion

                            #region CPY

                        case (0xc0):
                            M = arg1;
                            CPY(M, 2, 2);
                            break;
                        case (0xc4):
                            M = ZeroPage(arg1);
                            CPY(M, 3, 2);
                            break;
                        case (0xcc):
                            M = Absolute(arg1, arg2);
                            CPY(M, 4, 3);
                            break;

                            #endregion

                            #region DEC

                        case (0xc6):
                            M = ZeroPage(arg1);
                            ZeroPageWrite(arg1, DEC(M));
                            CycleCounter += 5;
                            REG_PC += 2;
                            break;
                        case (0xce):
                            M = Absolute(arg1, arg2);
                            AbsoluteWrite(arg1, arg2, DEC(M));
                            CycleCounter += 6;
                            REG_PC += 3;
                            break;
                        case (0xd6):
                            M = ZeroPageX(arg1);
                            ZeroPageXWrite(arg1, DEC(M));
                            CycleCounter += 6;
                            REG_PC += 2;
                            break;
                        case (0xde):
                            M = AbsoluteX(arg1, arg2, false);
                            AbsoluteXWrite(arg1, arg2, DEC(M));
                            CycleCounter += 7;
                            REG_PC += 3;
                            break;

                            #endregion

                            #region DEX

                        case (0xca):
                            DEX();
                            break;

                            #endregion

                            #region DEY

                        case (0x88):
                            DEY();
                            break;

                            #endregion

                            #region EOR

                        case (0x41):
                            M = IndirectX(arg1);
                            EOR(M, 6, 2);
                            break;
                        case (0x45):
                            M = ZeroPage(arg1);
                            EOR(M, 3, 2);
                            break;
                        case (0x49):
                            M = arg1;
                            EOR(M, 2, 2);
                            break;
                        case (0x4d):
                            M = Absolute(arg1, arg2);
                            EOR(M, 3, 3);
                            break;
                        case (0x51):
                            M = IndirectY(arg1, true);
                            EOR(M, 5, 2);
                            break;
                        case (0x55):
                            M = ZeroPageX(arg1);
                            EOR(M, 4, 2);
                            break;
                        case (0x59):
                            M = AbsoluteY(arg1, arg2, true);
                            EOR(M, 4, 3);
                            break;
                        case (0x5d):
                            M = AbsoluteX(arg1, arg2, true);
                            EOR(M, 4, 3);
                            break;

                            #endregion

                            #region INC

                        case (0xe6):
                            M = ZeroPage(arg1);
                            ZeroPageWrite(arg1, INC(M));
                            CycleCounter += 5;
                            REG_PC += 2;
                            break;
                        case (0xee):
                            M = Absolute(arg1, arg2);
                            AbsoluteWrite(arg1, arg2, INC(M));
                            CycleCounter += 6;
                            REG_PC += 3;
                            break;
                        case (0xf6):
                            M = ZeroPageX(arg1);
                            ZeroPageXWrite(arg1, INC(M));
                            CycleCounter += 6;
                            REG_PC += 2;
                            break;
                        case (0xfe):
                            M = AbsoluteX(arg1, arg2, false);
                            AbsoluteXWrite(arg1, arg2, INC(M));
                            CycleCounter += 7;
                            REG_PC += 3;
                            break;

                            #endregion

                            #region INX

                        case (0xe8):
                            INX();
                            break;

                            #endregion

                            #region INY

                        case (0xc8):
                            INY();
                            break;

                            #endregion

                            #region JMP

                        case (0x4c):
                            REG_PC = MEM.Read16((ushort) (REG_PC + 1));
                            CycleCounter += 3;
                            break;
                        case (0x6c):
                            var myAddress = MEM.Read16((ushort) (REG_PC + 1));
                            if ((myAddress & 0x00FF) == 0x00FF)
                            {
                                REG_PC = MEM.Read(myAddress);
                                myAddress &= 0xFF00;
                                REG_PC |= (ushort) ((MEM.Read(myAddress)) << 8);
                            }
                            else
                                REG_PC = MEM.Read16(myAddress);
                            CycleCounter += 5;
                            break;

                            #endregion

                            #region JSR

                        case (0x20):
                            JSR(arg1, arg2);
                            break;

                            #endregion

                            #region LDA

                        case (0xa1):
                            REG_A = IndirectX(arg1);
                            CycleCounter += 6;
                            REG_PC += 2;
                            LDA();
                            break;
                        case (0xa5):
                            REG_A = ZeroPage(arg1);
                            CycleCounter += 3;
                            REG_PC += 2;
                            LDA();
                            break;
                        case (0xa9):
                            REG_A = arg1;
                            CycleCounter += 2;
                            REG_PC += 2;
                            LDA();
                            break;
                        case (0xad):
                            REG_A = Absolute(arg1, arg2);
                            CycleCounter += 4;
                            REG_PC += 3;
                            LDA();
                            break;
                        case (0xb1):
                            REG_A = IndirectY(arg1, true);
                            CycleCounter += 5;
                            REG_PC += 2;
                            LDA();
                            break;
                        case (0xb5):
                            REG_A = ZeroPageX(arg1);
                            CycleCounter += 4;
                            REG_PC += 2;
                            LDA();
                            break;
                        case (0xb9):
                            REG_A = AbsoluteY(arg1, arg2, true);
                            CycleCounter += 4;
                            REG_PC += 3;
                            LDA();
                            break;
                        case (0xbd):
                            REG_A = AbsoluteX(arg1, arg2, true);
                            CycleCounter += 4;
                            REG_PC += 3;
                            LDA();
                            break;

                            #endregion

                            #region LDX

                        case (0xa2):
                            REG_X = arg1;
                            CycleCounter += 2;
                            REG_PC += 2;
                            LDX();
                            break;
                        case (0xa6):
                            REG_X = ZeroPage(arg1);
                            CycleCounter += 3;
                            REG_PC += 2;
                            LDX();
                            break;
                        case (0xae):
                            REG_X = Absolute(arg1, arg2);
                            CycleCounter += 4;
                            REG_PC += 3;
                            LDX();
                            break;
                        case (0xb6):
                            REG_X = ZeroPageY(arg1);
                            CycleCounter += 4;
                            REG_PC += 2;
                            LDX();
                            break;
                        case (0xbe):
                            REG_X = AbsoluteY(arg1, arg2, true);
                            CycleCounter += 4;
                            REG_PC += 3;
                            LDX();
                            break;

                            #endregion

                            #region LDY

                        case (0xa0):
                            REG_Y = arg1;
                            CycleCounter += 2;
                            REG_PC += 2;
                            LDY();
                            break;
                        case (0xa4):
                            REG_Y = ZeroPage(arg1);
                            CycleCounter += 3;
                            REG_PC += 2;
                            LDY();
                            break;
                        case (0xac):
                            REG_Y = Absolute(arg1, arg2);
                            CycleCounter += 4;
                            REG_PC += 3;
                            LDY();
                            break;
                        case (0xb4):
                            REG_Y = ZeroPageX(arg1);
                            CycleCounter += 4;
                            REG_PC += 2;
                            LDY();
                            break;
                        case (0xbc):
                            REG_Y = AbsoluteX(arg1, arg2, true);
                            CycleCounter += 4;
                            REG_PC += 3;
                            LDY();
                            break;

                            #endregion

                            #region LSR

                        case (0x46):
                            M = ZeroPage(arg1);
                            ZeroPageWrite(arg1, LSR(M));
                            CycleCounter += 5;
                            REG_PC += 2;
                            break;
                        case (0x4a):
                            M = REG_A;
                            REG_A = LSR(M);
                            CycleCounter += 2;
                            REG_PC += 1;
                            break;
                        case (0x4e):
                            M = Absolute(arg1, arg2);
                            AbsoluteWrite(arg1, arg2, LSR(M));
                            CycleCounter += 6;
                            REG_PC += 3;
                            break;
                        case (0x56):
                            M = ZeroPageX(arg1);
                            ZeroPageXWrite(arg1, LSR(M));
                            CycleCounter += 6;
                            REG_PC += 2;
                            break;
                        case (0x5e):
                            M = AbsoluteX(arg1, arg2, false);
                            AbsoluteXWrite(arg1, arg2, LSR(M));
                            CycleCounter += 7;
                            REG_PC += 3;
                            break;

                            #endregion

                            #region NOP

                        case (0xEA):
                            NOP();
                            break;

                            #endregion

                            #region ORA

                        case (0x01):
                            M = IndirectX(arg1);
                            ORA(M, 6, 2);
                            break;
                        case (0x05):
                            M = ZeroPage(arg1);
                            ORA(M, 3, 2);
                            break;
                        case (0x09):
                            M = arg1;
                            ORA(M, 2, 2);
                            break;
                        case (0x0d):
                            M = Absolute(arg1, arg2);
                            ORA(M, 4, 3);
                            break;
                        case (0x11):
                            M = IndirectY(arg1, false);
                            ORA(M, 5, 2);
                            break;
                        case (0x15):
                            M = ZeroPageX(arg1);
                            ORA(M, 4, 2);
                            break;
                        case (0x19):
                            M = AbsoluteY(arg1, arg2, true);
                            ORA(M, 4, 3);
                            break;
                        case (0x1d):
                            M = AbsoluteX(arg1, arg2, true);
                            ORA(M, 4, 3);
                            break;

                            #endregion

                            #region PHA

                        case (0x48):
                            PHA();
                            break;

                            #endregion

                            #region PHP

                        case (0x08):
                            PHP();
                            break;

                            #endregion

                            #region PLA

                        case (0x68):
                            PLA();
                            break;

                            #endregion

                            #region PLP

                        case (0x28):
                            PLP();
                            break;

                            #endregion

                            #region ROL

                        case (0x26):
                            M = ZeroPage(arg1);
                            ZeroPageWrite(arg1, ROL(M));
                            CycleCounter += 5;
                            REG_PC += 2;
                            break;
                        case (0x2a):
                            M = REG_A;
                            REG_A = ROL(M);
                            CycleCounter += 2;
                            REG_PC += 1;
                            break;
                        case (0x2e):
                            M = Absolute(arg1, arg2);
                            AbsoluteWrite(arg1, arg2, ROL(M));
                            CycleCounter += 6;
                            REG_PC += 3;
                            break;
                        case (0x36):
                            M = ZeroPageX(arg1);
                            ZeroPageXWrite(arg1, ROL(M));
                            CycleCounter += 6;
                            REG_PC += 2;
                            break;
                        case (0x3e):
                            M = AbsoluteX(arg1, arg2, false);
                            AbsoluteXWrite(arg1, arg2, ROL(M));
                            CycleCounter += 7;
                            REG_PC += 3;
                            break;

                            #endregion

                            #region ROR

                        case (0x66):
                            M = ZeroPage(arg1);
                            ZeroPageWrite(arg1, ROR(M));
                            CycleCounter += 5;
                            REG_PC += 2;
                            break;
                        case (0x6a):
                            M = REG_A;
                            REG_A = ROR(M);
                            CycleCounter += 2;
                            REG_PC += 1;
                            break;
                        case (0x6e):
                            M = Absolute(arg1, arg2);
                            AbsoluteWrite(arg1, arg2, ROR(M));
                            CycleCounter += 6;
                            REG_PC += 3;
                            break;
                        case (0x76):
                            M = ZeroPageX(arg1);
                            ZeroPageXWrite(arg1, ROR(M));
                            CycleCounter += 6;
                            REG_PC += 2;
                            break;
                        case (0x7e):
                            M = AbsoluteX(arg1, arg2, false);
                            AbsoluteXWrite(arg1, arg2, ROR(M));
                            CycleCounter += 7;
                            REG_PC += 3;
                            break;

                            #endregion

                            #region RIT

                        case (0x40):
                            RTI();
                            break;

                            #endregion

                            #region RTS

                        case (0x60):
                            RTS();
                            break;

                            #endregion

                            #region SBC

                        case (0xe1):
                            M = IndirectX(arg1);
                            SBC(M, 6, 2);
                            break;
                        case (0xe5):
                            M = ZeroPage(arg1);
                            SBC(M, 3, 2);
                            break;
                        case (0xe9):
                            M = arg1;
                            SBC(M, 2, 2);
                            break;
                        case (0xed):
                            M = Absolute(arg1, arg2);
                            SBC(M, 4, 3);
                            break;
                        case (0xf1):
                            M = IndirectY(arg1, false);
                            SBC(M, 5, 2);
                            break;
                        case (0xf5):
                            M = ZeroPageX(arg1);
                            SBC(M, 4, 2);
                            break;
                        case (0xf9):
                            M = AbsoluteY(arg1, arg2, true);
                            SBC(M, 4, 3);
                            break;
                        case (0xfd):
                            M = AbsoluteX(arg1, arg2, true);
                            SBC(M, 4, 3);
                            break;

                            #endregion

                            #region SEC

                        case (0x38):
                            SEC();
                            break;

                            #endregion

                            #region SED

                        case (0xf8):
                            SED();
                            break;

                            #endregion

                            #region SEI

                        case (0x78):
                            SEI();
                            break;

                            #endregion

                            #region STA

                        case (0x85):
                            ZeroPageWrite(arg1, REG_A);
                            CycleCounter += 3;
                            REG_PC += 2;
                            break;
                        case (0x95):
                            ZeroPageXWrite(arg1, REG_A);
                            CycleCounter += 4;
                            REG_PC += 2;
                            break;
                        case (0x8D):
                            AbsoluteWrite(arg1, arg2, REG_A);
                            CycleCounter += 4;
                            REG_PC += 3;
                            break;
                        case (0x9D):
                            AbsoluteXWrite(arg1, arg2, REG_A);
                            CycleCounter += 5;
                            REG_PC += 3;
                            break;
                        case (0x99):
                            AbsoluteYWrite(arg1, arg2, REG_A);
                            CycleCounter += 5;
                            REG_PC += 3;
                            break;
                        case (0x81):
                            IndirectXWrite(arg1, REG_A);
                            CycleCounter += 6;
                            REG_PC += 2;
                            break;
                        case (0x91):
                            IndirectYWrite(arg1, REG_A);
                            CycleCounter += 6;
                            REG_PC += 2;
                            break;

                            #endregion

                            #region STX

                        case (0x86):
                            ZeroPageWrite(arg1, REG_X);
                            CycleCounter += 3;
                            REG_PC += 2;
                            break;
                        case (0x96):
                            ZeroPageYWrite(arg1, REG_X);
                            CycleCounter += 4;
                            REG_PC += 2;
                            break;
                        case (0x8E):
                            AbsoluteWrite(arg1, arg2, REG_X);
                            CycleCounter += 4;
                            REG_PC += 3;
                            break;

                            #endregion

                            #region STY

                        case (0x84):
                            ZeroPageWrite(arg1, REG_Y);
                            CycleCounter += 3;
                            REG_PC += 2;
                            break;
                        case (0x94):
                            ZeroPageXWrite(arg1, REG_Y);
                            CycleCounter += 4;
                            REG_PC += 2;
                            break;
                        case (0x8C):
                            AbsoluteWrite(arg1, arg2, REG_Y);
                            CycleCounter += 4;
                            REG_PC += 3;
                            break;

                            #endregion

                            #region TAX

                        case (0xaa):
                            TAX();
                            break;

                            #endregion

                            #region TAY

                        case (0xa8):
                            TAY();
                            break;

                            #endregion

                            #region TSX

                        case (0xba):
                            TSX();
                            break;

                            #endregion

                            #region TXA

                        case (0x8a):
                            TXA();
                            break;

                            #endregion

                            #region TXS

                        case (0x9a):
                            TXS();
                            break;

                            #endregion

                            #region TYA

                        case (0x98):
                            TYA();
                            break;

                            #endregion

                            /*Illegal Opcodes*/

                            #region AAC

                        case 0x0B:
                        case 0x2B:
                            M = arg1;
                            AAC(M, 2, 2);
                            break;

                            #endregion

                            #region AAX

                        case 0x87:
                            M = ZeroPage(arg1);
                            ZeroPageWrite(arg1, AAX(M));
                            REG_PC += 2;
                            CycleCounter += 3;
                            break;
                        case 0x97:
                            M = ZeroPageY(arg1);
                            ZeroPageYWrite(arg1, AAX(M));
                            REG_PC += 2;
                            CycleCounter += 4;
                            break;
                        case 0x83:
                            M = IndirectX(arg1);
                            IndirectXWrite(arg1, AAX(M));
                            REG_PC += 2;
                            CycleCounter += 6;
                            break;
                        case 0x8F:
                            M = Absolute(arg1, arg2);
                            AbsoluteWrite(arg1, arg2, AAX(M));
                            REG_PC += 3;
                            CycleCounter += 4;
                            break;

                            #endregion

                            #region ARR

                        case 0x6B:
                            M = arg1;
                            ARR(M);
                            break;

                            #endregion

                            #region ASR

                        case 0x4B:
                            M = arg1;
                            ASR(M);
                            break;

                            #endregion

                            #region ATX

                        case 0xAB:
                            M = arg1;
                            ATX(M);
                            break;

                            #endregion

                            #region AXA

                        case 0x9F:
                            M = AbsoluteY(arg1, arg2, false);
                            AbsoluteYWrite(arg1, arg2, AXA(M));
                            REG_PC += 3;
                            CycleCounter += 5;
                            break;
                        case 0x93:
                            M = IndirectY(arg1, false);
                            IndirectYWrite(arg1, AXA(M));
                            REG_PC += 2;
                            CycleCounter += 6;
                            break;

                            #endregion

                            #region AXS

                        case 0xCB:
                            M = arg1;
                            AXS(M);
                            break;

                            #endregion

                            #region DCP

                        case 0xC7:
                            M = ZeroPage(arg1);
                            DCP(M, 5, 2);
                            break;
                        case 0xD7:
                            M = ZeroPageX(arg1);
                            DCP(M, 6, 2);
                            break;
                        case 0xCF:
                            M = Absolute(arg1, arg2);
                            DCP(M, 6, 3);
                            break;
                        case 0xDF:
                            M = AbsoluteX(arg1, arg2, false);
                            DCP(M, 7, 3);
                            break;
                        case 0xDB:
                            M = AbsoluteY(arg1, arg2, false);
                            DCP(M, 7, 3);
                            break;
                        case 0xC3:
                            M = IndirectX(arg1);
                            DCP(M, 8, 2);
                            break;
                        case 0xD3:
                            M = IndirectY(arg1, false);
                            DCP(M, 8, 2);
                            break;

                            #endregion

                            #region DOP

                        case 0x14:
                        case 0x54:
                        case 0x74:
                        case 0xD4:
                        case 0xF4:
                        case 0x34:
                            DOP(4, 2);
                            break;
                        case 0x04:
                        case 0x64:
                        case 0x44:
                            DOP(3, 2);
                            break;
                        case 0x82:
                        case 0x89:
                        case 0xC2:
                        case 0xE2:
                        case 0x80:
                            DOP(2, 2);
                            break;

                            #endregion

                            #region ISC

                        case 0xE7:
                            M = ZeroPage(arg1);
                            ISC(M, 5, 2);
                            break;
                        case 0xF7:
                            M = ZeroPageX(arg1);
                            ISC(M, 6, 2);
                            break;
                        case 0xEF:
                            M = Absolute(arg1, arg2);
                            ISC(M, 6, 3);
                            break;
                        case 0xFF:
                            M = AbsoluteX(arg1, arg2, false);
                            ISC(M, 7, 3);
                            break;
                        case 0xFB:
                            M = AbsoluteY(arg1, arg2, false);
                            ISC(M, 7, 3);
                            break;
                        case 0xE3:
                            M = IndirectX(arg1);
                            ISC(M, 8, 2);
                            break;
                        case 0xF3:
                            M = IndirectY(arg1, false);
                            ISC(M, 8, 2);
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
                        case 0xF2:
                            KIL();
                            break;

                            #endregion

                            #region LAR

                        case 0xBB:
                            M = AbsoluteY(arg1, arg2, true);
                            LAR(M, 4, 2);
                            break;

                            #endregion

                            #region LAX

                        case 0xA7:
                            M = ZeroPage(arg1);
                            LAX(M, 3, 2);
                            break;
                        case 0xB7:
                            M = ZeroPageY(arg1);
                            LAX(M, 4, 2);
                            break;
                        case 0xAF:
                            M = Absolute(arg1, arg2);
                            LAX(M, 4, 3);
                            break;
                        case 0xBF:
                            M = AbsoluteY(arg1, arg2, true);
                            LAX(M, 4, 3);
                            break;
                        case 0xA3:
                            M = IndirectX(arg1);
                            LAX(M, 6, 2);
                            break;
                        case 0xB3:
                            M = IndirectY(arg1, true);
                            LAX(M, 5, 2);
                            break;

                            #endregion

                            #region NOP

                        case 0x1A:
                        case 0x3A:
                        case 0x5A:
                        case 0x7A:
                        case 0xDA:
                        case 0xFA:
                            NOP();
                            break;

                            #endregion

                            #region RLA

                        case 0x27:
                            M = ZeroPage(arg1);
                            RLA(M, 5, 2);
                            break;
                        case 0x37:
                            M = ZeroPageX(arg1);
                            RLA(M, 6, 2);
                            break;
                        case 0x2F:
                            M = Absolute(arg1, arg2);
                            RLA(M, 6, 3);
                            break;
                        case 0x3F:
                            M = AbsoluteX(arg1, arg2, false);
                            RLA(M, 7, 3);
                            break;
                        case 0x3B:
                            M = AbsoluteY(arg1, arg2, false);
                            RLA(M, 7, 3);
                            break;
                        case 0x23:
                            M = IndirectX(arg1);
                            RLA(M, 8, 2);
                            break;
                        case 0x33:
                            M = IndirectY(arg1, false);
                            RLA(M, 8, 2);
                            break;

                            #endregion

                            #region RRA

                        case 0x67:
                            M = ZeroPage(arg1);
                            RRA(M, 5, 2);
                            break;
                        case 0x77:
                            M = ZeroPageX(arg1);
                            RRA(M, 6, 2);
                            break;
                        case 0x6F:
                            M = Absolute(arg1, arg2);
                            RRA(M, 6, 3);
                            break;
                        case 0x7F:
                            M = AbsoluteX(arg1, arg2, false);
                            RRA(M, 7, 3);
                            break;
                        case 0x7B:
                            M = AbsoluteY(arg1, arg2, false);
                            RRA(M, 7, 3);
                            break;
                        case 0x63:
                            M = IndirectX(arg1);
                            RRA(M, 8, 2);
                            break;
                        case 0x73:
                            M = IndirectY(arg1, false);
                            RRA(M, 8, 2);
                            break;

                            #endregion

                            #region SBC

                        case 0xEB:
                            M = arg1;
                            SBC(M, 2, 2);
                            break;

                            #endregion

                            #region SLO

                        case 0x07:
                            M = ZeroPage(arg1);
                            SLO(M, 5, 2);
                            break;
                        case 0x17:
                            M = ZeroPageX(arg1);
                            SLO(M, 6, 2);
                            break;
                        case 0x0F:
                            M = Absolute(arg1, arg2);
                            SLO(M, 6, 3);
                            break;
                        case 0x1F:
                            M = AbsoluteX(arg1, arg2, false);
                            SLO(M, 7, 3);
                            break;
                        case 0x1B:
                            M = AbsoluteY(arg1, arg2, false);
                            SLO(M, 7, 3);
                            break;
                        case 0x03:
                            M = IndirectX(arg1);
                            SLO(M, 8, 2);
                            break;
                        case 0x13:
                            M = IndirectY(arg1, false);
                            SLO(M, 8, 2);
                            break;

                            #endregion

                            #region SRE

                        case 0x47:
                            SRE(ZeroPage(arg1), 5, 2);
                            break;
                        case 0x57:
                            SRE(ZeroPageX(arg1), 6, 2);
                            break;
                        case 0x4F:
                            SRE(Absolute(arg1, arg2), 6, 3);
                            break;
                        case 0x5F:
                            SRE(AbsoluteX(arg1, arg2, false), 7, 3);
                            break;
                        case 0x5B:
                            SRE(AbsoluteY(arg1, arg2, false), 7, 3);
                            break;
                        case 0x43:
                            SRE(IndirectX(arg1), 8, 2);
                            break;
                        case 0x53:
                            SRE(IndirectY(arg1, false), 8, 2);
                            break;

                            #endregion

                            #region SXA

                        case 0x9E:
                            AbsoluteYWrite(arg1, arg2, SXA(arg1));
                            break;

                            #endregion

                            #region SYA

                        case 0x9C:
                            AbsoluteXWrite(arg1, arg2, SYA(arg1));
                            break;

                            #endregion

                            #region TOP

                        case 0x0C:
                            TOP(4, 3);
                            break;
                        case 0x1C:
                        case 0x3C:
                        case 0x5C:
                        case 0x7C:
                        case 0xDC:
                        case 0xFC:
                            AbsoluteX(arg1, arg2, true);
                            TOP(4, 3);
                            break;

                            #endregion

                            #region XAA

                        case 0x8B:
                            XAA(2, 2);
                            break;

                            #endregion

                            #region XAS

                        case 0x9B:
                            M = AbsoluteY(arg1, arg2, false);
                            AbsoluteYWrite(arg1, arg2, XAS(M, arg1));
                            CycleCounter += 5;
                            REG_PC += 3;
                            break;

                            #endregion

                        default: //Should not reach here, if it, shutdown the system
                            Debug.WriteLine(this, "<UNKOWN OPCODE> 0x" +
                                                  string.Format("{0:X}", OpCode), DebugStatus.Error);
                            _engine.ShutDown();
                            break;
                    }

                    #endregion

                    _engine.Apu.Play(); //If the sound is paused
                }
                else
                {
                    Thread.Sleep(100);
                    _engine.Apu.Pause();
                    Paused = true;
                }
                if (CycleCounter >= CyclesPerScanline)
                {
                    if (_engine.Ppu.DoScanline()) /*NMI*/
                    {
                        Flag_B = false;
                        Push16(REG_PC);
                        PushStatus();
                        Flag_I = true;
                        REG_PC = MEM.Read16(0xFFFA);
                    }
                    if (IRQNextTime) /*IRQ*/
                    {
                        IRQ();
                        IRQNextTime = false;
                    }
                    //CycleCounter -= CyclesPerScanline;
                    CycleCounter = 0; //?!! Just don't know what to do with the rest cycles !!
                }
            }
        }

        public void Reset()
        {
            REG_S = 0xFF;
            REG_A = 0;
            REG_X = 0;
            REG_Y = 0;
            Flag_N = false;
            Flag_V = false;
            Flag_B = false;
            Flag_D = false;
            Flag_I = true;
            Flag_Z = false;
            Flag_C = false;
            REG_PC = MEM.Read16(0xFFFC);
        }

        public void SoftReset()
        {
            Pause = true;
            REG_S -= 3;
            Flag_I = true;
            REG_PC = MEM.Read16(0xFFFC);
            _engine.Memory.Map.CurrentMapper.SoftReset();
            Pause = false;
        }

        /// <summary>
        /// Pause the cpu
        /// </summary>
        public void TogglePause()
        {
            _Pause = !_Pause;
            //Wait until the cpu pauses if the user resume the cpu loop
            if (_Pause)
                while (!Paused)
                {
                }
            //Rise the event
            if (PauseToggle != null)
                PauseToggle(this, null);
        }

        /// <summary>
        /// Initialize and set the timing of the cpu
        /// </summary>
        /// <param name="FORMAT">The tv format to set the cpu emulation for</param>
        private void InitializeCPU(TVFORMAT FORMAT)
        {
            switch (FORMAT)
            {
                case TVFORMAT.NTSC:
                    CyclesPerScanline = 113;
                    break;
                case TVFORMAT.PAL:
                    CyclesPerScanline = 106;
                    break;
            }
            REG_S = 0xFF;
            REG_A = 0;
            REG_X = 0;
            REG_Y = 0;
            Debug.WriteLine(this, "CPU initialized ok.", DebugStatus.Cool);
        }

        /// <summary>
        /// get two bytes into a correct address
        /// </summary>
        /// <param name="c">Byte A</param>
        /// <param name="d">Byte B</param>
        /// <returns>ushort A & B togather</returns>
        private ushort MakeAddress(byte c, byte d)
        {
            var New = (ushort) ((d << 8) | c);
            return New;
        }

        public void SetTVFormat(TVFORMAT FORMAT)
        {
            if (!_Pause)
                return;
            switch (FORMAT)
            {
                case TVFORMAT.NTSC:
                    CyclesPerScanline = 113;
                    break;
                case TVFORMAT.PAL:
                    CyclesPerScanline = 106;
                    break;
            }
        }

        //Properties

        /// <summary>
        /// Event rised when the cpu is paused or rusumed
        /// </summary>
        public event EventHandler<EventArgs> PauseToggle;
    }
}