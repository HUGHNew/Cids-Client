using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Client.Message
{
    public class Show{
        // 摘要
        //  弹出窗口(Toast&MessageBox)显示消息
        public static void MessageShow(List<Json.ReceiveComponent.MessageData> msglist,ToastScenario scenario=ToastScenario.Reminder)
        {
            //UrgentMessageShow method = FormShow;
            UrgentMessageShow method = FormShowParallel;
            method(msglist);
            //if (msglist == null||msglist.Count==0) return;
            //foreach(var it in msglist)
            //{
            //    ToastGenerator.Build(it.Text,it.Title,scenario).Show(toast =>
            //        {
            //            toast.ExpirationTime = DateTime.Now.AddSeconds(it.ExpireTime);
            //            toast.Group = "Cids-Client";
            //        }
            //    );
            //    TimedMsgBox.TimedMessageBox(it.Title,it.Text,it.ExpireTime,true);
            //}
        }
        public static void Sleep(int milli)
        {
            System.Threading.Thread.Sleep(milli);
        }
        /// <summary>
        /// 显示紧急消息
        /// </summary>
        /// <param name="msglist">消息列表</param>
        public delegate void UrgentMessageShow(List<Json.ReceiveComponent.MessageData> msglist);
        public static void FormShowBase(List<Json.ReceiveComponent.MessageData> msglist,bool parallel)
        {
            if (msglist == null) return;
            foreach(var it in msglist)
            {
                AutoClosingForm.NewAutoClosingForm(it.ExpireTime*1000, it.Text, parallel);
            }
        }
        public static void FormShow(List<Json.ReceiveComponent.MessageData> msglist)
        {
            FormShowBase(msglist, false);
        }
        public static void FormShowParallel(List<Json.ReceiveComponent.MessageData> msglist)
        {
            FormShowBase(msglist, true);
        }
    }
}
