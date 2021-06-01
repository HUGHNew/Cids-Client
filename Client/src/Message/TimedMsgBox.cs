using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Data;

namespace Client.Message{ 
    public class TimedMsgBox
    {
        public static int Unit = 1000;
        public static void TimedMessageBox(string title,string msg, int showTimeSeconds,bool wait=false) {
            CancellationTokenSource CancellSrc = new CancellationTokenSource();
            CancellationToken token = CancellSrc.Token;
            Task task = Task.Factory.StartNew(() => {
                MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }, token);
            token.Register(()=> {});
            //CancellationToken cancellation = new CancellationToken(true);

            task.Wait(showTimeSeconds * Unit);
            //task.Dispose();
        }
        public static void TimedMessageBox(string msg,int showSeconds,bool wait=false)
        {
            TimedMessageBox(ConfData.ClientTitle, msg, showSeconds, wait);
        }
    }
}