using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OculusHandOrbGrabManager : MonoBehaviour {
    private bool grabbing = false; //Are we trying to grab an object?
    public OVRInput.Controller m_controller; 
    public OVRInput.Button GripButton;
    public OVRInput.Button CommandButton;
    public OVRInput.Button TriggerButton; 

    private Material normalMat; //The material that looks like an actual hand
    private Material glowMat; //The material that looks like it is are part of the radiacive blue man group
    private Renderer rend; // the render object of the game object, this controls the way things are rendered and is where we can change materials
    private LCM.LCM.LCM myLCM; //The LCM object
    private oculuslcm.cmd_t command; //Used to send close and open grippers over LCM
    private oculuslcm.trigger_t trigger; //Used to shoot things of LCM
    private short prevCommand; // The previous command message sent
    private bool prevTrigger; // The previous trigger message

    public ChannelPublisher triggerSender;
    public ChannelPublisher commandSender;

    // Use this for initialization
    void Start()
    {
        rend = gameObject.GetComponentInChildren<Renderer>(); //Fetches the render object
        normalMat = rend.material; //Sets the normalMat as what the object is set to in Unity's enviornment (this should be the fleshy meterial
        glowMat = (Material)Resources.Load("Hands_Glow") as Material; // This loads the glow material, which MUST be located inside the resource folder
        myLCM = LCM.LCMManager.lcmManager.getLCM(); //Gets the LCM object
        command = new oculuslcm.cmd_t(); //Initializes the command message
        trigger = new oculuslcm.trigger_t(); //Initializes the trigger message
        prevCommand = -1;
    }

    // Update is called once per frame
    void Update()
    {
        grabbing = OVRInput.Get(GripButton, m_controller);//Set grabbing to wether we are pulling the trigger

        if (m_controller == OVRInput.Controller.LTouch)
        {
            // If the user is pressing the bumper, determine wether command.command is 1 or 0, only for left
            if (OVRInput.Get(CommandButton, m_controller))
            {
                command.command = 0;
            }
            else
            {
                command.command = 1;
            }


            if ( prevCommand != command.command) // Only send the message if the message is diffrent from the previous
            {
                    commandSender.Send(command);
            }

            prevCommand = command.command; //Updating the previous command

        }
        else if (m_controller == OVRInput.Controller.RTouch) {
            Vector2 xy = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller);

            //If the user presses both four and the bumper then the trigger is pulled, other wise it is not. Only for the right hand
            trigger.trigger = ((xy[1] > 0.5) && OVRInput.Get(OVRInput.Button.PrimaryThumbstick));

            if (prevTrigger != trigger.trigger) // Only send the message if the message is diffrent from the previous
            {
                triggerSender.Send(trigger);
            }

            prevTrigger = trigger.trigger; //Updating the previous trigger



            if (OVRInput.Get(CommandButton, m_controller))
            {
                command.command = 0;
            }
            else
            {
                command.command = 1;
            }


            if (prevCommand != command.command) // Only send the message if the message is diffrent from the previous
            {
                commandSender.Send(command);
            }

            prevCommand = command.command; //Updating the previous command

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
        if (OVRInput.Get(TriggerButton, m_controller))
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
            P.w = (float)System.Math.Cos(yaw / 2.0);
            P.x = 0;
            P.y = 0;
            P.z = (float)System.Math.Sin(yaw / 2.0);
            Quaternion R = new Quaternion((float).5, (float).5, (float)-.5, (float).5);

            return R*P ;
//            return new Quaternion((float).5, (float).5, (float)-.5, (float).5);
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
