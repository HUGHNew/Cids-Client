using System;
using System.Net.Sockets;
using System.Net;

namespace Client
{
	public class ClientFunc
	{
		private UdpClient Client;
		public const int MainPort= 20800;
		public readonly IPAddress MainServer = IPAddress.Parse(""); // Server IP need
		private String lastTime,MirrorIP;
		//public Client
		// ctor
		public void Startup()
        {
            //Client.Send
        }
		
	}
}
