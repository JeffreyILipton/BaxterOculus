using oculuslcm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Displays a semi circle around one of the hands indicating the current confidence threshold.
/// It allows the user to set a new threshold and send it to the robot
/// </summary>
public class ConfidenceThresholdCircle : ChannelPublisher {

    public OVRInput.Controller m_controller;
    public OVRInput.Button m_button;
    public OVRInput.Touch m_touch;
    public Transform handTransform;
    public Image UserBar;
    public Image InfoBar;
    public Image background;
    public Text text;
    public float threshold;

    public Color infoColor;
    public Color interactColor;

    // Update is called once per frame
    void Update()
    {
        InfoBar.color = infoColor;
        UserBar.color = interactColor;

        //If we are just touching the button, not pressing we want to display the robot's current confidence threshold
        if (OVRInput.Get(m_touch, m_controller))
        {
            text.text = "" + (int)(HomunculusGlobals.instance.CurrentInfo.threshold * 100);

            background.fillAmount = .5f;
            InfoBar.fillAmount = HomunculusGlobals.instance.CurrentInfo.threshold / 2;

            background.transform.localRotation  = Quaternion.Euler(0, 0, handTransform.localRotation.eulerAngles.z); // Allows the rotation to stay constant regaurdless of hand rotation (Yes, I realize I could have just set the postion in global coordiantes, but this works aswell)
            text.transform.localRotation        = Quaternion.Euler(0, 180, -handTransform.localRotation.eulerAngles.z+90);
            InfoBar.transform.localRotation     = Quaternion.Euler(0, 0, handTransform.localRotation.eulerAngles.z);
            UserBar.transform.localRotation     = Quaternion.Euler(0, 0, handTransform.localRotation.eulerAngles.z);

        }
        //If we are no longer touching we want to hide the UI
        else if (OVRInput.GetUp(m_touch, m_controller))
        {
            text.text = "";
            InfoBar.fillAmount = 0;
            background.fillAmount = 0;
        }

        //If we are pressing down then we will calculate a new coinfidence thershold and display a new semi circle representing it
        if (OVRInput.Get(m_button, m_controller)){
            float imageFill = 1 - ((handTransform.localRotation.eulerAngles.z / 360) + .5f);
            if (imageFill > -.5 && imageFill < -.25)
            {
                imageFill = .5f;
            } else if(imageFill < 0)
            {
                imageFill = 0;
            }
            UserBar.fillAmount = imageFill;
            threshold = imageFill * 2;

            text.text = "" + (int)(threshold * 100);
        }

        //If we release the button then we send the LCM message for the new confidence threshold
        else if (OVRInput.GetUp(m_button, m_controller))
        {
            UserBar.fillAmount = 0;
            confidencethreshold_t message = new confidencethreshold_t();
            message.confidence = threshold;
            Send(message);
        }
    }

    /// <summary>
    /// Hide UI if we can't use it
    /// </summary>

    protected override void NotInUse()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Unhide UI if the channel is available
    /// </summary>
    /// 
    protected override void InUse()
    {
        gameObject.SetActive(true);
    }
}

