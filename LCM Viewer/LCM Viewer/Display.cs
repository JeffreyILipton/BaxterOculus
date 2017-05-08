﻿using System;
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


            try
            {
                myLCM = new LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");

                myLCM.SubscribeAll ( new SimpleSubscriber());
                //RangeTransmit.Transmit();
                while (true)
                    System.Threading.Thread.Sleep(1000);
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
                //Console.WriteLine("recv: " + channel);
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
                //if (channel == "right_lcm_trigger")
                //{
                //    oculuslcm.trigger_t cmd = new oculuslcm.trigger_t(dins);
                //    Console.WriteLine(channel + ": " + cmd.trigger);
                //}
                //if (channel == "right_lcm_trigger")
                //{
                //    oculuslcm.trigger_t cmd = new oculuslcm.trigger_t(dins);
                //    Console.WriteLine(channel + ": " + cmd.trigger);
                //}
                if (channel == "left_lcm")
                {
                    Console.WriteLine("recv: " + channel);
                }
                if (channel == "right_lcm")
                {
                    Console.WriteLine("recv: " + channel);
                    oculuslcm.pose_t pose = new oculuslcm.pose_t(dins);
                    Console.WriteLine("0: " + pose.position[0] + " 1: " + pose.position[1] + " 2: " + pose.position[2]);
                    //Console.WriteLine("0: " + pose.orientation[0] + " 1: "
                        //+ pose.orientation[1] + " 2: " + pose.orientation[2] + " 3: " + pose.orientation[3]);

                }
                //} 0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000

            }
        }
    }
}
