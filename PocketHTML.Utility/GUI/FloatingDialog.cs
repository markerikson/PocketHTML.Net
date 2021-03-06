#region using directives
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
#endregion

namespace ISquared.Windows.Forms
{
	public class FloatingDialog : System.Windows.Forms.Form
	{
		#region members
		protected TitleBarPanel m_pnlTitle;
		protected Panel m_pnlContent;
		protected Pen m_penBlack;
		protected bool m_moving;

		private Form m_frmParent;
		#endregion

		#region properties
		public Form ParentForm
		{
			get
			{
				return m_frmParent;
			}
			set
			{
				m_frmParent = value;
			}
		}

		public string Caption
		{
			get
			{
				return m_pnlTitle.Caption;
			}
			set
			{
				m_pnlTitle.Caption = value;
			}
		}

		public Panel ContentPanel
		{
			get
			{
				return m_pnlContent;
			}
		}
		#endregion

		#region Constructor
		public FloatingDialog()
		{
			InitializeComponent();
			this.Text = "";

			this.FormBorderStyle = FormBorderStyle.None;
			this.ControlBox = false;
			this.MinimizeBox = false;
			this.MaximizeBox = false;

			m_pnlTitle.ParentDialog = this;

			m_moving = false;
			m_penBlack = new Pen(Color.Black);
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
			this.m_pnlContent = new System.Windows.Forms.Panel();
			this.m_pnlTitle = new TitleBarPanel();
			this.Controls.Add(m_pnlTitle);

			m_pnlTitle.ParentDialog = this;
            int scaled16 = DpiHelper.Scale(16);
            this.m_pnlTitle.Bounds = new Rectangle(0, 0, this.Width, scaled16);
			// 
			// backPanel
			// 
            this.m_pnlContent.Location = new System.Drawing.Point(1, scaled16);
			this.m_pnlContent.Size = new System.Drawing.Size(this.Width - 2, DpiHelper.Scale(83));
			Button b = new Button();




			this.Controls.Add(this.m_pnlContent);


			/*
			this.buttonFind = new Button();
			this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);

			this.buttonClose = new Button();
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			*/
		}
		#endregion

		#endregion

		#region Event handlers
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.Text = "";
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (m_pnlTitle != null)
			{
				m_pnlTitle.Bounds = new Rectangle(0, 0, this.Width, DpiHelper.Scale(16));
			}

			if (m_pnlContent != null)
			{
				m_pnlContent.Bounds = new Rectangle(1, DpiHelper.Scale(16), this.Width - 2, this.Height - m_pnlTitle.Height - 2);
			}

			//DpiHelper.AdjustAllControls(this);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			if (!m_pnlTitle.mouseDown)
			{
				base.OnPaint(e);
			}

			Rectangle r = this.ClientRectangle;
			r.Height -= 1;
			r.Width -= 1;
			g.DrawRectangle(m_penBlack, r);

		}
		#endregion

	}
}

