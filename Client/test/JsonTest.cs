using System;
using Windows.Foundation;

namespace Client
{
    public class JsonTest
    {
        public static void newlytest()
        {
            test("recity.json");
        }
        // test class for json
        public static string prefix="../../test/json/";
        public static string log = "json.log";
        public static string[] defaults = { "NoUpdate.json", "emptyMsg.json", "lastEvent.json", "min.json", "noUrl.json", "recv.json" };
        public static void test(string file, System.IO.StreamWriter writer, out Json.MirrorReceive receive)
        {
            Console.WriteLine($"In file : {file}");
            receive = Newtonsoft.Json.JsonConvert.DeserializeObject<Json.MirrorReceive>(System.IO.File.ReadAllText(prefix + file));
            if (receive.NeedUpdate)
            {
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(receive.Event.GetReadable()));
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(receive.Next_event.GetReadable()));
                //Console.WriteLine($"Url:{receive.Image_url}\nEvent-Teacher:{receive.Event.Jsxm}");
                //if (receive.Message.Count > 0)
                //{
                //    Console.WriteLine($"Msg[0]-ExpireTime|Second:{receive.Message[0].ExpireTime}");
                //}
                //Console.WriteLine($"Next-Event-CourseNo:{receive.Next_event.Kxh}");
            }
            else
            {
                Console.WriteLine("not update");
            }
            //writer.WriteLine(receive.ToString());
        }
        public static void test(string file) {
            string[] files = { file };
            test(files);
        }
        public static void test(string[] files)
        {
            Json.MirrorReceive receive;
            System.IO.StreamWriter writer = new System.IO.StreamWriter(prefix + log);
            try { System.IO.File.Open(prefix + log, System.IO.FileMode.OpenOrCreate | System.IO.FileMode.Append);
            }
            catch (Exception)
            {
                Console.WriteLine("Error Occurs While Log File Open");
            }
            foreach (string file in files)
            {
                test(file, writer,out receive);
                //writer.WriteLine(receive.ToString());
            }writer.Close();

        }
    }
}
