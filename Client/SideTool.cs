using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
    public partial class SideTool : Form {
        public SideTool() {
            InitializeComponent();
        }

        private void ToolsPanelResize(object sender, EventArgs e) { ToolsPanelResize_CallBack(); }
        private void ToolsPanelResize_CallBack() {
            int Margins = ToolsPanel.Margin.Right + ToolsPanel.Margin.Left;
            int PanelHeightCell = ToolsPanel.Height / (ToolsPanel.Controls.Count + 1);
            foreach (Control clt in ToolsPanel.Controls) {
                // Width Extends
                clt.Width = ToolsPanel.Width - Margins * 2;
                // Height Etends
                clt.Height = PanelHeightCell;
            }
        }

        private void SideTool_Load(object sender, EventArgs e) {
            int ScreenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            Location = new Point(ScreenWidth - this.Width, 0);
            ToolsPanelResize_CallBack();
        }

        private bool EdgeJudge() {
            if (this.Top <= 0 && this.Left <= 0) {
                StopAnchor = AnchorStyles.None;
                return false;
            } else if (this.Top <= 0) {
                StopAnchor = AnchorStyles.Top;
            } else if (this.Left <= 0) {
                StopAnchor = AnchorStyles.Left;
            } else if (this.Left >= Screen.PrimaryScreen.Bounds.Width - this.Width) {
                StopAnchor = AnchorStyles.Right;
            } else if (this.Top >= Screen.PrimaryScreen.Bounds.Height - this.Height) {
                StopAnchor = AnchorStyles.Bottom;
            } else {
                StopAnchor = AnchorStyles.None;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Location Changed Callback : hide the form if near edge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hide_Gauge(object sender, EventArgs e) {
            if (EdgeJudge()) {
                // should hide
            }
        }
        internal AnchorStyles StopAnchor = AnchorStyles.None;

        private void Hide_Tick(object sender, EventArgs e) {
            if (this.Bounds.Contains(Cursor.Position)) {
                switch (StopAnchor) {
                    case AnchorStyles.Top:
                        this.Location = new Point(this.Location.X, 0);
                        break;
                    case AnchorStyles.Left:
                        this.Location = new Point(0, this.Location.Y);
                        break;
                    case AnchorStyles.Right:
                        this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, this.Location.Y);
                        break;
                    case AnchorStyles.Bottom:
                        this.Location = new Point(this.Location.X, Screen.PrimaryScreen.Bounds.Height - this.Height);
                        break;
                }
            } else {
                switch (StopAnchor) {
                    case AnchorStyles.Top:
                        this.Location = new Point(this.Location.X, (this.Height - 8) * (-1));
                        break;
                    case AnchorStyles.Left:
                        this.Location = new Point((-1) * (this.Width - 8), this.Location.Y);
                        break;
                    case AnchorStyles.Right:
                        this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 8, this.Location.Y);
                        break;
                    case AnchorStyles.Bottom:
                        this.Location = new Point(this.Location.X, Screen.PrimaryScreen.Bounds.Height - 8);
                        break;
                }
            }
        }

        private void STBtnIssueClick(object sender, EventArgs e) {
            Feedback fd=new Feedback();
            DialogResult result= fd.ShowDialog();
            if(result == DialogResult.OK) {
                // get msg
                Debug.WriteLine(fd.FeedbackMessage);
            }
        }
    }
}
