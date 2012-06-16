using System;
using System.Windows.Forms;
using NesHd.Core.Input;
using SlimDX.DirectInput;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_Key : Form
    {
        private readonly InputManager _manager;
        private bool _Ok;
        private string _inputName;

        public Frm_Key(string ButtonName)
        {
            InitializeComponent();
            label1.Text = "Press a keyboard / Joystick key for " + ButtonName + " button ...";
            _manager = new InputManager(Handle);
            timer1.Interval = 1000/30;
            timer1.Enabled = true;
            Select();
        }

        public bool OK
        {
            get { return _Ok; }
        }

        public string InputName
        {
            get { return _inputName; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _manager.Update();

            for (var i = 0; i < _manager.Devices.Count; i++)
            {
                var device = _manager.Devices[i];

                var pressed = false;

                if (device.Type == DeviceType.Keyboard)
                {
                    pressed = CheckInput(device.KeyboardState, i);
                }
                else if (device.Type == DeviceType.Joystick)
                {
                    pressed = CheckInput(device.JoystickState, i);
                }

                if (pressed)
                {
                    _Ok = true;
                    timer1.Enabled = false;
                    Close();
                    return;
                }
            }
        }

        private bool CheckInput(KeyboardState state, int index)
        {
            if (state == null)
                return false;

            if (state.PressedKeys.Count > 0)
            {
                _inputName = "Keyboard." + state.PressedKeys[0].ToString();
                return true;
            }
            return false;
        }

        private bool CheckInput(JoystickState state, int index)
        {
            if (state == null)
                return false;

            var buttons = state.GetButtons();
            for (var button = 0; button < buttons.Length; button++)
            {
                if (buttons[button])
                {
                    _inputName = "Joystick" + index + "." + button;
                    return true;
                }

                if (state.X > 0xC000)
                {
                    _inputName = "Joystick" + index + ".X+";
                    return true;
                }
                else if (state.X < 0x4000)
                {
                    _inputName = "Joystick" + index + ".X-";
                    return true;
                }
                else if (state.Y > 0xC000)
                {
                    _inputName = "Joystick" + index + ".Y+";
                    return true;
                }
                else if (state.Y < 0x4000)
                {
                    _inputName = "Joystick" + index + ".Y-";
                    return true;
                }
            }
            return false;
        }
    }
}