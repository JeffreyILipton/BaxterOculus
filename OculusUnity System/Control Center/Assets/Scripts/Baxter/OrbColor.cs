using LCM.LCM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbColor : ChannelSubscriber
{
    private bool validBool;
    private Renderer rend; // the render object of the game object, this controls the way things are rendered and is where we can change colors
    // Use this for initialization
    void Awake () {
        rend = gameObject.GetComponentInChildren<Renderer>();//initialize the renderer
    }
	
	// Update is called once per frame
	void Update () {
        if (IsInUse)
        {
            if (validBool) // If the command being handed to baxter is valid, make the ball blue
            {
                rend.material.SetColor("_EmissionColor", new Color((float).4, (float).875, (float).875));
                rend.material.color = Color.blue;
            }
            else // Otherwise the ball is red and invalid
            {
                rend.material.SetColor("_EmissionColor", new Color((float).3, (float).05, (float).05));
                rend.material.color = Color.red;
            }
        } else
        {
            rend.material.SetColor("_EmissionColor", new Color((float).3, (float).3, (float).3));
            rend.material.color = Color.gray;
        }
    }

    /// <sum>
    /// Recieves the LCM message "hand"_lcm_valid and passes the value to validBool
    /// </summary>
    /// <param name="lcm"></param>
    /// <param name="channel"></param>
    /// <param name="ins"></param>
    public override void HandleMessage(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        oculuslcm.trigger_t valid = new oculuslcm.trigger_t(ins);
        validBool = valid.trigger;
    }
    protected override void NotInUse()//Don't want to disable object
    {
    }
}
