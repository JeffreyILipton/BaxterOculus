using UnityEngine;
using System.Collections;
using LCM.LCM;
using oculuslcm;
using System;



public class LCMtoTexture : ChannelSubscriber
{

    private image_t imageRAW; // the message recieved from LCM containing the raw texture data
    private LCM.LCM.LCM myLCM; //the LCM object
    private bool runTexture; // true when there is a fresh image recieved
    private Texture2D texture; // the texture object to be applied to the monitors 



    /// <summary>
    /// Updates image raw and sets runTexture to true
    /// </summary>
    /// <param name="lcm"></param>
    /// <param name="channel"></param>
    /// <param name="ins"></param>
    public override void HandleMessage(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        base.MessageReceived(lcm, channel, ins);
        imageRAW = new image_t(ins);
        runTexture = true;
    }

    void Start()
    {
        //myLCM = LCM.LCMManager.lcmManager.getLCM(); //Sets the LCM object to the global LCM object
        //myLCM.Subscribe(channel, this); // Subscribe to to "hand"_lcm_camera
        imageRAW = new oculuslcm.image_t(); // Initializes imageRaw

        GetComponent<Renderer>().material.EnableKeyword("_EmissionMap"); // The _EmmisionMap keyword is enabled so that the texture can be applied to Emission

        /// Initializes the Texture2DObject, putting it in BGRA mode and applying trilinear filtering
        texture = new Texture2D(imageRAW.width, imageRAW.height, TextureFormat.BGRA32, false);
        texture.filterMode = FilterMode.Trilinear;
        texture.Apply();

    }

    // Update is called once per frame
    void Update()
    {
        if (runTexture) // execute only if there is a fresh image
        {
            // If it is detected that the Texture2dObject was not able to initialize proporley than it will reintialize it.
            if (texture == null || texture.width != imageRAW.width || texture.height != imageRAW.height)
            {
                texture = new Texture2D(imageRAW.width, imageRAW.height, TextureFormat.BGRA32, false);
                texture.filterMode = FilterMode.Trilinear;
                texture.Apply();
               
            }
            texture.LoadRawTextureData(imageRAW.data);

            //The texture is applied to the _MainTex and _EmissionMap textures within the the object's material
            texture.Apply();
            GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            GetComponent<Renderer>().material.SetTexture("_EmissionMap", texture);

            //false so that it wont run agian untill a new image is recieved
            runTexture = false;
        }
    }
}

