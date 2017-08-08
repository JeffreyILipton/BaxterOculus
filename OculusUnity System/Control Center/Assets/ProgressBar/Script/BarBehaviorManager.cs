using UnityEngine;
using UnityEngine.UI;
using ProgressBar.Utils;
using System.Collections;
using LCM;
using ProgressBar;
using LCM.LCM;
using System;



public class BarBehaviorManager : ChannelSubscriber
{
    public string channel;
    ProgressBarBehaviour bar; //The progress bar
    bool initialized; //Has LCM warmed up?


    private double value;

    public override void HandleMessage(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        oculuslcm.range_t range = new oculuslcm.range_t(ins);
        value = range.range;
    }

    // Use this for initialization
    void Start()
    {

        bar = gameObject.GetComponent(typeof(ProgressBarBehaviour)) as ProgressBarBehaviour;
        
    }

    // Update is called once per frame
    void Update()
    {
        bar.SetFillerSizeAsPercentage((float)value*100);
    }
}

