using LCM.LCM;
using oculuslcm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Info Manager subscribes to all info_t and robotself_t type messages and sends them to "Robot Info" Prefabs. 
/// Info Manager will create prefabs when it recieves a robotself_t message with an ID it has not seen before.
/// Info Manager will launch into a scene when it detects that a robot is asking to be controlled
/// </summary>
public class InfoManager : MonoBehaviour, LCMSubscriber
{
    /// <summary>
    /// The globaly available InfoManager object. Unity doesn't work in a way as to allow truly static MonoBehavior objects, this is a work around.
    /// </summary>
    public static InfoManager instance;

    /// <summary>
    /// Our LCM object
    /// </summary>
    LCM.LCM.LCM myLCM;

    /// <summary>
    /// This dictionary allows us to keep track of multiple Baxters without switching scenes, which could be usefull. 
    /// The id number of the robot is the key
    /// </summary>
    Dictionary<int, InfoBlock> infoMap = new Dictionary<int, InfoBlock>();

    /// <summary>
    /// Will spawn fake Robot Infos when true. It will then return to false.
    /// </summary>
    public bool Test = false;

    /// <summary>
    /// The name of the channel where global info data can be recieved
    /// </summary>
    public string infoChannel;

    /// <summary>
    /// The name of the channel where global robotself data can be recieved
    /// </summary>
    public string robotselfChannel;

    /// <summary>
    /// A queue of robotself_t data. We recieve data on a seperate thread than the thread we want to process it on.
    /// This allows us to process the data on the main unity thread.
    /// </summary>
    public Queue<robotself_t> selfQueue;

    /// <summary>
    /// A queue of info_t data. We recieve data on a seperate thread than the thread we want to process it on.
    /// This allows us to process the data on the main unity thread.
    /// </summary>
    public Queue<info_t> infoQueue;

    /// <summary>
    /// This must be assigned in the Unity Drag and Drop enviornment. 
    /// It contains a canvas with a RobotInfo script along with the visual displays, monitors and the buttons needed to query a robot to allow control.
    /// </summary>
    public GameObject RobotInfoPrefab;

    /// <summary>
    /// The information we want to keep track of per robot.
    /// </summary>
    public struct InfoBlock
    {
        public info_t info;
        public robotself_t self;
        public GameObject prefabObject;
    }

    /// <summary>
    /// Called by LCM when a message is recieved on a channel it subscribes to. 
    /// It will then enqueue this data for processing in the main thread (update)
    /// </summary>
    public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        if (channel.Equals(infoChannel))
        {
            infoQueue.Enqueue(new info_t(ins));

        } else if (channel.Equals(robotselfChannel))
        {
            selfQueue.Enqueue(new robotself_t(ins));
        }
    }

    /// <summary>
    /// This is the method that will physically arange the "RobotInfo" prefab objects in the lobby.
    /// </summary>
    public void UpdateLayout()
    {
        KeyValuePair<int, InfoBlock>[] array = infoMap.ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].Value.prefabObject.transform.localPosition = transfomPositionByNumber(i, array.Length-1);
            array[i].Value.prefabObject.transform.rotation = Quaternion.AngleAxis((float)(transfomAngleByNumber(i, array.Length-1)), new Vector3(0, 1, 0));
                //SetAxisAngle(new Vector3(0, 1, 0), (float)Math.Atan(array[i].Value.prefabObject.transform.localPosition.x));
        }
    }

    /// <summary>
    /// This is where the calculations are done to determine physical location for the "RobotInfo" prefab objects.
    /// </summary>
    /// <param name="num">
    /// The row location of the object.
    /// </param>
    /// /// <param name="total">
    /// The total objects in the row.
    /// </param>
    /// <returns></returns>
    private Vector3 transfomPositionByNumber(int num, int total)
    {
        return new Vector3(Mathf.Sin( transfomAngleByNumber(num, total) * Mathf.PI / 180) * 2,0, (Mathf.Cos( transfomAngleByNumber(num, total) * Mathf.PI / 180) * 2) - 1.75f);
        //return new Vector3(1.1f * (num - total/2) * RobotInfoPrefab.GetComponent<RectTransform>().rect.width * RobotInfoPrefab.transform.localScale.x , 0, 0);
    }

    /// <summary>
    /// This is where the calculations are done to determine the y angle in degrees for the "RobotInfo" prefab objects.
    /// </summary>
    /// <param name="num">
    /// The row location of the object.
    /// </param>
    /// /// <param name="total">
    /// The total objects in the row.
    /// </param>
    /// <returns></returns>
    private float transfomAngleByNumber(int num, int total)
    {
        return -((float)total / 2 - num) * .4f * (180 / Mathf.PI); ;
        }

    /// <summary>
    /// Will update the info_t data within the map and in the "Robot Info" object.
    /// If the info_t user id matches the local user ID, it will open the scene determined by type in self_t.
    /// </summary>
    /// <param name="info">
    /// The new info_t data
    /// </param>
    public void UpdateMapInfo (info_t info)
    {
        InfoBlock newInfoBlock;
        newInfoBlock.info = info;
        // Not going to do anything with the data unless we have already recieved the robotself_t data which is periodically sent.
        if (infoMap.ContainsKey(info.id)) 
        {
            newInfoBlock.prefabObject = infoMap[info.id].prefabObject; // Making our new object and old object the exact same
            newInfoBlock.self = infoMap[info.id].self; // Matching our new and olf self data
            newInfoBlock.prefabObject.GetComponentInChildren<RobotInfo>().RefreshInfo(info); //Sending our RobotInfo script the new info
           
            infoMap[info.id] = newInfoBlock; //Applying our changes to the map

            if (info.user == HomunculusGlobals.instance.userID && SceneManager.GetActiveScene().name.Equals("Lobby")) //Only would want this to work if we are in the lobby
            {
                HomunculusGlobals.instance.currentRobotSelf = infoMap[info.id].self; //The next scene will need our self information
                SceneManager.LoadScene(infoMap[info.id].self.type);
            }
        }
    }

    /// <summary>
    /// Will update the robotself_t data within the map and in the "Robot Info" object.
    /// If the map has no key 
    /// </summary>
    /// <param name="self">
    /// The new robotself_t data
    /// </param>
    public void UpdateMapSelf(robotself_t self)
    {

        InfoBlock newInfoBlock;
        newInfoBlock.self = self;
        if (infoMap.ContainsKey(self.id))//If we already have data for this robot
        {
            newInfoBlock.prefabObject = infoMap[self.id].prefabObject; // Making our new object and old object the exact same
            newInfoBlock.info   = infoMap[self.id].info; // Matching our new and olf self data

            newInfoBlock.prefabObject.GetComponentInChildren<RobotInfo>().RefreshSelf(self); //Sending our RobotInfo script the new robotself_t data

            infoMap[self.id]    = newInfoBlock; //Applying our changes to the map
        }
        else //Time to spawn a new one!
        {
            newInfoBlock.prefabObject = Instantiate(RobotInfoPrefab, this.transform); //Spawn the prefab
            newInfoBlock.info   = new info_t(); //Don't have the info yet (or atleast we may have been ignoring previously)

            newInfoBlock.prefabObject.GetComponentInChildren<RobotInfo>().RefreshSelf(self); //Apply the robotself_t data

            myLCM.Subscribe("monitor_" + self.id, newInfoBlock.prefabObject.GetComponentInChildren<LCMtoTexture>()); //We have the monitors directly subscribe to the video feeds

            infoMap.Add(self.id, newInfoBlock); //Add it to the map

            UpdateLayout(); //Time apply the physcial layout of all of our prefabs
        }

       
    }

    /// <summary>
    /// ***UNTESTED*** Should safley remove all data associated with a particular ID number
    /// </summary>
    /// <param name="id">
    /// The ID number of the robot info to be removed
    /// </param>
    public void RemoveFromMap(int id)
    {
        myLCM.Unsubscribe(infoMap[id].prefabObject.GetComponentInChildren<LCMtoTexture>().getSubscriptionChannel(), infoMap[id].prefabObject.GetComponentInChildren<LCMtoTexture>());
        GameObject.Destroy(infoMap[id].prefabObject);
        infoMap.Remove(id);
    }

    /// <summary>
    /// Called when the objects spawns, after Awake() and OnEnable(). We instantiate things and make sure this is the only InfoManager object.
    /// </summary>
    void Start()
    {
        infoQueue = new Queue<info_t>();
        selfQueue = new Queue<robotself_t>();
        if (instance == null)
        {
            instance = this;
            myLCM = LCM.LCMManager.lcmManager.getLCM();
            myLCM.Subscribe(infoChannel, this);
            myLCM.Subscribe(robotselfChannel, this);
        } else
        {
            throw new Exception("Only one InfoManager can exist at a time");
        }
    }

    /// <summary>
    /// Called every frame, we are really just dequeueing messages and checking if we want to spawn a test object
    /// </summary>
    void Update()
    {

        while (infoQueue.Count > 0)
        {
            UpdateMapInfo(infoQueue.Dequeue());
            
        }
        while (selfQueue.Count > 0)
        {
            UpdateMapSelf(selfQueue.Dequeue());

        }

        if (Test)
        {
            Test = false;
            info_t fakeInfo         = new info_t();
            robotself_t fakeSelf    = new robotself_t();
            int id  = (int)(UnityEngine.Random.value * 1000);

            fakeSelf.id = id;
            UpdateMapSelf(fakeSelf);

            fakeInfo.id = id;
            UpdateMapInfo(fakeInfo);
        }
    }
}
