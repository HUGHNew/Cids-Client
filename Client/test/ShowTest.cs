using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Client.Test
{
    class ShowTest
    {
        public static void toastEx()
        {
            var msg=Message.ToastGenerator.Build("5s Test");
            msg.Show(tst =>
            {
                tst.ExpirationTime = System.DateTime.Now.AddSeconds(5);
            });
            //Message.Show.Sleep(1000);
        }
        public static void SingleShow()
        {
            Message.Show.MessageShow(
                new List<Json.ReceiveComponent.MessageData>
                {
                    new Json.ReceiveComponent.MessageData("消息演示","消息内容展示\n展示",5)
                }
                );
        }
        public static void SeriesShow()
        {
            Message.Show.MessageShow(
                new List<Json.ReceiveComponent.MessageData>
                { new Json.ReceiveComponent.MessageData("Title","Msg",10),
                    new Json.ReceiveComponent.MessageData("Alarm", "Alarming", 5),
                    new Json.ReceiveComponent.MessageData("World", "hello", 3),
                    new Json.ReceiveComponent.MessageData("Last", "LastOne", 1)
                }
            );
        }
        public static void Show()
        {
            var list = new List<Json.ReceiveComponent.MessageData> {
                new Json.ReceiveComponent.MessageData("Title","Msg",3),
            };
            Message.Show.MessageShow(
                new List<Json.ReceiveComponent.MessageData> 
                { new Json.ReceiveComponent.MessageData("Title","Msg",30),
                    
                }
                , ToastScenario.Reminder);
            
            Message.Show.MessageShow(
                new List<Json.ReceiveComponent.MessageData> 
                { new Json.ReceiveComponent.MessageData("Alarm", "Alarming", 5) }
                , ToastScenario.Alarm);
            Message.Show.MessageShow(
                new List<Json.ReceiveComponent.MessageData>
                { new Json.ReceiveComponent.MessageData("Call", "Call is Showing", 5) }
                , ToastScenario.IncomingCall);
            Message.Show.MessageShow(
                new List<Json.ReceiveComponent.MessageData>
                { new Json.ReceiveComponent.MessageData("Default", "Default is Showing", 3) }
                , ToastScenario.Default);
        }
    }
}
