using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LCM;

namespace LCM.LCM_Viewer
{
    /// <summary>
    /// Demo listener, demonstrating interoperability with other implementations
    /// Just run this listener and use any of the example_t message senders
    /// </summary>
    class Display
    {
        static public LCM.LCM myLCM;
        public static void Main(string[] args)
        {
            bool sendDummyValues = false;
            Console.WriteLine("Do you want to run a self and info simulator as opposed to displaying lcm signals?");
            if (Console.ReadLine().StartsWith("y") || Console.ReadLine().StartsWith("Y"))
            {
                InfoandSelfSender.RunInfoandSelfSender();
            }
            Console.WriteLine("Display Mode");
            Console.WriteLine("Do you want to send dummy range and valid data");
            sendDummyValues = (Console.ReadLine().StartsWith("y") || Console.ReadLine().StartsWith("Y"));
            try
            {
                myLCM = new LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");

                myLCM.SubscribeAll(new SimpleSubscriber());
                if (sendDummyValues)
                {
                    RangeTransmit.Transmit();
                    ValidTransmit.Transmit();
                }
                while (true)
                {
                    if (sendDummyValues)
                    {
                        RangeTransmit.Transmit();
                        ValidTransmit.Transmit();
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine("Ex: " + ex);
                Environment.Exit(1);
            }
        }

        internal class SimpleSubscriber : LCM.LCMSubscriber
        {
            public void MessageReceived(LCM.LCM lcm, string channel, LCM.LCMDataInputStream dins)
            {
                Console.WriteLine("recv: " + channel);
                //if (channel != "acrobot_y")
                //{
                //Console.WriteLine("recv: " + channel);
                //}

                //if ( channel == "left_lcm_camera")
                //{
                //    Console.WriteLine("Camera Width :");
                //    oculuslcm.image_t image = new oculuslcm.image_t(dins);
                //    Console.WriteLine(image.width);
                //}

                //if (channel == "right_lcm_currentpos")
                //{
                //    oculuslcm.pose_t pose = new oculuslcm.pose_t(dins);
                //    Console.WriteLine("0: " + pose.position[0] + " 1: " + pose.position[1] + " 2: " + pose.position[2]);
                //    Console.WriteLine("0: " + pose.orientation[0] + " 1: "
                //        + pose.orientation[1] + " 2: " + pose.orientation[2] + " 3: " + pose.orientation[3]);
                //}
                //if (channel == "right_lcm_range")
                //{
                //    oculuslcm.range_t range = new oculuslcm.range_t(dins);
                //    Console.WriteLine(channel + ": " + range.range);
                //}
                //if (channel == "right_lcm_valid")
                //{
                //    oculuslcm.trigger_t cmd = new oculuslcm.trigger_t(dins);
                //    Console.WriteLine(channel + ": " + cmd.trigger);
                //}
                //if (channel == "right_lcm_valid_1")
                //{
                //    oculuslcm.trigger_t cmd = new oculuslcm.trigger_t(dins);
                //    Console.WriteLine(channel + ": " + cmd.trigger);
                //}
                //if (channel == "right_lcm_valid_0")
                //{
                //    oculuslcm.trigger_t cmd = new oculuslcm.trigger_t(dins);
                //    Console.WriteLine(channel + ": " + cmd.trigger);
                //}
                //if (channel == "left_lcm_cmd")
                //{
                //    Console.WriteLine("recv: " + channel);
                //}
                //if (channel == "right_lcm_cmd")
                //{
                //    Console.WriteLine("recv: " + channel);
                //}
                //} 0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000

            }
        }
    }
}
