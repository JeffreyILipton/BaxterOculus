using UnityEngine;
using System.Collections;
using System;
using LCM.LCM;

public class OrbMain : MonoBehaviour, LCM.LCM.LCMSubscriber
{

    public string ValidChannel;
    public string MoveChannel;
    public GameObject holderObject; // the object for the hand holding us. Set in the Unity drag n drop interface. WILL NOT RUN WITHOUT
    private OculusHandOrbGrabManager holderScript;// the main script for comunicaiting with our hand

    private bool held; // are we being held by the hand?
    private bool prevHeld; // was the orb held in the previous frame?

    private Vector3 offsetPosition; // the vector of the diffrence between the orb and the hand at the begining of the grab
    private Vector3 position; // the vector of our position
    private Quaternion rotation; // rotation of the object as a quaternian
    private bool inRange; // is the the orb in range of the hands

    private LCM.LCM.LCM myLCM; // the LCM object we will be using
    private oculuslcm.pose_t msg; // the LCM message we will pass that describes the target hand location for baxter 
    private bool initializedLCM; // has LCM been initialized?
    private bool validBool; // boolean that reflects if LCM tells us our target position is valid
    private long prevTimeStamp; //time of the last frame

    private Renderer rend; // the render object of the game object, this controls the way things are rendered and is where we can change colors
    private double range = .15; //max distance betgween the center of the hand and the orb where the hand can still grab the orb
    private AlphaManager alphaManager; // the script that we will call to change transperencty


    // Use this for initialization
    void Start () {
        holderScript = (OculusHandOrbGrabManager)(holderObject.GetComponent(typeof(OculusHandOrbGrabManager))); // We search for the HandOrbGrabManager script and assugn it to the holderScript
        alphaManager = gameObject.GetComponentInChildren<AlphaManager>(); // Finds the AlphaManager within the gameObject and assignes it to the alphaManager object
        position = gameObject.transform.position; // Initializes the position variable to that of the object
        rotation = gameObject.transform.rotation; // Initializes the rotation variable to that of the object
        rend = gameObject.GetComponentInChildren<Renderer>();//initialize the renderer


        msg = new oculuslcm.pose_t(); //Initializes the message we will be sending through LCM 

        prevTimeStamp = System.DateTime.Now.Ticks; //Initializes the prevTimeStamp to the current time

    }
    

    // Update is called once per frame
    void Update()
    {
        /* If this instance of orb main is yet to properley intitialize,
        then set the myLCM object to the global LCM from LCMManager and mark the initialized variable true to prevent this being called twice. */

        if (!initializedLCM && LCM.LCMManager.isInitialized())
        {
            myLCM = LCM.LCMManager.getLCM();
            myLCM.Subscribe(ValidChannel, this);
            initializedLCM = true;
        }

        checkForHandInRange(range); // Check if the Orb is in range to be grabed by the hand 
        if (inRange) { // If the hand is in range
            if (!held && holderScript.isGrabbing()) // the first frame the ball is held
            {
                held = true; //We are now being held!
                offsetPosition = holderObject.transform.InverseTransformPoint(gameObject.transform.position);
            }
            else if (!holderScript.isGrabbing()) // if the hand isnt actualy grabbing
            {
                held = false; //The grip is released, we are no longer held
            }
            if (held) // while the ball is held
            {
                position = holderObject.transform.TransformPoint(offsetPosition); //applying the transform of the offset to the new position of the holder
                rotation = holderScript.getRotation(); //update rotation
               //render.enabled = false;
                holderScript.setGlow(true); //hand turns blue
                alphaManager.fade(-.1);
            }
            else { // while the ball is released
                //render.enabled = true;
                holderScript.setGlow(false); //hand turns red
                alphaManager.fade(.1);
            }
            if (Vector3.Distance(holderObject.transform.position, position) > .2) //if the ball magically is moved by some outside force, we will stop holding if it gets to far
            {
                inRange = false;
            }
        }

        if (validBool) // If the command being handed to baxter is valid, make the ball blue
        {
            rend.material.SetColor("_EmissionColor", new Color((float).4, (float).875, (float).875));
            rend.material.color = Color.blue;
        }else // Otherwise the ball is red and invalid
        {
            rend.material.SetColor("_EmissionColor", new Color((float).3, (float).05, (float).05));
            rend.material.color = Color.red;
        }




        gameObject.transform.position = position; // applying our changes
        gameObject.transform.rotation = rotation; // applying our changes



        //mapping of position coordinates to baxter done on unity side
        msg.position[0] =  gameObject.transform.localPosition.z; //position.z;
        msg.position[1] = -gameObject.transform.localPosition.x; //-position.x;
        msg.position[2] =  gameObject.transform.localPosition.y; //position.y;

        //mapping of quaternian coordinates to baxter done on baxter side
        msg.orientation[0] = rotation.w;
        msg.orientation[1] = rotation.x;
        msg.orientation[2] = rotation.y;
        msg.orientation[3] = rotation.z;


        if (!held && prevHeld) // If ball is being released at a new location
        {
            prevTimeStamp = System.DateTime.Now.Ticks; // left in case message passing is done by time, not by release
            myLCM.Publish(MoveChannel, msg); //publish the message!
            
        }
        prevHeld = held;
    } 

 
    /// <summary>
    /// Checks if the orb and the holderObject (the hand) are within the distance of double range
    /// </summary>
    /// <param name="range"></param>
    private void checkForHandInRange(double range)
    {
        if (Vector3.Distance(holderObject.transform.position, position) < range)
        {
            inRange = true;
        }
    }
    /// <summary>
    /// Returns the position
    /// </summary>
    /// <returns></returns>
    public Vector3 getPosition()
    {
        return position;
    }

    /// <summary>
    /// Recieves the LCM message "hand"_lcm_valid and passes the value to validBool
    /// </summary>
    /// <param name="lcm"></param>
    /// <param name="channel"></param>
    /// <param name="ins"></param>
    public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        oculuslcm.trigger_t valid = new oculuslcm.trigger_t(ins);
        validBool = valid.trigger;
        //Debug.Log(valid.trigger);
    }
}
