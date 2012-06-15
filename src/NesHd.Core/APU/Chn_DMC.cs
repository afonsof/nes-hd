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

using NesHd.Core.Misc;

namespace NesHd.Core.APU
{
    public class Chn_DMC
    {
        public Chn_DMC(NesEngine NesEmu)
        {
            this._Nes = NesEmu;
        }
        double[] DMC_FREQUENCY = 
        { 
        0xD60,0xBE0,0xAA0,0xA00,0x8F0,0x7F0,0x710,0x6B0,
        0x5F0,0x500,0x470,0x400,0x350,0x2A8,0x240,0x1B0
        };
        NesEngine _Nes;
        double _Frequency = 0;
        double _RenderedLength = 0;
        double _SampleCount = 0;

        public bool DMCIRQEnabled = false;
        bool _Enabled = false;
        bool _Loop = false;
        double _FreqTimer = 0;
        byte DAC = 0;

        ushort DMAStartAddress = 0;
        ushort DMAAddress = 0;

        ushort DMALength = 0;
        ushort DMALengthCounter = 0;

        byte DMCBIT = 0;
        byte DMCBYTE = 0;

        public short RenderSample()
        {
            if (this._Enabled)
            {
                this._SampleCount++;
                if (this._SampleCount > this._RenderedLength)
                {
                    this._SampleCount -= this._RenderedLength;
                    if (this.DMCBIT == 7)
                    {
                        if (this.DMALength > 0)
                        {
                            this.DMCBIT = 0;
                            this.DMCBYTE = this._Nes.MEMORY.Read(this.DMAAddress);
                            this.DMAAddress++;
                            this.DMALength--;
                            if (this.DMALength <= 0 & this._Loop)
                            {
                                this.DMAAddress = this.DMAStartAddress;
                                this.DMALength = this.DMALengthCounter;
                            }
                            if (this.DMALength <= 0 & !this._Loop & this.DMCIRQEnabled)
                            {
                                this._Nes.APU.DMCIRQPending = true;
                                this._Nes.CPU.IRQNextTime = true;
                            }
                        }
                        else
                        {
                            this._Enabled = false;
                        }
                    }
                    else
                    {
                        this.DMCBIT++;
                        this.DMCBYTE >>= 1;
                    }
                    if (this._Enabled)
                    {
                        if ((this.DMCBYTE & 1) != 0)
                        {
                            if (this.DAC < 0x7E)
                            { this.DAC++; }
                        }
                        else if (this.DAC > 1)
                        { this.DAC--; }
                    }
                }
                if (this.DAC > 25)
                    this.DAC = 25;
                return (short)((this.DAC - 14) * 2);
            }
            return 0;
        }
        void UpdateFrequency()
        {
            this._Frequency = 1790000 / (this._FreqTimer + 1) * 8;
            this._RenderedLength = 44100 / this._Frequency;
        }
        #region Registers
        public void Write_4010(byte data)
        {
            this.DMCIRQEnabled = (data & 0x80) != 0;//Bit 7
            this._Loop = (data & 0x40) != 0;//Bit 6
            //IRQ
            if (!this.DMCIRQEnabled)
                this._Nes.APU.DMCIRQPending = false;

            this._FreqTimer = this.DMC_FREQUENCY[data & 0xF];//Bit 0 - 3
            this.UpdateFrequency();
        }
        public void Write_4011(byte data)
        {
            this.DAC = (byte)(data & 0x7f); 
            this.UpdateFrequency();
        }
        public void Write_4012(byte data)
        {
            this.DMAStartAddress = (ushort)((data * 0x40) + 0xC000); 
            this.UpdateFrequency();
        }
        public void Write_4013(byte data)
        {
            this.DMALengthCounter = (ushort)((data * 0x10) + 1); 
            this.UpdateFrequency();
        }
        #endregion
        #region Properties
        public bool Enabled
        {
            get
            {
                this._Enabled = this.DMALength > 0;
                return this._Enabled;
            }
            set
            {
                this._Enabled = value;
                if (value)
                {
                    if (this.DMALength <= 0)
                    {
                        this.DMALength = this.DMALengthCounter;
                        this.DMCBIT = 7;
                        this.DMAAddress = this.DMAStartAddress;
                    }
                }
                else
                {
                    this.DMALength = 0;
                }
            }
        }
        #endregion

        public void SaveState(StateHolder st)
        {
            st.DMC_Frequency = this._Frequency;
            st.DMC_RenderedLength = this._RenderedLength;
            st.DMC_SampleCount = this._SampleCount;
            st.DMCDMCIRQEnabled = this.DMCIRQEnabled;
            st.DMC_Enabled = this._Enabled;
            st.DMC_Loop = this._Loop;
            st.DMC_FreqTimer = this._FreqTimer;
            st.DMCDAC = this.DAC;
            st.DMCDMAStartAddress = this.DMAStartAddress;
            st.DMCDMAAddress = this.DMAAddress;
            st.DMCDMALength = this.DMALength;
            st.DMCDMALengthCounter = this.DMALengthCounter;
            st.DMCDMCBIT = this.DMCBIT;
            st.DMCDMCBYTE = this.DMCBYTE;
        }
        public void LoadState(StateHolder st)
        {
            this._Frequency = st.DMC_Frequency;
            this._RenderedLength = st.DMC_RenderedLength;
            this._SampleCount = st.DMC_SampleCount;
            this.DMCIRQEnabled = st.DMCDMCIRQEnabled;
            this._Enabled = st.DMC_Enabled;
            this._Loop = st.DMC_Loop;
            this._FreqTimer = st.DMC_FreqTimer;
            this.DAC = st.DMCDAC;
            this.DMAStartAddress = st.DMCDMAStartAddress;
            this.DMAAddress = st.DMCDMAAddress;
            this.DMALength = st.DMCDMALength;
            this.DMALengthCounter = st.DMCDMALengthCounter;
            this.DMCBIT = st.DMCDMCBIT;
            this.DMCBYTE = st.DMCDMCBYTE;
        }
    }
}
