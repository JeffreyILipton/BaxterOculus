using Boo.Lang.Runtime.DynamicDispatching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class NDIFindMaster : MonoBehaviour {

    #region PrivateMembers

    // a pointer to our unmanaged NDI finder instance
    IntPtr _findInstancePtr = IntPtr.Zero;

    // a pointer to our unmanaged NDI receiver instance
    //Dictionary<String, IntPtr> _recvInstancePtr = new Dictionary<string, IntPtr>();

    IntPtr _recvInstancePtr = IntPtr.Zero;

    // a thread to receive frames on so that the UI is still functional
    Thread _receiveThread = null;

    // a way to exit the thread safely
    bool _exitThread = false;

    // a map of names to sources
    #endregion PrivateMembers

    public static NDIFindMaster instance;

    public Dictionary<String, NDI.NDIlib_source_t> _sources = new Dictionary<string, NDI.NDIlib_source_t>();

    public ArrayList SourceNames;

    public bool manualFind;

    private long timestampFind;

    // Use this for initialization
    void Start () {
        InitFind();

        timestampFind = DateTime.Now.Ticks;

        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
            throw new Exception("There should only be one NDIFindMaster!");   
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DateTime.Now.Ticks - timestampFind > 100000000)
        {
            UpdateFindList();
            timestampFind = DateTime.Now.Ticks;
        }
        if (manualFind)
        {
            UpdateFindList();
            manualFind = false;
        }
    }

    #region NdiFind

    void InitFind()
    {
        // This will be IntPtr.Zero 99.999% of the time.
        // Could be one "MyGroup" or multiples "public,My Group,broadcast 42" etc.
        // Create a UTF-8 buffer from our string
        // Must use Marshal.FreeHGlobal() after use!
        // IntPtr groupsPtr = NDI.Common.StringToUtf8("public");
        IntPtr groupsPtr = IntPtr.Zero;

        // how we want our find to operate
        NDI.NDIlib_find_create_t findDesc = new NDI.NDIlib_find_create_t()
        {
            // Needs an IntPtr to a UTF-8 string
            p_groups = groupsPtr,

            // also the ones on this computer - useful for debugging
            show_local_sources = true
        };

        // create our find instance
        _findInstancePtr = NDI.Find.NDIlib_find_create(ref findDesc);

        // free our UTF-8 buffer if we created one
        if (groupsPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(groupsPtr);
        }

        // did it succeed?
        System.Diagnostics.Debug.Assert(_findInstancePtr != IntPtr.Zero, "Failed to create NDI find instance.");

        // update the list
        UpdateFindList();
    }

    void UpdateFindList()
    {
        //Debug.Log("findlist");
        int NumSources = 0;
        SourceNames = new ArrayList();
        // ask for an update
        // timeout == 0, always return the full list
        // timeout > 0, wait timeout ms, then return 0 for no change or the total number of sources found
        IntPtr SourcesPtr = NDI.Find.NDIlib_find_get_sources(_findInstancePtr, ref NumSources, 0);

        //Debug.Log(NumSources);

        //SourceNames.Add("dummy0");

        // if sources == 0, then there was no change, keep your list
        if (NumSources > 0)
        {
            // clear our list and dictionary
            SourceNames.Clear();
            _sources.Clear();

            //SourceNames.Add("dummy1");
            // the size of an NDIlib_source_t, for pointer offsets
            int SourceSizeInBytes = Marshal.SizeOf(typeof(NDI.NDIlib_source_t));

            // convert each unmanaged ptr into a managed NDIlib_source_t
            for (int i = 0; i < NumSources; i++)
            {
                // source ptr + (index * size of a source)
                IntPtr p = new IntPtr(SourcesPtr.ToInt64() + (i * SourceSizeInBytes));

                // marshal it to a managed source and assign to our list
                NDI.NDIlib_source_t src = (NDI.NDIlib_source_t)Marshal.PtrToStructure(p, typeof(NDI.NDIlib_source_t));

                // .Net doesn't handle marshaling UTF-8 strings properly
                String name = NDI.Common.Utf8ToString(src.p_ndi_name);

                // add it to the list and dictionary
                if (!_sources.ContainsKey(name) && !SourceNames.Contains(name))
                {
                    _sources.Add(name, src);
                    SourceNames.Add(name);
                }
            }
        }
        for (int i = 0; i < SourceNames.Count; i++)
        {
            Debug.Log((string)(SourceNames[i]));
        }
    }
    #endregion NdiFind

    private void OnApplicationQuit()
    {
    }
}
