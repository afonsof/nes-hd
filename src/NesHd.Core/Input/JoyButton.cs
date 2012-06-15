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

using SlimDX.DirectInput;

namespace NesHd.Core.Input
{
    public class JoyButton
    {
        private InputManager _manager;
        private int _device;
        private int _code;
        private string _input;

        public string Input
        {
            get
            {
                return this._input;
            }
            set
            {
                this._input = value;
                if (this._input.StartsWith("Keyboard"))
                {
                    string keyName = this._input.Substring(9, this._input.Length - 9);
                    this._device = 0;
                    this._code = (int)Enum.Parse(typeof(Key), keyName);
                }
                else if (this._input.StartsWith("Joystick"))
                {
                    string buttonNumber = this._input.Substring(10, this._input.Length - 10);
                    this._device = Convert.ToInt32(this._input.Substring(8, 1));

                    if (buttonNumber == "X+")
                        this._code = -1;
                    else if (buttonNumber == "X-")
                        this._code = -2;
                    else if (buttonNumber == "Y+")
                        this._code = -3;
                    else if (buttonNumber == "Y-")
                        this._code = -4;
                    else
                        this._code = Convert.ToInt32(buttonNumber);
                }
            }
        }

        public JoyButton(InputManager manager)
        {
            this._manager = manager;
        }

        public bool IsPressed()
        {
            if (this._device >= this._manager.Devices.Count)
            { return false; }

            InputDevice device = this._manager.Devices[this._device];

            if (device.Type == DeviceType.Keyboard)
            {
                if (device.KeyboardState != null)
                    return device.KeyboardState.IsPressed((Key)this._code);
            }
            else if (device.Type == DeviceType.Joystick)
            {
                if (this._code < 0)
                {
                    if (this._code == -1)
                        return device.JoystickState.X > 0xC000;
                    else if (this._code == -2)
                        return device.JoystickState.X < 0x4000;
                    else if (this._code == -3)
                        return device.JoystickState.Y > 0xC000;
                    else if (this._code == -4)
                        return device.JoystickState.Y < 0x4000;
                }
                else
                {
                    return device.JoystickState.IsPressed(this._code);
                }
            }

            return false;
        }
    }
}
