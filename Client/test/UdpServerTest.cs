#define Heart
using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Test
{
    // test class
    class UdpTest
    {
        public static readonly string Tmp = System.IO.Path.Combine(
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Temp"),
            "img");
        public const string localhost="127.0.0.1";

        public static void MainZeroTest()
        {
            CidsClient client = new CidsClient(uuid, localhost);
            Task.Factory.StartNew(() =>
            {
                byte[] zero = { 0,0,0,0 };
                byte[] local = { 127, 0, 0, 1 };
                UdpServer.MainServer(zero);
                Thread.Sleep(1000);
                UdpServer.MainServer(local);
            });
            client.SendMain();
            Thread.Sleep(1000);
            Console.WriteLine(client.Available);
        }
        [Obsolete]
        public static void Server()
        {
            UdpServer server = new UdpServer();
            server.ServerOn();
        }
        public const string uuid = "0000475";
        public const string testCenter = "192.168.233.14";
        public const string testMirror = "192.168.233.13";
        public static void ClientCenterOnly()
        {
            CidsClient client = new CidsClient(uuid, testCenter);
            string ip=client.SendMain();
            Console.WriteLine(ip);
            ip = client.ReSendMain();
            Console.WriteLine(ip);
        }
#if DEBUG
        public static void ClientMirrorOnly() {
            CidsClient client = new CidsClient(uuid,testMirror);
            client.SetMirrorIp(testMirror).SendFirstMirror();
        }
#endif
        public static void Client()
        {
            CidsClient client = new CidsClient(uuid, "127.0.0.1");
            client.SendMain();
            var json=client.SendFirstMirror();
            //CidsClient.UdpClientBeat(client,ref json);
            int beats = 0;
            if(client.HeartBeat(ref json)>0)
            {
                Console.WriteLine(beats+++" Times HeatBeat");
            }
        }
        public static void ClientRealTest()
        {
            string center = "192.168.233.14";
            CidsClient client = new CidsClient(uuid, center);
            Console.WriteLine("Mirror Ip:"+client.SendMain());
            var json = client.SendFirstMirror();
            //CidsClient.UdpClientBeat(client,ref json);
            Console.WriteLine("First Mirror Msg:");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(json));
            Console.WriteLine("Msg Ends");
            int beats = 0;
            Console.WriteLine("Beat Goes");
            ClientTool.SetWallpaper();
            CidsClient.ClientBeat(client,ref json);
            do
            {
                //Console.WriteLine(beats++ + " Times HeatBeat");
                if (json.NeedUpdate)
                {
                    //Console.WriteLine("url:" + json.Image_url);
                    //Console.WriteLine("当前课程"+json.Event.GetReadable().CourseTitle);
                    if(json.Image_url!=null&& json.Image_url != "")
                    {
                        DLoadTest(json.Image_url);
                        Console.WriteLine("DL new image");
                    }
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(json));
                    Message.Show.MessageShow(json.Message);
                }
                //Thread.Sleep(1000);
                //json = null;
            } while (client.HeartBeat(ref json) > 0);
            Console.WriteLine("Beat Time Out");
        }
        public static bool DLoadTest(String path)
        {
            Console.WriteLine("Path:" + Tmp);
            return ClientTool.DownloadAbsFile(path, Tmp + "\\tmp.jpg");
        }
        public static bool DLoadTest()
        {
            return DLoadTest("http://192.168.233.13:20803/images/1.jpg");
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
        public static readonly StreamWriter mainlog= new StreamWriter(mainlogfile) {
            AutoFlush = true
        };
        public static readonly StreamWriter mirrorlog = new StreamWriter(mirrorlogfile)
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
        public static void MainServer(byte[] loop)
        {
            StreamWriter stream = mainlog;
            UdpClient udp = main;
            udp.Receive(ref end); // request from client for mirror
            stream.WriteLine(end.Port);
            
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
        public static void MainServer() {
            byte[] loop = { 127, 0, 0, 1 };
            MainServer(loop);
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
    class TcpTest
    {
        public const int port = 20801;
        public const string localhost = "127.0.0.1";
        public static void TcpMSvr()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(localhost),port);
            listener.Start();
            TcpClient tcp = listener.AcceptTcpClient();
            String halfJson = "{\"needUpdate\":false";
            byte[] bhj = Charset.UTF8(halfJson);
            tcp.GetStream().Write(bhj,0,bhj.Length);
            Task.Factory.StartNew(
                () =>
                {
                    Thread.Sleep(3000);
                }
            ).Wait();
            string end = "}";
            byte[] be = Charset.UTF8(end);
            tcp.GetStream().WriteByte(be[0]);
            Thread.Sleep(3000);
            tcp.Close();
        }
        public static void TcpClt()
        {
            Task.Factory.StartNew(
                () => {
                    TcpMSvr();
                }
            );
            CidsClient client = new CidsClient("1231231", localhost);
            client.SetProtocol(CidsClient.MirrorProtocol.Tcp)
                .SetMirrorIp(localhost);
            Thread.Sleep(500);
            var json=client.SendFirstMirror();
            Console.WriteLine(json.NeedUpdate);
        }
        public static void TcpHb()
        {
            Task.Factory.StartNew(
                () => {
                    TcpMSvr();
                }
            );
            CidsClient client = new CidsClient("1231231", localhost);
            client.SetProtocol(CidsClient.MirrorProtocol.Tcp)
                .SetMirrorIp(localhost);
            Json.MirrorReceive jr=null;
            client.HeartBeat(ref jr);
            Console.WriteLine(jr == null);
        }
        public static void TcpTimeOutTest()
        {
            const int port = 20000;
            Task.Factory.StartNew(() =>
            {
                TcpListener listener = new TcpListener(IPAddress.Parse(localhost), port);
                listener.Start();
                TcpClient tcp = listener.AcceptTcpClient();
                string half = "{\"a\":7";
                byte[] bstr = Encoding.UTF8.GetBytes(half);

                tcp.GetStream().Write(bstr, 0, bstr.Length);
                Thread.Sleep(2000);
                tcp.GetStream().WriteByte(6);
            });
            TcpClient client = new TcpClient(localhost, port);
            client.ReceiveTimeout = 1000;
            byte[] json = new byte[32];
            int got = client.GetStream().Read(json, 0, json.Length);
            Console.WriteLine(Encoding.UTF8.GetString(json, 0, got));
            Console.WriteLine($"got:{got}");
            Thread.Sleep(2000);
        }
    }
}