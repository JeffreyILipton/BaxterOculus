using oculuslcm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLauncher : MonoBehaviour   
{
    public robotself_t self;
    public void Launch()
    {
        HomunculusGlobals.instance.currentRobotSelf = self;
        SceneManager.LoadScene(self.type);
    }
}


