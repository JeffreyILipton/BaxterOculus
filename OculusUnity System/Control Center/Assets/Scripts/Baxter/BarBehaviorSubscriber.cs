using UnityEngine;
using UnityEngine.UI;
using ProgressBar.Utils;
using System.Collections;
using LCM;
using ProgressBar;
using LCM.LCM;
using System;



public class BarBehaviorSubscriber : ChannelSubscriber
{
    public ProgressBarBehaviour bar; //The progress bar


    private double value;

    public override void HandleMessage(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        oculuslcm.range_t range = new oculuslcm.range_t(ins);
        value = range.range;
    }

    // Update is called once per frame
    void Update()
    {
        bar.SetFillerSize((float)value*100);
    }
}

