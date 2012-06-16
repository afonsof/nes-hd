using System;
using System.Collections.Generic;
using SlimDX.DirectInput;

namespace NesHd.Core.Input
{
    public class InputManager
    {
        private readonly IList<InputDevice> _devices;

        public InputManager(IntPtr handle)
        {
            _devices = new List<InputDevice>();
            var di = new DirectInput();
            foreach (var device in di.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AttachedOnly))
            {
                if ((device.Type & DeviceType.Keyboard) == DeviceType.Keyboard)
                {
                    var keyboard = new Keyboard(di);
                    keyboard.SetCooperativeLevel(handle, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
                    _devices.Add(new InputDevice(keyboard));
                }
                else if ((device.Type & DeviceType.Joystick) == DeviceType.Joystick)
                {
                    var joystick = new Joystick(di, device.InstanceGuid);
                    joystick.SetCooperativeLevel(handle, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
                    _devices.Add(new InputDevice(joystick));
                }
            }
        }

        public IList<InputDevice> Devices
        {
            get { return _devices; }
        }

        public void Update()
        {
            for (var i = 0; i < _devices.Count; i++)
            {
                _devices[i].Update();
            }
        }
    }
}