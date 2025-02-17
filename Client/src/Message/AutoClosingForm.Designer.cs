﻿
namespace Client.Message
{
    public partial class AutoClosingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoClosingForm));
            this.TimeProgressBar = new System.Windows.Forms.ProgressBar();
            this.MainLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TimeProgressBar
            // 
            this.TimeProgressBar.Location = new System.Drawing.Point(-1, 150);
            this.TimeProgressBar.Name = "TimeProgressBar";
            this.TimeProgressBar.Size = new System.Drawing.Size(401, 10);
            this.TimeProgressBar.TabIndex = 1;
            // 
            // MainLabel
            // 
            this.MainLabel.Font = new System.Drawing.Font("SimSun", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.MainLabel.Location = new System.Drawing.Point(12, 6);
            this.MainLabel.Name = "MainLabel";
            this.MainLabel.Size = new System.Drawing.Size(339, 141);
            this.MainLabel.TabIndex = 0;
            this.MainLabel.Text = "Msg";
            // 
            // CloseButton
            // 
            this.CloseButton.Font = new System.Drawing.Font("SimSun", 20F);
            this.CloseButton.Image = ((System.Drawing.Image)(resources.GetObject("CloseButton.Image")));
            this.CloseButton.Location = new System.Drawing.Point(370, 0);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(30, 30);
            this.CloseButton.TabIndex = 2;
            this.CloseButton.Text = "X";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // AutoClosingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 160);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.MainLabel);
            this.Controls.Add(this.TimeProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AutoClosingForm";
            this.Opacity = 0.84D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "紧急消息";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ProgressBar TimeProgressBar;
        private System.Windows.Forms.Label MainLabel;
        private System.Windows.Forms.Button CloseButton;
    }
}