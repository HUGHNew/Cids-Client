namespace Client
{
    partial class Feedback
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Feedback));
            this.FdTBIssue = new System.Windows.Forms.TextBox();
            this.FdPanelBtn = new System.Windows.Forms.FlowLayoutPanel();
            this.FdBtnSubmit = new System.Windows.Forms.Button();
            this.FdBtnCancel = new System.Windows.Forms.Button();
            this.FdPanelBtn.SuspendLayout();
            this.SuspendLayout();
            // 
            // FdTBIssue
            // 
            this.FdTBIssue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FdTBIssue.Location = new System.Drawing.Point(12, 92);
            this.FdTBIssue.MaxLength = 256;
            this.FdTBIssue.Multiline = true;
            this.FdTBIssue.Name = "FdTBIssue";
            this.FdTBIssue.Size = new System.Drawing.Size(760, 220);
            this.FdTBIssue.TabIndex = 0;
            this.FdTBIssue.TextChanged += new System.EventHandler(this.FdTextChange);
            // 
            // FdPanelBtn
            // 
            this.FdPanelBtn.Controls.Add(this.FdBtnSubmit);
            this.FdPanelBtn.Controls.Add(this.FdBtnCancel);
            this.FdPanelBtn.Location = new System.Drawing.Point(12, 318);
            this.FdPanelBtn.Name = "FdPanelBtn";
            this.FdPanelBtn.Size = new System.Drawing.Size(760, 71);
            this.FdPanelBtn.TabIndex = 1;
            // 
            // FdBtnSubmit
            // 
            this.FdBtnSubmit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.FdBtnSubmit.Location = new System.Drawing.Point(0, 0);
            this.FdBtnSubmit.Margin = new System.Windows.Forms.Padding(0);
            this.FdBtnSubmit.Name = "FdBtnSubmit";
            this.FdBtnSubmit.Size = new System.Drawing.Size(380, 68);
            this.FdBtnSubmit.TabIndex = 0;
            this.FdBtnSubmit.Text = "确定";
            this.FdBtnSubmit.UseVisualStyleBackColor = true;
            this.FdBtnSubmit.Click += new System.EventHandler(this.FdBtnSubmitClick);
            // 
            // FdBtnCancel
            // 
            this.FdBtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.FdBtnCancel.Location = new System.Drawing.Point(380, 0);
            this.FdBtnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.FdBtnCancel.Name = "FdBtnCancel";
            this.FdBtnCancel.Size = new System.Drawing.Size(380, 68);
            this.FdBtnCancel.TabIndex = 1;
            this.FdBtnCancel.Text = "取消";
            this.FdBtnCancel.UseVisualStyleBackColor = true;
            this.FdBtnCancel.Click += new System.EventHandler(this.FdBtnCancelClick);
            // 
            // Feedback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.FdPanelBtn);
            this.Controls.Add(this.FdTBIssue);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Feedback";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "意见反馈";
            this.FdPanelBtn.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FdTBIssue;
        private System.Windows.Forms.FlowLayoutPanel FdPanelBtn;
        private System.Windows.Forms.Button FdBtnSubmit;
        private System.Windows.Forms.Button FdBtnCancel;
    }
}