using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for AboutDialog.
	/// </summary>
	public class AboutPanel : System.Windows.Forms.Panel
	{
		private System.Windows.Forms.Label m_lblISquared;
		private System.Windows.Forms.Label m_lblPocketHTML;
		private System.Windows.Forms.Label m_lblVersion;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label m_lblMessage;
		private System.Windows.Forms.Label m_lblURL;
	
		public AboutPanel()
		{
			InitializeComponent();
				
			Assembly asm = Assembly.GetExecutingAssembly();
			string logoName = "PocketHTML.Net.Graphics.PocketHTML.png";
			Stream stream = asm.GetManifestResourceStream(logoName);
			Bitmap bmp = new Bitmap(stream);
			
			pictureBox1.Image = bmp;
			string buildNumber = Utility.AssemblyVersion();
			m_lblVersion.Text = PocketHTMLEditor.VersionText +"\r\n" + buildNumber;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutPanel));
			this.m_lblISquared = new System.Windows.Forms.Label();
			this.m_lblPocketHTML = new System.Windows.Forms.Label();
			this.m_lblVersion = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button1 = new System.Windows.Forms.Button();
			this.m_lblURL = new System.Windows.Forms.Label();
			this.m_lblMessage = new System.Windows.Forms.Label();
			// 
			// m_lblISquared
			// 
			this.m_lblISquared.Location = new System.Drawing.Point(64, 0);
			this.m_lblISquared.Size = new System.Drawing.Size(112, 16);
			this.m_lblISquared.Text = "ISquared Software";
			this.m_lblISquared.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.m_lblPocketHTML.Location = new System.Drawing.Point(64, 16);
			this.m_lblPocketHTML.Size = new System.Drawing.Size(112, 16);
			this.m_lblPocketHTML.Text = "PocketHTML.Net";
			this.m_lblPocketHTML.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// m_lblVersion
			// 
			this.m_lblVersion.Location = new System.Drawing.Point(64, 32);
			this.m_lblVersion.Size = new System.Drawing.Size(112, 32);
			this.m_lblVersion.Text = "Version 1.1";
			this.m_lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// pictureBox1
			// 
			//this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("PocketHTML")));
			this.pictureBox1.Location = new System.Drawing.Point(28, 8);
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(84, 208);
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// m_lblURL
			// 
			this.m_lblURL.Location = new System.Drawing.Point(0, 64);
			this.m_lblURL.Size = new System.Drawing.Size(232, 16);
			this.m_lblURL.Text = "http://www.isquaredsoftware.com";
			this.m_lblURL.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// m_lblMessage
			// 
			this.m_lblMessage.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.m_lblMessage.Location = new System.Drawing.Point(0, 84);
			this.m_lblMessage.Size = new System.Drawing.Size(240, 120);
			this.m_lblMessage.Text = @"PocketHTML.Net is donationware.  Loosely translated, that means that if you like this program and continue to use it, I'd appreciate it if you registered it for $5.  But, since I hate reg keys and crippleware, this program is fully functional and no registration is required.  Of course, if you like it enough to send more than $5, that would be appreciated too. See the website for donation instructions.";
			this.m_lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// AboutPanel
			// 
			this.Controls.Add(this.m_lblMessage);
			this.Controls.Add(this.m_lblURL);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.m_lblVersion);
			this.Controls.Add(this.m_lblPocketHTML);
			this.Controls.Add(this.m_lblISquared);

		}
		#endregion

		private void button1_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

	}
}
