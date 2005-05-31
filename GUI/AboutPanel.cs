using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for AboutDialog.
	/// </summary>
	public class AboutPanel : System.Windows.Forms.Panel
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label m_labVersion;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
	
		public AboutPanel()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			m_labVersion.Text = Utility.AssemblyVersion();


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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.m_labVersion = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(64, 0);
			this.label1.Size = new System.Drawing.Size(112, 16);
			this.label1.Text = "ISquared Software";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(64, 16);
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.Text = "PocketHTML.Net";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// m_labVersion
			// 
			this.m_labVersion.Location = new System.Drawing.Point(64, 32);
			this.m_labVersion.Size = new System.Drawing.Size(112, 16);
			this.m_labVersion.Text = "Version 1.1";
			this.m_labVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(28, 8);
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(84, 192);
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, 48);
			this.label6.Size = new System.Drawing.Size(232, 16);
			this.label6.Text = "http://www.isquaredsoftware.com";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label7.Location = new System.Drawing.Point(0, 68);
			this.label7.Size = new System.Drawing.Size(240, 120);
			this.label7.Text = @"PocketHTML.Net is shareware / donationware.  Loosely translated, that means that if you like this program and continue to use it, I'd appreciate it if you registered it for $5.  But, since I hate reg keys and crippleware, this program is fully functional and no registration is required.  If you like it enough to send more than $5, I won't complain too much :) See the website for registration instructions.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// AboutPanel
			// 
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.m_labVersion);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Text = "AboutDialog";

		}
		#endregion

		private void button1_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

	}
}
