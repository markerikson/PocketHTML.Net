//==========================================================================================
//
//		OpenNETCF.Windows.Forms.ControlMessageWindow
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
#if !DESIGN
using System;
using System.Windows.Forms;
using OpenNETCF.Win32;
using OpenNETCF.Windows.Forms;
#if !NDOC
using Microsoft.WindowsCE.Forms;
#endif
using ISquared.Win32Interop;

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Generic MessageWindow implementation for hosting native controls
	/// </summary>

	#if !NDOC
	public class ControlMessageWindow : MessageWindow
	#else
	public class ControlMessageWindow
	#endif
	{
		#region Fields
		//parent control
		private Control m_parent;
		private IntPtr m_parenthwnd;

		//control hwnd
		private IntPtr m_control;

		//hinstance
		private IntPtr m_instance;
		#endregion

		//message received from native control as notification
		private const int WM_NOTIFY = 0x004E;

		#region Constructor
		/// <summary>
		/// Creates a new ControlMessageWindow as child of specified Control with specified native control.
		/// </summary>
		/// <param name="parent">Parent Control. Must implement <see cref="T:OpenNETCF.Windows.Forms.IWin32Window"/> and <see cref="T:OpenNETCF.Windows.Forms.INotifiable"/> interfaces.</param>
		/// <param name="classname">Class name of native control to create.</param>
		public ControlMessageWindow(Control parent, string classname) : this(parent, classname, 0){}
		
		/// <summary>
		/// Creates a new ControlMessageWindow as child of specified Control with specified native control.
		/// </summary>
		/// <param name="parent">Parent Control. Must implement <see cref="T:OpenNETCF.Windows.Forms.IWin32Window"/> and <see cref="T:OpenNETCF.Windows.Forms.INotifiable"/> interfaces.</param>
		/// <param name="classname">Class name of native control to create.</param>
		/// <param name="flags">Flags which specify additional options.</param>
		public ControlMessageWindow(Control parent, string classname, ControlMessageWindowFlags flags)
		{
			//set parent
			m_parent = parent;


#if !NDOC
			//get instance handle
			m_instance = CoreDLL.GetModuleHandle(null);

			//make this window a child of parent control
			OpenNETCF.Win32.Win32Window.SetParent(this.Hwnd, this.ParentHWnd);

			//set window style - make visible
			Win32Window.SetWindowLong(this.Hwnd, (int)Win32Window.GetWindowLongParam.GWL_STYLE, 0x46000000);
			//set ex style top-most
			Win32Window.SetWindowLong(this.Hwnd, (int)Win32Window.GetWindowLongParam.GWL_EXSTYLE, 0x00000008);

			//set visible style full size of parent
			OpenNETCF.Win32.Win32Window.SetWindowPos(this.Hwnd, (OpenNETCF.Win32.Win32Window.SetWindowPosZOrder)0, 0, 0, m_parent.Width, m_parent.Height, (OpenNETCF.Win32.Win32Window.SetWindowPosFlags)0x0047);

			//create the native control
			m_control = OpenNETCF.Win32.Win32Window.CreateWindowEx(IntPtr.Zero, classname, "NativeControl", (IntPtr)(Win32Window.WindowStyle.WS_CHILD | Win32Window.WindowStyle.WS_VISIBLE), 0, 0, m_parent.Width, m_parent.Height, this.Hwnd, IntPtr.Zero, m_instance, IntPtr.Zero);
			//react to parent control events
			m_parent.Resize+=new EventHandler(Parent_Resize);
			
			if((flags & ControlMessageWindowFlags.IgnoreFocus) != ControlMessageWindowFlags.IgnoreFocus)
			{
				m_parent.GotFocus+=new EventHandler(Parent_GotFocus);
			}
			

			//hook keyboard
			//m_parent.KeyDown+=new KeyEventHandler(m_parent_KeyDown);
			//m_parent.KeyUp+=new KeyEventHandler(m_parent_KeyUp);

			//hook disposed event to invoke dispose on this object
			m_parent.Disposed+=new EventHandler(Parent_Disposed);
#endif
		}
		/// <summary>
		/// Creates a ControlMessageWindow for a native control constructed by an API function
		/// </summary>
		/// <param name="parent">Parent Control. Must implement <see cref="T:OpenNETCF.Windows.Forms.IWin32Window"/> and <see cref="T:OpenNETCF.Windows.Forms.INotifiable"/> interfaces.</param>
		/// <param name="handle">Window Handle of control.</param>
		public ControlMessageWindow(Control parent, IntPtr handle) : this(parent)
		{
			m_control = handle;
#if !NDOC
			//make the control a child of this messagewindow
			OpenNETCF.Win32.Win32Window.SetParent(m_control, this.Hwnd);
#endif

		}
		internal ControlMessageWindow(Control parent)
		{
			//set parent
			m_parent = parent;

#if !NDOC
			//make this window a child of parent control
			OpenNETCF.Win32.Win32Window.SetParent(this.Hwnd, ((IWin32Window)m_parent).Handle);

			//set window style - make visible
			Win32Window.SetWindowLong(this.Hwnd, (int)Win32Window.GetWindowLongParam.GWL_STYLE, 0x46000000);
			//set ex style top-most
			Win32Window.SetWindowLong(this.Hwnd, (int)Win32Window.GetWindowLongParam.GWL_EXSTYLE, 0x00000008);

			//set visible style full size of parent
			OpenNETCF.Win32.Win32Window.SetWindowPos(this.Hwnd, (OpenNETCF.Win32.Win32Window.SetWindowPosZOrder)0, 0, 0, m_parent.Width, m_parent.Height, (OpenNETCF.Win32.Win32Window.SetWindowPosFlags)0x0047);

			//react to parent control events
			m_parent.Resize+=new EventHandler(Parent_Resize);
			m_parent.GotFocus+=new EventHandler(Parent_GotFocus);
#endif
		}
		#endregion

		#region Add Control
		internal void AddControl(IntPtr handle)
		{
			m_control = handle;
		}
		#endregion

		#region Dispose
#if !NDOC
		/// <summary>
		/// Frees up resources used by the ControlMessageWindow
		/// </summary>
		public new void Dispose()
		{
			//kill the native control
			Win32Window.DestroyWindow(ControlHWnd);
			//dispose of the messagewindow
			base.Dispose();
		}
#endif
		#endregion

		#region BorderStyle
		/// <summary>
		/// Gets or sets the border style for the ControlMessageWindow.
		/// </summary>
		public BorderStyle BorderStyle
		{
			get
			{
#if!NDOC
				int style = Win32Window.GetWindowLong(this.Hwnd, Win32Window.GetWindowLongParam.GWL_STYLE);
				if((style & (int)Win32Window.WindowStyle.WS_BORDER) == (int)Win32Window.WindowStyle.WS_BORDER)
				{
					return BorderStyle.FixedSingle;
				}
				else
				{
					return BorderStyle.None;
				}
#else
				return BorderStyle.None;				
#endif
			}
			set
			{
#if !NDOC
				if(value==BorderStyle.None)
				{
					Win32Window.UpdateWindowStyle(this.Hwnd, (int)Win32Window.WindowStyle.WS_BORDER, 0);
				}
				else
				{
					Win32Window.UpdateWindowStyle(this.Hwnd, 0,(int)Win32Window.WindowStyle.WS_BORDER);
				}
#endif
			}
		}
		#endregion

		#region ControlHWnd
		/// <summary>
		/// Window Handle of native control.
		/// </summary>
		public IntPtr ControlHWnd
		{
			get
			{
				return m_control;
			}
		}
		#endregion

		#region ParentHWnd
		/// <summary>
		/// Window Handle of parent managed control.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public IntPtr ParentHWnd
		{
			get
			{
#if !NDOC
				//if we don't have the handle get it
				if(m_parenthwnd==IntPtr.Zero)
				{
					m_parent.Capture = true;
					m_parenthwnd = OpenNETCF.Win32.Win32Window.GetCapture();
					m_parent.Capture = false;
				}
#endif
				return m_parenthwnd;
			}
		}
		#endregion

		#region Parent
		/// <summary>
		/// Gets the parent Control for this ControlMessageWindow
		/// </summary>
		public Control Parent
		{
			get
			{
				return m_parent;
			}
		}
		#endregion

		#region SendMessage
		/// <summary>
		/// Sends a message to the native control
		/// </summary>
		/// <param name="msg">Windows message to send</param>
		/// <param name="WParam">WParam</param>
		/// <param name="LParam">LParam</param>
		/// <returns>Return code</returns>
		public IntPtr SendMessage(int msg, int WParam, int LParam)
		{
			return Win32Window.SendMessage(ControlHWnd, msg, WParam, LParam);
		}
		#endregion

		#region Clipboard Support
		/// <summary>
		/// Moves the current selection in the text box to the Clipboard.
		/// </summary>
		public void Cut()
		{
			//send Cut message
			SendMessage((int)CommonMessage.WM_CUT, 0, 0);
		}
		/// <summary>
		/// Copies the current selection in the text box to the Clipboard.
		/// </summary>
		public void Copy()
		{
			//send Copy message
			SendMessage((int)CommonMessage.WM_COPY, 0, 0);
		}
		/// <summary>
		/// Replaces the current selection in the text box with the contents of the Clipboard.
		/// </summary>
		public void Paste()
		{
			//send Paste message
			SendMessage((int)CommonMessage.WM_PASTE, 0, 0);
		}
		#endregion

		#region WndProc
		#if !NDOC
		/// <summary>
		/// Handles incoming windows messages from the native control
		/// </summary>
		/// <param name="m">Message</param>
		protected override void WndProc(ref Message m)
		{
			//only catch notify messages
			if(m.Msg == WM_NOTIFY)
			{
				//pass to the parent control's Notify method
				((INotifiable)m_parent).Notify(m.LParam);
			}

			//catch messages here
			base.WndProc (ref m);
		}
		#endif
		#endregion

		/// <summary>
		/// Flags which specify additional options when creating a <see cref="ControlMessageWindow"/>.
		/// </summary>
		public enum ControlMessageWindowFlags : int
		{
			/// <summary>
			/// Don't act on the parent control receiving focus.
			/// </summary>
			IgnoreFocus = 1,
		}

		internal enum CommonMessage : int
		{
			WM_KEYDOWN = 0x0100,
			WM_KEYUP = 0x0101,

			WM_CUT  = 0x0300,
			WM_COPY = 0x0301,
			WM_PASTE = 0x0302,
			WM_CLEAR = 0x0303,
		}

		#region Parent Resize
		/// <summary>
		/// Handles resize events from the parent control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Parent_Resize(object sender, EventArgs e)
		{
#if !NDOC
			//resize messagewindow container and then control
			Win32Window.SetWindowPos(this.Hwnd, (Win32Window.SetWindowPosZOrder)0, 0, 0, m_parent.Width, m_parent.Height, (OpenNETCF.Win32.Win32Window.SetWindowPosFlags)0);
			Win32Window.SetWindowPos(this.ControlHWnd, (Win32Window.SetWindowPosZOrder)0, 0, 0, m_parent.Width, m_parent.Height, (Win32Window.SetWindowPosFlags)0);
#endif
		}
		#endregion

		#region Parent Focus
		/// <summary>
		/// Catch when focus is passed to our parent managed control (and pass it on to the native control we are hosting)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Parent_GotFocus(object sender, EventArgs e)
		{
			//pass on the focus to the native control
			Win32Window.SetFocus(ControlHWnd);
		}
		#endregion

		#region Parent Disposed
		/// <summary>
		/// Catch when parent control is disposed and dispose this object.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Parent_Disposed(object sender, EventArgs e)
		{
#if !NDOC
			this.Dispose();
#endif
		}
		#endregion

		/*private void m_parent_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Tab)
			{
				uint data = 0;
				if(e.Alt)
				{
					data = data & 0x20000000;
				}
				data = data & 0x40000000;
			
				SendMessage((int)CommonMessage.WM_KEYDOWN,e.KeyValue, Convert.ToInt32(data));
				e.Handled = true;
			}
		}

		private void m_parent_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Tab)
			{
				uint data = 0;
				if(e.Alt)
				{
					data = data & 0x20000000;
				}
				data = data & 0x80000000;

				SendMessage((int)CommonMessage.WM_KEYUP,e.KeyValue, Convert.ToInt32(data));
				e.Handled = true;

			}
		}*/
	}

	
}
#endif
