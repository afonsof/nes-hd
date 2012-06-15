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

namespace NesHd.Core.PPU
{
    public class PaletteFormat
    {
        bool _UseInternalPalette = true;
        string _ExternalPalettePath = "";
        UseInternalPaletteMode _UseInternalPaletteMode = UseInternalPaletteMode.Auto;
        public bool UseInternalPalette
        { get { return this._UseInternalPalette; } set { this._UseInternalPalette = value; } }
        public UseInternalPaletteMode UseInternalPaletteMode
        { get { return this._UseInternalPaletteMode; } set { this._UseInternalPaletteMode = value; } }
        public string ExternalPalettePath
        { get { return this._ExternalPalettePath; } set { this._ExternalPalettePath = value; } }
    }
    public enum UseInternalPaletteMode
    { Auto, PAL, NTSC }
}
