using System.Globalization;
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

    public class Cartridge
    {
        private readonly Memory _memory;

        public string SaveFilename = "";

        /// <summary>
        /// This is the cart that the nes uses and accesses all the time.
        /// </summary>
        /// <param name="memory"></param>
        public Cartridge(Memory memory)
        {
            RomPath = "";
            _memory = memory;
        }

        //Properties
        /// <summary>
        /// Get or set the PRG rom pages
        /// </summary>
        public byte[][] Prg { get; set; }

        /// <summary>
        /// Get or set the CHR rom pages
        /// </summary>
        public byte[][] Chr { get; set; }


        /// <summary>
        /// Get or set the current mirroring type
        /// </summary>
        public Mirroring Mirroring { get; set; }

        /// <summary>
        /// Get if trainer
        /// </summary>
        public bool IsTrainer { get; private set; }

        /// <summary>
        /// Get or set if save ram
        /// </summary>
        public bool IsSaveRam { get; set; }

        /// <summary>
        /// Get if vram or not, true if chr pages count = 0
        /// </summary>
        public bool IsVram { get; set; }

        /// <summary>
        /// Get the count of the prg pages
        /// </summary>
        public byte PrgPages { get; private set; }

        /// <summary>
        /// Get the count of the chr pages
        /// </summary>
        public byte ChrPages { get; private set; }

        /// <summary>
        /// Get the mapper #
        /// </summary>
        public byte MapperNo { get; private set; }

        /// <summary>
        /// Get the rom path, can be set only via LoadCart()
        /// </summary>
        public string RomPath { get; private set; }

        public uint MirroringBase { get; set; }

        public bool LoadCart(string filePath)
        {
            RomPath = filePath;
            var nesHeader = new byte[16];
            try
            {
                Debug.WriteLine(this, "Loading rom ...", DebugStatus.None);
                using (var reader = File.Open(RomPath, FileMode.Open, FileAccess.Read))
                {
                    reader.Read(nesHeader, 0, 16);
                    if ((nesHeader[6] & 0x4) != 0x0) //Load the trainer
                    {
                        reader.Read(_memory.SRam, 0x1000, 512);
                        Debug.WriteLine(this, "Trainer loaded.", DebugStatus.None);
                    }
                    var prgRoms = nesHeader[4]*4;
                    PrgPages = nesHeader[4];
                    Debug.WriteLine(this, "PRG pages = " + PrgPages, DebugStatus.None);
                    Prg = new byte[prgRoms][];
                    for (var i = 0; i < (prgRoms); i++)
                    {
                        Prg[i] = new byte[4096];
                        reader.Read(Prg[i], 0, 4096);
                    }
                    var chrRoms = nesHeader[5]*8;
                    ChrPages = nesHeader[5];
                    Debug.WriteLine(this, "CHR pages = " + ChrPages, DebugStatus.None);
                    if (ChrPages != 0)
                    {
                        Chr = new byte[chrRoms][];
                        for (var i = 0; i < (chrRoms); i++)
                        {
                            Chr[i] = new byte[1024];
                            reader.Read(Chr[i], 0, 1024);
                        }
                        IsVram = false;
                    }
                    else
                    {
                        Chr = new byte[512][];
                        for (var i = 0; i < 512; i++)
                        {
                            Chr[i] = new byte[1024];
                        }
                        IsVram = true;
                    }
                    Mirroring = (nesHeader[6] & 0x1) == 0x0 ? Mirroring.Horizontal : Mirroring.Vertical;
                    IsSaveRam = (nesHeader[6] & 0x2) != 0x0;
                    IsTrainer = (nesHeader[6] & 0x4) != 0x0;

                    if ((nesHeader[6] & 0x8) != 0x0)
                    {
                        Mirroring = Mirroring.FourScreen;
                    }
                    Debug.WriteLine(this, "Mirroring = " + Mirroring.ToString(), DebugStatus.None);
                    if ((nesHeader[7] & 0xF) == 0)
                        MapperNo = (byte) ((nesHeader[7] & 0xF0) | ((nesHeader[6] & 0xF0) >> 4));
                    else
                        MapperNo = (byte) ((nesHeader[6] & 0xF0) >> 4);
                    Debug.WriteLine(this, "Mapper # = " + MapperNo.ToString(CultureInfo.InvariantCulture), DebugStatus.None);

                    #region Fixes

                    //smb3
                    if ((Prg[0][0x75] == 0x11) &&
                        (Prg[0][0x76] == 0x12) &&
                        (Prg[0][0x77] == 0x13) &&
                        (Prg[0][0x78] == 0x14) &&
                        (Prg[0][0x79] == 0x07) &&
                        (Prg[0][0x7a] == 0x03) &&
                        (Prg[0][0x7b] == 0x03) &&
                        (Prg[0][0x7c] == 0x03) &&
                        (Prg[0][0x7d] == 0x03)
                        )
                    {
                        _memory.Map.Engine.Ppu.FixScroll3 = true;
                    }
                    //werewolf
                    if ((Prg[0][0x76] == 15) &&
                        (Prg[0][0x77] == 8) &&
                        (Prg[0][0x78] == 24) &&
                        (Prg[0][0x79] == 40)
                        )
                    {
                        _memory.Map.Engine.Ppu.FixScroll = true;
                    }
                    /*int A = _PRG[1][0x76];
                    int A1 = _PRG[1][0x77];
                    int A2 = _PRG[1][0x78];
                    int A3 = _PRG[1][0x79];*/
                    //karnov
                    if ((Prg[1][0x76] == 183) &&
                        (Prg[1][0x77] == 247) &&
                        (Prg[1][0x78] == 253) &&
                        (Prg[1][0x79] == 254)
                        )
                    {
                        _memory.Map.Engine.Ppu.FixScroll = true;
                    }
                    //Aladdin 3
                    if ((Prg[1][0x76] == 254) &&
                        (Prg[1][0x77] == 0) &&
                        (Prg[1][0x78] == 22) &&
                        (Prg[1][0x79] == 0)
                        )
                    {
                        _memory.Map.Engine.Ppu.FixScroll = true;
                    }
                    //zelda
                    if ((Prg[0][0x76] == 255) &&
                        (Prg[0][0x77] == 255) &&
                        (Prg[0][0x78] == 255) &&
                        (Prg[0][0x79] == 255)
                        )
                    {
                        _memory.Map.Engine.Ppu.FixScroll2 = true;
                    }
                    //mappers
                    if (MapperNo == 225 | MapperNo == 255 | MapperNo == 16)
                        _memory.Map.Engine.Ppu.FixScroll = true;

                    #endregion
                }
            }
            catch
            {
                Debug.WriteLine(this, "Can't read the rom.", DebugStatus.Error);
                return false;
            }

            if (IsSaveRam)
            {
                SaveFilename = RomPath.Remove(RomPath.Length - 3, 3);
                SaveFilename = SaveFilename.Insert(SaveFilename.Length, "sav");
                Debug.WriteLine(this, "Trying to read SRAM from file : " + SaveFilename, DebugStatus.None);
                try
                {
                    using (var reader = File.OpenRead(SaveFilename))
                    {
                        reader.Read(_memory.SRam, 0, 0x2000);
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
    }
}