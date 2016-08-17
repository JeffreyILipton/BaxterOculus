using UnityEngine;
using System.Collections;

public class MasterInit : MonoBehaviour {

	// Use this for initialization
	/// <summary>
    /// Just initializes LCMManager
    /// </summary>
    void Start () {
        LCM.LCMManager.init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
