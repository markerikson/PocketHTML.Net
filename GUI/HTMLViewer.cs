//==========================================================================================
//
//		OpenNETCF.Windows.Forms.HTMLViewer
//		Copyright (c) 2004, OpenNETCF.org
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

#region using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OpenNETCF.Runtime.InteropServices;
using ISquared.Win32Interop;
using System.IO;

#if !DESIGN

using OpenNETCF.Win32;
using OpenNETCF.Windows.Forms; 
#if !NDOC
using Microsoft.WindowsCE.Forms;
#endif
#endif
#endregion

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Displays HTML Content.
	/// </summary>
#if DESIGN
	[System.ComponentModel.DefaultEvent("HotSpotClick"),
	System.ComponentModel.DefaultProperty("Url")]
	public class HTMLViewer : Control
#else
	public class HTMLViewer : Control, IWin32Window, INotifiable
#endif
	{
		#region Fields
		//the handle of this control
		private IntPtr m_handle = IntPtr.Zero;

		//do a border?
		private BorderStyle m_border = BorderStyle.FixedSingle;

		//current settings
		private string m_url = "";
		private string m_source = "";
		private string m_title = "";
		private bool m_cleartype = false;
		private bool m_contextmenu = true;
		private bool m_shrink = true;
		private bool m_script = true;

		//private string m_filename;
		private WindowSink m_sink;
		private IntPtr m_html;

		#endregion

		#region Constructor
		public HTMLViewer()
		{
			m_sink = new WindowSink();
		}
		

		public void CreateHTMLControl()
		{
			m_handle = CoreDLL.GetHandle(this);

			m_html = CreateChildHTMLControl(m_handle, m_sink.Hwnd);
		}

		#endregion

		#region Properties

		#region BorderStyle
		/// <summary>
		/// Gets or sets the border style for the control.
		/// </summary>
		/// <value>One of the <see cref="BorderStyle"/> values.
		/// Fixed3D is interpreted the same as FixedSingle.</value>
#if DESIGN
		[System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The style of the border.")]
#endif
		public BorderStyle BorderStyle
		{
			get
			{
				return m_border;
			}
			set
			{
				m_border = value;
				Invalidate();
#if !DESIGN				
				//m_msgwnd.BorderStyle = value;
#endif
			}
		}
		#endregion

		#region EnableContextMenu Property
		/// <summary>
		/// Gets or sets a value which determines whether a ContextMenu is available for this control
		/// </summary>
		/// <remarks>Setting this property to true will allow the default context menu to display.
		/// This control does not currently support a custom context menu.</remarks>
#if DESIGN
		[Category("Behavior"),
		Description("Show the default context menu."),
		DefaultValue(true)]
#endif
		public bool EnableContextMenu
		{
			get
			{
				return m_contextmenu;
			}
			set
			{
				m_contextmenu = value;
#if !DESIGN
				//send to control
				int intval = 0;
				if(value)
					intval = -1;

				CoreDLL.SendMessage(m_html, (int)HtmlMessage.EnableContextMenu, 0, intval);
#endif
			}
		}
		#endregion

		
		#region Handle Property
#if !DESIGN
		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		/// <value>An <see cref="IntPtr"/> that contains the window handle (HWND) of the control.</value>
		public IntPtr Handle
		{
			get
			{
				return m_html;
			}
		}

#endif
		#endregion
		

		#region LayoutHeight Property
		/// <summary>
		/// Gets the height in pixels of the content in the Html Viewer.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public int LayoutHeight
		{
			get
			{
				#if !DESIGN
				return CoreDLL.SendMessage(m_html, (int)HtmlMessage.LayoutHeight, 0, 0);
				#else
				return 0;
				#endif
			}
		}
		#endregion

		#region LayoutWidth Property
		/// <summary>
		/// Gets the width in pixels of the content in the Html Viewer.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public int LayoutWidth
		{
			get
			{
#if !DESIGN
				return CoreDLL.SendMessage(m_html, (int)HtmlMessage.LayoutWidth, 0, 0);
#else
				return 0;
#endif
			}
		}
		#endregion

		#region Url Property
		/// <summary>
		/// Gets or sets the Url used to retrieve a html document
		/// </summary>
#if DESIGN
		[Category("Behavior"),
		Description("The URL of the page to display.")]
#endif
		public string Url
		{
			get
			{
				return m_url;
			}
			set
			{
				m_url = value;

#if !DESIGN
				//allocate temporary native buffer
				IntPtr stringptr = MarshalEx.StringToHGlobalUni(value + '\0');
				
				//send message to native control
				CoreDLL.SendMessage(m_html, (int)HtmlMessage.Navigate, 0, (int)stringptr);

				//free native memory
				MarshalEx.FreeHGlobal(stringptr);
#endif
				
			}
		}
		#endregion

		#region Selected Text Property
		/*public string SelectedText
		{
			get
			{
			}
		}*/
		#endregion

		#region Source Property
		/// <summary>
		/// Gets or Sets the HTML text value contained by the control.
		/// </summary>
#if DESIGN
		[Category("Behavior"),
		Description("Raw HTML to display")]
#endif
		public string Source
		{
			get
			{
				return m_source;
			}
			set
			{
				m_source = value;

#if !DESIGN
				//add text
				AddText(value);
				//mark as end of source
				EndOfSource();
#endif
			}
		}
		#endregion

		#region Title Property
		/// <summary>
		/// Gets the Title of the current document (if available).
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public string Title
		{
			get
			{
				return m_title;
			}
		}
		#endregion

		#region ZoomLevel property
		public int ZoomLevel
		{
			set
			{
				int zoom = value;
				if (zoom > 4)
				{
					zoom = 4;
				}
				if (zoom < 0)
				{
					zoom = 0;
				}

				Win32Window.SendMessage(this.Handle, (int)HtmlMessage.ZoomLevel, 0, zoom);
			}
		}

		public string CurrentFilename
		{
			get
			{
				return m_sink.CurrentFilename;
			}
			set
			{
				m_sink.CurrentFilename = value;
			}
		}
		#endregion

		#region EnableClearType Property
		/// <summary>
		/// Gets or sets a value indicating whether the control renders with ClearType text
		/// </summary>
		/// <remarks>By default ClearType is off.</remarks>
#if DESIGN
		[Category("Appearance"),
		Description("Indicates whether to render the page with Cleartype smoothing."),
		DefaultValue(false)]
#endif
		public bool EnableClearType
		{
			get
			{
				return m_cleartype;
			}
			set
			{
				m_cleartype = value;

#if !DESIGN
				int intval = 0;
				if(value)
					intval = -1;
				
				CoreDLL.SendMessage(m_html, (int)HtmlMessage.EnableClearType, 0, intval);
#endif
			}
		}
		#endregion

		#region ShrinkMode Property
		/// <summary>
		/// Gets or sets a value indicating whether the control renders with Shrink mode.
		/// </summary>
		/// <remarks>If shrink mode is enabled, the HTML control will shrink images as required to make the page fit the window.
		/// By default ShrinkMode is on.</remarks>
#if DESIGN
		[Category("Behaviour"),
		Description("Indicates whether to try to fit the page to the screen."),
		DefaultValue(true)]
#endif
		public bool ShrinkMode
		{
			get
			{
				return m_shrink;
			}
			set
			{
				m_shrink = value;

#if !DESIGN
				int intval = 0;
				if(value)
					intval = -1;
				
				CoreDLL.SendMessage(m_html, (int)HtmlMessage.EnableShrink, 0, intval);
#endif
			}
		}
		#endregion

		#region Scripting Property
		/// <summary>
		/// Gets or sets a value indicating whether the control supports Scripting.
		/// </summary>
		/// <remarks>By default Scripting is on.</remarks>
#if DESIGN
		[Category("Behaviour"),
		Description("Indicates whether to enable or disable J/Script support."),
		DefaultValue(true)]
#endif
		public bool Scripting
		{
			get
			{
				return m_script;
			}
			set
			{
				m_script = value;

#if !DESIGN
				int intval = 0;
				if(value)
					intval = -1;
				
				CoreDLL.SendMessage(m_html, (int)HtmlMessage.EnableScripting, 0, intval);
#endif
			}
		}
		#endregion

		#endregion
				

		#region Public Methods

		#region AddText Method
		/// <summary>
		/// Adds HTML text to the HTMLViewer control.
		/// </summary>
		/// <param name="text">HTML text to be added.</param>
		public void AddText(string text)
		{
			AddText(text, false);
		}
		/// <summary>
		/// Adds text to the HTMLViewer control.
		/// </summary>
		/// <param name="text">Text to be added.</param>
		/// <param name="plain">If TRUE text is treated as plain text, else treated as HTML Source.</param>
		public void AddText(string text, bool plain)
		{
#if !DESIGN
			//allocate temporary native buffer
			IntPtr stringptr = MarshalEx.StringToHGlobalUni(text + '\0');
				
			int iplain = 0;

			if(plain)
				iplain = -1;

			//send message to native control adding to current text
			CoreDLL.SendMessage(m_html, (int)HtmlMessage.AddTextW, iplain, (int)stringptr);

			//free native memory
			MarshalEx.FreeHGlobal(stringptr);
#endif
		}
		#endregion

		#region EndOfSource Method
		/// <summary>
		/// Inform the HTMLViewer that the current document is complete.
		/// </summary>
		public void EndOfSource()
		{
#if !DESIGN
			//send message to native control indicating end of source text
			CoreDLL.SendMessage(m_html, (int)HtmlMessage.EndOfSource, 0, 0);
#endif
		}
		#endregion

		#region Clear Method
		/// <summary>
		/// Clears the contents of the Html Viewer.
		/// </summary>
		public void Clear()
		{
			//clear local text
			this.Text = "";
#if !DESIGN
			CoreDLL.SendMessage(m_html, (int)HtmlMessage.Clear, 0, 0);
#endif
		}
		#endregion

		#region SelectAll Method
		/// <summary>
		/// Selects all the text in the current HTML page.
		/// </summary>
		public void SelectAll()
		{
#if !DESIGN
			//send message
			CoreDLL.SendMessage(m_html, (int)HtmlMessage.SelectAll, 0, 0);
#endif
		}

		#endregion

		#endregion

		#region Events

		#region HotSpotClick
		/// <summary>
		/// Occurs when the user activates a hyperlink within the current document.
		/// </summary>
		public event HotSpotClickEventHandler HotSpotClick;

		/// <summary>
		/// Raises the HotSpotClick event.
		/// </summary>
		/// <param name="e">HotSpotClickEventArgs containing the Url of the requested page</param>
		internal void OnHotSpotClick(HotSpotClickEventArgs e)
		{
			if(this.HotSpotClick!=null)
			{
				this.HotSpotClick(this, e);
			}
		}

		/// <summary>
		/// Represents the method that will handle the HotSpotClick event.
		/// </summary>
		public delegate void HotSpotClickEventHandler(object sender, HotSpotClickEventArgs e);
		
		#endregion

		#region NavigateComplete
		/// <summary>
		/// Occurs when a page navigation request is completed.
		/// </summary>
		public event EventHandler NavigateComplete;

		/// <summary>
		/// Raises the NavigateComplete event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnNavigateComplete(EventArgs e)
		{
			if(this.NavigateComplete!=null)
			{
				this.NavigateComplete(this, e);
			}
		}
		#endregion

		#region DocumentComplete
		/// <summary>
		/// Occurs when document loading is completed.
		/// </summary>
		public event EventHandler DocumentComplete;

		/// <summary>
		/// Raises the DocumentComplete event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnDocumentComplete(EventArgs e)
		{
			if(this.DocumentComplete!=null)
			{
				this.DocumentComplete(this, e);
			}
		}
		#endregion


		protected override void OnGotFocus(EventArgs e)
		{
#if !DESIGN
			//get handle to internal webview control
			IntPtr webviewhandle = Win32Window.GetWindow(m_html, GW.CHILD);
			//set focus to webview control
			Win32Window.SetFocus(webviewhandle);
#endif
			base.OnGotFocus (e);
		}

		#endregion
		
		#region Designer Support
#if DESIGN
		protected override void OnPaint(PaintEventArgs e)
		{
			//draw white rectange with black border
			e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, this.Width, this.Height);
			
			if(m_border!=BorderStyle.None)
			{
				e.Graphics.DrawRectangle(new Pen(Color.Black), 0, 0, this.Width-1, this.Height-1);
			}

			base.OnPaint (e);
		}

		protected override void OnResize(EventArgs e)
		{
			Invalidate();
			base.OnResize(e);
		}
#endif
		#endregion

		#region HTML Control Functions

#if !DESIGN
		[DllImport("htmlview.dll", EntryPoint="InitHTMLControl", SetLastError=true)] 
		private static extern int InitHTMLControl(IntPtr hinst);

		[DllImport("HTMLContainer.dll", EntryPoint = "CreateHTMLControl")]
		private static extern IntPtr CreateChildHTMLControl(IntPtr hwndParent, IntPtr hwndMessageWindow);

        public void ResizeHTMLControl()
        {
            IntPtr pHTMLContainer = CoreDLL.GetParent(m_html);

            CoreDLL.MoveWindow(pHTMLContainer, 0, 0, this.Width, this.Height, true);
            CoreDLL.MoveWindow(m_html, 0, 0, this.Width, this.Height, true);
        }
#endif
		#endregion

		#region HtmlMessage Enumeration

		private enum HtmlMessage : int
		{
			/// <summary>
			/// Base message
			/// </summary>
			WM_USER = 1024,
			
			/// <summary>
			/// Required 
			/// </summary>
			WM_SETTEXT = 0x000C,
			
			WM_GETTEXT = 0x000D,
			WM_GETTEXTLENGTH = 0x000E,
			/// <summary>
			/// Sent by an application to the HTML viewer control to request that default stylesheet rules be applied to the current HTML window. 
			/// </summary>
			AddStyle = (WM_USER + 126),

			/// <summary>
			/// Sent by an application to add text to the HTML Control. 
			/// </summary>
			AddText = (WM_USER + 101),

			/// <summary>
			/// Sent by an application to add Unicode text to the HTML control. 
			/// </summary>
			AddTextW = (WM_USER + 102),

			/// <summary>
			/// Sent by an application to tell the HTML control to jump to the indicated anchor. 
			/// </summary>
			Anchor = (WM_USER + 105),

			/// <summary>
			/// Sent by an application to tell the HTML control to jump to the indicated anchor. 
			/// </summary>
			AnchorW = (WM_USER + 106),
		
			/// <summary>
			///  Sent by an application to the HTML viewer control to request a reference to its IDispatch interface. 	
			/// </summary>
			BrowserDispatch = (WM_USER + 124),

			/// <summary>
			///  Sent by an application to clear the contents of the HTML control. 
			/// </summary>
			Clear = (WM_USER + 113),
		
			/// <summary>
			///  Sent by an application to enable or disable ClearType for HTML text rendering. 
			/// </summary>
			EnableClearType = (WM_USER + 114),

			/// <summary>
			///  Sent by an application to enable or disable the context-sensitive menu for the HTML control. 
			///  </summary>
			EnableContextMenu = (WM_USER + 110),

			/// <summary>
			///  Sent by an application to enable or disable scripting for the HTML control. 
			/// </summary>
			EnableScripting = (WM_USER + 115),
		
			/// <summary>
			///  Sent by an application to command the HTML control to toggle the image shrink enable mode, or shrink mode. 
			/// </summary>
			EnableShrink = (WM_USER + 107),
		
			/// <summary>
			///  Sent by an application to inform the HTML control that the document is complete. 
			/// </summary>
			EndOfSource = (WM_USER + 104),

			/// <summary>
			/// Sent by an application to inform the HTML control that the image indicated by the cookie could not be loaded. 
			/// </summary>
			ImageFail = (WM_USER + 109),
		
			/// <summary>
			///  Sent by an application to determine if some text was selected in the HTML control. 
			/// </summary>
			IsSelection = (WM_USER + 112),
	
			/// <summary>
			///  Sent by an application to determine the layout height (in pixels) of the content in the HTML control. 
			/// </summary>
			LayoutHeight = (WM_USER + 118),

			/// <summary>
			///  Sent by an application to determine the layout width (in pixels) of the content in the HTML control. 
			/// </summary>
			LayoutWidth = (WM_USER + 117),
		
			/// <summary>
			///  Sent by an application to navigate to a particular URL in the HTML control. 
			/// </summary>
			Navigate = (WM_USER + 120),
		
			/// <summary>
			///  Sent by an application to tell an HTML viewer control to select all the text in the current HTML page. 
			/// </summary>
			SelectAll = (WM_USER + 111),
		
			/// <summary>
			///  Sent by an application to associate a bitmap with an image. 
			/// </summary>
			SetImage = (WM_USER + 103),

			/// <summary>
			///  Sent by an application to STOP a navigation started in the HTML control. 
			/// </summary>
			Stop = (WM_USER + 125),

			/// <summary>
			/// Sent by an application to specify the zoom level (0-4). 
			/// </summary>
			ZoomLevel = (WM_USER + 116),
		}
		#endregion

		#region HtmlNotification Enumeration
		/// <summary>
		/// Notifications received from the native control
		/// </summary>
		private enum HtmlNotification : int
		{
			WM_USER = 1024,
			/// <summary>
			/// Sent by the HTML viewer control if the user taps on a link or submits a form.
			/// </summary>
			Hotspot =             (WM_USER + 101),
		
			/// <summary>
			/// Sent by the HTML viewer control to notify the application to load the image.
			/// </summary>
			InlineImage =        (WM_USER + 102),
		
			/// <summary>
			/// Sent by the HTML viewer control to notify the application to load the sound.
			/// </summary>
			InlineSound =        (WM_USER + 103),

			/// <summary>
			/// Sent by the HTML viewer control to notify the application and provide the title of the HTML document.
			/// </summary>
			Title =               (WM_USER + 104),

			/// <summary>
			/// Sent by the HTML viewer control to notify the application and provide the HTTP-EQUIV and CONTENT parameters of the META tag in the HTML document.
			/// </summary>
			Meta =                (WM_USER + 105),

			/// <summary>
			/// Sent by the HTML viewer control when it encounters a BASE tag in an HTML document.
			/// </summary>
			Base =                (WM_USER + 106),

			/// <summary>
			/// Sent by the HTML viewer control when a user holds the stylus down in the viewer window before a context menu appears.
			/// </summary>
			ContextMenu =         (WM_USER + 107),

			/// <summary>
			/// Sent by the HTML viewer control to notify the application that XML was loaded.
			/// </summary>
			InlineXml =          (WM_USER + 108),

			/// <summary>
			/// Sent by the HTML viewer control before a navigation request to a URL begins.
			/// </summary>
			BeforeNavigate =      (WM_USER + 109),

			/// <summary>
			/// Sent by the HTML viewer control when the document navigation is complete.
			/// </summary>
			DocumentComplete =    (WM_USER + 110),

			/// <summary>
			/// Sent by the HTML viewer control when a navigation request has completed.
			/// </summary>
			NavigateComplete =    (WM_USER + 111),
	
			/// <summary>
			/// Sent by the HTML viewer control when the document title changes.
			/// </summary>
			TitleChange =         (WM_USER + 112),

			/// <summary>
			/// *DOCUMENTATION UNAVAILABLE*
			/// </summary>
			InlineStyle =        (WM_USER + 113),
		}
		#endregion

#if !DESIGN
		#region NM_HTMLVIEW
		//define your control specific notification message header (or use NMHDR from OpenNETCF.Win32.Win32Window)
		private struct NM_HTMLVIEW
		{
			public IntPtr hwndFrom; 
			public int idFrom; 
			public int code; 
			public IntPtr szTarget;
			public IntPtr szData;
			public IntPtr dwCookie;
			public IntPtr szExInfo;
		}
		#endregion

		#region INotifiable Members

		/// <summary>
		/// Implementation of the INotifiable Notify method.
		/// </summary>
		/// <remarks>Receives notification messages from the native HTMLViewer control.</remarks>
		/// <param name="notifydata">Pointer to a Notification struct.</param>
		void INotifiable.Notify(IntPtr notifydata)
		{
			//marshal data to custom nmhtml struct
			//Marshal.PtrToStructure doesn't work so marshalling items individually
			NM_HTMLVIEW nmhtml = new NM_HTMLVIEW();
			nmhtml.hwndFrom = (IntPtr)Marshal.ReadInt32(notifydata, 0);
			nmhtml.idFrom = Marshal.ReadInt32(notifydata, 4);
			nmhtml.code = Marshal.ReadInt32(notifydata, 8);
			nmhtml.szTarget = (IntPtr)Marshal.ReadInt32(notifydata, 12);
			nmhtml.szData = (IntPtr)Marshal.ReadInt32(notifydata, 16);
			nmhtml.dwCookie = (IntPtr)Marshal.ReadInt32(notifydata, 20);
			nmhtml.szExInfo = (IntPtr)Marshal.ReadInt32(notifydata, 24);

			//check the incoming message code and process as required
			switch(nmhtml.code)
			{
					//hotspotclick
				case (int)HtmlNotification.Hotspot:
					//use the new PtrToStringAuto to get string whether Unicode or ASCII
					string url = OpenNETCF.Runtime.InteropServices.MarshalEx.PtrToStringAuto(nmhtml.szTarget);

					HotSpotClickEventArgs e = new HotSpotClickEventArgs(url);
					OnHotSpotClick(e);
					break;
					//navigate complete
				case (int)HtmlNotification.NavigateComplete:
					OnNavigateComplete(new EventArgs());
					break;
					//document complete
				case (int)HtmlNotification.DocumentComplete:
					OnDocumentComplete(new EventArgs());
					break;
					//title notification
				case (int)HtmlNotification.Title:
					//copy target to title field
					//byte[] titlechars = new byte[256];
					//Marshal.Copy(nmhtml.szTarget, titlechars, 0, 256);
					//m_title = System.Text.Encoding.ASCII.GetString(titlechars, 0, 256);
					//marshal the title
					m_title = MarshalEx.PtrToStringAuto(nmhtml.szTarget);
					break;
			}
		}
#endregion
#endif		
	}

	#region HotSpotClickEventArgs
	/// <summary>
	/// Provides data for the HotSpotClick event
	/// </summary>
	public class HotSpotClickEventArgs : EventArgs
	{
		private string m_url;

		internal HotSpotClickEventArgs(string url)
		{
			m_url = url;
		}

		/// <summary>
		/// The target url of the link that was activated.
		/// </summary>
		public string Url
		{
			get
			{
				return m_url;
			}
		}
	}
	#endregion

	internal class WindowSink : Microsoft.WindowsCE.Forms.MessageWindow
	{

		#region Private members
		private const int WM_USER = 1024;
		private const int WM_IMAGENOTIFY = WM_USER + 42;

		private string m_fileName;
		#endregion

		#region Properties
		public string CurrentFilename
		{
			get
			{
				return m_fileName;
			}
			set
			{
				m_fileName = value;
			}
		}
		#endregion

		#region Constructor
		public WindowSink()
		{
			CurrentFilename = null;
		}

		#endregion

		#region WndProc
		protected override void WndProc(ref Message msg)
		{
			if (msg.Msg == WM_IMAGENOTIFY)
			{
				string fileName = Marshal.PtrToStringUni(msg.WParam);

				// ignore non-local filenames
				if (fileName.StartsWith("http")
					|| fileName.StartsWith("ftp")
					|| CurrentFilename == null)
				{
					return;
				}

				if (!fileName.StartsWith("file://"))
				{
					// CurrentFileName is the saved name of the currently loaded file
					String currentPath = Path.GetDirectoryName(CurrentFilename);
					fileName = currentPath + "\\" + fileName;
				}

				// Write the current filename to the pointer in lParam
				unsafe
				{
					char* szFileName = (char*)msg.LParam;

					for (int i = 0; i < fileName.Length; i++)
					{
						*szFileName = fileName[i];
						szFileName++;
					}
					*szFileName = '\0';
				}

				msg.Result = (IntPtr)1;
			}

		}
		#endregion

	}

}
