using System;
using System.IO;
using Newtonsoft.Json;

//static class Temp {
//    static void Main() {
//        string test = "{\"Update\":";
//        Tmp t=null;
//        try { 
//            t = JsonConvert.DeserializeObject<Tmp>(test);
//        }
//        catch (Exception)
//        {

//        }
//        if (null == t)
//        {
//            Console.WriteLine("null");
//        }
//    }
//}

//class Tmp { 
//    public bool Update { get; set; }
//}

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
            halfpack();
            //emptyList();
            //imgt0();
            //JsonTest.newlytest();
            //JsonTest.test();
            //SendRecv();
            //t2();
            //OctBytesTest();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
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
        static void RegisrtyTest()
        {
            Init.Configuration();
        }
        static void scatter(string field = "TMP")
        {
            Console.WriteLine((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User)[field] as string));
            Console.WriteLine((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process)[field] as string));
            Console.WriteLine((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine)[field] as string));
            //Console.WriteLine(Path.Combine("dir","file"));
        }
        static void emptyList()
        {
            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();
            foreach (string str in ls)
            {
                Console.WriteLine(str);
            }
        }
        static void imgt0()
        {
            ImgTest.t0();
        }
        static void localCS()
        {
            System.Threading.Thread clt = new System.Threading.Thread(Clt);
            System.Threading.Thread svr = new System.Threading.Thread(connect);
            svr.Start();
            System.Threading.Thread.Sleep(1000);
            clt.Start();
            clt.Join();
            svr.Join();
        }
        static void connect()
        {
            //File.Create("../../test/connect.log");
            UdpServer us = new UdpServer();
            us.ServerOn();
        }
        static void Clt()
        {
            var Ct = new CidsClient("1234567", "127.0.0.1", false);
            Ct.SendMain();
            var json = Ct.SendFirstMirror();
            File.WriteAllText("../../test/connect.log", $"time:{json.Time}");
        }
        static void SendRecv()
        {
            var Ct = new CidsClient("1234567", "192.168.233.14");
            Ct.SendMain();
            var json = Ct.SendFirstMirror();
        }
        static void IpParse()
        {
            byte[] getip = new byte[4] { 127, 0, 0, 1 };
            string ip = String.Join(".", getip);
            Console.WriteLine(ip);
        }
        static void OctBytesTest()
        {
            String id = "1234567";
            byte[] s7 = ClientTool.GetOctByte(ref id);
            for (int i = 0; i < 7; ++i)
            {
                Console.WriteLine($"s7[${i}]:{s7[i]}");
            }
            id = "9876543";
            byte[] g987654 = ClientTool.GetOctByte(ref id);
            byte[] b987654 = new byte[8] { 9, 8, 7, 6, 5, 4, 3, 0 };
            for (int i = 0; i < 8; ++i)
            {
                if (g987654[i] != b987654[i])
                {
                    Console.WriteLine($"bytes equals at{i}");
                    break;
                }
            }
            Console.WriteLine(g987654 == b987654);
        }
        static void t1()
        {
            String jsonfile = File.ReadAllText("../../test/json/recv.json");
            Json.MirrorReceive mr = JsonConvert.DeserializeObject<Json.MirrorReceive>(jsonfile);
            if (mr != null)
            {
                mr = JsonConvert.DeserializeObject<Json.MirrorReceive>(JsonConvert.SerializeObject(mr));
                Console.WriteLine(JsonConvert.SerializeObject(mr));
            }
            else Console.WriteLine("Null Object");
        }
    }
}
