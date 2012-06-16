using System.IO;

namespace NesHd.Core.Misc
{
    public class Paletter
    {
        public static int[] NtscPalette
        {
            get { return new[]{
                0x788084, 0x0000FC, 0x0000C4, 0x4028C4, 0x94008C, 0xAC0028, 0xAC1000, 0x8C1800,
                0x503000, 0x007800, 0x006800, 0x005800, 0x004058, 0x000000, 0x000000, 0x000008,
                0xBCC0C4, 0x0078FC, 0x0088FC, 0x6848FC, 0xDC00D4, 0xE40060, 0xFC3800, 0xE46018,
                0xAC8000, 0x00B800, 0x00A800, 0x00A848, 0x008894, 0x2C2C2C, 0x000000, 0x000000,
                0xFCF8FC, 0x38C0FC, 0x6888FC, 0x9C78FC, 0xFC78FC, 0xFC589C, 0xFC7858, 0xFCA048,
                0xFCB800, 0xBCF818, 0x58D858, 0x58F89C, 0x00E8E4, 0x606060, 0x000000, 0x000000,
                0xFCF8FC, 0xA4E8FC, 0xBCB8FC, 0xDCB8FC, 0xFCB8FC, 0xF4C0E0, 0xF4D0B4, 0xFCE0B4,
                0xFCD884, 0xDCF878, 0xB8F878, 0xB0F0D8, 0x00F8FC, 0xC8C0C0, 0x000000, 0x000000
            }; }
        }

        public static int[] PalPalette
        {
            get
            {
                return new[]
                           {
                               0x808080, 0xbb, 0x3700bf, 0x8400a6, 0xbb006a, 0xb7001e,
                               0xb30000, 0x912600,
                               0x7b2b00, 0x3e00, 0x480d, 0x3c22, 0x2f66, 0, 0x50505,
                               0x50505,
                               0xc8c8c8, 0x59ff, 0x443cff, 0xb733cc, 0xff33aa, 0xff375e,
                               0xff371a, 0xd54b00,
                               0xc46200, 0x3c7b00, 0x1e8415, 0x9566, 0x84c4, 0x111111,
                               0x90909, 0x90909,
                               0xffffff, 0x95ff, 0x6f84ff, 0xd56fff, 0xff77cc, 0xff6f99,
                               0xff7b59, 0xff915f,
                               0xffa233, 0xa6bf00, 0x51d96a, 0x4dd5ae, 0xd9ff, 0x666666,
                               0xd0d0d, 0xd0d0d,
                               0xffffff, 0x84bfff, 0xbbbbff, 0xd0bbff, 0xffbfea, 0xffbfcc,
                               0xffc4b7, 0xffccae,
                               0xffd9a2, 0xcce199, 0xaeeeb7, 0xaaf7ee, 0xb3eeff, 0xdddddd,
                               0x111111, 0x111111
                           };
            }
        }

        public static int[] LoadPalette(string filePath)
        {
            var nesPalette = new int[64];
            if (File.Exists(filePath))
            {
                Stream str = new FileStream(filePath, FileMode.Open);
                var buffer = new byte[192];
                str.Read(buffer, 0, 192);
                var j = 0;
                for (var i = 0; i < 64; i++)
                {
                    var redValue = buffer[j];
                    j++;
                    var greenValue = buffer[j];
                    j++;
                    var blueValue = buffer[j];
                    j++;
                    nesPalette[i] = (0xFF << 24) | (redValue << 16) |
                                     (greenValue << 8) | blueValue;
                }
                str.Close();
                return nesPalette;
            }
            return null;
        }
    }
}