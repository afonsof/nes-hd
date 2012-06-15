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
    partial class Frm_Palette
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.radioButton1_useInternal = new System.Windows.Forms.RadioButton();
            this.radioButton2_useExternal = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton5_ntsc = new System.Windows.Forms.RadioButton();
            this.radioButton4_pal = new System.Windows.Forms.RadioButton();
            this.radioButton3_auto = new System.Windows.Forms.RadioButton();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.radioButton1_useInternal);
            this.groupBox1.Controls.Add(this.radioButton2_useExternal);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 168);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(59, 140);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(235, 20);
            this.textBox1.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 140);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(47, 20);
            this.button1.TabIndex = 3;
            this.button1.Text = ".....";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButton1_useInternal
            // 
            this.radioButton1_useInternal.AutoSize = true;
            this.radioButton1_useInternal.Location = new System.Drawing.Point(6, 19);
            this.radioButton1_useInternal.Name = "radioButton1_useInternal";
            this.radioButton1_useInternal.Size = new System.Drawing.Size(119, 17);
            this.radioButton1_useInternal.TabIndex = 0;
            this.radioButton1_useInternal.TabStop = true;
            this.radioButton1_useInternal.Text = "Use internal palette";
            this.radioButton1_useInternal.UseVisualStyleBackColor = true;
            this.radioButton1_useInternal.CheckedChanged += new System.EventHandler(this.radioButton1_useInternal_CheckedChanged);
            // 
            // radioButton2_useExternal
            // 
            this.radioButton2_useExternal.AutoSize = true;
            this.radioButton2_useExternal.Location = new System.Drawing.Point(6, 117);
            this.radioButton2_useExternal.Name = "radioButton2_useExternal";
            this.radioButton2_useExternal.Size = new System.Drawing.Size(97, 17);
            this.radioButton2_useExternal.TabIndex = 2;
            this.radioButton2_useExternal.TabStop = true;
            this.radioButton2_useExternal.Text = "Use palette file";
            this.radioButton2_useExternal.UseVisualStyleBackColor = true;
            this.radioButton2_useExternal.CheckedChanged += new System.EventHandler(this.radioButton2_useExternal_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton5_ntsc);
            this.groupBox2.Controls.Add(this.radioButton4_pal);
            this.groupBox2.Controls.Add(this.radioButton3_auto);
            this.groupBox2.Location = new System.Drawing.Point(8, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(286, 88);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // radioButton5_ntsc
            // 
            this.radioButton5_ntsc.AutoSize = true;
            this.radioButton5_ntsc.Location = new System.Drawing.Point(6, 65);
            this.radioButton5_ntsc.Name = "radioButton5_ntsc";
            this.radioButton5_ntsc.Size = new System.Drawing.Size(128, 17);
            this.radioButton5_ntsc.TabIndex = 2;
            this.radioButton5_ntsc.TabStop = true;
            this.radioButton5_ntsc.Text = "Use the NTSC palette";
            this.radioButton5_ntsc.UseVisualStyleBackColor = true;
            this.radioButton5_ntsc.CheckedChanged += new System.EventHandler(this.radioButton5_ntsc_CheckedChanged);
            // 
            // radioButton4_pal
            // 
            this.radioButton4_pal.AutoSize = true;
            this.radioButton4_pal.Location = new System.Drawing.Point(6, 42);
            this.radioButton4_pal.Name = "radioButton4_pal";
            this.radioButton4_pal.Size = new System.Drawing.Size(120, 17);
            this.radioButton4_pal.TabIndex = 1;
            this.radioButton4_pal.TabStop = true;
            this.radioButton4_pal.Text = "Use the PAL palette";
            this.radioButton4_pal.UseVisualStyleBackColor = true;
            this.radioButton4_pal.CheckedChanged += new System.EventHandler(this.radioButton4_pal_CheckedChanged);
            // 
            // radioButton3_auto
            // 
            this.radioButton3_auto.AutoSize = true;
            this.radioButton3_auto.Location = new System.Drawing.Point(6, 19);
            this.radioButton3_auto.Name = "radioButton3_auto";
            this.radioButton3_auto.Size = new System.Drawing.Size(141, 17);
            this.radioButton3_auto.TabIndex = 0;
            this.radioButton3_auto.TabStop = true;
            this.radioButton3_auto.Text = "Auto select (PAL, NTSC)";
            this.radioButton3_auto.UseVisualStyleBackColor = true;
            this.radioButton3_auto.CheckedChanged += new System.EventHandler(this.radioButton3_auto_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(237, 286);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "&Ok";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(156, 286);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "&Cancel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 286);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "&Defaults";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Location = new System.Drawing.Point(12, 186);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(300, 91);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "View";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(6, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(288, 72);
            this.panel1.TabIndex = 0;
            // 
            // Frm_Palette
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 321);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Frm_Palette";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Palette";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton1_useInternal;
        private System.Windows.Forms.RadioButton radioButton2_useExternal;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton radioButton5_ntsc;
        private System.Windows.Forms.RadioButton radioButton4_pal;
        private System.Windows.Forms.RadioButton radioButton3_auto;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel1;
    }
}