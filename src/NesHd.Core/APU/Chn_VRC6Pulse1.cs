using NesHd.Core.Misc;

namespace NesHd.Core.APU
{
    public class Chn_VRC6Pulse1
    {
        private double DutyPercentage;
        private short OUT;
        private bool WaveStatus;
        private int _DutyCycle;
        private bool _Enabled;
        private int _FreqTimer;
        private double _Frequency;
        private double _RenderedLength;
        private double _SampleCount;
        private byte _Volume;

        public short RenderSample()
        {
            if (_Enabled)
            {
                _SampleCount++;
                if (WaveStatus && (_SampleCount > (_RenderedLength*DutyPercentage)))
                {
                    _SampleCount -= _RenderedLength*DutyPercentage;
                    WaveStatus = !WaveStatus;
                }
                else if (!WaveStatus && (_SampleCount > (_RenderedLength*(1.0 - DutyPercentage))))
                {
                    _SampleCount -= _RenderedLength*(1.0 - DutyPercentage);
                    WaveStatus = !WaveStatus;
                }
                if (WaveStatus)
                    OUT = (short) (-_Volume);
                else
                    OUT = (_Volume);

                return OUT;
            }
            return 0;
        }

        public void Write9000(byte data)
        {
            _Volume = (byte) (data & 0x0F); //Bit 0 - 3
            _DutyCycle = (data >> 4); //Bit 4 - 7
            if (_DutyCycle == 0)
                DutyPercentage = 0.6250;
            else if (_DutyCycle == 1)
                DutyPercentage = 0.1250;
            else if (_DutyCycle == 2)
                DutyPercentage = 0.1875;
            else if (_DutyCycle == 3)
                DutyPercentage = 0.2500;
            else if (_DutyCycle == 4)
                DutyPercentage = 0.3125;
            else if (_DutyCycle == 5)
                DutyPercentage = 0.3750;
            else if (_DutyCycle == 6)
                DutyPercentage = 0.4375;
            else if (_DutyCycle == 7)
                DutyPercentage = 0.5000;
            else
                DutyPercentage = 1.0;
        }

        public void Write9001(byte data)
        {
            _FreqTimer = (_FreqTimer & 0x0F00) | data;
            //Update freq
            _Frequency = 1790000/16/(_FreqTimer + 1);
            _RenderedLength = 44100/_Frequency;
        }

        public void Write9002(byte data)
        {
            _FreqTimer = (_FreqTimer & 0x00FF) | ((data & 0x0F) << 8);
            _Enabled = (data & 0x80) != 0;
            //Update freq
            _Frequency = 1790000/16/(_FreqTimer + 1);
            _RenderedLength = 44100/_Frequency;
        }

        public void SaveState(StateHolder st)
        {
            st.VRC6Pulse1_Volume = _Volume;
            st.VRC6Pulse1DutyPercentage = DutyPercentage;
            st.VRC6Pulse1_DutyCycle = _DutyCycle;
            st.VRC6Pulse1_FreqTimer = _FreqTimer;
            st.VRC6Pulse1_Enabled = _Enabled;
            st.VRC6Pulse1_Frequency = _Frequency;
            st.VRC6Pulse1_SampleCount = _SampleCount;
            st.VRC6Pulse1_RenderedLength = _RenderedLength;
            st.VRC6Pulse1WaveStatus = WaveStatus;
            st.VRC6Pulse1OUT = OUT;
        }

        public void LoadState(StateHolder st)
        {
            _Volume = st.VRC6Pulse1_Volume;
            DutyPercentage = st.VRC6Pulse1DutyPercentage;
            _DutyCycle = st.VRC6Pulse1_DutyCycle;
            _FreqTimer = st.VRC6Pulse1_FreqTimer;
            _Enabled = st.VRC6Pulse1_Enabled;
            _Frequency = st.VRC6Pulse1_Frequency;
            _SampleCount = st.VRC6Pulse1_SampleCount;
            _RenderedLength = st.VRC6Pulse1_RenderedLength;
            WaveStatus = st.VRC6Pulse1WaveStatus;
            OUT = st.VRC6Pulse1OUT;
        }
    }
}