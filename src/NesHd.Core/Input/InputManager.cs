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
using System.Collections.Generic;

using SlimDX.DirectInput;

namespace NesHd.Core.Input
{
    public class InputManager
    {
        private IList<InputDevice> _devices;

        public IList<InputDevice> Devices { get { return this._devices; } }

        public InputManager(IntPtr handle)
        {
            this._devices = new List<InputDevice>();
            DirectInput di = new DirectInput();
            foreach (var device in di.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AttachedOnly))
            {
                if ((device.Type & DeviceType.Keyboard) == DeviceType.Keyboard)
                {
                    Keyboard keyboard = new Keyboard(di);
                    keyboard.SetCooperativeLevel(handle, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
                    this._devices.Add(new InputDevice(keyboard));
                }
                else if ((device.Type & DeviceType.Joystick) == DeviceType.Joystick)
                {
                    Joystick joystick = new Joystick(di, device.InstanceGuid);
                    joystick.SetCooperativeLevel(handle, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
                    this._devices.Add(new InputDevice(joystick));
                }
            }
        }
        public void Update()
        {
            for (int i = 0; i < this._devices.Count; i++)
            {
                this._devices[i].Update();
            }
        }
    }
}
