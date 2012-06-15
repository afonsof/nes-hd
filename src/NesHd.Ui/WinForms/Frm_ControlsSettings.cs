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

using NesHd.Ui.Misc;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_ControlsSettings : Form
    {
        bool _Ok = false;
        public bool OK
        { get { return this._Ok; } set { this._Ok = value; } }
        /// <summary>
        /// Build a default control profile if the user run 
        /// My Nes for the first time
        /// </summary>
        public static void BuildControlProfile()
        {
            if (Program.Settings.ControlProfiles == null)
            {
                Program.Settings.ControlProfiles = new ControlProfilesCollection();
                Program.Settings.CurrentControlProfile = new ControlProfile();
                Program.Settings.CurrentControlProfile.Name = "<Default>";
                Program.Settings.ControlProfiles.Add(Program.Settings.CurrentControlProfile);
                Program.Settings.Save();
            }
            else if (Program.Settings.ControlProfiles.Count == 0)//Make the compiler happy
            {
                Program.Settings.ControlProfiles = new ControlProfilesCollection();
                Program.Settings.CurrentControlProfile = new ControlProfile();
                Program.Settings.CurrentControlProfile.Name = "<Default>";
                Program.Settings.ControlProfiles.Add(Program.Settings.CurrentControlProfile);
                Program.Settings.Save();
            }
        }
        void RefreshProfiles()
        {
            this.comboBox1.Items.Clear();
            foreach (ControlProfile con in Program.Settings.ControlProfiles)
            {
                this.comboBox1.Items.Add(con.Name);
            }
            this.comboBox1.SelectedItem = Program.Settings.CurrentControlProfile.Name;
        }
        public Frm_ControlsSettings()
        {
            this.InitializeComponent();
            //Load settings
            this.RefreshProfiles();
            /*Player1_A.Text = Program.Settings.Player1_A;
            Player1_B.Text = Program.Settings.Player1_B;
            Player1_Up.Text = Program.Settings.Player1_Up;
            Player1_Left.Text = Program.Settings.Player1_Left;
            Player1_Right.Text = Program.Settings.Player1_Right;
            Player1_Down.Text = Program.Settings.Player1_Down;
            Player1_Select.Text = Program.Settings.Player1_Select;
            Player1_Start.Text = Program.Settings.Player1_Start;

            Player2_A.Text = Program.Settings.Player2_A;
            Player2_B.Text = Program.Settings.Player2_B;
            Player2_Up.Text = Program.Settings.Player2_Up;
            Player2_Left.Text = Program.Settings.Player2_Left;
            Player2_Right.Text = Program.Settings.Player2_Right;
            Player2_Down.Text = Program.Settings.Player2_Down;
            Player2_Select.Text = Program.Settings.Player2_Select;
            Player2_Start.Text = Program.Settings.Player2_Start;*/
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //reload the settings
            Program.Settings.Reload();
            this._Ok = false;
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Save the settings
            Program.Settings.Save();
            this._Ok = true;
            this.Close();
        }

        private void Player1_Left_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player1 Left");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player1_Left.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player1_Left = kk.InputName;
            }
        }
        private void Player1_Right_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player1 Right");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player1_Right.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player1_Right = kk.InputName;
            }
        }
        private void Player1_Up_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player1 Up");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player1_Up.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player1_Up = kk.InputName;
            }
        }
        private void Player1_Down_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player1 Down");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player1_Down.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player1_Down = kk.InputName;
            }
        }
        private void Player1_A_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player1 A");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player1_A.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player1_A = kk.InputName;
            }
        }
        private void Player1_B_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player1 B");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player1_B.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player1_B = kk.InputName;
            }
        }
        private void Player1_Start_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player1 Start");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player1_Start.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player1_Start = kk.InputName;
            }
        }
        private void Player1_Select_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player1 Select");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player1_Select.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player1_Select = kk.InputName;
            }
        }

        private void Player2_Left_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player2 Left");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player2_Left.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player2_Left = kk.InputName;
            }
        }
        private void Player2_Right_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player2 Right");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player2_Right.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player2_Right = kk.InputName;
            }
        }
        private void Player2_Up_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player2 Up");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player2_Up.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player2_Up = kk.InputName;
            }
        }
        private void Player2_Down_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player2 Down");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player2_Down.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player2_Down = kk.InputName;
            }
        }
        private void Player2_A_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player2 A");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player2_A.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player2_A = kk.InputName;
            }
        }
        private void Player2_B_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player2 B");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player2_B.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player2_B = kk.InputName;
            }
        }
        private void Player2_Start_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player2 Start");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player2_Start.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player2_Start = kk.InputName;
            }
        }
        private void Player2_Select_Click(object sender, EventArgs e)
        {
            Frm_Key kk = new Frm_Key("Player2 Select");
            kk.ShowDialog();
            if (kk.OK)
            {
                this.Player2_Select.Text = kk.InputName;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Player2_Select = kk.InputName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Player1_A.Text = "Keyboard.X";
            this.Player1_B.Text = "Keyboard.Z";
            this.Player1_Up.Text = "Keyboard.UpArrow";
            this.Player1_Left.Text = "Keyboard.LeftArrow";
            this.Player1_Right.Text = "Keyboard.RightArrow";
            this.Player1_Down.Text = "Keyboard.DownArrow";
            this.Player1_Select.Text = "Keyboard.C";
            this.Player1_Start.Text = "Keyboard.V";

            this.Player2_A.Text = "Keyboard.K";
            this.Player2_B.Text = "Keyboard.J";
            this.Player2_Up.Text = "Keyboard.W";
            this.Player2_Left.Text = "Keyboard.A";
            this.Player2_Right.Text = "Keyboard.D";
            this.Player2_Down.Text = "Keyboard.S";
            this.Player2_Select.Text = "Keyboard.Q";
            this.Player2_Start.Text = "Keyboard.E";
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Set profile
            Program.Settings.CurrentControlProfile = Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex];
            //Load buttons
            this.Player1_A.Text = Program.Settings.CurrentControlProfile.Player1_A;
            this.Player1_B.Text = Program.Settings.CurrentControlProfile.Player1_B;
            this.Player1_Up.Text = Program.Settings.CurrentControlProfile.Player1_Up;
            this.Player1_Left.Text = Program.Settings.CurrentControlProfile.Player1_Left;
            this.Player1_Right.Text = Program.Settings.CurrentControlProfile.Player1_Right;
            this.Player1_Down.Text = Program.Settings.CurrentControlProfile.Player1_Down;
            this.Player1_Select.Text = Program.Settings.CurrentControlProfile.Player1_Select;
            this.Player1_Start.Text = Program.Settings.CurrentControlProfile.Player1_Start;

            this.Player2_A.Text = Program.Settings.CurrentControlProfile.Player2_A;
            this.Player2_B.Text = Program.Settings.CurrentControlProfile.Player2_B;
            this.Player2_Up.Text = Program.Settings.CurrentControlProfile.Player2_Up;
            this.Player2_Left.Text = Program.Settings.CurrentControlProfile.Player2_Left;
            this.Player2_Right.Text = Program.Settings.CurrentControlProfile.Player2_Right;
            this.Player2_Down.Text = Program.Settings.CurrentControlProfile.Player2_Down;
            this.Player2_Select.Text = Program.Settings.CurrentControlProfile.Player2_Select;
            this.Player2_Start.Text = Program.Settings.CurrentControlProfile.Player2_Start;
            //Check enablation
            if (this.comboBox1.SelectedItem.ToString() == "<Default>")
            {
                this.linkLabel3.Enabled = false;
                this.linkLabel2.Enabled = false;

                this.Player1_B.Enabled = false;
                this.Player1_A.Enabled = false;
                this.Player1_Left.Enabled = false;
                this.Player1_Right.Enabled = false;
                this.Player1_Up.Enabled = false;
                this.Player1_Down.Enabled = false;
                this.Player1_Select.Enabled = false;
                this.Player1_Start.Enabled = false;

                this.Player2_B.Enabled = false;
                this.Player2_A.Enabled = false;
                this.Player2_Left.Enabled = false;
                this.Player2_Right.Enabled = false;
                this.Player2_Up.Enabled = false;
                this.Player2_Down.Enabled = false;
                this.Player2_Select.Enabled = false;
                this.Player2_Start.Enabled = false;
            }
            else
            {
                this.linkLabel3.Enabled = true;
                this.linkLabel2.Enabled = true;

                this.Player1_B.Enabled = true;
                this.Player1_A.Enabled = true;
                this.Player1_Left.Enabled = true;
                this.Player1_Right.Enabled = true;
                this.Player1_Up.Enabled = true;
                this.Player1_Down.Enabled = true;
                this.Player1_Select.Enabled = true;
                this.Player1_Start.Enabled = true;

                this.Player2_B.Enabled = true;
                this.Player2_A.Enabled = true;
                this.Player2_Left.Enabled = true;
                this.Player2_Right.Enabled = true;
                this.Player2_Up.Enabled = true;
                this.Player2_Down.Enabled = true;
                this.Player2_Select.Enabled = true;
                this.Player2_Start.Enabled = true;
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Frm_EnterName nam = new Frm_EnterName();
            nam.ShowDialog(this);
            if (nam.OK)
            {
                ControlProfile pro = new ControlProfile();
                pro.Name = nam.NameEntered;
                Program.Settings.ControlProfiles.Add(pro);
                this.RefreshProfiles();
                this.comboBox1.SelectedIndex = this.comboBox1.Items.Count - 1;
            }
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("Are you sure ?", "Remove profile", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Program.Settings.ControlProfiles.RemoveAt(this.comboBox1.SelectedIndex);
                this.RefreshProfiles();
                this.comboBox1.SelectedIndex = 0;
            }
        }
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Frm_EnterName nam = new Frm_EnterName();
            nam.ShowDialog(this);
            if (nam.OK)
            {
                int SelectedIndex = this.comboBox1.SelectedIndex;
                Program.Settings.ControlProfiles[this.comboBox1.SelectedIndex].Name = nam.NameEntered;
                this.RefreshProfiles();
                this.comboBox1.SelectedIndex = SelectedIndex;
            }
        }
    }
}
