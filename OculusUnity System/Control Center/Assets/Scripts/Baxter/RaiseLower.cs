using UnityEngine;
using System.Collections;

/// <summary>
/// Will raise or lower the blast door when the button of the hand object is triggered
/// </summary>
public class RaiseLower : MonoBehaviour {
    public OculusHand hand; // The hand objet handed to us through Unity's drag n drop editor, WILL NOT WORK IF NULL
    public bool state; //Are we trying to go up or down
    public double roof = 4; //Max elevation for center of object
    public double floor = 0.2; //Min elevation of the center of the object
    public double speed = 0.02; //The rate, per frame, at which the blast door will rise

	// Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	/// <summary>
    /// Will raise or lower the blast door when the button of the hand object is triggered
    /// </summary>
    void Update () {

        //if (hand.m_controller.GetButtonDown(SixenseButtons.JOYSTICK)) // If the four button on the designated hand is down
        //{
        //    state = !state; //If up, then down, if down then up. Changes the target location
        //}
        //if (state && gameObject.transform.position.y > floor) // If we are above the floor and trying to go down
        //{
        //    gameObject.transform.Translate(0, (float)speed * -1, 0); //go down
        //}
        //else
        //if (!state && gameObject.transform.position.y < roof)// If we are below the roof and trying to go up
        //{
        //    gameObject.transform.Translate(0, (float)speed * 1, 0); //go up
        //}
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown,hand.m_controller) && gameObject.transform.position.y > floor) // If we are above the floor and trying to go down
        {
            gameObject.transform.Translate(0, (float)speed * -1, 0); //go down
        }
        else
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp, hand.m_controller) && gameObject.transform.position.y < roof)// If we are below the roof and trying to go up
        {
            gameObject.transform.Translate(0, (float)speed * 1, 0); //go up
        }
    }
}
