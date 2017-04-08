using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCM.LCM;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AforgeCam
{
    class BroadCaster
    {
        static public LCM.LCM.LCM myLCM;
        public string channel { get; set; }
        private Bitmap lastframe;
        bool locked = false;

        private System.Timers.Timer aTimer = new System.Timers.Timer();
        
        private int height = 300;
        private int width = 200;

        public BroadCaster()
        {
            myLCM = new LCM.LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");
            channel = "";
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(publish);
            aTimer.Interval = 200;
            //aTimer.Enabled = true;
        }

        public void setImage(Bitmap imframe)
        {
            if (!locked)
            {
                //if (lastframe != null) { lastframe.Dispose(); }
                lastframe = new Bitmap(imframe);
            }
        }


        public void start()
        {
            aTimer.Enabled = true;
        }

        public void stop()
        {
            aTimer.Enabled = false;
        }

        private void publish(object sender, System.Timers.ElapsedEventArgs e)
        {
            locked = true;
            if (channel != "" && lastframe != null)
            {
                oculuslcm.image_t frame = new oculuslcm.image_t();
                frame.width = width;
                frame.height = height;
                frame.data = BitmapToArray(new Bitmap(lastframe));
                frame.size = frame.data.Length;
                myLCM.Publish(channel, frame);
                //Console.WriteLine("frame");
                
                Console.WriteLine(".");
            }
            else
            {
                Console.Write("tick: ");
                Console.Write(channel);
                Console.Write("\n");
            }
            locked = false;
        }

        public static byte[] BitmapToArray(System.Drawing.Bitmap image)
        {
            var bmp = ConvertBitmap(image);
            bmp = new FormatConvertedBitmap(bmp, PixelFormats.Bgra32, null, 0);
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];

            bmp.CopyPixels(pixels, stride, 0);
            return pixels;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //    return ms.ToArray();
            //}
        }

        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

    }
}
