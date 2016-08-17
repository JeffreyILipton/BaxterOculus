﻿using UnityEngine;
using UnityEngine.UI;
using ProgressBar.Utils;
using System.Collections;
using LCM;
using ProgressBar;
using LCM.LCM;
using System;

public enum SelectedHand
{
    left = 0,
    right = 1,
}

public class BarBehaviorManager : MonoBehaviour, LCM.LCM.LCMSubscriber
{
    public SelectedHand hand; //Hand given through Unity's drag n drop system
    ProgressBarBehaviour bar; //The progress bar
    bool initialized; //Has LCM warmed up?


    private double value;

    public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        oculuslcm.range_t range = new oculuslcm.range_t(ins);
        value = range.range;
        //Debug.Log(range.range);
    }

    // Use this for initialization
    void Start()
    {

        bar = gameObject.GetComponent(typeof(ProgressBarBehaviour)) as ProgressBarBehaviour;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized && LCM.LCMManager.isInitialized())
        {
            LCMManager.getLCM().Subscribe(hand.ToString().ToLower() + "_lcm_range", this);
            initialized = true;
            //Debug.Log(initialized);
        }
        bar.SetFillerSizeAsPercentage((float)value*100);
    }
}
