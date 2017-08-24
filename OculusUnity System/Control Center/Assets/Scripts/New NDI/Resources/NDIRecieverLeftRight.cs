using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An NDIReciever that determines its sourceName from HomunculusGlobals based on Left or Right
/// </summary>
public class NDIRecieverLeftRight : NDIReciever
{
    public bool left; 

    // Use this for initialization
    void Start()
    {
        initTexture();
        if (left)
        {
            sourceName = HomunculusGlobals.instance.CurrentRobotSelf.leftNDIChannel;
        } else
        {
            sourceName = HomunculusGlobals.instance.CurrentRobotSelf.rightNDIChannel;
        }
    }
}
