using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;


namespace ISquared.PocketHTML
{

	class OptionsPanel : System.Windows.Forms.Panel
	{
		private Button buttonOK;
		private Button buttonCancel;
		//private OptionsThingy ot;
		private DialogResult m_result;

		private Hashtable m_tagHash;
		private Ayende.Configuration config;
		private PocketHTMLEditor maineditor;
		private string[] currentButtonValues;
		private string[] boolnames;

		private Panel m_innerPanel;

		#region Options dialog controls
		
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private CheckBox cbAutoIndent;
		private CheckBox cbIndentHTML;
		private CheckBox cbClearType;
		private CheckBox cbXHTML;
		private CheckBox cbPageWrap;
		private CheckBox cbTextWrap;
		private ComboBox m_comboButtonNumber;
		private Label label5;
		private Label label4;
		private Label label2;
		private Label label1;
		private ComboBox m_comboButtonLabel;
		private CheckBox cbMonospacedFont;
		private Label label6;
		private Label label3;
		private NumericUpDown nudHardwareButton;
		private CheckBox cbHardwareButtons;
		private CheckBox[] boxes;
		private ComboBox m_comboZoomLevel;
		private Label m_lblZoomLevel;
		#endregion

		public DialogResult Result
		{
			get
			{
				return m_result;
			}
		}

		public NumericUpDown HardwareButtonNumber
		{
			get { return nudHardwareButton; }
			set { nudHardwareButton = value; }
		}

		internal String[] CurrentButtonValues
		{
			get
			{
				return currentButtonValues;
			}
		}

		internal Ayende.Configuration Config
		{
			get
			{
				return config;
			}
		}

		internal String[] BoolNames
		{
			get
			{
				return boolnames;
			}
		}

		internal CheckBox[] CheckBoxes
		{
			get
			{
				return boxes;
			}
		}
		

		public OptionsPanel(PocketHTMLEditor parent)
		{
			InitializeHeader();
			InitializeOptionsControls();

			maineditor = parent;

			DpiHelper.AdjustAllControls(this);

			this.config = PocketHTMLEditor.Config;
			maineditor = parent;

			currentButtonValues = new string[16];

			m_tagHash = parent.TagHash;
			ICollection ickeys = m_tagHash.Keys;
			string[] keys = new string[ickeys.Count];
			ickeys.CopyTo(keys, 0);
			Array.Sort(keys);

			foreach (string s in keys)
			{
				m_comboButtonLabel.Items.Add(s);
			}

			// TODO This is hardcoded.  Fix it somehow?
			boolnames = new string[]{"XHTMLTags", "TextWrap", "PageWrap", "ClearType",
										"IndentHTML", "AutoIndent", "HardwareButtonShowsMenu",
										"MonospacedFont"};
			boxes = new CheckBox[]{cbXHTML, cbTextWrap, cbPageWrap, cbClearType, 
									cbIndentHTML, cbAutoIndent, cbHardwareButtons,
									cbMonospacedFont};

			//ResetCheckboxes();
			//ResetValues();
			Reset();

		}

		private void InitializeHeader()
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

		private void InitializeOptionsControls()
		{
			m_innerPanel = new Panel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.cbMonospacedFont = new System.Windows.Forms.CheckBox();
			this.cbAutoIndent = new System.Windows.Forms.CheckBox();
			this.cbIndentHTML = new System.Windows.Forms.CheckBox();
			this.cbClearType = new System.Windows.Forms.CheckBox();
			this.cbXHTML = new System.Windows.Forms.CheckBox();
			this.cbPageWrap = new System.Windows.Forms.CheckBox();
			this.cbTextWrap = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.nudHardwareButton = new System.Windows.Forms.NumericUpDown();
			this.cbHardwareButtons = new System.Windows.Forms.CheckBox();
			this.m_comboButtonNumber = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.m_comboButtonLabel = new System.Windows.Forms.ComboBox();
			m_comboZoomLevel = new ComboBox();
			m_lblZoomLevel = new Label();

			this.ClientSize = new System.Drawing.Size(240, 294);

			m_innerPanel.Location = new Point(2, 30);
			m_innerPanel.Size = new Size(240, 240);
			m_innerPanel.Parent = this;

			tabControl1.Parent = m_innerPanel;
			tabControl1.Bounds = m_innerPanel.ClientRectangle;
			
			//this.tabControl1.Controls.Add(this.tabPage1);
			//this.tabControl1.Controls.Add(this.tabPage2);

			tabPage1.Parent = tabControl1;
			tabPage2.Parent = tabControl1;
			//this.Controls.Add(this.tabControl1);
			//m_innerPanel.Controls.Add(tabControl1);


			this.tabControl1.SelectedIndex = 0;
			

			cbMonospacedFont.Parent = tabPage1;
			cbAutoIndent.Parent = tabPage1;
			cbIndentHTML.Parent = tabPage1;
			cbClearType.Parent = tabPage1;
			cbXHTML.Parent = tabPage1;
			cbPageWrap.Parent = tabPage1;
			cbTextWrap.Parent = tabPage1;
			m_lblZoomLevel.Parent = tabPage1;
			m_comboZoomLevel.Parent = tabPage1;

			label1.Parent = tabPage2;
			label2.Parent = tabPage2;
			label3.Parent = tabPage2;
			label4.Parent = tabPage2;
			label5.Parent = tabPage2;
			label6.Parent = tabPage2;
			nudHardwareButton.Parent = tabPage2;
			m_comboButtonLabel.Parent = tabPage2;
			m_comboButtonNumber.Parent = tabPage2;
			cbHardwareButtons.Parent = tabPage2;




			// 
			// tabControl1
			// 

			
			
			
			
			// 
			// tabPage1
			// 
			
			// 
			// cbMonospacedFont
			// 
			this.cbMonospacedFont.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbMonospacedFont.Location = new System.Drawing.Point(7, 136);
			this.cbMonospacedFont.Size = new System.Drawing.Size(164, 16);
			this.cbMonospacedFont.Text = "Monospaced font";
			// 
			// cbAutoIndent
			// 
			this.cbAutoIndent.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbAutoIndent.Location = new System.Drawing.Point(7, 70);
			this.cbAutoIndent.Size = new System.Drawing.Size(136, 16);
			this.cbAutoIndent.Text = "Autoindent on Enter";
			// 
			// cbIndentHTML
			// 
			this.cbIndentHTML.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbIndentHTML.Location = new System.Drawing.Point(7, 48);
			this.cbIndentHTML.Size = new System.Drawing.Size(128, 16);
			this.cbIndentHTML.Text = "Indent HTML tags";
			// 
			// cbClearType
			// 
			this.cbClearType.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbClearType.Location = new System.Drawing.Point(7, 114);
			this.cbClearType.Size = new System.Drawing.Size(164, 16);
			this.cbClearType.Text = "Use ClearType in preview";
			// 
			// cbXHTML
			// 
			this.cbXHTML.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbXHTML.Location = new System.Drawing.Point(7, 92);
			this.cbXHTML.Size = new System.Drawing.Size(160, 16);
			this.cbXHTML.Text = "XHTML-style single tags";
			// 
			// cbPageWrap
			// 
			this.cbPageWrap.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbPageWrap.Location = new System.Drawing.Point(7, 26);
			this.cbPageWrap.Size = new System.Drawing.Size(128, 16);
			this.cbPageWrap.Text = "Wrap page preview";
			// 
			// cbTextWrap
			// 
			this.cbTextWrap.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbTextWrap.Location = new System.Drawing.Point(7, 4);
			this.cbTextWrap.Size = new System.Drawing.Size(128, 16);
			this.cbTextWrap.Text = "Wrap editor text";
			// 
			// tabPage2
			// 
			
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.label6.Location = new System.Drawing.Point(7, 4);
			this.label6.Size = new System.Drawing.Size(137, 20);
			this.label6.Text = "Button Assignments:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(33, 151);
			this.label3.Size = new System.Drawing.Size(49, 20);
			this.label3.Text = "Button:";
			// 
			// nudHardwareButton
			// 
			this.nudHardwareButton.Location = new System.Drawing.Point(88, 149);
			this.nudHardwareButton.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudHardwareButton.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudHardwareButton.Size = new System.Drawing.Size(43, 22);
			this.nudHardwareButton.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// cbHardwareButtons
			// 
			this.cbHardwareButtons.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbHardwareButtons.Location = new System.Drawing.Point(5, 129);
			this.cbHardwareButtons.Size = new System.Drawing.Size(196, 16);
			this.cbHardwareButtons.Text = "Hardware button shows tag menu";
			this.cbHardwareButtons.CheckStateChanged += new System.EventHandler(this.cbHardwareButtons_CheckStateChanged);
			// 
			// comboBox2
			// 
			this.m_comboButtonNumber.Items.Add("1");
			this.m_comboButtonNumber.Items.Add("2");
			this.m_comboButtonNumber.Items.Add("3");
			this.m_comboButtonNumber.Items.Add("4");
			this.m_comboButtonNumber.Items.Add("5");
			this.m_comboButtonNumber.Items.Add("6");
			this.m_comboButtonNumber.Items.Add("7");
			this.m_comboButtonNumber.Items.Add("8");
			this.m_comboButtonNumber.Items.Add("9");
			this.m_comboButtonNumber.Items.Add("10");
			this.m_comboButtonNumber.Items.Add("11");
			this.m_comboButtonNumber.Items.Add("12");
			this.m_comboButtonNumber.Items.Add("13");
			this.m_comboButtonNumber.Items.Add("14");
			this.m_comboButtonNumber.Items.Add("15");
			this.m_comboButtonNumber.Items.Add("16");
			this.m_comboButtonNumber.Location = new System.Drawing.Point(49, 27);
			this.m_comboButtonNumber.Size = new System.Drawing.Size(36, 22);
			this.m_comboButtonNumber.SelectedIndexChanged += new System.EventHandler(this.comboButtonNumber_SelectedIndexChanged);

			m_lblZoomLevel.Text = "Preview zoom level: ";
			m_lblZoomLevel.Bounds = new Rectangle(7, 166, 100, 16);
			m_comboZoomLevel.Items.Add("0");
			m_comboZoomLevel.Items.Add("1");
			m_comboZoomLevel.Items.Add("2");
			m_comboZoomLevel.Items.Add("3");
			m_comboZoomLevel.Items.Add("4");
			m_comboZoomLevel.Bounds = new Rectangle(110, 166, 40, 22);
			
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label5.Location = new System.Drawing.Point(93, 27);
			this.label5.Size = new System.Drawing.Size(36, 20);
			this.label5.Text = "Label:";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label4.Location = new System.Drawing.Point(5, 27);
			this.label4.Size = new System.Drawing.Size(44, 20);
			this.label4.Text = "Button:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label2.Location = new System.Drawing.Point(41, 55);
			this.label2.Size = new System.Drawing.Size(196, 73);
			this.label2.Text = "label2";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label1.Location = new System.Drawing.Point(5, 55);
			this.label1.Size = new System.Drawing.Size(32, 26);
			this.label1.Text = "Tag:";
			// 
			// comboBox1
			// 
			this.m_comboButtonLabel.Location = new System.Drawing.Point(129, 27);
			this.m_comboButtonLabel.Size = new System.Drawing.Size(108, 22);
			this.m_comboButtonLabel.SelectedIndexChanged += new System.EventHandler(this.comboButtonLabel_SelectedIndexChanged);
			// 
			// OptionsThingy
			// 
			

			/*
			this.tabPage1.Controls.Add(this.cbMonospacedFont);
			this.tabPage1.Controls.Add(this.cbAutoIndent);
			this.tabPage1.Controls.Add(this.cbIndentHTML);
			this.tabPage1.Controls.Add(this.cbClearType);
			this.tabPage1.Controls.Add(this.cbXHTML);
			this.tabPage1.Controls.Add(this.cbPageWrap);
			this.tabPage1.Controls.Add(this.cbTextWrap);
			*/

			
			//this.tabPage1.Location = new System.Drawing.Point(0, 0);
			//this.tabPage1.Size = new System.Drawing.Size(236, 240);
			this.tabPage1.Text = "Options";
			tabPage1.Bounds = tabControl1.Bounds;

			/*
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.nudHardwareButton);
			this.tabPage2.Controls.Add(this.cbHardwareButtons);
			this.tabPage2.Controls.Add(this.comboBox2);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Controls.Add(this.label4);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Controls.Add(this.label1);
			this.tabPage2.Controls.Add(this.comboBox1);
			*/

			

			this.tabPage2.Text = "QuickTags";
			tabPage2.Bounds = tabControl1.Bounds;
		}

		// HACK This is ugly.  There's gotta be a better way.  Oh, and no 240x320, either.
		/*
		internal void Load(PocketHTMLEditor phe)
		{
			ot = new OptionsThingy(this, phe);
			ot.Location = new Point(0, 24);
			ot.Size = new Size(240, 200);
			ot.Show();
		}
		*/

		internal void Reset()
		{
			//ot.ResetValues();
			//ot.ResetCheckboxes();			
			ResetValues();
			ResetCheckboxes();

			m_comboButtonNumber.SelectedIndex = 0;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			Graphics g = e.Graphics;
			SolidBrush sb2 = new SolidBrush(Color.White);
			
			Rectangle r = new Rectangle(DpiHelper.Scale(8), DpiHelper.Scale(2), DpiHelper.Scale(120), DpiHelper.Scale(22));
			SolidBrush sb = new SolidBrush(Color.Blue);
			Font titleFont = new Font("Tahoma", 10, FontStyle.Bold);
			g.DrawString("Options", titleFont, sb, r);
			Pen p = new Pen(Color.Black);
			g.DrawLine(p, 0, DpiHelper.Scale(23), DpiHelper.Scale(240), DpiHelper.Scale(23));
		}
/*
		internal void ResizePanel(Microsoft.WindowsCE.Forms.InputPanel inputPanel1)
		{

		}
		*/

		private void comboButtonLabel_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string text = (string)m_comboButtonLabel.SelectedItem;
			Tag t = (Tag)m_tagHash[text];
			label2.Text = t.StartTag;

			int idx = m_comboButtonNumber.SelectedIndex;
			currentButtonValues[idx] = text;

		}

		private void comboButtonNumber_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int num = m_comboButtonNumber.SelectedIndex;
			string refname = currentButtonValues[num];//maineditor.GetButtonString(num);
			int index = m_comboButtonLabel.Items.IndexOf(refname);
			m_comboButtonLabel.SelectedIndex = index;

		}

		private void cbHardwareButtons_CheckStateChanged(object sender, EventArgs e)
		{
			nudHardwareButton.Enabled = cbHardwareButtons.Checked;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			string[] cbv = CurrentButtonValues;			
            
			for(int i = 0; i < cbv.Length; i++)
			{
				int nameval = i + 1;
				string buttonname = "button" + nameval.ToString();
				Config.SetValue("Button Tags", buttonname, cbv[i]);
			}

			for(int i = 0; i < BoolNames.Length; i++)
			{
				string boolvalue = CheckBoxes[i].Checked.ToString();
				string name = BoolNames[i];
				Config.SetValue("Options", name, boolvalue);
			}

			int buttonNumber = (int)HardwareButtonNumber.Value;
			string sButtonNumber = "Hardware" + buttonNumber.ToString();
			Config.SetValue("Options", "HardwareButton", sButtonNumber);

			Config.SetValue("Options", "ZoomLevel", (String)m_comboZoomLevel.SelectedItem);

			m_result = DialogResult.OK;
			((PocketHTMLEditor)Parent).MenuToolsOptions_Click(sender, e);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			m_result = DialogResult.Cancel;
			((PocketHTMLEditor)Parent).MenuToolsOptions_Click(sender, e);
		}

		internal void ResetCheckboxes()
		{
			for (int i = 0; i < boolnames.Length; i++)
			{

				string name = boolnames[i];
				bool chk = config.GetBool("Options", name);
				boxes[i].Checked = chk;
			}
			//m_comboButtonNumber.SelectedIndex = 0;
		}

		internal void ResetValues()
		{
			Hashtable buttonTags = maineditor.ButtonTags;
			NamedButton[] buttons = maineditor.Buttons;
			for (int i = 0; i < currentButtonValues.Length; i++)
			{
				currentButtonValues[i] = (string)buttonTags[buttons[i].Name];
			}
			//m_comboButtonNumber.SelectedIndex = 0;

			string hardwareButtonName = config.GetValue("Options", "HardwareButton");
			string sButtonNumber = hardwareButtonName.Substring(hardwareButtonName.Length - 1);
			int buttonNumber = Int32.Parse(sButtonNumber);

			nudHardwareButton.Value = buttonNumber;

			string zoomLevel = config.GetValue("Options", "ZoomLevel");
			m_comboZoomLevel.SelectedItem = zoomLevel;

		}
	}
}