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
    public class Chn_Noize
    {
        double[] NOISE_FREQUENCY_TABLE = 
        { 
 0x002, 0x004, 0x008, 0x010, 0x020, 0x030, 0x040, 0x050, 
 0x065, 0x07F, 0x0BE, 0x0FE, 0x17D, 0x1FC, 0x3F9, 0x7F2/*, 0xFE4*/
        };
        byte[] LENGTH_COUNTER_TABLE = 
        {
 0x5,0x7f,0xA,0x1,0x14,0x2,0x28,0x3,0x50,0x4,0x1E,0x5,0x7,0x6,0x0E,0x7,
 0x6,0x08,0xC,0x9,0x18,0xa,0x30,0xb,0x60,0xc,0x24,0xd,0x8,0xe,0x10,0xf
        };
        bool _Enabled;
        byte _Volume = 0;
        byte _Envelope = 0;
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        double _FreqTimer = 0;
        byte _LengthCount = 0;
        ushort _ShiftReg = 1;
        byte _DecayCount = 0;
        byte _DecayTimer = 0;
        bool _DecayDiable;
        int _NoiseMode;
        bool _DecayLoopEnable;
        bool _DecayReset = false;
        short OUT = 0;
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (!this._DecayLoopEnable & this._LengthCount > 0)
                this._LengthCount--;
        }
        /// <summary>
        /// Update Envelope / Decay / Linear Counter
        /// </summary>
        public void UpdateEnvelope()
        {
            if (this._DecayReset)
            {
                this._DecayCount = this._DecayTimer;
                this._Envelope = 0x0F;
                this._DecayReset = false;
                if (!this._DecayDiable)
                    this._Volume = 0x0F;
            }
            else
            {
                if (this._DecayCount > 0)
                    this._DecayCount--;
                else
                {
                    this._DecayCount = this._DecayTimer;
                    if (this._Envelope > 0)
                        this._Envelope--;
                    else if (this._DecayLoopEnable)
                        this._Envelope = 0x0F;

                    if (!this._DecayDiable)
                        this._Volume = this._Envelope;
                }
            }

        }
        //Do NOZ samples
        public short RenderSample()
        {
            if (this._LengthCount > 0)
            {
                this._SampleCount++;
                if (this._SampleCount >= this._RenderedLength)
                {
                    this._SampleCount -= this._RenderedLength;
                    this._ShiftReg <<= 1;
                    this._ShiftReg |= (ushort)(((this._ShiftReg >> 15) ^ (this._ShiftReg >> this._NoiseMode)) & 1);
                }
                this.OUT = (short)((this._DecayDiable ? this._Volume : this._Envelope));
                if ((this._ShiftReg & 1) == 0)
                    this.OUT *= -1;
                return this.OUT;
            }

            return 0;
        }
        void UpdateFrequency()
        {
            this._Frequency = 1790000 / 2 / (this._FreqTimer + 1);
            if (this._FreqTimer > 4)
                this._RenderedLength = 44100 / this._Frequency;
        }
        #region Registerts
        public void Write_400C(byte data)
        {
            this._DecayTimer = (byte)(data & 0xF);//bit 0 - 3
            this._DecayDiable = ((data & 0x10) != 0);//bit 4
            this._DecayLoopEnable = ((data & 0x20) != 0);//bit 5
            if (this._DecayDiable)
                this._Volume = this._DecayTimer;
            else
                this._Volume = this._Envelope;
            this.UpdateFrequency();
        }
        public void Write_400E(byte data)
        {
            this._FreqTimer = this.NOISE_FREQUENCY_TABLE[data & 0x0F];//bit 0 - 3
            this._NoiseMode = ((data & 0x80) != 0) ? 9 : 14;//bit 7
            this.UpdateFrequency();
        }
        public void Write_400F(byte data)
        {
            this._LengthCount = this.LENGTH_COUNTER_TABLE[data >> 3];//bit 3 - 7
            this._DecayReset = true;
            this.UpdateFrequency();
        }
        #endregion
        #region Properties
        public bool Enabled
        {
            get
            {
                this._Enabled = (this._LengthCount > 0);
                return this._Enabled;
            }
            set
            {
                this._Enabled = value;
                if (!value)
                    this._LengthCount = 0;
            }
        }
        #endregion

        public void SaveState(StateHolder st)
        {
            st.NOIZE_Enabled = this._Enabled;
            st.NOIZE_Volume = this._Volume;
            st.NOIZE_Envelope = this._Envelope;
            st.NOIZE_Frequency = this._Frequency;
            st.NOIZE_SampleCount = this._SampleCount;
            st.NOIZE_RenderedLength = this._RenderedLength;
            st.NOIZE_FreqTimer = this._FreqTimer;
            st.NOIZE_LengthCount = this._LengthCount;
            st.NOIZE_ShiftReg = this._ShiftReg;
            st.NOIZE_DecayCount = this._DecayCount;
            st.NOIZE_DecayTimer = this._DecayTimer;
            st.NOIZE_DecayDiable = this._DecayDiable;
            st.NOIZE_NoiseMode = this._NoiseMode;
            st.NOIZE_DecayLoopEnable = this._DecayLoopEnable;
            st.NOIZE_DecayReset = this._DecayReset;
            st.NOIZEOUT = this.OUT;
        }
        public void LoadState(StateHolder st)
        {
            this._Enabled = st.NOIZE_Enabled;
            this._Volume = st.NOIZE_Volume;
            this._Envelope = st.NOIZE_Envelope;
            this._Frequency = st.NOIZE_Frequency;
            this._SampleCount = st.NOIZE_SampleCount;
            this._RenderedLength = st.NOIZE_RenderedLength;
            this._FreqTimer = st.NOIZE_FreqTimer;
            this._LengthCount = st.NOIZE_LengthCount;
            this._ShiftReg = st.NOIZE_ShiftReg;
            this._DecayCount = st.NOIZE_DecayCount;
            this._DecayTimer = st.NOIZE_DecayTimer;
            this._DecayDiable = st.NOIZE_DecayDiable;
            this._NoiseMode = st.NOIZE_NoiseMode;
            this._DecayLoopEnable = st.NOIZE_DecayLoopEnable;
            this._DecayReset = st.NOIZE_DecayReset;
            this.OUT = st.NOIZEOUT;
        }
    }
}