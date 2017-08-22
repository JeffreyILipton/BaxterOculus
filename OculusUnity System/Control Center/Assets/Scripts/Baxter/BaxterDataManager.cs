using System.Collections;
using System.Collections.Generic;
using LCM.LCM;
using UnityEngine;
using oculuslcm;
using System;

/// <summary>
/// The class that manages LCM subscriptions and LCM publishing within Baxter, 
/// and can keep track of data from multiple baxters if necesary.
/// </summary>
public class BaxterDataManager : MonoBehaviour
{
    /// <summary>
    /// The golbably available object that manages LCM subscriptions and LCM publishing within Baxter
    /// </summary>
    public static BaxterDataManager baxterDataManager;

    /// <summary>
    /// The dictionary the cotains the data for each robot
    /// </summary>
    public Dictionary<int, BaxterData> dataMap;

    /// <summary>
    /// The default right orb position
    /// </summary>
    public Transform defaultRightTransform;

    /// <summary>
    /// The default left orb position
    /// </summary>
    public Transform defaultLeftTransform;

    /// <summary>
    /// The ID number of the Baxter we are currently working with
    /// </summary>
    private int targetNumber;

    /// <summary>
    /// These publishers and senders will change channels to the correct robot
    /// </summary>
    public ChannelPublisher rightMoveSender, rightCommandSender, rightTriggerSender, leftMoveSender, leftCommandSender, leftTriggerSender, confidenceThresholdSender;
    public ChannelSubscriber rightValidReciever, rightHandCameraReciever, rightWristCameraReciever, rightCurrentPosReciever, leftValidReciever, leftHandCameraReciever, leftWristCameraReciever, leftCurrentPosReciever;

    /// <summary>
    /// An array of all the ChannelSubscribers, this for BaxterAssetManager to access
    /// </summary>
    public ArrayList ChannelSubscribers;

    /// <summary>
    /// An array of all the ChannelPublishers, this for BaxterAssetManager to access
    /// </summary>
    public ArrayList ChannelPublishers;

    private LCM.LCM.LCM myLCM;
    
    /// <summary>
    /// A struct that keeps track of previous orb positions and all of the channels used by the robot
    /// </summary>
    public struct BaxterData
    {
        public Vector3 rightOrbPosition, leftOrbPosition;
        public Quaternion rightOrbRotation, leftOrbRotation;
        public string rightMoveChannel, rightValidChannel, rightHandCameraChannel, rightWristCameraChannel, rightCommandChannel, rightTriggerChannel, rightCurrentPosChannel, leftMoveChannel, leftValidChannel, leftHandCameraChannel, leftWristCameraChannel, leftCommandChannel, leftTriggerChannel, leftCurrentPosChannel, confidenceThresholdChannel;
    }

    /// <summary>
    /// This is called before OnEnable() and before Start(). This is for initialization.
    /// </summary>
    void Awake()
    {
        if (baxterDataManager == null) //Only do this if this the first and only BaxterDataManager
        {
            myLCM = LCM.LCMManager.lcmManager.getLCM();
            dataMap = new Dictionary<int, BaxterData>();
            targetNumber = HomunculusGlobals.instance.CurrentRobotSelf.id;//Get us on the correct robot
            dataMap.Add(targetNumber, initalizeBaxterData(targetNumber)); //Set our channels

            //Lets actually subscribe to the channels whose names we determined in initalizeBaxterData()
            rightValidReciever.         Subscribe   (CurrentRightValidChannel);
            leftValidReciever.          Subscribe   (CurrentLeftValidChannel);
            rightHandCameraReciever.    Subscribe   (CurrentRightHandCameraChannel);
            leftHandCameraReciever.     Subscribe   (CurrentLeftHandCameraChannel);
            rightWristCameraReciever.   Subscribe   (CurrentRightWristCameraChannel);
            leftWristCameraReciever.    Subscribe   (CurrentLeftWristCameraChannel);
            rightCurrentPosReciever.    Subscribe   (CurrentRightPosChannel);
            leftCurrentPosReciever.     Subscribe   (CurrentLeftPosChannel);

            //Lets actually set the publishing channels whose names we determined in initalizeBaxterData()
            rightMoveSender.    sendChannel         = (CurrentRightMoveChannel);
            leftMoveSender.     sendChannel         = (CurrentLeftMoveChannel);
            rightCommandSender. sendChannel         = (CurrentRightCommandChannel);
            leftCommandSender.  sendChannel         = (CurrentLeftCommandChannel);
            rightTriggerSender. sendChannel         = (CurrentRightTriggerChannel);
            leftTriggerSender.  sendChannel         = (CurrentLeftTriggerChannel);
            confidenceThresholdSender.sendChannel   = ((CurrentThresholdSender));

            //Now lets add the subscribers to the ArrayList so that BaxterAssetManager (or whatever else) can iterate through them
            ChannelSubscribers = new ArrayList();
            ChannelSubscribers.Add(rightValidReciever);
            ChannelSubscribers.Add(leftValidReciever);
            ChannelSubscribers.Add(rightHandCameraReciever);
            ChannelSubscribers.Add(leftHandCameraReciever);
            ChannelSubscribers.Add(rightWristCameraReciever);
            ChannelSubscribers.Add(leftWristCameraReciever);
            ChannelSubscribers.Add(rightCurrentPosReciever);
            ChannelSubscribers.Add(leftCurrentPosReciever);

            ChannelPublishers = new ArrayList();
            ChannelPublishers.Add(rightMoveSender);
            ChannelPublishers.Add(rightCommandSender);
            ChannelPublishers.Add(rightTriggerSender);
            ChannelPublishers.Add(leftMoveSender);
            ChannelPublishers.Add(leftCommandSender);
            ChannelPublishers.Add(leftTriggerSender);
            ChannelPublishers.Add(confidenceThresholdSender);

            //Lets set our selves as the public static instance that is globaly available
            baxterDataManager = this;  
        }
        else
        {
            //We aren't the first... so therefore we are posers. POSERS MUST BE DESTROYED!!!
            Destroy(this.gameObject);
            throw new Exception("Only one BaxterDataManager can exist at a time!");
        }
    }

    /// <summary>
    /// Set our target robot to be the one in HomunculusGlobals 
    /// </summary>
    void Update()
    {
        //This means that only way to change the target baxter is through HomunculusGlobals
        if (HomunculusGlobals.instance.CurrentRobotSelf.id != targetNumber)
        {
            changeTargetBaxter(HomunculusGlobals.instance.CurrentRobotSelf.id);
        }
    }

    public BaxterData initalizeBaxterData(int id)
    {
        BaxterData tempData = new BaxterData();

        //Set the default position of the orbs
        tempData.rightOrbPosition   = defaultRightTransform.position;
        tempData.leftOrbPosition    = defaultLeftTransform.position;
        tempData.rightOrbRotation   = defaultRightTransform.rotation;
        tempData.leftOrbRotation    = defaultLeftTransform.rotation;

        //Set the channel names. The format includes adding the id number to the end
        tempData.rightHandCameraChannel     = "right_lcm_camera-"           + id;
        tempData.leftHandCameraChannel      = "left_lcm_camera-"            + id;
        tempData.rightWristCameraChannel    = "right_lcm_camera-"           + id;
        tempData.leftWristCameraChannel     = "left_lcm_camera-"            + id;
        tempData.rightValidChannel          = "right_lcm_valid-"            + id;
        tempData.leftValidChannel           = "left_lcm_valid-"             + id;
        tempData.rightMoveChannel           = "right_lcm-"                  + id;
        tempData.leftMoveChannel            = "left_lcm-"                   + id;
        tempData.rightCommandChannel        = "right_lcm_cmd-"              + id;
        tempData.leftCommandChannel         = "left_lcm_cmd-"               + id;
        tempData.rightTriggerChannel        = "right_trigger_lcm-"          + id;
        tempData.leftTriggerChannel         = "left_trigger_lcm-"           + id;
        tempData.rightCurrentPosChannel     = "right_lcm_currentpos-"       + id;
        tempData.leftCurrentPosChannel      = "left_lcm_currentpos-"        + id;
        tempData.confidenceThresholdChannel = "confidence_threshold_lcm-"   + id;

        return tempData;
    }

    /// <summary>
    /// A method to change our focus and control to the correct Baxter
    /// </summary>
    /// <param name="targetNumber">
    /// The ID number of the Baxter</param>
    void changeTargetBaxter(int targetNumber)
    {
        this.targetNumber = targetNumber;

        if (!dataMap.ContainsKey(targetNumber)) //If we have not worked with this baxter yet, its time to initialize a BaxterData for it 
        {
            dataMap.Add(targetNumber, initalizeBaxterData(targetNumber));
        }

        //Lets fetch the new channels and apply them
        rightMoveSender     .sendChannel    = CurrentRightMoveChannel;
        leftMoveSender      .sendChannel    = CurrentLeftMoveChannel;
        rightCommandSender  .sendChannel    = CurrentRightCommandChannel;
        leftCommandSender   .sendChannel    = CurrentLeftCommandChannel;
        rightTriggerSender  .sendChannel    = CurrentRightTriggerChannel;
        leftTriggerSender   .sendChannel    = CurrentLeftTriggerChannel;

        rightValidReciever      .Subscribe  (CurrentRightValidChannel);
        leftValidReciever       .Subscribe  (CurrentLeftValidChannel);
        rightHandCameraReciever .Subscribe  (CurrentRightHandCameraChannel);
        leftHandCameraReciever  .Subscribe  (CurrentLeftHandCameraChannel);
        rightWristCameraReciever.Subscribe  (CurrentRightWristCameraChannel);
        leftWristCameraReciever .Subscribe  (CurrentLeftWristCameraChannel);
        rightCurrentPosReciever .Subscribe  (CurrentRightPosChannel);
        leftCurrentPosReciever  .Subscribe  (CurrentLeftPosChannel);

    }


    #region Getters and Setters
    public Vector3 CurrentRightPosition
    {
        get
        {
            return dataMap[targetNumber].rightOrbPosition;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightOrbPosition = value;
            dataMap[targetNumber] = tempData;
        }

    }

    public Vector3 CurrentLeftPosition
    {
        get
        {
            return dataMap[targetNumber].leftOrbPosition;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.leftOrbPosition = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public Quaternion CurrentRightRotation
    {
        get
        {
            return dataMap[targetNumber].rightOrbRotation;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightOrbRotation = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public Quaternion CurrentLeftRotation
    {
        get
        {
            return dataMap[targetNumber].leftOrbRotation;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.leftOrbRotation = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentRightHandCameraChannel
    {
        get
        {
            return dataMap[targetNumber].rightHandCameraChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightHandCameraChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentLeftHandCameraChannel
    {
        get
        {
            return dataMap[targetNumber].leftHandCameraChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.leftHandCameraChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentRightWristCameraChannel
    {
        get
        {
            return dataMap[targetNumber].rightWristCameraChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightWristCameraChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentLeftWristCameraChannel
    {
        get
        {
            return dataMap[targetNumber].leftWristCameraChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.leftWristCameraChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentRightPosChannel
    {
        get
        {
            return dataMap[targetNumber].rightCurrentPosChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightCurrentPosChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentLeftPosChannel
    {
        get
        {
            return dataMap[targetNumber].leftCurrentPosChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.leftCurrentPosChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentRightValidChannel
    {
        get
        {
            return dataMap[targetNumber].rightValidChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightValidChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentLeftValidChannel
    {
        get
        {
            return dataMap[targetNumber].leftValidChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.leftValidChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentRightMoveChannel
    {
        get
        {
            return dataMap[targetNumber].rightMoveChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightMoveChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentLeftMoveChannel
    {
        get
        {
            return dataMap[targetNumber].leftMoveChannel;
        }

        set
        {

            BaxterData tempData = dataMap[targetNumber];
            tempData.leftMoveChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }
    public string CurrentRightCommandChannel
    {
        get
        {
            return dataMap[targetNumber].leftCommandChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.leftCommandChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentLeftCommandChannel
    {
        get
        {
            return dataMap[targetNumber].rightCommandChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightCommandChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }
    public string CurrentRightTriggerChannel
    {
        get
        {
            return dataMap[targetNumber].rightTriggerChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.rightTriggerChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentLeftTriggerChannel
    {
        get
        {
            return dataMap[targetNumber].leftTriggerChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.leftTriggerChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }

    public string CurrentThresholdSender
    {
        get
        {
            return dataMap[targetNumber].confidenceThresholdChannel;
        }

        set
        {
            BaxterData tempData = dataMap[targetNumber];
            tempData.confidenceThresholdChannel = value;
            dataMap[targetNumber] = tempData;
        }
    }
    #endregion
}