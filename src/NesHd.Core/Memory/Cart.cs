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

using System.Text;
using System.IO;

using NesHd.Core.Debugger;

namespace NesHd.Core.Memory
{
    /*
    * INESHeaderReader
    * ****************
    * This class used only for reading the header of INES
    * rom format file, so you can use it ONLY for browser,
    * to check the validate of rom and to detected rom info 
    * like mapper #.
    * I uses this in browser while reading a rom, 'cause
    * it's faster (it doesn't load any PRG or CHR).
    * ********************************************
    * IMPORTANT : when you add / update a mapper *
    * don't forget to assign it here             *
    * (in SupportedMappersNo)                    *
    * to make it available.                      *
    * ********************************************
    */
    public class INESHeaderReader
    {
        //ADD YOUR NEW MAPPER # HERE, ALSO MAKE THE NEW MAPPER INITAILIZE
        //AT InitializeMapper() IN MAP.cs CLASS.
        public static int[] SupportedMappersNo = 
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 16, 17,
            18, 19, 21, 22, 23, 24, 32, 33, 34, 41, 48, 61, 64, 
            65, 66, 69, 71, 78, 79, 80, 81, 82, 91, 113, 225, 255
        };
        // Fields
        public int ChrRomPageCount;
        public bool FourScreenVRAMLayout;
        public int MemoryMapper;
        public int PrgRomPageCount;
        public bool SRamEnabled;
        public bool TrainerPresent512;
        public bool validRom;
        public bool VerticalMirroring;
        public string FilePath = "";
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
        public INESHeaderReader(string fileName)
        {
            this.FilePath = fileName;
            try
            {
                using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    Encoding aSCII = Encoding.ASCII;
                    byte[] buffer = new byte[0x80];
                    stream.Read(buffer, 0, 3);
                    this.validRom = true;
                    if (aSCII.GetString(buffer, 0, 3) != "NES")
                    {
                        this.validRom = false;
                    }
                    if (stream.ReadByte() != 0x1a)
                    {
                        this.validRom = false;
                    }
                    this.PrgRomPageCount = stream.ReadByte();
                    this.ChrRomPageCount = stream.ReadByte();
                    int num = stream.ReadByte();

                    this.VerticalMirroring = (num & 1) == 1;
                    this.SRamEnabled = (num & 2) == 2;
                    this.TrainerPresent512 = (num & 4) == 4;
                    this.FourScreenVRAMLayout = (num & 8) == 8;

                    this.MemoryMapper = num >> 4;
                    int num2 = stream.ReadByte();
                    if ((num2 & 15) != 0)
                    {
                        num2 = 0;
                    }
                    this.MemoryMapper |= num2 & 240;
                    stream.Read(buffer, 0, 8);
                    stream.Close();
                }
            }
            catch { this.validRom = false; }
        }

        public bool SupportedMapper()
        {
            foreach (int mapp in SupportedMappersNo)
            {
                if (mapp == this.MemoryMapper)
                    return true;
            }
            return false;
        }
        public bool SupportedMapper(int MapperNo)
        {
            foreach (int mapp in SupportedMappersNo)
            {
                if (mapp == MapperNo)
                    return true;
            }
            return false;
        }
        public string GetMapperName()
        {
            switch (this.MemoryMapper)
            {
                case 0: return "NROM, no mapper";
                case 1: return "MMC1";
                case 2: return "UNROM";
                case 3: return "CNROM";
                case 4: return "MMC3";
                case 5: return "MMC5";
                case 6: return "FFE F4xxx";
                case 7: return "AOROM";
                case 8: return "FFE F3xxx";
                case 9: return "MMC2";
                case 10: return "MMC4";
                case 11: return "ColorDreams chip";
                case 12: return "FFE F6xxx";
                case 13: return "ColorDreams chip";
                case 15: return "100-in-1 switch";
                case 16: return "Bandai chip";
                case 17: return "FFE F8xxx";
                case 18: return "Jaleco SS8806 chip";
                case 19: return "Namcot 106 chip";
                case 20: return "Nintendo DiskSystem";
                case 21: return "Konami VRC4a";
                case 22: return "Konami VRC2a";
                case 23: return "Konami VRC2a";
                case 24: return "Konami VRC6";
                case 25: return "Konami VRC4b";
                case 32: return "Irem G-101 chip";
                case 33: return "Taito TC0190/TC0350";
                case 34: return "32 KB ROM switch";
                case 41: return "Caltron 6-in-1";
                case 48: return "Taito TC190V";
                case 61: return "20-in-1";
                case 64: return "Tengen RAMBO-1 chip";
                case 65: return "Irem H-3001 chip";
                case 66: return "GNROM";
                case 67: return "SunSoft3 chip";
                case 68: return "SunSoft4 chip";
                case 69: return "SunSoft5 FME-7 chip";
                case 71: return "Camerica chip";
                case 78: return "Irem 74HC161/32-based";
                case 80: return "Taito X-005";
                case 82: return "Taito C075";
                case 91: return "Pirate HK-SF3 chip";
                case 119: return "MMC3 TQROM with VROM + VRAM Pattern Tables";
                case 225: return "X-in-1";
                case 255: return "X-in-1";
                default: return "???";
            }
        }
        public static string GetMapperName(int MapperNo)
        {
            switch (MapperNo)
            {
                case 0: return "NROM, no mapper";
                case 1: return "MMC1";
                case 2: return "UNROM";
                case 3: return "CNROM";
                case 4: return "MMC3";
                case 5: return "MMC5";
                case 6: return "FFE F4xxx";
                case 7: return "AOROM";
                case 8: return "FFE F3xxx";
                case 9: return "MMC2";
                case 10: return "MMC4";
                case 11: return "ColorDreams chip";
                case 12: return "FFE F6xxx";
                case 13: return "ColorDreams chip";
                case 15: return "100-in-1 switch";
                case 16: return "Bandai chip";
                case 17: return "FFE F8xxx";
                case 18: return "Jaleco SS8806 chip";
                case 19: return "Namcot 106 chip";
                case 20: return "Nintendo DiskSystem";
                case 21: return "Konami VRC4a";
                case 22: return "Konami VRC2a";
                case 23: return "Konami VRC2a";
                case 24: return "Konami VRC6";
                case 25: return "Konami VRC4b";
                case 32: return "Irem G-101 chip";
                case 33: return "Taito TC0190/TC0350";
                case 34: return "32 KB ROM switch";
                case 41: return "Caltron 6-in-1";
                case 48: return "Taito TC190V";
                case 61: return "20-in-1";
                case 64: return "Tengen RAMBO-1 chip";
                case 65: return "Irem H-3001 chip";
                case 66: return "GNROM";
                case 67: return "SunSoft3 chip";
                case 68: return "SunSoft4 chip";
                case 69: return "SunSoft5 FME-7 chip";
                case 71: return "Camerica chip";
                case 78: return "Irem 74HC161/32-based";
                case 80: return "Taito X-005";
                case 82: return "Taito C075";
                case 91: return "Pirate HK-SF3 chip";
                case 119: return "MMC3 TQROM with VROM + VRAM Pattern Tables";
                case 225: return "X-in-1";
                case 255: return "X-in-1";
                default: return "???";
            }
        }
    }
    public enum MIRRORING { HORIZONTAL, VERTICAL, FOUR_SCREEN, ONE_SCREEN };
    public class Cart
    {
        Memory MEM;
        /// <summary>
        /// This is the cart that the nes uses and accesses all the time.
        /// </summary>
        /// <param name="Mem"></param>
        public Cart(Memory Mem)
        { this.MEM = Mem; }
        byte[][] _PRG;
        byte[][] _CHR;
        MIRRORING _MIRRORING;
        bool _trainer_present;
        bool _save_ram_present;
        bool _is_vram;
        byte _MAPPERNO;
        byte _PRG_PAGES;
        byte _CHR_PAGES;
        string _FilePath = "";
        public string saveFilename = "";
        uint _mirroringBase; //For one screen mirroring
        public bool LoadCart(string FilePath)
        {
            this._FilePath = FilePath;
            byte[] nesHeader = new byte[16];
            int i;
            try
            {
                Debug.WriteLine(this, "Loading rom ...", DebugStatus.None);
                using (FileStream reader = File.Open(this._FilePath, FileMode.Open, FileAccess.Read))
                {
                    reader.Read(nesHeader, 0, 16);
                    if ((nesHeader[6] & 0x4) != 0x0)//Load the trainer
                    {
                        reader.Read(this.MEM.SRAM, 0x1000, 512);
                        Debug.WriteLine(this, "Trainer loaded.", DebugStatus.None);
                    }
                    int prg_roms = nesHeader[4] * 4;
                    this._PRG_PAGES = nesHeader[4];
                    Debug.WriteLine(this, "PRG pages = " + this._PRG_PAGES, DebugStatus.None);
                    this._PRG = new byte[prg_roms][];
                    for (i = 0; i < (prg_roms); i++)
                    {
                        this._PRG[i] = new byte[4096];
                        reader.Read(this._PRG[i], 0, 4096);
                    }
                    int chr_roms = nesHeader[5] * 8;
                    this._CHR_PAGES = nesHeader[5];
                    Debug.WriteLine(this, "CHR pages = " + this._CHR_PAGES, DebugStatus.None);
                    if (this._CHR_PAGES != 0)
                    {
                        this._CHR = new byte[chr_roms][];
                        for (i = 0; i < (chr_roms); i++)
                        {
                            this._CHR[i] = new byte[1024];
                            reader.Read(this._CHR[i], 0, 1024);
                        }
                        this._is_vram = false;
                    }
                    else
                    {
                        this._CHR = new byte[512][];
                        for (i = 0; i < 512; i++)
                        {
                            this._CHR[i] = new byte[1024];
                        }
                        this._is_vram = true;
                    }
                    if ((nesHeader[6] & 0x1) == 0x0)
                    {
                        this._MIRRORING = MIRRORING.HORIZONTAL;
                    }
                    else
                    {
                        this._MIRRORING = MIRRORING.VERTICAL;
                    }

                    if ((nesHeader[6] & 0x2) == 0x0)
                    {
                        this._save_ram_present = false;
                    }
                    else
                    {
                        this._save_ram_present = true;
                    }

                    if ((nesHeader[6] & 0x4) == 0x0)
                    {
                        this._trainer_present = false;
                    }
                    else
                    {
                        this._trainer_present = true;
                    }

                    if ((nesHeader[6] & 0x8) != 0x0)
                    {
                        this._MIRRORING = MIRRORING.FOUR_SCREEN;
                    }
                    Debug.WriteLine(this, "Mirroring = " + this._MIRRORING.ToString(), DebugStatus.None);
                    if ((nesHeader[7] & 0xF) == 0)
                        this._MAPPERNO = (byte)((nesHeader[7] & 0xF0) | ((nesHeader[6] & 0xF0) >> 4));
                    else
                        this._MAPPERNO = (byte)((nesHeader[6] & 0xF0) >> 4);
                    Debug.WriteLine(this, "Mapper # = " + this._MAPPERNO.ToString(), DebugStatus.None);
                    #region Fixes
                    //smb3
                    if ((this._PRG[0][0x75] == 0x11) &&
                                (this._PRG[0][0x76] == 0x12) &&
                                (this._PRG[0][0x77] == 0x13) &&
                                (this._PRG[0][0x78] == 0x14) &&
                                (this._PRG[0][0x79] == 0x07) &&
                                (this._PRG[0][0x7a] == 0x03) &&
                                (this._PRG[0][0x7b] == 0x03) &&
                                (this._PRG[0][0x7c] == 0x03) &&
                                (this._PRG[0][0x7d] == 0x03)
                                )
                    {
                        this.MEM.MAP.NES.PPU.FIX_scroll3 = true;
                    }
                    //werewolf
                    if ((this._PRG[0][0x76] == 15) &&
                                (this._PRG[0][0x77] == 8) &&
                                (this._PRG[0][0x78] == 24) &&
                                (this._PRG[0][0x79] == 40)
                                )
                    {
                        this.MEM.MAP.NES.PPU.FIX_scroll = true;
                    }
                    /*int A = _PRG[1][0x76];
                    int A1 = _PRG[1][0x77];
                    int A2 = _PRG[1][0x78];
                    int A3 = _PRG[1][0x79];*/
                    //karnov
                    if ((this._PRG[1][0x76] == 183) &&
                              (this._PRG[1][0x77] == 247) &&
                              (this._PRG[1][0x78] == 253) &&
                              (this._PRG[1][0x79] == 254)
                              )
                    {
                        this.MEM.MAP.NES.PPU.FIX_scroll = true;
                    }
                    //Aladdin 3
                    if ((this._PRG[1][0x76] == 254) &&
                              (this._PRG[1][0x77] == 0) &&
                              (this._PRG[1][0x78] == 22) &&
                              (this._PRG[1][0x79] == 0)
                              )
                    {
                        this.MEM.MAP.NES.PPU.FIX_scroll = true;
                    }
                    //zelda
                    if ((this._PRG[0][0x76] == 255) &&
                              (this._PRG[0][0x77] == 255) &&
                              (this._PRG[0][0x78] == 255) &&
                              (this._PRG[0][0x79] == 255)
                              )
                    {
                        this.MEM.MAP.NES.PPU.FIX_scroll2 = true;
                    }
                    //mappers
                    if (this._MAPPERNO == 225 | this._MAPPERNO == 255 | this._MAPPERNO == 16)
                        this.MEM.MAP.NES.PPU.FIX_scroll = true;
                    #endregion
                }
            }
            catch
            {
                Debug.WriteLine(this, "Can't read the rom.", DebugStatus.Error);
                return false;
            }

            if (this._save_ram_present)
            {
                this.saveFilename = this._FilePath.Remove(this._FilePath.Length - 3, 3);
                this.saveFilename = this.saveFilename.Insert(this.saveFilename.Length, "sav");
                Debug.WriteLine(this, "Trying to read SRAM from file : " + this.saveFilename, DebugStatus.None);
                try
                {
                    using (FileStream reader = File.OpenRead(this.saveFilename))
                    {
                        reader.Read(this.MEM.SRAM, 0, 0x2000);
                        Debug.WriteLine(this, "Done read SRAM.", DebugStatus.Cool);
                    }
                }
                catch
                {
                    Debug.WriteLine(this, "Faild to read SRAM", DebugStatus.Warning);
                    //Ignore it, we'll make our own.
                }
            }
            Debug.WriteLine(this, "Cart read OK !!", DebugStatus.Cool);
            return true;
        }
        //Properties
        /// <summary>
        /// Get or set the PRG rom pages
        /// </summary>
        public byte[][] PRG
        { get { return this._PRG; } set { this._PRG = value; } }
        /// <summary>
        /// Get or set the CHR rom pages
        /// </summary>
        public byte[][] CHR
        { get { return this._CHR; } set { this._CHR = value; } }
        /// <summary>
        /// Get or set the current mirroring type
        /// </summary>
        public MIRRORING Mirroring
        { get { return this._MIRRORING; } set { this._MIRRORING = value; } }
        /// <summary>
        /// Get if trainer
        /// </summary>
        public bool IsTrainer
        { get { return this._trainer_present; } }
        /// <summary>
        /// Get or set if save ram
        /// </summary>
        public bool IsSaveRam
        { get { return this._save_ram_present; } set { this._save_ram_present = value; } }
        /// <summary>
        /// Get if vram or not, true if chr pages count = 0
        /// </summary>
        public bool IsVRAM
        { get { return this._is_vram; } set { this._is_vram = value; } }
        /// <summary>
        /// Get the count of the prg pages
        /// </summary>
        public byte PRG_PAGES
        { get { return this._PRG_PAGES; } }
        /// <summary>
        /// Get the count of the chr pages
        /// </summary>
        public byte CHR_PAGES
        { get { return this._CHR_PAGES; } }
        /// <summary>
        /// Get the mapper #
        /// </summary>
        public byte MapperNo
        { get { return this._MAPPERNO; } }
        /// <summary>
        /// Get the rom path, can be set only via LoadCart()
        /// </summary>
        public string RomPath
        { get { return this._FilePath; } }
        public uint MirroringBase
        { get { return this._mirroringBase; } set { this._mirroringBase = value; } }
    }
}