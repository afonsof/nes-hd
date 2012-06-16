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

        private readonly byte[] LENGTH_COUNTER_TABLE =
            {
                0x5, 0x7f, 0xA, 0x1, 0x14, 0x2, 0x28, 0x3, 0x50, 0x4, 0x1E, 0x5, 0x7, 0x6, 0x0E, 0x7,
                0x6, 0x08, 0xC, 0x9, 0x18, 0xa, 0x30, 0xb, 0x60, 0xc, 0x24, 0xd, 0x8, 0xe, 0x10, 0xf
            };

        private bool HALT;
        private short OUT;
        private bool _Enabled;

        private int _FreqTimer;
        private double _Frequency;
        private byte _LengthCount;
        private bool _LengthEnabled;
        private bool _LinearControl;
        private int _LinearCounter;
        private int _LinearCounterLoad;
        private double _RenderedLength;
        private double _SampleCount;
        private int _Sequence;
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (_LengthEnabled & _LengthCount > 0)
                _LengthCount--;
        }

        //Linear Counter
        public void UpdateEnvelope()
        {
            if (HALT)
                _LinearCounter = _LinearCounterLoad;
            else if (_LinearCounter > 0)
                _LinearCounter--;
            if (!_LinearControl)
                HALT = false;
        }

        public short RenderSample()
        {
            if (_LinearCounter > 0 & _LengthCount > 0 & _FreqTimer >= 8)
            {
                _SampleCount++;
                if (_SampleCount >= _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    _Sequence = (_Sequence + 1) & 0x1F;
                    if ((_Sequence & 0x10) != 0)
                        OUT = (short) (_Sequence ^ 0x1F);
                    else
                        OUT = (short) _Sequence;
                    OUT -= 7;
                    OUT *= 2;
                }
                return OUT;
            }
            if (OUT > 0)
                OUT--;
            else if (OUT < 0)
                OUT++;
            return OUT;
        }

        public void SaveState(StateHolder st)
        {
            st.Triangle_Frequency = _Frequency;
            st.Triangle_SampleCount = _SampleCount;
            st.Triangle_RenderedLength = _RenderedLength;
            st.Triangle_FreqTimer = _FreqTimer;
            st.Triangle_LengthCount = _LengthCount;
            st.Triangle_LinearCounter = _LinearCounter;
            st.Triangle_LinearCounterLoad = _LinearCounterLoad;
            st.Triangle_LinearControl = _LinearControl;
            st.Triangle_LengthEnabled = _LengthEnabled;
            st.Triangle_Sequence = _Sequence;
            st.TriangleHALT = HALT;
            st.Triangle_Enabled = _Enabled;
            st.TriangleOUT = OUT;
        }

        public void LoadState(StateHolder st)
        {
            _Frequency = st.Triangle_Frequency;
            _SampleCount = st.Triangle_SampleCount;
            _RenderedLength = st.Triangle_RenderedLength;
            _FreqTimer = st.Triangle_FreqTimer;
            _LengthCount = st.Triangle_LengthCount;
            _LinearCounter = st.Triangle_LinearCounter;
            _LinearCounterLoad = st.Triangle_LinearCounterLoad;
            _LinearControl = st.Triangle_LinearControl;
            _LengthEnabled = st.Triangle_LengthEnabled;
            _Sequence = st.Triangle_Sequence;
            HALT = st.TriangleHALT;
            _Enabled = st.Triangle_Enabled;
            OUT = st.TriangleOUT;
        }

        #region Registers

        public void Write_4008(byte data)
        {
            _LinearCounterLoad = data & 0x7F; //Bit 0 - 6
            _LinearControl = (data & 0x80) != 0; //Bit 7
            _LengthEnabled = !_LinearControl;
        }

        public void Write_400A(byte data)
        {
            _FreqTimer = ((_FreqTimer & 0x700) | data);
            //Update Frequency
            _Frequency = 1790000/(_FreqTimer + 1);
            _RenderedLength = 44100/_Frequency;
        }

        public void Write_400B(byte data)
        {
            _FreqTimer = ((_FreqTimer & 0xFF) | (data & 0x7) << 8); //Bit 0 - 2
            //Update Frequency
            _Frequency = 1790000/(_FreqTimer + 1);
            _RenderedLength = 44100/_Frequency;
            if (_Enabled)
                _LengthCount = LENGTH_COUNTER_TABLE[data >> 3]; //bit 3 - 7 
            HALT = true;
        }

        #endregion

        #region Properties

        public bool Enabled
        {
            get { return (_LengthCount > 0); }
            set
            {
                _Enabled = value;
                if (!value)
                    _LengthCount = 0;
            }
        }

        #endregion
    }
}