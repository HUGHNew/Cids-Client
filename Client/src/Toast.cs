using System;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Client
{
	public class ToastGenerator
	{
        public static Uri defaultLogoUri= new Uri("file://" + TMP + "/scu.ico");
        // defaultHeroUri is no in need

        public static string TMP = ((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine)["TMP"] as string)?.Replace('\\', '/') ?? "")+"/CIDS";
        public static int Id=0;
        public ToastGenerator(){}
        static public ToastContentBuilder Build(string content, string title="紧急事件",ToastScenario ts=ToastScenario.Reminder
                ,Uri hero = null, Uri inline = null, Uri logo = null)
        {
            var toast = new ToastContentBuilder()
                .AddArgument("message")
                .AddArgument("conversationId",++Id)
                .AddText(title)
                .AddText(content)
                .AddButton(
                    new ToastButton()
                    .SetContent("确定")
                    .AddArgument("action", "reply")
                    .SetBackgroundActivation()
                )
                .SetToastScenario(ToastScenario.Reminder);
            Logo(toast, logo);
            Hero(toast, hero);
            Inline(toast, inline);
            return toast;
        }
        static private ToastContentBuilder Logo(ToastContentBuilder tcb) {
            tcb.AddAppLogoOverride(new Uri("file://"+ TMP+ "/scu.ico"),ToastGenericAppLogoCrop.Circle);
            return tcb;
        }
        static private ToastContentBuilder Logo(ToastContentBuilder tcb, Uri logo=null)
        {
            logo = logo ?? defaultLogoUri;
            tcb.AddAppLogoOverride(logo, ToastGenericAppLogoCrop.Circle);
            return tcb;
        }
        static private ToastContentBuilder Hero(ToastContentBuilder tcb, Uri hero = null)
        {
            if (hero != null)
            {
                tcb.AddHeroImage(hero);
            }
            else {
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
            }return tcb;
        }
    }
}
