//==========================================================================================
//
//		OpenNETCF.Win32.EM
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
	/// Edit control Messages.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum EM: int
	{
		GETSEL              = 0x00B0,
		SETSEL              = 0x00B1,
		GETRECT             = 0x00B2,
		SETRECT             = 0x00B3,
		SETRECTNP           = 0x00B4,
		SCROLL              = 0x00B5,
		LINESCROLL          = 0x00B6,
		SCROLLCARET         = 0x00B7,
		GETMODIFY           = 0x00B8,
		SETMODIFY           = 0x00B9,
		GETLINECOUNT        = 0x00BA,
		LINEINDEX           = 0x00BB,
		LINELENGTH          = 0x00C1,
		REPLACESEL          = 0x00C2,
		GETLINE             = 0x00C4,
		LIMITTEXT           = 0x00C5,
		CANUNDO             = 0x00C6,
		UNDO                = 0x00C7,
		FMTLINES            = 0x00C8,
		LINEFROMCHAR        = 0x00C9,
		SETTABSTOPS         = 0x00CB,
		SETPASSWORDCHAR     = 0x00CC,
		EMPTYUNDOBUFFER     = 0x00CD,
		GETFIRSTVISIBLELINE = 0x00CE,
		SETREADONLY         = 0x00CF,
		GETPASSWORDCHAR     = 0x00D2,
		SETMARGINS          = 0x00D3,
		GETMARGINS          = 0x00D4,
		SETLIMITTEXT        = LIMITTEXT,
		GETLIMITTEXT        = 0x00D5,
		POSFROMCHAR         = 0x00D6,
		CHARFROMPOS         = 0x00D7
	}
}
