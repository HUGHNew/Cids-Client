using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Client
{

    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //MainForm MainForm = new MainForm();
            Application.Run(new SideTool());
            //Application.Run(new Message.AutoClosingForm(10000, "中文"));
            //Application.Run(new Message.AutoClosingForm(10000, "尝尝中文 长一点"));
            //Application.Run(new Message.AutoClosingForm(10000, "尝尝中文 长一点 再长一点"));
        }
        public static void ShowFrameTest(List<Json.ReceiveComponent.MessageData> msglist)
        {
            foreach(var it in msglist)
            {
                Application.Run(new Message.AutoClosingForm(it.ExpireTime,it.Text));
            }
        }
    }
}
