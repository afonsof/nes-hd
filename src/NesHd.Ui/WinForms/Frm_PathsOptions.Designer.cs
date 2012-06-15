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
    partial class Frm_PathsOptions
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
            this.textBox1_Snapshots = new System.Windows.Forms.TextBox();
            this.button1_Snapshots = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1_States = new System.Windows.Forms.TextBox();
            this.button4_States = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1_Snapshots);
            this.groupBox1.Controls.Add(this.button1_Snapshots);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(387, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Snapshots folder";
            // 
            // textBox1_Snapshots
            // 
            this.textBox1_Snapshots.Location = new System.Drawing.Point(61, 21);
            this.textBox1_Snapshots.Name = "textBox1_Snapshots";
            this.textBox1_Snapshots.ReadOnly = true;
            this.textBox1_Snapshots.Size = new System.Drawing.Size(320, 20);
            this.textBox1_Snapshots.TabIndex = 1;
            // 
            // button1_Snapshots
            // 
            this.button1_Snapshots.Location = new System.Drawing.Point(6, 21);
            this.button1_Snapshots.Name = "button1_Snapshots";
            this.button1_Snapshots.Size = new System.Drawing.Size(49, 20);
            this.button1_Snapshots.TabIndex = 0;
            this.button1_Snapshots.Text = "....";
            this.button1_Snapshots.UseVisualStyleBackColor = true;
            this.button1_Snapshots.Click += new System.EventHandler(this.button1_Snapshots_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(324, 139);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "&Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(243, 139);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "&Close";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 139);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "&Defaults";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1_States);
            this.groupBox2.Controls.Add(this.button4_States);
            this.groupBox2.Location = new System.Drawing.Point(12, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(387, 50);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "State saves folder";
            // 
            // textBox1_States
            // 
            this.textBox1_States.Location = new System.Drawing.Point(61, 21);
            this.textBox1_States.Name = "textBox1_States";
            this.textBox1_States.ReadOnly = true;
            this.textBox1_States.Size = new System.Drawing.Size(320, 20);
            this.textBox1_States.TabIndex = 1;
            // 
            // button4_States
            // 
            this.button4_States.Location = new System.Drawing.Point(6, 21);
            this.button4_States.Name = "button4_States";
            this.button4_States.Size = new System.Drawing.Size(49, 20);
            this.button4_States.TabIndex = 0;
            this.button4_States.Text = "....";
            this.button4_States.UseVisualStyleBackColor = true;
            this.button4_States.Click += new System.EventHandler(this.button4_States_Click);
            // 
            // Frm_PathsOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 174);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_PathsOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Paths";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1_Snapshots;
        private System.Windows.Forms.Button button1_Snapshots;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1_States;
        private System.Windows.Forms.Button button4_States;
    }
}