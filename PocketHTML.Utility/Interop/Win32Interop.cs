#region using directives
using System;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace ISquared.Win32Interop
{
	public class CoreDLL
	{
		#region P/Invokes
		[DllImport("coredll.dll",EntryPoint="SendMessage")]
		public static extern int SendMessageGetLine(IntPtr _WindowHandler, int
			_WM_USER, int _data, StringBuilder sb );

		[DllImport("coredll",EntryPoint="SendMessage")]
		public static extern int SendMessageString(IntPtr hwnd, int message,
			int data, String s);

		[DllImport("coredll",EntryPoint="SendMessage")]
		public static extern int SendMessage(IntPtr hwnd, int message, 
												int wparam, int lparam);

		[DllImport("coredll", EntryPoint = "SendMessage")]
		public static extern int SendMessage(IntPtr hwnd, int message,
												int wparam, int[] lparam);

		[DllImport("coredll")]
		public static extern IntPtr GetFocus();

        [DllImport("coredll")]
        public static extern IntPtr GetParent(IntPtr child);
		
		[DllImport("coredll")]
		public static extern int LoadIcon(int hInstance, int lpIconName); 

		[DllImport("coredll", EntryPoint="GetModuleHandleW", SetLastError=true)]
		public static extern IntPtr GetModuleHandle(string lpmodname);

		[DllImport("coredll")]
		public static extern int LoadImage(int hinst, string lpszName, 
			uint uType, int cxDesired, int cyDesired,  uint fuLoad );

		[DllImport("coredll")]
		public static extern IntPtr CreateFontIndirect(IntPtr pLF);

		[DllImport("coredll", EntryPoint="LocalAlloc", SetLastError=true)]
		public static extern IntPtr LocalAllocCE(int uFlags, int uBytes);

		[DllImport("coredll", EntryPoint="LocalFree", SetLastError=true)]
		public static extern IntPtr LocalFreeCE(IntPtr hMem);

		[DllImport("coredll")]
		public static extern int GetWindowLong(IntPtr hwnd, int nindex);

		[DllImport("coredll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int n); 

		[DllImport("coredll",SetLastError=true)]
		public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, 
												//System.Drawing.Point[] lpPoints, 
												IntPtr lppoints,
												int cPoints ); 

		[DllImport("coredll")]
		public static extern bool ClientToScreen(IntPtr hwnd, IntPtr p);

		[DllImport("coredll.dll")]
		public extern static bool SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter,
			Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);

        [DllImport("coredll.dll")]
        public extern static bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint); 

		[DllImport("coredll", EntryPoint="LoadLibraryW", SetLastError=true)]
		public static extern IntPtr LoadLibraryCE( string lpszLib );

		[DllImport("coredll", EntryPoint = "FormatMessageW", SetLastError = false)]
		public static extern int FormatMessage(FormatMessageFlags dwFlags, int lpSource, int dwMessageId, int dwLanguageId, out IntPtr lpBuffer, int nSize, int[] Arguments);


		[DllImport("CoreDll.DLL",
			 SetLastError = true)]
		public extern static int CreateProcess(String fileName, String cmdLine,
											IntPtr lpProcessAttributes,
											IntPtr lpThreadAttributes,
											Int32 boolInheritHandles,
											Int32 dwCreationFlags,
											IntPtr lpEnvironment,
											IntPtr lpszCurrentDir,
											byte[] si,
											ProcessInfo pi);


		[DllImport("coredll.dll", EntryPoint = "SHGetSpecialFolderPath", SetLastError = false)]
		public static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, StringBuilder lpszPath, int nFolder, int fCreate);

		[DllImport("coredll")]
		public static extern IntPtr GetCapture();


		#endregion

		#region Format Message Flags Enumeration
		/// <summary>
		/// Specifies aspects of the formatting process and how to interpret the lpSource parameter.
		/// </summary>
		/// <remarks>The low-order byte of dwFlags specifies how the function handles line breaks in the output buffer.
		/// The low-order byte can also specify the maximum width of a formatted output line.</remarks>
		[Flags]
			public enum FormatMessageFlags : int
		{
			/// <summary>
			/// The function allocates a buffer large enough to hold the formatted message, and places a pointer to the allocated buffer at the address specified by lpBuffer.
			/// </summary>
			AllocateBuffer = 0x00000100,
			/// <summary>
			/// Insert sequences in the message definition are to be ignored and passed through to the output buffer unchanged.
			/// </summary>
			IgnoreInserts  = 0x00000200,
			/// <summary>
			/// Specifies that lpSource is a pointer to a null-terminated message definition.
			/// </summary>
			FromString     = 0x00000400,
			/// <summary>
			/// Specifies that lpSource is a module handle containing the message-table resource(s) to search.
			/// </summary>
			FromHModule    = 0x00000800,
			/// <summary>
			/// Specifies that the function should search the system message-table resource(s) for the requested message.
			/// </summary>
			FromSystem     = 0x00001000,
			/// <summary>
			/// Specifies that the Arguments parameter is not a va_list structure, but instead is just a pointer to an array of 32-bit values that represent the arguments.
			/// </summary>
			ArgumentArray  = 0x00002000,
			/// <summary>
			/// Use the <b>MaxWidthMask</b> constant and bitwise Boolean operations to set and retrieve this maximum width value.
			/// </summary>
			MaxWidthMask  = 0x000000FF,
		}
		#endregion

		#region Utility functions
		public static IntPtr LoadLibrary(string library)
		{
			IntPtr ret = LoadLibraryCE(library);
			if (ret.ToInt32() <= 31 & ret.ToInt32() >= 0)
				throw new /*WinAPI*/Exception("Failed to load library " + library + ", code: " + Marshal.GetLastWin32Error());
			return ret;
		}

		public static IntPtr GetHandle(System.Windows.Forms.Control c)
		{
			c.Focus();
			IntPtr pControl = GetFocus();
			return pControl;
		}

		public static string SystemDirectory
		{
			get
			{
				try
				{
					return GetFolderPath(SpecialFolder.Windows);
				}
				catch
				{
					return "\\Windows";
				}
			}
		}


		public static string GetFolderPath(SpecialFolder folder)
		{
			StringBuilder path = new StringBuilder(260 + 2);

			if (!SHGetSpecialFolderPath(IntPtr.Zero, path, (int)folder, 0))
			{
				throw new Exception("Error retrieving folder path: " + Marshal.GetLastWin32Error());
			}

			return path.ToString();
		}
		#endregion

	}

	public class TGetFile
	{
		#region P/Invokes
		[DllImport("filedialogs.dll")]				
		public static extern bool tGetOpen1(int i1, int i2, int i3, IntPtr p1);

		[DllImport("filedialogs.dll")]				
		public static extern bool tGetSave1(int i1, int i2, int i3, IntPtr p1);
		#endregion

		#region TGetFileName
		public static string TGetFileName(bool save, string originalFileName, int initialFilter, 
											string fileFilter, string initialDirectory) 
		{
			OpenFileName ofn = new OpenFileName();

			ofn.structSize = 0;
			ofn.dlgOwner = IntPtr.Zero; 
			ofn.instance = IntPtr.Zero;
			ofn.customFilter = null;
			ofn.maxCustFilter = 0;
			ofn.filterIndex = 0;
			ofn.file = IntPtr.Zero;
			ofn.maxFile = 0;    
			ofn.fileTitle = IntPtr.Zero;
			ofn.maxFileTitle = 0;	
			ofn.initialDir = IntPtr.Zero;    
			ofn.title = IntPtr.Zero;       
			ofn.flags = 0; 
			ofn.fileOffset = 0;
			ofn.fileExtension = 0;    
			ofn.defExt = null;     
			ofn.custData = IntPtr.Zero;  
			ofn.hook = IntPtr.Zero;      
			ofn.templateName = null;     
			ofn.reservedPtr = IntPtr.Zero; 
			ofn.reservedInt = 0;
			ofn.flagsEx = 0;

			ofn.filter = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * fileFilter.Length);
			InteropUtility.StringToPointer(fileFilter, ofn.filter);
			
			ofn.filterIndex = initialFilter;
					

			int OFN_OVERWRITEPROMPT = 0x02;
			ofn.flags |= OFN_OVERWRITEPROMPT;		
			ofn.file = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * 256);						
			ofn.maxFile = 256;		
			ofn.fileTitle = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * 64);
			ofn.maxFileTitle = 64;				

			if(initialDirectory == null)
			{
				initialDirectory = "\\";
			}

			ofn.initialDir = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * initialDirectory.Length);
			InteropUtility.StringToPointer(initialDirectory, ofn.initialDir);

			if(save && originalFileName != null)
			{
				InteropUtility.StringToPointer(originalFileName, ofn.file);
			}

			ofn.structSize = Marshal.SizeOf( ofn );
			
			IntPtr pOFN = CoreDLL.LocalAllocCE(0x40, 512);
			Marshal.StructureToPtr(ofn, pOFN, false);
		
			bool res;
			
			// 177 is the magic registration number needed to keep the 
			// trial message from popping up.
			if(save)
			{
				res = TGetFile.tGetSave1(0, 0, 177, pOFN );
			}
			else
			{
				res = TGetFile.tGetOpen1(0, 0, 177, pOFN);
			}

			string result = String.Empty;
			if(res)
			{
				result = InteropUtility.PointerToString(ofn.file);
				int slashidx = result.IndexOf("\\");
				result = result.Substring(slashidx);

				OpenFileName ofn2 = (OpenFileName)Marshal.PtrToStructure(pOFN, typeof(OpenFileName));
				if(save && (ofn2.filterIndex == 1))
				{
					int idx = result.LastIndexOf(".htm");
					if(idx == -1)
					{
						result += ".html";
					}
				}				
			}
			
			CoreDLL.LocalFreeCE(ofn.file);
			CoreDLL.LocalFreeCE(ofn.fileTitle);
			CoreDLL.LocalFreeCE(ofn.title);
			CoreDLL.LocalFreeCE(ofn.filter);
			CoreDLL.LocalFreeCE(ofn.initialDir);
			CoreDLL.LocalFreeCE(pOFN);

			return result;
		}
		#endregion

	}

	#region Enums and structs
	public enum SpecialFolder
	{
		// <summary>
		// Windows CE.NET 4.2 and above
		// </summary>
		//VirtualRoot		= 0x00,
		/// <summary>
		/// The directory that contains the user's program groups.
		/// </summary>
		Programs = 0x02,
		// <summary>
		// control panel icons
		// </summary>
		//Controls		= 0x03,
		// <summary>
		// printers folder
		// </summary>
		//Printers		= 0x04,
		/// <summary>
		/// The directory that serves as a common repository for documents. (Not supported in Pocket PC and Pocket PC 2002 - "\My Documents").
		/// </summary>
		Personal = 0x05,
		/// <summary>
		/// The directory that serves as a common repository for the user's favorite items.
		/// </summary>
		Favorites = 0x06,
		/// <summary>
		/// The directory that corresponds to the user's Startup program group.   The system starts these programs whenever a user starts Windows CE.
		/// </summary>
		Startup = 0x07,
		/// <summary>
		/// The directory that contains the user's most recently used documents.
		/// Not supported on Windows Mobile.
		/// </summary>
		Recent = 0x08,
		// <summary>
		// The directory that contains the Send To menu items.
		// </summary>
		//SendTo			= 0x09,
		// <summary>
		// Recycle bin.
		// </summary>
		//RecycleBin		= 0x0A,
		/// <summary>
		/// The directory that contains the Start menu items.
		/// </summary>
		StartMenu = 0x0B,
		/// <summary>
		/// The directory used to physically store file objects on the desktop.   Do not confuse this directory with the desktop folder itself, which is a virtual folder.
		/// Not supported on Windows Mobile.
		/// </summary>
		DesktopDirectory = 0x10,
		// <summary>
		// The "My Computer" folder.
		// </summary>
		//MyComputer		= 0x11,
		// <summary>
		// Network Neighbourhood
		// </summary>
		//NetNeighborhood = 0x12,
		/// <summary>
		/// The Fonts folder.
		/// </summary>
		Fonts = 0x14,
		/// <summary>
		/// The directory that serves as a common repository for application-specific data for the current user.
		/// Not supported on Windows Mobile.
		/// </summary>
		ApplicationData = 0x1a,
		/// <summary>
		/// The Windows folder.
		/// Not supported by Pocket PC and Pocket PC 2002.
		/// </summary>
		Windows = 0x24,
		/// <summary>
		/// The program files directory.
		/// </summary>
		ProgramFiles = 0x26,

	}

	public class ProcessInfo
	{
		public int hProcess = 0;
		public int hThread = 0;
		public int ProcessID = 0;
		public int ThreadID = 0;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OpenFileName
	{
		public int structSize;
		public IntPtr dlgOwner;
		public IntPtr instance;
		public IntPtr filter;
		public String customFilter;
		public int maxCustFilter;
		public int filterIndex;
		public IntPtr file;
		public int maxFile;
		public IntPtr fileTitle;
		public int maxFileTitle;
		public IntPtr initialDir;
		public IntPtr title;
		public int flags;
		public short fileOffset;
		public short fileExtension;
		public String defExt;
		public IntPtr custData;
		public IntPtr hook;
		public String templateName;
		public IntPtr reservedPtr;
		public int reservedInt;
		public int flagsEx;
	}
	#endregion

	public class InteropUtility
	{
		#region String functions
		public unsafe static string PointerToString(IntPtr ptr)   
		{   
			int i;   
			StringBuilder sb = new StringBuilder();   
			char *p = (char*)ptr.ToPointer();   
    
			for (i = 0 ; p[i] != '\0' ; i++)   
			{   
				sb.Append(p[i]);   
			}   
    
			return sb.ToString();   
		}

		public unsafe static void StringToPointer(string src, IntPtr dest)   
		{   
			int i;   
    
			char *p = (char *)dest.ToPointer();   
    
			for(i = 0 ; i < src.Length ; i++)   
			{   
				p[i] = src[i];   
			}   
			p[i] = '\0';   
		}

		/*
		public static void InsertString(string st, IntPtr ip)
		{
			ip = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * st.Length);
			StringToPointer(st, ip);
		}
		*/
		#endregion

		#region Other functions
		public static void SetTabStop(System.Windows.Forms.TextBox textBox)
		{
			IntPtr pTB = CoreDLL.GetHandle(textBox);
			int EM_SETTABSTOPS = 0x00CB;
			int[] stops = { 16 };
			int result = CoreDLL.SendMessage(pTB, EM_SETTABSTOPS, 1, stops);
		}
		#endregion
	}
}

 