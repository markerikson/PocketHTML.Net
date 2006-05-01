//==========================================================================================
//
//		OpenNETCF.Win32.GW
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
	/// Window relationships used with GetWindow.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum GW : int
	{
		/// <summary>
		/// 
		/// </summary>
		HWNDFIRST  =      0,
		/// <summary>
		/// 
		/// </summary>
		HWNDLAST   =      1,
		/// <summary>
		/// 
		/// </summary>
		HWNDNEXT   =      2,
		/// <summary>
		/// 
		/// </summary>
		HWNDPREV   =      3,
		/// <summary>
		/// 
		/// </summary>
		OWNER      =      4,
		/// <summary>
		/// 
		/// </summary>
		CHILD      =      5,
	}
}
