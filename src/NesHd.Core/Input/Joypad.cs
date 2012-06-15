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
    public class Joypad
    {
        private readonly JoyButton[] _buttons;

        public JoyButton Up { get { return this._buttons[0]; } }
        public JoyButton Down { get { return this._buttons[1]; } }
        public JoyButton Left { get { return this._buttons[2]; } }
        public JoyButton Right { get { return this._buttons[3]; } }
        public JoyButton Select { get { return this._buttons[4]; } }
        public JoyButton Start { get { return this._buttons[5]; } }
        public JoyButton A { get { return this._buttons[6]; } }
        public JoyButton B { get { return this._buttons[7]; } }

        public Joypad(InputManager manager)
        {
            this._buttons = new JoyButton[8];
            for (int i = 0; i < 8; i++)
            {
                this._buttons[i] = new JoyButton(manager);
            }
        }

        // Methods
        public int GetJoyData()
        {
            int num = 0;

            if (this.A.IsPressed())
                num |= 1;

            if (this.B.IsPressed())
                num |= 2;

            if (this.Select.IsPressed())
                num |= 4;

            if (this.Start.IsPressed())
                num |= 8;

            if (this.Up.IsPressed())
                num |= 0x10;

            if (this.Down.IsPressed())
                num |= 0x20;

            if (this.Left.IsPressed())
                num |= 0x40;

            if (this.Right.IsPressed())
                num |= 0x80;

            return num;
        }
    }
}
