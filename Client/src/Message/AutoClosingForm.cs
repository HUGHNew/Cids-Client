using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Client.Message
{
    partial class AutoClosingForm : Form
    {
        private static int MessageCounts=0;
        private const int FormGap = 20; // pixels
        
        private readonly static int ScreenWidth = Screen.PrimaryScreen.WorkingArea.Width;
        public AutoClosingForm(int time_msec,string msg)
        {
            InitializeComponent();
            FormInit(msg);
            CloseAfterTime(time_msec);
        }
        protected void CloseAfterTime(int msec)
        {
            // The progress to show
            Timer CloseTimer = new Timer{Interval = 1000};
            this.TimeProgressBar.Maximum = msec;
            this.TimeProgressBar.Minimum = 0;
            this.TimeProgressBar.Step = CloseTimer.Interval;
            CloseTimer.Tick += (sender, args) => { 
                TimeProgressBar.PerformStep();
                if (TimeProgressBar.Value == TimeProgressBar.Maximum)
                {
                    TimeProgressBar.Visible = false;
                    TimeProgressBar.Dispose();
                    --MessageCounts;
                    this.Close();
                }
            };
            Debug.WriteLine("Timer Start");
            CloseTimer.Start();
        }
        /// <summary>
        /// Init Form Label
        /// </summary>
        /// <param name="msg"></param>
        private void FormInit(string msg)
        {
            this.Location=new Point(ScreenWidth-this.Width, MessageCounts*FormGap);
            ++MessageCounts;
            int lines=0;
            this.MainLabel.Text = ManualWrapperLabel
                (msg,Convert.ToInt32(MainLabel.Font.Size),MainLabel.Width,ref lines);
            this.MainLabel.Height *= lines;
            Debug.WriteLine(MainLabel.Text+$" {MainLabel.Width}");
        }

        public static string ManualWrapperLabel
            (string text,int fontsize,int width) {
            int ph=0;
            return ManualWrapperLabel(text, fontsize, width,ref ph);
        }
        /// <summary>
        /// 手动字符串换行
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="width">上层容器宽度</param>
        /// <param name="lines">返回行数</param>
        /// <returns>
        /// 切割好填充的字符串
        /// </returns>
        public static string ManualWrapperLabel
            (string text, int fontsize, int width,ref int lines)
        {
            Decimal ID(int i) =>Convert.ToDecimal(i);
            int DI(Decimal d) => Convert.ToInt32(d);
            int IntCeil(int num, int den) => DI(Math.Ceiling(num / ID(den)));
            int IntFloor(int num, int den) => DI(Math.Floor(num / ID(den)));
            string[] SplitByLen(string txt, int len)
            {
                int ceil = IntCeil(text.Length, len), floor = IntFloor(text.Length, len);
                String[] rels = new string[ceil];
                for (int i = 0; i < floor; ++i)
                {
                    rels[i] = txt.Substring(i * len, len);
                }
                if (floor != ceil)
                {
                    rels[floor] = txt.Substring(floor * len);
                }
                return rels;
            }
            lines = IntCeil(text.Length,IntFloor(width,fontsize));
            Debug.WriteLine($"{text}\n Len-{text.Length}\t{IntFloor(width, fontsize)}\tline:{lines}");
            return String.Join("\n", SplitByLen(text, IntFloor(width, fontsize)));
        }
        public static void NewAutoClosingForm(int msec,string text,bool parallel)
        {
            if (parallel)
            {
                Task.Factory.StartNew(
                    ()=>Application.Run(new AutoClosingForm(msec,text)));
            }
            else{Application.Run(new AutoClosingForm(msec, text));}
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
