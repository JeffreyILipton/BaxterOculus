using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Loads from the NDI dlls
/// </summary>
public class Loader : MonoBehaviour {

    [DllImport("Loader")]
    public static extern bool SetNDIFrameBuffer(int nChannel, System.IntPtr target, int nWidth, int nHeight);

    [DllImport("Loader")]
    public static extern bool ClearNDIFrameBuffer(int nChannel);

    [DllImport("Loader")]
    public static extern bool StartThread();

    [DllImport("Loader")]
    public static extern void EndThread();

    [DllImport("Loader")]
    public static extern bool EnumNDIPorts(int nPort, StringBuilder sPortName, int nBuffSize);

    [DllImport("Loader")]
    public static extern bool OpenNDIPort(int nChannel, int nPort);

    [DllImport("Loader")]
    public static extern bool GetNDIPortName(int nChannel, StringBuilder sPortName, int nBuffSize);

    [DllImport("Loader")]
    public static extern int GetNDIPortWidth(int nChannel);

    [DllImport("Loader")]
    public static extern int GetNDIPortHeight(int nChannel);

    [DllImport("Loader")]
    public static extern bool InitNDIPort(int nChannel);

	[DllImport ("Loader")]
	public static extern bool ReadNDIFrame(int nChannel, 
        System.IntPtr target,
        int width,
        int height);

}
