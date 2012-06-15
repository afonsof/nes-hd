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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using NesHd.Core.Debugger;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_Debugger : Form
    {
        public Frm_Debugger()
        {
            this.InitializeComponent();
            this.Location = new Point(Program.Settings.DebuggerWin_X, Program.Settings.DebuggerWin_Y);
            this.Size = new Size(Program.Settings.DebuggerWin_W, Program.Settings.DebuggerWin_H);
            Debug.DebugRised += new EventHandler<DebugArg>(this.DEBUG_DebugRised);
            this.ClearDebugger();
        }
        void DEBUG_DebugRised(object sender, DebugArg e)
        {
            this.WriteLine(e.DebugLine, e.Status);
        }
        public void SaveSettings()
        {
            Program.Settings.DebuggerWin_X = this.Location.X;
            Program.Settings.DebuggerWin_Y = this.Location.Y;
            Program.Settings.DebuggerWin_W = this.Width;
            Program.Settings.DebuggerWin_H = this.Height;
            Program.Settings.Save();
        }
        private void Frm_Debugger_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.SaveSettings();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void WriteLine(string Text, DebugStatus State)
        {
            try
            {
                this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
                switch (State)
                {
                    case DebugStatus.None:
                        this.richTextBox1.SelectionColor = Color.Black;
                        break;
                    case DebugStatus.Cool:
                        this.richTextBox1.SelectionColor = Color.Green;
                        break;
                    case DebugStatus.Error:
                        this.richTextBox1.SelectionColor = Color.Red;
                        break;
                    case DebugStatus.Warning:
                        this.richTextBox1.SelectionColor = Color.Yellow;
                        break;
                }
                this.richTextBox1.SelectedText = Text + "\n";
                this.richTextBox1.SelectionColor = Color.Green;
                this.richTextBox1.ScrollToCaret();
            }
            catch { }
        }
        public void WriteLine(string Text)
        { this.WriteLine(Text, DebugStatus.None); }
        public void ClearDebugger()
        {
            this.richTextBox1.Text = "";
            this.WriteLine("Welcome to My Nes console");
            this.WriteLine(@"Write ""Help"" to see a list of instructions.");
            Debug.WriteSeparateLine(this, DebugStatus.None);
        }
        void DoInstruction(string Instruction)
        {
            this.WriteLine(Instruction);
            switch (Instruction.ToLower())
            {
                case "cl d": this.ClearDebugger(); break;
                case "exit": this.Close(); break;
                case "help": this.Help(); break;
                case "rom":
                    if (Program.Form_Main.NES != null)
                    {
                        this.WriteLine("Rom info:", DebugStatus.None);
                        string st =
                        "PRG count: " + Program.Form_Main.NES.MEMORY.MAP.Cartridge.PRG_PAGES.ToString() + "\n" +
                        "CHR count: " + Program.Form_Main.NES.MEMORY.MAP.Cartridge.CHR_PAGES.ToString() + "\n" +
                        "Mapper: #" + Program.Form_Main.NES.MEMORY.MAP.Cartridge.MapperNo.ToString() + "\n";
                        this.WriteLine(st, DebugStatus.Cool);
                    }
                    else
                    {
                        this.WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                case "r":
                    this.WriteLine("Cpu registers:", DebugStatus.None);
                    if (Program.Form_Main.NES != null)
                    {
                        string st =
                            "PC: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_PC) + "\n"
                            + "A: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_A) + "\n"
                            + "X: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_X) + "\n"
                            + "Y: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_Y) + "\n"
                            + "S: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.REG_S) + "\n"
                            + "P: $" + string.Format("{0:X}", Program.Form_Main.NES.CPU.StatusRegister());
                        this.WriteLine(st, DebugStatus.Cool);
                        this.WriteLine("Flags", DebugStatus.Cool);
                        st =
                            "B : " + (Program.Form_Main.NES.CPU.Flag_B ? "1" : "0") + "\n" +
                            "C : " + (Program.Form_Main.NES.CPU.Flag_C ? "1" : "0") + "\n" +
                            "D : " + (Program.Form_Main.NES.CPU.Flag_D ? "1" : "0") + "\n" +
                            "I : " + (Program.Form_Main.NES.CPU.Flag_I ? "1" : "0") + "\n" +
                            "N : " + (Program.Form_Main.NES.CPU.Flag_N ? "1" : "0") + "\n" +
                            "V : " + (Program.Form_Main.NES.CPU.Flag_V ? "1" : "0") + "\n" +
                            "Z : " + (Program.Form_Main.NES.CPU.Flag_Z ? "1" : "0");
                        this.WriteLine(st, DebugStatus.Cool);
                    }
                    else
                    {
                        this.WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                case "cl sram":
                    if (Program.Form_Main.NES != null)
                    {
                        this.WriteLine("Clearing the S-RAM ....", DebugStatus.Warning);
                        for (int i = 0; i < Program.Form_Main.NES.MEMORY.SRAM.Length; i++)
                            Program.Form_Main.NES.MEMORY.SRAM[i] = 0;
                        this.WriteLine("Done.", DebugStatus.Cool);
                    }
                    else
                    {
                        this.WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                default://Memory instructions handled here
                    try
                    {
                        if (Instruction == "")
                        { this.WriteLine("Enter something !!", DebugStatus.Warning); break; }
                        else if (Instruction.ToLower().Substring(0, 2) == "w ")
                        {
                            if (Program.Form_Main.NES == null)
                                this.WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                ushort ADDRESS = Convert.ToUInt16(Instruction.ToLower().Substring(3, 4), 16);
                                byte VALUE = Convert.ToByte(Instruction.ToLower().Substring(9, 2), 16);
                                Program.Form_Main.NES.MEMORY.Write(ADDRESS, VALUE);
                                this.WriteLine("Done.", DebugStatus.Cool);
                            }
                            catch { this.WriteLine("Bad address or value.", DebugStatus.Error); }
                        }
                        else if (Instruction.ToLower().Substring(0, 2) == "m ")
                        {
                            if (Program.Form_Main.NES == null)
                                this.WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                ushort ADDRESS = Convert.ToUInt16(Instruction.ToLower().Substring(3, 4), 16);
                                byte VALUE = Program.Form_Main.NES.MEMORY.Read(ADDRESS);
                                this.WriteLine("Done, the value = $" + string.Format("{0:X}", VALUE), DebugStatus.Cool);
                            }
                            catch { this.WriteLine("Bad address.", DebugStatus.Error); }
                        }
                        else if (Instruction.ToLower().Substring(0, 3) == "mr ")
                        {
                            if (Program.Form_Main.NES == null)
                                this.WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                ushort ADDRESS = Convert.ToUInt16(Instruction.ToLower().Substring(4, 4), 16);
                                int countt = Convert.ToInt32(Instruction.ToLower().Substring(9, Instruction.ToLower().Length - 9));
                                for (int i = 0; i < countt; i++)
                                {
                                    byte VALUE = Program.Form_Main.NES.MEMORY.Read((ushort)(ADDRESS + i));
                                    this.WriteLine("$" + string.Format("{0:X}", (ADDRESS + i)) + " : $" + string.Format("{0:X}", VALUE), DebugStatus.Cool);
                                }
                            }
                            catch { this.WriteLine("Bad address or range out of memory.", DebugStatus.Error); }
                        }
                        else
                            this.WriteLine("Bad command ...", DebugStatus.Warning); break;
                    }
                    catch
                    { this.WriteLine("Bad command ...", DebugStatus.Warning); break; }
            }
        }
        void Help()
        {
            this.WriteLine("Instructions list :", DebugStatus.Cool);
            this.WriteLine("  > exit : Close the console");
            this.WriteLine("  > help : Display this list !!");
            this.WriteLine("  > rom : Show the current rom info.");
            this.WriteLine("  > R : Dumps the cpu registers");
            this.WriteLine("  > W <address> <value> : Set a value into the memory at the specific address.");
            this.WriteLine("  The address and the value must be in hex starting with $");
            this.WriteLine("  e.g : w $1F23 $10");
            this.WriteLine("  > M <address> : Dumps the memory at the specific address.");
            this.WriteLine("  > MR <StartAddress> <Count> : Dumps the specified memory range.");
            this.WriteLine("  The address must be in hex starting with $, count must be dec");
            this.WriteLine("  e.g : mr $1F23 1000");
            this.WriteLine("  > CL D : Clear debugger");
            this.WriteLine("  > CL SRAM : Clear S-RAM");
            this.WriteLine("Instructions are NOT case sensitive.");
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (this.richTextBox1.Lines.Length > 0)
                    this.richTextBox1.SelectedText = this.richTextBox1.Lines[this.richTextBox1.Lines.Length - 1];
                this.DoInstruction(this.textBox1.Text);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.richTextBox1.Lines.Length > 0)
                this.richTextBox1.SelectedText = this.richTextBox1.Lines[this.richTextBox1.Lines.Length - 1];
            this.DoInstruction(this.textBox1.Text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.ClearDebugger();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog();
            sav.Title = "Save console lines to file";
            sav.Filter = "RTF DOCUMENT|*.rtf|TEXT FILE|*.txt|DOC DOCUMENT|*.doc";
            if (sav.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (Path.GetExtension(sav.FileName).ToLower() == ".txt")
                {
                    File.WriteAllLines(sav.FileName, this.richTextBox1.Lines);
                }
                else
                {
                    this.richTextBox1.SaveFile(sav.FileName, RichTextBoxStreamType.RichText);
                }
            }
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.textBox1.SelectAll();
        }
    }
}
