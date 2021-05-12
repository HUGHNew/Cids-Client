using System;
using System.Net.Sockets;
using System.Net;

namespace Client
{
	public class UDPsocket
	{
		public static String url;
		private Socket client;
		static public String ip;
		static public int port;
		public UDPsocket()
		{
			client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);// udp
			client.Bind(new IPEndPoint(IPAddress.Parse(ip), port));//绑定端口号和IP
		}
		public UDPsocket startup() {
			//client.SendTo();
			return this;
		}
	}
}
