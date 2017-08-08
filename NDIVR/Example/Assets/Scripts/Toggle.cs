using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Toggle : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
//		cache = FindObjectsOfType<Sample> ();
	}

    // Unity GUI event we will display the statistcs and provide a toggle button
    void OnGUI()
    {

        // Create dimensions for the frame
        Rect rect = new Rect(500, 10, 400, 100);

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
            Loader.GetNDIPortName(nChannel, sName, sName.Capacity);
            int nWidth = Loader.GetNDIPortWidth(nChannel);
            int nHeight = Loader.GetNDIPortHeight(nChannel);
            msg += nChannel.ToString() + ": " + sName.ToString();
            msg += " Width, Height: ";
            msg += nWidth.ToString() + "," + nHeight.ToString() + "\n";
        }

        // Display collected data
        GUI.Label(rect, msg);

        // Create The Init button
        rect = new Rect(50, 10, 100, 25);
        if (GUI.Button(rect, "Init Ports") == true)
        {
            Loader.EndThread();
            for (int nChannel = 0; nChannel < 3; nChannel++)
            {
                StringBuilder sName = new StringBuilder(100);
                if (Loader.EnumNDIPorts(nChannel, sName, sName.Capacity))
                {
                    Loader.OpenNDIPort(nChannel, nChannel);
                }
            }
        }

        // Create the "Start" button
        rect = new Rect(200, 10, 100, 25);
        if (GUI.Button(rect, "Start") == true)
        {
            Loader.StartThread();
        }

        // Create the "End" button
        rect = new Rect(350, 10, 100, 25);
        if (GUI.Button(rect, "End") == true)
        {
            Loader.EndThread();
        }
    }
}
