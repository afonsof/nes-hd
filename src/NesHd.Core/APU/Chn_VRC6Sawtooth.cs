using NesHd.Core.Misc;

namespace NesHd.Core.APU
{
    public class Chn_VRC6Sawtooth
    {
        private byte Accum;
        private byte AccumRate;
        private byte AccumStep;
        private short OUT;
        private bool _Enabled;
        private int _FreqTimer;
        private double _Frequency;
        private double _RenderedLength;
        private double _SampleCount;

        public short RenderSample()
        {
            if (_Enabled)
            {
                _SampleCount++;
                if (_SampleCount >= _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    AccumStep++;
                    if ((AccumStep & 2) != 0)
                        Accum += AccumRate;
                    if (AccumStep >= 14)
                        AccumStep = Accum = 0;

                    OUT = (short) (Accum >> 3);
                }
                return (short) ((OUT - 5));
            }
            return 0;
        }

        public void WriteB000(byte data)
        {
            AccumRate = (byte) (data & 0x3F);
        }

        public void WriteB001(byte data)
        {
            _FreqTimer = (_FreqTimer & 0x0F00) | data;
            //Update freq
            _Frequency = 1790000/(_FreqTimer + 1);
            _RenderedLength = 44100/_Frequency;
        }

        public void WriteB002(byte data)
        {
            _FreqTimer = (_FreqTimer & 0x00FF) | ((data & 0x0F) << 8);
            _Enabled = (data & 0x80) != 0;
            //Update freq
            _Frequency = 1790000/(_FreqTimer + 1);
            _RenderedLength = 44100/_Frequency;
        }

        public void SaveState(StateHolder st)
        {
            st.VRC6SawtoothAccumRate = AccumRate;
            st.VRC6SawtoothAccumStep = AccumStep;
            st.VRC6SawtoothAccum = Accum;
            st.VRC6Sawtooth_FreqTimer = _FreqTimer;
            st.VRC6Sawtooth_Enabled = _Enabled;
            st.VRC6Sawtooth_Frequency = _Frequency;
            st.VRC6Sawtooth_SampleCount = _SampleCount;
            st.VRC6Sawtooth_RenderedLength = _RenderedLength;
            st.VRC6SawtoothOUT = OUT;
        }

        public void LoadState(StateHolder st)
        {
            AccumRate = st.VRC6SawtoothAccumRate;
            AccumStep = st.VRC6SawtoothAccumStep;
            Accum = st.VRC6SawtoothAccum;
            _FreqTimer = st.VRC6Sawtooth_FreqTimer;
            _Enabled = st.VRC6Sawtooth_Enabled;
            _Frequency = st.VRC6Sawtooth_Frequency;
            _SampleCount = st.VRC6Sawtooth_SampleCount;
            _RenderedLength = st.VRC6Sawtooth_RenderedLength;
            OUT = st.VRC6SawtoothOUT;
        }
    }
}