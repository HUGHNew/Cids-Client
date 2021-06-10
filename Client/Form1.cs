using System.ComponentModel;
using System.IO;
using System.Net.Security;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Windows.Forms;
using Microsoft.Win32;
using Client.Data;
using Client.Image;
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
            Thread.Sleep(3000);
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            UdpClient = new CidsClient();
            UdpClient.SendMain();
            bool resend = false;
            do {
                data = null;
                try {
                    ClientTool.TryDownload(ref UdpClient, ref data,
                        ClientTool.time_out, ClientTool.interval, true);
                    resend = false;
                }
                catch (IOException) { UdpClient.ReSendMain();resend = true; }
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
