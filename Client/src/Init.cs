using System;
using System.IO;
using System.Windows.Forms;
using Client.Data;

namespace Client
{
    class Init
    {        
        public static string ConfFile => Path.Combine(Data.ConfData.CidsPath, Data.ConfData.Conf); // where to get uuid
        public static bool Configuration()
        {
            return IntegrityCheck();
        }
        // 摘要
        //  应用于 installer 安装之后的启动文件检查 也可应用于其他安装情形
        private static bool IntegrityCheck()
        {
           if (null==ConfData.InitData||null== ConfData.UuId) return false;
            // return null or not exists
            Debug.WriteLine(Path.Combine(ConfData.CidsPath, ConfData.Conf));
            return File.Exists(Path.Combine(ConfData.CidsPath,ConfData.Conf));
        }
        #region Things of LocalInstall
        public const string deskInitConf = "Cids.txt";
        // Get Set Using User Level Registry
        public const EnvironmentVariableTarget Target = EnvironmentVariableTarget.User;
        public static readonly string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static string ValueOfCids = null; // store value
        
        private static bool LocalBuddle()
        {
            if (Startup())
            {
                return true;
            }
            if (InitCidsInRegistry() && DirCheckOrCreate()) // add key and create dir successfully
            {
                // Create File to store UUID
                // Read From File:desktop\deskInitConf
                try
                {
                    string id = File.ReadAllText(Path.Combine(desktop, deskInitConf)); // read UUID from file
                    if (IdValidate(id))
                    {
                        //MessageBox.Show(Directory.GetCurrentDirectory(), ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        byte[] data = System.Text.Encoding.ASCII.GetBytes(id);
                        FileStream StdOut = File.Create(ConfFile);
                        StdOut.Write(data, 0, data.Length);
                        StdOut.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("Id格式不正确", ConfData.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Id写入本地目录出错", ConfData.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }
        // Judge whether Registry Key and Directory created
        public static bool Startup()
        {
            ValueOfCids = ValueOfCids ?? Environment.GetEnvironmentVariable(ConfData.EnvName, Target);
            return Directory.Exists(ConfData.CidsPath) && ConfData.CidsPath.Equals(ValueOfCids);//Environment.SetEnvironmentVariable(EnvName, CidsPath);
        }
        #region UUId and Key setup
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
                DirectoryInfo info=Directory.CreateDirectory(ConfData.CidsPath);
                if (!info.Exists)
                {
                    info.Create();
                }
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("创建目录Cids出错"+e.Message, ConfData.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

        }
        private static bool InitCidsInRegistry(bool box=true)
        {
            try {
                // add new key:Cids
                if (false == ConfData.CidsPath.Equals(ValueOfCids)) // not Equals
                {
                    Environment.SetEnvironmentVariable(ConfData.EnvName, ConfData.CidsPath,Target);
                    ValueOfCids = ConfData.CidsPath;
                }
            }
            catch (Exception e)
            {
                if (box)
                {
                    MessageBox.Show("添加注册表项出错:"+e.Message, ConfData.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }
            finally { 
            }return true;
        }
        #endregion
        #endregion//local
    }
}