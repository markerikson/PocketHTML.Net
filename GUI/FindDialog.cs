using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using ISquared.Win32Interop.WinEnums;
using System.Text.RegularExpressions;

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for FindDialog.
	/// </summary>
	public class FindDialog : ISquared.PocketHTML.FloatingDialog
	{
		private System.Windows.Forms.Label findLabel;


		
		/// <summary>
		/// Property Word (CheckBox)
		/// </summary>
		public bool WholeWord
		{
			get
			{
				return this.cbWord.Checked;
			}
			set
			{
				this.cbWord.Checked = value;
			}
		}	

		public bool MatchCase
		{
			get
			{
				return this.cbCase.Checked;
			}
			set
			{
				this.cbCase.Checked = value;
			}
		}	
		public FindDialog(PocketHTMLEditor phe) : base(phe)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			//app = phe;

			this.Parent = phe;
			this.titleLabel.Text = "Find";
			this.backPanel.Width = this.Width - 2;
			

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.cbCase = new System.Windows.Forms.CheckBox();
			this.cbWord = new System.Windows.Forms.CheckBox();
			this.findLabel = new System.Windows.Forms.Label();

			// 
			// buttonClose
			// 
			this.buttonClose.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.buttonClose.Location = new System.Drawing.Point(110, 64);
			this.buttonClose.Size = new System.Drawing.Size(68, 16);
			this.buttonClose.Text = "Close";

			// buttonFind
			// 
			this.buttonFind.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.buttonFind.Location = new System.Drawing.Point(110, 44);
			this.buttonFind.Size = new System.Drawing.Size(68, 16);
			this.buttonFind.Text = "Find Next";
			// cbCase
			// 
			this.cbCase.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.cbCase.Location = new System.Drawing.Point(6, 64);
			this.cbCase.Size = new System.Drawing.Size(100, 16);
			this.cbCase.Text = "Match case";
			// 
			// cbWord
			// 
			this.cbWord.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.cbWord.Location = new System.Drawing.Point(6, 44);
			this.cbWord.Size = new System.Drawing.Size(100, 16);
			this.cbWord.Text = "Whole word";
			// 
			// textBox1
			// 
			this.tbFind.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.tbFind.Location = new System.Drawing.Point(6, 20);
			this.tbFind.Size = new System.Drawing.Size(172, 21);
			this.tbFind.Text = "";
			// 
			// label1
			// 
			this.findLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.findLabel.Location = new System.Drawing.Point(6, 4);
			this.findLabel.Size = new System.Drawing.Size(64, 16);
			this.findLabel.Text = "Find what:";
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			// 
			// FindDialog
			// 
			this.ClientSize = new System.Drawing.Size(184, 100);
			this.Controls.Add(this.backPanel);
			this.findLabel.Parent = this.backPanel;
			this.tbFind.Parent = this.backPanel;
			this.cbWord.Parent = this.backPanel;
			this.cbCase.Parent = this.backPanel;
			this.buttonClose.Parent = this.backPanel;
			this.buttonFind.Parent = this.backPanel;
		}
		#endregion
	}

}
