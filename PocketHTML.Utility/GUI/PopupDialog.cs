#region Using directives

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using OpenNETCF.Win32;

#endregion

namespace ISquared.Windows.Forms
{
	#region Enums
	[Flags]
	public enum WS
	{
		OVERLAPPED = BORDER | CAPTION,
		CLIPSIBLINGS = 0x04000000,
		CLIPCHILDREN = 0x02000000,
		CAPTION = 0x00C00000,
		BORDER = 0x00800000,
		DLGFRAME = 0x00400000,
		VSCROLL = 0x00200000,
		HSCROLL = 0x00100000,
		SYSMENU = 0x00080000,
		THICKFRAME = 0x00040000,
		MAXIMIZEBOX = 0x00020000,
		MINIMIZEBOX = 0x00010000,
		SIZEBOX = THICKFRAME,
		POPUP = -0x7fffffff,
		TABSTOP = 0x00010000,
		VISIBLE = 0x10000000,
		DISABLED = 0x08000000,
		GROUP = 0x00020000,
		CHILD = 0x40000000,
	}

	[Flags]
	public enum WS_EX
	{
		DLGMODALFRAME = 0x00000001,
		NOPARENTNOTIFY = 0x00000004,
		TOPMOST = 0x00000008,
		MDICHILD = 0x00000040,
		TOOLWINDOW = 0x00000080,

		WINDOWEDGE = 0x00000100,
		CLIENTEDGE = 0x00000200,

		CONTEXTHELP = 0x00000400,
		STATICEDGE = 0x00020000,
		OVERLAPPEDWINDOW = (WINDOWEDGE | CLIENTEDGE),

		//CAPTIONOKBTN		= 0x80000000,
		NODRAG = 0x40000000,
		ABOVESTARTUP = 0x20000000,
		INK = 0x10000000,
		NOANIMATION = 0x04000000,
	}

	[Flags]
	public enum SWPF
	{
		NOSIZE = 0x0001,
		NOMOVE = 0x0002,
		NOZORDER = 0x0004,
		NOREDRAW = 0x0008,
		NOACTIVATE = 0x0010,
		FRAMECHANGED = 0x0020,
		SHOWWINDOW = 0x0040,
		HIDEWINDOW = 0x0080,
		NOCOPYBITS = 0x0100,
		NOOWNERZORDER = 0x0200,
		NOSENDCHANGING = 0x0400,
		DRAWFRAME = FRAMECHANGED,
		NOREPOSITION = NOOWNERZORDER,
		DEFERERASE = 0x2000,
		ASYNCWINDOWPOS = 0x4000
	}

	public enum SWPZO
	{
		TOP = 0,
		BOTTOM = 1,
		TOPMOST = -1,
		NOTOPMOST = -2,
		MESSAGE = -3,
	}

	public enum GWL
	{
		WNDPROC = (-4),
		HINSTANCE = (-6),
		HWNDPARENT = (-8),
		STYLE = (-16),
		EXSTYLE = (-20),
		USERDATA = (-21),
		ID = (-12),
	}
	#endregion

	/// <summary>
	/// Summary description for PopupDialog.
	/// </summary>
	public class PopupDialog : System.Windows.Forms.Form
	{
		#region Private fields
		protected System.Windows.Forms.MainMenu mainMenu1;

		private IntPtr m_handle = IntPtr.Zero;
		private Form m_owner;
		private bool m_topmost;
		private bool m_loaded;
		private bool m_activatingOwner;
		private bool m_focusing;
		private bool m_ownerActivated;
		private bool m_activated;
		#endregion

		#region Properties
		public bool OwnerActivated
		{
			get { return m_ownerActivated; }
			set { m_ownerActivated = value; }
		}

		public IntPtr Handle
		{
			get
			{
				if (m_handle == IntPtr.Zero)
				{
					this.Capture = true;
					m_handle = GetCapture();
					this.Capture = false;
				}

				return m_handle;
			}
		}

		public Form OwnerForm
		{
			get
			{
				return m_owner;
			}
			set
			{
				m_owner = value;
			}
		}

		public Form Owner
		{
			set
			{
				Form owner = value;
				owner.Capture = true;
				IntPtr hwndOwner = GetCapture();
				owner.Capture = false;

				this.Capture = true;
				m_handle = GetCapture();
				this.Capture = false;

				IntPtr result = IntPtr.Zero;

				uint style = (uint)GetWindowLong(m_handle, (int)GWL.STYLE);

				WS wsStyle = WS.BORDER | WS.CAPTION | WS.POPUP & ~WS.MINIMIZEBOX;

				style |= (uint)wsStyle;

				SetWindowLong(m_handle, (int)GWL.STYLE, (int)style);
			}
		}
		#endregion

		public PopupDialog(Form parent)
		{
			this.FormBorderStyle = FormBorderStyle.None;
			this.FormBorderStyle = FormBorderStyle.None;

			this.ControlBox = false;
			this.MinimizeBox = false;
			this.MaximizeBox = false;

			//this.Visible = false;



			InitializeComponent();
			OwnerForm = parent;
			//this.Menu = mainMenu1;
		}

		protected override void Dispose(bool disposing)
		{
			
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.Text = "";
			//this.Menu = this.mainMenu1;
		}

		#endregion

		#region Event handlers and overrides
		void parent_Deactivate(object sender, EventArgs e)
		{
			if (Visible)
			{
				SetTopmost(false);
			}
		}

		void parent_Activated(object sender, EventArgs e)
		{
			if (Visible)
			{
				SetTopmost(true);
			}
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			SetStyle();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			m_owner.Activated -= new EventHandler(parent_Activated);
			m_owner.Deactivate -= new EventHandler(parent_Deactivate);
			base.OnClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			SetStyle();
			OwnerForm.Activated += new EventHandler(parent_Activated);
			OwnerForm.Deactivate += new EventHandler(parent_Deactivate);
			m_loaded = true;
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			//this.Menu = mainMenu1;
			m_activated = true;
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);

			m_activated = false;

			if (m_loaded && !m_ownerActivated)
			{
				SetTopmost(false);
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			if (!m_focusing)
			{
				m_focusing = true;

				if (!m_topmost && m_loaded && !m_activatingOwner)
				{
					if (m_owner != null)
					{
						m_activatingOwner = true;
						m_owner.Show();
						//this.Focus();
						this.BringToFront();
						m_activatingOwner = false;
					}
				}

				m_focusing = false;
			}
		}
		#endregion

		#region Style functions
		public void SetStyle()
		{


			this.Capture = true;
			m_handle = GetCapture();
			this.Capture = false;

			//uint style = (uint)GetWindowLong(m_handle, (int)GWL.STYLE);

			WS wsStyle = WS.BORDER | WS.CAPTION | WS.POPUP & ~WS.MINIMIZEBOX;

			uint style = 0x90c808c4;//(uint)wsStyle;
			uint minimize = (uint)WS.MINIMIZEBOX;
			uint sysmenu = (uint)WS.SYSMENU;
			style &= ~minimize;
			style &= ~sysmenu;

			SetWindowLong(m_handle, (int)GWL.STYLE, (int)style);
			//this.Bounds = new Rectangle(20, 40, 200, 100);

			SWPF swpf = SWPF.NOZORDER | SWPF.SHOWWINDOW | SWPF.NOSIZE | SWPF.NOMOVE;// | SWPF.NOACTIVATE;
			SetWindowPos(m_handle, (int)SWPZO.TOP, 60, 200,
				this.Width, this.Height, (int)swpf);

			SetTopmost(true);
		}

		public void SetTopmost(bool enable)
		{
			SWPF swpf = SWPF.NOSIZE | SWPF.NOMOVE | SWPF.NOACTIVATE;

			SetWindowPos(m_handle, enable ? (int)SWPZO.TOPMOST : (int)SWPZO.NOTOPMOST, this.Left, this.Top,
				this.Width, this.Height, (int)swpf);

			m_topmost = enable;
		}
		#endregion


		#region PInvokes
		[DllImport("coredll")]
		private static extern int SetWindowLong(IntPtr hwnd, int message, int style);

		[DllImport("coredll")]
		private static extern int GetWindowLong(IntPtr hwnd, int message);

		[DllImport("coredll")]
		private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool something);

		[DllImport("coredll")]
		private static extern bool SetWindowPos(IntPtr hwnd, int message, int x, int y, int width, int height, int something);

		[DllImport("coredll")]
		private static extern IntPtr GetCapture();

		[DllImport("coredll")]
		private static extern int GetLastError();

		[DllImport("coredll")]
		private static extern void SetLastError(int error);

		[DllImport("coredll")]
		private static extern IntPtr SetParent(IntPtr hwndChild, IntPtr hwndParent);
		#endregion

	}
}
