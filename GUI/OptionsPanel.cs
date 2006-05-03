#region using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Text;
#endregion

namespace ISquared.PocketHTML
{
	class OptionsPanel : System.Windows.Forms.Panel
	{
		#region private members
		private Button m_btnOK;
		private Button m_btnCancel;
		private DialogResult m_result;
		private Panel m_innerPanel;

		private Hashtable m_htTags;
		private Hashtable m_htCheckboxes;
		private Ayende.Configuration m_config;
		private PocketHTMLEditor m_mainEditor;
		private string[] m_currentButtonValues;

		private string[,] boolnames = new string[,]
		{
			{"XHTMLTags","XHTML-style single tags"},
			{"TextWrap", "Wrap editor text"},
			{"PageWrap", "Wrap page preview"},
			{"ClearType", "Use ClearType in preview"},			
			{"IndentHTML", "Indent HTML tags"},
			{"AutoIndent", "Auto-indent on Enter"},
			{"MonospacedFont", "Use monospaced font"},
		};

		private string[,] encodingnames = new string[,]
		{
			{"Arabic (Windows)",             		"windows-1256"},
			{"Baltic (ISO)",                 		"iso-8859-4"},
			{"Central European (ISO)",      		"iso-8859-2"},
			{"Chinese Simplified (GB2312)",  	"gb2312"} ,     
			{"Chinese Traditional (Big5)",   	"big5"},
			{"Cyrillic (KOI8-R)",           		"koi8-r"},          	
			{"Cyrillic (Windows)",          		"windows-1251"},
			{"Greek (ISO)",                  		"iso-8859-7"},  	
			{"Hebrew (Windows)",             		"windows-1255"},
			{"Japanese (JIS)",               		"iso-2022-jp"}, 
			{"Korean",                       		"ks_c_5601-1987"},
			{"Thai (Windows)",               		"windows-874"},
			{"Turkish (ISO)",                		"iso-8859-9"},
			{"Unicode (UTF-8)",              		"utf-8"},
			{"US-ASCII",                     		"us-ascii"},
        	{"Vietnamese (Windows)",        		"windows-1258"},
			{"Western European (ISO)",       	"iso-8859-1"} , 
		};
		#endregion

		#region Options dialog controls

		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private ComboBox m_comboButtonNumber;
		private Label label5;
		private Label label4;
		private Label label2;
		private Label label1;
		private ComboBox m_comboButtonLabel;
		
		private Label label6;
		private Label label3;
		private NumericUpDown nudHardwareButton;
		private CheckBox cbHardwareButtons;
		private ComboBox m_comboZoomLevel;
		private Label m_lblZoomLevel;
		private ListView m_lvOptions;
		private Label m_lblDefaultEncoding;
		private ComboBox m_comboDefaultEncoding;
		private Button m_btnTestEncoding;
		#endregion

		#region properties
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
				return m_currentButtonValues;
			}
		}

		internal Ayende.Configuration Config
		{
			get
			{
				return m_config;
			}
		}

		internal String[,] BoolNames
		{
			get
			{
				return boolnames;
			}
		}
		#endregion

		#region Constructor
		public OptionsPanel(PocketHTMLEditor parent)
		{
			// First used inside InitializeOptionsControls
			m_htCheckboxes = new Hashtable();

			InitializeHeader();
			InitializeOptionsControls();
			DpiHelper.AdjustAllControls(this);

			m_mainEditor = parent;
			this.m_config = PocketHTMLEditor.Config;

			m_currentButtonValues = new string[16];

			
			m_htTags = parent.TagHash;			

			ICollection ickeys = m_htTags.Keys;
			string[] keys = new string[ickeys.Count];
			ickeys.CopyTo(keys, 0);
			Array.Sort(keys);

			foreach (string s in keys)
			{
				m_comboButtonLabel.Items.Add(s);
			}

			Reset();
		}
		#endregion

		#region Setup functions
		private void InitializeHeader()
		{
			m_btnOK = new Button();
			this.m_btnOK.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold);
			this.m_btnOK.Location = new System.Drawing.Point(155, 2);
			this.m_btnOK.Size = new System.Drawing.Size(30, 20);
			this.m_btnOK.Text = "OK";
			this.m_btnOK.Click += new EventHandler(buttonOK_Click);
			this.Controls.Add(m_btnOK);

			m_btnCancel = new Button();
			this.m_btnCancel.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold);
			this.m_btnCancel.Location = new System.Drawing.Point(188, 2);
			this.m_btnCancel.Size = new System.Drawing.Size(50, 20);
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.Click +=new EventHandler(buttonCancel_Click);
			this.Controls.Add(m_btnCancel);					
		}

		private void InitializeOptionsControls()
		{
			m_innerPanel = new Panel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();

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
			m_lvOptions = new ListView();
			m_comboButtonLabel = new ComboBox();
			m_comboDefaultEncoding = new ComboBox();
			m_comboZoomLevel = new ComboBox();
			m_lblZoomLevel = new Label();
			m_lblDefaultEncoding = new Label();
			m_btnTestEncoding = new Button();

			this.ClientSize = new System.Drawing.Size(240, 294);

			m_innerPanel.Location = new Point(2, 30);
			m_innerPanel.Size = new Size(236, 240);
			m_innerPanel.Parent = this;

			tabControl1.Parent = m_innerPanel;
			tabControl1.Bounds = m_innerPanel.ClientRectangle;

			tabPage1.Parent = tabControl1;
			tabPage2.Parent = tabControl1;

			this.tabControl1.SelectedIndex = 0;

			m_lvOptions.Parent = tabPage1;

			m_lblZoomLevel.Parent = tabPage1;
			m_comboZoomLevel.Parent = tabPage1;
			m_comboDefaultEncoding.Parent = tabPage1;
			m_lblDefaultEncoding.Parent = tabPage1;
			m_btnTestEncoding.Parent = tabPage1;

			tabPage1.Bounds = tabControl1.Bounds;
			tabPage2.Bounds = tabControl1.Bounds;

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

			m_lvOptions.Bounds = new Rectangle(0, 0, 236, 124);
			ColumnHeader header = new ColumnHeader();
			header.Text = "Options";
			header.Width = 195;
			m_lvOptions.View = View.Details;
			m_lvOptions.Columns.Add(header);
			m_lvOptions.HeaderStyle = ColumnHeaderStyle.None;
			m_lvOptions.CheckBoxes = true;

			for (int i = 0; i < boolnames.GetLength(0); i++)
			{
				ListViewItem lvi = new ListViewItem();
				lvi.Text = boolnames[i,1];
				m_lvOptions.Items.Add(lvi);

				m_htCheckboxes[boolnames[i, 0]] = lvi;
			}


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

			m_lblZoomLevel.Text = "Preview zoom level:";
			m_lblZoomLevel.Bounds = new Rectangle(4, 130, 112, 16);
			m_comboZoomLevel.Items.Add("0");
			m_comboZoomLevel.Items.Add("1");
			m_comboZoomLevel.Items.Add("2");
			m_comboZoomLevel.Items.Add("3");
			m_comboZoomLevel.Items.Add("4");
			m_comboZoomLevel.Bounds = new Rectangle(124, 130, 40, 22);



			m_lblDefaultEncoding.Text = "Default text encoding:";
			m_lblDefaultEncoding.Bounds = new Rectangle(4, 160, 180, 16);

			m_comboDefaultEncoding.Bounds = new Rectangle(4, 180, 180, 22);
			for (int i = 0; i < encodingnames.GetLength(0); i++)
			{
				m_comboDefaultEncoding.Items.Add(encodingnames[i, 0]);
			}

			m_btnTestEncoding.Text = "Test";
			m_btnTestEncoding.Bounds = new Rectangle(188, 180, 40, 20);
			m_btnTestEncoding.Click += new EventHandler(m_btnTestEncoding_Click);


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
			
			this.tabPage1.Text = "Options";
			this.tabPage2.Text = "QuickTags";

		}

		internal void Reset()
		{
			ResetValues();
			ResetCheckboxes();

			m_comboButtonNumber.SelectedIndex = 0;
		}

		internal void ResetCheckboxes()
		{
			for (int i = 0; i < boolnames.GetLength(0); i++)
			{

				string name = boolnames[i, 0];
				bool chk = m_config.GetBool("Options", name);
				ListViewItem lvi = (ListViewItem)m_htCheckboxes[name];
				lvi.Checked = chk;
			}
		}

		internal void ResetValues()
		{
			Hashtable buttonTags = m_mainEditor.ButtonTags;
			NamedButton[] buttons = m_mainEditor.Buttons;
			for (int i = 0; i < m_currentButtonValues.Length; i++)
			{
				m_currentButtonValues[i] = (string)buttonTags[buttons[i].Name];
			}

			string hardwareButtonName = m_config.GetValue("Options", "HardwareButton");
			string sButtonNumber = hardwareButtonName.Substring(hardwareButtonName.Length - 1);
			int buttonNumber = Int32.Parse(sButtonNumber);

			nudHardwareButton.Value = buttonNumber;

			string zoomLevel = m_config.GetValue("Options", "ZoomLevel");
			m_comboZoomLevel.SelectedItem = zoomLevel;

			string encodingName = m_config.GetValue("Options", "DefaultEncoding");

			int encodingIndex = -1;
			for (int i = 0; i < encodingnames.GetLength(0); i++)
			{
				if (encodingnames[i, 1] == encodingName)
				{
					encodingIndex = i;
					break;
				}
			}

			if (encodingIndex == -1)
			{
				// UTF-8
				encodingIndex = 13;
			}

			m_comboDefaultEncoding.SelectedIndex = encodingIndex;
		}
		#endregion

		#region Event handlers
		void m_btnTestEncoding_Click(object sender, EventArgs e)
		{
			string selectedEncodingFullName = (string)m_comboDefaultEncoding.SelectedItem;
			string selectedEncodingShortName = string.Empty;
			for (int i = 0; i < encodingnames.GetLength(0); i++)
			{
				if (encodingnames[i, 0] == selectedEncodingFullName)
				{
					selectedEncodingShortName = encodingnames[i, 1];
					break;
				}
			}

			string resultMessage = string.Empty;
			MessageBoxIcon mbi = MessageBoxIcon.Exclamation;
			try
			{
				Encoding.GetEncoding(selectedEncodingShortName);
				resultMessage = "This encoding is supported on this device.";
			}
			catch(PlatformNotSupportedException)
			{
				resultMessage = "This encoding is NOT supported on this device.";
				mbi = MessageBoxIcon.Hand;
			}

			MessageBox.Show(resultMessage, "Encoding Test Results", 
							MessageBoxButtons.OK, mbi, MessageBoxDefaultButton.Button1);
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

		private void comboButtonLabel_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string text = (string)m_comboButtonLabel.SelectedItem;
			Tag t = (Tag)m_htTags[text];
			label2.Text = t.StartTag;

			int idx = m_comboButtonNumber.SelectedIndex;
			m_currentButtonValues[idx] = text;

		}

		private void comboButtonNumber_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int num = m_comboButtonNumber.SelectedIndex;
			string refname = m_currentButtonValues[num];
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

			for(int i = 0; i < boolnames.GetLength(0); i++)
			{
				string name = boolnames[i, 0];
				ListViewItem lvi = (ListViewItem)m_htCheckboxes[name];
				string boolvalue = lvi.Checked.ToString();
				
				Config.SetValue("Options", name, boolvalue);
			}

			int buttonNumber = (int)HardwareButtonNumber.Value;
			string sButtonNumber = "Hardware" + buttonNumber.ToString();
			Config.SetValue("Options", "HardwareButton", sButtonNumber);

			Config.SetValue("Options", "ZoomLevel", (String)m_comboZoomLevel.SelectedItem);

			string selectedEncodingFullName = (string)m_comboDefaultEncoding.SelectedItem;
			string selectedEncodingShortName = string.Empty;
			for(int i = 0; i < encodingnames.GetLength(0); i++)
			{
				if(encodingnames[i, 0] == selectedEncodingFullName)
				{
					selectedEncodingShortName = encodingnames[i, 1];
					break;
				}
			}

			Config.SetValue("Options", "DefaultEncoding", selectedEncodingShortName);

			m_result = DialogResult.OK;
			((PocketHTMLEditor)Parent).MenuToolsOptions_Click(sender, e);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			m_result = DialogResult.Cancel;
			((PocketHTMLEditor)Parent).MenuToolsOptions_Click(sender, e);
		}
		#endregion

	}
}