using System;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Client
{
	public class ClientTool
    {
		public const int ToMainRequestLength = 8;
		public const int DelayBetweenSending = 100; // milli seconds
		public const int HeartBeatTimeGap = 1000; // 心跳包时间间隔 milli seconds
		public static readonly Random random = new Random((int)DateTime.Now.Ticks);
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
		// 摘要
		//	返回一个 3-5 秒 的随机睡眠时间
		public static int SendGapTime => NextInt % 2000 + 3000;
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
		public  String ReSendMain()
        {
			return SendMain(1);
        }
		// 摘要
		//	给主服务器发UDP包直至获得镜像服务器IP
		// 参数 
		//	InitSendTime:2--Default
		//		测试使用:更改默认值
		//		正常使用:使用默认值
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
            // SendTimes(Gram,ClientTool.ToMainRequestLength, remote);

			do{ // send until receive
				if (Test){
					Console.WriteLine($"\nSend for Mirror IP {SendTime} times");
				}
				// Gram[7] equals 0
				SendTimes(Gram, ClientTool.ToMainRequestLength, remote); // The first send
				System.Threading.Thread.Sleep(ClientTool.SendGapTime); // 3-5 seconds
				Gram[7] = SendTime; // time increase
				if (SendTime !=1 && SendTime != byte.MaxValue - 1) // limit 254 and lock 1
				{
					++SendTime;
				}
			} while (System.Threading.Interlocked.Equals(GetMirrorIp,0));
			return MirrorIP;
		}
		// 摘要
		//	给镜像服务器发送UDP包直至获取镜像服务器JSON
		//	发送 ASCII 字节流 收取 UTF8 字节流
		// 异常
		//	
		private Json.MirrorReceive SendMirror(int SleepTimeMilli,bool MustGet=true)
        {
			//Client.Connect(IPAddress.Parse(MirrorIP), MirrorPort); 
			Json.MirrorReceive RecvJson=null;
			IPEndPoint remote = new IPEndPoint(IPAddress.Parse(MirrorIP), MirrorPort); // mirror remote
			int success = 0;
            #region a timer need to recv in  a limited time
			System.Threading.Tasks.Task task = null;
            // get the update msg
			#region A Task for Udp Recv LOOP until get json
			task=System.Threading.Tasks.Task.Factory.StartNew(() =>
            #region Task
            { // endless block and wait
                if (Test){
					Console.WriteLine("Init Task to Get Update Information");
				}
				// get Mirror Response
				// get Update Information
				byte[] JsonText; // Content Received
				do // receive and judge if it's the main response
				{
					JsonText = Client.Receive(ref remote);
				} while (JsonText.Length == 4); // throw the extra Ip packages
				// convert to string
				String MRecv = System.Text.Encoding.UTF8.GetString(JsonText); // Recv UTF8 String
				RecvJson = JsonConvert.DeserializeObject<Json.MirrorReceive>(MRecv);
				if (Test)
				{
					Console.WriteLine("Get Update Information\nJson:\n");
					Console.WriteLine(MRecv);
				}
				System.Threading.Interlocked.Increment(ref success);
				if (RecvJson.NeedUpdate) // update time if  need
				{
					lastTime = RecvJson.Time;
				}
			}
            #endregion// end of task
            );
            #endregion// Recv Task
            
            #endregion// end of a timer need
            String StoMirror = ClientTool.ComposeMirrorRequest(uuid, lastTime); // the MSG will be sent to mirror
			byte[] JsonBytes = System.Text.Encoding.ASCII.GetBytes(StoMirror);


			// send request
			do{
				// Sleep Set
				SendTimes(JsonBytes,JsonBytes.Length,remote);
				System.Threading.Thread.Sleep(SleepTimeMilli);
            } while (System.Threading.Interlocked.Equals(success, 0)&&MustGet);
			if (false == MustGet) // Wait half second and check
			{
				if (task.Wait(SleepTimeMilli >> 1)) { // complete

				}
				//System.Threading.Thread.Sleep(SleepTimeMilli >> 1);
			}
			return RecvJson;
		}
		public Json.MirrorReceive SendFirstMirror() {
			return SendMirror(ClientTool.SendGapTime); // sleep 3-5 seconds each gap
		}
		// 摘要
		//	发送心跳包 检查Mirror是否存活 需要外部计时计数
		// 返回
		//	是否需要更新壁纸
		public bool HeartBeat(ref Json.MirrorReceive data)
        {
			Json.MirrorReceive receive = SendMirror(ClientTool.HeartBeatTimeGap,false);
            if (receive.NeedUpdate)
            {
				data = receive;
            }
			return receive.NeedUpdate;
        }
        #region maybe need
        public void todo() {
			int success = 0;
			#region A Timer to check if mirror lives
			System.Threading.Tasks.Task.Factory.StartNew(() => { // Exception may be thrown
				DateTime begin = DateTime.Now;
				DateTime shouldEndBefore = DateTime.Now.AddMinutes(1);
				do
				{

					System.Threading.Thread.Sleep(2000); // check every 2 seconds
				} while (System.Threading.Interlocked.Equals(success, 0));
			});
			#endregion// Task for Timer
		}
        #endregion
    }
}
