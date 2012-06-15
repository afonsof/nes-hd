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

using System.Windows.Forms;

using NesHd.Core.Debugger;
using NesHd.Core.Output.Audio;

using SlimDX.DirectSound;
using SlimDX.Multimedia;

namespace NesHd.Core.APU
{
    public class APU
    {
        NesEngine _Nes;
        Control _Control;
        //slimdx
        DirectSound _SoundDevice;
        SecondarySoundBuffer buffer;
        //Channels
        Chn_Rectangle1 _Chn_REC1;
        Chn_Rectangle2 _Chn_REC2;
        Chn_Triangle _Chn_TRL;
        Chn_Noize _Chn_NOZ;
        Chn_DMC _Chn_DMC;
        Chn_VRC6Pulse1 _Chn_VRC6Pulse1;
        Chn_VRC6Pulse2 _Chn_VRC6Pulse2;
        Chn_VRC6Sawtooth _Chn_VRC6Sawtooth;
        bool _Enabled_REC1 = true;
        bool _Enabled_REC2 = true;
        bool _Enabled_TRL = true;
        bool _Enabled_NOZ = true;
        bool _Enabled_DMC = true;
        bool _Enabled_VRC6Pulse1 = true;
        bool _Enabled_VRC6Pulse2 = true;
        bool _Enabled_VRC6SawTooth = true;
        //APU
        public int _FrameCounter = 0;
        public bool _PAL;
        bool _FirstRender = true;
        bool IsPaused;
        public bool IsRendering = false;

        public bool DMCIRQPending = false;
        public bool FrameIRQEnabled = false;
        public bool FrameIRQPending = false;

        //Buffer
        byte[] DATA = new byte[88200];
        int BufferSize = 88200;
        int W_Pos = 0;//Write position
        int L_Pos = 0;//Last position
        int D_Pos = 0;//Data position
        public bool STEREO = false;
        int AD = 2;
        //Recorder 
        public WaveRecorder RECODER = new WaveRecorder();

        public APU(NesEngine NesEmu, IAudioDevice SoundDevice)
        {
            this._Nes = NesEmu;
            this._Control = SoundDevice.SoundDevice;
            this.STEREO = SoundDevice.Stereo;
            this.InitDirectSound(SoundDevice.SoundDevice);
        }
        void InitDirectSound(Control parent)
        {
            Debug.WriteLine(this, "Initializing APU ....", DebugStatus.None);
            //Create the device
            this._SoundDevice = new DirectSound();
            this._SoundDevice.SetCooperativeLevel(parent.Parent.Handle, CooperativeLevel.Normal);
            //Create the wav format
            WaveFormat wav = new WaveFormat();
            wav.FormatTag = WaveFormatTag.Pcm;
            wav.SamplesPerSecond = 44100;
            wav.Channels = (short)(this.STEREO ? 2 : 1);
            this.AD = (this.STEREO ? 4 : 2);//Stereo / Mono
            wav.BitsPerSample = 16;
            wav.AverageBytesPerSecond = wav.SamplesPerSecond * wav.Channels * (wav.BitsPerSample / 8);
            wav.BlockAlignment = (short)(wav.Channels * wav.BitsPerSample / 8);
            this.BufferSize = wav.AverageBytesPerSecond;
            //Description
            SoundBufferDescription des = new SoundBufferDescription();
            des.Format = wav;
            des.SizeInBytes = this.BufferSize;
            //des.Flags = BufferFlags.GlobalFocus | BufferFlags.Software;
            des.Flags = BufferFlags.ControlVolume | BufferFlags.ControlFrequency | BufferFlags.ControlPan | BufferFlags.ControlEffects;
            //buffer
            this.DATA = new byte[this.BufferSize];
            this.buffer = new SecondarySoundBuffer(this._SoundDevice, des);
            this.buffer.Play(0, PlayFlags.Looping);
            //channels
            this.InitChannels();
            Debug.WriteLine(this, "APU initialized ok !!", DebugStatus.Cool);
        }
        void InitChannels()
        {
            this._Chn_REC1 = new Chn_Rectangle1();
            this._Chn_REC2 = new Chn_Rectangle2();
            this._Chn_TRL = new Chn_Triangle();
            this._Chn_NOZ = new Chn_Noize();
            this._Chn_DMC = new Chn_DMC(this._Nes);
            this._Chn_VRC6Pulse1 = new Chn_VRC6Pulse1();
            this._Chn_VRC6Pulse2 = new Chn_VRC6Pulse2();
            this._Chn_VRC6Sawtooth = new Chn_VRC6Sawtooth();
        }
        public void RenderFrame()
        {
            this._FrameCounter++;
            if (this.buffer == null | this.buffer.Disposed)
            { this.IsRendering = false; return; }
            this.IsRendering = true;
            this.W_Pos = 0;
            int Seq = this._PAL ? this._FrameCounter % 5 : this._FrameCounter % 4;
            #region Update channels depending on Seq tick.
            if (Seq == 0 | Seq == 2)
            {
                this._Chn_REC1.UpdateEnvelope(); 
                this._Chn_REC1.UpdateEnvelope();
                this._Chn_REC1.UpdateSweep(); 

                this._Chn_REC2.UpdateEnvelope(); 
                this._Chn_REC2.UpdateEnvelope();
                this._Chn_REC2.UpdateSweep(); 

                this._Chn_NOZ.UpdateEnvelope(); 
                this._Chn_NOZ.UpdateEnvelope();
                this._Chn_NOZ.UpdateLengthCounter();

                this._Chn_TRL.UpdateEnvelope(); 
                this._Chn_TRL.UpdateEnvelope();
            }
            else if (Seq == 1 | Seq == 3)
            {
                this._Chn_REC1.UpdateEnvelope(); 
                this._Chn_REC1.UpdateEnvelope();
          
                this._Chn_REC1.UpdateSweep();
                this._Chn_REC1.UpdateLengthCounter();

                this._Chn_REC2.UpdateEnvelope(); 
                this._Chn_REC2.UpdateEnvelope();
    
                this._Chn_REC2.UpdateSweep();
                this._Chn_REC2.UpdateLengthCounter();

                this._Chn_NOZ.UpdateEnvelope(); 
                this._Chn_NOZ.UpdateEnvelope();
              
                this._Chn_TRL.UpdateEnvelope(); 
                this._Chn_TRL.UpdateEnvelope();
                this._Chn_TRL.UpdateLengthCounter();
                if (Seq == 3)
                {
                    if (this.FrameIRQEnabled)
                    {
                        this.FrameIRQPending = true;
                        this._Nes.CPU.IRQNextTime = true;
                    }
                }
            }
            #endregion
            #region Write the buffer
            this.W_Pos = this.buffer.CurrentWritePosition;
            if (this._FirstRender)
            {
                this._FirstRender = false;
                this.D_Pos = this.buffer.CurrentWritePosition + (this.STEREO ? 0x2000 : 0x1000);
                this.L_Pos = this.buffer.CurrentWritePosition;
            }
            int po = this.W_Pos - this.L_Pos;
            if (po < 0)
            {
                po = (this.BufferSize - this.L_Pos) + this.W_Pos;
            }
            if (po != 0)
            {
                for (int i = 0; i < po; i += this.AD)
                {
                    short OUT = 0;
                    #region Mix !!
                    if (this._Enabled_REC1)
                        OUT += this._Chn_REC1.RenderSample();
                    if (this._Enabled_REC2)
                        OUT += this._Chn_REC2.RenderSample();
                    if (this._Enabled_NOZ)
                        OUT += this._Chn_NOZ.RenderSample();
                    if (this._Enabled_TRL)
                        OUT += this._Chn_TRL.RenderSample();
                    if (this._Enabled_DMC)
                        OUT += this._Chn_DMC.RenderSample();
                    if (this._Enabled_VRC6Pulse1)
                        OUT += this._Chn_VRC6Pulse1.RenderSample();
                    if (this._Enabled_VRC6Pulse2)
                        OUT += this._Chn_VRC6Pulse2.RenderSample();
                    if (this._Enabled_VRC6SawTooth)
                        OUT += this._Chn_VRC6Sawtooth.RenderSample();
                    //Level up
                    OUT *= 2;
                    //Limit if needed to avoid overflow
                    if (OUT > 110)
                        OUT = 110;
                    else if (OUT < -110)
                        OUT = -110;
                    //RECORD
                    if (this.RECODER.IsRecording)
                        this.RECODER.AddSample(OUT);
                    #endregion
                    if (this.D_Pos < this.DATA.Length)
                    {
                        this.DATA[this.D_Pos] = (byte)((OUT & 0xFF00) >> 8);
                        this.DATA[this.D_Pos + 1] = (byte)(OUT & 0xFF);
                        if (this.STEREO)//Add the same sample to the left channel
                        {
                            this.DATA[this.D_Pos + 2] = (byte)((OUT & 0xFF00) >> 8);
                            this.DATA[this.D_Pos + 3] = (byte)(OUT & 0xFF);
                        }
                    }
                    this.D_Pos += this.AD;
                    this.D_Pos = this.D_Pos % this.BufferSize;
                }
                this.buffer.Write(this.DATA, 0, LockFlags.None);
                this.L_Pos = this.W_Pos;
            }
            this.IsRendering = false;
            #endregion
        }
        public void Play()
        {
            if (!this.IsPaused)
            { return; }
            if (this.buffer != null && !this.buffer.Disposed & !this.IsRendering)
            {
                this.IsPaused = false;
                try//Sometimes this line thorws an exception for unkown reason !!
                {
                    this.buffer.Play(0, PlayFlags.Looping);
                }
                catch { }
            }
        }
        public void Pause()
        {
            if (this.IsPaused)
            { return; }
            if (this.buffer != null && !this.buffer.Disposed & !this.IsRendering)
            {
                this.buffer.Stop();
                this.IsPaused = true;
            }
        }
        public void Shutdown()
        {
            this.IsPaused = true;
            if (this.buffer != null && this.buffer.Disposed & !this.IsRendering)
            {
                try
                {
                    this.buffer.Stop();
                }
                catch { return; }
            }
            while (this.IsRendering)
            { }
            this.buffer.Dispose();
            this._SoundDevice.Dispose();
            if (this.RECODER.IsRecording)
                this.RECODER.Stop();
        }
        /// <summary>
        /// Set the volume 
        /// </summary>
        /// <param name="Vol">The volume level (-3000 = min, 0 = max)</param>
        public void SetVolume(int Vol)
        {
            if (this.buffer != null && !this.buffer.Disposed & !this.IsRendering)
            {
                this.buffer.Volume = Vol;
            }
        }
        /// <summary>
        /// Set the pan
        /// </summary>
        /// <param name="Pan">The pan</param>
        public void SetPan(int Pan)
        {
            if (this.buffer != null && !this.buffer.Disposed & !this.IsRendering)
            {
                this.buffer.Pan = Pan;
            }
        }
        #region Registers
        //Rec 1
        public void Write_4000(byte data) { this._Chn_REC1.Write_4000(data); }
        public void Write_4001(byte data) { this._Chn_REC1.Write_4001(data); }
        public void Write_4002(byte data) { this._Chn_REC1.Write_4002(data); }
        public void Write_4003(byte data) { this._Chn_REC1.Write_4003(data); }
        //Rec 2
        public void Write_4004(byte data) { this._Chn_REC2.Write_4004(data); }
        public void Write_4005(byte data) { this._Chn_REC2.Write_4005(data); }
        public void Write_4006(byte data) { this._Chn_REC2.Write_4006(data); }
        public void Write_4007(byte data) { this._Chn_REC2.Write_4007(data); }
        //Trl 
        public void Write_4008(byte data) { this._Chn_TRL.Write_4008(data); }
        public void Write_400A(byte data) { this._Chn_TRL.Write_400A(data); }
        public void Write_400B(byte data) { this._Chn_TRL.Write_400B(data); }
        //Noz 
        public void Write_400C(byte data) { this._Chn_NOZ.Write_400C(data); }
        public void Write_400E(byte data) { this._Chn_NOZ.Write_400E(data); }
        public void Write_400F(byte data) { this._Chn_NOZ.Write_400F(data); }
        //DMC
        public void Write_4010(byte data) { this._Chn_DMC.Write_4010(data); }
        public void Write_4011(byte data) { this._Chn_DMC.Write_4011(data); }
        public void Write_4012(byte data) { this._Chn_DMC.Write_4012(data); }
        public void Write_4013(byte data) { this._Chn_DMC.Write_4013(data); }
        //Status
        public void Write_4015(byte data)
        {
            this._Chn_REC1.Enabled = (data & 0x01) != 0;
            this._Chn_REC2.Enabled = (data & 0x02) != 0;
            this._Chn_TRL.Enabled = (data & 0x04) != 0;
            this._Chn_NOZ.Enabled = (data & 0x08) != 0;
            this._Chn_DMC.Enabled = (data & 0x10) != 0;
            this.DMCIRQPending = false;
        }
        public byte Read_4015()
        {
            byte rt = 0;
            if (this._Chn_REC1.Enabled)
                rt |= 0x01;
            if (this._Chn_REC2.Enabled)
                rt |= 0x02;
            if (this._Chn_TRL.Enabled)
                rt |= 0x04;
            if (this._Chn_NOZ.Enabled)
                rt |= 0x08;
            if (this._Chn_DMC.Enabled)
                rt |= 0x10;
            if (this.FrameIRQPending)
                rt |= 0x40;
            if (this.DMCIRQPending)
                rt |= 0x80;
            this.FrameIRQPending = false;
            //Find IRQ
            if (this.FrameIRQPending)
                this._Nes.CPU.IRQNextTime = true;
            else if (this.DMCIRQPending)
                this._Nes.CPU.IRQNextTime = true;
            return rt;
        }
        public void Write_4017(byte data)
        {
            this._PAL = (data & 0x80) != 0;
            this._FrameCounter = 0;
            this.FrameIRQEnabled = (data & 0x40) == 0;
            if (this.FrameIRQEnabled)
                this.FrameIRQPending = false;
            //Find IRQ
            if (this.FrameIRQPending)
                this._Nes.CPU.IRQNextTime = true;
            else if (this.DMCIRQPending)
                this._Nes.CPU.IRQNextTime = true;
        }
        #endregion
        #region Properties
        public bool Square1Enabled { get { return this._Enabled_REC1; } set { this._Enabled_REC1 = value; } }
        public bool Square2Enabled { get { return this._Enabled_REC2; } set { this._Enabled_REC2 = value; } }
        public bool TriangleEnabled { get { return this._Enabled_TRL; } set { this._Enabled_TRL = value; } }
        public bool NoiseEnabled { get { return this._Enabled_NOZ; } set { this._Enabled_NOZ = value; } }
        public bool VRC6P1Enabled { get { return this._Enabled_VRC6Pulse1; } set { this._Enabled_VRC6Pulse1 = value; } }
        public bool VRC6P2Enabled { get { return this._Enabled_VRC6Pulse2; } set { this._Enabled_VRC6Pulse2 = value; } }
        public bool DMCEnabled { get { return this._Enabled_DMC; } set { this._Enabled_DMC = value; } }
        public bool VRC6SawToothEnabled { get { return this._Enabled_VRC6SawTooth; } set { this._Enabled_VRC6SawTooth = value; } }
        public int FrameCounter { get { return this._FrameCounter; } set { this._FrameCounter = value; } }
        public bool PAL { get { return this._PAL; } set { this._PAL = value; } }
        public Chn_DMC DMC { get { return this._Chn_DMC; } }
        public Chn_Noize NOIZE { get { return this._Chn_NOZ; } }
        public Chn_Rectangle1 RECT1 { get { return this._Chn_REC1; } }
        public Chn_Rectangle2 RECT2 { get { return this._Chn_REC2; } }
        public Chn_Triangle TRIANGLE { get { return this._Chn_TRL; } }
        public Chn_VRC6Pulse1 VRC6PULSE1 { get { return this._Chn_VRC6Pulse1; } }
        public Chn_VRC6Pulse2 VRC6PULSE2 { get { return this._Chn_VRC6Pulse2; } }
        public Chn_VRC6Sawtooth VRC6SAWTOOTH { get { return this._Chn_VRC6Sawtooth; } }
        #endregion
    }
}
