using System;
using System.IO;
using System.Windows.Forms;

namespace Client
{
    class Init
    {
        public const string ClientTitle = "四川大学智慧教学系统壁纸同步工具";
        public const string deskInitConf = "Cids.txt";
        public const string RegName = "Cids"; // add to registry
        public const string Conf = "Cids.conf"; // configuration file
        public static readonly string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        // Image Stored in %TMP% file
        public static readonly string CidsPath = ((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine)["TMP"] as string)
            ?? @"C:\Windows\Temp") +"\\Cids";
        public static readonly string ConfFile = Path.Combine(CidsPath, Conf);
        public static bool Configuration()
        {
            if (InitCidsInRegistry()&& DirCheckOrCreate())
            {
                // Create File to store UUID
                // Read From File:desktop\deskInitConf
                try
                {
                    string id=File.ReadAllText(Path.Combine(desktop, deskInitConf)); // read UUID from file
                    if (IdValidate(id))
                    {
                        //MessageBox.Show(Directory.GetCurrentDirectory(), ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        byte[] data = System.Text.Encoding.ASCII.GetBytes(id);
                        FileStream StdOut=File.Create(ConfFile);
                        StdOut.Write(data, 0, data.Length);
                        StdOut.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("Id格式不正确", ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Id写入本地目录出错", ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }return true;
        }
        private static bool IdValidate(string id) {
            const char lowBound = '0',highBound='9';
            foreach (char nu in id)
            {
                if (nu > highBound || nu < lowBound)
                {
                    return false;
                }
            }return true;
        }
        private static bool DirCheckOrCreate()
        {
            try
            {
                DirectoryInfo info=Directory.CreateDirectory(Init.CidsPath);
                if (!info.Exists)
                {
                    info.Create();
                }
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("创建目录Cids出错"+e.Message, ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

        }
        private static bool InitCidsInRegistry(bool box=true)
        {
            Microsoft.Win32.RegistryKey key=null;
            try {
                string value=Microsoft.Win32.Registry.CurrentUser.GetValue(RegName) as string;
                if (!(value?.Equals(CidsPath)??true))
                {
                    key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegName);
                
                    key.SetValue(RegName, CidsPath);
                }
            }
            catch (Exception)
            {
                //e.Message() for log
                if (box)
                {
                    MessageBox.Show("添加注册表项出错", ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }
            finally { 
                key?.Close();
            }return true;
        }
    }
}