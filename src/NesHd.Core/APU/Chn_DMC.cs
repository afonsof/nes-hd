using NesHd.Core.Misc;

namespace NesHd.Core.APU
{
    public class Chn_DMC
    {
        private readonly double[] DMC_FREQUENCY =
            {
                0xD60, 0xBE0, 0xAA0, 0xA00, 0x8F0, 0x7F0, 0x710, 0x6B0,
                0x5F0, 0x500, 0x470, 0x400, 0x350, 0x2A8, 0x240, 0x1B0
            };

        private readonly NesEngine _engine;
        private byte DAC;

        private ushort DMAAddress;

        private ushort DMALength;
        private ushort DMALengthCounter;
        private ushort DMAStartAddress;

        private byte DMCBIT;
        private byte DMCBYTE;
        public bool DMCIRQEnabled;
        private bool _Enabled;
        private double _FreqTimer;
        private double _Frequency;
        private bool _Loop;
        private double _RenderedLength;
        private double _SampleCount;

        public Chn_DMC(NesEngine NesEmu)
        {
            _engine = NesEmu;
        }

        public short RenderSample()
        {
            if (_Enabled)
            {
                _SampleCount++;
                if (_SampleCount > _RenderedLength)
                {
                    _SampleCount -= _RenderedLength;
                    if (DMCBIT == 7)
                    {
                        if (DMALength > 0)
                        {
                            DMCBIT = 0;
                            DMCBYTE = _engine.Memory.Read(DMAAddress);
                            DMAAddress++;
                            DMALength--;
                            if (DMALength <= 0 & _Loop)
                            {
                                DMAAddress = DMAStartAddress;
                                DMALength = DMALengthCounter;
                            }
                            if (DMALength <= 0 & !_Loop & DMCIRQEnabled)
                            {
                                _engine.Apu.DMCIRQPending = true;
                                _engine.Cpu.IRQNextTime = true;
                            }
                        }
                        else
                        {
                            _Enabled = false;
                        }
                    }
                    else
                    {
                        DMCBIT++;
                        DMCBYTE >>= 1;
                    }
                    if (_Enabled)
                    {
                        if ((DMCBYTE & 1) != 0)
                        {
                            if (DAC < 0x7E)
                            {
                                DAC++;
                            }
                        }
                        else if (DAC > 1)
                        {
                            DAC--;
                        }
                    }
                }
                if (DAC > 25)
                    DAC = 25;
                return (short) ((DAC - 14)*2);
            }
            return 0;
        }

        private void UpdateFrequency()
        {
            _Frequency = 1790000/(_FreqTimer + 1)*8;
            _RenderedLength = 44100/_Frequency;
        }

        public void SaveState(StateHolder st)
        {
            st.DMC_Frequency = _Frequency;
            st.DMC_RenderedLength = _RenderedLength;
            st.DMC_SampleCount = _SampleCount;
            st.DMCDMCIRQEnabled = DMCIRQEnabled;
            st.DMC_Enabled = _Enabled;
            st.DMC_Loop = _Loop;
            st.DMC_FreqTimer = _FreqTimer;
            st.DMCDAC = DAC;
            st.DMCDMAStartAddress = DMAStartAddress;
            st.DMCDMAAddress = DMAAddress;
            st.DMCDMALength = DMALength;
            st.DMCDMALengthCounter = DMALengthCounter;
            st.DMCDMCBIT = DMCBIT;
            st.DMCDMCBYTE = DMCBYTE;
        }

        public void LoadState(StateHolder st)
        {
            _Frequency = st.DMC_Frequency;
            _RenderedLength = st.DMC_RenderedLength;
            _SampleCount = st.DMC_SampleCount;
            DMCIRQEnabled = st.DMCDMCIRQEnabled;
            _Enabled = st.DMC_Enabled;
            _Loop = st.DMC_Loop;
            _FreqTimer = st.DMC_FreqTimer;
            DAC = st.DMCDAC;
            DMAStartAddress = st.DMCDMAStartAddress;
            DMAAddress = st.DMCDMAAddress;
            DMALength = st.DMCDMALength;
            DMALengthCounter = st.DMCDMALengthCounter;
            DMCBIT = st.DMCDMCBIT;
            DMCBYTE = st.DMCDMCBYTE;
        }

        #region Registers

        public void Write4010(byte data)
        {
            DMCIRQEnabled = (data & 0x80) != 0; //Bit 7
            _Loop = (data & 0x40) != 0; //Bit 6
            //IRQ
            if (!DMCIRQEnabled)
                _engine.Apu.DMCIRQPending = false;

            _FreqTimer = DMC_FREQUENCY[data & 0xF]; //Bit 0 - 3
            UpdateFrequency();
        }

        public void Write4011(byte data)
        {
            DAC = (byte) (data & 0x7f);
            UpdateFrequency();
        }

        public void Write4012(byte data)
        {
            DMAStartAddress = (ushort) ((data*0x40) + 0xC000);
            UpdateFrequency();
        }

        public void Write4013(byte data)
        {
            DMALengthCounter = (ushort) ((data*0x10) + 1);
            UpdateFrequency();
        }

        #endregion

        #region Properties

        public bool Enabled
        {
            get
            {
                _Enabled = DMALength > 0;
                return _Enabled;
            }
            set
            {
                _Enabled = value;
                if (value)
                {
                    if (DMALength <= 0)
                    {
                        DMALength = DMALengthCounter;
                        DMCBIT = 7;
                        DMAAddress = DMAStartAddress;
                    }
                }
                else
                {
                    DMALength = 0;
                }
            }
        }

        #endregion
    }
}