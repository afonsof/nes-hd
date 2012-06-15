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
    partial class Frm_RomInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1_Name = new System.Windows.Forms.TextBox();
            this.textBox2_Mapper = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1_prgs = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2_chr = new System.Windows.Forms.TextBox();
            this.label4_chrs = new System.Windows.Forms.Label();
            this.textBox3_mirroring = new System.Windows.Forms.TextBox();
            this.label5_mirroring = new System.Windows.Forms.Label();
            this.checkBox1_saveram = new System.Windows.Forms.CheckBox();
            this.checkBox2_trainer = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1_four = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name : ";
            // 
            // textBox1_Name
            // 
            this.textBox1_Name.Location = new System.Drawing.Point(77, 12);
            this.textBox1_Name.Name = "textBox1_Name";
            this.textBox1_Name.ReadOnly = true;
            this.textBox1_Name.Size = new System.Drawing.Size(193, 20);
            this.textBox1_Name.TabIndex = 1;
            // 
            // textBox2_Mapper
            // 
            this.textBox2_Mapper.Location = new System.Drawing.Point(77, 38);
            this.textBox2_Mapper.Name = "textBox2_Mapper";
            this.textBox2_Mapper.ReadOnly = true;
            this.textBox2_Mapper.Size = new System.Drawing.Size(193, 20);
            this.textBox2_Mapper.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mapper : ";
            // 
            // textBox1_prgs
            // 
            this.textBox1_prgs.Location = new System.Drawing.Point(77, 64);
            this.textBox1_prgs.Name = "textBox1_prgs";
            this.textBox1_prgs.ReadOnly = true;
            this.textBox1_prgs.Size = new System.Drawing.Size(62, 20);
            this.textBox1_prgs.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "PRG\'s :";
            // 
            // textBox2_chr
            // 
            this.textBox2_chr.Location = new System.Drawing.Point(192, 64);
            this.textBox2_chr.Name = "textBox2_chr";
            this.textBox2_chr.ReadOnly = true;
            this.textBox2_chr.Size = new System.Drawing.Size(78, 20);
            this.textBox2_chr.TabIndex = 7;
            // 
            // label4_chrs
            // 
            this.label4_chrs.AutoSize = true;
            this.label4_chrs.Location = new System.Drawing.Point(145, 67);
            this.label4_chrs.Name = "label4_chrs";
            this.label4_chrs.Size = new System.Drawing.Size(42, 13);
            this.label4_chrs.TabIndex = 6;
            this.label4_chrs.Text = "CHR\'s :";
            // 
            // textBox3_mirroring
            // 
            this.textBox3_mirroring.Location = new System.Drawing.Point(77, 90);
            this.textBox3_mirroring.Name = "textBox3_mirroring";
            this.textBox3_mirroring.ReadOnly = true;
            this.textBox3_mirroring.Size = new System.Drawing.Size(193, 20);
            this.textBox3_mirroring.TabIndex = 9;
            // 
            // label5_mirroring
            // 
            this.label5_mirroring.AutoSize = true;
            this.label5_mirroring.Location = new System.Drawing.Point(12, 93);
            this.label5_mirroring.Name = "label5_mirroring";
            this.label5_mirroring.Size = new System.Drawing.Size(59, 13);
            this.label5_mirroring.TabIndex = 8;
            this.label5_mirroring.Text = "Mirroring : ";
            // 
            // checkBox1_saveram
            // 
            this.checkBox1_saveram.AutoSize = true;
            this.checkBox1_saveram.Enabled = false;
            this.checkBox1_saveram.Location = new System.Drawing.Point(167, 116);
            this.checkBox1_saveram.Name = "checkBox1_saveram";
            this.checkBox1_saveram.Size = new System.Drawing.Size(82, 17);
            this.checkBox1_saveram.TabIndex = 10;
            this.checkBox1_saveram.Text = "Is save ram";
            this.checkBox1_saveram.UseVisualStyleBackColor = true;
            // 
            // checkBox2_trainer
            // 
            this.checkBox2_trainer.AutoSize = true;
            this.checkBox2_trainer.Enabled = false;
            this.checkBox2_trainer.Location = new System.Drawing.Point(101, 116);
            this.checkBox2_trainer.Name = "checkBox2_trainer";
            this.checkBox2_trainer.Size = new System.Drawing.Size(60, 17);
            this.checkBox2_trainer.TabIndex = 11;
            this.checkBox2_trainer.Text = "Trainer";
            this.checkBox2_trainer.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(80, 142);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "&Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1_four
            // 
            this.checkBox1_four.AutoSize = true;
            this.checkBox1_four.Enabled = false;
            this.checkBox1_four.Location = new System.Drawing.Point(12, 116);
            this.checkBox1_four.Name = "checkBox1_four";
            this.checkBox1_four.Size = new System.Drawing.Size(83, 17);
            this.checkBox1_four.TabIndex = 13;
            this.checkBox1_four.Text = "Four screen";
            this.checkBox1_four.UseVisualStyleBackColor = true;
            // 
            // Frm_RomInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 177);
            this.ControlBox = false;
            this.Controls.Add(this.checkBox1_four);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox2_trainer);
            this.Controls.Add(this.checkBox1_saveram);
            this.Controls.Add(this.textBox3_mirroring);
            this.Controls.Add(this.label5_mirroring);
            this.Controls.Add(this.textBox2_chr);
            this.Controls.Add(this.label4_chrs);
            this.Controls.Add(this.textBox1_prgs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox2_Mapper);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1_Name);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Frm_RomInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rom Info";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1_Name;
        private System.Windows.Forms.TextBox textBox2_Mapper;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1_prgs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2_chr;
        private System.Windows.Forms.Label label4_chrs;
        private System.Windows.Forms.TextBox textBox3_mirroring;
        private System.Windows.Forms.Label label5_mirroring;
        private System.Windows.Forms.CheckBox checkBox1_saveram;
        private System.Windows.Forms.CheckBox checkBox2_trainer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1_four;
    }
}