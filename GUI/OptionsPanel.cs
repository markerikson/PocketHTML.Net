#region using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using CustomControls;
#endregion

namespace ISquared.PocketHTML
{
	class OptionsPanel : System.Windows.Forms.Panel
	{
		#region private members
		private Button m_btnOK;
		private Button m_btnCancel;
		private DialogResult m_result;
		private Panel m_pnlContents;
		private ScrollablePanel m_pnlOptions;
		private ScrollablePanel m_pnlQuicktags;

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
			//DpiHelper.AdjustAllControls(this);

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
            int scale = DpiHelper.Scale(1);
			m_btnOK = new Button();
			this.m_btnOK.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_btnOK.Location = new System.Drawing.Point(scale * 140, scale * 2);
            this.m_btnOK.Size = new System.Drawing.Size(scale * 30, scale * 20);
			this.m_btnOK.Text = "OK";
			this.m_btnOK.Click += new EventHandler(buttonOK_Click);
			this.Controls.Add(m_btnOK);

			m_btnCancel = new Button();
			this.m_btnCancel.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_btnCancel.Location = new System.Drawing.Point(scale * 176, scale * 2);
            this.m_btnCancel.Size = new System.Drawing.Size(scale * 60, scale * 20);
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.Click +=new EventHandler(buttonCancel_Click);
			this.Controls.Add(m_btnCancel);					
		}

		private void InitializeOptionsControls()
		{
            int scale = DpiHelper.Scale(1);
			m_pnlContents = new Panel();
			m_pnlOptions = new ScrollablePanel();
			m_pnlQuicktags = new ScrollablePanel();
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

            this.ClientSize = new System.Drawing.Size(scale * 240, scale * 294);

            m_pnlContents.Location = new Point(scale * 2, scale * 30);
            m_pnlContents.Size = new Size(scale * 236, scale * 240);
			m_pnlContents.Parent = this;

			tabControl1.Parent = m_pnlContents;
			tabControl1.Bounds = m_pnlContents.ClientRectangle;

			tabPage1.Parent = tabControl1;
			tabPage2.Parent = tabControl1;

			tabPage1.Bounds = tabControl1.Bounds;
			tabPage2.Bounds = tabControl1.Bounds;

			this.tabControl1.SelectedIndex = 0;

			m_pnlOptions.Parent = tabPage1;
			m_pnlOptions.Bounds = tabPage1.Bounds;
			m_pnlOptions.SetScrollHeight(scale * 200);

			m_pnlQuicktags.Parent = tabPage2;
			m_pnlQuicktags.Bounds = tabPage2.Bounds;
			m_pnlQuicktags.SetScrollHeight(scale * 200);

			m_lvOptions.Parent = m_pnlOptions.Contents;
			m_lblZoomLevel.Parent = m_pnlOptions.Contents;
			m_comboZoomLevel.Parent = m_pnlOptions.Contents;
			m_comboDefaultEncoding.Parent = m_pnlOptions.Contents;
			m_lblDefaultEncoding.Parent = m_pnlOptions.Contents;
			m_btnTestEncoding.Parent = m_pnlOptions.Contents;

			label1.Parent = m_pnlQuicktags.Contents;
			label2.Parent = m_pnlQuicktags.Contents;
			label3.Parent = m_pnlQuicktags.Contents;
			label4.Parent = m_pnlQuicktags.Contents;
			label5.Parent = m_pnlQuicktags.Contents;
			label6.Parent = m_pnlQuicktags.Contents;
			nudHardwareButton.Parent = m_pnlQuicktags.Contents;
			m_comboButtonLabel.Parent = m_pnlQuicktags.Contents;
			m_comboButtonNumber.Parent = m_pnlQuicktags.Contents;
			cbHardwareButtons.Parent = m_pnlQuicktags.Contents;

            m_lvOptions.Bounds = new Rectangle(0, 0, scale * 220, scale * 124);
			ColumnHeader header = new ColumnHeader();
			header.Text = "Options";
            header.Width = scale * 195;
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
            this.label6.Location = new System.Drawing.Point(scale * 7, scale * 4);
            this.label6.Size = new System.Drawing.Size(scale * 137, scale * 20);
			this.label6.Text = "Button Assignments:";
			// 
			// label3
			// 
            this.label3.Location = new System.Drawing.Point(scale * 33, scale * 151);
            this.label3.Size = new System.Drawing.Size(scale * 49, scale * 20);
			this.label3.Text = "Button:";
			// 
			// nudHardwareButton
			// 
            this.nudHardwareButton.Location = new System.Drawing.Point(scale * 88, scale * 149);
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
            this.nudHardwareButton.Size = new System.Drawing.Size(scale * 43, scale * 22);
			this.nudHardwareButton.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// cbHardwareButtons
			// 
			this.cbHardwareButtons.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.cbHardwareButtons.Location = new System.Drawing.Point(scale * 5, scale * 129);
            this.cbHardwareButtons.Size = new System.Drawing.Size(scale * 196, scale * 16);
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
            this.m_comboButtonNumber.Location = new System.Drawing.Point(scale * 49, scale * 27);
            this.m_comboButtonNumber.Size = new System.Drawing.Size(scale * 36, scale * 22);
			this.m_comboButtonNumber.SelectedIndexChanged += new System.EventHandler(this.comboButtonNumber_SelectedIndexChanged);

			m_lblZoomLevel.Text = "Preview zoom level:";
            m_lblZoomLevel.Bounds = new Rectangle(scale * 4, scale * 130, scale * 112, scale * 16);
			m_comboZoomLevel.Items.Add("0");
			m_comboZoomLevel.Items.Add("1");
			m_comboZoomLevel.Items.Add("2");
			m_comboZoomLevel.Items.Add("3");
			m_comboZoomLevel.Items.Add("4");
            m_comboZoomLevel.Bounds = new Rectangle(scale * 124, scale * 130, scale * 40, scale * 22);



			m_lblDefaultEncoding.Text = "Default text encoding:";
            m_lblDefaultEncoding.Bounds = new Rectangle(scale * 4, m_comboZoomLevel.Bottom + scale * 8, 
                                                        scale * 172, scale * 16);

            m_comboDefaultEncoding.Bounds = new Rectangle(scale * 4, m_lblDefaultEncoding.Bottom + scale * 4, 
                                                            scale * 172, scale * 22);
			for (int i = 0; i < encodingnames.GetLength(0); i++)
			{
				m_comboDefaultEncoding.Items.Add(encodingnames[i, 0]);
			}

			m_btnTestEncoding.Text = "Test";
            m_btnTestEncoding.Bounds = new Rectangle(scale * 180, m_lblDefaultEncoding.Bottom + scale * 4,
                                                    scale * 40, scale * 20);
			m_btnTestEncoding.Click += new EventHandler(m_btnTestEncoding_Click);


				// 
				// label5
				// 
			this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
            this.label5.Location = new System.Drawing.Point(scale * 93, scale * 27);
            this.label5.Size = new System.Drawing.Size(scale * 36, scale * 20);
			this.label5.Text = "Label:";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
            this.label4.Location = new System.Drawing.Point(scale * 5, scale * 27);
            this.label4.Size = new System.Drawing.Size(scale * 44, scale * 20);
			this.label4.Text = "Button:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
            this.label2.Location = new System.Drawing.Point(scale * 41, scale * 55);
            this.label2.Size = new System.Drawing.Size(scale * 196, scale * 73);
			this.label2.Text = "label2";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(scale * 5, scale * 55);
            this.label1.Size = new System.Drawing.Size(scale * 32, scale * 26);
			this.label1.Text = "Tag:";
			// 
			// comboBox1
			// 
            this.m_comboButtonLabel.Location = new System.Drawing.Point(scale * 129, scale * 27);
            this.m_comboButtonLabel.Size = new System.Drawing.Size(scale * 97, scale * 22);
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

            int scale = DpiHelper.Scale(1);
			Graphics g = e.Graphics;
			SolidBrush sb2 = new SolidBrush(Color.White);
			
			Rectangle r = new Rectangle(scale * 8, scale * 2, scale * 120, scale * 22);
			SolidBrush sb = new SolidBrush(Color.Blue);
			Font titleFont = new Font("Tahoma", 10, FontStyle.Bold);
			g.DrawString("Options", titleFont, sb, r);
			Pen p = new Pen(Color.Black);
			g.DrawLine(p, 0, scale * 23, this.Width, scale * 23);
		}

		protected override void OnResize(EventArgs e)
		{
            int scale = DpiHelper.Scale(1);
			base.OnResize(e);
            m_pnlContents.Bounds = new Rectangle(0, scale * 30, this.Width, this.Height - scale * 30); 
			tabControl1.Bounds = m_pnlContents.ClientRectangle;

			m_pnlOptions.Bounds = tabPage1.ClientRectangle;
			m_pnlQuicktags.Bounds = tabPage2.ClientRectangle;

			m_btnCancel.Left =  this.Width - DpiHelper.Scale(4) - m_btnCancel.Width;
			m_btnOK.Left = m_btnCancel.Left - DpiHelper.Scale(8) - m_btnOK.Width;

			//DpiHelper.AdjustAllControls(this);
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