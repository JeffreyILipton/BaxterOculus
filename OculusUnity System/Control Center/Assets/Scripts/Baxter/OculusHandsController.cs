using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusHandsController : MonoBehaviour {

    OculusHand[] m_hands;

    float m_sensitivity = 1.0f;
    bool m_bInitialized;
    private bool bPause;

    // Use this for initialization
    void Start()
    {
        m_bInitialized = true;
        m_hands = GetComponentsInChildren<OculusHand>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (OculusHand hand in m_hands)
        {
            if (m_bInitialized)
            {
                UpdateHand(hand);
            }
        }
    }


    /** Updates hand position and rotation */
    void UpdateHand(OculusHand hand)
    {
         hand.transform.localPosition = OVRInput.GetLocalControllerPosition(hand.m_controller) * m_sensitivity;
         hand.transform.localRotation = OVRInput.GetLocalControllerRotation(hand.m_controller) * hand.InitialRotation;
    }

}
