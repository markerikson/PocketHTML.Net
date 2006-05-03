#region Using directives

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using ISquared.PocketHTML;
using ISquared.Win32Interop;
using ISquared.Windows.Forms;

#endregion

namespace ISquared.PocketHTML
{
	public class InputDialog : FloatingDialog
	{
		#region Private members
		private TextBox textBox1;
		private Button m_btnOK;
		private Button m_btnCancel;
		private Button m_btnPaste;
		private bool m_multiLine;
		private MainMenu mainMenu2;
		private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;

		private static int WM_PASTE = 0x0302;
		#endregion

		#region properties
		public bool MultiLine
		{
			get { return m_multiLine; }
			set { m_multiLine = value; }
		}

		public String InputText
		{
			get
			{
				return textBox1.Text;
			}
			set
			{
				textBox1.Text = value;
			}
		}
		#endregion

		#region Constructor
		public InputDialog(bool multiline)
		{
			InitializeComponent();
			MultiLine = multiline;

			DpiHelper.AdjustAllControls(this);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_btnPaste = new System.Windows.Forms.Button();
			this.mainMenu2 = new System.Windows.Forms.MainMenu();
			this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.textBox1.Location = new System.Drawing.Point(3, 3);
			this.textBox1.Size = new System.Drawing.Size(200, 19);
			this.textBox1.Text = "line 1\r\nline 2\r\nline 3\r\nline 4\r\nline 5";
			// 
			// m_btnOK
			// 
			this.m_btnOK.Location = new System.Drawing.Point(3, 28);
			this.m_btnOK.Size = new System.Drawing.Size(60, 20);
			this.m_btnOK.Text = "OK";
			this.m_btnOK.Click += new System.EventHandler(this.m_btnOK_Click);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.Location = new System.Drawing.Point(69, 28);
			this.m_btnCancel.Size = new System.Drawing.Size(60, 20);
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
			// 
			// m_btnPaste
			// 
			this.m_btnPaste.Location = new System.Drawing.Point(135, 28);
			this.m_btnPaste.Size = new System.Drawing.Size(60, 20);
			this.m_btnPaste.Text = "Paste";
			this.m_btnPaste.Click += new System.EventHandler(this.m_btnPaste_Click);
			// 
			// InputDialog
			// 
			this.ClientSize = new System.Drawing.Size(240, 268);
			this.ControlBox = false;
			/*
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.m_btnPaste);
			*/
			this.Menu = this.mainMenu2;
			this.Load += new System.EventHandler(this.InputDialog_Load);

			m_btnCancel.Parent = this.ContentPanel;
			m_btnOK.Parent = this.ContentPanel;
			m_btnPaste.Parent = this.ContentPanel;
			textBox1.Parent = this.ContentPanel;

		}

		#endregion
		#endregion

		#region Event handlers
		private void InputDialog_Load(object sender, EventArgs e)
		{
			this.Visible = false;

			if(m_multiLine)
			{
				SetMultiLine();
			}
			else
			{
				SetSingleLine();
			}

			this.Visible = true;
		}

		private void m_btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void m_btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void m_btnPaste_Click(object sender, EventArgs e)
		{
			textBox1.Capture = true;
			IntPtr handle = CoreDLL.GetCapture();
			textBox1.Capture = false;

			CoreDLL.SendMessage(handle, WM_PASTE, 0, 0);
		}
		#endregion

		#region Resize functions
		public void SetMultiLine()
		{
			this.Size = new Size(210, 130);

			textBox1.Multiline = true;
			textBox1.Bounds = new Rectangle(3, 3, 203, 72);
			textBox1.ScrollBars = ScrollBars.Vertical;

			m_btnOK.Location = new Point(3, 81);
			m_btnCancel.Location = new Point(69, 81);
			m_btnPaste.Location = new Point(135, 81);
		}

		public void SetSingleLine()
		{
			this.Size = new Size(210, 88);

			textBox1.Multiline = false;
			textBox1.ScrollBars = ScrollBars.None;
			textBox1.Bounds = new Rectangle(3, 3, 200, 19);

			m_btnOK.Location = new Point(3, 28);
			m_btnCancel.Location = new Point(69, 28);
			m_btnPaste.Location = new Point(135, 28);
		}
		#endregion



		/*
		#region P/Invokes
		[DllImport("coredll.dll")]
		internal extern static IntPtr GetCapture();

		[DllImport("coredll.dll")]
		internal extern static int SendMessage(IntPtr Hwnd, int Msg, int WParam, int LParam);
		#endregion
		*/
	}
}
