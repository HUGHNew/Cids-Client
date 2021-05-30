using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Client
{
    public partial class Form1 : Form
    {
        bool fatal = false;
        //public const string ClientTitle = "四川大学智慧教学系统壁纸同步工具";
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        private Json.MirrorReceive data;
        private CidsClient UdpClient;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni); // for Wallpaper Set
        private enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }
        private static void SetWallpaper(string strSavePath, Style style)
        {
            Bitmap myBmp = new Bitmap(strSavePath);
            string fileName = Path.GetTempFileName() + ".bmp";
            myBmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
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
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, fileName, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        public Form1()
        {
            InitializeComponent();
            Conf();
            BGWorkerMain.RunWorkerAsync();
        }
        private static void Conf()
        {
            if (Init.Configuration()==false) // failed
            {
                MessageBox.Show(Init.ClientTitle, "Cids配置文件不完整", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw new Exception(); //  terminate the whole program
            }
        }
        private String GetUrl(int choice=0){ return null; }
        private void BGWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        { 
            Thread.Sleep(3000);
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            UdpClient = new CidsClient();
            UdpClient.SendMain();
            int try_count = 10;
            // Timeout, millsec
            int time_out = 10000;
            // Interval, millsec
            int interval = 2000;
            int success = 1;
            #region Switch to endless loop to get wallpaper
            for (int c = 0; c < try_count; ++c)
            {
                if (c > 0) // ?
                    bgWorker.ReportProgress(c); // Error of communication with Mirror Server -- Timed Out
                try
                {
                    if (DownloadAndSet(time_out, interval) == false)
                    {
                        continue;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
                success = 2; // set Wallpaper successfully
                break;
            }
            if (success == 1)
                bgWorker.ReportProgress(-2); // Communication Error
            #endregion// Socket Communication
        }
        private void BGWorkerMain_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!fatal)
                Close();
        }
        private void Shut_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        // It  may be discarded
        private void BGWorkerMain_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                fatal = true;
                Visible = false;
                MessageBox.Show("请求服务器失败\n请检查网络连接情况",
                    Init.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            else if (e.ProgressPercentage == -2)
            {
                fatal = true;
                Visible = false;
                MessageBox.Show("联络服务器失败", Init.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            Shut.Visible = true;
            Title.Text = "错误：无法连接至服务器。当前已尝试" + e.ProgressPercentage.ToString() + "次";
        }

        private String UdpMirrorIp()
        {
            //todo using udp to get ip of mirror
            return UdpClient.SendMain();
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
        private static bool DownloadFile(string URL, string filename=Data.ConfData.SaveFile)
        {
            return DownloadAbsFile(URL, Path.Combine(Init.CidsImagePath,filename));
        }
        // 摘要
        //  从 URL 下载文件到 filename 中
        // 参数
        //  filename 绝对路径文件名
        private static bool DownloadAbsFile(string URL, string filename)
        {
            try
            {
                File.Delete(filename);
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(URL);
                HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                Stream st = myrp.GetResponseStream();
                Stream so = new FileStream(filename, FileMode.Create);
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
            return true;
        }
        private bool DownloadAndSet(int time_out,int interval)
        {
            #region needed to change
            // get wallpaper file
            // An Absolute One
            string wallpaperPath = Data.ConfData.SaveAbsPathFile;

            // get json
            data = UdpClient.SendFirstMirror();
            String ImgUrl=data.Image_url;
            #region Download File
            var tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var task = Task.Factory.StartNew(() => DownloadAbsFile(ImgUrl, wallpaperPath), token); // download
            if (!task.Wait(time_out, token) || !task.Result) // timed out
            {
                Thread.Sleep(interval);
                return false;
            }
            #endregion//download
            #endregion//change region

            string wallpaper = Image.CourceBoxes.GraphicsCompose(data, wallpaperPath);
            SetWallpaper(wallpaper, Style.Stretched);
            return true;
        }
        #endregion
    }
}
