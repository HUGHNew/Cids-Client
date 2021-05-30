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
            public const string Author = "智锐科创计算机协会";
            public const string ClientTitle = "四川大学信息分发系统"+" by "+Author;
            public const string EnvName = "Cids"; // Env Path
            public const string EnvId = "CidsUUID"; // CidsUUID
            public const string Conf = "CidsConf.json"; // configuration file
            #endregion
            #region Top Level Variable
            public static readonly string CidsPath = // Get Path First
                Environment.GetEnvironmentVariable(EnvName, EnvironmentVariableTarget.Machine);
            public static readonly string UuId = // Get UUID
                Environment.GetEnvironmentVariable(EnvId, EnvironmentVariableTarget.Machine);
            public static readonly Json.Conf InitData; // remain to be assigned in Ctor
            #endregion

            #region Socket
            public readonly static int MainPort, MirrorPort;
            public readonly static IPAddress DefaultMServer; // Server IP need
            public const int DefaultPackageNumber = 10;
            #endregion

            // Image Stored in %Cids% file
            public static readonly string CidsImagePath = Path.Combine(CidsPath, "image"); // created while installing
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
                    ConfData.InitData = Newtonsoft.Json.JsonConvert.DeserializeObject<Json.Conf>
                        (File.ReadAllText(CidsPath));
                }
                catch (Exception) // can't access or something else
                {
                    InitData = null;
                    return;
                }

                #endregion

                #region Socket
                DefaultMServer = IPAddress.Parse(InitData.Net.Main_Ip);
                MainPort = InitData.Net.Main_Port;
                MirrorPort = InitData.Net.Mirror_Port;
                #endregion

                #region Toast
                ConfData.Logo = InitData.Logo;
                ConfData.LogoUri = new Uri(Logo);
                #endregion
            }
        }

    }
}
