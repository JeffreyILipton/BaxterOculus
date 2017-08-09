using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Toggle : MonoBehaviour {
    private bool isRunning = false; //Wether or not the images are activly being sent to the display
    public string NDIChannelName;

    // Use this for initialization
    void Start () {
        
        //		cache = FindObjectsOfType<Sample> ();
    }

    // Unity GUI event we will display the statistcs and provide a toggle button
    void OnGUI()
    {
        // Create The Init button
        Rect rect = new Rect(50, 10, 200, 50);
        GUI.Label(rect, "Press / to initialize ports");
        

        // Create the "Start" button
        rect = new Rect(200, 10, 200, 50);
        GUI.Label(rect, "Press , to start");
        if (Input.GetKeyDown(KeyCode.Comma) && !isRunning) // If we press comma and we arent already running, start running
        {
            Loader.StartThread();
            isRunning = true;
        }

        // Create the "End" button 
        rect = new Rect(350, 10, 200, 50); 
        GUI.Label(rect, "Press . to stop");
        if (Input.GetKey(KeyCode.Period) && isRunning)  // If we press perioid and we are already running, stop running
        {
            Loader.EndThread();
            isRunning = false;
        }

        // Create dimensions for the frame
        rect = new Rect(500, 10, 400, 100);

        // Display the frame
        GUI.Box(rect, "Ports");

        // Move the dimensions donwn-right so we don't overwrite the title
        rect.yMin += 20;
        rect.xMin += 20;

        // Msg is a string containing all the data that will be displayed
        string msg = "NDI Ports: \n";// + cache.Length;

        for (int nChannel = 0; nChannel < 3; nChannel++)
        {
            StringBuilder sName = new StringBuilder(100);
            Loader.GetNDIPortName(nChannel, sName, sName.Capacity);//Gets info for each channel

            // Displays the info on each individual stream

            int nWidth = Loader.GetNDIPortWidth(nChannel);
            int nHeight = Loader.GetNDIPortHeight(nChannel);
            msg += nChannel.ToString() + ": " + sName.ToString();
            msg += " Width, Height: ";
            msg += nWidth.ToString() + "," + nHeight.ToString() + "\n";
        }


        // Display collected data
        GUI.Label(rect, msg);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash)) //If slash is hit then reinit ports
        {
            initChannels();
        }

    }
    void initChannels()
    {
        Loader.EndThread();
        isRunning = false;
        for (int nChannel = 0; nChannel < 3; nChannel++)
            //Initializes the ports
            {
            StringBuilder sName = new StringBuilder(100);
            //Debug.Log(sName + "=?" + NDIChannelName);
            Debug.Log(Loader.CloseNDIPort(nChannel));
            if (Loader.EnumNDIPorts(nChannel, sName, sName.Capacity))// && sName.ToString().StartsWith(NDIChannelName))
            {
                Loader.OpenNDIPort(nChannel, nChannel);
                Debug.Log(sName + "=?" + NDIChannelName);
            }
            

            //Loader.
        }
    }

    private void OnApplicationQuit()
    {
        Loader.EndThread();
       // Loader.
    }
}
