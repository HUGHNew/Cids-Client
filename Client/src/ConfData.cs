using System;
using System.IO;
using System.Net;

namespace Client
{
    namespace Data
    {
        public class ConfData
        {
            #region public properties
            #region Top Level Value
            private const string Author = "智锐科创计算机协会";
            public const string ClientTitle = "四川大学信息分发系统"+" by "+Author;
            public const string EnvName = "Cids"; // Env Path
            public const string EnvId = "CidsUUID"; // CidsUUID
            public const string Conf = "CidsConf.json"; // configuration file
            #endregion
            #region Top Level Variable
            public const EnvironmentVariableTarget Target = EnvironmentVariableTarget.Machine;
            public static readonly string CidsTmpPath =
                Path.Combine(Environment.GetEnvironmentVariable("TMP", Target),EnvName);
            public static readonly string CidsPath = // Get Path First
                Environment.GetEnvironmentVariable(EnvName, Target);
            public static readonly string UuId = // Get UUID
                Environment.GetEnvironmentVariable(EnvId, Target);
            public static readonly Json.Conf InitData; // remain to be assigned in Ctor
            #endregion

            #region Socket
            public const int DefaultPackageNumber = 10;
            public readonly static int MainPort, MirrorPort,MirrorProtocol;
            public readonly static IPAddress DefaultMServer; // Server IP need

            public readonly static int MirrorRecvLimit; // counts limit

            // Unit:Milli Second
            public readonly static int SendDelayTime; // delay of every send
            public readonly static int HeartBeatGap; // heartbeat gap
            public readonly static int SleepMin; // Min Time
            public readonly static int SleepMax; // Max Time
            #endregion

            #region Image Part
            // Image Stored in %Cids% file
            public static readonly string CidsImagePath = Path.Combine(CidsTmpPath, "image"); // created while installing
            public const string SaveFile = "raw.jpg";
            public static readonly string SaveAbsPathFile = Path.Combine(CidsImagePath, SaveFile);
            #endregion

            #region Toast Part Properties
            public static readonly string Logo;
            public static readonly Uri LogoUri;
            #endregion// Toast

            #endregion// public properties

            static ConfData()
            {
                // initialization of properties
                #region Top i.e. base
                try
                {
                    Debug.WriteLine("Cids Path : "+CidsPath);
                    ConfData.InitData = Newtonsoft.Json.JsonConvert.DeserializeObject<Json.Conf>
                        (File.ReadAllText(Path.Combine(CidsPath,Conf)));
                }
                catch (Exception) // can't access or something else
                {
                    InitData = null;
                    return;
                }

                #endregion

                #region Socket
                MirrorProtocol = InitData.Protocol;
                DefaultMServer = IPAddress.Parse(InitData.Net.Main_Ip);
                MainPort = InitData.Net.Main_Port;
                MirrorPort = InitData.Net.Mirror_Port;

                #region Socket Time
                MirrorRecvLimit = InitData.Time.Limit;
                SendDelayTime = InitData.Time.Delay;
                HeartBeatGap = InitData.Time.HeartBeat;
                SleepMin = InitData.Time.Sleep.Min;
                SleepMax = InitData.Time.Sleep.Max;
                #endregion//socket time
                #endregion//socket

                #region Toast
                ConfData.Logo = Path.Combine(CidsPath,InitData.Logo);
                ConfData.LogoUri = new Uri(Logo);
                #endregion
                try { 
                    if(! Directory.Exists(CidsImagePath))
                        Directory.CreateDirectory(CidsImagePath);
                }catch (Exception){}
                Image.Operation.CopyDefaultWallpaperToRaw();
            }
        }

    }
}
