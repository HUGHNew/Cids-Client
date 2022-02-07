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
            //Form Main = new Feedback();
            Form Main = new MainForm();
            Application.Run(Main);
            Application.Run(new SideTool());
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
