using UnityEngine;
using System.Collections;

/// <summary>
/// Simply resets the Rift's Orientation
/// </summary>
public class ResetRiftOrientation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.VR.InputTracking.Recenter();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
