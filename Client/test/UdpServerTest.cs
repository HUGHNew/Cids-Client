#define Test
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client.Test
{
    // test class
    class UdpTest
    {
        public static void Server()
        {
            UdpServer server = new UdpServer();
            server.ServerOn();
        }
        public static void Client()
        {
            CidsClient client = new CidsClient("7123456", "127.0.0.1");
            client.SendMain();
            var json=client.SendFirstMirror();
            CidsClient.UdpClientBeat(client,ref json);
        }
    }
    class UdpServer
    {
        private string Ip;
        private int Port;
        private IPEndPoint sender;
        public UdpServer(int port = 20800, String Ip = "127.0.0.1")
        {
            this.Ip = Ip;
            this.Port = port;
            sender = new IPEndPoint(IPAddress.Any, 0);
        }
        public void ServerOn()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Port);
            UdpClient newsock = new UdpClient(ipep);

            RecvMain(ref newsock);

            RecvMirror();
        }
        public void RecvMain(ref UdpClient udp) {
            udp.Receive(ref sender); // request from client for mirror
            byte[] loop = { 127, 0, 0, 1 };
#if Test
            Console.WriteLine("Message received from {0}:", sender.ToString());
#endif
            for (int i = 0; i < 10; ++i)
            {
                udp.Send(loop, 4, sender);
            }
        }
        public void RecvMirror()
        {
            byte[] data=null;
            UdpClient udp=new UdpClient(new IPEndPoint(IPAddress.Any, 20801));
            while (data==null||data?.Length < 10)
            {
                data = udp.Receive(ref sender);
#if Test
                Console.WriteLine($"Receive:{Encoding.ASCII.GetString(data)}");
#endif
            }
            Console.WriteLine("Message Send To {0}:", sender.ToString());
            string json = System.IO.File.ReadAllText("../../test/json/emptyMsg.json");
            data = Encoding.UTF8.GetBytes(json);
            MessageBox.Show("Click to Sent Json","Server");
            udp.Send(data,data.Length,sender);
        }
        public void HB()
        {

            byte[] data = null;
            UdpClient udp = new UdpClient(new IPEndPoint(IPAddress.Any, 20801));
            while (data == null)
            {
                data = udp.Receive(ref sender);
#if Test
                Console.WriteLine($"HeartBeat:{Encoding.ASCII.GetString(data)}");
#endif
            }
            string json = System.IO.File.ReadAllText("../../test/json/NoUpdate.json");
            data = Encoding.UTF8.GetBytes(json);
            udp.Send(data, data.Length, sender);
        }
    }
}
