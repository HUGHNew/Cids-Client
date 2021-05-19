using System;

namespace Client
{
    public class JsonTest
    {
        // test class for json
        public static void test()
        {
            string prefix="../../test/json/";
            string log = "json.log";
            string[] files = { "NoUpdate.json",  "emptyMsg.json", "lastEvent.json", "min.json", "noUrl.json", "recv.json" };
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
                Console.WriteLine($"In file : {file}");
                //writer.WriteLine($"In file : {file}");
                receive = Newtonsoft.Json.JsonConvert.DeserializeObject<Json.MirrorReceive>(System.IO.File.ReadAllText(prefix + file));
                if (receive.NeedUpdate)
                {
                    Console.WriteLine($"Url:{receive.Image_url}\nEvent-Color:{receive.Event.Color}");
                    if (receive.Message.Count > 0)
                    {
                        Console.WriteLine($"Msg[0]-Title:{receive.Message[0].Title}");
                    }
                    Console.WriteLine($"Next-Event-Color:{receive.Next_event.Color??""}");
                }
                else
                {
                    Console.WriteLine("not update");
                }
                //writer.WriteLine(receive.ToString());
            }writer.Close();

        }
    }
}
