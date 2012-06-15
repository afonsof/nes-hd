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
namespace NesHd.Ui.WinForms
{
    partial class Frm_ControlsSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Player1_Down = new System.Windows.Forms.Button();
            this.Player1_Right = new System.Windows.Forms.Button();
            this.Player1_Select = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.Player1_Left = new System.Windows.Forms.Button();
            this.Player1_Up = new System.Windows.Forms.Button();
            this.Player1_Start = new System.Windows.Forms.Button();
            this.Player1_B = new System.Windows.Forms.Button();
            this.Player1_A = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.Player2_Select = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Player2_Down = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Player2_Right = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Player2_Left = new System.Windows.Forms.Button();
            this.Player2_Up = new System.Windows.Forms.Button();
            this.Player2_Start = new System.Windows.Forms.Button();
            this.Player2_B = new System.Windows.Forms.Button();
            this.Player2_A = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 64);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(378, 290);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabPage1.Controls.Add(this.Player1_Down);
            this.tabPage1.Controls.Add(this.Player1_Right);
            this.tabPage1.Controls.Add(this.Player1_Select);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.Player1_Left);
            this.tabPage1.Controls.Add(this.Player1_Up);
            this.tabPage1.Controls.Add(this.Player1_Start);
            this.tabPage1.Controls.Add(this.Player1_B);
            this.tabPage1.Controls.Add(this.Player1_A);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(370, 264);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Player 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Player1_Down
            // 
            this.Player1_Down.Location = new System.Drawing.Point(56, 114);
            this.Player1_Down.Name = "Player1_Down";
            this.Player1_Down.Size = new System.Drawing.Size(307, 23);
            this.Player1_Down.TabIndex = 6;
            this.Player1_Down.Text = "button4";
            this.Player1_Down.UseVisualStyleBackColor = true;
            this.Player1_Down.Click += new System.EventHandler(this.Player1_Down_Click);
            // 
            // Player1_Right
            // 
            this.Player1_Right.Location = new System.Drawing.Point(56, 56);
            this.Player1_Right.Name = "Player1_Right";
            this.Player1_Right.Size = new System.Drawing.Size(307, 23);
            this.Player1_Right.TabIndex = 5;
            this.Player1_Right.Text = "button4";
            this.Player1_Right.UseVisualStyleBackColor = true;
            this.Player1_Right.Click += new System.EventHandler(this.Player1_Right_Click);
            // 
            // Player1_Select
            // 
            this.Player1_Select.Location = new System.Drawing.Point(56, 230);
            this.Player1_Select.Name = "Player1_Select";
            this.Player1_Select.Size = new System.Drawing.Size(307, 23);
            this.Player1_Select.TabIndex = 3;
            this.Player1_Select.Text = "button4";
            this.Player1_Select.UseVisualStyleBackColor = true;
            this.Player1_Select.Click += new System.EventHandler(this.Player1_Select_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 234);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 14);
            this.label9.TabIndex = 31;
            this.label9.Text = "Select : ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 205);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 14);
            this.label10.TabIndex = 30;
            this.label10.Text = "Start : ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(6, 176);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(27, 14);
            this.label11.TabIndex = 29;
            this.label11.Text = "B : ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(6, 147);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(28, 14);
            this.label12.TabIndex = 28;
            this.label12.Text = "A : ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(6, 118);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 14);
            this.label13.TabIndex = 27;
            this.label13.Text = "Down : ";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(6, 89);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 14);
            this.label14.TabIndex = 26;
            this.label14.Text = "Up : ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(6, 60);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 14);
            this.label15.TabIndex = 25;
            this.label15.Text = "Right : ";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(6, 31);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(44, 14);
            this.label16.TabIndex = 24;
            this.label16.Text = "Left : ";
            // 
            // Player1_Left
            // 
            this.Player1_Left.Location = new System.Drawing.Point(56, 27);
            this.Player1_Left.Name = "Player1_Left";
            this.Player1_Left.Size = new System.Drawing.Size(307, 23);
            this.Player1_Left.TabIndex = 7;
            this.Player1_Left.Text = "button4";
            this.Player1_Left.UseVisualStyleBackColor = true;
            this.Player1_Left.Click += new System.EventHandler(this.Player1_Left_Click);
            // 
            // Player1_Up
            // 
            this.Player1_Up.Location = new System.Drawing.Point(56, 85);
            this.Player1_Up.Name = "Player1_Up";
            this.Player1_Up.Size = new System.Drawing.Size(307, 23);
            this.Player1_Up.TabIndex = 4;
            this.Player1_Up.Text = "button4";
            this.Player1_Up.UseVisualStyleBackColor = true;
            this.Player1_Up.Click += new System.EventHandler(this.Player1_Up_Click);
            // 
            // Player1_Start
            // 
            this.Player1_Start.Location = new System.Drawing.Point(56, 201);
            this.Player1_Start.Name = "Player1_Start";
            this.Player1_Start.Size = new System.Drawing.Size(307, 23);
            this.Player1_Start.TabIndex = 2;
            this.Player1_Start.Text = "button4";
            this.Player1_Start.UseVisualStyleBackColor = true;
            this.Player1_Start.Click += new System.EventHandler(this.Player1_Start_Click);
            // 
            // Player1_B
            // 
            this.Player1_B.Location = new System.Drawing.Point(56, 172);
            this.Player1_B.Name = "Player1_B";
            this.Player1_B.Size = new System.Drawing.Size(307, 23);
            this.Player1_B.TabIndex = 1;
            this.Player1_B.Text = "button4";
            this.Player1_B.UseVisualStyleBackColor = true;
            this.Player1_B.Click += new System.EventHandler(this.Player1_B_Click);
            // 
            // Player1_A
            // 
            this.Player1_A.Location = new System.Drawing.Point(56, 143);
            this.Player1_A.Name = "Player1_A";
            this.Player1_A.Size = new System.Drawing.Size(307, 23);
            this.Player1_A.TabIndex = 0;
            this.Player1_A.Text = "button4";
            this.Player1_A.UseVisualStyleBackColor = true;
            this.Player1_A.Click += new System.EventHandler(this.Player1_A_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabPage2.Controls.Add(this.Player2_Select);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.Player2_Down);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.Player2_Right);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.Player2_Left);
            this.tabPage2.Controls.Add(this.Player2_Up);
            this.tabPage2.Controls.Add(this.Player2_Start);
            this.tabPage2.Controls.Add(this.Player2_B);
            this.tabPage2.Controls.Add(this.Player2_A);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(370, 264);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Player 2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Player2_Select
            // 
            this.Player2_Select.Location = new System.Drawing.Point(56, 230);
            this.Player2_Select.Name = "Player2_Select";
            this.Player2_Select.Size = new System.Drawing.Size(307, 23);
            this.Player2_Select.TabIndex = 11;
            this.Player2_Select.Text = "button4";
            this.Player2_Select.UseVisualStyleBackColor = true;
            this.Player2_Select.Click += new System.EventHandler(this.Player2_Select_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 234);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 14);
            this.label8.TabIndex = 23;
            this.label8.Text = "Select : ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 205);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 14);
            this.label7.TabIndex = 22;
            this.label7.Text = "Start : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 176);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 14);
            this.label6.TabIndex = 21;
            this.label6.Text = "B : ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 14);
            this.label5.TabIndex = 20;
            this.label5.Text = "A : ";
            // 
            // Player2_Down
            // 
            this.Player2_Down.Location = new System.Drawing.Point(56, 114);
            this.Player2_Down.Name = "Player2_Down";
            this.Player2_Down.Size = new System.Drawing.Size(307, 23);
            this.Player2_Down.TabIndex = 14;
            this.Player2_Down.Text = "button4";
            this.Player2_Down.UseVisualStyleBackColor = true;
            this.Player2_Down.Click += new System.EventHandler(this.Player2_Down_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 14);
            this.label4.TabIndex = 19;
            this.label4.Text = "Down : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 14);
            this.label3.TabIndex = 18;
            this.label3.Text = "Up : ";
            // 
            // Player2_Right
            // 
            this.Player2_Right.Location = new System.Drawing.Point(56, 56);
            this.Player2_Right.Name = "Player2_Right";
            this.Player2_Right.Size = new System.Drawing.Size(307, 23);
            this.Player2_Right.TabIndex = 13;
            this.Player2_Right.Text = "button4";
            this.Player2_Right.UseVisualStyleBackColor = true;
            this.Player2_Right.Click += new System.EventHandler(this.Player2_Right_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 14);
            this.label2.TabIndex = 17;
            this.label2.Text = "Right : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 14);
            this.label1.TabIndex = 16;
            this.label1.Text = "Left : ";
            // 
            // Player2_Left
            // 
            this.Player2_Left.Location = new System.Drawing.Point(56, 27);
            this.Player2_Left.Name = "Player2_Left";
            this.Player2_Left.Size = new System.Drawing.Size(307, 23);
            this.Player2_Left.TabIndex = 15;
            this.Player2_Left.Text = "button4";
            this.Player2_Left.UseVisualStyleBackColor = true;
            this.Player2_Left.Click += new System.EventHandler(this.Player2_Left_Click);
            // 
            // Player2_Up
            // 
            this.Player2_Up.Location = new System.Drawing.Point(56, 85);
            this.Player2_Up.Name = "Player2_Up";
            this.Player2_Up.Size = new System.Drawing.Size(307, 23);
            this.Player2_Up.TabIndex = 12;
            this.Player2_Up.Text = "button4";
            this.Player2_Up.UseVisualStyleBackColor = true;
            this.Player2_Up.Click += new System.EventHandler(this.Player2_Up_Click);
            // 
            // Player2_Start
            // 
            this.Player2_Start.Location = new System.Drawing.Point(56, 201);
            this.Player2_Start.Name = "Player2_Start";
            this.Player2_Start.Size = new System.Drawing.Size(307, 23);
            this.Player2_Start.TabIndex = 10;
            this.Player2_Start.Text = "button4";
            this.Player2_Start.UseVisualStyleBackColor = true;
            this.Player2_Start.Click += new System.EventHandler(this.Player2_Start_Click);
            // 
            // Player2_B
            // 
            this.Player2_B.Location = new System.Drawing.Point(56, 172);
            this.Player2_B.Name = "Player2_B";
            this.Player2_B.Size = new System.Drawing.Size(307, 23);
            this.Player2_B.TabIndex = 9;
            this.Player2_B.Text = "button4";
            this.Player2_B.UseVisualStyleBackColor = true;
            this.Player2_B.Click += new System.EventHandler(this.Player2_B_Click);
            // 
            // Player2_A
            // 
            this.Player2_A.Location = new System.Drawing.Point(56, 143);
            this.Player2_A.Name = "Player2_A";
            this.Player2_A.Size = new System.Drawing.Size(307, 23);
            this.Player2_A.TabIndex = 8;
            this.Player2_A.Text = "button4";
            this.Player2_A.UseVisualStyleBackColor = true;
            this.Player2_A.Click += new System.EventHandler(this.Player2_A_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(315, 360);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "&Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(234, 360);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "&Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.linkLabel3);
            this.groupBox1.Controls.Add(this.linkLabel2);
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 46);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Profile";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(242, 22);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(26, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Add";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(274, 22);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(46, 13);
            this.linkLabel2.TabIndex = 1;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Remove";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(326, 22);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(46, 13);
            this.linkLabel3.TabIndex = 2;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Rename";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(6, 19);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(230, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Frm_ControlsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 395);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Frm_ControlsSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Controls settings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button Player1_A;
        private System.Windows.Forms.Button Player1_Left;
        private System.Windows.Forms.Button Player1_Down;
        private System.Windows.Forms.Button Player1_Right;
        private System.Windows.Forms.Button Player1_Up;
        private System.Windows.Forms.Button Player1_Select;
        private System.Windows.Forms.Button Player1_Start;
        private System.Windows.Forms.Button Player1_B;
        private System.Windows.Forms.Button Player2_Left;
        private System.Windows.Forms.Button Player2_Down;
        private System.Windows.Forms.Button Player2_Right;
        private System.Windows.Forms.Button Player2_Up;
        private System.Windows.Forms.Button Player2_Select;
        private System.Windows.Forms.Button Player2_Start;
        private System.Windows.Forms.Button Player2_B;
        private System.Windows.Forms.Button Player2_A;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}