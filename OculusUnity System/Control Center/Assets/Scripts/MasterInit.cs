using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
/// <summary>
/// Launches Lobby once everything is initialized
/// </summary>
public class MasterInit : MonoBehaviour {

    private long time;

    // Use this for initialization
    void Start () {
        time = DateTime.Now.Ticks;
	}
	
	// Update is called once per frame
	void Update () {
        if (DateTime.Now.Ticks - time > 100000)
        {
            SceneManager.LoadScene("Lobby");
        }
	}
}
