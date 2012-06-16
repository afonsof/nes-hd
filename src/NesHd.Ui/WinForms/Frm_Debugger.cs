using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using NesHd.Core.Debugger;

namespace NesHd.Ui.WinForms
{
    public partial class DebuggerForm : Form
    {
        public DebuggerForm()
        {
            InitializeComponent();
            Location = new Point(Program.Settings.DebuggerWin_X, Program.Settings.DebuggerWin_Y);
            Size = new Size(Program.Settings.DebuggerWin_W, Program.Settings.DebuggerWin_H);
            Debug.DebugRised += DEBUG_DebugRised;
            ClearDebugger();
        }

        private void DEBUG_DebugRised(object sender, DebugArg e)
        {
            WriteLine(e.DebugLine, e.Status);
        }

        public void SaveSettings()
        {
            Program.Settings.DebuggerWin_X = Location.X;
            Program.Settings.DebuggerWin_Y = Location.Y;
            Program.Settings.DebuggerWin_W = Width;
            Program.Settings.DebuggerWin_H = Height;
            Program.Settings.Save();
        }

        private void Frm_Debugger_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void WriteLine(string Text, DebugStatus State)
        {
            try
            {
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                switch (State)
                {
                    case DebugStatus.None:
                        richTextBox1.SelectionColor = Color.Black;
                        break;
                    case DebugStatus.Cool:
                        richTextBox1.SelectionColor = Color.Green;
                        break;
                    case DebugStatus.Error:
                        richTextBox1.SelectionColor = Color.Red;
                        break;
                    case DebugStatus.Warning:
                        richTextBox1.SelectionColor = Color.Yellow;
                        break;
                }
                richTextBox1.SelectedText = Text + "\n";
                richTextBox1.SelectionColor = Color.Green;
                richTextBox1.ScrollToCaret();
            }
            catch
            {
            }
        }

        public void WriteLine(string Text)
        {
            WriteLine(Text, DebugStatus.None);
        }

        public void ClearDebugger()
        {
            richTextBox1.Text = "";
            WriteLine("Welcome to My Nes console");
            WriteLine(@"Write ""Help"" to see a list of instructions.");
            Debug.WriteSeparateLine(this, DebugStatus.None);
        }

        private void DoInstruction(string Instruction)
        {
            WriteLine(Instruction);
            switch (Instruction.ToLower())
            {
                case "cl d":
                    ClearDebugger();
                    break;
                case "exit":
                    Close();
                    break;
                case "help":
                    Help();
                    break;
                case "rom":
                    if (Program.MainForm.Engine != null)
                    {
                        WriteLine("Rom info:", DebugStatus.None);
                        var st =
                            "PRG count: " + Program.MainForm.Engine.Memory.Map.Cartridge.PrgPages.ToString(CultureInfo.InvariantCulture) + "\n" +
                            "CHR count: " + Program.MainForm.Engine.Memory.Map.Cartridge.ChrPages.ToString(CultureInfo.InvariantCulture) + "\n" +
                            "Mapper: #" + Program.MainForm.Engine.Memory.Map.Cartridge.MapperNo.ToString(CultureInfo.InvariantCulture) + "\n";
                        WriteLine(st, DebugStatus.Cool);
                    }
                    else
                    {
                        WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                case "r":
                    WriteLine("Cpu registers:", DebugStatus.None);
                    if (Program.MainForm.Engine != null)
                    {
                        var st =
                            "PC: $" + string.Format("{0:X}", Program.MainForm.Engine.Cpu.REG_PC) + "\n"
                            + "A: $" + string.Format("{0:X}", Program.MainForm.Engine.Cpu.REG_A) + "\n"
                            + "X: $" + string.Format("{0:X}", Program.MainForm.Engine.Cpu.REG_X) + "\n"
                            + "Y: $" + string.Format("{0:X}", Program.MainForm.Engine.Cpu.REG_Y) + "\n"
                            + "S: $" + string.Format("{0:X}", Program.MainForm.Engine.Cpu.REG_S) + "\n"
                            + "P: $" + string.Format("{0:X}", Program.MainForm.Engine.Cpu.StatusRegister());
                        WriteLine(st, DebugStatus.Cool);
                        WriteLine("Flags", DebugStatus.Cool);
                        st =
                            "B : " + (Program.MainForm.Engine.Cpu.Flag_B ? "1" : "0") + "\n" +
                            "C : " + (Program.MainForm.Engine.Cpu.Flag_C ? "1" : "0") + "\n" +
                            "D : " + (Program.MainForm.Engine.Cpu.Flag_D ? "1" : "0") + "\n" +
                            "I : " + (Program.MainForm.Engine.Cpu.Flag_I ? "1" : "0") + "\n" +
                            "N : " + (Program.MainForm.Engine.Cpu.Flag_N ? "1" : "0") + "\n" +
                            "V : " + (Program.MainForm.Engine.Cpu.Flag_V ? "1" : "0") + "\n" +
                            "Z : " + (Program.MainForm.Engine.Cpu.Flag_Z ? "1" : "0");
                        WriteLine(st, DebugStatus.Cool);
                    }
                    else
                    {
                        WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                case "cl sram":
                    if (Program.MainForm.Engine != null)
                    {
                        WriteLine("Clearing the S-RAM ....", DebugStatus.Warning);
                        for (var i = 0; i < Program.MainForm.Engine.Memory.SRam.Length; i++)
                            Program.MainForm.Engine.Memory.SRam[i] = 0;
                        WriteLine("Done.", DebugStatus.Cool);
                    }
                    else
                    {
                        WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                    }
                    break;
                default: //Memory instructions handled here
                    try
                    {
                        if (Instruction == "")
                        {
                            WriteLine("Enter something !!", DebugStatus.Warning);
                            break;
                        }
                        if (Instruction.ToLower().Substring(0, 2) == "w ")
                        {
                            if (Program.MainForm.Engine == null)
                                WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                var address = Convert.ToUInt16(Instruction.ToLower().Substring(3, 4), 16);
                                var value = Convert.ToByte(Instruction.ToLower().Substring(9, 2), 16);
                                Program.MainForm.Engine.Memory.Write(address, value);
                                WriteLine("Done.", DebugStatus.Cool);
                            }
                            catch
                            {
                                WriteLine("Bad address or value.", DebugStatus.Error);
                            }
                        }
                        else if (Instruction.ToLower().Substring(0, 2) == "m ")
                        {
                            if (Program.MainForm.Engine == null)
                                WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                var ADDRESS = Convert.ToUInt16(Instruction.ToLower().Substring(3, 4), 16);
                                var VALUE = Program.MainForm.Engine.Memory.Read(ADDRESS);
                                WriteLine("Done, the value = $" + string.Format("{0:X}", VALUE), DebugStatus.Cool);
                            }
                            catch
                            {
                                WriteLine("Bad address.", DebugStatus.Error);
                            }
                        }
                        else if (Instruction.ToLower().Substring(0, 3) == "mr ")
                        {
                            if (Program.MainForm.Engine == null)
                                WriteLine("SYSTEM IS OFF", DebugStatus.Error);
                            try
                            {
                                var ADDRESS = Convert.ToUInt16(Instruction.ToLower().Substring(4, 4), 16);
                                var countt =
                                    Convert.ToInt32(Instruction.ToLower().Substring(9, Instruction.ToLower().Length - 9));
                                for (var i = 0; i < countt; i++)
                                {
                                    var VALUE = Program.MainForm.Engine.Memory.Read((ushort) (ADDRESS + i));
                                    WriteLine(
                                        "$" + string.Format("{0:X}", (ADDRESS + i)) + " : $" +
                                        string.Format("{0:X}", VALUE), DebugStatus.Cool);
                                }
                            }
                            catch
                            {
                                WriteLine("Bad address or range out of memory.", DebugStatus.Error);
                            }
                        }
                        else
                            WriteLine("Bad command ...", DebugStatus.Warning);
                        break;
                    }
                    catch
                    {
                        WriteLine("Bad command ...", DebugStatus.Warning);
                        break;
                    }
            }
        }

        private void Help()
        {
            WriteLine("Instructions list :", DebugStatus.Cool);
            WriteLine("  > exit : Close the console");
            WriteLine("  > help : Display this list !!");
            WriteLine("  > rom : Show the current rom info.");
            WriteLine("  > R : Dumps the cpu registers");
            WriteLine("  > W <address> <value> : Set a value into the memory at the specific address.");
            WriteLine("  The address and the value must be in hex starting with $");
            WriteLine("  e.g : w $1F23 $10");
            WriteLine("  > M <address> : Dumps the memory at the specific address.");
            WriteLine("  > MR <StartAddress> <Count> : Dumps the specified memory range.");
            WriteLine("  The address must be in hex starting with $, count must be dec");
            WriteLine("  e.g : mr $1F23 1000");
            WriteLine("  > CL D : Clear debugger");
            WriteLine("  > CL SRAM : Clear S-RAM");
            WriteLine("Instructions are NOT case sensitive.");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (richTextBox1.Lines.Length > 0)
                    richTextBox1.SelectedText = richTextBox1.Lines[richTextBox1.Lines.Length - 1];
                DoInstruction(textBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Lines.Length > 0)
                richTextBox1.SelectedText = richTextBox1.Lines[richTextBox1.Lines.Length - 1];
            DoInstruction(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearDebugger();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var sav = new SaveFileDialog();
            sav.Title = "Save console lines to file";
            sav.Filter = "RTF DOCUMENT|*.rtf|TEXT FILE|*.txt|DOC DOCUMENT|*.doc";
            if (sav.ShowDialog(this) == DialogResult.OK)
            {
                if (Path.GetExtension(sav.FileName).ToLower() == ".txt")
                {
                    File.WriteAllLines(sav.FileName, richTextBox1.Lines);
                }
                else
                {
                    richTextBox1.SaveFile(sav.FileName, RichTextBoxStreamType.RichText);
                }
            }
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.SelectAll();
        }
    }
}