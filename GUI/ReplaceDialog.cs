using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for ReplaceDialog.
	/// </summary>
	public class ReplaceDialog : FloatingDialog
	{
		private System.Windows.Forms.Label findLabel;
		private System.Windows.Forms.TextBox tbReplace;
		private System.Windows.Forms.Label replaceLabel;
		private System.Windows.Forms.Button buttonReplace;
		private System.Windows.Forms.Button buttonReplaceAll;
	
		public ReplaceDialog(PocketHTMLEditor phe) : base(phe)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.titleLabel.Text = "Replace";
			this.backPanel.Height = this.Height - 16 - 1;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Size = new Size(204, 126);
			this.cbCase = new System.Windows.Forms.CheckBox();
			this.cbWord = new System.Windows.Forms.CheckBox();
			this.findLabel = new System.Windows.Forms.Label();
			this.tbReplace = new System.Windows.Forms.TextBox();
			this.replaceLabel = new System.Windows.Forms.Label();
			this.buttonReplace = new System.Windows.Forms.Button();
			this.buttonReplaceAll = new System.Windows.Forms.Button();
			
			// 
			// buttonClose
			// 
			this.buttonClose.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.buttonClose.Location = new System.Drawing.Point(132, 76);
			this.buttonClose.Size = new System.Drawing.Size(68, 16);
			this.buttonClose.Text = "Close";
			// 
			// buttonFind
			// 
			this.buttonFind.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.buttonFind.Location = new System.Drawing.Point(132, 12);
			this.buttonFind.Size = new System.Drawing.Size(68, 16);
			this.buttonFind.Text = "Find Next";
			// 
			// cbCase
			// 
			this.cbCase.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.cbCase.Location = new System.Drawing.Point(8, 88);
			this.cbCase.Size = new System.Drawing.Size(100, 16);
			this.cbCase.Text = "Match case";
			// 
			// cbWord
			// 
			this.cbWord.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.cbWord.Location = new System.Drawing.Point(8, 72);
			this.cbWord.Size = new System.Drawing.Size(100, 16);
			this.cbWord.Text = "Whole word";
			// 
			// tbFind
			// 
			this.tbFind.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.tbFind.Location = new System.Drawing.Point(8, 12);
			this.tbFind.Size = new System.Drawing.Size(120, 21);
			this.tbFind.Text = "";
			// 
			// findLabel
			// 
			this.findLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.findLabel.Location = new System.Drawing.Point(8, 0);
			this.findLabel.Size = new System.Drawing.Size(64, 16);
			this.findLabel.Text = "Find what:";
			// 
			// backPanel
			// 

			// 
			// tbReplace
			// 
			this.tbReplace.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.tbReplace.Location = new System.Drawing.Point(8, 48);
			this.tbReplace.Size = new System.Drawing.Size(120, 21);
			this.tbReplace.Text = "";
			// 
			// replaceLabel
			// 
			this.replaceLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.replaceLabel.Location = new System.Drawing.Point(8, 36);
			this.replaceLabel.Size = new System.Drawing.Size(64, 16);
			this.replaceLabel.Text = "Replace";
			// 
			// buttonReplace
			// 
			this.buttonReplace.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.buttonReplace.Location = new System.Drawing.Point(132, 32);
			this.buttonReplace.Size = new System.Drawing.Size(68, 16);
			this.buttonReplace.Text = "Replace";
			this.buttonReplace.Click += new EventHandler(buttonReplace_Click);
			// 
			// buttonReplaceAll
			// 
			this.buttonReplaceAll.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.buttonReplaceAll.Location = new System.Drawing.Point(132, 54);
			this.buttonReplaceAll.Size = new System.Drawing.Size(68, 16);
			this.buttonReplaceAll.Text = "Replace All";
			this.buttonReplaceAll.Click +=new EventHandler(buttonReplaceAll_Click);

			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			// 
			// ReplaceDialog
			// 

			this.backPanel.Location = new System.Drawing.Point(1, 16);
			this.backPanel.Size = new System.Drawing.Size(this.Width - 2, 140);

			this.buttonReplaceAll.Parent = this.backPanel;
			this.buttonReplace.Parent = this.backPanel;
			this.tbReplace.Parent = this.backPanel;
			this.replaceLabel.Parent = this.backPanel;
			this.buttonClose.Parent = this.backPanel;
			this.buttonFind.Parent = this.backPanel;
			this.tbFind.Parent = this.backPanel;
			this.findLabel.Parent = this.backPanel;
			this.cbWord.Parent = this.backPanel;
			this.cbCase.Parent = this.backPanel;
		}
		#endregion

		private void buttonFind_Click(object sender, System.EventArgs e)
		{
			Regex findregex = this.CreateRegex(tbFind.Text, cbCase.Checked, 
				cbWord.Checked);
			int index = app.TextBox.SelectionStart + app.TextBox.SelectionLength;
			app.Find(app.TextBox.Text, index, findregex, true);
		}
	
		new private void buttonClose_Click(object sender, System.EventArgs e)
		{
			this.Hide();			
		}

		private void buttonReplace_Click(object sender, EventArgs e)
		{
			TextBox tb = app.TextBox;
			if(tb.SelectionLength > 1)
			{
				tb.SelectedText = this.tbReplace.Text;
			}
			tb.Modified = true;
		}

		private void buttonReplaceAll_Click(object sender, EventArgs e)
		{
			TextBox tb = app.TextBox;
			int counter = 0;

			Regex findregex = this.CreateRegex(tbFind.Text, cbCase.Checked, 
												cbWord.Checked);
			
			if(tb.SelectionLength > 1)
			{
				MatchCollection mc = findregex.Matches(tb.SelectedText);
				tb.SelectedText = findregex.Replace(tb.SelectedText, tbReplace.Text);
				counter = mc.Count;
			}
			else
			{
				while(app.Find(tb.Text, tb.SelectionStart, 
								findregex, false) )
				{
					if(tb.SelectionLength > 1)
					{
						tb.SelectedText = this.tbReplace.Text;
					}
					counter++;
				}
			}
			

			MessageBox.Show(counter.ToString() + " items replaced", "Replace Results");			
			
		}
	}
}
