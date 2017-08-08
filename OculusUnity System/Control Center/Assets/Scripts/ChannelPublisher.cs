using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ChannelPublisher is a way to pass LCM publishers the channel to send at, and to facilitate sending
/// </summary>
public class ChannelPublisher : MonoBehaviour {

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
}
