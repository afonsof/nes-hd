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
    public class Chn_Triangle
    {
        /*byte[] LENGTH_COUNTER_TABLE = 
             {
 0x5*2,0x7f*2,0xA*2,0x1*2,0x14*2,0x2*2,0x28*2,0x3*2,0x50*2,0x4*2,0x1E*2,0x5*2,0x7*2,0x6*2,0x0E*2,0x7*2,
 0x6*2,0x08*2,0xC*2,0x9*2,0x18*2,0xa*2,0x30*2,0xb*2,0x60*2,0xc*2,0x24*2,0xd*2,0x8*2,0xe*2,0x10*2,0xf*2
             };*/
        byte[] LENGTH_COUNTER_TABLE = 
             {
 0x5,0x7f,0xA,0x1,0x14,0x2,0x28,0x3,0x50,0x4,0x1E,0x5,0x7,0x6,0x0E,0x7,
 0x6,0x08,0xC,0x9,0x18,0xa,0x30,0xb,0x60,0xc,0x24,0xd,0x8,0xe,0x10,0xf
             };
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        int _FreqTimer = 0;
        byte _LengthCount = 0;
        int _LinearCounter = 0;
        int _LinearCounterLoad = 0;
        bool _LinearControl;
        bool _LengthEnabled;
        int _Sequence = 0;
        bool HALT;
        bool _Enabled;
        short OUT = 0;
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (this._LengthEnabled & this._LengthCount > 0)
                this._LengthCount--;
        }
        //Linear Counter
        public void UpdateEnvelope()
        {
            if (this.HALT)
                this._LinearCounter = this._LinearCounterLoad;
            else if (this._LinearCounter > 0)
                this._LinearCounter--;
            if (!this._LinearControl)
                this.HALT = false;
        }
        public short RenderSample()
        {
            if (this._LinearCounter > 0 & this._LengthCount > 0 & this._FreqTimer >= 8)
            {
                this._SampleCount++;
                if (this._SampleCount >= this._RenderedLength)
                {
                    this._SampleCount -= this._RenderedLength;
                    this._Sequence = (this._Sequence + 1) & 0x1F;
                    if ((this._Sequence & 0x10) != 0)
                        this.OUT = (short)(this._Sequence ^ 0x1F);
                    else
                        this.OUT = (short)this._Sequence;
                    this.OUT -= 7;
                    this.OUT *= 2;
                }
                return this.OUT;
            }
            if (this.OUT > 0)
                this.OUT--;
            else if (this.OUT < 0)
                this.OUT++;
            return this.OUT;
        }
        #region Registers
        public void Write_4008(byte data)
        {
            this._LinearCounterLoad = data & 0x7F;//Bit 0 - 6
            this._LinearControl = (data & 0x80) != 0;//Bit 7
            this._LengthEnabled = !this._LinearControl;
        }
        public void Write_400A(byte data)
        {
            this._FreqTimer = ((this._FreqTimer & 0x700) | data);
            //Update Frequency
            this._Frequency = 1790000 / (this._FreqTimer + 1);
            this._RenderedLength = 44100 / this._Frequency;
        }
        public void Write_400B(byte data)
        {
            this._FreqTimer = ((this._FreqTimer & 0xFF) | (data & 0x7) << 8);//Bit 0 - 2
            //Update Frequency
            this._Frequency = 1790000 / (this._FreqTimer + 1);
            this._RenderedLength = 44100 / this._Frequency;
            if (this._Enabled)
                this._LengthCount = this.LENGTH_COUNTER_TABLE[data >> 3];//bit 3 - 7 
            this.HALT = true;
        }
        #endregion
        #region Properties
        public bool Enabled
        {
            get { return (this._LengthCount > 0); }
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
            st.Triangle_Frequency = this._Frequency;
            st.Triangle_SampleCount = this._SampleCount;
            st.Triangle_RenderedLength = this._RenderedLength;
            st.Triangle_FreqTimer = this._FreqTimer;
            st.Triangle_LengthCount = this._LengthCount;
            st.Triangle_LinearCounter = this._LinearCounter;
            st.Triangle_LinearCounterLoad = this._LinearCounterLoad;
            st.Triangle_LinearControl = this._LinearControl;
            st.Triangle_LengthEnabled = this._LengthEnabled;
            st.Triangle_Sequence = this._Sequence;
            st.TriangleHALT = this.HALT;
            st.Triangle_Enabled = this._Enabled;
            st.TriangleOUT = this.OUT;
        }
        public void LoadState(StateHolder st)
        {
            this._Frequency = st.Triangle_Frequency;
            this._SampleCount = st.Triangle_SampleCount;
            this._RenderedLength = st.Triangle_RenderedLength;
            this._FreqTimer = st.Triangle_FreqTimer;
            this._LengthCount = st.Triangle_LengthCount;
            this._LinearCounter = st.Triangle_LinearCounter;
            this._LinearCounterLoad = st.Triangle_LinearCounterLoad;
            this._LinearControl = st.Triangle_LinearControl;
            this._LengthEnabled = st.Triangle_LengthEnabled;
            this._Sequence = st.Triangle_Sequence;
            this.HALT = st.TriangleHALT;
            this._Enabled = st.Triangle_Enabled;
            this.OUT = st.TriangleOUT;
        }
    }
}