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
        private static bool Zero = false; // Current WallPaper No
        // 摘要
        //  将 UseZero 取反 然后返回改后值
        private static bool Toggel() => Zero = !Zero;
        #endregion
        public static string GetDestFile()
        {
#if DEBUG
            Console.WriteLine("Next File To Set: "+ToSetWallFile());
#else
#endif
            //if (UseZero)
            //{
            //    UseZero = false;
            //    return DstFiles[1];
            //}
            //else { UseZero = true;
            //    return DstFiles[0];
            //}
            return DstFiles[Toggel() ? 0 : 1];
        }
        /// <summary>
        /// 获取现在用的壁纸
        /// </summary>
        public static string ToSetWallFile()
        {
            return DstFiles[Zero? 0 : 1];
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
    public struct Course
    {
        public Course(Json.ReceiveComponent.ReadableEvent readable,Color[]scheme= null)
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

        public Course(string titleString, Color titleColor, string idString, Color idColor,
            string teacherString, Color teacherColor, Color frameColor, Color backColor)
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
    public class CoursesBox
    {
        public int Size{get; private set; }
        public const int TextDem = 50;
        public CoursesBox() {
            Size = 0;
            //head = new Cource() { 
            //    idStr="0"
            //};
            Cources = new List<Course>();
        }
        Course head;
        public List<Course> Cources;
        /// <summary>
        /// call once each time in all occasions
        /// however it may be revised
        /// </summary>
        /// <param name="c">new course added</param>
        public void Add(Course c)
        {
            if (c.idStr == "0") return; // the empty one
            if (Size == 0)
                head = c;
            else
                Cources.Add(c);
            ++Size;
        }
        public enum DrawSchema {
            Decrementing, // the old one
            Equidistant // Three same-size boxes
        }
        /// <summary>
        /// 画图并保存
        /// </summary>
        /// <param name="img"></param>
        /// <param name="path"></param>
        /// <param name="method"></param>
        public void DrawImageSaveAs(System.Drawing.Image img,string path,DrawSchema schema)
        {
            Operation.DrawImageSave method;
            switch (schema)
            {
                case DrawSchema.Decrementing:
                    method = new Operation.DrawImageSave(DrawImageSaveAs);
                    break;
                case DrawSchema.Equidistant:
                    method = new Operation.DrawImageSave(DrawParallelBoxes);
                    break;
                default:
                    method = new Operation.DrawImageSave(DrawParallelBoxes);
                    break;
            }
            method(img, path);
        }
        /// <summary>
        /// 绘制三个等大窗口 依次为消息框 当前课程框 下节课框
        /// 窗口宽为1/4 高为1/4
        /// </summary>
        /// <param name="image">底图，会自动释放</param>
        /// <param name="path">保存路径</param>
        public void DrawParallelBoxes(System.Drawing.Image image, string path) {
            Graphics g = Graphics.FromImage(image);
            int[] rate = {2,4,2};
            #region Draw Course and Content
            int XAxisSize = image.Width >> 2, XAxisLocate = XAxisSize*3;
            int YAxisSize = image.Height >> 2,YAxisDelta= YAxisSize >> 3;
            // 预留20字位置 再删去两边边框 算21字位置
            int BaseFontSize = (image.Width>>2)/21;
            int edgeThick = BaseFontSize; // 字体大小一半
            const int magic = 5;
            // BaseFontSize * 4 的情况下 最多可以一行5个字
            // 最多占满 
            //if()
            int TSLen = head.teacherStr?.Length??0;
            float TearcherRate = (float)(rate[1]*1.0/(TSLen>magic? magic:1));
            #region Font
            Font TitleFont = new Font("黑体", BaseFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            Font CourseFont = new Font("黑体", BaseFontSize*rate[0], FontStyle.Regular, GraphicsUnit.Pixel);
            Font TearcherFont = new Font("黑体", BaseFontSize * TearcherRate, FontStyle.Regular, GraphicsUnit.Pixel);
            Font NumFont = new Font("黑体", BaseFontSize * rate[2], FontStyle.Regular, GraphicsUnit.Pixel);
            // 第一行 课程名
            // 第二行 教师名
            // 第三行 课序号
            // 空间与大小占比为 2:4:2 不可能把课程放太大，不然放不下
            Pen pen = new Pen(head.frameClr,edgeThick);
            #endregion
            StringFormat leftFormat = new StringFormat {Alignment = StringAlignment.Near };
            StringFormat rightFormat = new StringFormat{Alignment = StringAlignment.Far};
            StringFormat centerFormat = new StringFormat(StringFormatFlags.LineLimit)
                {Alignment=StringAlignment.Center};
            // present message thought new form
            void DrawText(Course course, Rectangle rect) {
                SolidBrush brush;
                Point LStrPoint, RStrPoint;
                Rectangle TextRect;
                int Xval = rect.X+XAxisSize/2; // X axis Start Point
                LStrPoint = new Point(rect.X, rect.Y+YAxisDelta*(8-rate[2])); // only used by Id_num
                RStrPoint = new Point(image.Width, rect.Y + YAxisDelta * (rate[0] + rate[1]));
                //RStrPoint = new Point(Xval, rect.Y);
                // 课程
                brush = new SolidBrush(course.titleClr);
                TextRect = new Rectangle(XAxisLocate, rect.Y, XAxisSize,CourseFont.Height<<1);
                g.DrawString(course.titleStr,CourseFont,brush,TextRect,centerFormat);
                // 教师
                brush = new SolidBrush(course.teacherClr);
                TextRect = new Rectangle(XAxisLocate, rect.Y+ Convert.ToInt32(YAxisDelta * rate[0] * 1.5),
                    XAxisSize, TearcherFont.Height<<1);
                g.DrawString(course.teacherStr, TearcherFont, brush, TextRect, centerFormat);
                RStrPoint = new Point(image.Width, rect.Y + YAxisDelta * (rate[0]+rate[1])); // max width
                // 课序号
                brush = new SolidBrush(course.idClr);
                g.DrawString("课序号为", NumFont, brush, LStrPoint, leftFormat);
                g.DrawString(course.idStr, NumFont, brush, RStrPoint, rightFormat);
            }
            #region Rectangle Window
            Rectangle MsgWnd = new Rectangle(XAxisLocate, 0, XAxisSize, YAxisSize); // message window
            DrawRectangle(ref g, new Course {backClr=head.backClr}, MsgWnd, pen); // draw msg rect
            Rectangle ThisCourse = new Rectangle(XAxisLocate, YAxisSize+edgeThick, XAxisSize, YAxisSize);
            DrawRectangle(ref g, head,ThisCourse,pen);
            Rectangle NextCourse = new Rectangle(XAxisLocate, YAxisSize + edgeThick <<1, XAxisSize, YAxisSize);
            DrawRectangle(ref g, head, NextCourse, pen);
            #endregion
            SolidBrush MsgBrush = new SolidBrush(head.titleClr);
            g.DrawString("消息框",CourseFont,MsgBrush,new Point(XAxisLocate+XAxisSize/2,0),centerFormat);
            // Draw Courses Info
            DrawText(head,ThisCourse);
            foreach(var c in Cources)
            {
                DrawText(c,NextCourse);
            }
            #endregion
            #region Save Part
            if (File.Exists(path)) File.Delete(path); // Delete if exists
            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            // release all resource
            image.Dispose();
            g.Dispose();
            #endregion
        }
        /// <summary>
        /// 绘图并保存
        /// </summary>
        /// <param name="img"></param>
        /// <param name="savePath"></param>
        public void DrawImageSaveAs(System.Drawing.Image img,string savePath)
        {
            double regularRate = 0.3; // 常规当前事件的大小
            double subShrink = 0.8; // 下一条事件相对于当前的缩放比例
            // Void error if no courses
            //if (head.idStr.Equals("0")) { img.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg); }
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
            // 把字画在图上
            void DrawText(ref Graphics graph, Course c, Rectangle rect,int leftPad, Font headFont, Font textFont,
                int cols= 5,int text1 = 2,int text2 = 3){
                Point strPoint;
                SolidBrush brush = new SolidBrush(c.titleClr);
                //int Xval = rec.X + leftPadding;
                int Xval = rect.X + rect.Width;
                strPoint = new Point(Xval, rect.Y + Convert.ToInt32(rect.Height / cols) * 1); // need to change
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Far
                };
                graph.DrawString(c.titleStr, headFont, brush, strPoint, format);// CourseTitle--写课程标题
                brush.Color = c.idClr;
                strPoint = new Point(Xval, rect.Y + Convert.ToInt32(rect.Height / cols) * text1);
                graph.DrawString(c.idStr, textFont, brush, strPoint, format); // CourseID--写课程号
                brush.Color = c.teacherClr;
                strPoint = new Point(Xval, rect.Y + Convert.ToInt32(rect.Height / cols) * text2);
                graph.DrawString(c.teacherStr, textFont, brush, strPoint, format);// Teacher--写老师
            };
            
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
            foreach (Course c in Cources) // safe if empty
            {
                pen.Color = c.frameClr;
                rec = new Rectangle(boxPoint, boxWH);
                DrawRectangle(ref g, c,ref rec, pen, thick);

                DrawText(ref g, c,rec, leftPadding, boldFont, regularFont);

                boxPoint.Y += boxWH.Height;
            }
            Debug.WriteLine("The File to Save:"+savePath);
            if (File.Exists(savePath)) File.Delete(savePath); // Delete if exists
            img.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            // release all resource
            img.Dispose();
            g.Dispose();
        }
        // 画出矩形框 然后修改矩形框的布局
        private static void DrawRectangle(ref Graphics graphics, Course c, ref Rectangle rect, Pen _pen, int _thick)
        {
            graphics.DrawRectangle(_pen, rect); // 画矩形框

            rect.X += _thick; rect.Y += _thick;
            rect.Width -= _thick * 2; rect.Height -= _thick * 2;
            SolidBrush brush = new SolidBrush(c.backClr);
            graphics.FillRectangle(brush, rect); // 填充矩形背景
        }
        private static void DrawRectangle(ref Graphics graphics, Course c,Rectangle rect, Pen _pen)
        {
            graphics.DrawRectangle(_pen, rect); // 画矩形框
            SolidBrush brush = new SolidBrush(c.backClr);
            graphics.FillRectangle(brush, rect); // 填充矩形背景
        }
    };
    public class Operation{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="SavePath"></param>
        public delegate void DrawImageSave(System.Drawing.Image image, string SavePath);
        /// <summary>
        /// 获取图片路径 更改图片 生成图片路径为 wp[01].jpg
        /// </summary>
        /// <param name="data">收到的数据</param>
        /// <param name="BasePicture">下载的图片路径</param>
        /// <param name="SavePath">图片保存路径</param>
        /// <returns>返回 图片绝对路径名</returns>
        public static string GraphicsCompose(Json.MirrorReceive data, string BasePicture,string SavePath)
        {
            Debug.WriteLine("GraphicsCompose Json : " + data.ToString());
            
            if (!data.Equals(null) && data.NeedUpdate)
            {
                CoursesBox boxes = new CoursesBox();
                boxes.Add(new Course(data.Event.GetReadable()));
                boxes.Add(new Course(data.Next_event.GetReadable()));
                boxes.DrawImageSaveAs(new Bitmap(BasePicture, true), SavePath,CoursesBox.DrawSchema.Equidistant);
            }
            else
            {
                File.Copy(BasePicture, SavePath);
            }
            return SavePath;
        }
        /// <summary>
        /// 获取图片路径 更改图片 生成图片路径为 wp[01].jpg 会更改使用图片值
        /// </summary>
        /// <param name="data">收到的数据</param>
        /// <param name="BasePicture">已下载的图片路径 即 raw.jpg</param>
        /// <returns>返回 图片绝对路径名</returns>
        public static string GraphicsCompose(Json.MirrorReceive data, string BasePicture)
        {
            string destPic = Path.Combine(
                Data.ConfData.CidsImagePath,
                ImageConf.GetDestFile());
            return GraphicsCompose(data, BasePicture, destPic);
        }
        /// <summary>
        /// 基于 raw.jpg 和 json 的数据 合成一张新的 课程表图片
        /// </summary>
        /// <param name="data"></param>
        /// <returns>图片的绝对路径名</returns>
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
            Debug.WriteLine("Current Wallpaper Path:"+CurrentPath);
            Bitmap CurWP = new Bitmap(CurrentPath.ToString()); // get current pic
            CurWP.Save(Data.ConfData.SaveAbsPathFile); // save
        }
    }
}
