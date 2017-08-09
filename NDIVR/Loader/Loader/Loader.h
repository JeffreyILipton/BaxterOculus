/*	Loader.h

	Unity library for receiving video streams via NDI and writing each video stream to a texture.
	
	(c) 2016 Aidan Fay, Todor Fay

*/

#pragma once	// Only load this header once.

#define _CRT_SECURE_NO_WARNINGS

// Dynamic link library export declaration.
#define UNITY_INTERFACE_EXPORT __declspec(dllexport)

// Standard WINAPI convention (dll compatibility reasons) 
#define UNITY_INTERFACE_API __stdcall


// Keep Windows.h from introducing a bunch of unneeded junk.
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <stdio.h> 

// Unity interop communication interfaces
#include "IUnityInterface.h"
#include "IUnityGraphics.h"
#include "Processing.NDI.Lib.h"

// To keep things simple, we'll used a fixed character array for the NDI port name.
#define MAX_PORT_NAME_LENGTH	100
// Support up to 16 NDI ports. This is overkill, so a slight optimization would be to
// just put in what you need - probably 2!
#define NDI_PORTS	16

/* CNDIChannel

   The CNDIChannel class manages one channel of NDI communication.
   This is owned by CNDIManager, which manages all of the NDI channels. 
*/


class CNDIChannel
{
public:
	CNDIChannel();
	~CNDIChannel();
	// Retrieve a friendly name description of the NDI channel.
	void GetName(char *pszName, int nBuffSize) { strncpy(pszName, m_szName, nBuffSize); }
	// Return the Height of the NDI frame coming in.
	int GetHeight() { return m_nInHeight; }
	// Return the Width of the NDI frame coming in.
	int GetWidth() { return m_nInWidth; }
	// Open the NDI port, given the provided NDI channel name and IP address.
	bool OpenPort(char *pszPortName, char *pszIPAddress);
	// Close the NDI port
	void ClosePort();
	// Set the Unity texture buffer to write to. This is presented as a buffer in memory, with its own width and height.
	void SetNDIFrameBuffer(void* pvTarget, int nWidth, int nHeight);
	// Clear the texture. Channel no longer can write to it.
	void ClearNDIFrameBuffer() { SetNDIFrameBuffer(NULL,0,0); }
	// ReadFrame() is called by the manager thread to see if there is a new frame waiting on NDI,
	// and if there is, write it to the buffer. This uses the parameters set up with 
	// SetNDIFrameBuffer().
	bool ReadFrame();
private:
	NDIlib_recv_instance_t	m_pNDI_recv;		// NDI API receive instance.
	bool	m_bInitialized;						// Is NDI initialized on this channel?
	int		m_nChannel;							// Index of NDI channel.
	int		m_nInWidth;							// Width of incoming NDI frame.
	int		m_nInHeight;						// Height of incoming NDI frame.	
	void *	m_pvTarget;							// Pointer to Unity provided texture buffer.
	int		m_nTargetWidth;						// Width of texture buffer to write to.
	int		m_nTargetHeight;					// Height of texture buffer to write to.
	char	m_szName[MAX_PORT_NAME_LENGTH];		// Friendly name of the port, for display purposes.
	int		m_nSeed;							// Number used for generating test pattern.
};


/*	CCSLock
	The CCSLock class is a quick way to make it simpler and safer to use critical sections.
	One of these can be placed in a code block and it will protect the code block from
	context switching to another thread also waiting to enter. 
	The constructor enters the critical section and stores it.
	The destructor releases the critical section, always when leaving the block. 
*/

class CCSLock
{
public:
	CCSLock(CRITICAL_SECTION * pCS)
	{
		m_pcsCriticalSection = pCS;
		EnterCriticalSection(m_pcsCriticalSection);
	}
	~CCSLock()
	{
		LeaveCriticalSection(m_pcsCriticalSection);
	}
	CRITICAL_SECTION *	m_pcsCriticalSection;
};

/*	CNDIManager

	CNDIManager manages the set of NDI channels.
	All commands from the Unity client run through it.
	It also creates the thread that is used to write frames into the texture buffers.
*/

class CNDIManager
{
	CNDIChannel			m_aChannels[NDI_PORTS];	// Array of NDI channels.
	bool				m_bInitialized;			// Is NDI initialized?
	int					m_nPortCount;			// Total number of available NDI channels, returned by the NDI find.
	char				m_aszPortNames[NDI_PORTS][MAX_PORT_NAME_LENGTH];	// Set of names of available NDI ports.
	char				m_aszIPAddresses[NDI_PORTS][MAX_PORT_NAME_LENGTH];	// And their IP addresses.
	DWORD				m_dwThreadId;			// Thread id for background thread.
	HANDLE				m_hThread;				// Windows OS handle of the background thread.
	bool				m_bAlive;				// Flag to control shutting down the thread.
	CRITICAL_SECTION	m_csCriticalSection;	// Critical section to ensure thread safety.
	
public:
	CNDIManager();
	~CNDIManager();
	bool StartThread();
	void EndThread();
	void RunThread();
	bool ReadNDIFrame(int nChannel, void* target, int nWidth, int nHeight);
	bool SetNDIFrameBuffer(int nChannel, void* target, int nWidth, int nHeight);
	bool ClearNDIFrameBuffer(int nChannel);
	bool GetNDIPortName(int nChannel, char *pszName, int nBuffSize);
	int GetNDIPortWidth(int nChannel);
	int GetNDIPortHeight(int nChannel);
	bool EnumNDIPorts(int nPort, char *pszPortName, int nBuffSize);
	bool OpenNDIPort(int nChannel, int nPort);
	bool CloseNDIPort(int nChannel);
};

