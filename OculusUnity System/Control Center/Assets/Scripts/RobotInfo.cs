using LCM.LCM;
using oculuslcm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// Keeps track of info_t and robotself_t and distributes data to display elements
/// </summary>
public class RobotInfo : MonoBehaviour {

    private info_t info;
    private robotself_t self;

    public Text IDText;
    public Text UserText;
    public Text PriorityText;
    public Text RobotType;
    public SceneLauncherQuery launcher;
    public LCMtoTexture Monitor;

    /// <summary>
    /// Update our info_t data and apply it to the UI
    /// </summary>
    /// <param name="info"></param>
    public void RefreshInfo (info_t info)
    {
        this.info = info;
        IDText.text         = "ID: " + info.id;
        UserText.text       = "User ID:" + info.user;
        PriorityText.text   = "Priority" + info.priority;
    }

    /// <summary>
    /// Update our robotself_t data and apply it to the UI
    /// </summary>
    /// <param name="robotSelf"></param>
    public void RefreshSelf(robotself_t robotSelf)
    {
        this.self  = robotSelf;
        RobotType.text  = robotSelf.type;
        launcher.self   = robotSelf;
    }
}
