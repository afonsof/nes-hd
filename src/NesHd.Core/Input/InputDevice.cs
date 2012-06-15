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

using SlimDX.DirectInput;

namespace NesHd.Core.Input
{
    public class InputDevice
    {
        private Device _device;
        private DeviceType _type;
        private KeyboardState _keyboardState;
        private JoystickState _joystickState;

        public DeviceType Type { get { return this._type; } }
        public KeyboardState KeyboardState { get { return this._keyboardState; } }
        public JoystickState JoystickState { get { return this._joystickState; } }

        public InputDevice(Device device)
        {
            this._device = device;
            if ((device.Information.Type & DeviceType.Keyboard) == DeviceType.Keyboard)
                this._type = DeviceType.Keyboard;
            else if ((device.Information.Type & DeviceType.Joystick) == DeviceType.Joystick)
                this._type = DeviceType.Joystick;
            else
                this._type = DeviceType.Other;
        }

        public void Update()
        {
            if (this._device.Acquire().IsFailure)
                return;

            if (this._device.Poll().IsFailure)
                return;

            if (this._type == DeviceType.Keyboard)
                this._keyboardState = ((Keyboard)this._device).GetCurrentState();
            else if (this._type == DeviceType.Joystick)
                this._joystickState = ((Joystick)this._device).GetCurrentState();
        }
    }
}
