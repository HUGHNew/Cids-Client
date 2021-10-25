using System;
using System.Windows.Forms;

namespace Client
{
    public class IconLabel:UserControl
    {
        private TableLayoutPanel RowTable;
        private Label LogoLabel;
        private PictureBox icon;

        public IconLabel() {
            InitializeComponent();
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            int length = this.Size.Height;
            this.icon.Size=new System.Drawing.Size(length,length);
            this.LogoLabel.Size=new System.Drawing.Size(length, Size.Width-length);
            base.OnLayout(e);
        }
        private void InitializeComponent()
        {
            this.RowTable = new System.Windows.Forms.TableLayoutPanel();
            this.icon = new System.Windows.Forms.PictureBox();
            this.LogoLabel = new System.Windows.Forms.Label();
            this.RowTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
            this.SuspendLayout();
            // 
            // RowTable
            // 
            this.RowTable.ColumnCount = 2;
            this.RowTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.RowTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RowTable.Controls.Add(this.icon, 0, 0);
            this.RowTable.Controls.Add(this.LogoLabel, 1, 0);
            this.RowTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RowTable.Location = new System.Drawing.Point(0, 0);
            this.RowTable.Name = "RowTable";
            this.RowTable.RowCount = 1;
            this.RowTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RowTable.Size = new System.Drawing.Size(200, 70);
            this.RowTable.TabIndex = 0;
            // 
            // icon
            // 
            this.icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.icon.Cursor = System.Windows.Forms.Cursors.Default;
            this.icon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.icon.Image = global::Client.Properties.Resources.SCU_jpg;
            this.icon.Location = new System.Drawing.Point(3, 3);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(44, 62);
            this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.icon.TabIndex = 0;
            this.icon.TabStop = false;
            // 
            // LogoLabel
            // 
            this.LogoLabel.AutoSize = true;
            this.LogoLabel.CausesValidation = false;
            this.LogoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogoLabel.Font = new System.Drawing.Font("Microsoft YaHei", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogoLabel.Location = new System.Drawing.Point(53, 0);
            this.LogoLabel.Name = "LogoLabel";
            this.LogoLabel.Size = new System.Drawing.Size(146, 68);
            this.LogoLabel.TabIndex = 1;
            this.LogoLabel.Text = "Cids";
            this.LogoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // IconLabel
            // 
            this.Controls.Add(this.RowTable);
            this.Name = "IconLabel";
            this.Size = new System.Drawing.Size(200, 70);
            this.RowTable.ResumeLayout(false);
            this.RowTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            this.ResumeLayout(false);

        }

        private void RowTable_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
