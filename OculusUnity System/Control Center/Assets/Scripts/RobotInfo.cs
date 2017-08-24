using LCM.LCM;
using oculuslcm;
using ProgressBar;
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

    public Image Border;
    public Text IDText;
    public Text UserText;
    public Text RobotType;
    public Button LaunchButton;
    public SceneLauncherQuery launcher;
    public Image movingPriorityBar;
    public LCMtoTexture Monitor;

    /// <summary>
    /// Update our info_t data and apply it to the UI
    /// </summary>
    /// <param name="info"></param>
    public void RefreshInfo(info_t info)
    {
        this.info = info;
        launcher.info = info;
        IDText.text = "ID: " + info.id;
        if (info.user == -1)
        {
            UserText.text = "    AUTO";
            UserText.alignment = TextAnchor.UpperLeft;
            UserText.color = Color.green;

            Border.color = Color.green;

            ColorBlock colorBlock = LaunchButton.colors;
            colorBlock.highlightedColor = Color.green;
            LaunchButton.colors = colorBlock;

        }
        else if (info.user == -2)
        {
            UserText.text = "STOPPED";
            UserText.alignment = TextAnchor.UpperLeft;
            UserText.color = Color.red;

            Border.color = Color.red;

            ColorBlock colorBlock = LaunchButton.colors;
            colorBlock.highlightedColor = Color.green;
            LaunchButton.colors = colorBlock;
        }
        else if (info.user > 0)
        {
            UserText.text = "User #" + info.user + " Operating ";
            UserText.alignment = TextAnchor.UpperRight;
            UserText.color = Color.yellow;

            Border.color = Color.yellow; ;

            ColorBlock colorBlock = LaunchButton.colors;
            colorBlock.highlightedColor = Color.red;
            LaunchButton.colors = colorBlock;
        }
        else if (info.user < -2)
        {
            UserText.text = "User #" + -info.user + " Supervising";
            UserText.alignment = TextAnchor.UpperRight;
            UserText.color = Color.yellow;

            Border.color = Color.yellow; ;

            ColorBlock colorBlock = LaunchButton.colors;
            colorBlock.highlightedColor = Color.red;
            LaunchButton.colors = colorBlock;
        }

        movingPriorityBar.rectTransform.localScale = new Vector3((float)info.confidence, movingPriorityBar.rectTransform.localScale.y, movingPriorityBar.rectTransform.localScale.z);

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
        Monitor.Subscribe(self.ability);
    }
}
