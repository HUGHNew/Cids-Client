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
		public static int SendGapTime => NextInt % ConfData.SleepMin + ConfData.SleepMax;
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
			catch (Exception)
			{
				return false;
			}
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
		#region private property
		private readonly UdpClient Client;
        private String lastTime=null,MirrorIP=null;
		private readonly bool Test = false;
        #endregion

        #region public property
		public static  String UuId => ConfData.UuId;
		public const int DefaultPackageNumber = 10;
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
        public CidsClient(String uuid,String Server,bool test=true) // for test
        {
			MainServer = IPAddress.Parse(Server);
			//Client = new UdpClient(new IPEndPoint(IPAddress.Any,65500)); // something may err here
			Client = new UdpClient();
			this.Test = test;
        }
        public CidsClient(String server=null)
        {
			MainServer = server == null ? ConfData.DefaultMServer : IPAddress.Parse(server);
			Client = new UdpClient();
			//Client.Connect(MainServer, MainPort);
		}
        #endregion
		public void DownLoadFail()
        {
			lastTime = null;
        }
        private void SendTimes(byte[]data,int bytes,IPEndPoint end,int times= DefaultPackageNumber)
        {
			for(int i = 0; i < times; ++i)
            {
				Client.Send(data, bytes, end);
				System.Threading.Thread.Sleep(ConfData.SendDelayTime);
            }
        }
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
			byte[] Gram = ClientTool.GetOctByte(ConfData.UuId);
			IPEndPoint remote = new IPEndPoint(MainServer, ConfData.MainPort);
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
			IPEndPoint remote = new IPEndPoint(IPAddress.Parse(MirrorIP), ConfData.MirrorPort); // mirror remote
			int success = 0;
            #region a timer need to recv in  a limited time
            // get the update msg
			#region A Task for Udp Recv LOOP until get json
			System.Threading.Tasks.Task task = 
				System.Threading.Tasks.Task.Factory.StartNew(() =>
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
				RecvJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Json.MirrorReceive>(MRecv);
				if (Test)
				{
					Console.WriteLine("Get Update Information\nJson:\n");
					Console.WriteLine(MRecv);
				}
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
            String StoMirror = ClientTool.ComposeMirrorRequest(ConfData.UuId, lastTime); // the MSG will be sent to mirror
			byte[] JsonBytes = System.Text.Encoding.ASCII.GetBytes(StoMirror);


			// send request
			do{
				// Sleep Set
				SendTimes(JsonBytes,JsonBytes.Length,remote);
				System.Threading.Thread.Sleep(SleepTimeMilli);
            } while (System.Threading.Interlocked.Equals(success, 0)&&MustGet);
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
		public Json.MirrorReceive SendFirstMirror() {
			return SendMirror(ClientTool.SendGapTime); // sleep 3-5 seconds each gap
		}
		// 摘要
		//	发送心跳包 检查Mirror是否存活 需要外部计时计数
		// 返回
		//	是否需要更新壁纸
		public bool HeartBeat(ref Json.MirrorReceive data)
        {
			Json.MirrorReceive receive = SendMirror(ConfData.HeartBeatGap,false);
            if (null == receive) // not recv
            {
				return false;
            }
            if (receive.NeedUpdate)
            {
				data = receive;
            }
			return receive.NeedUpdate;
        }
		// 摘要
		//	对于心跳包的次数包装
		// 返回
		//	是否在限时内获取Mirror的数据包
		//	如果为否 则需要再次向 Main 申请 Ip
		public bool LimitedHeartBeat(ref Json.MirrorReceive data,int LimitTimes) {
			do
			{
				if (HeartBeat(ref data))
				{
					return true;
				}
			} while (--LimitTimes != 0);
			return false;
		}
		public bool LimitedHeartBeat(ref Json.MirrorReceive data)
        {
			return LimitedHeartBeat(ref data, ConfData.MirrorRecvLimit);
        }
		// 摘要
		//	第一次连接 发心跳包 有问题重发 解决后续所有问题
		public static void UdpClientBeat(CidsClient client,ref Json.MirrorReceive data)
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
    }
}
