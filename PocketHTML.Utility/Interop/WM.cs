//==========================================================================================
//
//		OpenNETCF.Win32.WM
//		Copyright (c) 2005, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================

using System;

namespace OpenNETCF.Win32
{
	/// <summary>
	/// Window Messages.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum WM: int
	{
		CREATE			= 0x1,
		DESTROY			= 0x2,
		MOVE			= 0x3,
		SIZE			= 0x5,
		ACTIVATE		= 0x6,
		SETFOCUS		= 0x7,
		KILLFOCUS		= 0x8,
		SETREDRAW		= 0xB,
		SETTEXT			= 0xC,
		GETTEXT			= 0xD,
		GETTEXTLENGTH	= 0xE,
		PAINT			= 0xF,
		CLOSE			= 0x0010,
		QUIT				= 0x0012,
		ERASEBKGND			= 0x0014,
		SYSCOLORCHANGE		= 0x0015,
		SHOWWINDOW			= 0x0018,
		WININICHANGE		= 0x001A,
		SETTINGCHANGE		= WININICHANGE,
		FONTCHANGE			= 0x001D,
		CANCELMODE			= 0x001F,
		SETCURSOR			= 0x20,
		NEXTDLGCTL			= 0x28,
		DRAWITEM			= 0x2B,
		MEASUREITEM =0x002C,   
		DELETEITEM =0x002D,   
		SETFONT =0x0030,   
		GETFONT =0x0031,   
		COMPAREITEM =0x0039,   
		WINDOWPOSCHANGED =0x0047, 
		NOTIFY				= 0x004E,
		HELP				= 0x0053,
		STYLECHANGED =0x007D,   
	GETDLGCODE =0x0087,   
	KEYFIRST =0x0100,   
	KEYDOWN =0x0100,   
	KEYUP =0x0101,   
	CHAR =0x0102,   
	DEADCHAR =0x0103,   
	SYSKEYDOWN =0x0104,   
	SYSKEYUP =0x0105,   
	SYSCHAR =0x0106,   
	SYSDEADCHAR =0x0107,   
	KEYLAST =0x0108,   
	IM_INFO =0x010C,   
	IME_STARTCOMPOSITION =0x010D,   
	IME_ENDCOMPOSITION =0x010E,   
	IME_COMPOSITION =0x010F,   
	IME_KEYLAST =0x010F, 

		IME_SETCONTEXT =0x0281,   
	IME_NOTIFY =0x0282,   
	IME_CONTROL =0x0283,   
	IME_COMPOSITIONFULL =0x0284   ,
	IME_SELECT =0x0285   ,
	IME_CHAR =0x0286   ,
	IME_SYSTEM =0x0287  , 
	IME_REQUEST =0x0288  , 
	IME_KEYDOWN =0x0290   ,
	IME_KEYUP =0x0291   ,
	INITDIALOG =0x0110   ,
	COMMAND =0x0111   ,
	SYSCOMMAND =0x0112 ,  
	TIMER =0x0113   ,
	HSCROLL =0x0114  , 
	VSCROLL =0x0115   ,
	INITMENUPOPUP =0x0117   ,
	MENUCHAR =0x0120   ,
	MOUSEFIRST =0x0200  , 
	MOUSEMOVE =0x0200   ,
	/// <summary>   
	/// This message is posted when the user presses the touch-screen in the client area of a window.   
	/// </summary>      
	LBUTTONDOWN =0x0201  , 
	LBUTTONUP =0x0202   ,
	LBUTTONDBLCLK =0x0203,   
	RBUTTONDOWN =0x0204   ,
	RBUTTONUP =0x0205   ,
	RBUTTONDBLCLK =0x0206,   
	MBUTTONDOWN =0x0207   ,
	MBUTTONUP =0x0208   ,
	MBUTTONDBLCLK =0x0209,   
	MOUSEWHEEL =0x020A   ,
	MOUSELAST =0x020A   ,
	ENTERMENULOOP =0x0211,   
	EXITMENULOOP =0x0212  , 
	CAPTURECHANGED =0x0215 ,  
	CUT =0x0300   ,
	COPY =0x0301   ,
	PASTE =0x0302   ,
	CLEAR =0x0303   ,
	UNDO =0x0304   ,
	RENDERFORMAT =0x0305   ,
	RENDERALLFORMATS =0x0306,   
	DESTROYCLIPBOARD =0x0307 ,  
	QUERYNEWPALETTE =0x030F   ,
	PALETTECHANGED =0x0311   ,
	CTLCOLORMSGBOX =0x0132   ,
	CTLCOLOREDIT =0x0133   ,
	CTLCOLORLISTBOX =0x0134 ,  
	CTLCOLORBTN =0x0135   ,
	CTLCOLORDLG =0x0136   ,
	CTLCOLORSCROLLBAR =0x0137   ,
	CTLCOLORSTATIC =0x0138   ,
	VKEYTOITEM =0x002E   ,
	CHARTOITEM =0x002F   ,
	QUERYDRAGICON =0x0037 ,  
	DBNOTIFICATION =0x03FD ,  
	NETCONNECT =0x03FE   ,
	HIBERNATE =0x03FF   ,
		/// <summary>   
		/// This message is used by applications to help define private messages.   
		/// </summary> 
		USER =0x0400   ,
		APP =0x8000 ,
	}

}
