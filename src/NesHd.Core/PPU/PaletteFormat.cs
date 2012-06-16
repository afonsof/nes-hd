namespace NesHd.Core.PPU
{
    public class PaletteFormat
    {
        public PaletteFormat()
        {
            ExternalPalettePath = "";
            UseInternalPaletteMode = UseInternalPaletteMode.Auto;
            UseInternalPalette = true;
        }

        public bool UseInternalPalette { get; set; }
        public UseInternalPaletteMode UseInternalPaletteMode { get; set; }
        public string ExternalPalettePath { get; set; }
    }
}