using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OculusHands
{
    UNKNOWN = 0,
    LEFT = 1,
    RIGHT = 2,
}

public class OculusHand : MonoBehaviour {
    public OculusHands m_hand;
    public OVRInput.Controller m_controller;

    Animator m_animator;
    float m_fLastTriggerVal;
    Vector3 m_initialPosition;
    Quaternion m_initialRotation;


    // Use this for initialization
    void Start () {
        // get the Animator
        m_animator = gameObject.GetComponent<Animator>();
        m_initialRotation = transform.localRotation;
        m_initialPosition = transform.localPosition;

    }

    // Update is called once per frame
    protected void Update()
    {
        if (m_controller != OVRInput.Controller.LTouch && m_controller != OVRInput.Controller.RTouch)
        {
            if (m_hand == OculusHands.LEFT)
            {
                m_controller = OVRInput.Controller.LTouch;
            }
            else
            {
                m_controller = OVRInput.Controller.RTouch;
            }
        }

        else if (m_animator != null)
        {
            UpdateHandAnimation();
        }
    }


    // Updates the animated object from controller input.
    protected void UpdateHandAnimation()
    {
        // Point
        if (!OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger,m_controller) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, m_controller))
        {
            m_animator.SetBool("Point", true);
        }
        else
        {
            m_animator.SetBool("Point", false);
        }

        // Grip Ball
        if ( (OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, m_controller) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, m_controller)) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, m_controller) )
        {
            m_animator.SetBool("GripBall", true);
        }
        else
        {
            m_animator.SetBool("GripBall", false);
        }

        // Hold Book
        if (OVRInput.Get(OVRInput.Button.Two, m_controller))
        {
            m_animator.SetBool("HoldBook", true);
        }
        else
        {
            m_animator.SetBool("HoldBook", false);
        }

        // Fist
        float fTriggerVal = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
        fTriggerVal = Mathf.Lerp(m_fLastTriggerVal, fTriggerVal, 0.1f);
        m_fLastTriggerVal = fTriggerVal;

        //if (fTriggerVal > 0.01f)
        if ((OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, m_controller) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, m_controller)) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, m_controller))
        {
            m_animator.SetBool("Fist", true);
        }
        else
        {
            m_animator.SetBool("Fist", false);
        }

        m_animator.SetFloat("FistAmount", fTriggerVal);

        // Idle
        if (m_animator.GetBool("Fist") == false &&
             m_animator.GetBool("HoldBook") == false &&
             m_animator.GetBool("GripBall") == false &&
             m_animator.GetBool("Point") == false)
        {
            m_animator.SetBool("Idle", true);
        }
        else
        {
            m_animator.SetBool("Idle", false);
        }
    }


    public Quaternion InitialRotation
    {
        get { return m_initialRotation; }
    }

    public Vector3 InitialPosition
    {
        get { return m_initialPosition; }
    }
}
