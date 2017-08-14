using oculuslcm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// The class that manages all of the subscribers that we may or may not use according to the global robotself_t data
/// </summary>
class BaxterAssetManager : MonoBehaviour
{
    /// <summary>
    /// The object that manages all of the subscribers that we may or may not use according to the global robotself_t data
    /// </summary>
    public static BaxterAssetManager baxterAssetManager;

    /// <summary>
    /// Update what channels are used from robotself_t data. And set the subscribers for these channels to be "in use" and the others to be "not in use"
    /// </summary>
    /// <param name="self"></param>
    public void updateFromRobotSelf(robotself_t self)
    {
        ArrayList channelSubscribers = BaxterDataManager.baxterDataManager.ChannelSubscribers;
        ArrayList channelPublishers = BaxterDataManager.baxterDataManager.ChannelPublishers;


        ArrayList enabledChannels = new ArrayList(self.channels);
        for (int i = 0; i < channelSubscribers.Count; i++)
        {
            ((ChannelSubscriber)channelSubscribers[i]).IsInUse = (enabledChannels.Contains(((ChannelSubscriber)channelSubscribers[i]).getSubscriptionChannel()));
        }

        for (int i = 0; i < channelPublishers.Count; i++)
        {
            ((ChannelPublisher)channelPublishers[i]).IsInUse = (enabledChannels.Contains(((ChannelPublisher)channelPublishers[i]).sendChannel));
        }

    }

    /// <summary>
    /// Update what channels are used from the globaly available robotself_t data. And set the subscribers for these channels to be "in use" and the others to be "not in use"
    /// </summary>
    /// <param name="self"></param>
    public void updateFromRobotSelf()
    {
        robotself_t self = HomunculusGlobals.instance.CurrentRobotSelf;
        updateFromRobotSelf(self);
    }

    private void OnEnable() //called inbetween awake and start
    {
        if (baxterAssetManager != null)
        {
            throw new Exception("Only one BaxterAssetManager can exist at a time!");
        }
        else
        {
            updateFromRobotSelf(HomunculusGlobals.instance.CurrentRobotSelf);
            baxterAssetManager = this;
        }
    }
}
