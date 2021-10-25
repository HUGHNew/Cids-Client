using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class SideTool : Form
    {
        public SideTool()
        {
            InitializeComponent();
        }

        private void ToolsPanelResize(object sender, EventArgs e)
        {
            foreach(Control clt in ToolsPanel.Controls)
            {
                int padding = ToolsPanel.Padding.Right + ToolsPanel.Padding.Left;
                clt.Width = ToolsPanel.Width-padding*2;
            }
        }
    }
}
