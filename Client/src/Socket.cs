using System;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Client
{
	public class ClientTool
    {
		public const int ToMainRequestLength = 8;
		public const int DelayBetweenSending = 100; // milli seconds
		public static readonly Random random = new Random((int)System.DateTime.Now.Ticks);
        public static int NextInt => ClientTool.random.Next();
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
		public static String ComposeMirrorRequest(String id,String Last) {
			String time=Last ?? "null";
			return $"{{\"UUID\":\"{id}\",\"time\":{time}}}";
		}
		//public static String GetIp() { }
    }
	public class CidsClient
	{
		#region private property
		private readonly UdpClient Client;
        private String lastTime=null,MirrorIP=null;
		private String uuid;
		private readonly bool Test = false;
        #endregion

        #region public property
        public const int MainPort= 20800,MirrorPort=20801;
		public readonly IPAddress DefaultMServer= IPAddress.Parse("127.0.0.1"); // Server IP need
		public IPAddress MainServer;
		public const int DefaultPackageNumber = 10;
        public String Mirror => MirrorIP;
        #endregion

        #region initilization uuid file or get uuid from conf file
		static public bool ConfCheck()
        {
			// Impossible to create or change file here
			return System.IO.File.Exists(Init.ConfFile);
        }
        #endregion
        #region constructor
        public CidsClient(String uuid,String Server,bool test=true) // for test
        {
			MainServer = IPAddress.Parse(Server);
			//Client = new UdpClient(new IPEndPoint(IPAddress.Any,65500)); // something may err here
			Client = new UdpClient();
			this.uuid = uuid;
			this.Test = test;
        }
        public CidsClient(String server=null)
        {
			MainServer = server == null ? DefaultMServer : IPAddress.Parse(server);
			Client = new UdpClient();
			//Client.Connect(MainServer, MainPort);
			GetUUID();
		}
        #endregion

        private void SendTimes(byte[]data,int bytes,IPEndPoint end,int times= DefaultPackageNumber)
        {
			for(int i = 0; i < times; ++i)
            {
				Client.Send(data, bytes, end);
				System.Threading.Thread.Sleep(ClientTool.DelayBetweenSending);
            }
        }
		//
		// Exception:
		//	If any Exception occurs
		private void GetUUID()
        {
			System.IO.StreamReader Reader;
			try { 
				Reader= new System.IO.StreamReader(Init.ConfFile);
            }
            catch (Exception)
            {
				throw;
			}

			while (true)
			{
				uuid = Reader.ReadLine(); // readline from conf file
				if (uuid == null) // empty file
					break;
				if (Regex.Match(uuid, "^\\s*$").Success) // empty line
					continue;
			}
			// access here if succeed
		}

		public String SendMain(byte InitSendTime=2)
        {
			int GetMirrorIp = 0;
			byte[] Gram = ClientTool.GetOctByte(ref uuid);
			IPEndPoint remote = new IPEndPoint(MainServer, MainPort);
			byte SendTime = InitSendTime;

			// Recv Information
			System.Threading.Tasks.Task.Factory.StartNew(()=> { // endless block and wait
				if (Test){
					Console.WriteLine("Init Task to Get Mirror Ip");
				}
				// get MainServer Response
				// get mirror ip
				byte[] getip=Client.Receive(ref remote);
				MirrorIP = String.Join(".", getip); // It's OK
				System.Threading.Interlocked.Increment(ref GetMirrorIp);
			});
			//SendTimes(Gram,ClientTool.ToMainRequestLength, remote); // send first time

			int sleepTime = 0;

			do{ // send until receive
				if (Test){
					Console.WriteLine($"\nSend for Mirror IP {SendTime} times");
				}
				SendTimes(Gram, ClientTool.ToMainRequestLength, remote);
				sleepTime = ClientTool.NextInt % 2000 + 3000;
				System.Threading.Thread.Sleep(sleepTime); // 3-5 seconds
				Gram[7] = SendTime; // time increase
				if (SendTime !=1 && SendTime != byte.MaxValue - 1) // limit 254 and lock 1
				{
					++SendTime;
				}
			} while (System.Threading.Interlocked.Equals(GetMirrorIp,0));
			return MirrorIP;
		}
		public Json.MirrorReceive SendMirror()
        {
			//Client.Connect(IPAddress.Parse(MirrorIP), MirrorPort); 
			Json.MirrorReceive RecvJson=null;
			IPEndPoint remote = new IPEndPoint(IPAddress.Parse(MirrorIP), MirrorPort);
			int success = 0;

			// get the update msg
			System.Threading.Tasks.Task.Factory.StartNew(() => { // endless block and wait
				if (Test){
					Console.WriteLine("Init Task to Get Update Information");
				}
				// get Mirror Response
				// get Update Information
				byte[] JsonText=null;
                while (JsonText == null || JsonText.Length == 4) // throw the extra Ip packages
                {
					JsonText = Client.Receive(ref remote);
                }
				// convert to string
				String MRecv = System.Text.Encoding.UTF8.GetString(JsonText);
				RecvJson = JsonConvert.DeserializeObject<Json.MirrorReceive>(MRecv);
				if (Test)
				{
					Console.WriteLine("Get Update Information\nJson:\n");
					Console.WriteLine(MRecv);
				}
				System.Threading.Interlocked.Increment(ref success);
                if (RecvJson.NeedUpdate) // update needed
                {
					lastTime = RecvJson.Time;
                }
			});

			String StoMirror = ClientTool.ComposeMirrorRequest(uuid, lastTime);
			byte[] JsonBytes = System.Text.Encoding.ASCII.GetBytes(StoMirror);

			// send request
			do {
				// Sleep Set
				SendTimes(JsonBytes,JsonBytes.Length,remote);
				System.Threading.Thread.Sleep(ClientTool.NextInt % 2000 + 3000); // 3-5 seconds
            } while (System.Threading.Interlocked.Equals(success, 0));
			return RecvJson;
		}

	}
}
