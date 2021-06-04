using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
//using Windows.Foundation;

namespace Client.Image
{

    class ImageConf
    {
        #region Const or Readonly and Function for getting a destination file
        private static readonly string[] DstFiles = { "wp0.jpg", "wp1.jpg" };
        private static bool UseZero = true;
        // 摘要
        //  将 UseZero 取反 然后返回当前值
        private static bool Toggel()
        {
            UseZero = !UseZero;
            return UseZero;
        }
        #endregion
        public static string GetDestFile()
        {
            return DstFiles[Toggel() ? 1 : 0];
        }
        public static string ToSetWallFile()
        {
            return DstFiles[UseZero? 0 : 1];
        }
    }
    #region Tool Classes about color and opacity scheme
    // reference : <https://coolors.co/palettes/trending>
    class ColorSchemes
    {
        // trend : B -> G -> R
        static public readonly Color[] SchemeOne = { // soft and old fashion
            Color.FromArgb(0x26,0x46,0x53),
            Color.FromArgb(0x2A,0x9D,0x8F),
            Color.FromArgb(0xE9,0xC4,0x6A),
            Color.FromArgb(0xF4,0xA2,0x61),
            Color.FromArgb(0xE7,0x6F,0x51),
        };
        static public readonly Color[] SchemeTwo = { // gradient
            Color.FromArgb(0x8E,0xCA,0xE6),
            Color.FromArgb(0x21,0x9E,0xBC),
            Color.FromArgb(0x2,0x30,0x47),
            Color.FromArgb(0xFF,0xB7,0x3),
            Color.FromArgb(0xEB,0x85,0),
        };
        static public readonly Color[] SchemeThree = { // High contrast
            Color.FromArgb(0,0,0),
            Color.FromArgb(0x14,0x21,0x3D),
            Color.FromArgb(0xFC,0xA3,0x11),
            Color.FromArgb(0xE5,0xE5,0xE5),
            Color.FromArgb(0xFF,0xFF,0XFF),
        };
    }
    class Opacity { // field/255
        public const int max = 255;
        public const int back = 100;
        public const int frame = back+back>>1;
        public const int text = 250;
    }
    #endregion
    public struct Cource
    {
        public Cource(Json.ReceiveComponent.ReadableEvent readable,Color[]scheme= null)
        {
            scheme= scheme ?? ColorSchemes.SchemeThree;
            titleStr = readable.CourseTitle;
            // 课序号
            idStr = readable.CourseNo.ToString();
            teacherStr = readable.Professor;
            backClr = Color.FromArgb(Opacity.back, scheme[0]);
            frameClr= Color.FromArgb(Opacity.frame, scheme[1]);
            idClr = Color.FromArgb(Opacity.text, scheme[2]);
            teacherClr = Color.FromArgb(Opacity.text, scheme[3]);
            titleClr = Color.FromArgb(Opacity.text, scheme[4]);
        }

        public Cource(string titleString, Color titleColor, string idString, Color idColor, string teacherString, Color teacherColor, Color frameColor, Color backColor)
        {
            titleStr = titleString;
            titleClr = titleColor;
            idStr = idString;
            idClr = idColor;
            teacherStr = teacherString;
            teacherClr = teacherColor;
            frameClr = frameColor;
            backClr = backColor;
        }
        public Color titleClr, idClr, teacherClr, frameClr, backClr;
        public string titleStr, /*课序号*/idStr, teacherStr;
    };
    class CourceBoxes
    {
        public int Size{get; private set; }
        public const int TextDem = 50;
        public CourceBoxes() {
            Size = 0;
            Cources = new List<Cource>();
        }
        Cource head;
        public List<Cource> Cources;

        public void Add(Cource c)
        {
            if (c.idStr == "0") return; // the empty one
            if (Size == 0)
                head = c;
            else
                Cources.Add(c);
            ++Size;
        }
        // 参数:
        //  regularRate :   常规当前事件的大小
        //  subShrink   :   下一条事件相对于当前的缩放比例
        public void DrawImageSaveAs(System.Drawing.Image img, string savePath,double regularRate=0.3,double subShrink=0.8)
        {
            Graphics g = Graphics.FromImage(img);
            int imgWidth = img.Width, imgHeight = img.Height;
            // font size 1/TextDem
            int thick = imgWidth/(2*TextDem), fontSize = imgWidth/ TextDem; // 笔刷粗度, 字体大小
            Font boldFont = new Font("黑体", fontSize, FontStyle.Bold,GraphicsUnit.Pixel);
            Font regularFont = new Font("黑体", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            Pen pen = new Pen(head.frameClr, thick)
            {
                Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
            };
            Rectangle rec;

            Point boxPoint; // 位置
            Size headBoxWH, boxWH; // 长宽

            // 30%
            headBoxWH = new Size(Convert.ToInt32(imgWidth * regularRate), Convert.ToInt32(imgHeight * regularRate));
            boxPoint = new Point(Convert.ToInt32(imgWidth * (1-regularRate)), 0); // put top-right

            rec = new Rectangle(boxPoint,headBoxWH);
            DrawRectangle(ref g, head,ref rec, pen, thick);

            int leftPadding = Convert.ToInt32(rec.Width * 0.02);

            DrawText(ref g, head, rec, leftPadding, boldFont, regularFont);

            // Shrink by the rate
            boldFont = new Font("黑体", Convert.ToInt32(fontSize * subShrink), FontStyle.Bold, GraphicsUnit.Pixel);
            regularFont = new Font("黑体", Convert.ToInt32(fontSize * subShrink), FontStyle.Regular, GraphicsUnit.Pixel);
            boxWH = new Size(Convert.ToInt32(imgWidth * regularRate*subShrink), Convert.ToInt32(imgHeight * regularRate * subShrink));
            boxPoint.X = Convert.ToInt32(imgWidth * (1-regularRate*subShrink));
            boxPoint.Y = headBoxWH.Height;
            foreach (Cource c in Cources) // safe if empty
            {
                pen.Color = c.frameClr;
                rec = new Rectangle(boxPoint, boxWH);
                DrawRectangle(ref g, c,ref rec, pen, thick);

                DrawText(ref g, c,rec, leftPadding, boldFont, regularFont);

                boxPoint.Y += boxWH.Height;
            }
            img.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        // 把字画在图上
        private void DrawText(ref Graphics g,Cource c,Rectangle rec,int leftPadding,Font headFont,Font textFont,int cols=5,int text1=2,int text2=3)
        {
            Point strPoint;
            SolidBrush brush = new SolidBrush(c.titleClr);
            strPoint = new Point(rec.X + leftPadding, rec.Y + Convert.ToInt32(rec.Height / cols) * 1);
            g.DrawString(c.titleStr, headFont, brush, strPoint);// CourseTitle--写课程标题
            brush.Color = c.idClr;
            strPoint = new Point(rec.X + leftPadding, rec.Y + Convert.ToInt32(rec.Height / cols) * text1);
            g.DrawString(c.idStr, textFont, brush, strPoint); // CourseID--写课程号
            brush.Color = c.teacherClr;
            strPoint = new Point(rec.X + leftPadding, rec.Y + Convert.ToInt32(rec.Height / cols) * text2);
            g.DrawString(c.teacherStr, textFont, brush, strPoint);// Teacher--写老c
        }
        // 画出矩形框 然后修改矩形框的布局
        private void DrawRectangle(ref Graphics graphics,Cource c,ref Rectangle rec,Pen pen,int thick) {
            graphics.DrawRectangle(pen, rec); // 画矩形框

            rec.X += thick; rec.Y += thick;
            rec.Width -= thick * 2; rec.Height -= thick * 2;
            SolidBrush brush = new SolidBrush(c.backClr);
            graphics.FillRectangle(brush, rec); // 填充矩形背景
        }            
    };
    public class Operation{
        // 摘要:
        //     获取图片路径 更改图片 生成图片路径为 wp[01].jpg
        // 参数:
        //  BasePicture: 下载的图片路径
        //  data        : 收到的数据
        //  SavePath    : 图片保存路径
        // 返回:
        //      返回 图片绝对路径名
        public static string GraphicsCompose(Json.MirrorReceive data, string BasePicture,string SavePath)
        {
            CourceBoxes boxes = new CourceBoxes();
            boxes.Add(new Cource(data.Event.GetReadable()));
            boxes.Add(new Cource(data.Next_event.GetReadable()));
            boxes.DrawImageSaveAs(new Bitmap(BasePicture, true), SavePath);
            return SavePath;
        }
        // 摘要:
        //     获取图片路径 更改图片 生成图片路径为 wp[01].jpg
        // 参数:
        //  BasePicture: 下载的图片路径
        //  data   : 收到的数据
        // 返回:
        //      返回 图片绝对路径名
        public static string GraphicsCompose(Json.MirrorReceive data, string BasePicture)
        {
            string destPic = Path.Combine(
                Data.ConfData.CidsImagePath,
                ImageConf.GetDestFile());
            return GraphicsCompose(data, BasePicture, destPic);
        }
        // 摘要
        //  基于 raw.jpg 和 json 的数据 合成一张新的 课程表图片
        // 返回
        //  图片的绝对路径名
        public static string GraphicsCompose(Json.MirrorReceive data)
        {
            return GraphicsCompose(data, Data.ConfData.SaveAbsPathFile);
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni); // get current Wallpaper path
        // 摘要
        //  将当前壁纸拷贝到 raw.jpg
        public static void CopyDefaultWallpaperToRaw()
        {
            const int SPI_GETDESKWALLPAPER = 0x0073;
            // get path
            StringBuilder CurrentPath = new StringBuilder(200);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, 200, CurrentPath, 0);
            Bitmap CurWP = new Bitmap(CurrentPath.ToString()); // get current pic
            CurWP.Save(Data.ConfData.SaveAbsPathFile); // save
        }
    }
}
