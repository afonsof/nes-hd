using System.Windows.Forms;
using NesHd.Core.Debugger;
using NesHd.Core.Output.Audio;
using SlimDX.DirectSound;
using SlimDX.Multimedia;

namespace NesHd.Core.APU
{
    public class Apu
    {
        private readonly NesEngine _engine;
        private int AD = 2;
        private int BufferSize = 88200;
        private byte[] DATA = new byte[88200];
        public bool DMCIRQPending;
        private int D_Pos; //Data position
        public bool FrameIRQEnabled;
        public bool FrameIRQPending;
        private bool IsPaused;
        public bool IsRendering;
        private int L_Pos; //Last position
        public WaveRecorder RECODER = new WaveRecorder();
        public bool STEREO;
        private int W_Pos; //Write position
        private ChnDmc _Chn_DMC;
        private ChnNoize _Chn_NOZ;
        //Channels
        private ChnRectangle1 _Chn_REC1;
        private ChnRectangle2 _Chn_REC2;
        private ChnTriangle _Chn_TRL;
        private ChnVrc6Pulse1 _Chn_VRC6Pulse1;
        private ChnVrc6Pulse2 _Chn_VRC6Pulse2;
        private ChnVrc6Sawtooth _Chn_VRC6Sawtooth;
        private bool _Enabled_DMC = true;
        private bool _Enabled_NOZ = true;
        private bool _Enabled_REC1 = true;
        private bool _Enabled_REC2 = true;
        private bool _Enabled_TRL = true;
        private bool _Enabled_VRC6Pulse1 = true;
        private bool _Enabled_VRC6Pulse2 = true;
        private bool _Enabled_VRC6SawTooth = true;
        private bool _FirstRender = true;
        //APU
        public int _FrameCounter;
        public bool _PAL;
        private DirectSound _SoundDevice;
        private SecondarySoundBuffer buffer;

        public Apu(NesEngine NesEmu, IAudioDevice SoundDevice)
        {
            _engine = NesEmu;
            STEREO = SoundDevice.Stereo;
            InitDirectSound(SoundDevice.SoundDevice);
        }

        private void InitDirectSound(Control parent)
        {
            Debug.WriteLine(this, "Initializing APU ....", DebugStatus.None);
            //Create the device
            _SoundDevice = new DirectSound();
            _SoundDevice.SetCooperativeLevel(parent.Parent.Handle, CooperativeLevel.Normal);
            //Create the wav format
            var wav = new WaveFormat();
            wav.FormatTag = WaveFormatTag.Pcm;
            wav.SamplesPerSecond = 44100;
            wav.Channels = (short) (STEREO ? 2 : 1);
            AD = (STEREO ? 4 : 2); //Stereo / Mono
            wav.BitsPerSample = 16;
            wav.AverageBytesPerSecond = wav.SamplesPerSecond*wav.Channels*(wav.BitsPerSample/8);
            wav.BlockAlignment = (short) (wav.Channels*wav.BitsPerSample/8);
            BufferSize = wav.AverageBytesPerSecond;
            //Description
            var des = new SoundBufferDescription
                          {
                              Format = wav,
                              SizeInBytes = BufferSize,
                              Flags =
                                  BufferFlags.ControlVolume | BufferFlags.ControlFrequency | BufferFlags.ControlPan |
                                  BufferFlags.ControlEffects
                          };
            //des.Flags = BufferFlags.GlobalFocus | BufferFlags.Software;
            //buffer
            DATA = new byte[BufferSize];
            buffer = new SecondarySoundBuffer(_SoundDevice, des);
            buffer.Play(0, PlayFlags.Looping);
            //channels
            InitChannels();
            Debug.WriteLine(this, "APU initialized ok !!", DebugStatus.Cool);
        }

        private void InitChannels()
        {
            _Chn_REC1 = new ChnRectangle1();
            _Chn_REC2 = new ChnRectangle2();
            _Chn_TRL = new ChnTriangle();
            _Chn_NOZ = new ChnNoize();
            _Chn_DMC = new ChnDmc(_engine);
            _Chn_VRC6Pulse1 = new ChnVrc6Pulse1();
            _Chn_VRC6Pulse2 = new ChnVrc6Pulse2();
            _Chn_VRC6Sawtooth = new ChnVrc6Sawtooth();
        }

        public void RenderFrame()
        {
            _FrameCounter++;
            if (buffer == null | buffer.Disposed)
            {
                IsRendering = false;
                return;
            }
            IsRendering = true;
            W_Pos = 0;
            var Seq = _PAL ? _FrameCounter%5 : _FrameCounter%4;

            #region Update channels depending on Seq tick.

            if (Seq == 0 | Seq == 2)
            {
                _Chn_REC1.UpdateEnvelope();
                _Chn_REC1.UpdateEnvelope();
                _Chn_REC1.UpdateSweep();

                _Chn_REC2.UpdateEnvelope();
                _Chn_REC2.UpdateEnvelope();
                _Chn_REC2.UpdateSweep();

                _Chn_NOZ.UpdateEnvelope();
                _Chn_NOZ.UpdateEnvelope();
                _Chn_NOZ.UpdateLengthCounter();

                _Chn_TRL.UpdateEnvelope();
                _Chn_TRL.UpdateEnvelope();
            }
            else if (Seq == 1 | Seq == 3)
            {
                _Chn_REC1.UpdateEnvelope();
                _Chn_REC1.UpdateEnvelope();

                _Chn_REC1.UpdateSweep();
                _Chn_REC1.UpdateLengthCounter();

                _Chn_REC2.UpdateEnvelope();
                _Chn_REC2.UpdateEnvelope();

                _Chn_REC2.UpdateSweep();
                _Chn_REC2.UpdateLengthCounter();

                _Chn_NOZ.UpdateEnvelope();
                _Chn_NOZ.UpdateEnvelope();

                _Chn_TRL.UpdateEnvelope();
                _Chn_TRL.UpdateEnvelope();
                _Chn_TRL.UpdateLengthCounter();
                if (Seq == 3)
                {
                    if (FrameIRQEnabled)
                    {
                        FrameIRQPending = true;
                        _engine.Cpu.IRQNextTime = true;
                    }
                }
            }

            #endregion

            #region Write the buffer

            W_Pos = buffer.CurrentWritePosition;
            if (_FirstRender)
            {
                _FirstRender = false;
                D_Pos = buffer.CurrentWritePosition + (STEREO ? 0x2000 : 0x1000);
                L_Pos = buffer.CurrentWritePosition;
            }
            var po = W_Pos - L_Pos;
            if (po < 0)
            {
                po = (BufferSize - L_Pos) + W_Pos;
            }
            if (po != 0)
            {
                for (var i = 0; i < po; i += AD)
                {
                    short OUT = 0;

                    #region Mix !!

                    if (_Enabled_REC1)
                        OUT += _Chn_REC1.RenderSample();
                    if (_Enabled_REC2)
                        OUT += _Chn_REC2.RenderSample();
                    if (_Enabled_NOZ)
                        OUT += _Chn_NOZ.RenderSample();
                    if (_Enabled_TRL)
                        OUT += _Chn_TRL.RenderSample();
                    if (_Enabled_DMC)
                        OUT += _Chn_DMC.RenderSample();
                    if (_Enabled_VRC6Pulse1)
                        OUT += _Chn_VRC6Pulse1.RenderSample();
                    if (_Enabled_VRC6Pulse2)
                        OUT += _Chn_VRC6Pulse2.RenderSample();
                    if (_Enabled_VRC6SawTooth)
                        OUT += _Chn_VRC6Sawtooth.RenderSample();
                    //Level up
                    OUT *= 2;
                    //Limit if needed to avoid overflow
                    if (OUT > 110)
                        OUT = 110;
                    else if (OUT < -110)
                        OUT = -110;
                    //RECORD
                    if (RECODER.IsRecording)
                        RECODER.AddSample(OUT);

                    #endregion

                    if (D_Pos < DATA.Length)
                    {
                        DATA[D_Pos] = (byte) ((OUT & 0xFF00) >> 8);
                        DATA[D_Pos + 1] = (byte) (OUT & 0xFF);
                        if (STEREO) //Add the same sample to the left channel
                        {
                            DATA[D_Pos + 2] = (byte) ((OUT & 0xFF00) >> 8);
                            DATA[D_Pos + 3] = (byte) (OUT & 0xFF);
                        }
                    }
                    D_Pos += AD;
                    D_Pos = D_Pos%BufferSize;
                }
                buffer.Write(DATA, 0, LockFlags.None);
                L_Pos = W_Pos;
            }
            IsRendering = false;

            #endregion
        }

        public void Play()
        {
            if (!IsPaused)
            {
                return;
            }
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                IsPaused = false;
                try //Sometimes this line thorws an exception for unkown reason !!
                {
                    buffer.Play(0, PlayFlags.Looping);
                }
                catch
                {
                }
            }
        }

        public void Pause()
        {
            if (IsPaused)
            {
                return;
            }
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                buffer.Stop();
                IsPaused = true;
            }
        }

        public void Shutdown()
        {
            IsPaused = true;
            if (buffer != null && buffer.Disposed & !IsRendering)
            {
                try
                {
                    buffer.Stop();
                }
                catch
                {
                    return;
                }
            }
            while (IsRendering)
            {
            }
            buffer.Dispose();
            _SoundDevice.Dispose();
            if (RECODER.IsRecording)
                RECODER.Stop();
        }

        /// <summary>
        /// Set the volume 
        /// </summary>
        /// <param name="Vol">The volume level (-3000 = min, 0 = max)</param>
        public void SetVolume(int Vol)
        {
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                buffer.Volume = Vol;
            }
        }

        /// <summary>
        /// Set the pan
        /// </summary>
        /// <param name="Pan">The pan</param>
        public void SetPan(int Pan)
        {
            if (buffer != null && !buffer.Disposed & !IsRendering)
            {
                buffer.Pan = Pan;
            }
        }

        #region Registers

        //Rec 1
        public void Write_4000(byte data)
        {
            _Chn_REC1.Write_4000(data);
        }

        public void Write_4001(byte data)
        {
            _Chn_REC1.Write_4001(data);
        }

        public void Write_4002(byte data)
        {
            _Chn_REC1.Write_4002(data);
        }

        public void Write_4003(byte data)
        {
            _Chn_REC1.Write_4003(data);
        }

        //Rec 2
        public void Write_4004(byte data)
        {
            _Chn_REC2.Write_4004(data);
        }

        public void Write_4005(byte data)
        {
            _Chn_REC2.Write_4005(data);
        }

        public void Write_4006(byte data)
        {
            _Chn_REC2.Write_4006(data);
        }

        public void Write_4007(byte data)
        {
            _Chn_REC2.Write_4007(data);
        }

        //Trl 
        public void Write_4008(byte data)
        {
            _Chn_TRL.Write_4008(data);
        }

        public void Write_400A(byte data)
        {
            _Chn_TRL.Write_400A(data);
        }

        public void Write_400B(byte data)
        {
            _Chn_TRL.Write_400B(data);
        }

        //Noz 
        public void Write_400C(byte data)
        {
            _Chn_NOZ.Write_400C(data);
        }

        public void Write_400E(byte data)
        {
            _Chn_NOZ.Write_400E(data);
        }

        public void Write_400F(byte data)
        {
            _Chn_NOZ.Write_400F(data);
        }

        //DMC
        public void Write_4010(byte data)
        {
            _Chn_DMC.Write4010(data);
        }

        public void Write_4011(byte data)
        {
            _Chn_DMC.Write4011(data);
        }

        public void Write_4012(byte data)
        {
            _Chn_DMC.Write4012(data);
        }

        public void Write_4013(byte data)
        {
            _Chn_DMC.Write4013(data);
        }

        //Status
        public void Write_4015(byte data)
        {
            _Chn_REC1.Enabled = (data & 0x01) != 0;
            _Chn_REC2.Enabled = (data & 0x02) != 0;
            _Chn_TRL.Enabled = (data & 0x04) != 0;
            _Chn_NOZ.Enabled = (data & 0x08) != 0;
            _Chn_DMC.Enabled = (data & 0x10) != 0;
            DMCIRQPending = false;
        }

        public byte Read_4015()
        {
            byte rt = 0;
            if (_Chn_REC1.Enabled)
                rt |= 0x01;
            if (_Chn_REC2.Enabled)
                rt |= 0x02;
            if (_Chn_TRL.Enabled)
                rt |= 0x04;
            if (_Chn_NOZ.Enabled)
                rt |= 0x08;
            if (_Chn_DMC.Enabled)
                rt |= 0x10;
            if (FrameIRQPending)
                rt |= 0x40;
            if (DMCIRQPending)
                rt |= 0x80;
            FrameIRQPending = false;
            //Find IRQ
            if (FrameIRQPending)
                _engine.Cpu.IRQNextTime = true;
            else if (DMCIRQPending)
                _engine.Cpu.IRQNextTime = true;
            return rt;
        }

        public void Write_4017(byte data)
        {
            _PAL = (data & 0x80) != 0;
            _FrameCounter = 0;
            FrameIRQEnabled = (data & 0x40) == 0;
            if (FrameIRQEnabled)
                FrameIRQPending = false;
            //Find IRQ
            if (FrameIRQPending)
                _engine.Cpu.IRQNextTime = true;
            else if (DMCIRQPending)
                _engine.Cpu.IRQNextTime = true;
        }

        #endregion

        #region Properties

        public bool Square1Enabled
        {
            get { return _Enabled_REC1; }
            set { _Enabled_REC1 = value; }
        }

        public bool Square2Enabled
        {
            get { return _Enabled_REC2; }
            set { _Enabled_REC2 = value; }
        }

        public bool TriangleEnabled
        {
            get { return _Enabled_TRL; }
            set { _Enabled_TRL = value; }
        }

        public bool NoiseEnabled
        {
            get { return _Enabled_NOZ; }
            set { _Enabled_NOZ = value; }
        }

        public bool VRC6P1Enabled
        {
            get { return _Enabled_VRC6Pulse1; }
            set { _Enabled_VRC6Pulse1 = value; }
        }

        public bool VRC6P2Enabled
        {
            get { return _Enabled_VRC6Pulse2; }
            set { _Enabled_VRC6Pulse2 = value; }
        }

        public bool DMCEnabled
        {
            get { return _Enabled_DMC; }
            set { _Enabled_DMC = value; }
        }

        public bool VRC6SawToothEnabled
        {
            get { return _Enabled_VRC6SawTooth; }
            set { _Enabled_VRC6SawTooth = value; }
        }

        public int FrameCounter
        {
            get { return _FrameCounter; }
            set { _FrameCounter = value; }
        }

        public bool PAL
        {
            get { return _PAL; }
            set { _PAL = value; }
        }

        public ChnDmc DMC
        {
            get { return _Chn_DMC; }
        }

        public ChnNoize NOIZE
        {
            get { return _Chn_NOZ; }
        }

        public ChnRectangle1 RECT1
        {
            get { return _Chn_REC1; }
        }

        public ChnRectangle2 RECT2
        {
            get { return _Chn_REC2; }
        }

        public ChnTriangle TRIANGLE
        {
            get { return _Chn_TRL; }
        }

        public ChnVrc6Pulse1 VRC6PULSE1
        {
            get { return _Chn_VRC6Pulse1; }
        }

        public ChnVrc6Pulse2 VRC6PULSE2
        {
            get { return _Chn_VRC6Pulse2; }
        }

        public ChnVrc6Sawtooth VRC6SAWTOOTH
        {
            get { return _Chn_VRC6Sawtooth; }
        }

        #endregion
    }
}