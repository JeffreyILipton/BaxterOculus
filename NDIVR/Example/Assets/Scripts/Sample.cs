using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class Sample : MonoBehaviour {

	//  
    public int nChannel = 0;             // NDI Channel
	public int nTextureWidth = 1920;     // Frame Width.
	public int nTextureHeight = 1080;    // Frame Height.

	private Texture2D t2dTexture = null;    // Texture to draw on.

    GCHandle handleToPinnedArray;              // Texture handle.
    IntPtr integerPointer;                   // Pointer to texture data
    byte[] pointerToByteData = null;

    void CreateTexture () {
		if (t2dTexture != null)
        {
            handleToPinnedArray.Free();
        }

        t2dTexture = new Texture2D (nTextureWidth, nTextureHeight, TextureFormat.RGBA32, false);
        t2dTexture.filterMode = FilterMode.Trilinear;
        t2dTexture.Apply ();

		GetComponent<Renderer> ().material.mainTexture = t2dTexture;

        // Raw pixel buffer of the bitmap
        pointerToByteData = new byte[nTextureWidth * nTextureHeight * 4];

        // Create a handle to the byte array
        handleToPinnedArray = GCHandle.Alloc(pointerToByteData, GCHandleType.Pinned);

        // Get a pointer for the DLL from the handle
        integerPointer = handleToPinnedArray.AddrOfPinnedObject();

        Loader.SetNDIFrameBuffer(nChannel, integerPointer, nTextureWidth, nTextureHeight);
    }

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
        int width = Loader.GetNDIPortWidth(nChannel);
        int height = Loader.GetNDIPortHeight(nChannel);
        if ((width != nTextureWidth) && (width > 0) && (height != nTextureHeight) && (height > 0))
        {
            nTextureWidth = width;
            nTextureHeight = height;
            CreateTexture();
        }
        FillTexture();
    }
}
