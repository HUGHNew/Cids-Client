//#define Connect
#define TCPMirror
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using Client.Data;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Client.Image;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Client
{
	public class ClientTool
    {
		public const int ToMainRequestLength = 8;
		public const int Second = 1000;
		// Timeout, millsec
		public const int time_out = 10000;
		// Interval, millsec
		public const int interval = 2000;
		public static readonly Random random = new Random((int)DateTime.Now.Ticks);
        public static int NextInt => ClientTool.random.Next();
        public static byte[] GetOctByte(String id) {
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
		//	返回一个 指定间隔 的随机睡眠时间
		//	min - max
#if DEBUG
		public static int SendGapTime => NextInt % (ConfData.SleepMin==0?2000: ConfData.SleepMin) 
			+ (ConfData.SleepMax == 0 ? 3000 : ConfData.SleepMax);
#else
		public static int SendGapTime => NextInt % ConfData.SleepMin + ConfData.SleepMax;
#endif
		#region Update And Set Wallpaper All the useful function
		const int SPI_SETDESKWALLPAPER = 20;
		const int SPIF_UPDATEINIFILE = 0x01;
		const int SPIF_SENDWININICHANGE = 0x02;
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni); // for Wallpaper Set
		public enum Style : int
		{
			Tiled,
			Centered,
			Stretched
		}
		public static void SetWallpaper(Style style=Style.Stretched)
		{
			string fileName = Path.Combine(ConfData.CidsImagePath, Image.ImageConf.ToSetWallFile());
			ClientTool.SetWallpaper(fileName, style);
		}
		public static void SetWallpaper(string strSavePath, Style style)
		{
#region Set Wall
			RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true); // get the key of desk wallpaper
			if (style == Style.Stretched)
			{
				key.SetValue(@"WallpaperStyle", 2.ToString());
				key.SetValue(@"TileWallpaper", 0.ToString());
			}
			if (style == Style.Centered)
			{
				key.SetValue(@"WallpaperStyle", 1.ToString());
				key.SetValue(@"TileWallpaper", 0.ToString());
			}
			if (style == Style.Tiled)
			{
				key.SetValue(@"WallpaperStyle", 1.ToString());
				key.SetValue(@"TileWallpaper", 1.ToString());
			}
			SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, strSavePath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
#endregion
		}
#region Download and Set Wallpaper
		private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
		// 摘要
		//  从 URL 下载文件到 filename 中
		// 参数
		//  filename 相对路径文件名
		//      前缀为 CidsImagePath
		//      默认值为 SaveFile
		public static bool DownloadFile(string URL, string filename = ConfData.SaveFile)
		{
			return DownloadAbsFile(URL, Path.Combine(ConfData.CidsImagePath, filename));
		}
		// 摘要
		//  从 URL 下载文件到 filename 中
		// 参数
		//  filename 绝对路径文件名
		public static bool DownloadAbsFile(string URL, string filename)
		{
			string tmp = Path.GetTempFileName();
			try
			{
				//File.Delete(filename);
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
				HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(URL);
				HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
				Stream st = myrp.GetResponseStream();
				Stream so = new FileStream(tmp, FileMode.Create);
				long totalDownloadedByte = 0;
				byte[] by = new byte[1024];
				int osize = st.Read(by, 0, by.Length);
				while (osize > 0)
				{
					totalDownloadedByte = osize + totalDownloadedByte;
					so.Write(by, 0, osize);
					osize = st.Read(by, 0, by.Length);
				}
				so.Close();
				st.Close();
			}
			catch (Exception e)
			{
#if DEBUG
				Console.WriteLine(e.Message);
#endif
				return false;
			}
			if (File.Exists(filename)) File.Delete(filename);
			File.Move(tmp, filename);
			return true;
		}
		public static bool TryDownload(ref CidsClient UdpClient, ref Json.MirrorReceive data, int time_out, int interval, int limit = 30)
		{
			// get wallpaper file
			// An Absolute One
			string wallpaperPath = Data.ConfData.SaveAbsPathFile;

			// get json
			data = UdpClient.SendFirstMirror();
			String ImgUrl = data.Image_url;
#region Download File
			// Set Attempt Limit
			var tokenSource = new CancellationTokenSource();
			CancellationToken token = tokenSource.Token;
			var task = Task.Factory.StartNew(() => DownloadAbsFile(ImgUrl, wallpaperPath), token); // download
			if (!task.Wait(time_out * limit, token) || !task.Result) // timed out
			{
				UdpClient.DownLoadFail();
				Thread.Sleep(interval);
			}
#endregion//download
			return true;
		}
		public static bool Update(ref Json.MirrorReceive data)
		{
			Operation.GraphicsCompose(data);
			SetWallpaper();
			return true;
		}
#endregion
		#endregion//Migrate to here
    }
	public class CidsClient
	{
		public enum MirrorProtocol:int{
			Udp=1,Tcp,Quic
		}
		#region private property
		private readonly UdpClient Client = new UdpClient();
        private String lastTime=null,MirrorIP=null;
		private MirrorProtocol Protocol;

		private TcpClient TcpMirror=null;
		private System.Text.StringBuilder Last;
		private int bracket=0;
		#endregion
#if DEBUG
		private string id_test = null;
#endif

		#region public property
		public static String UuId => ConfData.UuId;
		public const int DefaultPackageNumber = 10;
		public const int MirrorEmptyRate = 100;
#endregion
        public String Mirror => MirrorIP;
		public IPAddress MainServer;

#region initilization uuid file or get uuid from conf file
		static public bool ConfCheck()
        {
			// Impossible to create or change file here
			return System.IO.File.Exists(Init.ConfFile);
        }
#endregion
#region constructor

		// 摘要
		//	专用于测试的构造函数
		public CidsClient(String uuid,String Server) // for test
        {
			//Client = new UdpClient(new IPEndPoint(IPAddress.Any,65500)); // something may err here
			CidsClientInit(Server);
			this.id_test = uuid;
        }
        #region Init Part of UdpClient
        private void CidsClientInit()
        {
			CidsClientInit(ConfData.DefaultMServer);
        }
		private void CidsClientInit(String server)
		{
			CidsClientInit(IPAddress.Parse(server));
		}
		private void CidsClientInit(IPAddress server)
		{
			MainServer = server;
			Protocol = (MirrorProtocol)(ConfData.MirrorProtocol);
#if Connect
			Client.Connect(MainServer, ConfData.MainPort);
#endif
		}
#endregion
        public CidsClient()
		{
			CidsClientInit();
		}
		public CidsClient(String server)
        {
			CidsClientInit(server == null ? ConfData.DefaultMServer : IPAddress.Parse(server));
		}
#endregion
		public void DownLoadFail()
        {
			lastTime = null;
        }
        private void SendTimes(byte[]data,int bytes,IPEndPoint end=null,int times= DefaultPackageNumber)
        {
			for(int i = 0; i < times; ++i)
            {
				if (end == null) // Think it is Connected
				{
					Client.Send(data, bytes);
				}
				else { 
					Client.Send(data, bytes, end);
				}
				System.Threading.Thread.Sleep(ConfData.SendDelayTime);
            }
        }
        #region Main Communications
        // 摘要
        //	重发数据报给 MainServer 并告知服务器重发原因
        // 返回
        //	Mirror IP
        public String ReSendMain()
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
			string id;
			int mainPort;
#if DEBUG

			id = id_test;
			mainPort = 20800 ;
#else
			id=ConfData.UuId;
			mainPort = ConfData.MainPort;
#endif
			byte[] Gram = ClientTool.GetOctByte(id);
#if Connect
			//IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
			IPEndPoint remote = new IPEndPoint(MainServer, mainPort);
#else
			IPEndPoint remote = new IPEndPoint(MainServer, mainPort);
#endif
			byte SendTime = InitSendTime;

			// Recv Information
			System.Threading.Tasks.Task.Factory.StartNew(()=> { // endless block and wait
				Thread.Sleep(100);
#if DEBUG
				Console.WriteLine("Init Task to Get Mirror Ip");
#endif
				// get MainServer Response
				// get mirror ip
				byte[] getip=Client.Receive(ref remote);
				MirrorIP = String.Join(".", getip); // It's OK
				System.Threading.Interlocked.Increment(ref GetMirrorIp);
			});
            // SendTimes(Gram,ClientTool.ToMainRequestLength, remote);

			do{ // send until receive
#if DEBUG
				Console.WriteLine($"\nSend for Mirror IP {SendTime} times");
#endif
				// Gram[7] equals 0
#if Connect
				SendTimes(Gram, ClientTool.ToMainRequestLength); // The first send
#else
				SendTimes(Gram, ClientTool.ToMainRequestLength,remote); // The first send
#endif
				System.Threading.Thread.Sleep(ClientTool.SendGapTime); // 3-5 seconds
				Gram[7] = SendTime; // time increase
				if (SendTime !=1 && SendTime != byte.MaxValue - 1) // limit 254 and lock 1
				{
					++SendTime;
				}
			} while (System.Threading.Interlocked.Equals(GetMirrorIp,0));
#if Connect
			int port;
#if DEBUG
			port = 20801;
#else
			port=ConfData.MirrorPort;
#endif
			Client.Close();
			Client.Connect(IPAddress.Parse(MirrorIP), port);
#endif
            switch (Protocol)
            {
				case MirrorProtocol.Udp:
					break;
				case MirrorProtocol.Quic:
					break;
				case MirrorProtocol.Tcp:
		#if DEBUG
					TcpMirror = new TcpClient(MirrorIP, 20801);
		#else
					TcpMirror = new TcpClient(MirrorIP, ConfData.MirrorPort);
		#endif// Debug
					break;
			}
			return MirrorIP;
		}
        #endregion//Main Com
#if DEBUG
        public CidsClient SetMirrorIp(String ip)
        {
			MirrorIP = ip;
            if (Protocol == MirrorProtocol.Tcp)
            {
				TcpMirror.Close();
				TcpMirror = new TcpClient(MirrorIP, 20801);
			}
			return this;
        }
#endif
		#region Mirror Communication
		private Json.MirrorReceive SendMirror(int SleepTimeMilli, bool MustGet = true)
        {
			switch (Protocol) {
				case MirrorProtocol.Udp:
					return UdpSendMirror(SleepTimeMilli,MustGet);
				case MirrorProtocol.Tcp:
					return TcpSendMirror(SleepTimeMilli,MustGet);
				case MirrorProtocol.Quic:
				default:
					return null;
			}
        }
		public Json.MirrorReceive SendFirstMirror() {
			return SendMirror(ClientTool.SendGapTime); // sleep 3-5 seconds each gap
		}
		// 摘要:
		//		发送心跳包 检查Mirror是否存活 需要外部计时计数
		// 返回:
		//		收到信息状态
		//		0 -- 没收到
		//		1 -- 当前没有更新或半包
		//		2 -- 更新
		public int HeartBeat(ref Json.MirrorReceive data)
        {
			int bracketBeforeSend = bracket;
			Json.MirrorReceive receive = SendMirror(ConfData.HeartBeatGap,false);
            if (null == receive) // not recv
            {
				if (bracketBeforeSend == bracket)
				{
					return 0;
				}
				else return 1;
            }
            if (receive.NeedUpdate)
            {
				data = receive;
				return 2;
            }
			return 1;
        }
		//	摘要:
		//		对于心跳包的次数包装
		//	返回:
		//		是否在限时内获取Mirror的数据包
		//		如果为否 则需要再次向 Main 申请 Ip
		public bool LimitedHeartBeat(ref Json.MirrorReceive data,uint LimitTimes) {
			uint counter = LimitTimes;
			do
			{
				switch(HeartBeat(ref data)) // Mirror 存活
				{
					case 2: // Update
						return true;
					case 1: // Exist
						counter = LimitTimes;
						break;
					default:break;// No Response
				}
			} while (--counter != 0);
			return false;
		}
		public bool LimitedHeartBeat(ref Json.MirrorReceive data)
        {
			return LimitedHeartBeat(ref data, (uint)ConfData.MirrorRecvLimit);
        }
		// 摘要
		//	第一次连接 发心跳包 有问题重发 解决后续所有问题
		public static void ClientBeat(CidsClient client,ref Json.MirrorReceive data)
        {
            while (true)
            {
				while (client.LimitedHeartBeat(ref data))
				{
					// update information
					if (data.Image_url != "")
					{
						ClientTool.TryDownload(ref client,ref data,
							ClientTool.time_out, ClientTool.interval);
						ClientTool.SetWallpaper();
					}
					ClientTool.Update(ref data);
				}
				client.ReSendMain(); // Get A New Mirror In Case The Old One is Down
            }
        }
		#endregion
		#region Udp Protocol Mirror
		//
		// 摘要:
		//		给镜像服务器发送UDP包直至获取镜像服务器JSON
		//		发送 ASCII 字节流 收取 UTF8 字节流
		//		对于心跳包类型 只发送一批数据包
		//		对于MustGet循环发送 并等待获取
		//
		private Json.MirrorReceive UdpSendMirror(int SleepTimeMilli,bool MustGet=true)
        {
			Json.MirrorReceive RecvJson=null;
#if Connect
			// new one for para
			IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
#else
#if DEBUG
			IPEndPoint remote = new IPEndPoint(IPAddress.Parse(MirrorIP), 20801); // mirror remote
#else
			IPEndPoint remote = new IPEndPoint(IPAddress.Parse(MirrorIP), ConfData.MirrorPort); // mirror remote
#endif//DEBUG
#endif//Connect
			int success = 0;
#region a timer need to recv in  a limited time
            // get the update msg
#region A Task for Udp Recv LOOP until get json
			System.Threading.Tasks.Task task = 
				System.Threading.Tasks.Task.Factory.StartNew(() =>
#region Task
            { // endless block and wait
#if DEBUG
				Console.WriteLine("Init Task to Get Update Information");
#endif
				// get Mirror Response
				// get Update Information
				byte[] JsonText; // Content Received

				do // receive and judge if it's the main response
				{
					JsonText = Client.Receive(ref remote);
#if DEBUG
					//foreach(var it in JsonText)
     //               {
					//	Console.Write(it);
     //               }Console.WriteLine();
#endif
				} while (JsonText.Length == 4); // throw the extra Ip packages
				// convert to string
				String MRecv = System.Text.Encoding.UTF8.GetString(JsonText); // Recv UTF8 String
				RecvJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Json.MirrorReceive>(MRecv);
#if DEBUG
				Console.WriteLine("Get Update Information\nJson:\n");
				Console.WriteLine(MRecv);
#endif
				System.Threading.Interlocked.Increment(ref success); // unlock i.e. break loop
				if (RecvJson.NeedUpdate) // update time if  need
				{
					lastTime = RecvJson.Time;
				}
			}
#endregion// end of task
            );
			#endregion// Recv Task

			#endregion// end of a timer need

#if DEBUG
			string id = id_test ?? ConfData.UuId;
			String StoMirror = ClientTool.ComposeMirrorRequest(id, lastTime); // the MSG will be sent to mirror
#else
			String StoMirror = ClientTool.ComposeMirrorRequest(ConfData.UuId, lastTime); // the MSG will be sent to mirror
#endif
			byte[] JsonBytes = System.Text.Encoding.ASCII.GetBytes(StoMirror);


			// send request
			do{
				// Sleep Set
#if Connect
				SendTimes(JsonBytes,JsonBytes.Length);
#else
				SendTimes(JsonBytes,JsonBytes.Length,remote);
#endif
				Thread.Sleep(SleepTimeMilli>>1);
            } while (MustGet&&Interlocked.Equals(success, 0));

			if (false == MustGet) // Wait half second and check
			{
				//task.Wait(SleepTimeMilli >> 1);
#region HalfPack
				try { 
					if (false==task.Wait(SleepTimeMilli >> 1)) { // not complete
						// 半包可能吗?
					}
				}catch(Newtonsoft.Json.JsonSerializationException) { // just capture is ok
					//RecvJson = null;
				}
#endregion//Half package
                //System.Threading.Thread.Sleep(SleepTimeMilli >> 1);
            }
            return RecvJson;
		}
		#endregion// Udp to Mirror
		#region Tcp Protocol Mirror
		public String TcpRecvJson(ref TcpClient tcp)
        {
			return TcpRecvJson(tcp.GetStream());
        }
		public string TcpRecvJson(NetworkStream tcpStream)
		{
			List<byte> result = new List<byte>();
			String ResultString;
			int ch_int;
			char ch;
			// Get Json
			while (true)
			{
				ch_int = tcpStream.ReadByte();
				if (ch_int == -1)    //流结束
					break;
				else
				{
					result.Add(((byte)ch_int));
					ch = (char)ch_int;

					if (ch == '{' || ch == '[') {
						++bracket;
					}
					else if (ch == '}' || ch == ']'){ //这里括号一定是匹配出现的
						--bracket;
					}
					if (bracket == 0) {
						break;
					}
				}
			}
			ResultString = System.Text.Encoding.UTF8.GetString(result.ToArray());
			Last.Append(ResultString);
            if (bracket == 0)
            {
				ResultString = Last.ToString();
				Last.Clear();
            }
            else
            {
				ResultString = null;
            }
			return ResultString;
		}
		private Json.MirrorReceive TcpSendMirror(int SleepTimeMilli, bool MustGet = true)
        {
			Json.MirrorReceive RecvJson = null;

			TcpMirror.ReceiveTimeout = SleepTimeMilli;

			NetworkStream tcpStream= TcpMirror.GetStream();

#if DEBUG
            string id = id_test ?? ConfData.UuId;
			String StoMirror = ClientTool.ComposeMirrorRequest(id, lastTime); // the MSG will be sent to mirror
#else
			String StoMirror = ClientTool.ComposeMirrorRequest(ConfData.UuId, lastTime); // the MSG will be sent to mirror
#endif
			byte[] JsonBytes = System.Text.Encoding.ASCII.GetBytes(StoMirror);

			tcpStream.Write(JsonBytes,0,JsonBytes.Length);

			String Json=TcpRecvJson(tcpStream);
            if (Json != null) // Get A Json
            {
				try { 
					RecvJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Json.MirrorReceive>(Json);
                }
                catch (Exception)
                {
					RecvJson = null;
                }
            }

			return RecvJson;
		}
		#endregion
	}
}
