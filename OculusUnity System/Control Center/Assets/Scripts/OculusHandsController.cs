using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusHandsController : MonoBehaviour {

    OculusHand[] m_hands;

    Vector3 m_baseOffset;
    float m_sensitivity = 1.0f;//0.001f; // Sixense units are in mm
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
        //bool bResetHandPosition = false;

        foreach (OculusHand hand in m_hands)
        {
            //    if (IsControllerActive(hand.m_controller) && hand.m_controller.GetButtonDown(SixenseButtons.START))
            //    {
            //        bResetHandPosition = true;
            //    }

            if (m_bInitialized)
            {
                UpdateHand(hand);
            }
        }

        m_baseOffset = Vector3.zero;

        //if (bResetHandPosition)
        //{
        //    m_bInitialized = true;

        //    m_baseOffset = Vector3.zero;

        //    // Get the base offset assuming forward facing down the z axis of the base
        //    foreach (OculusHand hand in m_hands)
        //    {
        //        m_baseOffset += OVRInput.GetLocalControllerPosition(hand.m_controller);
        //        //m_baseOffset += hand.m_controller.Position;
        //    }

        //    m_baseOffset /= 2;
        //}
    }


    /** Updates hand position and rotation */
    void UpdateHand(OculusHand hand)
    {
         hand.transform.localPosition = (OVRInput.GetLocalControllerPosition(hand.m_controller)) * m_sensitivity;
         hand.transform.localRotation = OVRInput.GetLocalControllerRotation(hand.m_controller) * hand.InitialRotation;
    }

}
