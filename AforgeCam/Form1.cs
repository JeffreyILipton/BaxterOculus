using System;
using System.IO;
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


        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;
        private Bitmap lastframe;
        private BroadCaster bc;

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = -1;
            FinalFrame = new VideoCaptureDevice();
            bc = new BroadCaster();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            //VideoCapabilities vc = FinalFrame.VideoResolution;
            //vc.FrameSize = new Size(300, 200);
            //FinalFrame.VideoResolution = new VideoCapabilities();
            FinalFrame.DisplayPropertyPage(IntPtr.Zero);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.NewFrame += new NewFrameEventHandler(bc.NewFrame);
            bc.channel = channelComboBox.Text;
            channelComboBox.Enabled = false;
            //aTimer.Enabled = true;
            FinalFrame.Start();
            bc.start();
        }


        void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Bitmap im = (Bitmap)eventArgs.Frame.Clone();
            if (pictureBox1.Image != null) { pictureBox1.Image.Dispose(); }
            if (pictureBox2.Image != null) { pictureBox2.Image.Dispose(); }
            if (lastframe != null) { lastframe.Dispose(); }
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            lastframe = new Bitmap(eventArgs.Frame, new Size(200, 300));
            pictureBox2.Image = (Bitmap)lastframe.Clone();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FinalFrame.Stop();
            bc.stop();
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
