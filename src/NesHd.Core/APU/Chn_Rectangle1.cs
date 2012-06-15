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
    public class Chn_Rectangle1
    {
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
        int _DutyCycle = 0;
        int _FreqTimer = 0;
        byte _DecayCount = 0;
        byte _DecayTimer = 0;
        bool _DecayDiable;
        bool _DecayReset;
        bool _DecayLoopEnable;
        byte _LengthCount = 0;
        byte _SweepShift = 0;
        bool _SweepDirection;
        byte _SweepRate = 0;
        bool _SweepEnable;
        byte _SweepCount = 0;
        bool _SweepReset;
        bool _SweepForceSilence;
        double DutyPercentage = 0;
        bool WaveStatus;
        /// <summary>
        /// Check to see if the sweep unit is forcing the channel to be silent 
        /// and update the frequency if not.
        /// </summary>
        void CheckSweepForceSilence()
        {
            this._SweepForceSilence = false;
            if (this._FreqTimer < 8)
                this._SweepForceSilence = true;
            else if (!this._SweepDirection)
            {
                if ((this._FreqTimer & 0x0800) != 0)
                    this._SweepForceSilence = true;
            }
            //Update Frequency
            if (!this._SweepForceSilence)
            {
                this._Frequency = 1790000 / 16 / (this._FreqTimer + 1);
                this._RenderedLength = 44100 / this._Frequency;
            }
        }
        public void UpdateLengthCounter()
        {
            //Length counter
            if (!this._DecayLoopEnable & this._LengthCount > 0)
                this._LengthCount--;
        }
        public void UpdateSweep()
        {
            if (this._SweepEnable & !this._SweepForceSilence)
            {
                if (this._SweepCount > 0)
                    this._SweepCount--;
                else
                {
                    this._SweepCount = this._SweepRate;
                    if (this._SweepDirection)
                        this._FreqTimer -= (this._FreqTimer >> this._SweepShift) + 1;
                    else
                        this._FreqTimer += (this._FreqTimer >> this._SweepShift);
                    this.CheckSweepForceSilence();
                }
            }
            if (this._SweepReset)
            {
                this._SweepReset = false;
                this._SweepCount = this._SweepRate;
            }
        }
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
        public short RenderSample()
        {
            if (this._LengthCount > 0 & !this._SweepForceSilence)
            {
                this._SampleCount++;
                if (this.WaveStatus && (this._SampleCount > (this._RenderedLength * this.DutyPercentage)))
                {
                    this._SampleCount -= this._RenderedLength * this.DutyPercentage;
                    this.WaveStatus = !this.WaveStatus;
                }
                else if (!this.WaveStatus && (this._SampleCount > (this._RenderedLength * (1.0 - this.DutyPercentage))))
                {
                    this._SampleCount -= this._RenderedLength * (1.0 - this.DutyPercentage);
                    this.WaveStatus = !this.WaveStatus;
                }
                if (this.WaveStatus)
                {
                    return (short)(-1 * (this._DecayDiable ? this._Volume : this._Envelope));
                }
                return (this._DecayDiable ? this._Volume : this._Envelope);
            }
            return 0;
        }
        #region Registers
        public void Write_4000(byte data)
        {
            this._DecayDiable = ((data & 0x10) != 0);//bit 4
            this._DecayLoopEnable = ((data & 0x20) != 0);//bit 5
            this._DutyCycle = (data & 0xC0) >> 6;
            if (this._DutyCycle == 0)
                this.DutyPercentage = 0.125;
            else if (this._DutyCycle == 1)
                this.DutyPercentage = 0.25;
            else if (this._DutyCycle == 2)
                this.DutyPercentage = 0.5;
            else if (this._DutyCycle == 3)
                this.DutyPercentage = 0.75;
            //Decay / Volume
            this._DecayTimer = (byte)(data & 0x0F);//bit 0 - 3
            if (this._DecayDiable)
                this._Volume = this._DecayTimer;
            else
                this._Volume = this._Envelope;

        }
        public void Write_4001(byte data)
        {
            this._SweepShift = (byte)(data & 0x7);//bit 0 - 2
            this._SweepDirection = ((data & 0x8) != 0);//bit 3
            this._SweepRate = (byte)((data & 0x70) >> 4);//bit 4 - 6
            this._SweepEnable = ((data & 0x80) != 0 & (this._SweepShift != 0));//bit 7
            this._SweepReset = true;
            this.CheckSweepForceSilence();
        }
        public void Write_4002(byte data)
        {
            this._FreqTimer = ((this._FreqTimer & 0x0700) | data);
            this.CheckSweepForceSilence();
        }
        public void Write_4003(byte data)
        {
            this._FreqTimer = ((this._FreqTimer & 0x00FF) | (data & 0x07) << 8);//Bit 0 - 2
            //if (_Enabled)
            this._LengthCount = this.LENGTH_COUNTER_TABLE[(data & 0xF8) >> 3];//bit 3 - 7 
            this._DecayReset = true;
            this.CheckSweepForceSilence();
        }
        #endregion
        #region Properties
        public bool Enabled
        {
            get
            {
                return (this._LengthCount > 0);
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
            st.Rectangle1_Enabled = this._Enabled;
            st.Rectangle1_Volume = this._Volume;
            st.Rectangle1_Envelope = this._Envelope;
            st.Rectangle1_Frequency = this._Frequency;
            st.Rectangle1_SampleCount = this._SampleCount;
            st.Rectangle1_RenderedLength = this._RenderedLength;
            st.Rectangle1_DutyCycle = this._DutyCycle;
            st.Rectangle1_FreqTimer = this._FreqTimer;
            st.Rectangle1_DecayCount = this._DecayCount;
            st.Rectangle1_DecayTimer = this._DecayTimer;
            st.Rectangle1_DecayDiable = this._DecayDiable;
            st.Rectangle1_DecayReset = this._DecayReset;
            st.Rectangle1_DecayLoopEnable = this._DecayLoopEnable;
            st.Rectangle1_LengthCount = this._LengthCount;
            st.Rectangle1_SweepShift = this._SweepShift;
            st.Rectangle1_SweepDirection = this._SweepDirection;
            st.Rectangle1_SweepRate = this._SweepRate;
            st.Rectangle1_SweepEnable = this._SweepEnable;
            st.Rectangle1_SweepCount = this._SweepCount;
            st.Rectangle1_SweepReset = this._SweepReset;
            st.Rectangle1_SweepForceSilence = this._SweepForceSilence;
            st.Rectangle1DutyPercentage = this.DutyPercentage;
            st.Rectangle1WaveStatus = this.WaveStatus;
        }
        public void LoadState(StateHolder st)
        {
            this._Enabled = st.Rectangle1_Enabled;
            this._Volume = st.Rectangle1_Volume;
            this._Envelope = st.Rectangle1_Envelope;
            this._Frequency = st.Rectangle1_Frequency;
            this._SampleCount = st.Rectangle1_SampleCount;
            this._RenderedLength = st.Rectangle1_RenderedLength;
            this._DutyCycle = st.Rectangle1_DutyCycle;
            this._FreqTimer = st.Rectangle1_FreqTimer;
            this._DecayCount = st.Rectangle1_DecayCount;
            this._DecayTimer = st.Rectangle1_DecayTimer;
            this._DecayDiable = st.Rectangle1_DecayDiable;
            this._DecayReset = st.Rectangle1_DecayReset;
            this._DecayLoopEnable = st.Rectangle1_DecayLoopEnable;
            this._LengthCount = st.Rectangle1_LengthCount;
            this._SweepShift = st.Rectangle1_SweepShift;
            this._SweepDirection = st.Rectangle1_SweepDirection;
            this._SweepRate = st.Rectangle1_SweepRate;
            this._SweepEnable = st.Rectangle1_SweepEnable;
            this._SweepCount = st.Rectangle1_SweepCount;
            this._SweepReset = st.Rectangle1_SweepReset;
            this._SweepForceSilence = st.Rectangle1_SweepForceSilence;
            this.DutyPercentage = st.Rectangle1DutyPercentage;
            this.WaveStatus = st.Rectangle1WaveStatus;
        }
    }
}