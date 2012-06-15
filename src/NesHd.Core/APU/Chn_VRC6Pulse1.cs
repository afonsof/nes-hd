using NesHd.Core.Misc;

namespace NesHd.Core.APU
{
    public class Chn_VRC6Pulse1
    {
        byte _Volume = 0;
        double DutyPercentage = 0;
        int _DutyCycle = 0;
        int _FreqTimer = 0;
        bool _Enabled = false;
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        bool WaveStatus = false;
        short OUT = 0;

        public short RenderSample()
        {
            if (this._Enabled)
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
                    this.OUT = (short)(-this._Volume);
                else
                    this.OUT = (short)(this._Volume);

                return this.OUT;
            }
            return 0;
        }
        public void Write9000(byte data)
        {
            this._Volume = (byte)(data & 0x0F);//Bit 0 - 3
            this._DutyCycle = (data >> 4); //Bit 4 - 7
            if (this._DutyCycle == 0)
                this.DutyPercentage = 0.6250;
            else if (this._DutyCycle == 1)
                this.DutyPercentage = 0.1250;
            else if (this._DutyCycle == 2)
                this.DutyPercentage = 0.1875;
            else if (this._DutyCycle == 3)
                this.DutyPercentage = 0.2500;
            else if (this._DutyCycle == 4)
                this.DutyPercentage = 0.3125;
            else if (this._DutyCycle == 5)
                this.DutyPercentage = 0.3750;
            else if (this._DutyCycle == 6)
                this.DutyPercentage = 0.4375;
            else if (this._DutyCycle == 7)
                this.DutyPercentage = 0.5000;
            else
                this.DutyPercentage = 1.0;
        }
        public void Write9001(byte data)
        {
            this._FreqTimer = (this._FreqTimer & 0x0F00) | data;
            //Update freq
            this._Frequency = 1790000 / 16 / (this._FreqTimer + 1);
            this._RenderedLength = 44100 / this._Frequency;
        }
        public void Write9002(byte data)
        {
            this._FreqTimer = (this._FreqTimer & 0x00FF) | ((data & 0x0F) << 8);
            this._Enabled = (data & 0x80) != 0;
            //Update freq
            this._Frequency = 1790000 / 16 / (this._FreqTimer + 1);
            this._RenderedLength = 44100 / this._Frequency;
        }

        public void SaveState(StateHolder st)
        {
            st.VRC6Pulse1_Volume = this._Volume;
            st.VRC6Pulse1DutyPercentage = this.DutyPercentage;
            st.VRC6Pulse1_DutyCycle = this._DutyCycle;
            st.VRC6Pulse1_FreqTimer = this._FreqTimer;
            st.VRC6Pulse1_Enabled = this._Enabled;
            st.VRC6Pulse1_Frequency = this._Frequency;
            st.VRC6Pulse1_SampleCount = this._SampleCount;
            st.VRC6Pulse1_RenderedLength = this._RenderedLength;
            st.VRC6Pulse1WaveStatus = this.WaveStatus;
            st.VRC6Pulse1OUT = this.OUT;
        }
        public void LoadState(StateHolder st)
        {
            this._Volume = st.VRC6Pulse1_Volume;
            this.DutyPercentage = st.VRC6Pulse1DutyPercentage;
            this._DutyCycle = st.VRC6Pulse1_DutyCycle;
            this._FreqTimer = st.VRC6Pulse1_FreqTimer;
            this._Enabled = st.VRC6Pulse1_Enabled;
            this._Frequency = st.VRC6Pulse1_Frequency;
            this._SampleCount = st.VRC6Pulse1_SampleCount;
            this._RenderedLength = st.VRC6Pulse1_RenderedLength;
            this.WaveStatus = st.VRC6Pulse1WaveStatus;
            this.OUT = st.VRC6Pulse1OUT;
        }
    }
}
