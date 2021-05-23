using System.Drawing;
using Client.Image;

namespace Client
{
    class ImgTest
    {
        public const string filepath = "../../test/img/";
        public static void t0()
        {
            for(int i = 0; i < 4; ++i)
            {
                CourceBoxes cb = new CourceBoxes();
                cb.Add(new CourceBoxes.Cource("正方形打野", Color.Black, "直播间：606118", Color.Red, "韩金轮", Color.Green, Color.DarkGray, Color.LightGray));
                cb.Add(new CourceBoxes.Cource("吉吉圣经解读", Color.Black, "直播间：12306", Color.Red, "棍爹", Color.Green, Color.DarkGray, Color.LightGray));
                cb.Add(new CourceBoxes.Cource("极上の肉体、最高のSEX 全ての理想を叶える究极射精スペシャル", Color.Black, "直播间：SSIS-062", Color.Red, "三上悠亜", Color.Green, Color.DarkGray, Color.LightGray));
                cb.DrawImageSaveAs(new Bitmap(filepath+$"{i}.jpg", true), filepath + $"test{i}.png");
            }
        }
    }
}
