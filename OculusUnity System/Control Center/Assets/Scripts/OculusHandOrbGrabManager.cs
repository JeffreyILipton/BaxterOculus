using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusHandOrbGrabManager : MonoBehaviour {
    private bool grabbing = false; //Are we trying to grab an object?
    public OculusHand hand; //The hand object we are passed through Unity's drag n drop interface

    private Material normalMat; //The material that looks like an actual hand
    private Material glowMat; //The material that looks like it is are part of the radiacive blue man group
    private Renderer rend; // the render object of the game object, this controls the way things are rendered and is where we can change materials
    private LCM.LCM.LCM myLCM; //The LCM object
    private oculuslcm.cmd_t command; //Used to send close and open grippers over LCM
    private oculuslcm.trigger_t trigger; //Used to shoot things of LCM
    private short prevCommand; // The previous command message sent
    private bool prevTrigger; // The previous trigger message

    // Use this for initialization
    void Start()
    {
        hand = gameObject.GetComponent(typeof(OculusHand)) as OculusHand; //Fetches he SixenseHand object within the gameObject
        rend = gameObject.GetComponentInChildren<Renderer>(); //Fetches the render object
        normalMat = rend.material; //Sets the normalMat as what the object is set to in Unity's enviornment (this should be the fleshy meterial
        glowMat = (Material)Resources.Load("Hands_Glow") as Material; // This loads the glow material, which MUST be located inside the resource folder
        myLCM = LCM.LCMManager.getLCM(); //Gets the LCM object
        command = new oculuslcm.cmd_t(); //Initializes the command message
        trigger = new oculuslcm.trigger_t(); //Initializes the trigger message
    }

    // Update is called once per frame
    void Update()
    {

         //grabbing = (bool)(hand.m_controller.GetButton(SixenseButtons.TRIGGER)); //Set grabbing to wether we ar pulling the trigger
        grabbing = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, hand.m_controller);


        if (hand.m_hand == OculusHands.LEFT)
        {
            // If the user is pressing the bumper, determine wether command.command is 1 or 0, only for left
            if (OVRInput.Get(OVRInput.Button.One))
            {
                command.command = 0;
            }
            else
            {
                command.command = 1;
            }


            if ( prevCommand != command.command) // Only send the message if the message is diffrent from the previous
            {
                myLCM.Publish(hand.m_hand.ToString().ToLower() + "_lcm_cmd", command);
            }

            prevCommand = command.command; //Updating the previous command

        }
        else if (hand.m_hand == OculusHands.RIGHT) {
            Vector2 xy = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, hand.m_controller);

            //If the user presses both four and the bumper then the trigger is pulled, other wise it is not. Only for the right hand
            if ( (xy[1]>0.5 )&& OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                trigger.trigger = true;
            }
            else
            {
                trigger.trigger = false;
            }

            if (prevTrigger != trigger.trigger) // Only send the message if the message is diffrent from the previous
            {
                myLCM.Publish(hand.m_hand.ToString().ToLower() + "_trigger_lcm", trigger);
            }

            prevTrigger = trigger.trigger; //Updating the previous trigger
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
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, hand.m_controller))
        {
            return new Quaternion((float).5, (float).5, (float)-.5, (float).5);
        }
        return gameObject.transform.rotation;
    }



    /// <summary>
    /// Set wether or not the hand is glowing
    /// True is glowing
    /// False is normal
    /// </summary>
    /// <param name="glowing"></param>
    public void setGlow(bool glowing)
    {
        if (glowing)
        {
            rend.material = glowMat;
        }
        else
        {
            rend.material = normalMat;
        }
    }
}
