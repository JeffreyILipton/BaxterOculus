using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using LCM.LCM;


namespace WPFCSharpWebCam
{
    /// <summary>
    /// Design by Pongsakorn Poosankam
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        static public LCM.LCM.LCM myLCM;
        public Window1()
        {
            InitializeComponent();
            
        }

        WebCam webcam;
        private String channel;
        private System.Timers.Timer aTimer = new System.Timers.Timer();
        private bool started = false;

        private void mainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
            myLCM = new LCM.LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");
            channel = "defaulImage";
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(publish);
            aTimer.Interval = 100;
            aTimer.Enabled = true;
        }

        private void bntStart_Click(object sender, RoutedEventArgs e)
        {
            
            webcam.Start();
            started = true;
        }

        private void bntStop_Click(object sender, RoutedEventArgs e)
        {
            webcam.Stop();
            started = false;
        }

        private void bntContinue_Click(object sender, RoutedEventArgs e)
        {
            webcam.Continue();
            started = true;
        }

        private void bntCapture_Click(object sender, RoutedEventArgs e)
        {
            imgCapture.Source = imgVideo.Source;
        }

        private void bntSaveImage_Click(object sender, RoutedEventArgs e)
        {
            Helper.SaveImageCapture((BitmapSource)imgCapture.Source);
        }

        private void bntResolution_Click(object sender, RoutedEventArgs e)
        {
            webcam.ResolutionSetting();
        }

        private void bntSetting_Click(object sender, RoutedEventArgs e)
        {
            webcam.AdvanceSetting();
        }

        private void publish(object sender, System.Timers.ElapsedEventArgs e)
        {
            publish();
        }
        private void publish() { 
            if (started == true && channel != "")
            {
                oculuslcm.image_t frame = new oculuslcm.image_t();
                imgCapture.Source = imgVideo.Source;
                BitmapSource BS = (BitmapSource)imgCapture.Source;

                frame.width = BS.PixelWidth;
                frame.height = BS.PixelHeight;
                frame.data = Helper.ReadImageMemory(BS);
                myLCM.Publish(channel, frame);
            }

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            channel = comboBox.Text;
        }

    }
}
