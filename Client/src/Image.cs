﻿using System;
using System.Collections.Generic;
using System.Drawing;
//using Windows.Foundation;

namespace Client
{

    namespace Image
    {
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
        class Opacity {
            public const int max = 255;
            public const int back = 100;
            public const int frame = back+back>>1;
            public const int text = 250;
        }
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
                public Cource(Json.ReceiveComponent.ReadableEvent readable,Color[]scheme= null)
                {
                    scheme= scheme ?? ColorSchemes.SchemeThree;
                    titleStr = readable.CourseTitle;
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
