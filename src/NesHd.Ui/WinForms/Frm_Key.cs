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
using System.Windows.Forms;

using NesHd.Core.Input;

using SlimDX.DirectInput;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_Key : Form
    {
        private bool _Ok = false;
        private string _inputName;
        private InputManager _manager;

        public bool OK { get { return this._Ok; } }
        public string InputName { get { return this._inputName; } }

        public Frm_Key(string ButtonName)
        {
            this.InitializeComponent();
            this.label1.Text = "Press a keyboard / Joystick key for " + ButtonName + " button ...";
            this._manager = new InputManager(this.Handle);
            this.timer1.Interval = 1000 / 30;
            this.timer1.Enabled = true;
            this.Select();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this._manager.Update();

            for (int i = 0; i < this._manager.Devices.Count; i++)
            {
                InputDevice device = this._manager.Devices[i];

                bool pressed = false;

                if (device.Type == DeviceType.Keyboard)
                {
                    pressed = this.CheckInput(device.KeyboardState, i);
                }
                else if (device.Type == DeviceType.Joystick)
                {
                    pressed = this.CheckInput(device.JoystickState, i);
                }

                if (pressed)
                {
                    this._Ok = true;
                    this.timer1.Enabled = false;
                    this.Close();
                    return;
                }
            }
        }
        bool CheckInput(KeyboardState state, int index)
        {
            if (state == null)
                return false;

            if (state.PressedKeys.Count > 0)
            {
                this._inputName = "Keyboard." + state.PressedKeys[0].ToString();
                return true;
            }
            return false;
        }
        private bool CheckInput(JoystickState state, int index)
        {
            if (state == null)
                return false;

            bool[] buttons = state.GetButtons();
            for (int button = 0; button < buttons.Length; button++)
            {
                if (buttons[button])
                {
                    this._inputName = "Joystick" + index + "." + button;
                    return true;
                }

                if (state.X > 0xC000)
                {
                    this._inputName = "Joystick" + index + ".X+";
                    return true;
                }
                else if (state.X < 0x4000)
                {
                    this._inputName = "Joystick" + index + ".X-";
                    return true;
                }
                else if (state.Y > 0xC000)
                {
                    this._inputName = "Joystick" + index + ".Y+";
                    return true;
                }
                else if (state.Y < 0x4000)
                {
                    this._inputName = "Joystick" + index + ".Y-";
                    return true;
                }
            }
            return false;
        }
    }
}
