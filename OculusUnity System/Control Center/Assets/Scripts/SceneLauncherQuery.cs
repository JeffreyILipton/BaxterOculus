using oculuslcm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A Unity Component with the ability to query a robot to initiate user control
/// </summary>
public class SceneLauncherQuery : MonoBehaviour   
{
    public robotself_t self;
    public info_t info;
    public LCM.LCM.LCM myLCM;

    public void Start()
    {
        myLCM = LCM.LCMManager.lcmManager.getLCM();
    }

    /// <summary>
    /// Query the robot refered to in our robotself_t data as to allow control. This is effecly initiating user control.
    /// If this is called expect a new scene to launch.
    /// </summary>
    public void Query()
    {
        query_t query = new query_t();
        if (info.user == -1)
        {
            query.userID = (short)(-1 * HomunculusGlobals.instance.userID);
        } else
        {
            query.userID = HomunculusGlobals.instance.userID;
        }
        
        myLCM.Publish(self.queryChannel, query);
    }
}


