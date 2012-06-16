using System;
using System.IO;
using System.Text;
using NesHd.Core.Debugger;

namespace NesHd.Core.APU
{
    /* Written by AHD at Wednesday, January 13, 2010.
     * Wave recorder for PCM/16-BIT/Mono or Stereo/44100Hz
     * Written this for debugging the sound
     * So that you can open generated audio in
     * Wave programs.
     */

    public class WaveRecorder
    {
        public bool IsRecording;
        private int NoOfSamples;
        private int SIZE;
        public bool STEREO;
        private Stream STR;
        public int Time;
        private int TimeSamples;

        public void Record(string FilePath, bool Stereo)
        {
            STEREO = Stereo;
            Time = 0;
            //Create the stream first
            STR = new FileStream(FilePath, FileMode.Create);
            var ASCII = new ASCIIEncoding();
            //1 Write the header "RIFF"
            STR.Write(ASCII.GetBytes("RIFF"), 0, 4);
            //2 Write Chunck Size (0 for now)
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            //3 Write WAVE
            STR.Write(ASCII.GetBytes("WAVE"), 0, 4);
            //4 Write "fmt "
            STR.Write(ASCII.GetBytes("fmt "), 0, 4);
            //5 Write Chunck Size (16 for PCM)
            STR.WriteByte(0x10);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            //6 Write audio format (1 = PCM)
            STR.WriteByte(0x01);
            STR.WriteByte(0x00);
            //7 Number of channels
            if (STEREO)
            {
                STR.WriteByte(0x02);
                STR.WriteByte(0x00);
            }
            else
            {
                STR.WriteByte(0x01);
                STR.WriteByte(0x00);
            }
            //8 Sample Rate (44100)
            STR.WriteByte(0x44);
            STR.WriteByte(0xAC);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            //9 Byte Rate 
            if (STEREO) //(176400)
            {
                STR.WriteByte(0x00);
                STR.WriteByte(0xEE);
                STR.WriteByte(0x02);
                STR.WriteByte(0x00);
            }
            else //(88200)
            {
                STR.WriteByte(0x88);
                STR.WriteByte(0x58);
                STR.WriteByte(0x01);
                STR.WriteByte(0x00);
            }
            //10 Block Align
            if (STEREO) //(4)
            {
                STR.WriteByte(0x04);
                STR.WriteByte(0x00);
            }
            else //(2)
            {
                STR.WriteByte(0x02);
                STR.WriteByte(0x00);
            }
            //11 Bits Per Sample (16)
            STR.WriteByte(0x10);
            STR.WriteByte(0x00);
            //12 Write "data"
            STR.Write(ASCII.GetBytes("data"), 0, 4);
            //13 Write Chunck Size (0 for now)
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            STR.WriteByte(0x00);
            //Confirm
            IsRecording = true;
            Debug.WriteLine(this, "Record wave started.", DebugStatus.None);
        }

        public void AddSample(short Sample)
        {
            if (!IsRecording)
                return;
            STR.WriteByte((byte) ((Sample & 0xFF00) >> 8));
            STR.WriteByte((byte) (Sample & 0xFF));
            if (STEREO) //Add the same sample to the left channel
            {
                STR.WriteByte((byte) ((Sample & 0xFF00) >> 8));
                STR.WriteByte((byte) (Sample & 0xFF));
            }
            NoOfSamples++;
            TimeSamples++;
            if (TimeSamples >= 44100)
            {
                Time++;
                TimeSamples = 0;
            }
        }

        public void Stop()
        {
            if (IsRecording & STR != null)
            {
                if (STEREO)
                {
                    NoOfSamples *= 4;
                }
                else
                {
                    NoOfSamples *= 2;
                }
                SIZE = NoOfSamples + 36;
                var buff_size = new byte[4];
                var buff_NoOfSammples = new byte[4];
                buff_size = BitConverter.GetBytes(SIZE);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buff_size);
                buff_NoOfSammples = BitConverter.GetBytes(NoOfSamples);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buff_NoOfSammples);
                IsRecording = false;
                STR.Position = 4;
                STR.Write(buff_size, 0, 4);
                STR.Position = 40;
                STR.Write(buff_NoOfSammples, 0, 4);
                STR.Close();
                Debug.WriteLine(this, "Wave recorded .. ENJOY !!", DebugStatus.Cool);
            }
        }
    }
}