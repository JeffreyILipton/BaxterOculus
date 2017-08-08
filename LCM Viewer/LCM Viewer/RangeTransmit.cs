using System;
using LCM.LCM;

namespace LCM.LCM_Viewer
{
    /// <summary>
    /// Demo transmitter, see LCM .NET tutorial for more information
    /// </summary>
    class RangeTransmit
    {
        public static void Transmit()
        {
            LCM.LCM myLCM = Display.myLCM;
            //Console.WriteLine("Range");

            //while (true)
            //{
            try
            {
                oculuslcm.range_t range = new oculuslcm.range_t();
                range.range = (float).5;//(25.0 + 5 * Math.Sin(DateTime.Now.Ticks / 10000000.0));

                myLCM.Publish("right_lcm_range", range);

                System.Threading.Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Ex: " + ex);
            }
        }
    }
}

