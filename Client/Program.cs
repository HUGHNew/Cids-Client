using System;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using static Client.Message.Show;

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
            //Debug.WriteLine("Before Sleep");
            //Thread.Sleep(2000);
            //Debug.WriteLine("End Sleep");
            //MainForm.Hide();
            //Debug.WriteLine("End Hide");
        }
        
        public static void ShowFrameTest(List<Json.ReceiveComponent.MessageData> msglist)
        {
            foreach(var it in msglist)
            {
                Application.Run(new Message.AutoClosingForm(it.ExpireTime,it.Text));
            }
        }
        public static void MvDir()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Directory.Move(Path.Combine(desktop, "cids-test"), Path.Combine(desktop, @"Temp\ct"));
        }
        public static void cover() {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var files=Directory.EnumerateFiles(Path.Combine(desktop, "test"));
            string fpath = Path.Combine(desktop, "convert");
            foreach (var it in files) {
                //Console.WriteLine(it);
                string[] fn = it.Split('\\');
                string f = fn[fn.Length - 1];
                if (! File.Exists(Path.Combine(fpath, f)))
                File.Move(it, Path.Combine(fpath,f));
            }
            //Directory.Move(Path.Combine(desktop, "test"), Path.Combine(desktop, "convert"));
        }
        static void write(string content)
        {
            Console.WriteLine(content);
        }
    }
}
