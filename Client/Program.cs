using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

static class Temp
{
    static void write(string content)
    {
        Console.WriteLine(content);
    }
    static void Main()
    {
        //write(File.Exists(@"C:\a\b\c") ? "null" : "False");
        //write(Client.Init.Configuration().ToString());
        //WPSave();
        desk();
        //Client.Test.ShowTest.SeriesShow();
        //LMBTest();
        //EnvTest();
        //PFT();
    }
    
    static void desk()
    {
        write(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        write(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        write(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
    }
    
    public static void LMBTest()
    {
        Client.Message.TimedMsgBox.TimedMessageBox("null msg", 10000);
        for(int i = 0; i < 10; ++i)
        {
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine(i);
        }
    }
    public static void PFT()
    {
        Console.WriteLine(File.ReadAllText(@"C:\Program Files (x86)\Cids\Cids.conf"));
        //File.WriteAllText(@"C:\Program Files (x86)\Cids\Cids.conf", "1234567");
    }
    static void EnvTra()
    {
        var v = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);
    }
    public static void EnvTest()
    {
        //Console.WriteLine(Environment.GetEnvironmentVariable("Program Files", EnvironmentVariableTarget.Machine));
        //Console.WriteLine(Environment.GetEnvironmentVariable("ProgramFiles", EnvironmentVariableTarget.Machine));
        //Console.WriteLine(Environment.GetEnvironmentVariable("ProgramFiles", EnvironmentVariableTarget.User));
        //Console.WriteLine(Environment.GetEnvironmentVariable("Program Files", EnvironmentVariableTarget.User));
        Console.WriteLine(Environment.GetEnvironmentVariable("ProgramFiles", EnvironmentVariableTarget.Process));
        //Console.WriteLine(Environment.GetEnvironmentVariable("Program Files", EnvironmentVariableTarget.Process));
        Console.WriteLine(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine));
    }
}

//class Tmp { 
//    public bool Update { get; set; }
//}

//namespace Client
//{

//    static class Program
//    {
//        /// <summary>
//        /// 应用程序的主入口点。
//        /// </summary>
//        [STAThread]
//        static void Main()
//        {
//            //scatter();
//            //tmptest();
//            //RegisrtyTest();
//            halfpack();
//            //emptyList();
//            //imgt0();
//            //JsonTest.newlytest();
//            //JsonTest.test();
//            //SendRecv();
//            //t2();
//            //OctBytesTest();
//            //Application.EnableVisualStyles();
//            //Application.SetCompatibleTextRenderingDefault(false);
//            //Application.Run(new Form1());
//        }
//        static void halfpack()
//        {
//            string half = "{\"needUpdate\":false";
//            Json.MirrorReceive jm = JsonConvert.DeserializeObject<Json.MirrorReceive>(half);
//            if (jm == null)
//            {
//                Console.WriteLine("null");
//            }
//            else
//            {
//                Console.WriteLine(jm.NeedUpdate);
//            }
//        }
//        static void tmptest()
//        {
//            string tmp = (Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine)["TMP"] as string);
//            Console.WriteLine(tmp);
//            try
//            {
//                File.Create(Path.Combine(tmp, "tmp.txt"));
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Create Failed:" + e.Message);
//            }
//        }
//        static void RegisrtyTest()
//        {
//            Init.Configuration();
//        }
//        static void scatter(string field = "TMP")
//        {
//            Console.WriteLine((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User)[field] as string));
//            Console.WriteLine((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process)[field] as string));
//            Console.WriteLine((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine)[field] as string));
//            //Console.WriteLine(Path.Combine("dir","file"));
//        }
//        static void emptyList()
//        {
//            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();
//            foreach (string str in ls)
//            {
//                Console.WriteLine(str);
//            }
//        }
//        static void imgt0()
//        {
//            ImgTest.t0();
//        }
//        static void localCS()
//        {
//            System.Threading.Thread clt = new System.Threading.Thread(Clt);
//            System.Threading.Thread svr = new System.Threading.Thread(connect);
//            svr.Start();
//            System.Threading.Thread.Sleep(1000);
//            clt.Start();
//            clt.Join();
//            svr.Join();
//        }
//        static void connect()
//        {
//            //File.Create("../../test/connect.log");
//            UdpServer us = new UdpServer();
//            us.ServerOn();
//        }
//        static void Clt()
//        {
//            var Ct = new CidsClient("1234567", "127.0.0.1", false);
//            Ct.SendMain();
//            var json = Ct.SendFirstMirror();
//            File.WriteAllText("../../test/connect.log", $"time:{json.Time}");
//        }
//        static void SendRecv()
//        {
//            var Ct = new CidsClient("1234567", "192.168.233.14");
//            Ct.SendMain();
//            var json = Ct.SendFirstMirror();
//        }
//        static void IpParse()
//        {
//            byte[] getip = new byte[4] { 127, 0, 0, 1 };
//            string ip = String.Join(".", getip);
//            Console.WriteLine(ip);
//        }
//        static void OctBytesTest()
//        {
//            String id = "1234567";
//            byte[] s7 = ClientTool.GetOctByte(ref id);
//            for (int i = 0; i < 7; ++i)
//            {
//                Console.WriteLine($"s7[${i}]:{s7[i]}");
//            }
//            id = "9876543";
//            byte[] g987654 = ClientTool.GetOctByte(ref id);
//            byte[] b987654 = new byte[8] { 9, 8, 7, 6, 5, 4, 3, 0 };
//            for (int i = 0; i < 8; ++i)
//            {
//                if (g987654[i] != b987654[i])
//                {
//                    Console.WriteLine($"bytes equals at{i}");
//                    break;
//                }
//            }
//            Console.WriteLine(g987654 == b987654);
//        }
//        static void t1()
//        {
//            String jsonfile = File.ReadAllText("../../test/json/recv.json");
//            Json.MirrorReceive mr = JsonConvert.DeserializeObject<Json.MirrorReceive>(jsonfile);
//            if (mr != null)
//            {
//                mr = JsonConvert.DeserializeObject<Json.MirrorReceive>(JsonConvert.SerializeObject(mr));
//                Console.WriteLine(JsonConvert.SerializeObject(mr));
//            }
//            else Console.WriteLine("Null Object");
//        }
//    }
//}
