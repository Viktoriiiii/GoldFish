using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace GoldFish.Classes
{
    public static class Captcha
    {
        private static string text = string.Empty;

        public static BitmapImage CreateCaptcha()
        {
            Random random = new Random();
            Bitmap bmp = new Bitmap(100, 100);

            int Xpos = 5;
            int Ypos = 5;

            Brush[] brushes =
            {
                Brushes.Black,
                Brushes.Red,
                Brushes.DarkBlue,
                Brushes.Gray,
                Brushes.Tomato
            };
            FontStyle style = FontStyle.Regular;
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.Clear(Color.White);

            text = String.Empty;
            string alf = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
            for (int i = 0; i < 5; ++i)
            {
                char t = alf[random.Next(alf.Length)];
                text += t;
                graphics.DrawString(t.ToString(), new Font("Arial", 15, style),
                    brushes[random.Next(brushes.Length)], new PointF(Xpos + random.Next(5) + i * 15, Ypos + random.Next(10)));
            }

            for (int i = 0; i < 100; ++i)
                for (int j = 0; j < 100; ++j)
                    if (random.Next() % 20 == 0)
                        bmp.SetPixel(i, j, Color.Gray);

            var bmpImage = ToBitmapImage(bmp);
            return bmpImage;
        }

        public static string GetText()
        {
            return text;
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
    }
}
