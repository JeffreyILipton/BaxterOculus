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

    public void toLobby()
    {
        SceneManager.LoadScene("Lobby");

        // The lobby will reinstantiate all of our protected objects, so lets destroy them to avoid duplicates. This includes destroying ourselves.
        DontDestroyOnLoadManager.instance.DestroyAllProtectedObjects();
        query_t query = new query_t();
        query.userID = -1; // We need to tell our robot we are leaving, otherwise it will send us right back when we get to the lobby
        LCM.LCMManager.lcmManager.getLCM().Publish(HomunculusGlobals.instance.currentRobotSelf.queryChannel, query);
        
    }

    public void Update()
    {
        //Did we press the button? If so lets go back to lobby
        if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch) && SceneManager.GetActiveScene().name != "Lobby") {
            toLobby();
        }
    }

    public void Start()
    {
        //We want this to stay with us to any scene we go to
        DontDestroyOnLoadManager.instance.ProtectObject(this.gameObject);
    }
}
