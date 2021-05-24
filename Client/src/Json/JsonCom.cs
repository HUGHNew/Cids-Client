using System;
using System.Collections.Generic;

namespace Client {
    class Field {
        private static readonly string urlForWallpaper = Environment.GetEnvironmentVariable("TMP");
        private const string fileName = "wallpaper_assistant_config.txt";
        public static String UrlForWallpaper{get;set;}
        public static String FileName { get; }
    }

    // <Summary>
    // Field : __Field__Data
    // use `Data` as suffix to avoid the collision of Class Name and Variable Name
    // </Summary>
    namespace Json {
        class MirrorRequest {
            private String uuid;
            private String mr_time; // time
            public String UUID{
                get {
                    return uuid;
                 }
                set {
                    uuid = value;
                }
            }
            public String Time{
                get
                {
                    return mr_time;
                }
                set
                {
                    mr_time = value;
                }
            }
            public MirrorRequest(String id,String time)
            {
                this.uuid = id;
                this.mr_time = time;
            }
            public override String ToString()
            {
                return "UUID:" + uuid + " time:" + mr_time; 
            }
        }
        namespace ReceiveComponent
        {
            // brief: Encapsulation of the Emengency Message for JSON
            public class MessageData
            {
                public string Title { get; set; }
                public string Text { get; set; }
                public int ExpireTime { get; set; } //Sec
                public override string ToString()
                {
                    return "Title:"+Title+"\tText:"+Text+"\tExpireTime:"+ ExpireTime;
                }
            }
            // 事件: 代表当前的课程(教室借用情况)
            
            public class EventData
            {
                // 课程号
                public string Kch { get; set; }
                // 课序号
                public int Kxh { get; set; }
                // 课程名
                public string Kcm { get; set; }
                // 教师姓名
                public string Jsxm { get; set; }
                // 教学地点
                public string Jxdd { get; set; }
                public ReadableEvent GetReadable()
                {
                    return new ReadableEvent(Kcm, Kxh, Jsxm);
                }

                public override string ToString()
                {
                    return "课程号:"+ Kch+ "\t课序号:"+Kxh+"\t课程名:"+Kcm
                            +"\n教师姓名:"+Jsxm+"\t教室地点:"+Jxdd;
                }
            }
            public class ReadableEvent
            {
                public string CourseTitle { get; set; } // 课程名
                public int CourseNo { get; set; } // 课序号
                public string Professor { get; set; } // 教师名

                public ReadableEvent(string title,int number,string teacher)
                {
                    CourseTitle = title;
                    CourseNo = number;
                    Professor = teacher;
                }
            }
        }
        public class MirrorReceive
        {
            public string Image_url { get; set; }
            public List<ReceiveComponent.MessageData> Message { get; set; }
            public ReceiveComponent.EventData Event { get; set; }
            public ReceiveComponent.EventData Next_event { get; set; }
            //public 
            public bool NeedUpdate { get; set; }
            public String Time { get; set; }
            public override String ToString() {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }
            //public MirrorReceive(bool needUpdate)
            //{
            //    this.update = needUpdate;
            //}
        }
    }
}
