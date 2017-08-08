using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LCM.LCM;
using UnityEngine;

namespace LCM
{
    public class LCMManager : MonoBehaviour
    {
        private LCM.LCM myLCM; // the LCM object for all of Unity
        private bool initialized;
        public static LCMManager lcmManager;

        /// <summary>
        /// Initializes the LCM object to "udpm://239.255.76.67:7667:?ttl=1"
        /// Awake() is called before Start()
        /// </summary>
        public void Awake()
        {
            myLCM = new LCM.LCM("udpm://239.255.76.67:7667:?ttl=1");
            lcmManager = this;
            initialized = true;
            DontDestroyOnLoadManager.instance.ProtectObject(this.gameObject);
        }

        /// <summary>
        /// Returns the LCM object to be used in all LCM functions in Unity
        /// If LCMManager has not been initialized, then it will be
        /// </summary>
        /// <returns></returns>
        public LCM.LCM getLCM()
        {
            if (initialized != true)
            {
                Awake();
            }
            return myLCM;
        }

        public void subscribeTo(string channelName, LCMSubscriber subscriber)
        {
            myLCM.Subscribe(channelName, subscriber);
        }

        public bool isInitialized()
        {
            return initialized;
        }

        private void OnApplicationQuit()
        {
            if (initialized)
            {
                myLCM.Close();
                initialized = false;
            }
        }
        
    }
}
