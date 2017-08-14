using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ChannelPublisher is a way to pass LCM publishers the channel to send at, and to facilitate sending
/// </summary>
public class ChannelPublisher : MonoBehaviour{

    private bool isInUse = true;

    /// <summary>
    /// The channel to publish LCM at
    /// </summary>
    public string sendChannel;

    /// <summary>
    /// Send our <paramref name="message"/> over sendChannel
    /// </summary>
    /// <param name="message"></param>
    public void Send(LCM.LCM.LCMEncodable message)
    {
        LCM.LCMManager.lcmManager.getLCM().Publish(sendChannel, message);
    }

    /// <summary>
    /// Called if we aren't going to recieve anyinformation on the channel. Feel free to overide
    /// </summary>
    protected virtual void NotInUse()
    {
    }

    /// <summary>
    /// Called if we are going to recieve anyinformation on the channel. Feel free to overide
    /// </summary>
    protected virtual void InUse()
    {
    }

    /// <summary>
    /// Is this channel active?
    /// </summary>
    public bool IsInUse
    {
        get
        {
            return isInUse;
        }
        set
        {
            isInUse = value;
            if (isInUse)
            {
                InUse();
            }
            else
            {
                NotInUse();
            }
        }
    }
}
