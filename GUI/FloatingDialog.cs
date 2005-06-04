using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ISquared.Win32Interop.WinEnums;
using System.Text.RegularExpressions;

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for FindDialog.
	/// </summary>
	public class FloatingDialog : System.Windows.Forms.Form
	{
		//private System.Windows.Forms.Panel panel1;
		protected TitleBarPanel titlePanel;
		internal System.Windows.Forms.Panel backPanel;
		protected PocketHTMLEditor app;
		protected Bitmap overlay;
		protected ImageAttributes imgattr;
		protected Pen penblack;
		protected bool moving;
		
		protected System.Windows.Forms.CheckBox cbCase;
		protected System.Windows.Forms.CheckBox cbWord;
		protected TextBox tbFind;
		protected Button buttonFind;
		protected Button buttonClose;
		
		protected System.Windows.Forms.Label titleLabel;

		public TextBox FindTextBox
		{
			get
			{
				return this.tbFind;
			}
		}	

		public FloatingDialog(PocketHTMLEditor phe)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Parent = phe;
			this.app = phe;
			this.titlePanel = new TitleBarPanel();
			titleLabel.Top = 0;
			this.titlePanel.Controls.Add(titleLabel);
			this.titlePanel.Bounds = new Rectangle(0, 0, this.Width, 16);
			this.titlePanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.Controls.Add(titlePanel);
			moving = false;
			penblack = new Pen(Color.Black);
			
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
			this.titleLabel = new System.Windows.Forms.Label();
			this.backPanel = new System.Windows.Forms.Panel();

			// 
			// label2
			// 
			this.titleLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.titleLabel.ForeColor = System.Drawing.Color.White;
			this.titleLabel.Location = new System.Drawing.Point(4, 4);
			this.titleLabel.Size = new System.Drawing.Size(100, 16);
			this.titleLabel.Text = "Find";
			// 
			// backPanel
			// 
			this.backPanel.Location = new System.Drawing.Point(1, 16);
			this.backPanel.Size = new System.Drawing.Size(this.Width, 83);

			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

			this.Controls.Add(this.backPanel);
			this.titleLabel.Parent = this.titlePanel;


			this.buttonFind = new Button();
			this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);

			this.buttonClose = new Button();
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			
			this.tbFind = new TextBox();

		}
		#endregion

		private void buttonFind_Click(object sender, System.EventArgs e)
		{
			int index = 0;
			bool doFind = true;
			bool bRet = false;
			int ss = app.TextBox.SelectionStart;
			int sl = app.TextBox.SelectionLength;
			int tl = app.TextBox.Text.Length;
			if(! ( (ss + sl) >= (tl - 1)))
			{
				index = app.TextBox.SelectionStart + app.TextBox.SelectionLength;
			}
			else
			{
				doFind = false;
			}

			if(doFind)
			{
				Regex findregex = this.CreateRegex(tbFind.Text, cbCase.Checked, 
					cbWord.Checked);
				bRet = app.Find(app.TextBox.Text, index, findregex, true);
			}

			if(!bRet)
			{
				app.TextBox.SelectionStart = 0;
				app.TextBox.SelectionLength = 0;
				MessageBox.Show("No more matches found", "Find Results");
			}
		}

		protected void buttonClose_Click(object sender, System.EventArgs e)
		{
			this.Hide();			
		}	

		protected Regex CreateRegex(string searchString, bool mcase, bool word)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if(word)
			{
				sb.Append(@"\b");
			}

			sb.Append(searchString);

			if(word)
			{
				sb.Append(@"\b");
			}

			RegexOptions ro = new RegexOptions();

			if(!mcase)
			{
				ro |= RegexOptions.IgnoreCase;
			}

			Regex findregex = new Regex(sb.ToString(), ro);

			return findregex;
		}

		internal void Moving(bool m)
		{
			moving = m;

			backPanel.Visible = !m;
		}
	
		protected override void OnPaint(PaintEventArgs e)
		{			
			Graphics g = e.Graphics;

			if(!titlePanel.mouseDown)
			{
				base.OnPaint (e);
			}		
			
			Rectangle r = this.ClientRectangle;
			r.Height -= 1;
			r.Width -=1;
			g.DrawRectangle(penblack, r);	

		}
	}


	public class TitleBarPanel : System.Windows.Forms.Panel
	{
		#region working code
		
		internal bool mouseDown=false;
		internal Point oldPoint=Point.Empty;
		
		/// <summary>
		/// Property OldPoint (Point)
		/// </summary>
		public Point OldPoint
		{
			get
			{
				return this.oldPoint;
			}
			set
			{
				this.oldPoint = value;
			}
		}		
 

		override protected void OnMouseDown(MouseEventArgs e)
		{
			mouseDown=true;
			oldPoint=new Point(e.X,e.Y);
			((FloatingDialog)this.Parent).Moving(true);
		}

		override protected void OnMouseMove(MouseEventArgs e)
		{
			if (mouseDown)
			{
				int dx,dy;
				dx=e.X-oldPoint.X;
				dy=e.Y-oldPoint.Y;				

				this.Parent.Location=new Point(this.Parent.Left+dx,this.Parent.Top+dy);
							
				this.Parent.Parent.Refresh();
			}
		}

		override protected void OnMouseUp(MouseEventArgs e)
		{
			mouseDown=false;
			((FloatingDialog)this.Parent).Moving(false);
			this.Parent.Refresh();
		}
	
		#endregion
	}
}

