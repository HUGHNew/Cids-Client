using System;
using Microsoft.Toolkit.Uwp.Notifications;
using Client.Data;

namespace Client.Message
{
    public class ToastGenerator
    {
        public static int Id = 0;// 当天消息记录
        static public ToastContentBuilder Build(string content, string title = "紧急事件", ToastScenario ts = ToastScenario.Reminder
                , Uri hero = null, Uri inline = null, Uri logo = null)
        {
            ToastContentBuilder toast = new ToastContentBuilder()
                .AddArgument("message")
                .AddArgument("conversationId", ++Id)
                .AddText(title)
                .AddText(content)
                //.AddButton(
                //    new ToastButton()
                //    .SetContent("确定")
                //    .AddArgument("action", "reply")
                //    .SetBackgroundActivation()
                //)
                .AddButton(
                    new ToastButtonDismiss()
                )
                .SetToastScenario(ts)
                ;
            Logo(toast, logo);
            Hero(toast, hero);
            Inline(toast, inline);
            return toast;
        }
        private static ToastContentBuilder Logo(ToastContentBuilder tcb)
        {
            return Logo(tcb, ConfData.LogoUri);
        }
        static private ToastContentBuilder Logo(ToastContentBuilder tcb, Uri logo )
        {
            if (logo != null)
            {
                tcb.AddAppLogoOverride(logo, ToastGenericAppLogoCrop.Circle);
            }
            return tcb;
        }
        static private ToastContentBuilder Hero(ToastContentBuilder tcb, Uri hero = null)
        {
            if (hero != null)
            {
                tcb.AddHeroImage(hero);
            }
            else
            {
                // remain to rethink
                //tcb.AddHeroImage(new Uri("file://" + TMP + "/cov.png"));
            }
            return tcb;
        }
        static private ToastContentBuilder Inline(ToastContentBuilder tcb, Uri inline = null)
        {
            if (inline != null)
            {
                tcb.AddInlineImage(inline);
            }
            return tcb;
        }
    }
}
