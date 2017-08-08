using System;
using LCM.LCM;

namespace LCM.LCM_Viewer
{
    /// <summary>
    /// Demo transmitter, see LCM .NET tutorial for more information
    /// </summary>
    class ValidTransmit
    {
        public static void Transmit()
        {
            LCM.LCM myLCM = Display.myLCM;
            //Console.WriteLine("Range");

            //while (true)
            //{
            try
            {
                oculuslcm.trigger_t valid = new oculuslcm.trigger_t();
                valid.trigger = true;

                myLCM.Publish("right_lcm_valid_1", valid);

                valid.trigger = false;

                myLCM.Publish("right_lcm_valid_0", valid);

                System.Threading.Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Ex: " + ex);
            }
            //}
        }
    }
}
