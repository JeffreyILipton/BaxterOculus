/*	Loader.cpp

	Unity library for receiving video streams via NDI and writing each video stream to a texture.

	(c) Todor Fay

*/

#include "loader.h"

// One global NDIManager object handles all behavior.
static CNDIManager g_NDIManager;

/*
	CNDIChannel class methods.
*/

CNDIChannel::CNDIChannel()

{
	m_bInitialized = false;
	m_pNDI_recv = NULL;
	m_szName[0] = 0;
	m_nInHeight = 0;
	m_nInWidth = 0;
	m_pvTarget = NULL;
	m_nTargetWidth = 0;
	m_nTargetHeight = 0;
	m_nSeed = 0;
}

CNDIChannel::~CNDIChannel()
{
	// Destroy the receiver
	if (m_pNDI_recv)
	{
		NDIlib_recv_destroy(m_pNDI_recv);
	}
}

/*	CNDIChannel::OpenPort
	Open the specified NDI port.
	The name and ip address were previously retrieved with the EnumNDIPorts method.
*/

bool CNDIChannel::OpenPort(char *pszPortName,char *pszIPAddress)

{
	// If this port was previously opened, close it.
	if (m_bInitialized)
	{
		if (m_pNDI_recv)
		{
			NDIlib_recv_destroy(m_pNDI_recv);
		}
		m_bInitialized = false;
	}

	// Copy the port description and ip address into an NDI structure.
	NDIlib_source_t ndisSource;
	ndisSource.p_ip_address = pszIPAddress;
	ndisSource.p_ndi_name = pszPortName;
	
	NDIlib_recv_create_t NDI_recv_create_desc = { ndisSource, FALSE, /* Highest quality */NDIlib_recv_bandwidth_highest, /* Allow fielded video */FALSE };

	// Open the port for receiving video.
	m_pNDI_recv = NDIlib_recv_create2(&NDI_recv_create_desc);
	if (m_pNDI_recv)
	{
		// If successful, send some meta data. 
		{

			// We tell any up-stream sources that we have the following preferences in terms of video formats. Please note that this is not required
			// however it allows the up-stream source to potentially choose video formats that you prefer. They are listed in terms of preference. The first
			// audio and video format are almost always what we really want to receive. It is not a requirement that a sender would actually share one of
			// these formats with you, they might ignore this.
			const char* p_connection_format = "<ndi_format>\n"
				"   <video_format xres=\"1920\" yres=\"1080\" frame_rate_n=\"30000\" frame_rate_d=\"1001\" aspect_ratio=\"1.77778\" progressive=\"true\"/>\n"
				"   <audio_format no_channels=\"2\" sample_rate=\"48000\"/>\n"
				"</ndi_format>";

			// Provide a meta-data registration that allows people to know what we are. Note that this is optional.
			static const char* p_connection_product = "<ndi_product long_name=\"NDILib Receive Example.\" "
				"             short_name=\"NDILib Receive\" "
				"             manufacturer=\"CoolCo, inc.\" "
				"             version=\"1.000.000\" "
				"             session=\"default\" "
				"             model_name=\"S1\" "
				"             serial=\"ABCDEFG\"/>";

			const NDIlib_metadata_frame_t NDI_format = {
				// The length
				(DWORD)::strlen(p_connection_format),
				// Timecode (synthesized for us !)
				NDIlib_send_timecode_synthesize,
				// The string
				(CHAR*)p_connection_format
			};
			NDIlib_recv_add_connection_metadata(m_pNDI_recv, &NDI_format);

			const NDIlib_metadata_frame_t NDI_product = {
				// The length
				(DWORD)::strlen(p_connection_product),
				// Timecode (synthesized for us !)
				NDIlib_send_timecode_synthesize,
				// The string
				(CHAR*)p_connection_product
			};
			NDIlib_recv_add_connection_metadata(m_pNDI_recv, &NDI_product);
		}

		// We are now going to mark this source as being on program output for tally purposes (but not on preview)
		const NDIlib_tally_t tally_state = { TRUE, FALSE };
		NDIlib_recv_set_tally(m_pNDI_recv, &tally_state);
		// Indicate that this channel is now initialized.
		m_bInitialized = true;
		// Keep a friendly name for the port, for display purposes later.
		sprintf_s(m_szName,"%s - %s",pszPortName, pszIPAddress);
	}
	return m_bInitialized;
}

/*	CNDIChannel::SetNDIFrameBuffer
	Set the buffer pointer, width and height, for the texture frame buffer.
*/

void CNDIChannel::SetNDIFrameBuffer(void* pvTarget, int nWidth, int nHeight)

{
	m_pvTarget = pvTarget;
	m_nTargetWidth = nWidth;
	m_nTargetHeight = nHeight;
}

/*	CNDIChannel::ReadFrame
	Read a frame from the NDI channel and copy it into the texture buffer. 
*/

bool CNDIChannel::ReadFrame()

{
	bool bResult = false;
	if (m_pNDI_recv)
	{
		// Set to receive video, audio, and meta data packets
		// (Though right now, all we are using are the video packets.)
		NDIlib_video_frame_t video_frame;
		NDIlib_audio_frame_t audio_frame;
		NDIlib_metadata_frame_t metadata_frame;

		// Wait up to 5ms for a new packet. Then, depending on what it is...
		switch (NDIlib_recv_capture(m_pNDI_recv, &video_frame, &audio_frame, &metadata_frame, 5))
		{
			// No data
		case NDIlib_frame_type_none:
			break;

			// Video data
		case NDIlib_frame_type_video:
			// This is what we are waiting for.
			{
				// Store the NDI frame size, in case it has changed.
				m_nInWidth = video_frame.xres;
				m_nInHeight = video_frame.yres;
				// Get a pointer to the NDI video data.
				BYTE* scanLine = video_frame.p_data;
				// Just copy the area that is covered by both incoming video frame and the texture.
				int nScanWidth = min(m_nTargetWidth, (int)video_frame.xres);
				int nScanHeight = min(m_nTargetHeight, (int)video_frame.yres);
				for (int nRow = 0; nRow < nScanHeight; nRow++)
				{
					// Calculate the row to write to by using the width of the texture and the 4 byte size of a pixel.
					BYTE* dest = (BYTE*)m_pvTarget + (nRow * m_nTargetWidth * 4);
					// Then iterate through all the pixesl on the row, copying them over.
					for (int nCol = nScanWidth - 1; nCol >= 0; nCol--)
					{
						int nIndex = nCol * 4;
						*dest++ = scanLine[nIndex + 2]; // red
						*dest++ = scanLine[nIndex + 1]; // green
						*dest++ = scanLine[nIndex + 0]; // blue
						*dest++ = scanLine[nIndex + 3]; // alpha
					}
					// Go to the next scan line.
					scanLine += video_frame.line_stride_in_bytes;
				}
				// When done, free the video packet.
				NDIlib_recv_free_video(m_pNDI_recv, &video_frame);
				bResult = true;
			}
			break;

			// Audio data
		case NDIlib_frame_type_audio:
			// printf("Audio data received (%d samples).\n", audio_frame.no_samples);
			// This is where we process incoming audio data. Free the frame for now.
			NDIlib_recv_free_audio(m_pNDI_recv, &audio_frame);
			break;

			// Meta data
		case NDIlib_frame_type_metadata:
			// printf("Meta data received.\n");
			// This is where we process incoming meta data. This is a convenient way to communicate back and forth
			// between sender and receiver. 
			NDIlib_recv_free_metadata(m_pNDI_recv, &metadata_frame);
			break;
		}
	}
	else
	{
		// Just for testing, we are writing a pattern in the frame to 
		// make sure that writing to the texture in real time really does work (and it does!)
		// This also gives a visual reference when it isn't connecting.
		for (int row = 0; row < m_nTargetHeight; row++)
		{
			BYTE* dest = (BYTE*)m_pvTarget + (row * m_nTargetWidth * 4);
			for (int col = 0; col < m_nTargetWidth; col++)
			{
				*dest++ = (m_nSeed + col) & 0xFF; // red
				*dest++ = (m_nSeed + row) & 0xFF; // green
				*dest++ = (col + row) & 0xFF; // blue
				*dest++ = 0xFF; // alpha
			}
		}
		bResult = true;
		m_nSeed++;	
	}
	return bResult;
}

/* CNDIManager
*/

CNDIManager::CNDIManager()
{
	m_bInitialized = false;
	for (int nX = 0; nX < NDI_PORTS;nX++)
	{
		// Clear the port name and ip address arrays.
		m_aszPortNames[nX][0] = 0;
		m_aszIPAddresses[nX][0] = 0;
	}
	m_nPortCount = 0;
	m_dwThreadId = 0;
	m_hThread = 0;
	m_bAlive = false;
	InitializeCriticalSection(&m_csCriticalSection);
}

CNDIManager::~CNDIManager()
{
	EndThread();
	NDIlib_destroy();
	DeleteCriticalSection(&m_csCriticalSection);
}

/*	CNDIManager::RunThread
	Thread function. Runs until m_bAlive is cleared, indicating it's time to kick the bucket.
	This thread repeatedly calls each NDI channel and reads data from it, transfering to the textures.
*/

void CNDIManager::RunThread()

{
	m_bAlive = true;
	while (m_bAlive)
	{
		// Iterate through the channels and read frames.
		for (int nChannel = 0; nChannel < NDI_PORTS; nChannel++)
		{
			CCSLock csLock(&m_csCriticalSection);	// Lock ensure that only one thread can touch this at a time.
			m_aChannels[nChannel].ReadFrame();
		}
		// Wait 5ms before doing again.. just to be safe.
		Sleep(5);
	}
	// Time to die. Clear the thread variables.
	m_hThread = 0;
	m_dwThreadId = 0;
	// Return to MyThreadFunction().
}

/*	MyThreadFunction() 
	The Windows entry point for the thread.
	Since this is not object oriented, it carries the object in the generic LPVOID parameter.
	It castes that to the CNDIManager and then calls the RunThread() method.
	When done, it returns 0, ending the thread.
*/

DWORD WINAPI MyThreadFunction(LPVOID lpParam)

{
	CNDIManager *pManager = (CNDIManager *)lpParam;
	pManager->RunThread();
	return 0;
}

/*	CNDIManager::StartThread()
	Start the thread running.
	This simply uses the Windows call.
*/

bool CNDIManager::StartThread()

{
	EndThread(); // Just in case...

	m_hThread = CreateThread(
		NULL,                   // default security attributes
		0,                      // use default stack size  
		MyThreadFunction,       // thread function name
		this,					// argument to thread function 
		0,                      // use default creation flags 
		&m_dwThreadId);			// returns the thread identifier 
	return (m_hThread != 0);
}

/*	CNDIManager::EndThread()
	End the thread by clearing m_bAlive, and then waiting for the thread to indicate it is done.
*/

void CNDIManager::EndThread()

{
	m_bAlive = false;
	while (m_hThread)
	{
		Sleep(10);
	}
}

bool CNDIManager::SetNDIFrameBuffer(int nChannel, void* pvTarget, int nWidth, int nHeight)

{
	bool bResult = false;
	if (nChannel < NDI_PORTS)
	{
		CCSLock csLock(&m_csCriticalSection);
		m_aChannels[nChannel].SetNDIFrameBuffer(pvTarget, nWidth, nHeight);
		bResult = true;
	}
	return bResult;
}

bool CNDIManager::ClearNDIFrameBuffer(int nChannel)

{
	bool bResult = false;
	if (nChannel < NDI_PORTS)
	{
		CCSLock csLock(&m_csCriticalSection);
		m_aChannels[nChannel].ClearNDIFrameBuffer();
		bResult = true;
	}
	return bResult;
}

/*	CNDIManager::ReadNDIFrame
	Left over from the original implementation, this directly reads an NDI frame into the buffer.
*/

bool CNDIManager::ReadNDIFrame(int nChannel, void* pvTarget, int nWidth, int nHeight)

{
	bool bResult = false;
	if (nChannel < NDI_PORTS)
	{
		CCSLock csLock(&m_csCriticalSection);
		m_aChannels[nChannel].SetNDIFrameBuffer(pvTarget, nWidth, nHeight);
		bResult = m_aChannels[nChannel].ReadFrame();
		m_aChannels[nChannel].ClearNDIFrameBuffer();
	}
	return bResult;
}


bool CNDIManager::GetNDIPortName(int nChannel, char *pszName, int nBuffSize)
{
	if (nChannel < NDI_PORTS)
	{
		CCSLock csLock(&m_csCriticalSection);
		m_aChannels[nChannel].GetName(pszName, nBuffSize);
		return true;
	}
	return false;
}

int CNDIManager::GetNDIPortWidth(int nChannel)
{
	if (nChannel < NDI_PORTS)
	{
		CCSLock csLock(&m_csCriticalSection);
		return m_aChannels[nChannel].GetWidth();
	}
	return 0;
}

int CNDIManager::GetNDIPortHeight(int nChannel)
{
	if (nChannel < NDI_PORTS)
	{
		CCSLock csLock(&m_csCriticalSection);
		return m_aChannels[nChannel].GetHeight();
	}
	return 0;
}

/*	CNDIManager::EnumNDIPorts
	This serves two purposes:
	It returns the friendly name for a specific port.
	It actually creates the list of ports. This occurs with the very first request, starting at port 0.
	In order to use NDI, you must first find all the ports and retrieve their name and IP address. 
	This stores all the information in the NDIManager, so later requests to open ports need only 
	provide the Port index required.
	The port name returned by this call is a friendly name for the UI.
*/

bool CNDIManager::EnumNDIPorts(int nPort, char *pszPortName, int nBuffSize)
{
	// First, make sure that NDI is initialized.
	if (!m_bInitialized)
	{
		m_bInitialized = NDIlib_initialize();
	}
	if (m_bInitialized)
	{
		// If this is the first enumeration, go ahead and create a fresh list of ports.
		if (nPort == 0)
		{
			// Starting, so create the list.

			const NDIlib_find_create_t NDI_find_create_desc = { TRUE, NULL };

			// Create a finder
			NDIlib_find_instance_t pNDI_find = NDIlib_find_create(&NDI_find_create_desc);
			if (pNDI_find)
			{

				DWORD dwSources = 0;
				const NDIlib_source_t* p_sources = NULL;

				for (int nTries = 3; nTries > 0; nTries--)
				{
					// For some reason, we should do this several times... 
					// 3 seems to work, but this causes a 30 second delay, which should not be necessary.
					// This needs to be optimized.
					p_sources = NDIlib_find_get_sources(pNDI_find, &dwSources, 10000);
				}
				// Did we get any ports?
				if (p_sources)
				{
					// Okay, now copy the names of these into the arrays of names and ip addresses.
					for (int nX = 0; nX < (int) dwSources; nX++)
					{
						strncpy_s(m_aszPortNames[nX], p_sources[nX].p_ndi_name, MAX_PORT_NAME_LENGTH);
						strncpy_s(m_aszIPAddresses[nX], p_sources[nX].p_ip_address, MAX_PORT_NAME_LENGTH);
					}
				}
				// How many ports?
				m_nPortCount = (int) dwSources;

				// Destroy the NDI finder. 
				NDIlib_find_destroy(pNDI_find);
			}
		}
		// Okay, go ahead and get the requested port name.
		if (nPort < m_nPortCount)
		{
			strncpy(pszPortName, m_aszPortNames[nPort], nBuffSize);
			return true;
		}
	}
	return false;
}

bool CNDIManager::OpenNDIPort(int nChannel, int nPort)

{
	if ((nChannel < NDI_PORTS) && (nPort < m_nPortCount))
	{
		CCSLock csLock(&m_csCriticalSection);
		return m_aChannels[nChannel].OpenPort(m_aszPortNames[nPort],m_aszIPAddresses[nPort]);
	}
	return false;
}

/*
	DLL entry points. These are the functions exposed via loader.cs.
*/

extern "C" bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API ReadNDIFrame(int nChannel, void* target, int nWidth, int nHeight)
{
	return g_NDIManager.ReadNDIFrame(nChannel, target, nWidth, nHeight);
}

extern "C"	bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API EnumNDIPorts(int nPort, char *pszPortName, int nBuffSize)
{
	return g_NDIManager.EnumNDIPorts(nPort, pszPortName, nBuffSize);
}

extern "C"	bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API OpenNDIPort(int nChannel, int nPort)
{
	return g_NDIManager.OpenNDIPort(nChannel, nPort);
}

extern "C"	bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetNDIPortName(int nChannel, char *pszName, int nBuffSize)
{
	return g_NDIManager.GetNDIPortName(nChannel, pszName, nBuffSize);
}

extern "C"	int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetNDIPortWidth(int nChannel)
{
	return g_NDIManager.GetNDIPortWidth(nChannel);
}

extern "C"	int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetNDIPortHeight(int nChannel)
{
	return g_NDIManager.GetNDIPortHeight(nChannel);
}

extern "C"	bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetNDIFrameBuffer(int nChannel, void* pvTarget, int nWidth, int nHeight)
{
	return g_NDIManager.SetNDIFrameBuffer(nChannel, pvTarget, nWidth, nHeight);
}

extern "C"	bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API ClearNDIFrameBuffer(int nChannel)
{
	return g_NDIManager.ClearNDIFrameBuffer(nChannel);
}

extern "C"	bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API StartThread()
{
	return g_NDIManager.StartThread();
}

extern "C"	void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API EndThread()
{
	g_NDIManager.EndThread();
}

