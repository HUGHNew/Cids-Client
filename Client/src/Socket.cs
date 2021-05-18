using System;
using System.Net.Sockets;
using System.Net;

namespace Client
{
	public class ClientTool
    {
		public const int ToMainRequestLength = 8;
		public static byte[] GetOctByte(ref String id) {
			byte[] bid = System.Text.Encoding.ASCII.GetBytes(id);
			byte[] Gram = new byte[8];
			for (int i = 0; i < 7; ++i)
			{
                Gram[i] = (byte)(bid[i] - '0');
			}
			Gram[7] = 0; // first time value
			return Gram;
		}
    }
	public class CidsClient
	{
		private UdpClient Client;
		public const int MainPort= 20800;
		public readonly IPAddress DefaultMServer= IPAddress.Parse(""); // Server IP need
		public IPAddress MainServer;
		private String lastTime,MirrorIP;
		private String uuid;
        public CidsClient(String uuid,String server=null)
        {
			this.uuid = uuid;
			MainServer = server == null ? DefaultMServer : IPAddress.Parse(server);
        }
		private void SendTen(byte[]data,IPEndPoint end)
        {
			for(int i = 0; i < 10; ++i)
            {
				Client.Send(data, data.Length, end);
            }
        }
		public void Startup()
        {
			
        }
		public void SendMain()
        {
			byte[] Gram = ClientTool.GetOctByte(ref uuid);
			IPEndPoint remote = new IPEndPoint(MainServer, MainPort);
			byte SendTime = 2;
			System.Threading.Tasks.Task.Factory.StartNew(()=> {
				// get MainServer Response
				// get mirror ip
				byte[] getip=Client.Receive(ref remote);
				MirrorIP = String.Join(".", getip); // It's OK
			});
			SendTen(Gram, remote); // send first time

			while (true)
			{
				Gram[7] = SendTime;
				if (SendTime != byte.MaxValue - 1) // limit 254
				{
					++SendTime;
				}
			}
			// how to implement mult-thread com
			//return this;
		}

	}
}
