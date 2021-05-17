using System;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;

namespace Client
{
    
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread] 
        static void Main()
        {
            //t2();
            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
        static void t1() {
            String jsonfile = File.ReadAllText("../../test/recv.json");
            Json.MirrorReceive mr = JsonConvert.DeserializeObject<Json.MirrorReceive>(jsonfile);
            if (mr != null)
            {
                mr = JsonConvert.DeserializeObject<Json.MirrorReceive>(JsonConvert.SerializeObject(mr));
                Console.WriteLine(JsonConvert.SerializeObject(mr));
            }
            else Console.WriteLine("Null Object");
        }
    }
}
