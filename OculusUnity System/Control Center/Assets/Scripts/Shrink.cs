using UnityEngine;
using System.Collections;

/// <summary>
/// Will shrink the viewscreen based on a controller axis
/// </summary>
public class Shrink : MonoBehaviour
{
    public OculusHand hand; // The hand objet handed to us through Unity's drag n drop editor, WILL NOT WORK IF NULL
    private Vector3 initVector;

    // Use this for initialization
    void Start()
    {
        initVector = gameObject.transform.localScale; //Original size to scale off of
    }

    // Update is called once per frame
    /// <summary>
    /// Scales the image according to the joystick
    /// </summary>
    void Update()
    {
        Vector2 vals = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, hand.m_controller);
        float yval = vals[0];
        //float scaleFactor = (hand.m_controller.JoystickY * (float).3) * (Mathf.Abs(hand.m_controller.JoystickY) * (float).3) + 1; // No point in having it go extreme, lets do some math to make it seem smooth
        float scaleFactor = (yval * (float).3) * (Mathf.Abs(yval) * (float).3) + 1; // No point in having it go extreme, lets do some math to make it seem smooth
        if (Mathf.Abs(vals[1]) < 0.2){
            gameObject.transform.localScale = new Vector3(scaleFactor * initVector.x, scaleFactor * initVector.y, scaleFactor * initVector.z); //scale
        }
        
        
    }
}
