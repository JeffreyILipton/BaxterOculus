//Don't forget to change project settings:
//1. C++: add include path to DirectShow include folder (such as c:\dxsdk\include)
//2. Link: add link path to DirectShow lib folder (such as c:\dxsdk\lib).
//3. Link: add strmiids.lib and quartz.lib

#include "stdafx.h"
#include <DShow.h>
#include <atlbase.h>
#include <initguid.h>
#include <dvdmedia.h>
// take this file from GraphEditPlus folder and use it 
#include "SampleGrabber.h" // if your version of Windows SDK doesn't know about SampleGrabber
#include "Processing.NDI.Lib.h"

#define FRAME_WIDTH  960 //1920
#define FRAME_HEIGHT 540 //1080

BOOL hrcheck(HRESULT hr, TCHAR* errtext)
{
	if (hr >= S_OK)
		return FALSE;
	TCHAR szErr[MAX_ERROR_TEXT_LEN];
	DWORD res = AMGetErrorText(hr, szErr, MAX_ERROR_TEXT_LEN);
	if (res)
		_tprintf(_T("Error %x: %s\n%s\n"), hr, errtext, szErr);
	else
		_tprintf(_T("Error %x: %s\n"), hr, errtext);
	return TRUE;
}

//change this macro to fit your style of error handling
#define CHECK_HR(hr, msg) if (hrcheck(hr, msg)) return hr;



CComPtr<IBaseFilter> CreateFilterByName(const WCHAR* filterName, const GUID& category)
{
	HRESULT hr = S_OK;
	CComPtr<ICreateDevEnum> pSysDevEnum;
	hr = pSysDevEnum.CoCreateInstance(CLSID_SystemDeviceEnum);
	if (hrcheck(hr, _T("Can't create System Device Enumerator")))
		return NULL;

	CComPtr<IEnumMoniker> pEnumCat;
	hr = pSysDevEnum->CreateClassEnumerator(category, &pEnumCat, 0);

	if (hr == S_OK)
	{
		CComPtr<IMoniker> pMoniker;
		ULONG cFetched;
		while (pEnumCat->Next(1, &pMoniker, &cFetched) == S_OK)
		{
			CComPtr<IPropertyBag> pPropBag;
			hr = pMoniker->BindToStorage(0, 0, IID_IPropertyBag, (void **)&pPropBag);
			if (SUCCEEDED(hr))
			{
				VARIANT varName;
				VariantInit(&varName);
				hr = pPropBag->Read(L"FriendlyName", &varName, 0);
				if (SUCCEEDED(hr))
				{
					if (wcscmp(filterName, varName.bstrVal) == 0) {
						CComPtr<IBaseFilter> pFilter;
						hr = pMoniker->BindToObject(NULL, NULL, IID_IBaseFilter, (void**)&pFilter);
						if (hrcheck(hr, _T("Can't bind moniker to filter object")))
							return NULL;
						return pFilter;
					}
				}
				VariantClear(&varName);
			}
			pMoniker.Release();
		}
	}
	return NULL;
}

CComPtr<IBaseFilter> CreateFilterByIndex(int nIndex, const GUID& category)
{
	HRESULT hr = S_OK;
	CComPtr<ICreateDevEnum> pSysDevEnum;
	hr = pSysDevEnum.CoCreateInstance(CLSID_SystemDeviceEnum);
	if (hrcheck(hr, _T("Can't create System Device Enumerator")))
		return NULL;

	CComPtr<IEnumMoniker> pEnumCat;
	hr = pSysDevEnum->CreateClassEnumerator(category, &pEnumCat, 0);

	if (hr == S_OK)
	{
		CComPtr<IMoniker> pMoniker;
		ULONG cFetched;
		while (pEnumCat->Next(1, &pMoniker, &cFetched) == S_OK)
		{
			CComPtr<IPropertyBag> pPropBag;
			hr = pMoniker->BindToStorage(0, 0, IID_IPropertyBag, (void **)&pPropBag);
			if (SUCCEEDED(hr))
			{
				VARIANT varName;
				VariantInit(&varName);
				hr = pPropBag->Read(L"FriendlyName", &varName, 0);
				if (SUCCEEDED(hr))
				{
					// Is this the nth one? Index starts at 0.
					if (nIndex <= 0)
					{
						CComPtr<IBaseFilter> pFilter;
						hr = pMoniker->BindToObject(NULL, NULL, IID_IBaseFilter, (void**)&pFilter);
						if (hrcheck(hr, _T("Can't bind moniker to filter object")))
							return NULL;
						return pFilter;
					}
					nIndex--;
				}
				VariantClear(&varName);
			}
			pMoniker.Release();
		}
	}
	return NULL;
}

bool EnumCameras(const GUID& category, int nIndex, char *pszNameBuffer, int nBufferLength)
{
	HRESULT hr = S_OK;
	CComPtr<ICreateDevEnum> pSysDevEnum;
	hr = pSysDevEnum.CoCreateInstance(CLSID_SystemDeviceEnum);
	if (hrcheck(hr, _T("Can't create System Device Enumerator")))
		return false;

	CComPtr<IEnumMoniker> pEnumCat;
	hr = pSysDevEnum->CreateClassEnumerator(category, &pEnumCat, 0);

	printf("These are all the available cameras:\n");
	if (hr == S_OK)
	{
		CComPtr<IMoniker> pMoniker;
		ULONG cFetched;
		while (pEnumCat->Next(1, &pMoniker, &cFetched) == S_OK)
		{
			CComPtr<IPropertyBag> pPropBag;
			hr = pMoniker->BindToStorage(0, 0, IID_IPropertyBag, (void **)&pPropBag);
			if (SUCCEEDED(hr))
			{
				VARIANT varName;
				VariantInit(&varName);
				hr = pPropBag->Read(L"FriendlyName", &varName, 0);
				if (SUCCEEDED(hr))
				{
					printf("    %S\n", varName.bstrVal);
					nIndex--;
					if (nIndex < 0)
					{
						WideCharToMultiByte(CP_UTF8, 0, varName.bstrVal, -1, pszNameBuffer, nBufferLength,NULL,NULL);
						VariantClear(&varName);
						pMoniker.Release();
						return true;

					}
				}
				VariantClear(&varName);
			}
			pMoniker.Release();
		}
	}
	return false;
}

CComPtr<IPin> GetPin(IBaseFilter *pFilter, LPCOLESTR pinname)
{
	CComPtr<IEnumPins>  pEnum;
	CComPtr<IPin>       pPin;

	HRESULT hr = pFilter->EnumPins(&pEnum);
	if (hrcheck(hr, _T("Can't enumerate pins.")))
		return NULL;

	while (pEnum->Next(1, &pPin, 0) == S_OK)
	{
		PIN_INFO pinfo;
		pPin->QueryPinInfo(&pinfo);
		BOOL found = !wcsicmp(pinname, pinfo.achName);
		if (pinfo.pFilter) pinfo.pFilter->Release();
		if (found)
			return pPin;
		pPin.Release();
	}
	printf("Pin not found!\n");
	return NULL;
}

// {6994AD05-93EF-11D0-A3CC-00A0C9223196}
DEFINE_GUID(CLSID_KSVideo,
	0x6994AD05, 0x93EF, 0x11D0, 0xA3, 0xCC, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96); //


																				 // {C1F400A0-3F08-11D3-9F0B-006008039E37}
DEFINE_GUID(CLSID_SampleGrabber,
	0xC1F400A0, 0x3F08, 0x11D3, 0x9F, 0x0B, 0x00, 0x60, 0x08, 0x03, 0x9E, 0x37); //qedit.dll


class CNDISender : public ISampleGrabberCB {
public:
	CNDISender(int nWidth, int nHeight, char *pszCameraName);


	STDMETHODIMP QueryInterface(REFIID riid, void **ppv) {
		if (NULL == ppv) return E_POINTER;
		if (riid == __uuidof(IUnknown))
		{
			*ppv = static_cast<IUnknown*>(this);
			return S_OK;
		}
		if (riid == __uuidof(ISampleGrabberCB))
		{
			*ppv = static_cast<ISampleGrabberCB*>(this);
			return S_OK;
		}
		return E_NOINTERFACE;
	};
	STDMETHODIMP_(ULONG) AddRef() {
		return S_OK;
	};
	STDMETHODIMP_(ULONG) Release() {
		return S_OK;
	};

	//ISampleGrabberCB
	STDMETHODIMP SampleCB(double SampleTime, IMediaSample *pSample);
	STDMETHODIMP BufferCB(double SampleTime, BYTE *pBuffer, long BufferLen) 
	{ 
		return S_OK; 
	};

protected:
	int m_nWidth, m_nHeight;
	NDIlib_send_instance_t	m_ndiSend;
	NDIlib_video_frame_t	m_ndiFrame;
};

CNDISender::CNDISender(int nWidth, int nHeight, char *pszCameraName)
{
	m_nWidth = nWidth;
	m_nHeight = nHeight;
	if (NDIlib_initialize())
	{
		// Create an NDI source that is called "USB Camera" and is clocked to the video.
		const NDIlib_send_create_t NDI_send_create_desc = { pszCameraName, NULL, TRUE, FALSE };

		// We create the NDI sender
		m_ndiSend = NDIlib_send_create(&NDI_send_create_desc);
		if (m_ndiSend)
		{
			m_ndiFrame.FourCC = NDIlib_FourCC_type_BGRA;
			m_ndiFrame.frame_rate_D = 1001;
			m_ndiFrame.frame_rate_N = 30000;
			m_ndiFrame.is_progressive = TRUE;
			m_ndiFrame.line_stride_in_bytes = 4 * nWidth;
			m_ndiFrame.picture_aspect_ratio = 16.0f / 9.0f;
			m_ndiFrame.p_data = (BYTE*)malloc(nWidth * nHeight * 4);
			m_ndiFrame.timecode = NDIlib_send_timecode_synthesize;
			m_ndiFrame.xres = nWidth;
			m_ndiFrame.yres = nHeight;
		}
	}
}

/* CNDISender::SampleCB

   This is the critical callback from the DShow SampleGrabber filter that
   hands us the frame buffer to send to NDI.

   Although the format is correct for NDI, it seems to be upside down, 
   so the code copies line by line from top to bottom over to bottom to top. 

*/

STDMETHODIMP CNDISender::SampleCB(double SampleTime, IMediaSample *pSample)
{
	if (pSample) 
	{
		long sz = pSample->GetActualDataLength();
		BYTE *pBuf = NULL;
		pSample->GetPointer(&pBuf);
		if (pBuf && m_ndiFrame.p_data)
		{
			int nSize = m_nWidth * 4;
			for (int nRow = 0; nRow < m_nHeight; nRow++)
			{
				BYTE *pOutLine = m_ndiFrame.p_data + (m_nHeight - (nRow + 1)) * nSize;
				memcpy_s(pOutLine, nSize, pBuf, nSize);
				pBuf += nSize;
			}
			
			NDIlib_send_send_video(m_ndiSend, &m_ndiFrame);
		}
	}
	return S_OK;
}


HRESULT BuildGraph(IGraphBuilder *pGraph, int nIndex, char *pszCameraName, int nWidth, int nHeight)
{
	HRESULT hr = S_OK;

	//graph builder
	CComPtr<ICaptureGraphBuilder2> pBuilder;
	hr = pBuilder.CoCreateInstance(CLSID_CaptureGraphBuilder2);
	CHECK_HR(hr, _T("Can't create Capture Graph Builder"));
	hr = pBuilder->SetFiltergraph(pGraph);
	CHECK_HR(hr, _T("Can't SetFiltergraph"));

	//add HD USB Camera
	const size_t WCHARBUF = 100;
	wchar_t  wszDest[WCHARBUF];
	MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, pszCameraName, -1, wszDest, WCHARBUF);
	CComPtr<IBaseFilter> pHDUSBCamera = CreateFilterByIndex(nIndex, CLSID_KSVideo);
	hr = pGraph->AddFilter(pHDUSBCamera, wszDest);
	CHECK_HR(hr, _T("Failed creating camera"));

	AM_MEDIA_TYPE pmt;
	ZeroMemory(&pmt, sizeof(AM_MEDIA_TYPE));
	pmt.majortype = MEDIATYPE_Video;
	pmt.subtype = MEDIASUBTYPE_MJPG;
	pmt.formattype = FORMAT_VideoInfo;
	pmt.bFixedSizeSamples = TRUE;
	pmt.cbFormat = 88;
	pmt.lSampleSize = 6220800;
	pmt.bTemporalCompression = FALSE;
	VIDEOINFOHEADER format;
	ZeroMemory(&format, sizeof(VIDEOINFOHEADER));
	format.dwBitRate = nWidth * nHeight * 3 * 8 * 30; // 1492992000;
	format.AvgTimePerFrame = 333333;
	format.bmiHeader.biSize = 40;
	format.bmiHeader.biWidth = nWidth;
	format.bmiHeader.biHeight = nHeight;
	format.bmiHeader.biPlanes = 1;
	format.bmiHeader.biBitCount = 24;
	format.bmiHeader.biCompression = 1196444237; 
	format.bmiHeader.biSizeImage = nWidth * nHeight * 3;
	pmt.pbFormat = (BYTE*)&format;
	CComQIPtr<IAMStreamConfig, &IID_IAMStreamConfig> isc(GetPin(pHDUSBCamera, L"Capture"));
	hr = isc->SetFormat(&pmt);
	CHECK_HR(hr, _T("Can't set format"));


	//add MJPEG Decompressor
	CComPtr<IBaseFilter> pMJPEGDecompressor;
	hr = pMJPEGDecompressor.CoCreateInstance(CLSID_MjpegDec);
	CHECK_HR(hr, _T("Can't create MJPEG Decompressor"));
	hr = pGraph->AddFilter(pMJPEGDecompressor, L"MJPEG Decompressor");
	CHECK_HR(hr, _T("Can't add MJPEG Decompressor to graph"));


	//connect HD USB Camera and MJPEG Decompressor
	hr = pGraph->ConnectDirect(GetPin(pHDUSBCamera, L"Capture"), GetPin(pMJPEGDecompressor, L"XForm In"), NULL);
	CHECK_HR(hr, _T("Can't connect Camera and MJPEG Decompressor"));


	//add Color Space Converter
	CComPtr<IBaseFilter> pColorSpaceConverter;
	hr = pColorSpaceConverter.CoCreateInstance(CLSID_Colour);
	CHECK_HR(hr, _T("Can't create Color Space Converter"));
	hr = pGraph->AddFilter(pColorSpaceConverter, L"Color Space Converter");
	CHECK_HR(hr, _T("Can't add Color Space Converter to graph"));


	//connect MJPEG Decompressor and Color Space Converter
	hr = pGraph->ConnectDirect(GetPin(pMJPEGDecompressor, L"XForm Out"), GetPin(pColorSpaceConverter, L"Input"), NULL);
	CHECK_HR(hr, _T("Can't connect MJPEG Decompressor and Color Space Converter"));


	//add SampleGrabber
	CComPtr<IBaseFilter> pSampleGrabber;
	hr = pSampleGrabber.CoCreateInstance(CLSID_SampleGrabber);
	CHECK_HR(hr, _T("Can't create SampleGrabber"));
	hr = pGraph->AddFilter(pSampleGrabber, L"SampleGrabber");
	CHECK_HR(hr, _T("Can't add SampleGrabber to graph"));
	AM_MEDIA_TYPE pSampleGrabber_pmt;
	ZeroMemory(&pSampleGrabber_pmt, sizeof(AM_MEDIA_TYPE));
	pSampleGrabber_pmt.majortype = MEDIATYPE_Video;
	pSampleGrabber_pmt.subtype = MEDIASUBTYPE_ARGB32;
	pSampleGrabber_pmt.formattype = FORMAT_VideoInfo;
	pSampleGrabber_pmt.bFixedSizeSamples = TRUE;
	pSampleGrabber_pmt.cbFormat = 88;
	pSampleGrabber_pmt.lSampleSize = 8294400;
	pSampleGrabber_pmt.bTemporalCompression = FALSE;
	VIDEOINFOHEADER pSampleGrabber_format;
	ZeroMemory(&pSampleGrabber_format, sizeof(VIDEOINFOHEADER));
	pSampleGrabber_format.dwBitRate = 1990657991;
	pSampleGrabber_format.AvgTimePerFrame = 333333;
	pSampleGrabber_format.bmiHeader.biSize = 40;
	pSampleGrabber_format.bmiHeader.biWidth = FRAME_WIDTH;
	pSampleGrabber_format.bmiHeader.biHeight = FRAME_HEIGHT;
	pSampleGrabber_format.bmiHeader.biPlanes = 1;
	pSampleGrabber_format.bmiHeader.biBitCount = 32;
	pSampleGrabber_format.bmiHeader.biSizeImage = 8294400;
	pSampleGrabber_pmt.pbFormat = (BYTE*)&pSampleGrabber_format;
	CComQIPtr<ISampleGrabber, &IID_ISampleGrabber> pSampleGrabber_isg(pSampleGrabber);
	hr = pSampleGrabber_isg->SetMediaType(&pSampleGrabber_pmt);
	CHECK_HR(hr, _T("Can't set media type to sample grabber"));

	char szNameForNDI[200];
	sprintf(szNameForNDI, "%s %d", pszCameraName, nIndex);

	CNDISender* pCB = new CNDISender(FRAME_WIDTH, FRAME_HEIGHT, szNameForNDI);
	hr = pSampleGrabber_isg->SetCallback(pCB, 0);
	CHECK_HR(hr, _T("Can't set sample grabber callback"));

	//connect Color Space Converter and SampleGrabber
	hr = pGraph->ConnectDirect(GetPin(pColorSpaceConverter, L"XForm Out"), GetPin(pSampleGrabber, L"Input"), NULL);
	CHECK_HR(hr, _T("Can't connect Color Space Converter and SampleGrabber"));


	return S_OK;
}

//int _tmain(int argc, _TCHAR* argv[]) //use this line in VS2008
int main(int argc, char* argv[])
{

	CoInitialize(NULL);

	CComPtr<IGraphBuilder> graphLeft;
	graphLeft.CoCreateInstance(CLSID_FilterGraph);
	CComPtr<IGraphBuilder> graphRight;
	graphRight.CoCreateInstance(CLSID_FilterGraph);

	
	bool bFirst = true;
	HRESULT hr;

	for (int nIndex = 0;; nIndex++)
	{
		char szCameraName[200];
		// Get the nth camera
		if (EnumCameras(CLSID_KSVideo, nIndex, szCameraName, sizeof(szCameraName)))
		{
			// THen build the graph with it.
			if (bFirst)
			{
				hr = BuildGraph(graphLeft, nIndex, szCameraName, FRAME_WIDTH, FRAME_HEIGHT);
			}
			else
			{
				hr = BuildGraph(graphRight, nIndex, szCameraName, FRAME_WIDTH, FRAME_HEIGHT);
			}
			if (SUCCEEDED(hr))
			{
				if (bFirst) bFirst = false;
				// If this is the second one, then we are done.
				else break;
			}
		}
		else
		{
			break;
		}
	}


	if (hr == S_OK) {
		printf("Running");
		CComQIPtr<IMediaControl, &IID_IMediaControl> mediaControlLeft(graphLeft);
		hr = mediaControlLeft->Run();
		CHECK_HR(hr, _T("Can't run the graph"));
		CComQIPtr<IMediaControl, &IID_IMediaControl> mediaControlRight(graphRight);
		hr = mediaControlRight->Run();
		CHECK_HR(hr, _T("Can't run the graph"));
		CComQIPtr<IMediaEvent, &IID_IMediaEvent> mediaEventLeft(graphLeft);
		CComQIPtr<IMediaEvent, &IID_IMediaEvent> mediaEventRight(graphRight);
		BOOL stop = FALSE;
		MSG msg;
		while (!stop)
		{
			long ev = 0;
			LONG_PTR p1 = 0, p2 = 0;
			Sleep(500);
			printf(".");
			while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
				DispatchMessage(&msg);
			while (mediaEventLeft->GetEvent(&ev, &p1, &p2, 0) == S_OK)
			{
				if (ev == EC_COMPLETE || ev == EC_USERABORT)
				{
					printf("Done!\n");
					stop = TRUE;
				}
				else
					if (ev == EC_ERRORABORT)
					{
						printf("An error occured: HRESULT=%x\n", p1);
						mediaControlLeft->Stop();
						stop = TRUE;
					}
				mediaEventLeft->FreeEventParams(ev, p1, p2);
			}
			while (mediaEventRight->GetEvent(&ev, &p1, &p2, 0) == S_OK)
			{
				if (ev == EC_COMPLETE || ev == EC_USERABORT)
				{
					printf("Done!\n");
					stop = TRUE;
				}
				else
					if (ev == EC_ERRORABORT)
					{
						printf("An error occured: HRESULT=%x\n", p1);
						mediaControlRight->Stop();
						stop = TRUE;
					}
				mediaEventRight->FreeEventParams(ev, p1, p2);
			}
		}
	}
	CoUninitialize();
	return 0;
}
