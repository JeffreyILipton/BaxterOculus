using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LCM.LCM;
using UnityEngine;

namespace LCM
{
    public static class LCMManager
    {
        private static LCM.LCM myLCM; // the LCM object for all of Unity
        private static bool initialized;

        /// <summary>
        /// Initializes the LCM object to "udpm://239.255.76.67:7667:?ttl=1"
        /// </summary>
        public static void init()
        {
            myLCM = new LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");
            initialized = true;
        }

        /// <summary>
        /// Returns the LCM object to be used in all LCM functions in Unity
        /// If LCMManager has not been initialized, then it will be
        /// </summary>
        /// <returns></returns>
        public static LCM.LCM getLCM()
        {
            if (initialized != true)
            {
                init();
            }
            return myLCM;
        }

        public static void subscribeTo(string channelName, LCMSubscriber subscriber)
        {
            myLCM.Subscribe(channelName, subscriber);
        }

        public static bool isInitialized()
        {
            return initialized;
        }
    }
}
