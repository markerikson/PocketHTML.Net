#region comments
/*=======================================================================================

	OpenNETCF.Windows.Forms.TextBoxEx
	Copyright © 2003-2004, OpenNETCF.org

	This library is free software; you can redistribute it and/or modify it under 
	the terms of the OpenNETCF.org Shared Source License.

	This library is distributed in the hope that it will be useful, but 
	WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
	FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
	for more details.

	You should have received a copy of the OpenNETCF.org Shared Source License 
	along with this library; if not, email licensing@opennetcf.org to request a copy.

	If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
	email licensing@opennetcf.org.

	For general enquiries, email enquiries@opennetcf.org or visit our website at:
	http://www.opennetcf.org

=======================================================================================*/

// Change log
//
// ----------------------------
// July 17, 2003 - Neil Cowburn
// ----------------------------
// - Base source submitted
//
// ---------------------------
// August 5, 2003 - Mark Stega
// ---------------------------
// - Added designer support
//
// ----------------------------
// August 12, 2003 - Mark Stega
// ----------------------------
// - Changed class name to TextBoxEx
// - Changed output to be OpenNETCF.Windows.Forms.TextBoxEx
//
// -------------------------------
// October 28, 2003 - Neil Cowburn
// -------------------------------
// - Changed preprocessor directives to use DESIGN
// - Added conditional preprocessor to RuntimeAssemblyAttribute
//
// -------------------------------
// February 03, 2004 - Peter Foot
// -------------------------------
// - Renamed HWnd property to Handle implementing IWin32Window interface

#endregion

#region using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using OpenNETCF.Win32;
using ISquared.Win32Interop;
using System.Text.RegularExpressions;
using System.Text;
#endregion

#if DESIGN && STANDALONE
[assembly: System.CF.Design.RuntimeAssemblyAttribute("OpenNETCF.Windows.Forms.TextBox, Version=1.0.5000.4, Culture=neutral, PublicKeyToken=null")]
#endif

namespace OpenNETCF.Windows.Forms
{
	#region enums
	/// <summary>
	/// Specifies the style of the <see cref="TextBoxEx"/>.
	/// </summary>
    public enum TextBoxStyle
    {
		/// <summary>
		/// Default.
		/// </summary>
        Default,
		/// <summary>
		/// Text is all numeric.
		/// </summary>
        Numeric,
		/// <summary>
		/// Text is all upper-case.
		/// </summary>
        UpperCase,
		/// <summary>
		/// Text is all lower-case.
		/// </summary>
        LowerCase
    }

	/// <summary>
	/// Specifies how the <see cref="TextBoxEx"/> border is drawn.
	/// </summary>
    public enum TextBoxBorder
    {
		/// <summary>
		/// No border is drawn around the textbox.
		/// </summary>
        None,
		/// <summary>
		/// A single border is drawn around the textbox.
		/// </summary>
        FixedSingle
	}
	#endregion

	/// <summary>
	/// Represents an enhanced TextBox.
	/// </summary>
	public class TextBoxEx : System.Windows.Forms.TextBox, IWin32Window
	{

#if DESIGN
        /// <summary>
        /// Constructor
        /// </summary>
        public TextBoxEx()
		{
            BorderStyle = BorderStyle.FixedSingle;
		}

        /// <summary>
        /// Destructor
        /// </summary>
        ~TextBoxEx()
        {
        }

        #region ------------ Design time painting ------------

		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains information about the control to paint.</param>
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
//			base.OnPaintBackground(e);
		}

        /// <summary>
        /// OnPaint override
        /// </summary>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea)
        {
            Pen pb = new Pen(Color.Black);

            SolidBrush bw = new SolidBrush(Color.White);
            SolidBrush bb = new SolidBrush(Color.Black);
            SolidBrush bg = new SolidBrush(Color.Gray);

            if (this.ReadOnly)
                pea.Graphics.FillRectangle(bg,this.ClientRectangle);
            else
                pea.Graphics.FillRectangle(bw,this.ClientRectangle);

            pea.Graphics.DrawString(
                this.Text,
                this.Font,
                bb,
                2,
                2);

            pb.Dispose();

            bb.Dispose();
            bw.Dispose();
            bg.Dispose();

//            base.OnPaint(pea);
        }

        /// Raises the Resize event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnResize(System.EventArgs e)
		{
			this.Invalidate();

			base.OnResize(e);
		}
        #endregion
#endif

#if !DESIGN

		#region private members
		private const int GWL_STYLE		= (-16);
        private const int ES_UPPERCASE   = 0x0008;
        private const int ES_LOWERCASE   = 0x0010;
        private const int ES_NUMBER     = 0x2000;

        private IntPtr hwnd = IntPtr.Zero;
        private int defaultStyle = 0;
        private TextBoxStyle style;
		private Regex m_reLeadingSpaces;
		private bool m_autoIndent;

		// Keeps the auto-indent function from recursing
		private bool m_firstEnter;

		protected virtual event EventHandler OnStyleChanged;

		#endregion

		#region Constructor
		public TextBoxEx()
        {
            hwnd = setHwnd();
            defaultStyle = GetWindowLong(hwnd, GWL_STYLE);

			m_reLeadingSpaces = new Regex(
				@"^\s+",
				RegexOptions.IgnoreCase |
				RegexOptions.Multiline |
				RegexOptions.IgnorePatternWhitespace);

			m_firstEnter = true;
			//m_indenting = false;

			KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
			KeyDown += new KeyEventHandler(textBox1_KeyDown);
		}
		#endregion

		#region properties
		public bool AutoIndent
		{
			get { return m_autoIndent; }
			set { m_autoIndent = value; }
		}

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		/// <value>An IntPtr that contains the window handle (HWND) of the control.</value>
        public IntPtr Handle
        {
            get { return hwnd; }
		}

		public TextBoxStyle Style
		{
			get { return style; }
			set
			{
				style = value;
				OnTextBoxStyleChanged(null);
			}
		}

		public bool CanUndo
		{
			get
			{
				setHwnd();
				int result = SendMessage(this.hwnd, (int)EM.CANUNDO, 0, 0);
				return (result == 1);
			}
		}

		public int CurrentLine
		{
			get
			{
				setHwnd();
				return SendMessage(this.hwnd, (int)EM.LINEFROMCHAR, -1, 0);
			}
		}
		#endregion

		#region Other functions
		private IntPtr setHwnd()
        {
            this.Capture = true;
            hwnd = GetCapture();
            this.Capture = false;

            return hwnd;
		}
		#endregion

		#region Clipboard functions
		public void Copy()
		{
			setHwnd();
			SendMessage(this.hwnd, (int)WM.COPY, 0, 0);
		}

		public void Undo()
		{
			setHwnd();
			SendMessage(this.hwnd, (int)WM.UNDO, 0, 0);
		}

		public void Paste()
		{
			setHwnd();
			SendMessage(this.hwnd, (int)WM.PASTE, 0, 0);
			this.Modified = true;
		}

		public void Clear()
		{
			setHwnd();
			SendMessage(this.hwnd, (int)EM.SETSEL, 0, -1);
			SendMessage(this.hwnd, (int)WM.CLEAR, 0, 0);
		}

		public void Cut()
		{
			setHwnd();
			SendMessage(this.hwnd, (int)WM.CUT, 0, 0);
		}
		#endregion

		#region Text manipulation functions
		public string GetLeadingSpaces()
		{
			setHwnd();
			IntPtr pTB = this.hwnd;
			int currentLine = CoreDLL.SendMessage(pTB, (int)EM.LINEFROMCHAR, -1, 0);
			
			return GetLeadingSpaces(currentLine);
		}

		public string GetLeadingSpaces(int lineNumber)
		{
			setHwnd();
			IntPtr pTB = this.hwnd;

			StringBuilder lineText;
			int charIndex = CoreDLL.SendMessage(pTB, (int)EM.LINEINDEX, lineNumber, 0);
			int lineLength = CoreDLL.SendMessage(pTB, (int)EM.LINELENGTH,
				charIndex, 0);


			lineText = new StringBuilder(" ", lineLength + 1);
			lineText[0] = (char)(lineLength + 1);

			int numChars = CoreDLL.SendMessageGetLine(pTB, (int)EM.GETLINE,
				lineNumber, lineText);

			string line = lineText.ToString();
			String spaces;

			Match lws = m_reLeadingSpaces.Match(line);

			if (lws.Success)
			{
				spaces = lws.Captures[0].Value;
			}
			else
			{
				spaces = String.Empty;
			}
			return spaces;
		}

		public void DoAutoIndent()
		{
			string spaces = GetLeadingSpaces();
			CoreDLL.SendMessageString(this.hwnd, (int)EM.REPLACESEL, 0, "\r\n" + spaces);
		}

		public void ReplaceSelection(string text)
		{
			setHwnd();
			CoreDLL.SendMessageString(this.hwnd, (int)EM.REPLACESEL, 1,text);	
		}

		public int GetLineIndex(int lineNumber)
		{
			setHwnd();
			return SendMessage(this.hwnd, (int)EM.LINEINDEX, lineNumber, 0); ;
		}
		
		public void IndentSelection()
		{
			if (SelectionLength == 0)
			{
				ReplaceSelection("\t");
				SelectionLength = 0;

				Focus();
				return;
			}

			string soep = "";
			int start = SelectionStart;
			int end = start + SelectionLength;
			soep = SelectedText;
			int loc = 0; soep = soep.Insert(0, "\t");

			while (loc != -1)
			{
				loc = soep.IndexOf("\n", loc);
				if (loc > 0)
				{
					soep = soep.Insert(loc + 1, "\t");
					loc = loc + 1;
					if (loc >= soep.Length)
						break;
				}
			}

			ReplaceSelection(soep);
			SelectionStart = start;
			SelectionLength = soep.Length;

			Focus();
		}

		public void UnindentSelection()
		{
			char previousChar = Text[SelectionStart - 1];

			if (SelectionLength == 0)
			{
				if (previousChar == '\t')
				{
					SelectionStart -= 1;
					SelectionLength = 1;
					ReplaceSelection("");
				}

				Focus();
				return;
			}
			string soep = "";

			if(previousChar == '\t')
			{
				SelectionStart -= 1;
			}
			int start = SelectionStart;
			
			int end = start + SelectionLength;
			soep = SelectedText;
			int loc = 0;
			int loc2 = 0;
			int loc3 = 0;
			loc2 = soep.IndexOf("\n", loc);
			loc3 = soep.IndexOf("\t", loc);
			if (loc3 < loc2 || loc2 == -1)
			{
				if (loc3 > -1)
				{
					soep = soep.Remove(loc3, 1);
				}
			} loc = 0;

			bool exitloop = false;

			while (exitloop == false)
			{
				loc = soep.IndexOf("\n", loc);
				loc2 = soep.IndexOf("\n", loc + 1);

				if (loc < 0)
					exitloop = true;

				if (loc > -1)
				{
					loc3 = soep.IndexOf("\t", loc);

					if ((loc3 < loc2 || loc2 == -1))
					{
						if (loc3 > 0)
						{
							soep = soep.Remove(loc3, 1);
						}
					}

					loc = loc + 1; // 

					if (loc >= soep.Length)
						exitloop = true;
				}
			}

			ReplaceSelection(soep);

			SelectionStart = start;

			SelectionLength = soep.Length;

			Focus();
		}
		#endregion

		#region Event handlers
		private void OnTextBoxStyleChanged(EventArgs e)
		{
			switch (style)
			{
				case TextBoxStyle.Default:
					SetWindowLong(hwnd, GWL_STYLE, defaultStyle);
					break;
				case TextBoxStyle.Numeric:
					SetWindowLong(hwnd, GWL_STYLE, defaultStyle | ES_NUMBER);
					break;
				case TextBoxStyle.UpperCase:
					SetWindowLong(hwnd, GWL_STYLE, defaultStyle | ES_UPPERCASE);
					break;
				case TextBoxStyle.LowerCase:
					SetWindowLong(hwnd, GWL_STYLE, defaultStyle | ES_LOWERCASE);
					break;
				default:
					SetWindowLong(hwnd, GWL_STYLE, defaultStyle);
					break;
			}

			if (OnStyleChanged != null)
				OnStyleChanged(this, e);
		}

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				// Tab
				case Keys.Tab:
				{
					if (e.Shift)
					{
						UnindentSelection();
						e.Handled = true;
					}
					else
					{
						if(SelectionLength > 0)
						{
							IndentSelection();
							e.Handled = true;
						}						
					}					
					break;
				}
			}
		}

		private void textBox1_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				// Tab
				case Keys.Tab:
				{
					if(SelectionLength > 0)
					{
						e.Handled = true;
					}
					
					break;
				}
			}
		}

		void textBox1_KeyPress(Object o, KeyPressEventArgs e)
		{
			// The keypressed method uses the KeyChar property to check 
			// whether the ENTER key is pressed. 

			// If the ENTER key is pressed, the Handled property is set to true, 
			// to indicate the event is handled.
			switch (e.KeyChar)
			{
				// Enter
				case (char)Keys.Enter:
				{
					if (m_firstEnter)
					{
						m_firstEnter = false;

						if (AutoIndent)
						{
							e.Handled = true;
							DoAutoIndent();
						}

						m_firstEnter = true;
					}
					break;
				}
				// Tab
				case (char)Keys.Tab:
				{
					if (SelectionLength > 0)
					{
						e.Handled = true;
					}
					break;
				}
			}
		}
		#endregion

		#region -------------------- P/Invokes --------------------
		[DllImport("coredll.dll")] 
		internal extern static IntPtr GetCapture();

		[DllImport("coredll.dll")]
		internal extern static int GetWindowLong(IntPtr Hwnd, int Index);

		[DllImport("coredll.dll")]
		internal extern static int SetWindowLong(IntPtr Hwnd, int Index, int NewIndex);

		[DllImport("coredll.dll")]
		internal extern static int SendMessage(IntPtr Hwnd, int Msg, int WParam, int LParam);

		[DllImport("coredll")]
		internal static extern IntPtr GetFocus();
		#endregion
#endif
	}
}
