
namespace Client
{
    partial class SideTool
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SideTool));
            this.ToolsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.Issue = new System.Windows.Forms.Button();
            this.Questionnaire = new System.Windows.Forms.Button();
            this.iconLabel1 = new Client.IconLabel();
            this.HideTimer = new System.Windows.Forms.Timer(this.components);
            this.ToolsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolsPanel
            // 
            this.ToolsPanel.AutoScroll = true;
            this.ToolsPanel.Controls.Add(this.Issue);
            this.ToolsPanel.Controls.Add(this.Questionnaire);
            this.ToolsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ToolsPanel.Location = new System.Drawing.Point(0, 70);
            this.ToolsPanel.Name = "ToolsPanel";
            this.ToolsPanel.Size = new System.Drawing.Size(218, 360);
            this.ToolsPanel.TabIndex = 1;
            this.ToolsPanel.Resize += new System.EventHandler(this.ToolsPanelResize);
            // 
            // Issue
            // 
            this.Issue.Location = new System.Drawing.Point(3, 3);
            this.Issue.Name = "Issue";
            this.Issue.Size = new System.Drawing.Size(211, 38);
            this.Issue.TabIndex = 0;
            this.Issue.Text = "意见反馈";
            this.Issue.UseVisualStyleBackColor = true;
            // 
            // Questionnaire
            // 
            this.Questionnaire.Location = new System.Drawing.Point(3, 47);
            this.Questionnaire.Name = "Questionnaire";
            this.Questionnaire.Size = new System.Drawing.Size(211, 38);
            this.Questionnaire.TabIndex = 1;
            this.Questionnaire.Text = "问卷调查";
            this.Questionnaire.UseVisualStyleBackColor = true;
            // 
            // iconLabel1
            // 
            this.iconLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.iconLabel1.Location = new System.Drawing.Point(0, 0);
            this.iconLabel1.Name = "iconLabel1";
            this.iconLabel1.Size = new System.Drawing.Size(218, 70);
            this.iconLabel1.TabIndex = 0;
            // 
            // HideTimer
            // 
            this.HideTimer.Enabled = true;
            this.HideTimer.Tick += new System.EventHandler(this.Hide_Tick);
            // 
            // SideTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 430);
            this.Controls.Add(this.ToolsPanel);
            this.Controls.Add(this.iconLabel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SideTool";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SideTool";
            this.Load += new System.EventHandler(this.SideTool_Load);
            this.LocationChanged += new System.EventHandler(this.Hide_Gauge);
            this.ToolsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private IconLabel iconLabel1;
        private System.Windows.Forms.FlowLayoutPanel ToolsPanel;
        private System.Windows.Forms.Button Issue;
        private System.Windows.Forms.Button Questionnaire;
        private System.Windows.Forms.Timer HideTimer;
    }
}