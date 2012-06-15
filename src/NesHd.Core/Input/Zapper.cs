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

namespace NesHd.Core.Input
{
    public class Zapper
    {
        int Trigger = 1;
        int Detected = 1;
        NesEngine _Nes;
        public Zapper(NesEngine NES)
        { this._Nes = NES; }
        public void PullTrigger(bool Pull, int X, int Y)
        {
            this.Trigger = Pull ? 0 : 1;
            if (Pull)
                this.Detected = 1;
            this._Nes.PPU.ZapperX = X;
            this._Nes.PPU.ZapperY = Y;
            this._Nes.PPU.CheckZapperHit = true;
            this._Nes.PPU.ZapperFrame = 0;

        }
        public byte GetData()
        {
            return (byte)((this.Detected | (this.Trigger << 1)) << 3);
        }
        public void SetDetect(int V)
        { this.Detected = V; }
    }
}
