using System;
using System.Collections.Generic;

namespace Client {
    class Field {
        private static readonly string urlForWallpaper = Environment.GetEnvironmentVariable("TMP");
        private static readonly string fileName = "wallpaper_assistant_config.txt";
        public static String UrlForWallpaper{get;set;}
        public static String FileName { get; }
    }
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
            class Message
            {
                public string Title { get; set; }
                public string Text { get; set; }
                public string ExpireTime { get; set; } //Min/Sec
                public override string ToString()
                {
                    return "Title:"+Title+"\tText:"+Text+"\tExpireTime:"+ ExpireTime;
                }
            }
            // 事件: 代表当前的课程(教室借用情况)
            class Event
            {
                // brief: Encapsulation of the Course Content for JSON
                public class ContentData
                {
                    public string Content { get; set; }
                    public override string ToString()
                    {
                        return Content;
                    }
                }
                // 持续时间
                public string ExpireTime { get; set; }
                // 课程内容
                public ContentData Contents { get; set; }
                // 颜色
                public string Color { get; set; }
                public override string ToString()
                {
                    return "ExpireTime:"+ ExpireTime+ "\tContents:"+Contents.ToString();
                }
            }
        }
        class MirrorReceive
        {
            public string Image_url { get; set; }
            public List<ReceiveComponent.Message> Message { get; set; }
            public ReceiveComponent.Event Event{ get; set; }
            public ReceiveComponent.Event Next_event{ get; set; }
            private bool update;
            //public 
            public bool Update { get { return update; } set { update = value; } }

            //public MirrorReceive(bool needUpdate)
            //{
            //    this.update = needUpdate;
            //}
        }
    }
}
