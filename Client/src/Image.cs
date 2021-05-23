using System;
using System.Collections.Generic;
using System.Drawing;
//using Windows.Foundation;

namespace Client
{

    namespace Image
    {
        class CourceBoxes
        {
            public int Size{get; private set; }
            public const int TextDem = 50;
            public CourceBoxes() {
                Size = 0;
                Cources = new List<Cource>();
            }
            public struct Cource
            {
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
                public string titleStr, idStr, teacherStr;
            };
            Cource head;
            public List<Cource> Cources;

            public void Add(Cource c)
            {
                if (Size == 0)
                    head = c;
                else
                    Cources.Add(c);
                ++Size;
            }
            public void DrawImageSaveAs(System.Drawing.Image img, string savePath)
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
                headBoxWH = new Size(Convert.ToInt32(imgWidth * 0.3), Convert.ToInt32(imgHeight * 0.3));
                boxPoint = new Point(Convert.ToInt32(imgWidth * 0.7), 0);

                rec = new Rectangle( boxPoint,headBoxWH );
                DrawRectangle(ref g, head,ref rec, pen, thick);

                int leftPadding = Convert.ToInt32(rec.Width * 0.02);

                DrawText(ref g, head, rec, leftPadding, boldFont, regularFont);

                boxWH = new Size(Convert.ToInt32(imgWidth * 0.2), Convert.ToInt32(imgHeight * 0.2));
                boxPoint.X = Convert.ToInt32(imgWidth * 0.8);
                boxPoint.Y = headBoxWH.Height;
                foreach (Cource c in Cources)
                {
                    pen.Color = c.frameClr;
                    rec = new Rectangle(boxPoint, boxWH);
                    DrawRectangle(ref g, c,ref rec, pen, thick);

                    DrawText(ref g, c,rec, leftPadding, boldFont, regularFont);

                    boxPoint.Y += boxWH.Height;
                }
                img.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
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
            //
            // 摘要:
            //     获取图片路径 更改图片 生成图片路径为 时间戳.png
            // 参数:
            //  picture: 下载的图片路径
            //  data   : 收到的数据
            // 返回:
            //      返回 图片路径名
            public static string GraphicsCompose(string picture, Json.MirrorReceive data)
            {
                Console.WriteLine("TODO in GraphicsCompose function");
                return null;
            }
        };
    }
}
