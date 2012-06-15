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

using System.IO;

namespace NesHd.Core.Misc
{
    public class Paletter
    {
        static int[] _NTSCPalette =
        {
        0x788084,0x0000FC,0x0000C4,0x4028C4,0x94008C,0xAC0028,0xAC1000,0x8C1800,
        0x503000,0x007800,0x006800,0x005800,0x004058,0x000000,0x000000,0x000008,
        0xBCC0C4,0x0078FC,0x0088FC,0x6848FC,0xDC00D4,0xE40060,0xFC3800,0xE46018,
        0xAC8000,0x00B800,0x00A800,0x00A848,0x008894,0x2C2C2C,0x000000,0x000000,
        0xFCF8FC,0x38C0FC,0x6888FC,0x9C78FC,0xFC78FC,0xFC589C,0xFC7858,0xFCA048,
        0xFCB800,0xBCF818,0x58D858,0x58F89C,0x00E8E4,0x606060,0x000000,0x000000,
        0xFCF8FC,0xA4E8FC,0xBCB8FC,0xDCB8FC,0xFCB8FC,0xF4C0E0,0xF4D0B4,0xFCE0B4,
        0xFCD884,0xDCF878,0xB8F878,0xB0F0D8,0x00F8FC,0xC8C0C0,0x000000,0x000000
        };
        static int[] _PALPalette = new int[] 
        { 
        0x808080, 0xbb, 0x3700bf, 0x8400a6, 0xbb006a, 0xb7001e, 0xb30000, 0x912600, 
        0x7b2b00, 0x3e00, 0x480d, 0x3c22, 0x2f66, 0, 0x50505, 0x50505, 
        0xc8c8c8, 0x59ff, 0x443cff, 0xb733cc, 0xff33aa, 0xff375e, 0xff371a, 0xd54b00,
        0xc46200, 0x3c7b00, 0x1e8415, 0x9566, 0x84c4, 0x111111, 0x90909, 0x90909, 
        0xffffff, 0x95ff, 0x6f84ff, 0xd56fff, 0xff77cc, 0xff6f99, 0xff7b59, 0xff915f,
        0xffa233, 0xa6bf00, 0x51d96a, 0x4dd5ae, 0xd9ff, 0x666666, 0xd0d0d, 0xd0d0d, 
        0xffffff, 0x84bfff, 0xbbbbff, 0xd0bbff, 0xffbfea, 0xffbfcc, 0xffc4b7, 0xffccae,
        0xffd9a2, 0xcce199, 0xaeeeb7, 0xaaf7ee, 0xb3eeff, 0xdddddd, 0x111111, 0x111111
        };
        public static int[] NTSCPalette
        { get { return _NTSCPalette; } }
        public static int[] PALPalette
        { get { return _PALPalette; } }
        public static int[] LoadPalette(string FilePath)
        {
            int[] Nes_Palette = new int[64];
            if (File.Exists(FilePath))
            {
                Stream STR = new FileStream(FilePath, FileMode.Open);
                byte[] buffer = new byte[192];
                STR.Read(buffer, 0, 192);
                int j = 0;
                for (int i = 0; i < 64; i++)
                {
                    byte RedValue = buffer[j]; j++;
                    byte GreenValue = buffer[j]; j++;
                    byte BlueValue = buffer[j]; j++;
                    Nes_Palette[i] = (0xFF << 24) | (RedValue << 16) |
                        (GreenValue << 8) | BlueValue;
                }
                STR.Close();
                return Nes_Palette;
            }
            return null;
        }
    }
}
