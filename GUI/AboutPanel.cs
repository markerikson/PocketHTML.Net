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

            DpiHelper.AdjustAllControls(this);
        }

        private void LayoutPortrait()
        {
            m_lblISquared.Bounds = new Rectangle(44, 0, 150, 32);
            m_lblVersion.Bounds = new Rectangle(44, 32, 150, 32);
            m_lblURL.Bounds = new Rectangle(0, 64, 232, 16);
            m_lblMessage.Bounds = new Rectangle(0, 84, 240, 120);
            pictureBox1.Bounds = new Rectangle(8, 8, 32, 32);

            this.button1.Location = new System.Drawing.Point(84, 208);

            m_lblISquared.TextAlign = ContentAlignment.TopCenter;
            m_lblVersion.TextAlign = ContentAlignment.TopCenter;

            m_lblISquared.Text = "ISquared Software\r\nPocketHTML.Net";
            m_lblVersion.Text = PocketHTMLEditor.VersionText + "\r\n" + m_buildNumber;
        }

        private void LayoutLandscape()
        {
            pictureBox1.Bounds = new Rectangle(8, 8, 32, 32);
            m_lblISquared.Bounds = new Rectangle(44, 0, 232, 16);
            m_lblVersion.Bounds = new Rectangle(44, 16, 232, 32);
            m_lblURL.Bounds = new Rectangle(44, 32, 232, 16);
            m_lblMessage.Bounds = new Rectangle(4, 60, 316, 100);

            this.button1.Location = new System.Drawing.Point(128, 164);


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
			
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.button1_Click);
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
