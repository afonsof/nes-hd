using System.IO;
using System.Linq;
using System.Text;

namespace NesHd.Core.Memory
{
    public class NesHeaderReader
    {
        //ADD YOUR NEW MAPPER # HERE, ALSO MAKE THE NEW MAPPER INITAILIZE
        //AT InitializeMapper() IN _map.cs CLASS.
        public static int[] SupportedMappersNo =
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 16, 17,
                18, 19, 21, 22, 23, 24, 32, 33, 34, 41, 48, 61, 64,
                65, 66, 69, 71, 78, 79, 80, 81, 82, 91, 113, 225, 255
            };

        // Fields
        public int ChrRomPageCount;
        public string FilePath = "";
        public bool FourScreenVRamLayout;
        public int MemoryMapper;
        public int PrgRomPageCount;
        public bool SRamEnabled;
        public bool TrainerPresent512;
        public bool VerticalMirroring;
        public bool ValidRom;

        /// <summary>
        /// This class used only for reading the header of INES
        /// rom format file, so you can use it ONLY for browser,
        /// to check the validate of rom and to detected rom info 
        /// like mapper #.
        /// I uses this in browser while reading a rom, 'cause
        /// it's faster (it doesn't load any PRG or CHR).
        /// 
        /// IMPORTANT : when you add / update a mapper
        /// don't forget to assign it here             
        /// (in SupportedMappersNo & SupportedMapper())
        /// to make it available.                      
        /// </summary>
        /// <param name="fileName">The rom file to check</param>
        public NesHeaderReader(string fileName)
        {
            FilePath = fileName;
            try
            {
                using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    var ascii = Encoding.ASCII;
                    var buffer = new byte[0x80];
                    stream.Read(buffer, 0, 3);
                    ValidRom = true;
                    if (ascii.GetString(buffer, 0, 3) != "NES")
                    {
                        ValidRom = false;
                    }
                    if (stream.ReadByte() != 0x1a)
                    {
                        ValidRom = false;
                    }
                    PrgRomPageCount = stream.ReadByte();
                    ChrRomPageCount = stream.ReadByte();
                    var num = stream.ReadByte();

                    VerticalMirroring = (num & 1) == 1;
                    SRamEnabled = (num & 2) == 2;
                    TrainerPresent512 = (num & 4) == 4;
                    FourScreenVRamLayout = (num & 8) == 8;

                    MemoryMapper = num >> 4;
                    var num2 = stream.ReadByte();
                    if ((num2 & 15) != 0)
                    {
                        num2 = 0;
                    }
                    MemoryMapper |= num2 & 240;
                    stream.Read(buffer, 0, 8);
                    stream.Close();
                }
            }
            catch
            {
                ValidRom = false;
            }
        }

        public bool SupportedMapper()
        {
            return SupportedMappersNo.Any(mapp => mapp == MemoryMapper);
        }

        public bool SupportedMapper(int mapperNo)
        {
            return SupportedMappersNo.Any(mapp => mapp == mapperNo);
        }

        public string GetMapperName()
        {
            switch (MemoryMapper)
            {
                case 0:
                    return "NROM, no mapper";
                case 1:
                    return "MMC1";
                case 2:
                    return "UNROM";
                case 3:
                    return "CNROM";
                case 4:
                    return "MMC3";
                case 5:
                    return "MMC5";
                case 6:
                    return "FFE F4xxx";
                case 7:
                    return "AOROM";
                case 8:
                    return "FFE F3xxx";
                case 9:
                    return "MMC2";
                case 10:
                    return "MMC4";
                case 11:
                    return "ColorDreams chip";
                case 12:
                    return "FFE F6xxx";
                case 13:
                    return "ColorDreams chip";
                case 15:
                    return "100-in-1 switch";
                case 16:
                    return "Bandai chip";
                case 17:
                    return "FFE F8xxx";
                case 18:
                    return "Jaleco SS8806 chip";
                case 19:
                    return "Namcot 106 chip";
                case 20:
                    return "Nintendo DiskSystem";
                case 21:
                    return "Konami VRC4a";
                case 22:
                    return "Konami VRC2a";
                case 23:
                    return "Konami VRC2a";
                case 24:
                    return "Konami VRC6";
                case 25:
                    return "Konami VRC4b";
                case 32:
                    return "Irem G-101 chip";
                case 33:
                    return "Taito TC0190/TC0350";
                case 34:
                    return "32 KB ROM switch";
                case 41:
                    return "Caltron 6-in-1";
                case 48:
                    return "Taito TC190V";
                case 61:
                    return "20-in-1";
                case 64:
                    return "Tengen RAMBO-1 chip";
                case 65:
                    return "Irem H-3001 chip";
                case 66:
                    return "GNROM";
                case 67:
                    return "SunSoft3 chip";
                case 68:
                    return "SunSoft4 chip";
                case 69:
                    return "SunSoft5 FME-7 chip";
                case 71:
                    return "Camerica chip";
                case 78:
                    return "Irem 74HC161/32-based";
                case 80:
                    return "Taito X-005";
                case 82:
                    return "Taito C075";
                case 91:
                    return "Pirate HK-SF3 chip";
                case 119:
                    return "MMC3 TQROM with VROM + VRAM Pattern Tables";
                case 225:
                    return "X-in-1";
                case 255:
                    return "X-in-1";
                default:
                    return "???";
            }
        }

        public static string GetMapperName(int mapperNo)
        {
            switch (mapperNo)
            {
                case 0:
                    return "NROM, no mapper";
                case 1:
                    return "MMC1";
                case 2:
                    return "UNROM";
                case 3:
                    return "CNROM";
                case 4:
                    return "MMC3";
                case 5:
                    return "MMC5";
                case 6:
                    return "FFE F4xxx";
                case 7:
                    return "AOROM";
                case 8:
                    return "FFE F3xxx";
                case 9:
                    return "MMC2";
                case 10:
                    return "MMC4";
                case 11:
                    return "ColorDreams chip";
                case 12:
                    return "FFE F6xxx";
                case 13:
                    return "ColorDreams chip";
                case 15:
                    return "100-in-1 switch";
                case 16:
                    return "Bandai chip";
                case 17:
                    return "FFE F8xxx";
                case 18:
                    return "Jaleco SS8806 chip";
                case 19:
                    return "Namcot 106 chip";
                case 20:
                    return "Nintendo DiskSystem";
                case 21:
                    return "Konami VRC4a";
                case 22:
                    return "Konami VRC2a";
                case 23:
                    return "Konami VRC2a";
                case 24:
                    return "Konami VRC6";
                case 25:
                    return "Konami VRC4b";
                case 32:
                    return "Irem G-101 chip";
                case 33:
                    return "Taito TC0190/TC0350";
                case 34:
                    return "32 KB ROM switch";
                case 41:
                    return "Caltron 6-in-1";
                case 48:
                    return "Taito TC190V";
                case 61:
                    return "20-in-1";
                case 64:
                    return "Tengen RAMBO-1 chip";
                case 65:
                    return "Irem H-3001 chip";
                case 66:
                    return "GNROM";
                case 67:
                    return "SunSoft3 chip";
                case 68:
                    return "SunSoft4 chip";
                case 69:
                    return "SunSoft5 FME-7 chip";
                case 71:
                    return "Camerica chip";
                case 78:
                    return "Irem 74HC161/32-based";
                case 80:
                    return "Taito X-005";
                case 82:
                    return "Taito C075";
                case 91:
                    return "Pirate HK-SF3 chip";
                case 119:
                    return "MMC3 TQROM with VROM + VRAM Pattern Tables";
                case 225:
                    return "X-in-1";
                case 255:
                    return "X-in-1";
                default:
                    return "???";
            }
        }
    }
}