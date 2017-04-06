﻿using System;
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
using System.Windows.Forms;



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


        private void mainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
            comboBox.Items.Add("Right_Wrist_Img");
            comboBox.Items.Add("Left_Wrist_Img");
        }

        private void bntStart_Click(object sender, RoutedEventArgs e)
        {
            
            webcam.Start();
            webcam.channel = comboBox.Text;

        }

        private void bntStop_Click(object sender, RoutedEventArgs e)
        {
            webcam.Stop();

        }

        private void bntContinue_Click(object sender, RoutedEventArgs e)
        {
            webcam.Continue();
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




        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox cmb = (ComboBox)sender;
            //cmb
            
        }

    }
}
