using UnityEngine;
using UnityEngine.UI;
using ProgressBar.Utils;
using System.Collections;
using LCM;
using ProgressBar;
using LCM.LCM;
using System;



public class BarBehaviorConfidence : MonoBehaviour
{
    public ProgressBarBehaviour bar; //The progress bar


    private double value;

    // Update is called once per frame
    void Update()
    {
        value = HomunculusGlobals.instance.CurrentInfo.confidence;
        bar.SetFillerSizeAsPercentage((float)value*100);
    }
}

