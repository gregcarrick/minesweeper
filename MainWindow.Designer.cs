﻿namespace Minesweeper
{
    partial class MainWindow
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
            if (disposing && (components != null))
            {
                if (this.panel1 != null)
                {
                    this.panel1.Paint -= panel1_Paint;
                }
                components.Dispose();
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
            this.panel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timerControl = new Minesweeper.CounterControl();
            this.mineCounter = new Minesweeper.CounterControl();
            this.counterControl1 = new Minesweeper.CounterControl();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Location = new System.Drawing.Point(0, 34);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(192, 192);
            this.panel.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Location = new System.Drawing.Point(57, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(78, 34);
            this.panel1.TabIndex = 3;
            // 
            // timerControl
            // 
            this.timerControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timerControl.BackColor = System.Drawing.Color.Silver;
            this.timerControl.Location = new System.Drawing.Point(135, 0);
            this.timerControl.Name = "timerControl";
            this.timerControl.Size = new System.Drawing.Size(57, 34);
            this.timerControl.TabIndex = 2;
            this.timerControl.Value = 0;
            // 
            // mineCounter
            // 
            this.mineCounter.BackColor = System.Drawing.Color.Silver;
            this.mineCounter.Location = new System.Drawing.Point(0, 0);
            this.mineCounter.Name = "mineCounter";
            this.mineCounter.Size = new System.Drawing.Size(57, 34);
            this.mineCounter.TabIndex = 1;
            this.mineCounter.Value = 0;
            // 
            // counterControl1
            // 
            this.counterControl1.BackColor = System.Drawing.Color.Silver;
            this.counterControl1.Location = new System.Drawing.Point(0, 0);
            this.counterControl1.Name = "counterControl1";
            this.counterControl1.Size = new System.Drawing.Size(57, 34);
            this.counterControl1.TabIndex = 1;
            this.counterControl1.Value = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(192, 226);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.timerControl);
            this.Controls.Add(this.mineCounter);
            this.Controls.Add(this.panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.Text = "Minesweeper";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private CounterControl mineCounter;
        private CounterControl counterControl1;
        private CounterControl timerControl;
        private System.Windows.Forms.Panel panel1;
    }
}
