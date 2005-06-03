using System;
using System.Runtime.InteropServices;
using System.Text;


namespace ISquared.Win32Interop
{
	class CoreDLL
	{
		[DllImport("coredll.dll",EntryPoint="SendMessage")]
		internal static extern int SendMessageGetLine(IntPtr _WindowHandler, int
			_WM_USER, int _data, StringBuilder sb );

		[DllImport("coredll",EntryPoint="SendMessage")]
		internal static extern int SendMessageString(IntPtr hwnd, int message,
			int data, String s);

		[DllImport("coredll",EntryPoint="SendMessage")]
		internal static extern int SendMessage(IntPtr hwnd, int message, 
												int wparam, int lparam);

		[DllImport("coredll", EntryPoint = "SendMessage")]
		internal static extern int SendMessage(IntPtr hwnd, int message,
												int wparam, int[] lparam);

		[DllImport("coredll")]
		internal static extern IntPtr GetFocus();
		
		[DllImport("coredll")]
		internal static extern int LoadIcon(int hInstance, int lpIconName); 

		[DllImport("coredll", EntryPoint="GetModuleHandleW", SetLastError=true)]
		internal static extern IntPtr GetModuleHandle(string lpmodname);

		[DllImport("coredll")]
		internal static extern int LoadImage(int hinst, string lpszName, 
			uint uType, int cxDesired, int cyDesired,  uint fuLoad );

		[DllImport("coredll")]
		internal static extern IntPtr CreateFontIndirect(IntPtr pLF);

		[DllImport("coredll", EntryPoint="LocalAlloc", SetLastError=true)]
		internal static extern IntPtr LocalAllocCE(int uFlags, int uBytes);

		[DllImport("coredll", EntryPoint="LocalFree", SetLastError=true)]
		internal static extern IntPtr LocalFreeCE(IntPtr hMem);

		[DllImport("coredll")]
		internal static extern int GetWindowLong(IntPtr hwnd, int nindex);

		[DllImport("coredll")]
		internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int n); 

		[DllImport("coredll",SetLastError=true)]
		internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, 
												//System.Drawing.Point[] lpPoints, 
												IntPtr lppoints,
												int cPoints ); 

		[DllImport("coredll")]
		internal static extern bool ClientToScreen(IntPtr hwnd, IntPtr p);

		[DllImport("coredll.dll")]
		internal extern static bool SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter,
			Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);

		[DllImport("coredll", EntryPoint="LoadLibraryW", SetLastError=true)]
		internal static extern IntPtr LoadLibraryCE( string lpszLib );

		public static IntPtr LoadLibrary(string library)
		{
			IntPtr ret = LoadLibraryCE(library);
			if ( ret.ToInt32() <= 31 & ret.ToInt32() >= 0)
				throw new /*WinAPI*/Exception("Failed to load library " + library + ", code: " + Marshal.GetLastWin32Error());
			return ret;
		}


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

		[DllImport("coredll", EntryPoint="FormatMessageW", SetLastError=false)]
		internal static extern int FormatMessage(FormatMessageFlags dwFlags, int lpSource, int dwMessageId, int dwLanguageId, out IntPtr lpBuffer, int nSize, int[] Arguments );


		[DllImport("CoreDll.DLL", 
			 SetLastError=true)]
		internal extern static int CreateProcess( String fileName, String cmdLine,
											IntPtr lpProcessAttributes,
											IntPtr lpThreadAttributes,
											Int32 boolInheritHandles,
											Int32 dwCreationFlags,
											IntPtr lpEnvironment,
											IntPtr lpszCurrentDir,
											byte [] si,
											ProcessInfo pi );

		internal static IntPtr GetHandle(System.Windows.Forms.Control c)
		{
			c.Focus();
			IntPtr pControl = GetFocus();
			return pControl;
		}
	}

	public class TGetFile
	{
		[DllImport("filedialogs.dll")]				
		internal static extern bool tGetOpen1(int i1, int i2, int i3, IntPtr p1);

		[DllImport("filedialogs.dll")]				
		internal static extern bool tGetSave1(int i1, int i2, int i3, IntPtr p1);

		public static string TGetFileName(bool save, string fileFilter, string initialDirectory) 
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


			//string html = "HTML Files (*.html, *.htm)\0*.html;*.htm\0Text files (*.txt)\0*.txt\0All files (*.*)\0*.*\0\0";
			
			ofn.filter = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * fileFilter.Length);
			Win32Interop.Utility.StringToPointer(fileFilter, ofn.filter);

			ofn.filterIndex = 0;

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
			//Win32Interop.Utility.InsertString(initialDirectory, ofn.initialDir);

			ofn.initialDir = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * initialDirectory.Length);
			Utility.StringToPointer(initialDirectory, ofn.initialDir);

			/*
						string title = String.Empty;
						ofn.title = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * title.Length);
						StringToPointer(title, ofn.title);

						if(saveas)
						{
							title = "Open"

						}
						*/

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
				result = Win32Interop.Utility.PointerToString(ofn.file);
				int slashidx = result.IndexOf("\\");
				result = result.Substring(slashidx);

				OpenFileName ofn2 = (OpenFileName)Marshal.PtrToStructure(pOFN, typeof(OpenFileName));
				if(save && (ofn2.filterIndex == 1))
				{
					//int idx = result.LastIndexOf(".");
					//string justname = result.Substring(0, idx);
					//result = justname + ".html";
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
	}

	internal class ProcessInfo
	{
		public int hProcess = 0;
		public int hThread = 0;
		public int ProcessID = 0;
		public int ThreadID = 0;
	}

	public class Utility
	{
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

		public static void InsertString(string st, IntPtr ip)
		{
			ip = CoreDLL.LocalAllocCE(0x40, Marshal.SystemDefaultCharSize * st.Length);
			StringToPointer(st, ip);
		}

		public static void SetTabStop(System.Windows.Forms.TextBox textBox)
		{
			IntPtr pTB = CoreDLL.GetHandle(textBox);
			int EM_SETTABSTOPS = 0x00CB;
			int[] stops = { 16 };
			int result = CoreDLL.SendMessage(pTB, EM_SETTABSTOPS, 1, stops);
		}
	}

	[StructLayout(LayoutKind.Sequential)]  
	public struct OpenFileName 
	{
		public int		structSize;
		public IntPtr	dlgOwner;
		public IntPtr	instance;
		public IntPtr filter;
		public String	customFilter;
		public int		maxCustFilter;
		public int		filterIndex;
		public IntPtr file;
		public int		maxFile;
		public IntPtr fileTitle;
		public int		maxFileTitle;
		public IntPtr initialDir;
		public IntPtr title;
		public int		flags;
		public short	fileOffset;
		public short	fileExtension;
		public String	defExt;
		public IntPtr	custData;
		public IntPtr	hook;
		public String	templateName;
		public IntPtr	reservedPtr;
		public int		reservedInt;
		public int		flagsEx ;
	}



	[StructLayout(LayoutKind.Sequential)]
	internal struct LOGFONT 
	{ 
		public int lfHeight; 
		public int lfWidth; 
		public int lfEscapement; 
		public int lfOrientation; 
		public int lfWeight; 
		public byte lfItalic; 
		public byte lfUnderline; 
		public byte lfStrikeOut; 
		public byte lfCharSet; 
		public byte lfOutPrecision ; 
		public byte lfClipPrecision ; 
		public byte lfQuality ; 
		public byte lfPitchAndFamily ; 
		public string lfFaceName; 
	} 


 

}

 