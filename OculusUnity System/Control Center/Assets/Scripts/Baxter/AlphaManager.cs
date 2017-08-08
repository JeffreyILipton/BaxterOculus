using UnityEngine;
using System.Collections;

public class AlphaManager : MonoBehaviour {

    private Renderer [] render; // the render object of the game object, this controls the way things are rendered and is where we can change colors
    private double rate; // the rate at which the alpha value will change
    private double alphaValue = 1; // the value which the alpha channel of the material will reflect

    // Use this for initialization
    void Start () {
        render = gameObject.GetComponentsInChildren<Renderer>(); //Gets an array of renderer objects
    }
	
	
	/// <summary>
    /// Increases or decreases alphaValue by rate, untill the value is 1 or 0 
    /// </summary>
    void Update () {
        if (alphaValue > 1 && rate > 0) //if the value has reached the max level and continues to rise
        {
            rate = 0; //stop raising the alpha level
        } else if (alphaValue < 0 && rate < 0)//if the value has reached the minimum level and continues to fall
        {
            rate = 0;//stop lowering the alpha level
        }
        if (rate != 0) //if we are currently changing the alpha levels
        {
            alphaValue += rate; //Actualy change the value which will the material's aplha chanell will reflect
            setTotalAlphaInternal(alphaValue); //Pass to where the color is changed
        }
    }

    // <summary>
    // Sets the total visibility of the material applied to the orb, and halts any fading (rate set to 0)
    // </summary>
    // <param name="value"></param>
    public void setTotalAlpha(double value)
    {
        setTotalAlphaInternal(value);
        alphaValue = value;
        rate = 0;
    }

    /// <summary>
    /// Actualy applies the alphga value to the material
    /// </summary>
    /// <param name="value"></param>
    private void setTotalAlphaInternal(double value)
    {
        foreach (Renderer renderObj in render) //For each loop that fades both emmision and difuse alpha channelss
        {
            alphaValue = value;
            renderObj.material.color = new Color((float)renderObj.material.color.r, renderObj.material.color.g, renderObj.material.color.b, (float)value);
            renderObj.material.SetColor("_Color", new Color(renderObj.material.GetColor("_Color").r, renderObj.material.GetColor("_Color").g, renderObj.material.GetColor("_Color").b, (float)value));
            renderObj.material.SetColor("_EmissionColor", new Color(renderObj.material.GetColor("_EmissionColor").r, renderObj.material.GetColor("_EmissionColor").g, renderObj.material.GetColor("_EmissionColor").b, (float)value));
        }
    }

    /// <summary>
    /// Sets the rate at which the orb will fade in or out of visibility. Positive for visible, Negative for invisible
    /// </summary>
    /// <param name="rate"></param>
    public void fade(double rate)
    {
        this.rate = rate;
    }
}
