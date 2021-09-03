using System;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading;

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
            //scatter();
            //tmptest();
            //RegisrtyTest();
            //NetTest();
            //Test.UdpTest.TcpTimeOutTest();
            //Test.UdpTest.ClientCenterOnly();
            //Test.UdpTest.ClientMirrorOnly();
            //Test.UdpTest.ClientRealTest();
            //Test.ImgTest.ppt();
            //Test.ShowTest.SeriesShow();
            //Toggel();
            //cover();
            //Test.ShowTest.SingleShow();
            //Test.UdpTest.DLoadTest();
            //Test.TcpTest.TcpClt();
            //Test.TcpTest.TcpHb();
            //halfpack();
            //emptyList();
            //imgt0();
            //JsonTest.newlytest();
            //JsonTest.test();
            //SendRecv();
            //t2();
            //OctBytesTest();
            //Test.ImgTest.ImgSwitch();
            //Test.ImgTest.image_switch();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 MainForm = new Form1();
            Application.Run(MainForm);
            //Debug.WriteLine("Before Sleep");
            //Thread.Sleep(2000);
            //Debug.WriteLine("End Sleep");
            //MainForm.Hide();
            //Debug.WriteLine("End Hide");
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
        static void Toggel()
        {
            bool boolean = false;
            write(boolean.ToString());
            boolean = !boolean;
            write(boolean.ToString());
            boolean = !boolean;
            write(boolean.ToString());
        }
        public static void CS()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                Test.UdpServer.MainServer();
            }
            );
            System.Threading.Thread.Sleep(100);
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                Test.UdpServer.MirrorServer();
                Test.UdpServer.HB();
            }
            );
            Test.UdpTest.Client();
        }
        static void halfpack()
        {
            string half = "{\"needUpdate\":false";
            Json.MirrorReceive jm = JsonConvert.DeserializeObject<Json.MirrorReceive>(half);
            if (jm == null)
            {
                Console.WriteLine("null");
            }
            else
            {
                Console.WriteLine(jm.NeedUpdate);
            }
        }
        static void tmptest()
        {
            string tmp = (Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine)["TMP"] as string);
            Console.WriteLine(tmp);
            try
            {
                File.Create(Path.Combine(tmp, "tmp.txt"));
            }
            catch (Exception e)
            {
                Console.WriteLine("Create Failed:" + e.Message);
            }
        }
        static void NetTest()
        {
            Json.ConfComponent.NetData net=new Json.ConfComponent.NetData { 
                Main_Ip="192.168.0.1",
                Main_Port=8080,
                Mirror_Port=9090
            };
            Console.WriteLine(JsonConvert.SerializeObject(net));
            Console.WriteLine(net.IPv4);
            //else Console.WriteLine("Null Object");
        }
    }
}
