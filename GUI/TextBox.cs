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

//

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using OpenNETCF.Win32;
using ISquared.Win32Interop;
using ISquared.Win32Interop.WinEnums;
using System.Text.RegularExpressions;
using System.Text;

#if DESIGN && STANDALONE
[assembly: System.CF.Design.RuntimeAssemblyAttribute("OpenNETCF.Windows.Forms.TextBox, Version=1.0.5000.4, Culture=neutral, PublicKeyToken=null")]
#endif

namespace OpenNETCF.Windows.Forms
{
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
        private const int GWL_STYLE		= (-16);
        private const int ES_UPPERCASE   = 0x0008;
        private const int ES_LOWERCASE   = 0x0010;
        private const int ES_NUMBER     = 0x2000;

        private IntPtr hwnd = IntPtr.Zero;
        private int defaultStyle = 0;
        private TextBoxStyle style;
		private Regex m_reLeadingSpaces;

        protected virtual event EventHandler OnStyleChanged;

        public TextBoxEx()
        {
            hwnd = setHwnd();
            defaultStyle = GetWindowLong(hwnd, GWL_STYLE);

			m_reLeadingSpaces = new Regex(
				@"^\s+",
				RegexOptions.IgnoreCase |
				RegexOptions.Multiline |
				RegexOptions.IgnorePatternWhitespace);
        }

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		/// <value>An IntPtr that contains the window handle (HWND) of the control.</value>
        public IntPtr Handle
        {
            get { return hwnd; }
        }

        private IntPtr setHwnd()
        {
            this.Capture = true;
            hwnd = GetCapture();
            this.Capture = false;

			

            return hwnd;
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

        private void OnTextBoxStyleChanged(EventArgs e)
        {
            switch(style)
            {					
                case TextBoxStyle.Default:
                    SetWindowLong(hwnd, GWL_STYLE, defaultStyle);
                    break;
                case TextBoxStyle.Numeric:
                    SetWindowLong(hwnd, GWL_STYLE, defaultStyle|ES_NUMBER);
                    break;
                case TextBoxStyle.UpperCase:
                    SetWindowLong(hwnd, GWL_STYLE, defaultStyle|ES_UPPERCASE);
                    break;
                case TextBoxStyle.LowerCase:
                    SetWindowLong(hwnd, GWL_STYLE, defaultStyle|ES_LOWERCASE);
                    break;
                default:
                    SetWindowLong(hwnd, GWL_STYLE, defaultStyle);
                    break;
            }

            if(OnStyleChanged != null)
                OnStyleChanged(this, e);
        }

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
		/*
		#region Clipboard Support
		/// <summary>
		/// Moves the current selection in the <see cref="TextBoxEx"/> to the Clipboard.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Cut()
		{
			//send Cut message
			SendMessage(this.hwnd, (int)ClipboardMessage.WM_CUT, 0, 0);
		}

		/// <summary>
		/// Copies the current selection in the <see cref="TextBoxEx"/> to the Clipboard.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Copy()
		{
			//send Copy message
			SendMessage(this.hwnd, (int)ClipboardMessage.WM_COPY, 0, 0);
		}

		/// <summary>
		/// Replaces the current selection in the <see cref="TextBoxEx"/> with the contents of the Clipboard.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Paste()
		{
			//send Paste message
			SendMessage(this.hwnd, (int)ClipboardMessage.WM_PASTE, 0, 0);
		}

		/// <summary>
		/// Clears all text from the <see cref="TextBoxEx"/> control.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Clear()
		{
			//send Clear message
			SendMessage(this.hwnd, (int)ClipboardMessage.WM_CLEAR, 0, 0);
		}

		/// <summary>
		/// Undoes the last edit operation in the <see cref="TextBoxEx"/>.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Undo()
		{
			//send Undo message
			SendMessage(this.hwnd, (int)ClipboardMessage.WM_UNDO, 0, 0);
		}
		#endregion
		*/
		private enum ClipboardMessage : int
		{
			WM_CUT  = 0x0300,
			WM_COPY = 0x0301,
			WM_PASTE = 0x0302,
			WM_CLEAR = 0x0303,
			WM_UNDO = 0x0304,
		}

		/*
		public void SelectAll()
		{
			SendMessage(this.hwnd, (int)EM.SETSEL, 0, -1);
		}
		*/

		public String GetLeadingSpaces()
		{
			setHwnd();
			IntPtr pTB = this.hwnd;//CoreDLL.GetHandle(m_editorPanel.TextBox);
			int currentLine = CoreDLL.SendMessage(pTB, (int)EM.LINEFROMCHAR, -1, 0);

			StringBuilder lineText;
			int charIndex = CoreDLL.SendMessage(pTB, (int)EM.LINEINDEX, currentLine, 0);
			int lineLength = CoreDLL.SendMessage(pTB, (int)EM.LINELENGTH,
				charIndex, 0);


			lineText = new StringBuilder(" ", lineLength + 1);
			lineText[0] = (char)(lineLength + 1);

			int numChars = CoreDLL.SendMessageGetLine(pTB, (int)EM.GETLINE,
				currentLine, lineText);

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

		public void AutoIndent()
		{
			string spaces = GetLeadingSpaces();
			//IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
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

		public int CurrentLine
		{
			get
			{
				setHwnd();
				return SendMessage(this.hwnd, (int)EM.LINEFROMCHAR, -1, 0); 
			}
		}

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
