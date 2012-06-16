using SlimDX.DirectInput;

namespace NesHd.Core.Input
{
    public class InputDevice
    {
        private readonly Device _device;
        private readonly DeviceType _type;
        private JoystickState _joystickState;
        private KeyboardState _keyboardState;

        public InputDevice(Device device)
        {
            _device = device;
            if ((device.Information.Type & DeviceType.Keyboard) == DeviceType.Keyboard)
                _type = DeviceType.Keyboard;
            else if ((device.Information.Type & DeviceType.Joystick) == DeviceType.Joystick)
                _type = DeviceType.Joystick;
            else
                _type = DeviceType.Other;
        }

        public DeviceType Type
        {
            get { return _type; }
        }

        public KeyboardState KeyboardState
        {
            get { return _keyboardState; }
        }

        public JoystickState JoystickState
        {
            get { return _joystickState; }
        }

        public void Update()
        {
            if (_device.Acquire().IsFailure)
                return;

            if (_device.Poll().IsFailure)
                return;

            if (_type == DeviceType.Keyboard)
                _keyboardState = ((Keyboard) _device).GetCurrentState();
            else if (_type == DeviceType.Joystick)
                _joystickState = ((Joystick) _device).GetCurrentState();
        }
    }
}