using System;
using System.IO;
using Newtonsoft.Json;


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
            //Test.UdpTest.ClientCenterOnly();
            //Test.UdpTest.TcpTimeOutTest();
            //Test.UdpTest.ClientRealTest();
            //Test.UdpTest.ClientMirrorOnly();
            Test.UdpTest.DLoadTest();
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
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
        static void write(string content)
        {
            Console.WriteLine(content);
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
