using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraRotator : MonoBehaviour
{
    private bool grabbing = false; //Are we trying to grab an object?
    private Quaternion H;
    public OVRInput.Controller m_controller;

    // Use this for initialization
    void Start()
    {
        H = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //grabbing = (bool)(hand.m_controller.GetButton(SixenseButtons.TRIGGER)); //Set grabbing to wether we ar pulling the trigger
        bool newgrabbing = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, m_controller);

        if (newgrabbing || grabbing)
        {
            grabbing = newgrabbing;
            gameObject.transform.rotation = getRotation();
        }
    }

    /// <summary>
    /// Return a boolean indicating if the hand is attempting to grab an orb
    /// </summary>
    /// <returns></returns>
    public bool isGrabbing()
    {
        return grabbing;
    }

    /// <summary>
    /// Return a vector reflecting the hand's position
    /// </summary>
    /// <returns></returns>
    public Vector3 getPosition()
    {
        return gameObject.transform.position;
    }

    /// <summary>
    /// Return a quaternian reflecting the hand's rotation
    /// </summary>
    /// <returns></returns>
    public Quaternion getRotation()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, m_controller))
        {
            Quaternion Q = OVRInput.GetLocalControllerRotation(m_controller);
            double w = Q[0];
            double x = Q[1];
            double y = Q[2];
            double z = Q[3];
            double yaw = System.Math.Atan2(2 * (y * z + w * x), w * w - x * x - y * y + z * z);
            double pitch = System.Math.Asin(-2 * (x * z - w * z));
            double roll = System.Math.Atan2(2 * (x * y + w * z), w * w + x * x - y * y - z * z);
            Quaternion P = new Quaternion();
            P.w = (float)System.Math.Cos(-yaw / 2.0);
            P.x = 0;
            P.y = (float)System.Math.Sin(-yaw / 2.0);
            P.z = 0;

            return H*P;
            //            return new Quaternion((float).5, (float).5, (float)-.5, (float).5);
        }
        return  gameObject.transform.rotation;
    }

}
