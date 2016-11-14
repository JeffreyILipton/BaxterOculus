using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class NDItoTexture : MonoBehaviour {

	//  
    public int nChannel = 0;             // NDI Channel
	public int nTextureWidth = 1920;     // Frame Width.
	public int nTextureHeight = 1080;    // Frame Height.

	private Texture2D t2dTexture = null;    // Texture to draw on.

    GCHandle handleToPinnedArray;              // Texture handle.
    IntPtr integerPointer;                   // Pointer to texture data
    byte[] pointerToByteData = null;

    /// <summary>
    /// Creates the texture which will be applied to the object
    /// </summary>
    void CreateTexture () {
		if (t2dTexture != null)
        {
            handleToPinnedArray.Free();
        }

        t2dTexture = new Texture2D (nTextureWidth, nTextureHeight, TextureFormat.RGBA32, false);
        t2dTexture.filterMode = FilterMode.Trilinear;
        t2dTexture.Apply ();

        GetComponent<Renderer>().material.EnableKeyword("_EmissionMap"); //Enable the keyword used two lines later to set emission mapping, which makes the object act as a light source
        GetComponent<Renderer>().material.SetTexture("_MainTex", t2dTexture); //Applies the texture to the main, non emissive, texture
        GetComponent<Renderer>().material.SetTexture("_EmissionMap", t2dTexture);//Applies the texture to the emessive texture map

        // Raw pixel buffer of the bitmap
        pointerToByteData = new byte[nTextureWidth * nTextureHeight * 4]; // Creates the arrat of pixels at the correct size, nTextureWidth * nTextureHeight is the number of pixels, 4 is for the four color channells per pixel

        // Create a handle to the byte array
        handleToPinnedArray = GCHandle.Alloc(pointerToByteData, GCHandleType.Pinned);

        // Get a pointer for the DLL from the handle
        integerPointer = handleToPinnedArray.AddrOfPinnedObject();

        Loader.SetNDIFrameBuffer(nChannel, integerPointer, nTextureWidth, nTextureHeight); //Calls to send the camera data to the pointer
    }

	/// <summary>
    /// Applies the texture
    /// </summary>
    void FillTexture()
	{

		//bool bSuccess = Loader.ReadNDIFrame(nChannel, integerPointer, nTextureWidth, nTextureHeight);

        // We have the data in the unity array, all we have to do is tell
        // unity to update the texture (this part was previously in the
        // DLL). But as you see, an ENGINE can do this by itself.
        //if (bSuccess)
        {
            t2dTexture.LoadRawTextureData(pointerToByteData);
            t2dTexture.Apply();
            
        }
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		CreateTexture ();
        FillTexture();        
    }

	/// <summary>
	/// Update each frame.
	/// </summary>
	void Update ()
    {
        int width = Loader.GetNDIPortWidth(nChannel); //Sets width to the width of the NDI stream being recieved
        int height = Loader.GetNDIPortHeight(nChannel);//Sets height to the width of the NDI stream being recieved

        //Make things match if they don't
        if ((width != nTextureWidth) && (width > 0) && (height != nTextureHeight) && (height > 0)) 
        {
            nTextureWidth = width;
            nTextureHeight = height;
            CreateTexture();
        }
        FillTexture();
    }
}
