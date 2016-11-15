using UnityEngine;
using System.Collections;

/// <summary>
/// Will shrink the viewscreen based on a controller axis
/// </summary>
public class Shrink : MonoBehaviour
{
    public SixenseHand hand; // The hand objet handed to us through Unity's drag n drop editor, WILL NOT WORK IF NULL
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
        if (hand.m_controller != null) //if the hands have been initialized (so they know which is left or right, code will work if this line is removed, but will throw errors
        {
            float scaleFactor = (hand.m_controller.JoystickY * (float).3) * (Mathf.Abs(hand.m_controller.JoystickY) * (float).3) + 1; // No point in having it go extreme, lets do some math to make it seem smooth
            gameObject.transform.localScale = new Vector3(scaleFactor * initVector.x, scaleFactor * initVector.y, scaleFactor * initVector.z); //scale
        }
    }
}
