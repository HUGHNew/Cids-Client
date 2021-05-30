using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Data;

namespace Client
{
    namespace Message
    { 
        public class TimedMsgBox
        {
            public static void TimedMessageBox(string msg,int showTime)
            {
                Task task = Task.Factory.StartNew(()=> {
                    MessageBox.Show(msg, ConfData.ClientTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                });
                new Thread(() => {
                    task.Wait(showTime,new CancellationToken());
                }).Start();
            }
        }
    }
}