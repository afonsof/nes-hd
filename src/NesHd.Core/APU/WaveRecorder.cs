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

using System;
using System.Text;
using System.IO;

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
        Stream STR;
        public bool IsRecording = false;
        int SIZE = 0;
        int NoOfSamples = 0;
        public int Time = 0;
        int TimeSamples = 0;
        public bool STEREO = false;
        public void Record(string FilePath, bool Stereo)
        {
            this.STEREO = Stereo;
            this.Time = 0;
            //Create the stream first
            this.STR = new FileStream(FilePath, FileMode.Create);
            ASCIIEncoding ASCII = new ASCIIEncoding();
            //1 Write the header "RIFF"
            this.STR.Write(ASCII.GetBytes("RIFF"), 0, 4);
            //2 Write Chunck Size (0 for now)
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            //3 Write WAVE
            this.STR.Write(ASCII.GetBytes("WAVE"), 0, 4);
            //4 Write "fmt "
            this.STR.Write(ASCII.GetBytes("fmt "), 0, 4);
            //5 Write Chunck Size (16 for PCM)
            this.STR.WriteByte(0x10);
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            //6 Write audio format (1 = PCM)
            this.STR.WriteByte(0x01);
            this.STR.WriteByte(0x00);
            //7 Number of channels
            if (this.STEREO)
            {
                this.STR.WriteByte(0x02);
                this.STR.WriteByte(0x00);
            }
            else
            {
                this.STR.WriteByte(0x01);
                this.STR.WriteByte(0x00);
            }
            //8 Sample Rate (44100)
            this.STR.WriteByte(0x44);
            this.STR.WriteByte(0xAC);
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            //9 Byte Rate 
            if (this.STEREO)//(176400)
            {
                this.STR.WriteByte(0x00);
                this.STR.WriteByte(0xEE);
                this.STR.WriteByte(0x02);
                this.STR.WriteByte(0x00);
            }
            else//(88200)
            {
                this.STR.WriteByte(0x88);
                this.STR.WriteByte(0x58);
                this.STR.WriteByte(0x01);
                this.STR.WriteByte(0x00);
            }
            //10 Block Align
            if (this.STEREO)//(4)
            {
                this.STR.WriteByte(0x04);
                this.STR.WriteByte(0x00);
            }
            else //(2)
            {
                this.STR.WriteByte(0x02);
                this.STR.WriteByte(0x00);
            }
            //11 Bits Per Sample (16)
            this.STR.WriteByte(0x10);
            this.STR.WriteByte(0x00);
            //12 Write "data"
            this.STR.Write(ASCII.GetBytes("data"), 0, 4);
            //13 Write Chunck Size (0 for now)
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            this.STR.WriteByte(0x00);
            //Confirm
            this.IsRecording = true;
            Debug.WriteLine(this, "Record wave started.", DebugStatus.None);
        }
        public void AddSample(short Sample)
        {
            if (!this.IsRecording)
                return;
            this.STR.WriteByte((byte)((Sample & 0xFF00) >> 8));
            this.STR.WriteByte((byte)(Sample & 0xFF));
            if (this.STEREO)//Add the same sample to the left channel
            {
                this.STR.WriteByte((byte)((Sample & 0xFF00) >> 8));
                this.STR.WriteByte((byte)(Sample & 0xFF));
            }
            this.NoOfSamples++;
            this.TimeSamples++;
            if (this.TimeSamples >= 44100)
            {
                this.Time++;
                this.TimeSamples = 0;
            }
        }
        public void Stop()
        {
            if (this.IsRecording & this.STR != null)
            {
                if (this.STEREO)
                {
                    this.NoOfSamples *= 4;
                }
                else
                {
                    this.NoOfSamples *= 2;
                }
                this.SIZE = this.NoOfSamples + 36;
                byte[] buff_size = new byte[4];
                byte[] buff_NoOfSammples = new byte[4];
                buff_size = BitConverter.GetBytes(this.SIZE);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buff_size);
                buff_NoOfSammples = BitConverter.GetBytes(this.NoOfSamples);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buff_NoOfSammples);
                this.IsRecording = false;
                this.STR.Position = 4;
                this.STR.Write(buff_size, 0, 4);
                this.STR.Position = 40;
                this.STR.Write(buff_NoOfSammples, 0, 4);
                this.STR.Close();
                Debug.WriteLine(this, "Wave recorded .. ENJOY !!", DebugStatus.Cool);
            }
        }
    }
}
