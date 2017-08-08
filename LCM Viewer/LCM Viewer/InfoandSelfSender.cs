﻿using System;
using LCM.LCM;
using oculuslcm;

namespace LCM.LCM_Viewer
{
    class InfoandSelfSender
    {
        static public LCM.LCM myLCM;
        static public robotself_t[] self;
        static public info_t[] info;


        static public bool run = true;
        public static void RunInfoandSelfSender()
        {
            self = new robotself_t[3];
            info = new info_t[3];

            for (int i = 0; i < self.Length; i++)
            {
                self[i] = new robotself_t();
                self[i].id = i;
                self[i].queryChannel = "control_query|" + self[i].id;
                info[i] = new info_t();
                info[i].id = i;
            }

            info[0].user = -1;
            info[1].user = -1;
            info[2].user = 7;

            self[0].type = "Baxter";
            self[1].type = "LameScene";
            self[2].type = "Lobby";

            self[0].channelCount = 14;
            self[0].channels = new string [self[0].channelCount];
            self[0].channels[0] = "incorrect channel";//"right_lcm_channel|"      + self[0].id;
            self[0].channels[1] = "left_lcm_channel|"       + self[0].id;
            self[0].channels[2] = "incorrect channel";//"right_lcm_valid|"        + self[0].id;
            self[0].channels[3] = "left_lcm_valid|"         + self[0].id;
            self[0].channels[4] = "right_lcm|"              + self[0].id;
            self[0].channels[5] = "left_lcm|"               + self[0].id;
            self[0].channels[6] = "right_lcm_cmd|"          + self[0].id;
            self[0].channels[7] = "left_lcm_cmd|"           + self[0].id;
            self[0].channels[8] = "right_trigger_lcm|"      + self[0].id;
            self[0].channels[9] = "left_trigger_lcm|"       + self[0].id;
            self[0].channels[10] = "right_lcm_currentpos|"  + self[0].id;
            self[0].channels[11] = "left_lcm_currentpos|"   + self[0].id;
            self[0].channels[12] = "right_lcm_range|"       + self[0].id;
            self[0].channels[13] = "left_lcm_range|"        + self[0].id;

            try
            {
                myLCM = new LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");
                
                myLCM.Subscribe("control_query|1", new SimpleSubscriber());
                myLCM.Subscribe("control_query|2", new SimpleSubscriber());
                myLCM.Subscribe("control_query|3", new SimpleSubscriber());
                Random rand = new Random();

                while (run)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        info[i].priority += (rand.NextDouble() - .5) / 100;
                        myLCM.Publish("global_info", info[i]);
                        myLCM.Publish("global_self", self[i]);
                    };
                    System.Threading.Thread.Sleep(1000);
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
                if (channel.StartsWith("control_query|")){
                    query_t query = new query_t(dins);
                    int id = int.Parse((channel.Split('|')[1]));
                    Console.WriteLine(id);
                    info[id].user = query.userID;
                }
            }
        }
    }
}