using LCM.LCM;
using oculuslcm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The class containing the globably available information that we want to transcend scenes, 
/// even after the scripts that updated these values have been destroyed
/// </summary>
public class HomunculusGlobals : MonoBehaviour, LCMSubscriber{

    public string InfoChannel, SelfChannel;

    /// <summary>
    /// The robotself_t data of the robot currently being targeted
    /// </summary>
    private robotself_t currentRobotSelf;

    /// <summary>
    /// The info_t data of the robot currently being targeted
    /// </summary>
    private info_t currentInfo;

    /// <summary>
    /// The userID of this user. This is the client side unique identifier.
    /// </summary>
    public short userID;

    /// <summary>
    /// The object containing the globably available information that we want to transcend scenes, 
    /// even after the scripts that updated these values have been destroyed.
    /// </summary>
    public static HomunculusGlobals instance;

    #region Getters and Setters

    public info_t CurrentInfo
    {
        get
        {
            return currentInfo;
        }

        set
        {
            currentInfo = value;   
        }
    }

    public robotself_t CurrentRobotSelf
    {
        get
        {
            return currentRobotSelf;
        }

        set
        {
            currentRobotSelf = value;
        }
    }

    #endregion

    private void Awake()
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

    public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        if (channel.Contains("info"))
        {
            info_t info = new info_t(ins);
            if (info.id == CurrentRobotSelf.id)
            {
                CurrentInfo = info;
            }
        } else if (channel.Contains("self"))
        {
            robotself_t self = new robotself_t(ins);
            if (self.id == CurrentRobotSelf.id)
            {
                CurrentRobotSelf = self;
            }
        }
    }

    void OnLevelWasLoaded()
    {
        //We dont want to keep InfoManager from subscribing to info and self, so we unsubscribe from those channels
        if (SceneManager.GetActiveScene().name.Contains("Lobby") || SceneManager.GetActiveScene().name.Contains("Start"))
        {
            LCM.LCMManager.lcmManager.getLCM().Unsubscribe("", this);
        } else
        {
            LCM.LCMManager.lcmManager.getLCM().Subscribe(InfoChannel, this);
            LCM.LCMManager.lcmManager.getLCM().Subscribe(SelfChannel, this);
        }
    }
}
