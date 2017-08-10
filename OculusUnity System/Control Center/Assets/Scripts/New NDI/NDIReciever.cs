using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The class that recieves bitmap data from an NDIUnit and displays it as a texture
/// </summary>
public class NDIReciever : MonoBehaviour {

    private int yres, xres;
    private byte[] byteBitMap;

    /// <summary>
    /// The name of the NDI channel it recieves from
    /// </summary>
    public string sourceName;
    private Texture2D t2dTexture;

    #region Getters and Setters
    public byte[] ByteBitMap
    {
        get
        {
            return byteBitMap;
        }

        set
        {
            byteBitMap = value;
        }
    }

    public int Yres
    {
        get
        {
            return yres;
        }

        set
        {
            yres = value;
        }
    }

    public int Xres
    {
        get
        {
            return xres;
        }

        set
        {
            xres = value;
        }
    }
    #endregion

    // Use this for initialization
    void Start () {
        initTexture();
	}
	
	// Update is called once per frame
	void Update () {

        if (byteBitMap != null)
        {
            t2dTexture.LoadRawTextureData(byteBitMap);
            t2dTexture.Apply();
            if (t2dTexture.width != xres || t2dTexture.height != yres)
            {
                initTexture();
            }
        }
    }

    /// <summary>
    /// Initializes the texture applied to the object
    /// </summary>
    protected void initTexture()
    {
        t2dTexture = new Texture2D(xres, yres, TextureFormat.BGRA32, false);
        t2dTexture.filterMode = FilterMode.Trilinear;
        t2dTexture.Apply();

        GetComponent<Renderer>().material.EnableKeyword("_EmissionMap"); //Enable the keyword used two lines later to set emission mapping, which makes the object act as a light source
        GetComponent<Renderer>().material.SetTexture("_MainTex", t2dTexture); //Applies the texture to the main, non emissive, texture
        GetComponent<Renderer>().material.SetTexture("_EmissionMap", t2dTexture);//Applies the texture to the emessive texture map

    }
}
