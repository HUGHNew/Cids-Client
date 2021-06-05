#define Heart
using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Client.Test
{
    // test class
    class UdpTest
    {
        public static readonly string Tmp = System.IO.Path.Combine(
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Temp"),
            "img");
        [Obsolete]
        public static void Server()
        {
            UdpServer server = new UdpServer();
            server.ServerOn();
        }
        public static void ClientCenterOnly()
        {
            CidsClient client = new CidsClient("7123456", "192.168.233.14");
            string ip=client.SendMain();
            Console.WriteLine(ip);
        }
        public static void Client()
        {
            CidsClient client = new CidsClient("7123456", "127.0.0.1");
            client.SendMain();
            var json=client.SendFirstMirror();
            //CidsClient.UdpClientBeat(client,ref json);
            int beats = 0;
            if(client.HeartBeat(ref json))
            {
                Console.WriteLine(beats+++" Times HeatBeat");
            }
        }
        public static bool DLoadTest()
        {
            Console.WriteLine("Path:"+Tmp);
            return ClientTool.DownloadAbsFile("http://192.168.233.14:80/3.jpg", Tmp+"\\tmp.jpg");
        }
    }
    class UdpServer
    {
        private string Ip;
        private int Port;
        private IPEndPoint sender;


        public const string mainlogfile = "../../test/main.log";
        public const string mirrorlogfile = "../../test/mirror.log";
        public const string localhost = "127.0.0.1";
        private static readonly StreamWriter mainlog= new StreamWriter(mainlogfile) {
            AutoFlush = true
        };
        private static readonly StreamWriter mirrorlog = new StreamWriter(mirrorlogfile)
        {
            AutoFlush = true
        };
        public static IPEndPoint end= new IPEndPoint(IPAddress.Any, 0);
        public static readonly UdpClient main = new UdpClient(20800);
        public static readonly UdpClient mirror= new UdpClient(20801);
        public UdpServer(int port = 20800, String Ip = "127.0.0.1")
        {
            this.Ip = Ip;
            this.Port = port;
            sender = new IPEndPoint(IPAddress.Any, 0);
        }
        public void ServerOn()
        {
            //IPEndPoint local = new IPEndPoint(IPAddress.Parse(Ip), Port);
            //UdpClient newsock = new UdpClient(local);
            UdpClient newsock = new UdpClient(20800);
            //Main(ref newsock);

            //Mirror();
            //HB();
        }
        public static void MainServer() {
            StreamWriter stream = mainlog;
            UdpClient udp = main;
            udp.Receive(ref end); // request from client for mirror
            stream.WriteLine(end.Port);
            byte[] loop = { 127, 0, 0, 1 };
            stream.WriteLine("Main Starts");
#if DEBUG
            
            stream.WriteLine("Message received from {0}:", end.ToString());
#endif
            for (int i = 0; i < 10; ++i)
            {
                udp.Send(loop, 4, end);
            }
            stream.WriteLine("Main Ends");
            //System.Threading.Thread.Sleep(1000);
            //udp.Close();
        }
        public static void MirrorServer()
        {
            StreamWriter stream = mirrorlog;
            stream.WriteLine("Mirror Begins");
            byte[] data=null;
            UdpClient udp = mirror;
            while (end.Port == 0)
            {
                System.Threading.Thread.Sleep(200);
            }
            while (data==null||data?.Length < 10)
            {
                data = udp.Receive(ref end);
#if DEBUG
                stream.WriteLine($"Receive:{Encoding.ASCII.GetString(data)}");
#endif
            }
            stream.WriteLine("Message Send To {0}:", end.ToString());
            string json = System.IO.File.ReadAllText("../../test/json/emptyMsg.json");
            data = Encoding.UTF8.GetBytes(json);
            //MessageBox.Show("Click to Sent Json","Server");
            udp.Send(data,data.Length,end);
        }
        public static void HB()
        {
            byte[] data = null;
            UdpClient udp = mirror;
#if Heart
            while (data == null)
            {
#endif
                while (data == null)
                {
                    data = udp.Receive(ref end);
    #if DEBUG
                    mirrorlog.WriteLine($"HeartBeat:{Encoding.ASCII.GetString(data)}");
    #endif
                }
                string json = System.IO.File.ReadAllText("../../test/json/NoUpdate.json");
                data = Encoding.UTF8.GetBytes(json);
                udp.Send(data, data.Length, end);
#if Heart
                data = null;
            }
#endif
        }
    }
}