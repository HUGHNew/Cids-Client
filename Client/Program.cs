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
            MainForm MainForm = new MainForm();
            Application.Run(MainForm);
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
