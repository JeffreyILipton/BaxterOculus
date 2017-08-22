using System;
using LCM.LCM;
using UnityEngine;


/// <summary>
/// A MonoBehavior that implements LCMSubscriber. It is responsible for subscribing and unsubscribing itself.
/// It can only be subscribed to one channel at a time. It is intended to be a superclass. 
/// </summary>
public class ChannelSubscriber : MonoBehaviour, LCMSubscriber
{
    private string subscriptionChannel = "No Channel... Yet";

    private bool isInUse = true;

    /// <summary>
    /// What channel is this ChannelSubscriber subscribed to?
    /// </summary>
    /// <returns>
    /// What channel is this ChannelSubscriber subscribed to
    /// </returns>
    public string getSubscriptionChannel()
    {
        return subscriptionChannel;
    }

    /// <summary>
    /// Called because ChannelSubscriber implements LCMSubscriber. The actual processing of the message will be done in HandleMessage().
    /// </summary>
    /// <param name="lcm"></param>
    /// <param name="channel"></param>
    /// <param name="ins"></param>
    public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        if (channel.Equals(subscriptionChannel))
        {
            HandleMessage(lcm, channel, ins);
        }else
        {
            lcm.Unsubscribe(channel, this);
            throw new Exception
                ("Somehow a ChannelSubscriber recieved a message it isn't supposed to be subscribed to!" + 
                " Message was on channel " + channel + " not on " + subscriptionChannel + "!");
        }
    }
    /// <summary>
    /// Inteded to be overwritten. This where the data recieved over LCM is processed.
    /// </summary>
    /// <param name="lcm"></param>
    /// <param name="channel"></param>
    /// <param name="ins"></param>
    public virtual void HandleMessage(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {

    }

    /// <summary>
    /// Subscribe to <param name="channel"></param> and unsubscribe from the previous subscription.
    /// </summary>
    /// 
    public void Subscribe(string channel)
    {
        if (!subscriptionChannel.Equals("No Channel")) {
            Unsubscribe();
        }
        LCM.LCMManager.lcmManager.getLCM().Subscribe(channel, this);
        subscriptionChannel = channel;
    }

    /// <summary>
    /// Unsubscribe from previous subscription
    /// </summary>
    public void Unsubscribe()
    {
        LCM.LCMManager.lcmManager.getLCM().Unsubscribe("", this);
        subscriptionChannel = "No Channel";
    }

    /// <summary>
    /// Called if we aren't going to recieve anyinformation on the channel. Feel free to overide
    /// </summary>
    protected virtual void NotInUse()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called if we are going to recieve anyinformation on the channel. Feel free to overide
    /// </summary>
    protected virtual void InUse()
    {
        gameObject.SetActive(true);
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
            } else
            {
                NotInUse();
            }
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

}