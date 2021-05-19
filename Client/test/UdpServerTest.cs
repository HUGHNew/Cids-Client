using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class UdpServer
    {
        public static void ServerOn(String Ip="127.0.0.1",int port=20800)
        {
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            UdpClient newsock = new UdpClient(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("Waiting for a client...");
            RecvMain(ref newsock,ref sender);

            RecvMirror(ref newsock, ref sender);
        }
        public static void RecvMain(ref UdpClient udp,ref IPEndPoint sender) {
            byte[] data;
            data = udp.Receive(ref sender); // request for mirror
            byte[] loop = { 127, 0, 0, 1 };
            Console.WriteLine("Message received from {0}:", sender.ToString());
            //for(int i = 0; i < 10; ++i)
            //{
            //}
                udp.Send(loop, 4, sender);
        }
        public static void RecvMirror(ref UdpClient udp, ref IPEndPoint sender)
        {
            byte[] data=null;
            while (data==null||data?.Length > 20)
            {
                data = udp.Receive(ref sender);
            }
            string json = System.IO.File.ReadAllText("../../test/json/noUrl.json");
            data = Encoding.UTF8.GetBytes(json);
            udp.Send(data,data.Length,sender);
        }
    }
}
