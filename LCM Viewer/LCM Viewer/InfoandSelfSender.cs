using System;
using LCM.LCM;
using oculuslcm;
using System.Collections.Generic;

namespace LCM.LCM_Viewer
{
    class InfoandSelfSender
    {
        static public LCM.LCM myLCM;
        static public Dictionary <int, robotself_t> self;
        static public Dictionary <int, info_t> info;


        static public bool run = true;
        public static void RunInfoandSelfSender()
        {
            self = new Dictionary<int, robotself_t>();
            info = new Dictionary<int, info_t>();

            for (short i = 0; i < 3; i++)
            {
                self[i + 100] = new robotself_t();
                self[i + 100].id = (short)(i + 100);
                self[i + 100].queryChannel = "control_query|" + self[i + 100].id;
                self[i + 100].ability = "idk";
                info[i + 100] = new info_t();
                info[i + 100].id = (short)(i + 100);
            }

            info[100].user = -2;
            info[101].user = -1;
            info[102].user = 7;

            info[100].confidence = .8f;
            info[101].confidence = .2f;
            info[102].confidence = .7f;

            self[100].type = "Baxter";
            self[101].type = "Baxter";
            self[102].type = "Fish";

            self[100].channelCount = 15;
            self[100].channels = new string [self[100].channelCount];
            self[100].channels[0] = "right_lcm_channel|"          + self[100].id;
            self[100].channels[1] = "left_lcm_channel|"           + self[100].id;
            self[100].channels[2] = "right_lcm_valid|"            + self[100].id;
            self[100].channels[3] = "left_lcm_valid|"             + self[100].id;
            self[100].channels[4] = "right_lcm|"                  + self[100].id;
            self[100].channels[5] = "left_lcm|"                   + self[100].id;
            self[100].channels[6] = "right_lcm_cmd|"              + self[100].id;
            self[100].channels[7] = "left_lcm_cmd|"               + self[100].id;
            self[100].channels[8] = "right_trigger_lcm|"          + self[100].id;
            self[100].channels[9] = "left_trigger_lcm|"           + self[100].id;   
            self[100].channels[10] = "right_lcm_currentpos|"      + self[100].id;
            self[100].channels[11] = "left_lcm_currentpos|"       + self[100].id;
            self[100].channels[12] = "right_lcm_range|"           + self[100].id;
            self[100].channels[13] = "left_lcm_range|"            + self[100].id;
            self[100].channels[14] = "confidence_threshold_lcm|"  + self[100].id;

            self[100].leftNDIChannel  = "NDIBOX (Logitech Webcam C930e 1)";
            self[100].rightNDIChannel = "NDIBOX (Logitech Webcam C930e 0)";

            self[101].channelCount = 14;
            self[101].channels = new string[self[100].channelCount];
            self[101].channels[0] = "incorrect channel";//"right_lcm_channel|"      + self[0].id;
            self[101].channels[1] = "left_lcm_channel|" + self[100].id;
            self[101].channels[2] = "incorrect channel";//"right_lcm_valid|"        + self[0].id;
            self[101].channels[3] = "left_lcm_valid|" + self[100].id;
            self[101].channels[4] = "right_lcm|" + self[100].id;
            self[101].channels[5] = "left_lcm|" + self[100].id;
            self[101].channels[6] = "right_lcm_cmd|" + self[100].id;
            self[101].channels[7] = "left_lcm_cmd|" + self[100].id;
            self[101].channels[8] = "right_trigger_lcm|" + self[100].id;
            self[101].channels[9] = "left_trigger_lcm|" + self[100].id;
            self[101].channels[10] = "right_lcm_currentpos|" + self[100].id;
            self[101].channels[11] = "left_lcm_currentpos|" + self[100].id;
            self[101].channels[12] = "right_lcm_range|" + self[100].id;
            self[101].channels[13] = "left_lcm_range|" + self[100].id;

            self[101].leftNDIChannel  = "DESKTOP-42UGJFF (Logitech Webcam C930e 0)";
            self[101].rightNDIChannel = "NDIBOX (Logitech Webcam C930e 0)";

            self[102].leftNDIChannel = "DESKTOP-42UGJFF (Logitech Webcam C930e 0)";
            self[102].rightNDIChannel = "NDIBOX (Logitech Webcam C930e 0)";
            try
            {
                myLCM = new LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");
                
                myLCM.Subscribe("control_query|1", new SimpleSubscriber());
                myLCM.Subscribe("control_query|2", new SimpleSubscriber());
                myLCM.Subscribe("control_query|3", new SimpleSubscriber());

                myLCM.Subscribe("confidence_threshold_lcm|1", new SimpleSubscriber());
                myLCM.Subscribe("confidence_threshold_lcm|2", new SimpleSubscriber());
                myLCM.Subscribe("confidence_threshold_lcm|3", new SimpleSubscriber());
                Random rand = new Random();

                while (run)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        //info[i].confidence += (rand.NextDouble() - .5) / 100;
                        if (info[i + 100].confidence > 1)
                        {
                            info[i+ 100].confidence = 1f;
                        }
                        else if (info[i + 100].confidence < 0f)
                        {
                            info[i + 100].confidence = 0f;
                        }
                        //Console.WriteLine(info[i].confidence);
                        myLCM.Publish("global_info", info[i + 100]);
                        myLCM.Publish("global_self", self[i + 100]);
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
                    if (info[id].user < 0 || query.userID < 0)
                    {
                        info[id].user = query.userID;
                    }
                }

                if (channel.StartsWith("confidence_threshold_lcm|"))
                {
                    confidencethreshold_t threshold = new confidencethreshold_t(dins);
                    int id = int.Parse((channel.Split('|')[1]));
                    info[id].threshold = threshold.confidence;
                }
            }
        }
    }
}
