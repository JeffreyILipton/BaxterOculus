using UnityEngine;
using LCM;
using System.Collections;
using LCM.LCM;
using System;

public class ArrowReciever : ChannelSubscriber
{

    //public string channel;
    //public SelectedHand hand; //Hand given through Unity's drag n drop system
    private bool newData;
    private Quaternion orientation;
    private Vector3 positon;

    public override void HandleMessage(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        oculuslcm.pose_t pos = new oculuslcm.pose_t(ins);
        positon = new Vector3((float)-pos.position[1], (float)pos.position[2], (float)pos.position[0]);
        orientation = new Quaternion((float)pos.orientation[0], (float)pos.orientation[1], (float)pos.orientation[2], (float)pos.orientation[3]);
        newData = true;
    }
	
	// Update is called once per frame
	void Update () {
         if (newData) //We do this so that it gets called in the main thread
        {
            gameObject.transform.localPosition = positon;
            gameObject.transform.localRotation = orientation;
            newData = false;
        }

    }
}
