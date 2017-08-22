using oculuslcm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Detects if an Oculus button is pressed, and if so sends us back to lobby
/// </summary>
public class BackToLobby: MonoBehaviour
{

    public OVRInput.Controller LobbyController;
    public OVRInput.Button LobbyButton;

    public OVRInput.Controller AutoController;
    public OVRInput.Button AutoButton;

    private bool isAuto;

    public void toLobby()
    {
        SceneManager.LoadScene("Lobby");

        if (isAuto) {
            launchQuery(-1);
        } else
        {
            launchQuery(-2);
        }
    }

    public void setAutonomous(bool auto)
    {
        if (auto)
        {
            launchQuery(HomunculusGlobals.instance.userID * -1);
        } else
        {
            launchQuery(HomunculusGlobals.instance.userID);
        }
        isAuto = auto;
    }

    public void toggleAuto()
    {
        setAutonomous(!isAuto);
    }

    public void Update()
    {
        //Did we press the button? If so lets go back to lobby
        if (OVRInput.Get(LobbyButton, LobbyController) && SceneManager.GetActiveScene().name != "Lobby") {
            toLobby();
        } 

        if (OVRInput.Get(AutoButton,  AutoController)  && SceneManager.GetActiveScene().name != "Lobby") {
            toLobby();
        }
    }

    public void Start()
    {
        //We want this to stay with us to any scene we go to
        DontDestroyOnLoadManager.instance.ProtectObject(this.gameObject);
    }

    public void launchQuery(int value)
    {
        query_t query = new query_t();
        query.userID = (short)value; // We need to tell our robot we are leaving, otherwise it will send us right back when we get to the lobby
        LCM.LCMManager.lcmManager.getLCM().Publish(HomunculusGlobals.instance.CurrentRobotSelf.queryChannel, query);
    }
}
