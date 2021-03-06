﻿using NesHd.Core.Misc;

namespace NesHd.Core.APU
{
    public class ChnRectangle1
    {
        private readonly byte[] LENGTH_COUNTER_TABLE =
            {
                0x5, 0x7f, 0xA, 0x1, 0x14, 0x2, 0x28, 0x3, 0x50, 0x4, 0x1E, 0x5, 0x7, 0x6, 0x0E, 0x7,
                0x6, 0x08, 0xC, 0x9, 0x18, 0xa, 0x30, 0xb, 0x60, 0xc, 0x24, 0xd, 0x8, 0xe, 0x10, 0xf
            };

        private double DutyPercentage;
        private bool WaveStatus;

        private byte _DecayCount;
        private bool _DecayDiable;
        private bool _DecayLoopEnable;
        private bool _DecayReset;
        private byte _DecayTimer;
        private int _DutyCycle;
        private bool _Enabled;
        private byte _Envelope;
        private int _FreqTimer;
        private double _Frequency;
        private byte _LengthCount;
        private double _RenderedLength;
        private double _SampleCount;
        private byte _SweepCount;
        private bool _SweepDirection;
        private bool _SweepEnable;
        private bool _SweepForceSilence;
        private byte _SweepRate;
        private bool _SweepReset;
        private byte _SweepShift;
        private byte _Volume;

        /// <summary>
        /// Check to see if the sweep unit is forcing the channel to be silent 
        /// and update the frequency if not.
        /// </summary>
        private void CheckSweepForceSilence()
        {
            _SweepForceSilence = false;
            if (_FreqTimer < 8)
                _SweepForceSilence = true;
            else if (!_SweepDirection)
            {
                if ((_FreqTimer & 0x0800) != 0)
                    _SweepForceSilence = true;
            }
            //Update Frequency
            if (!_SweepForceSilence)
            {
                _Frequency = 1790000/16/(_FreqTimer + 1);
                _RenderedLength = 44100/_Frequency;
            }
        }

        public void UpdateLengthCounter()
        {
            //Length counter
            if (!_DecayLoopEnable & _LengthCount > 0)
                _LengthCount--;
        }

        public void UpdateSweep()
        {
            if (_SweepEnable & !_SweepForceSilence)
            {
                if (_SweepCount > 0)
                    _SweepCount--;
                else
                {
                    _SweepCount = _SweepRate;
                    if (_SweepDirection)
                        _FreqTimer -= (_FreqTimer >> _SweepShift) + 1;
                    else
                        _FreqTimer += (_FreqTimer >> _SweepShift);
                    CheckSweepForceSilence();
                }
            }
            if (_SweepReset)
            {
                _SweepReset = false;
                _SweepCount = _SweepRate;
            }
        }

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

        public short RenderSample()
        {
            if (_LengthCount > 0 & !_SweepForceSilence)
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
                {
                    return (short) (-1*(_DecayDiable ? _Volume : _Envelope));
                }
                return (_DecayDiable ? _Volume : _Envelope);
            }
            return 0;
        }

        public void SaveState(StateHolder st)
        {
            st.Rectangle1_Enabled = _Enabled;
            st.Rectangle1_Volume = _Volume;
            st.Rectangle1_Envelope = _Envelope;
            st.Rectangle1_Frequency = _Frequency;
            st.Rectangle1_SampleCount = _SampleCount;
            st.Rectangle1_RenderedLength = _RenderedLength;
            st.Rectangle1_DutyCycle = _DutyCycle;
            st.Rectangle1_FreqTimer = _FreqTimer;
            st.Rectangle1_DecayCount = _DecayCount;
            st.Rectangle1_DecayTimer = _DecayTimer;
            st.Rectangle1_DecayDiable = _DecayDiable;
            st.Rectangle1_DecayReset = _DecayReset;
            st.Rectangle1_DecayLoopEnable = _DecayLoopEnable;
            st.Rectangle1_LengthCount = _LengthCount;
            st.Rectangle1_SweepShift = _SweepShift;
            st.Rectangle1_SweepDirection = _SweepDirection;
            st.Rectangle1_SweepRate = _SweepRate;
            st.Rectangle1_SweepEnable = _SweepEnable;
            st.Rectangle1_SweepCount = _SweepCount;
            st.Rectangle1_SweepReset = _SweepReset;
            st.Rectangle1_SweepForceSilence = _SweepForceSilence;
            st.Rectangle1DutyPercentage = DutyPercentage;
            st.Rectangle1WaveStatus = WaveStatus;
        }

        public void LoadState(StateHolder st)
        {
            _Enabled = st.Rectangle1_Enabled;
            _Volume = st.Rectangle1_Volume;
            _Envelope = st.Rectangle1_Envelope;
            _Frequency = st.Rectangle1_Frequency;
            _SampleCount = st.Rectangle1_SampleCount;
            _RenderedLength = st.Rectangle1_RenderedLength;
            _DutyCycle = st.Rectangle1_DutyCycle;
            _FreqTimer = st.Rectangle1_FreqTimer;
            _DecayCount = st.Rectangle1_DecayCount;
            _DecayTimer = st.Rectangle1_DecayTimer;
            _DecayDiable = st.Rectangle1_DecayDiable;
            _DecayReset = st.Rectangle1_DecayReset;
            _DecayLoopEnable = st.Rectangle1_DecayLoopEnable;
            _LengthCount = st.Rectangle1_LengthCount;
            _SweepShift = st.Rectangle1_SweepShift;
            _SweepDirection = st.Rectangle1_SweepDirection;
            _SweepRate = st.Rectangle1_SweepRate;
            _SweepEnable = st.Rectangle1_SweepEnable;
            _SweepCount = st.Rectangle1_SweepCount;
            _SweepReset = st.Rectangle1_SweepReset;
            _SweepForceSilence = st.Rectangle1_SweepForceSilence;
            DutyPercentage = st.Rectangle1DutyPercentage;
            WaveStatus = st.Rectangle1WaveStatus;
        }

        #region Registers

        public void Write_4000(byte data)
        {
            _DecayDiable = ((data & 0x10) != 0); //bit 4
            _DecayLoopEnable = ((data & 0x20) != 0); //bit 5
            _DutyCycle = (data & 0xC0) >> 6;
            if (_DutyCycle == 0)
                DutyPercentage = 0.125;
            else if (_DutyCycle == 1)
                DutyPercentage = 0.25;
            else if (_DutyCycle == 2)
                DutyPercentage = 0.5;
            else if (_DutyCycle == 3)
                DutyPercentage = 0.75;
            //Decay / Volume
            _DecayTimer = (byte) (data & 0x0F); //bit 0 - 3
            if (_DecayDiable)
                _Volume = _DecayTimer;
            else
                _Volume = _Envelope;
        }

        public void Write_4001(byte data)
        {
            _SweepShift = (byte) (data & 0x7); //bit 0 - 2
            _SweepDirection = ((data & 0x8) != 0); //bit 3
            _SweepRate = (byte) ((data & 0x70) >> 4); //bit 4 - 6
            _SweepEnable = ((data & 0x80) != 0 & (_SweepShift != 0)); //bit 7
            _SweepReset = true;
            CheckSweepForceSilence();
        }

        public void Write_4002(byte data)
        {
            _FreqTimer = ((_FreqTimer & 0x0700) | data);
            CheckSweepForceSilence();
        }

        public void Write_4003(byte data)
        {
            _FreqTimer = ((_FreqTimer & 0x00FF) | (data & 0x07) << 8); //Bit 0 - 2
            //if (_Enabled)
            _LengthCount = LENGTH_COUNTER_TABLE[(data & 0xF8) >> 3]; //bit 3 - 7 
            _DecayReset = true;
            CheckSweepForceSilence();
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