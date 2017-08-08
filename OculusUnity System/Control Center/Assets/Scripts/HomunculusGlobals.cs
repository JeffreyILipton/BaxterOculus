using oculuslcm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class containing the globably available information that we want to transcend scenes, 
/// even after the scripts that updated these values have been destroyed
/// </summary>
public class HomunculusGlobals : MonoBehaviour {

    /// <summary>
    /// The robotself_t data of the robot currently being targeted
    /// </summary>
    public robotself_t currentRobotSelf;

    /// <summary>
    /// The userID of this user. This is the client side unique identifier.
    /// </summary>
    public int userID;

    /// <summary>
    /// The object containing the globably available information that we want to transcend scenes, 
    /// even after the scripts that updated these values have been destroyed.
    /// </summary>
    public static HomunculusGlobals instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoadManager.instance.ProtectObject(this.gameObject);//This sets this to not be destoryed when the scene changes
        } else
        {
            Destroy(this);
            throw new Exception("Only one homunculous globals object should exist at once! Terminating new instance."); 
        }
    }
}
