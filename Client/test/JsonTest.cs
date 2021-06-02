using System;
using System.IO;
using Newtonsoft.Json;
using Client.Json;
using Client.Json.ReceiveComponent;

namespace Client.Test
{
    public class JsonTest
    {
        const string root = "../../test/json/";
        public static string FilePath(string file) => root+ file;
        public static void Bundle()
        {
            ConfTest("conf.json");
            ConfTest("conflogo.json");
            test(defaults);
        }
        // brief
        //  %Cids%Conf test
        public static void ConfTest(string filename)
        {
            Console.WriteLine("\nConf Test Begin Here\n");
            var json = JsonConvert.DeserializeObject<Conf>(File.ReadAllText(FilePath(filename)));
            Console.WriteLine(JsonConvert.SerializeObject(json));
            Console.WriteLine(json.Logo ?? "Null Logo");
            Console.WriteLine("\nConf Test Ends  Here\n");
        }
        public static void ConfTest(string filename, StreamWriter writer)
        {
            writer.WriteLine("\nConf Test Begin Here\n");
            var json = JsonConvert.DeserializeObject<Conf>(File.ReadAllText(FilePath(filename)));
            writer.WriteLine(JsonConvert.SerializeObject(json));
            writer.WriteLine(json.Logo ?? "Null Logo");
            writer.WriteLine("\nConf Test Ends  Here\n");
        }
        public static void newlytest()
        {
            test("recity.json");
        }
        // test class for json
        public static string log = "json.log";
        public static string[] defaults = { "NoUpdate.json", "emptyMsg.json", "lastEvent.json",  "noUrl.json", "recity.json" };
        public static void test(string file, System.IO.StreamWriter writer, out Json.MirrorReceive receive)
        {
            if (writer != null)
            {
                writer.WriteLine($"In file : {file}");
            }
            else
            {
                Console.WriteLine($"In file : {file}");
            }
            receive = Newtonsoft.Json.JsonConvert.DeserializeObject<Json.MirrorReceive>(System.IO.File.ReadAllText(FilePath(file)));
            if (receive.NeedUpdate)
            {
                if (writer != null)
                {
                    writer.WriteLine(JsonConvert.SerializeObject(receive));
                    writer.WriteLine("\nreceive \nreadable \n");
                    writer.WriteLine(JsonConvert.SerializeObject(receive.Next_event.GetReadable()));
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(receive));
                    Console.WriteLine("\nreceive \nreadable \n");
                    Console.WriteLine(JsonConvert.SerializeObject(receive.Next_event.GetReadable()));
                }
            }
            else
            {
                if (writer != null)
                {
                    writer?.WriteLine("not Update");
                }
                else Console.WriteLine("not update");
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
            System.IO.StreamWriter writer = new System.IO.StreamWriter(FilePath(log));
            //try { System.IO.File.Open(FilePath(log), System.IO.FileMode.OpenOrCreate | System.IO.FileMode.Append);
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("Error Occurs While Log File Open");
            //}
            foreach (string file in files)
            {
                test(file, writer,out receive);
                //writer.WriteLine(receive.ToString());
            }writer.Close();

        }
    }
}
