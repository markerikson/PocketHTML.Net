// HTMLContainer.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "HTMLContainer.h"
#include <string.h>

#define WC_HTMLCONTAINER TEXT("HTMLContainer")
LRESULT TestWndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

HINSTANCE g_hInstance = NULL;
HWND g_hwndContainer = NULL;
HWND g_hwndHtml = NULL;
HWND g_hwndParent = NULL;
HWND g_hwndMessageWindow = NULL;

#define WM_IMAGENOTIFY (WM_USER + 42)

HRESULT RegisterTestClass(HINSTANCE hInstance)
{
	WNDCLASS    wc = { 0 };
	HRESULT     hrResult;

	if (!GetClassInfo(hInstance, WC_HTMLCONTAINER, &wc))
	{
		wc.style            = CS_HREDRAW | CS_VREDRAW | CS_GLOBALCLASS;
		wc.lpfnWndProc      = TestWndProc;
		wc.hInstance        = hInstance;
		wc.lpszClassName    = WC_HTMLCONTAINER;
		wc.hCursor          = LoadCursor(NULL, IDC_ARROW);
		wc.hbrBackground    = (HBRUSH)(COLOR_WINDOW + 1);

		hrResult = (RegisterClass(&wc) ? S_OK : E_FAIL);
	}
	else
		hrResult = S_OK;

	return hrResult;
}

BOOL APIENTRY DllMain( HANDLE hModule, 
					  DWORD  ul_reason_for_call, 
					  LPVOID lpReserved)
{
	switch(ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		{
			g_hInstance = (HMODULE)hModule;

			return (RegisterTestClass(g_hInstance) == S_OK);
		}
		break;
	case DLL_PROCESS_DETACH:
		{
			UnregisterClass(WC_HTMLCONTAINER, g_hInstance);
		}
		break;
	}
	return FALSE;
}



HTMLContainer_API HWND CreateHTMLControl(HWND hwndParent, HWND hwndMessageWindow)
{
	g_hwndParent = hwndParent;
	g_hwndMessageWindow = hwndMessageWindow;
	RECT rc;
	GetClientRect(hwndParent, &rc);

	g_hwndContainer = CreateWindow(  WC_HTMLCONTAINER,
		NULL,
		WS_VISIBLE,
		CW_USEDEFAULT, 
		CW_USEDEFAULT,
		rc.right,
		rc.bottom,
		g_hwndParent,
		NULL,
		g_hInstance,
		0);

	InitHTMLControl(g_hInstance);

	g_hwndHtml = CreateWindow(WC_HTML,
		NULL,
		WS_CHILD | WS_BORDER | WS_VISIBLE,
		0,
		0,
		rc.right,
		rc.bottom,
		g_hwndContainer,
		NULL,
		g_hInstance,
		NULL);

	return g_hwndHtml;

}


HTMLContainer_API void DestroyHTMLControl()
{
	DestroyWindow(g_hwndContainer);
}


LRESULT TestWndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	LRESULT lResult = 0;

	switch (uMsg)
	{
	case WM_NOTIFY:
		{
			NMHDR * pnmh = (NMHDR *)lParam;
			NM_HTMLVIEW * pnmHTML = (NM_HTMLVIEW *) lParam;

			switch (pnmh->code)
			{
			case NM_HOTSPOT:
				{
					//OutputDebugString(TEXT("NM_HOTSPOT\r\n"));
					break;
				}

			case NM_INLINE_IMAGE:
				{
					TCHAR pathName[255];
					TCHAR* szPathName = pathName;

					HBITMAP hBitmap = NULL;
					int result = SendMessage(g_hwndMessageWindow, WM_IMAGENOTIFY, 
											(WPARAM)pnmHTML->szTarget, (LPARAM)szPathName);
					//MessageBox(g_hwndHtml, TEXT("NM_INLINE_IMAGE\r\n"), TEXT("Caption"), MB_OK);

					if(result > 0)
					{
						hBitmap = SHLoadImageFile(szPathName);
					}

					if(hBitmap != NULL)
					{
						BITMAP bitmap;
						GetObject(hBitmap, sizeof(bitmap), &bitmap);

						INLINEIMAGEINFO imageInfo;

						imageInfo.dwCookie    = pnmHTML->dwCookie;
						imageInfo.bOwnBitmap  = TRUE;
						imageInfo.hbm         = hBitmap;
						imageInfo.iOrigWidth = bitmap.bmWidth;
						imageInfo.iOrigHeight = bitmap.bmHeight;
						::SendMessage(g_hwndHtml, DTM_SETIMAGE, 0, (LPARAM)(INLINEIMAGEINFO*)&imageInfo);						
					}
					else
					{
						SendMessage(g_hwndHtml, DTM_IMAGEFAIL, 0, (LPARAM)pnmHTML->dwCookie);
						//lResult = 0;
					}
					
					lResult = 1;
					//break;
				}

			case NM_INLINE_SOUND:
				{
					//OutputDebugString(TEXT("NM_INLINE_SOUND\r\n"));
					break;
				}

			case NM_TITLE:
				{
					//OutputDebugString(TEXT("NM_TITLE\r\n"));
					break;
				}

			case NM_META:
				{
					//OutputDebugString(TEXT("NM_META\r\n"));
					break;
				}

			case NM_BASE:
				{
					//OutputDebugString(TEXT("NM_BASE\r\n"));
					break;
				}

			case NM_CONTEXTMENU:
				{
					//OutputDebugString(TEXT("NM_CONTEXTMENU\r\n"));
					break;
				}

			case NM_INLINE_XML:
				{
					//OutputDebugString(TEXT("NM_INLINE_XML\r\n"));
					break;
				}

			case NM_BEFORENAVIGATE:
				{
					//OutputDebugString(TEXT("NM_BEFORENAVIGATE\r\n"));
					break;
				}

			case NM_DOCUMENTCOMPLETE:
				{
					//OutputDebugString(TEXT("NM_DOCUMENTCOMPLETE\r\n"));
					break;
				}

			case NM_NAVIGATECOMPLETE:
				{
					//OutputDebugString(TEXT("NM_NAVIGATECOMPLETE\r\n"));
					break;
				}

			case NM_TITLECHANGE:
				{
					//OutputDebugString(TEXT("NM_TITLECHANGE\r\n"));
					break;
				}

			default:
				{
					ASSERT(FALSE);
					break;
				}
			}

			break;
		}

		/*
		case WM_LBUTTONDOWN:
		{
		DestroyWindow(hwnd);
		break;
		}

		case WM_DESTROY:
		{
		PostQuitMessage(0);
		break;
		}
		*/  

	default:
		{
			lResult = DefWindowProc(hwnd, uMsg, wParam, lParam);
			break;
		}
	}

	return lResult;
}

