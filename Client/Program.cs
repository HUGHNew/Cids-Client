using System;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
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
            t1();
            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
        static void t0() {
            String jsonfile = File.ReadAllText("../../test/t0.json");
            jsontest mr = JsonConvert.DeserializeObject<jsontest>(jsonfile);
            if (mr != null)
            {
                Console.WriteLine(mr.Time);
                Console.WriteLine(mr.uuid);
                Console.WriteLine(mr.ToString());
            }
            else Console.WriteLine("Null Object");
        }
        static void t1() {
            String jsonfile = File.ReadAllText("../../test/recv.json");
            Json.MirrorReceive mr = JsonConvert.DeserializeObject<Json.MirrorReceive>(jsonfile);
            if (mr != null)
            {
                Console.WriteLine(mr.Message[0].Title);
            }
            else Console.WriteLine("Null Object");
        }
    }
    class jsontest
    {
        public string Time { get; set; }
        public string uuid { get; set; }
        //public jsontest() {
        //    time = "";
        //    uuid = "";
        //}
    }
}
