﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Client.Image;

namespace Client.Test
{
    class ImgTest
    {
        public const string filepath = "../../test/img/";
#if DEBUG
        public static void ImgSwitch() {
            //Console.WriteLine("First Use "+ImageConf.ToSetWallFile());
            while (true)
            {
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine("Now We Use " + ImageConf.GetDestFile());
            }
        }
#endif
        public static void t0()
        {
            var files=Directory.EnumerateFiles(filepath);
            int i = 0;
            foreach(var it in files)
            {
                CourceBoxes cb = new CourceBoxes();
                Json.ReceiveComponent.ReadableEvent re = new Json.ReceiveComponent.ReadableEvent("编程原理实战与击剑技术",42, "Everyone");
                //{
                //    CourseTitle = "编程原理实战与击剑技术",
                //    CourseNo = 42,
                //    Professor = "Everyone"
                //};
                cb.Add(new Cource(re));
                re=new Json.ReceiveComponent.ReadableEvent("击剑技术", 9, "金轮");
                cb.Add(new Cource(re));
                //cb.Add(new CourceBoxes.Cource(re));
                //cb.Add(new CourceBoxes.Cource("正方形打野", Color.Black, "直播间：606118", Color.Red, "韩金轮", Color.Green, Color.DarkGray, Color.LightGray));
                //cb.Add(new CourceBoxes.Cource("吉吉圣经解读", Color.Black, "直播间：12306", Color.Red, "棍爹", Color.Green, Color.DarkGray, Color.LightGray));
                //cb.Add(new CourceBoxes.Cource("极上の肉体、最高のSEX 全ての理想を叶える究极射精スペシャル", Color.Black, "直播间：SSIS-062", Color.Red, "三上悠亜", Color.Green, Color.DarkGray, Color.LightGray));
                cb.DrawImageSaveAs(new Bitmap(it, true), filepath + $"test{i++}.jpg");
            }
        }
        public static void ppt()
        {
            var files = Path.Combine(filepath,"6.jpg");
            CourceBoxes cb = new CourceBoxes();
            Json.ReceiveComponent.ReadableEvent re = new Json.ReceiveComponent.ReadableEvent("编译原理", 3, "潘微");
            //{
            //    CourseTitle = "编程原理实战与击剑技术",
            //    CourseNo = 42,
            //    Professor = "Everyone"
            //};
            cb.Add(new Cource(re));
            re = new Json.ReceiveComponent.ReadableEvent("击剑技术", 0, "金轮");
            cb.Add(new Cource(re));
            //cb.Add(new CourceBoxes.Cource(re));
            //cb.Add(new CourceBoxes.Cource("正方形打野", Color.Black, "直播间：606118", Color.Red, "韩金轮", Color.Green, Color.DarkGray, Color.LightGray));
            //cb.Add(new CourceBoxes.Cource("吉吉圣经解读", Color.Black, "直播间：12306", Color.Red, "棍爹", Color.Green, Color.DarkGray, Color.LightGray));
            //cb.Add(new CourceBoxes.Cource("极上の肉体、最高のSEX 全ての理想を叶える究极射精スペシャル", Color.Black, "直播间：SSIS-062", Color.Red, "三上悠亜", Color.Green, Color.DarkGray, Color.LightGray));
            cb.DrawImageSaveAs(new Bitmap(files, true), filepath + $"test6.jpg");
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni); // for Wallpaper Set
        public static void WPSave()
        {
            StringBuilder wallPaperPath = new StringBuilder(200);
            String save = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "test.jpg");
            SystemParametersInfo(0x73, 200, wallPaperPath, 0).ToString();
            new System.Drawing.Bitmap(wallPaperPath.ToString()).Save(save);
        }
    }
}
