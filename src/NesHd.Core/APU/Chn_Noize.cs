using NesHd.Core.Misc;

namespace NesHd.Core.APU
{
    public class Chn_Noize
    {
        private readonly byte[] LENGTH_COUNTER_TABLE =
            {
                0x5, 0x7f, 0xA, 0x1, 0x14, 0x2, 0x28, 0x3, 0x50, 0x4, 0x1E, 0x5, 0x7, 0x6, 0x0E, 0x7,
                0x6, 0x08, 0xC, 0x9, 0x18, 0xa, 0x30, 0xb, 0x60, 0xc, 0x24, 0xd, 0x8, 0xe, 0x10, 0xf
            };

        private readonly double[] NOISE_FREQUENCY_TABLE =
            {
                0x002, 0x004, 0x008, 0x010, 0x020, 0x030, 0x040, 0x050,
                0x065, 0x07F, 0x0BE, 0x0FE, 0x17D, 0x1FC, 0x3F9, 0x7F2 /*, 0xFE4*/
            };

        private short OUT;
        private byte _DecayCount;
        private bool _DecayDiable;
        private bool _DecayLoopEnable;
        private bool _DecayReset;
        private byte _DecayTimer;

        private bool _Enabled;
        private byte _Envelope;
        private double _FreqTimer;
        private double _Frequency;
        private byte _LengthCount;
        private int _NoiseMode;
        private double _RenderedLength;
        private double _SampleCount;
        private ushort _ShiftReg = 1;
        private byte _Volume;
        //Length Counter
        public void UpdateLengthCounter()
        {
            if (!_DecayLoopEnable & _LengthCount > 0)
                _LengthCount--;
        }

        /// <summary>
        /// Update Envelope / Decay / Linear Counter
        /// </summary>
        public void UpdateEnvelope()
        {
            if (_DecayReset)
            {
                _DecayCount = _DecayTimer;
                _Envelope = 0x0F;
                _DecayReset = false;
                if (!_DecayDiable)
                    _Volume = 0x0F;
            }
            else
            {
                if (_DecayCount > 0)
                    _DecayCount--;
                else
                {
                    _DecayCount = _DecayTimer;
                    if (_Envelope > 0)
                        _Envelope--;
                    else if (_DecayLoopEnable)
                        _Envelope = 0x0F;

                    if (!_DecayDiable)
                        _Volume = _Envelope;
                }
            }
        }

        //Do NOZ samples
        public short RenderSample()
        {
            if (_LengthCount > 0)
            {
                _SampleCount++;
                if (_SampleCount >= _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    _ShiftReg <<= 1;
                    _ShiftReg |= (ushort) (((_ShiftReg >> 15) ^ (_ShiftReg >> _NoiseMode)) & 1);
                }
                OUT = ((_DecayDiable ? _Volume : _Envelope));
                if ((_ShiftReg & 1) == 0)
                    OUT *= -1;
                return OUT;
            }

            return 0;
        }

        private void UpdateFrequency()
        {
            _Frequency = 1790000/2/(_FreqTimer + 1);
            if (_FreqTimer > 4)
                _RenderedLength = 44100/_Frequency;
        }

        public void SaveState(StateHolder st)
        {
            st.NOIZE_Enabled = _Enabled;
            st.NOIZE_Volume = _Volume;
            st.NOIZE_Envelope = _Envelope;
            st.NOIZE_Frequency = _Frequency;
            st.NOIZE_SampleCount = _SampleCount;
            st.NOIZE_RenderedLength = _RenderedLength;
            st.NOIZE_FreqTimer = _FreqTimer;
            st.NOIZE_LengthCount = _LengthCount;
            st.NOIZE_ShiftReg = _ShiftReg;
            st.NOIZE_DecayCount = _DecayCount;
            st.NOIZE_DecayTimer = _DecayTimer;
            st.NOIZE_DecayDiable = _DecayDiable;
            st.NOIZE_NoiseMode = _NoiseMode;
            st.NOIZE_DecayLoopEnable = _DecayLoopEnable;
            st.NOIZE_DecayReset = _DecayReset;
            st.NOIZEOUT = OUT;
        }

        public void LoadState(StateHolder st)
        {
            _Enabled = st.NOIZE_Enabled;
            _Volume = st.NOIZE_Volume;
            _Envelope = st.NOIZE_Envelope;
            _Frequency = st.NOIZE_Frequency;
            _SampleCount = st.NOIZE_SampleCount;
            _RenderedLength = st.NOIZE_RenderedLength;
            _FreqTimer = st.NOIZE_FreqTimer;
            _LengthCount = st.NOIZE_LengthCount;
            _ShiftReg = st.NOIZE_ShiftReg;
            _DecayCount = st.NOIZE_DecayCount;
            _DecayTimer = st.NOIZE_DecayTimer;
            _DecayDiable = st.NOIZE_DecayDiable;
            _NoiseMode = st.NOIZE_NoiseMode;
            _DecayLoopEnable = st.NOIZE_DecayLoopEnable;
            _DecayReset = st.NOIZE_DecayReset;
            OUT = st.NOIZEOUT;
        }

        #region Registerts

        public void Write_400C(byte data)
        {
            _DecayTimer = (byte) (data & 0xF); //bit 0 - 3
            _DecayDiable = ((data & 0x10) != 0); //bit 4
            _DecayLoopEnable = ((data & 0x20) != 0); //bit 5
            if (_DecayDiable)
                _Volume = _DecayTimer;
            else
                _Volume = _Envelope;
            UpdateFrequency();
        }

        public void Write_400E(byte data)
        {
            _FreqTimer = NOISE_FREQUENCY_TABLE[data & 0x0F]; //bit 0 - 3
            _NoiseMode = ((data & 0x80) != 0) ? 9 : 14; //bit 7
            UpdateFrequency();
        }

        public void Write_400F(byte data)
        {
            _LengthCount = LENGTH_COUNTER_TABLE[data >> 3]; //bit 3 - 7
            _DecayReset = true;
            UpdateFrequency();
        }

        #endregion

        #region Properties

        public bool Enabled
        {
            get
            {
                _Enabled = (_LengthCount > 0);
                return _Enabled;
            }
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