﻿using System.ComponentModel;
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
                MessageBox.Show(Init.ClientTitle, "配置Cids出错", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw new Exception(); //  terminate the whole program
            }
        }
        private String GetUrl(int choice=0)
        {
            switch (choice)
            {
                case 0: 
                    return UdpUrl();
                case 1:
                    return ConfWayForUrl();
                default:
                    return null;
            }
        }
        private void BGWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        { 
            Thread.Sleep(3000);
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            UdpClient = new CidsClient();
            String url = GetUrl();
            int try_count = 10;
            // Timeout, millsec
            int time_out = 10000;
            // Interval, millsec
            int interval = 2000;
            if (url == null)
            {
                bgWorker.ReportProgress(-1); // Configuration Error
                return;
            }
            int success = 1;
            for (int c = 0; c < try_count; ++c)
            {
                if (c > 0) // ?
                    bgWorker.ReportProgress(c); // Error of communication with Server -- Timed Out
                try
                {
                    if (DownloadAndSet(url, time_out, interval) == false)
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
        private void BGWorkerMain_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                fatal = true;
                Visible = false;
                MessageBox.Show("读取配置文件失败\n请检查网络连接情况",
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
        #region Ways for Url
        private String ConfWayForUrl()
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(Field.UrlForWallpaper + Field.FileName);
            }
            catch (Exception)
            {
                return null;
            }
            string url = null;
            while (true)
            {
                url = sr.ReadLine(); // readline from conf file
                if (url == null) // empty file
                    break;
                if (Regex.Match(url, "^\\s*$").Success) // empty line
                    continue;
            }
            return url;
        }
        private String UdpUrl()
        {
            //todo using udp to get url
            return UdpClient.SendMain();
        }
        #endregion
        #region Download and Set Wallpaper
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private static bool DownloadFile(string URL, string filename)
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
        private bool DownloadAndSet(String ImgUrl,int time_out,int interval)
        {
            #region needed to change
            string wallpaperPath = "";// File.Create(Init.CidsPath); // revise file name
            var tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            data = UdpClient.SendMirror();
            var task = Task.Factory.StartNew(() => DownloadFile(ImgUrl, wallpaperPath), token); // download
            if (!task.Wait(time_out, token) || !task.Result) // timed out
            {
                Thread.Sleep(interval);
                return false;
            }
            // something to do here before set
            #endregion

            string wallpaper = Image.CourceBoxes.GraphicsCompose(wallpaperPath, data);
            SetWallpaper(wallpaper, Style.Stretched);
            return true;
        }
        #endregion
    }
}
