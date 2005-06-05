using System;
using System.Drawing;
using System.Windows.Forms;


namespace ISquared.PocketHTML
{

	class OptionsPanel : System.Windows.Forms.Panel
	{
		private Button buttonOK;
		private Button buttonCancel;
		private OptionsThingy ot;
		private bool ok;
		
		/// <summary>
		/// Property Ok (bool)
		/// </summary>
		public bool OK
		{
			get
			{
				return this.ok;
			}
		}
		

		public OptionsPanel()
		{
			Initialize();
			//DrawHeader();
		}

		private void Initialize()
		{
			buttonOK = new Button();
			this.buttonOK.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold);
			this.buttonOK.Location = new System.Drawing.Point(155, 2);
			this.buttonOK.Size = new System.Drawing.Size(30, 20);
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new EventHandler(buttonOK_Click);
			this.Controls.Add(buttonOK);
			// 
			// buttonCancel
			// 
			buttonCancel = new Button();
			this.buttonCancel.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold);
			this.buttonCancel.Location = new System.Drawing.Point(188, 2);
			this.buttonCancel.Size = new System.Drawing.Size(50, 20);
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click +=new EventHandler(buttonCancel_Click);
			this.Controls.Add(buttonCancel);					
		}

		// HACK This is ugly.  There's gotta be a better way.  Oh, and no 240x320, either.
		internal void Load(PocketHTMLEditor phe)
		{
			ot = new OptionsThingy(this, phe);
			ot.Location = new Point(0, 24);
			ot.Size = new Size(240, 200);
			ot.Show();
		}
		
		internal void Reset()
		{
			ot.ResetValues();
			ot.ResetCheckboxes();			
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			Graphics g = e.Graphics;
			SolidBrush sb2 = new SolidBrush(Color.White);
			Rectangle r = new Rectangle(8, 2, 120, 22);
			SolidBrush sb = new SolidBrush(Color.Blue);
			Font titleFont = new Font("Tahoma", 10, FontStyle.Bold);
			g.DrawString("Options", titleFont, sb, r);
			Pen p = new Pen(Color.Black);
			g.DrawLine(p, 0, 23, 240, 23);
		}
/*
		internal void ResizePanel(Microsoft.WindowsCE.Forms.InputPanel inputPanel1)
		{

		}
		*/

		private void buttonOK_Click(object sender, EventArgs e)
		{
			string[] cbv = ot.CurrentButtonValues;			
            
			for(int i = 0; i < cbv.Length; i++)
			{
				int nameval = i + 1;
				string buttonname = "button" + nameval.ToString();
				ot.Config.SetValue("Button Tags", buttonname, cbv[i]);
			}

			for(int i = 0; i < ot.BoolNames.Length; i++)
			{
				string boolvalue = ot.CheckBoxes[i].Checked.ToString();
				string name = ot.BoolNames[i];
				ot.Config.SetValue("Options", name, boolvalue);
			}

			int buttonNumber = (int)ot.HardwareButtonNumber.Value;
			string sButtonNumber = "Hardware" + buttonNumber.ToString();
			ot.Config.SetValue("Options", "HardwareButton", sButtonNumber);


			this.ok = true;
			((PocketHTMLEditor)Parent).MenuToolsOptions_Click(sender, e);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.ok = false;
			((PocketHTMLEditor)Parent).MenuToolsOptions_Click(sender, e);
		}
	}
}