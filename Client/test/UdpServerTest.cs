using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    // test class
    class UdpTest
    {
        public static void Test()
        {

        }
    }
    class UdpServer
    {
        private string Ip;
        private int Port;
        private IPEndPoint Sender;
        public UdpServer(String Ip = "127.0.0.1", int port = 20800)
        {
            this.Ip = Ip;
            this.Port = port;
        }
        public void ServerOn()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Port);
            UdpClient newsock = new UdpClient(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            RecvMain(ref newsock,ref sender);

            RecvMirror(ref sender);
        }
        public void RecvMain(ref UdpClient udp,ref IPEndPoint sender) {
            byte[] data= udp.Receive(ref sender); // request from client for mirror
            byte[] loop = { 127, 0, 0, 1 };
            Console.WriteLine("Message received from {0}:", sender.ToString());
            for (int i = 0; i < 10; ++i)
            {
                udp.Send(loop, 4, sender);
            }
        }
        public void RecvMirror(ref IPEndPoint sender)
        {
            byte[] data=null;
            UdpClient udp=new UdpClient(new IPEndPoint(IPAddress.Any, 20801));
            while (data==null||data?.Length < 18)
            {
                data = udp.Receive(ref sender);
                Console.WriteLine($"Receive:{Encoding.ASCII.GetString(data)}");
            }
            Console.WriteLine("Message Send To {0}:", sender.ToString());
            string json = System.IO.File.ReadAllText("../../test/json/noUrl.json");
            data = Encoding.UTF8.GetBytes(json);
            Console.WriteLine("Json Stirng Sent");
            udp.Send(data,data.Length,sender);
        }
    }
}
