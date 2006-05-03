using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ISquared.Windows.Forms;

namespace ISquared.Windows.Forms
{
	public class ReplaceDialog : FindDialog
	{
		private System.Windows.Forms.TextBox m_tbReplace;
		private System.Windows.Forms.Label m_lblReplace;
		private System.Windows.Forms.Button m_btnReplace;
		private System.Windows.Forms.Button m_btnReplaceAll;

		public string ReplaceText
		{
			get
			{
				return m_tbReplace.Text;
			}
			set
			{
				m_tbReplace.Text = value;
				m_tbReplace.Focus();
			}
		}

		public ReplaceDialog(TextBox tb)
			: base(tb)
		{
			InitializeComponent();
			this.Size = new Size(204, 126);
			this.Caption = "Replace";

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
			this.m_tbReplace = new System.Windows.Forms.TextBox();
			this.m_lblReplace = new System.Windows.Forms.Label();
			this.m_btnReplace = new System.Windows.Forms.Button();
			this.m_btnReplaceAll = new System.Windows.Forms.Button();

			// 
			// buttonClose
			// 
			this.m_btnClose.Location = new System.Drawing.Point(132, 76);

			// 
			// buttonFind
			// 
			this.m_btnFind.Location = new System.Drawing.Point(132, 12);
			// 
			// cbCase
			// 
			this.m_cbCase.Location = new System.Drawing.Point(8, 88);
			// 
			// cbWord
			// 
			this.m_cbWord.Location = new System.Drawing.Point(8, 72);
			// 
			// tbFind
			// 
			this.m_tbFind.Location = new System.Drawing.Point(8, 12);
			this.m_tbFind.Size = new System.Drawing.Size(120, 21);
			// 
			// findLabel
			// 
			this.m_lblFind.Location = new System.Drawing.Point(8, 0);

			// 
			// tbReplace
			// 
			this.m_tbReplace.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.m_tbReplace.Location = new System.Drawing.Point(8, 48);
			this.m_tbReplace.Size = new System.Drawing.Size(120, 21);
			this.m_tbReplace.Text = "";
			// 
			// replaceLabel
			// 
			this.m_lblReplace.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.m_lblReplace.Location = new System.Drawing.Point(8, 36);
			this.m_lblReplace.Size = new System.Drawing.Size(64, 16);
			this.m_lblReplace.Text = "Replace";
			// 
			// buttonReplace
			// 
			this.m_btnReplace.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.m_btnReplace.Location = new System.Drawing.Point(132, 32);
			this.m_btnReplace.Size = new System.Drawing.Size(68, 16);
			this.m_btnReplace.Text = "Replace";
			this.m_btnReplace.Click += new EventHandler(buttonReplace_Click);
			// 
			// buttonReplaceAll
			// 
			this.m_btnReplaceAll.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.m_btnReplaceAll.Location = new System.Drawing.Point(132, 54);
			this.m_btnReplaceAll.Size = new System.Drawing.Size(68, 16);
			this.m_btnReplaceAll.Text = "Replace All";
			this.m_btnReplaceAll.Click += new EventHandler(buttonReplaceAll_Click);

			// 
			// ReplaceDialog
			// 


			this.m_btnReplaceAll.Parent = this.ContentPanel;
			this.m_btnReplace.Parent = this.ContentPanel;
			this.m_tbReplace.Parent = this.ContentPanel;
			this.m_lblReplace.Parent = this.ContentPanel;
		}
		#endregion


		private void buttonReplace_Click(object sender, EventArgs e)
		{
			if (TargetTextbox == null)
			{
				return;
			}

			if (TargetTextbox.SelectionLength > 1)
			{
				TargetTextbox.SelectedText = this.m_tbReplace.Text;
				TargetTextbox.Modified = true;
			}

			Find();
		}

		private void buttonReplaceAll_Click(object sender, EventArgs e)
		{
			int counter = 0;
			Regex searchRegex = CreateRegex();
			string replacementText = m_tbReplace.Text;

			string textToSearch = string.Empty;


			if (TargetTextbox.SelectionLength > 1)
			{
				MatchCollection mc = searchRegex.Matches(TargetTextbox.SelectedText);
				TargetTextbox.SelectedText = searchRegex.Replace(TargetTextbox.SelectedText, replacementText);
				counter = mc.Count;
			}
			else
			{
				while (Find(false))
				{
					if (TargetTextbox.SelectionLength > 1)
					{
						TargetTextbox.SelectedText = replacementText;
					}
					counter++;
				}
			}

			MessageBox.Show(counter.ToString() + " item(s) replaced", "Replace Results");
		}
	}
}
