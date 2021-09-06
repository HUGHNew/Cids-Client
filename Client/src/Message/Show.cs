using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Client.Message
{
    class Show{
        // 摘要
        //  弹出窗口(Toast&MessageBox)显示消息
        public static void MessageShow(List<Json.ReceiveComponent.MessageData> msglist,ToastScenario scenario=ToastScenario.Reminder)
        {
            if (msglist == null||msglist.Count==0) return;
            foreach(var it in msglist)
            {
                ToastGenerator.Build(it.Text,it.Title,scenario).Show(toast =>
                    {
                        toast.ExpirationTime = DateTime.Now.AddSeconds(it.ExpireTime);
                        toast.Group = "Cids-Client";
                    }
                );
                TimedMsgBox.TimedMessageBox(it.Title,it.Text,it.ExpireTime,true);
            }
        }
        public static void Sleep(int milli)
        {
            System.Threading.Thread.Sleep(milli);
        }
    }
}
