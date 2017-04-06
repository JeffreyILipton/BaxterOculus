using System;
using System.IO;
using System.Linq;
using System.Text;
using WebCam_Capture;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using LCM.LCM;

namespace WPFCSharpWebCam
{
    //Design by Pongsakorn Poosankam
    class WebCam
    {

        static public LCM.LCM.LCM myLCM;
        public string channel { get; set; }
        private System.Timers.Timer aTimer = new System.Timers.Timer();
        private bool started = false;

        private WebCamCapture webcam;
        private System.Windows.Controls.Image _FrameImage;
        private System.Drawing.Bitmap imframe;
        private int FrameNumber = 30;
        public void InitializeWebCam(ref System.Windows.Controls.Image ImageControl)
        {
            webcam = new WebCamCapture();
            webcam.FrameNumber = ((ulong)(0ul));
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.ImageCaptured += new WebCamCapture.WebCamEventHandler(webcam_ImageCaptured);
            _FrameImage = ImageControl;
            myLCM = new LCM.LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");
            channel = "";
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(publish);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
        }

        void webcam_ImageCaptured(object source, WebcamEventArgs e)
        {
            _FrameImage.Source = Helper.LoadBitmap((System.Drawing.Bitmap)e.WebCamImage);
            imframe = (System.Drawing.Bitmap)e.WebCamImage;
        }

        public void Start()
        {
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.Start(0);
            started = true;
        }

        public void Stop()
        {
            webcam.Stop();
            started = false;
        }

        public void Continue()
        {
            // change the capture time frame
            webcam.TimeToCapture_milliseconds = FrameNumber;

            // resume the video capture from the stop
            webcam.Start(this.webcam.FrameNumber);
            started = true;
        }

        public void ResolutionSetting()
        {
            webcam.Config();
        }

        public void AdvanceSetting()
        {
            webcam.Config2();
        }

        private void publish(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            if (started == true && channel != "" && imframe != null)
            {
                oculuslcm.image_t frame = new oculuslcm.image_t();
                frame.width = imframe.Size.Width;
                frame.height = imframe.Size.Height;
                
                frame.data = Helper.BitmapToArray(imframe);
                myLCM.Publish(channel, frame);
                Console.WriteLine("frame");
            }
            else
            {
                Console.Write("tick on Channel: ");
                Console.Write(channel);
                Console.Write("\n");
            }
        }

    }
}
