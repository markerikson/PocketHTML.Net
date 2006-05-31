#region Using directives

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using ISquared.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.WindowsCE.Forms;

#endregion

namespace ISquared.Windows.Forms
{
	public class FindDialog : FloatingDialog
	{
		#region protected members
		protected System.Windows.Forms.CheckBox m_cbCase;
		protected System.Windows.Forms.CheckBox m_cbWord;
		protected TextBox m_tbFind;
		protected Button m_btnFind;
		protected Button m_btnClose;
		protected Label m_lblFind;
		protected InputPanel m_inputPanel;

		protected string m_previousSearchPattern = string.Empty;

		private TextBox m_tbTargetTextbox;
		#endregion

		#region properties
		public TextBox TargetTextbox
		{
			get { return m_tbTargetTextbox; }
			set { m_tbTargetTextbox = value; }
		}

		public string SearchText
		{
			get
			{
				return m_tbFind.Text;
			}
			set
			{
				m_tbFind.Text = value;
				m_tbFind.Focus();
			}
		}

		public bool WholeWord
		{
			get
			{
				return this.m_cbWord.Checked;
			}
			set
			{
				this.m_cbWord.Checked = value;
			}
		}

		public bool MatchCase
		{
			get
			{
				return this.m_cbCase.Checked;
			}
			set
			{
				this.m_cbCase.Checked = value;
			}
		}
		#endregion

		#region Constructor
		public FindDialog(TextBox tb)
		{
			this.Visible = false;
            this.Size = new Size(DpiHelper.Scale(184), DpiHelper.Scale(108));

            //int scaled16 = DpiHelper.Scale(16);
            //m_pnlTitle.Bounds = new Rectangle(0, 0, this.Width, scaled16);
            //m_pnlContent.Bounds = new Rectangle(1, scaled16, this.Width - 2, this.Height - scaled16);
			InitializeComponent();
			this.Caption = "Find";
			m_inputPanel = new InputPanel();

			TargetTextbox = tb;

			

			//DpiHelper.AdjustAllControls(this);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.FormBorderStyle = FormBorderStyle.None;

			m_cbCase = new System.Windows.Forms.CheckBox();
			m_cbWord = new System.Windows.Forms.CheckBox();
			m_lblFind = new System.Windows.Forms.Label();
			this.m_tbFind = new TextBox();

			m_btnFind = new Button();
			this.m_btnFind.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			//this.m_btnFind.Size = new System.Drawing.Size(68, 16);
			//m_btnFind.Location = new Point(110, 44);
            m_btnFind.Bounds = new Rectangle(DpiHelper.Scale(110), DpiHelper.Scale(44),
                                            DpiHelper.Scale(68), DpiHelper.Scale(16));
			this.m_btnFind.Text = "Find Next";
			this.m_btnFind.Click += new EventHandler(m_btnFind_Click);

			m_btnClose = new Button();
			this.m_btnClose.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			//this.m_btnClose.Size = new System.Drawing.Size(68, 16);
			//m_btnClose.Location = new Point(110, 64);
            m_btnClose.Bounds = new Rectangle(DpiHelper.Scale(110), DpiHelper.Scale(64),
                                               DpiHelper.Scale(68), DpiHelper.Scale(16));
			this.m_btnClose.Text = "Close";
			m_btnClose.Click += new EventHandler(m_btnClose_Click);


			this.m_tbFind.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			//this.m_tbFind.Size = new System.Drawing.Size(172, 21);
			//m_tbFind.Location = new Point(6, 20);
            m_tbFind.Bounds = new Rectangle(DpiHelper.Scale(6), DpiHelper.Scale(20),
                                             DpiHelper.Scale(172), DpiHelper.Scale(21));
			this.m_tbFind.Text = "";

			this.m_lblFind.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			//this.m_lblFind.Size = new System.Drawing.Size(64, 16);
			//m_lblFind.Location = new Point(6, 4);
            m_lblFind.Bounds = new Rectangle(DpiHelper.Scale(6), DpiHelper.Scale(4),
                                            DpiHelper.Scale(64), DpiHelper.Scale(16));
			this.m_lblFind.Text = "Find what:";

			this.m_cbWord.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			//this.m_cbWord.Location = new System.Drawing.Point(6, 44);
			//this.m_cbWord.Size = new System.Drawing.Size(100, 16);
            m_cbWord.Bounds = new Rectangle(DpiHelper.Scale(6), DpiHelper.Scale(44),
                                            DpiHelper.Scale(100), DpiHelper.Scale(16));
			this.m_cbWord.Text = "Whole word";

			this.m_cbCase.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			//this.m_cbCase.Location = new System.Drawing.Point(6, 64);
			//this.m_cbCase.Size = new System.Drawing.Size(100, 16);
            m_cbCase.Bounds = new Rectangle(DpiHelper.Scale(6), DpiHelper.Scale(64),
                                            DpiHelper.Scale(100), DpiHelper.Scale(16));
			this.m_cbCase.Text = "Match case";

			m_cbCase.Parent = this.ContentPanel;
			m_cbWord.Parent = this.ContentPanel;
			m_tbFind.Parent = this.ContentPanel;
			m_lblFind.Parent = this.ContentPanel;
			m_btnClose.Parent = this.ContentPanel;
			m_btnFind.Parent = this.ContentPanel;
		}



		#endregion

		#region Event handlers
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			Refresh();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		void m_btnClose_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

		void m_btnFind_Click(object sender, EventArgs e)
		{
			if (!Find())
			{
				MessageBox.Show("No more matches found", "Find Results");
			}
		}
		#endregion

		#region Search functions
		protected Regex CreateRegex()
		{
			string searchPattern = m_tbFind.Text;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (m_cbWord.Checked)
			{
				sb.Append(@"\b");
			}

			sb.Append(searchPattern);

			if (m_cbWord.Checked)
			{
				sb.Append(@"\b");
			}

			string fullSearchPattern = sb.ToString();

			RegexOptions ro = new RegexOptions();

			if (!m_cbCase.Checked)
			{
				ro |= RegexOptions.IgnoreCase;
			}

			return new Regex(fullSearchPattern, ro);
		}

		protected bool Find()
		{
			return Find(true);
		}

		protected bool Find(bool focusText)
		{
			// Start the search at the current location
			string textToSearch = string.Empty;
			string searchPattern = m_tbFind.Text;
			int startingSearchIndex = 0;

			/*
			 * 1) Check on whether it's a new search or an old search:
			 *    A) If it's a new search, save the new search string and search
			 *       from the selection start to the end of the text
			 *    B) If it's an old search, search from the end of the current 
			 *       selection to the end of the text
			 */
			if (m_previousSearchPattern != searchPattern)
			{
				m_previousSearchPattern = searchPattern;

				textToSearch = TargetTextbox.Text.Substring(TargetTextbox.SelectionStart);
				startingSearchIndex = TargetTextbox.SelectionStart;
			}
			else
			{
				int selectionEnd = TargetTextbox.SelectionStart + TargetTextbox.SelectionLength;
				textToSearch = TargetTextbox.Text.Substring(selectionEnd);
				startingSearchIndex = selectionEnd;
			}

			// 2) Create the regular expression based on the search text and options
			Regex searchRegex = CreateRegex();

			// 3) Run a search for the next single match
			Match match = searchRegex.Match(textToSearch);//Regex.Match(textToSearch, searchPattern);


			// 4) If something's found, highlight it.  Otherwise, "No more matches".
			if (match.Success)
			{
				if (focusText)
				{
					TargetTextbox.Focus();
				}

				TargetTextbox.Select(startingSearchIndex + match.Index, match.Length);
				TargetTextbox.ScrollToCaret();
			}
			else
			{
				TargetTextbox.SelectionStart = 0;
				TargetTextbox.SelectionLength = 0;
			}

			return match.Success;
		}
		#endregion
	}
}
