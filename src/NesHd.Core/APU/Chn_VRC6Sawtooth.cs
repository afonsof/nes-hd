using NesHd.Core.Misc;

namespace NesHd.Core.APU
{
    public class Chn_VRC6Sawtooth
    {
        byte AccumRate = 0;
        byte AccumStep = 0;
        byte Accum = 0;
        int _FreqTimer = 0;
        bool _Enabled = false;
        double _Frequency = 0;
        double _SampleCount = 0;
        double _RenderedLength = 0;
        short OUT = 0;
        public short RenderSample()
        {
            if (this._Enabled)
            {
                this._SampleCount++;
                if (this._SampleCount >= this._RenderedLength)
                {
                    this._SampleCount -= this._RenderedLength;
                    this.AccumStep++;
                    if ((this.AccumStep & 2) != 0)
                        this.Accum += this.AccumRate;
                    if (this.AccumStep >= 14)
                        this.AccumStep = this.Accum = 0;

                    this.OUT = (short)(this.Accum >> 3);

                }
                return (short)((this.OUT - 5));
            }
            return 0;
        }
        public void WriteB000(byte data)
        {
            this.AccumRate = (byte)(data & 0x3F);
        }
        public void WriteB001(byte data)
        {
            this._FreqTimer = (this._FreqTimer & 0x0F00) | data;
            //Update freq
            this._Frequency = 1790000 / (this._FreqTimer + 1);
            this._RenderedLength = 44100 / this._Frequency;
        }
        public void WriteB002(byte data)
        {
            this._FreqTimer = (this._FreqTimer & 0x00FF) | ((data & 0x0F) << 8);
            this._Enabled = (data & 0x80) != 0;
            //Update freq
            this._Frequency = 1790000 / (this._FreqTimer + 1);
            this._RenderedLength = 44100 / this._Frequency;
        }

        public void SaveState(StateHolder st)
        {
            st.VRC6SawtoothAccumRate = this.AccumRate;
            st.VRC6SawtoothAccumStep = this.AccumStep;
            st.VRC6SawtoothAccum = this.Accum;
            st.VRC6Sawtooth_FreqTimer = this._FreqTimer;
            st.VRC6Sawtooth_Enabled = this._Enabled;
            st.VRC6Sawtooth_Frequency = this._Frequency;
            st.VRC6Sawtooth_SampleCount = this._SampleCount;
            st.VRC6Sawtooth_RenderedLength = this._RenderedLength;
            st.VRC6SawtoothOUT = this.OUT;
        }
        public void LoadState(StateHolder st)
        {
            this.AccumRate = st.VRC6SawtoothAccumRate;
            this.AccumStep = st.VRC6SawtoothAccumStep;
            this.Accum = st.VRC6SawtoothAccum;
            this._FreqTimer = st.VRC6Sawtooth_FreqTimer;
            this._Enabled = st.VRC6Sawtooth_Enabled;
            this._Frequency = st.VRC6Sawtooth_Frequency;
            this._SampleCount = st.VRC6Sawtooth_SampleCount;
            this._RenderedLength = st.VRC6Sawtooth_RenderedLength;
            this.OUT = st.VRC6SawtoothOUT;
        }
    }
}
