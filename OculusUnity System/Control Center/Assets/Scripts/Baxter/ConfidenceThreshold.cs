using oculuslcm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfidenceThreshold : ChannelPublisher {

    public OVRInput.Controller m_controller;
    public OVRInput.Button m_button;
    public OVRInput.Touch m_touch;
    public Transform handTransform;
    public Image bar;
    public Image background;
    public float confidence;

    public Color viewColor;
    public Color interactColor;


    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(m_touch, m_controller))
        {
            background.fillAmount = .5f;
            bar.fillAmount = confidence / 2;

            background.transform.localRotation = Quaternion.Euler(0, 0, handTransform.localRotation.eulerAngles.z);
            bar.transform.localRotation = Quaternion.Euler(0, 0, handTransform.localRotation.eulerAngles.z);

            bar.color = viewColor;
        }
        else if (OVRInput.GetUp(m_touch, m_controller))
        {
            bar.fillAmount = 0;
            background.fillAmount = 0;
        }

        if (OVRInput.Get(m_button, m_controller)){
            float imageFill = 1 - ((handTransform.localRotation.eulerAngles.z / 360) + .5f);
            if (imageFill > -.5 && imageFill < -.25)
            {
                imageFill = .5f;
            }
            bar.fillAmount = imageFill;
            confidence = imageFill * 2;

            bar.color = interactColor;
        }

        else if (OVRInput.GetUp(m_button, m_controller))
        {
            confidence_t message = new confidence_t();
            message.confidence = confidence;
            Send(message);
        }
    }
}

