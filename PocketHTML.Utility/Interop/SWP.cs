//==========================================================================================
//
//		OpenNETCF.Win32.SWP
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
	/// Flags used with SetWindowPos.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	public enum SWP : int
	{
		NOSIZE          = 0x0001,
		NOMOVE          = 0x0002,
		NOZORDER        = 0x0004,
		NOREDRAW        = 0x0008,
		NOACTIVATE      = 0x0010,
		FRAMECHANGED    = 0x0020,
		SHOWWINDOW      = 0x0040,
		HIDEWINDOW      = 0x0080,
		NOCOPYBITS      = 0x0100,
		NOOWNERZORDER   = 0x0200, 
		NOSENDCHANGING  = 0x0400,
		DRAWFRAME       = FRAMECHANGED,
		NOREPOSITION    = NOOWNERZORDER,
		DEFERERASE      = 0x2000,
		ASYNCWINDOWPOS  = 0x4000
	}
}
