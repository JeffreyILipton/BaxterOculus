using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoTeleopIndicator : MonoBehaviour {

    public Text text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (HomunculusGlobals.instance.isAuto)
        {
            text.text = "AUTO";
            text.color = Color.green;
        } else
        {
            text.text = "TELEOP";
            text.color = Color.red;
        }
	}
}
