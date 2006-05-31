#region using directives
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
#endregion

namespace ISquared.PocketHTML
{
	public class AboutPanel : System.Windows.Forms.Panel
	{
		#region private members
		private System.Windows.Forms.Label m_lblISquared;
		private System.Windows.Forms.Label m_lblVersion;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label m_lblMessage;
		private System.Windows.Forms.Label m_lblURL;
        private string m_buildNumber;
		#endregion

		#region Constructor
		public AboutPanel()
		{
			InitializeComponent();

			Assembly asm = Assembly.GetExecutingAssembly();
			string logoName = "PocketHTML.Net.Graphics.PocketHTML.png";
			Stream stream = asm.GetManifestResourceStream(logoName);
			Bitmap bmp = new Bitmap(stream);
			
			pictureBox1.Image = bmp;
			m_buildNumber = Utility.AssemblyVersion();


			string aboutMessage = @"PocketHTML.Net is donationware.  Loosely translated, that means that if you like this program and continue to use it, I'd appreciate it if you registered it for $5.  But, since I hate reg keys and crippleware, this program is fully functional and no registration is required.  Of course, if you like it enough to send more than $5, that would be appreciated too. See the website for donation instructions.";
			m_lblMessage.Text = aboutMessage;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}
		#endregion

        #region Layout
        public void UpdateLayout()
        {
            if (Screen.PrimaryScreen.Bounds.Width > Screen.PrimaryScreen.Bounds.Height)
            {
                LayoutLandscape();
            }
            else
            {
                LayoutPortrait();
            }

            //DpiHelper.AdjustAllControls(this);
        }

        private void LayoutPortrait()
        {
            int scale = DpiHelper.Scale(1);

            m_lblISquared.Bounds = new Rectangle(scale * 44, scale * 0, scale * 150, scale * 32);
            m_lblVersion.Bounds = new Rectangle(scale * 44, scale * 32, scale * 150, scale * 32);
            m_lblURL.Bounds = new Rectangle(0, scale * 64, scale * 232, scale * 16);
            m_lblMessage.Bounds = new Rectangle(0, scale * 84, scale * 240, scale * 120);
            pictureBox1.Bounds = new Rectangle(scale * 8, scale * 8, scale * 32, scale * 32);

            this.button1.Location = new System.Drawing.Point(scale * 84, scale * 208);

            m_lblISquared.TextAlign = ContentAlignment.TopCenter;
            m_lblVersion.TextAlign = ContentAlignment.TopCenter;

            m_lblMessage.TextAlign = ContentAlignment.TopLeft;

            m_lblISquared.Text = "ISquared Software\r\nPocketHTML.Net";
            m_lblVersion.Text = PocketHTMLEditor.VersionText + "\r\n" + m_buildNumber;
        }

        private void LayoutLandscape()
        {
            int scale = DpiHelper.Scale(1);

            pictureBox1.Bounds = new Rectangle(scale * 8, scale * 8, scale * 32, scale * 32);
            m_lblISquared.Bounds = new Rectangle(scale * 44, scale * 0, scale * 232, scale * 16);
            m_lblVersion.Bounds = new Rectangle(scale * 44, scale * 16, scale * 232, scale * 32);
            m_lblURL.Bounds = new Rectangle(scale * 44, scale * 32, scale * 232, scale * 16);
            m_lblMessage.Bounds = new Rectangle(scale * 4, scale * 60, scale * 316, scale * 100);

            this.button1.Location = new System.Drawing.Point(scale * 128, scale * 164);


            m_lblISquared.TextAlign = ContentAlignment.TopCenter;
            m_lblVersion.TextAlign = ContentAlignment.TopCenter;

            m_lblISquared.Text = "ISquared Software PocketHTML.Net";
            m_lblVersion.Text = PocketHTMLEditor.VersionText + "  (" + m_buildNumber + ")";
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutPanel));
			this.m_lblISquared = new System.Windows.Forms.Label();
			this.m_lblVersion = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button1 = new System.Windows.Forms.Button();
			this.m_lblURL = new System.Windows.Forms.Label();
			this.m_lblMessage = new System.Windows.Forms.Label();
			// 
			// m_lblISquared
			// 
			
			this.m_lblISquared.Text = "ISquared Software";
			this.m_lblISquared.TextAlign = System.Drawing.ContentAlignment.TopCenter;

			this.m_lblVersion.Text = "Version 1.1";
			this.m_lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;

			// 
			// button1
			// 

            int scale = DpiHelper.Scale(1);
			
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.Size = new Size(scale * button1.Width, scale * button1.Height);
			// 
			// m_lblURL
			// 
			
			this.m_lblURL.Text = "http://www.isquaredsoftware.com";
			this.m_lblURL.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// m_lblMessage
			// 
			this.m_lblMessage.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.m_lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// AboutPanel
			// 

            m_lblMessage.Parent = this;
            m_lblURL.Parent = this;
            button1.Parent = this;
            pictureBox1.Parent = this;
            m_lblVersion.Parent = this;
            m_lblISquared.Parent = this;

		}
		#endregion

		#region Event handlers
		private void button1_Click(object sender, EventArgs e)
		{
			this.Hide();
		}
		#endregion

	}
}
