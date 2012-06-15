using System.Collections.Generic;

namespace NesHd.Ui.Misc
{
    public class ControlProfile
    {
        public string Name = "";
        public string Player1_A = "Keyboard.X";
        public string Player1_B = "Keyboard.Z";
        public string Player1_Left = "Keyboard.LeftArrow";
        public string Player1_Right = "Keyboard.RightArrow";
        public string Player1_Up = "Keyboard.UpArrow";
        public string Player1_Down = "Keyboard.DownArrow";
        public string Player1_Start = "Keyboard.V";
        public string Player1_Select = "Keyboard.C";

        public string Player2_A = "Keyboard.K";
        public string Player2_B = "Keyboard.J";
        public string Player2_Left = "Keyboard.A";
        public string Player2_Right = "Keyboard.D";
        public string Player2_Up = "Keyboard.W";
        public string Player2_Down = "Keyboard.S";
        public string Player2_Start = "Keyboard.E";
        public string Player2_Select = "Keyboard.Q";
    }
    public class ControlProfilesCollection : List<ControlProfile>//For settings
    {

    }
}
