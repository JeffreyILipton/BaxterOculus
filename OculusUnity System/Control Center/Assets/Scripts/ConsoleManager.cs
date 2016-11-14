using UnityEngine;
using System.Collections;

public class ConsoleManager : MonoBehaviour {
    public SixenseHand hand; // The hand objet handed to us through Unity's drag n drop editor, WILL NOT WORK IF NULL
    private AlphaManager alphaManager; // the script that we will call to change transperencty
    private bool state = true; //Are we trying to be visible or invisible

    // Use this for initialization
    void Start () {
        alphaManager = gameObject.GetComponentInChildren<AlphaManager>(); // Finds the AlphaManager within the gameObject and assignes it to the alphaManager object
    }
	
	// Update is called once per frame
	void Update () {
        if (hand.m_controller != null) //If the hands have been initialized (so they know which is left or right, code will work if this line is removed, but will throw errors
        {
            if (hand.m_controller.GetButtonDown(SixenseButtons.JOYSTICK) ) // If the four button on the designated hand is down
            {
                state = !state; //If up, then down, if down then up. Changes the target location
            }
            if (state) //If we want to be visible
            {
                alphaManager.fade(.1); // amount more visible per frame
            }
            else
            if (!state) //If we want to be invisible
            {
                alphaManager.fade(-.1); // amount less visible per frame
            }
        }
    }
}
