using Boo.Lang.Runtime.DynamicDispatching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class NDIUnit : MonoBehaviour {

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
    Dictionary<String, NDI.NDIlib_source_t> _sources = new Dictionary<string, NDI.NDIlib_source_t>();

    private ArrayList SourceNames;

    private Texture2D t2dTexture = null;    // Texture to draw on.

    public NDIReciever reciever;

    #endregion PrivateMembers

    //private byte[] byteBitMap;

    public bool ConnectChannel = true;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update()
    {
        _sources = NDIFindMaster.instance._sources;
        SourceNames = NDIFindMaster.instance.SourceNames;

        if (ConnectChannel)
        {
            ConnectChannel = false;
            Connect(reciever);
        }
    }

    #region NdiReceive

    // connect to an NDI source in our Dictionary by name
    void Connect(NDIReciever reciever)
    {
        Debug.Log("Connecting to " + reciever.sourceName);

        // just in case we're already connected
        Disconnect();

        // we need valid information to connect
        if (String.IsNullOrEmpty(reciever.sourceName) || !_sources.ContainsKey(reciever.sourceName))
        {
            return;
        }

        // find our new source
        NDI.NDIlib_source_t source = _sources[reciever.sourceName];

        // make a description of the receiver we want
        NDI.NDIlib_recv_create_t recvDescription = new NDI.NDIlib_recv_create_t()
        {
            // the source we selected
            source_to_connect_to = source,

            // we want BGRA frames for this example
            prefer_UYVY = false,

            // we want full quality - for small previews or limited bandwidth, choose lowest
            bandwidth = NDI.NDIlib_recv_bandwidth_e.NDIlib_recv_bandwidth_highest
        };

        // create a new instance connected to this source
        _recvInstancePtr = NDI.Receive.NDIlib_recv_create(ref recvDescription);

        // did it work?
        System.Diagnostics.Debug.Assert(_recvInstancePtr != IntPtr.Zero, "Failed to create NDI receive instance.");

        if (_recvInstancePtr != IntPtr.Zero)
        {
            // We are now going to mark this source as being on program output for tally purposes (but not on preview)
            SetTallyIndicators(true, false);

            // start up a thread to receive on
            _receiveThread = new Thread(new ParameterizedThreadStart(ReceiveThreadProc)) { IsBackground = true, Name = "NdiExampleReceiveThread" };
            _receiveThread.Start(reciever);
            
        }


    }

    void Disconnect()
    {
        // in case we're connected, reset the tally indicators
        SetTallyIndicators(false, false);

        // check for a running thread
        if (_receiveThread != null)
        {
            // tell it to exit
            _exitThread = true;

            // wait for it to exit
            while (_receiveThread.IsAlive)
                Thread.Sleep(100);
        }

        // reset thread defaults
        _receiveThread = null;
        _exitThread = false;

        // Destroy the receiver
        NDI.Receive.NDIlib_recv_destroy(_recvInstancePtr);

        // set it to a safe value
        _recvInstancePtr = IntPtr.Zero;
    }

    void SetTallyIndicators(bool onProgram, bool onPreview)
    {
        // we need to have a receive instance
        if (_recvInstancePtr != IntPtr.Zero)
        {
            // set up a state descriptor
            NDI.NDIlib_tally_t tallyState = new NDI.NDIlib_tally_t()
            {
                on_program = onProgram,
                on_preview = onPreview
            };

            // set it on the receiver instance
            NDI.Receive.NDIlib_recv_set_tally(_recvInstancePtr, ref tallyState);
        }
    }

    // the receive thread runs though this loop until told to exit
    void ReceiveThreadProc(object rcvr)
    {
        NDIReciever reciever = (NDIReciever)rcvr;
        while (!_exitThread && _recvInstancePtr != IntPtr.Zero)
        {
            
            // The descriptors
            NDI.NDIlib_video_frame_t    videoFrame      = new NDI.NDIlib_video_frame_t();
            NDI.NDIlib_audio_frame_t    audioFrame      = new NDI.NDIlib_audio_frame_t();
            NDI.NDIlib_metadata_frame_t metadataFrame   = new NDI.NDIlib_metadata_frame_t();

            switch (NDI.Receive.NDIlib_recv_capture(_recvInstancePtr, ref videoFrame, ref audioFrame, ref metadataFrame, 1000))
            {
                // No data
                case NDI.NDIlib_frame_type_e.NDIlib_frame_type_none:
                    // No data received
                    break;

                // Video data
                case NDI.NDIlib_frame_type_e.NDIlib_frame_type_video:
                    // this can occasionally happen when changing sources
                    if (videoFrame.p_data != IntPtr.Zero)
                    {
                        // get all our info so that we can free the frame
                        int yres = (int)videoFrame.yres;
                        int xres = (int)videoFrame.xres;

                        reciever.Xres = xres;
                        reciever.Yres = yres;


                        // quick and dirty aspect ratio correction for non-square pixels - SD 4:3, 16:9, etc.
                        double dpiX = 96.0 * (videoFrame.picture_aspect_ratio / ((double)xres / (double)yres));

                        int stride = (int)videoFrame.line_stride_in_bytes;
                        int bufferSize = yres * stride;

                        // copy the bitmap out
                        Byte[] buffer = new Byte[bufferSize];
                        Marshal.Copy(videoFrame.p_data, buffer, 0, bufferSize);



                        reciever.ByteBitMap = buffer;

                    }
                    else
                    {
                        reciever.ByteBitMap = new byte[reciever.Yres * (int)videoFrame.line_stride_in_bytes];
                        Marshal.Copy(videoFrame.p_data, reciever.ByteBitMap, 0, reciever.Yres * (int)videoFrame.line_stride_in_bytes);
                    }
                    // free frames that were received
                    NDI.Receive.NDIlib_recv_free_video(_recvInstancePtr, ref videoFrame);
                    break;

                // audio is beyond the scope of this example
                case NDI.NDIlib_frame_type_e.NDIlib_frame_type_audio:

                    // free frames that were received
                    NDI.Receive.NDIlib_recv_free_audio(_recvInstancePtr, ref audioFrame);
                    break;

                // Metadata
                case NDI.NDIlib_frame_type_e.NDIlib_frame_type_metadata:

                    // UTF-8 strings must be converted for use - length includes the terminating zero
                    //String metadata = Utf8ToString(metadataFrame.p_data, metadataFrame.length-1);

                    //System.Diagnostics.Debug.Print(metadata);

                    // free frames that were received
                    NDI.Receive.NDIlib_recv_free_metadata(_recvInstancePtr, ref metadataFrame);
                    break;
            }
            //Debug.Log(!_exitThread + "  " + (_recvInstancePtr != IntPtr.Zero));
        }
    }
    #endregion NdiReceive

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
