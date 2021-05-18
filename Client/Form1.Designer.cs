namespace Client
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.CenterPicture = new System.Windows.Forms.PictureBox();
            this.LoadLabel = new System.Windows.Forms.Label();
            this.BGWorkerMain = new System.ComponentModel.BackgroundWorker();
            this.Title = new System.Windows.Forms.Label();
            this.Shut = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.CenterPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // CenterPicture
            // 
            this.CenterPicture.Image = ((System.Drawing.Image)(resources.GetObject("CenterPicture.Image")));
            this.CenterPicture.Location = new System.Drawing.Point(0, 57);
            this.CenterPicture.Name = "CenterPicture";
            this.CenterPicture.Size = new System.Drawing.Size(552, 76);
            this.CenterPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.CenterPicture.TabIndex = 0;
            this.CenterPicture.TabStop = false;
            // 
            // LoadLabel
            // 
            this.LoadLabel.AutoSize = true;
            this.LoadLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LoadLabel.Location = new System.Drawing.Point(214, 159);
            this.LoadLabel.Name = "LoadLabel";
            this.LoadLabel.Size = new System.Drawing.Size(125, 17);
            this.LoadLabel.TabIndex = 1;
            this.LoadLabel.Text = "加载资源中，请稍候...";
            // 
            // BGWorkerMain
            // 
            this.BGWorkerMain.WorkerReportsProgress = true;
            this.BGWorkerMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BGWorkerMain_DoWork);
            this.BGWorkerMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BGWorkerMain_ProgressChanged);
            this.BGWorkerMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BGWorkerMain_RunWorkerCompleted);
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Title.Location = new System.Drawing.Point(12, 9);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(200, 17);
            this.Title.TabIndex = 2;
            this.Title.Text = "四川大学智慧教学系统壁纸同步工具";
            // 
            // Shut
            // 
            this.Shut.AutoSize = true;
            this.Shut.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Shut.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Shut.Location = new System.Drawing.Point(476, 9);
            this.Shut.Name = "Shut";
            this.Shut.Size = new System.Drawing.Size(64, 17);
            this.Shut.TabIndex = 3;
            this.Shut.Text = "[强制关闭]";
            this.Shut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Shut.Visible = false;
            this.Shut.Click += new System.EventHandler(this.Shut_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(552, 200);
            this.Controls.Add(this.Shut);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.LoadLabel);
            this.Controls.Add(this.CenterPicture);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.CenterPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox CenterPicture;
        private System.Windows.Forms.Label LoadLabel;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label Shut;
        private System.ComponentModel.BackgroundWorker BGWorkerMain;

        // Unknown Error Here
        private void BackWorker()
        {

        }
        private void PictureProcess()
        {
            
        }
        private void LabelTitle() {

        }
        private void LabelLoad() {

        }
        private void LabelShut() {

        }
    }
}

