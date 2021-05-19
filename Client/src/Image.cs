using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Controls;

namespace Image
{
    class CourceBoxes
    {
        public int Size{get; private set; }
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
            int wid = img.Width, hei = img.Height;
            int thick = wid/400, fontSize = wid/70; // 笔刷粗度, 字体大小
            Font boldFont = new Font("黑体", fontSize, FontStyle.Bold);
            Font regularFont = new Font("黑体", fontSize, FontStyle.Regular);
            Pen pen = new Pen(head.frameClr, thick);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
            Rectangle rec;
            SolidBrush brush;

            Point strPoint, boxPoint; // 位置
            Size headBoxWH, boxWH; // 长宽
           
            headBoxWH = new Size(Convert.ToInt32(wid * 0.3), Convert.ToInt32(hei * 0.3));
            boxPoint = new Point(Convert.ToInt32(wid * 0.7), 0);

            rec = new Rectangle( boxPoint,headBoxWH );
            g.DrawRectangle(pen, rec); // 画矩形框

            rec.X += thick; rec.Y += thick;
            rec.Width -= thick * 2; rec.Height -= thick * 2;
            brush = new SolidBrush(head.backClr);
            g.FillRectangle(brush, rec); // 填充矩形背景

            brush.Color = head.titleClr;
            strPoint = new Point(rec.X + Convert.ToInt32(rec.Width * 0.06), rec.Y + Convert.ToInt32(rec.Height / 6) * 1);
            g.DrawString(head.titleStr, boldFont, brush, strPoint); // 写课程标题
            brush.Color = head.idClr;
            strPoint = new Point(rec.X + Convert.ToInt32(rec.Width * 0.06), rec.Y + Convert.ToInt32(rec.Height / 6) * 2);
            g.DrawString(head.idStr, regularFont, brush, strPoint); // 写课程号
            brush.Color = head.teacherClr;
            strPoint = new Point(rec.X + Convert.ToInt32(rec.Width * 0.06), rec.Y + Convert.ToInt32(rec.Height / 6) * 5);
            g.DrawString(head.teacherStr, regularFont, brush, strPoint); // 写老c

            boxWH = new Size(Convert.ToInt32(wid * 0.2), Convert.ToInt32(hei * 0.2));
            boxPoint.X = Convert.ToInt32(wid * 0.8);
            boxPoint.Y = headBoxWH.Height;
            foreach (Cource c in Cources)
            {
                rec = new Rectangle(boxPoint, boxWH);
                pen.Color = c.frameClr;
                g.DrawRectangle(pen, rec);

                rec.X += thick; rec.Y += thick;
                rec.Width -= thick * 2; rec.Height -= thick * 2;
                brush = new SolidBrush(c.backClr);
                g.FillRectangle(brush, rec);

                brush.Color = c.titleClr;
                strPoint = new Point(rec.X + Convert.ToInt32(rec.Width * 0.06), rec.Y + Convert.ToInt32(rec.Height / 5) * 1);
                g.DrawString(c.titleStr, boldFont, brush, strPoint);
                brush.Color = c.idClr;
                strPoint = new Point(rec.X + Convert.ToInt32(rec.Width * 0.06), rec.Y + Convert.ToInt32(rec.Height / 5) * 2);
                g.DrawString(c.idStr, regularFont, brush, strPoint);
                brush.Color = c.teacherClr;
                strPoint = new Point(rec.X + Convert.ToInt32(rec.Width * 0.06), rec.Y + Convert.ToInt32(rec.Height / 5) * 3);
                g.DrawString(c.teacherStr, regularFont, brush, strPoint);

                boxPoint.Y += boxWH.Height;
            }
            img.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
        }
    };
    class Program
    {
        static void Main(string[] args)
        {
            CourceBoxes cb = new CourceBoxes();
            cb.Add(new CourceBoxes.Cource("正方形打野", Color.Black, "直播间：606118", Color.Red, "韩金轮", Color.Green, Color.DarkGray, Color.LightGray));
            cb.Add(new CourceBoxes.Cource("吉吉圣经解读", Color.Black, "直播间：12306", Color.Red, "棍爹", Color.Green, Color.DarkGray, Color.LightGray));
            cb.Add(new CourceBoxes.Cource("极上の肉体、最高のSEX 全ての理想を叶える究极射精スペシャル", Color.Black, "直播间：SSIS-062", Color.Red, "三上悠亜", Color.Green, Color.DarkGray, Color.LightGray));
            cb.DrawImageSaveAs(new Bitmap("src.jpg",true),"test.png");
        }

    }
}
