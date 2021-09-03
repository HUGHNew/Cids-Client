using System.ComponentModel;
using System.IO;
using System.Threading;
using System;
using System.Windows.Forms;
using Client.Data;

namespace Client
{
    public partial class Form1 : Form
    {
        bool fatal = false;
        //public const string ClientTitle = "四川大学智慧教学系统壁纸同步工具";
        

        private Json.MirrorReceive data;
        private CidsClient UdpClient;

        
        public Form1()
        {
            InitializeComponent();
            Conf();
            BGWorkerMain.RunWorkerAsync();
        }
        private void HideForm()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            SetVisibleCore(false);
        }
        /// <summary>
        /// seems not need
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() => {
                this.Hide();
                this.Opacity = 1;
            }));
        }
        private static void Conf()
        {
            if (Init.Configuration()==false) // failed
            {
                MessageBox.Show("Cids配置文件不完整", ConfData.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw new Exception(); //  terminate the whole program
            }
        }
        private String GetUrl(int choice=0){ return null; }
        private void BGWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
#if RELEASE
            Thread.Sleep(3000); // Optimise Region
#endif
            //// Set Wallpaper wp0.jpg
            //string wp0 = Image.ImageConf.ToSetWallFile();
            //if (File.Exists(wp0)) { File.Delete(wp0); }
            //File.Copy(ConfData.SaveAbsPathFile,wp0);
            //ClientTool.SetWallpaper();
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            UdpClient = new CidsClient();
            Debug.WriteLine("Begin Send Main");
            UdpClient.SendMain();
            Debug.WriteLine("End Send Main");
            bool resend = false;
            do {
                data = null;
                try {
                    ClientTool.TryDownload(ref UdpClient, ref data,
                        ClientTool.time_out, ClientTool.interval, true);
                    resend = false;
                }
                catch (IOException ioe) {
                    Debug.WriteLine("IOException:"+ioe.Message);
                    UdpClient.ReSendMain();
                    resend = true;
                }
            } while (resend);
            ClientTool.Update(ref data);
            Client.Message.Show.MessageShow(data.Message);
            CidsClient.ClientBeat(UdpClient,ref data);
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
                    ConfData.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            else if (e.ProgressPercentage == -2)
            {
                fatal = true;
                Visible = false;
                MessageBox.Show("联络服务器失败", ConfData.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            Shut.Visible = true;
            Title.Text = "错误：无法连接至服务器。当前已尝试" + e.ProgressPercentage.ToString() + "次";
        }

        
    }
}
