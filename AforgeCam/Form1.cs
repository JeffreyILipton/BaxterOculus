using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

namespace AforgeCam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;

        private void Form1_Load(object sender, EventArgs e)
        {
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = -1;
            FinalFrame = new VideoCaptureDevice();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            //VideoCapabilities vc = FinalFrame.VideoResolution;
            //vc.FrameSize = new Size(300, 200);
            //FinalFrame.VideoResolution = new VideoCapabilities();
            FinalFrame.DisplayPropertyPage(IntPtr.Zero);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            channelComboBox.Enabled = false;
            FinalFrame.Start();
        }


        void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap im = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = new Bitmap(im, new Size(300, 200));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FinalFrame.Stop();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            //label2.Text = channelComboBox.Text;
        }
    }
}
