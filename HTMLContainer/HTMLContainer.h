
// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the HTMLContainer_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// HTMLContainer_API functions as being imported from a DLL, wheras this DLL sees symbols
// defined with this macro as being exported.
#ifdef HTMLContainer_EXPORTS
#define HTMLContainer_API __declspec(dllexport)
#else
#define HTMLContainer_API __declspec(dllimport)
#endif

#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <htmlctrl.h>

extern "C"
{
	HTMLContainer_API HWND CreateHTMLControl(HWND hwndParent, HWND hwndMessageWindow);
	HTMLContainer_API void DestroyHTMLControl();
}
