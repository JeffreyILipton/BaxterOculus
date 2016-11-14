///Don't forget to change project settings:
//1. C++: add include path to DirectShow include folder (such as c:\dxsdk\include)
//2. Link: add link path to DirectShow lib folder (such as c:\dxsdk\lib).
//3. Link: add strmiids.lib and quartz.lib

#include "stdafx.h"
#include <DShow.h>
#include <atlbase.h>
#include <initguid.h>
#include <dvdmedia.h>

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


																				 // {B87BEB7B-8D29-423F-AE4D-6582C10175AC}
DEFINE_GUID(CLSID_VideoRenderer,
	0xB87BEB7B, 0x8D29, 0x423F, 0xAE, 0x4D, 0x65, 0x82, 0xC1, 0x01, 0x75, 0xAC); //quartz.dll




HRESULT BuildGraph(IGraphBuilder *pGraph)
{
	HRESULT hr = S_OK;

	//graph builder
	CComPtr<ICaptureGraphBuilder2> pBuilder;
	hr = pBuilder.CoCreateInstance(CLSID_CaptureGraphBuilder2);
	CHECK_HR(hr, _T("Can't create Capture Graph Builder"));
	hr = pBuilder->SetFiltergraph(pGraph);
	CHECK_HR(hr, _T("Can't SetFiltergraph"));

	//add HD USB Camera
	CComPtr<IBaseFilter> pHDUSBCamera2 = CreateFilterByName(L"HD USB Camera", CLSID_KSVideo);
	hr = pGraph->AddFilter(pHDUSBCamera2, L"HD USB Camera");
	CHECK_HR(hr, _T("Can't add HD USB Camera to graph"));


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
	format.dwBitRate = 1492992000;
	format.AvgTimePerFrame = 333333;
	format.bmiHeader.biSize = 40;
	format.bmiHeader.biWidth = 1920;
	format.bmiHeader.biHeight = 1080;
	format.bmiHeader.biPlanes = 1;
	format.bmiHeader.biBitCount = 24;
	format.bmiHeader.biCompression = 1196444237;
	format.bmiHeader.biSizeImage = 6220800;
	pmt.pbFormat = (BYTE*)&format;
	CComQIPtr<IAMStreamConfig, &IID_IAMStreamConfig> isc(GetPin(pHDUSBCamera2, L"Capture"));
	hr = isc->SetFormat(&pmt);
	CHECK_HR(hr, _T("Can't set format"));


	//add MJPEG Decompressor
	CComPtr<IBaseFilter> pMJPEGDecompressor;
	hr = pMJPEGDecompressor.CoCreateInstance(CLSID_MjpegDec);
	CHECK_HR(hr, _T("Can't create MJPEG Decompressor"));
	hr = pGraph->AddFilter(pMJPEGDecompressor, L"MJPEG Decompressor");
	CHECK_HR(hr, _T("Can't add MJPEG Decompressor to graph"));


	//connect HD USB Camera and MJPEG Decompressor
	hr = pGraph->ConnectDirect(GetPin(pHDUSBCamera2, L"Capture"), GetPin(pMJPEGDecompressor, L"XForm In"), NULL);
	CHECK_HR(hr, _T("Can't connect HD USB Camera and MJPEG Decompressor"));


	//add Color Space Converter
	CComPtr<IBaseFilter> pColorSpaceConverter;
	hr = pColorSpaceConverter.CoCreateInstance(CLSID_Colour);
	CHECK_HR(hr, _T("Can't create Color Space Converter"));
	hr = pGraph->AddFilter(pColorSpaceConverter, L"Color Space Converter");
	CHECK_HR(hr, _T("Can't add Color Space Converter to graph"));


	//connect MJPEG Decompressor and Color Space Converter
	hr = pGraph->ConnectDirect(GetPin(pMJPEGDecompressor, L"XForm Out"), GetPin(pColorSpaceConverter, L"Input"), NULL);
	CHECK_HR(hr, _T("Can't connect MJPEG Decompressor and Color Space Converter"));


	//add Video Renderer
	CComPtr<IBaseFilter> pVideoRenderer2;
	hr = pVideoRenderer2.CoCreateInstance(CLSID_VideoRenderer);
	CHECK_HR(hr, _T("Can't create Video Renderer"));
	hr = pGraph->AddFilter(pVideoRenderer2, L"Video Renderer");
	CHECK_HR(hr, _T("Can't add Video Renderer to graph"));


	//connect Color Space Converter and Video Renderer
	hr = pGraph->ConnectDirect(GetPin(pColorSpaceConverter, L"XForm Out"), GetPin(pVideoRenderer2, L"VMR Input0"), NULL);
	CHECK_HR(hr, _T("Can't connect Color Space Converter and Video Renderer"));


	return S_OK;
}

//int _tmain(int argc, _TCHAR* argv[]) //use this line in VS2008
int main(int argc, char* argv[])
{
	CoInitialize(NULL);
	CComPtr<IGraphBuilder> graph;
	graph.CoCreateInstance(CLSID_FilterGraph);

	printf("Building graph...\n");
	HRESULT hr = BuildGraph(graph);
	if (hr == S_OK) {
		printf("Running");
		CComQIPtr<IMediaControl, &IID_IMediaControl> mediaControl(graph);
		hr = mediaControl->Run();
		CHECK_HR(hr, _T("Can't run the graph"));
		CComQIPtr<IMediaEvent, &IID_IMediaEvent> mediaEvent(graph);
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
			while (mediaEvent->GetEvent(&ev, &p1, &p2, 0) == S_OK)
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
						mediaControl->Stop();
						stop = TRUE;
					}
				mediaEvent->FreeEventParams(ev, p1, p2);
			}
		}
	}
	CoUninitialize();
	return 0;
}
