namespace NesHd.Core.Input
{
    public class Joypad
    {
        private readonly JoyButton[] _buttons;

        public Joypad(InputManager manager)
        {
            _buttons = new JoyButton[8];
            for (var i = 0; i < 8; i++)
            {
                _buttons[i] = new JoyButton(manager);
            }
        }

        public JoyButton Up
        {
            get { return _buttons[0]; }
        }

        public JoyButton Down
        {
            get { return _buttons[1]; }
        }

        public JoyButton Left
        {
            get { return _buttons[2]; }
        }

        public JoyButton Right
        {
            get { return _buttons[3]; }
        }

        public JoyButton Select
        {
            get { return _buttons[4]; }
        }

        public JoyButton Start
        {
            get { return _buttons[5]; }
        }

        public JoyButton A
        {
            get { return _buttons[6]; }
        }

        public JoyButton B
        {
            get { return _buttons[7]; }
        }

        // Methods
        public int GetJoyData()
        {
            var num = 0;

            if (A.IsPressed())
                num |= 1;

            if (B.IsPressed())
                num |= 2;

            if (Select.IsPressed())
                num |= 4;

            if (Start.IsPressed())
                num |= 8;

            if (Up.IsPressed())
                num |= 0x10;

            if (Down.IsPressed())
                num |= 0x20;

            if (Left.IsPressed())
                num |= 0x40;

            if (Right.IsPressed())
                num |= 0x80;

            return num;
        }
    }
}