using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using EricUtility.Iterators;

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for OptionsThingy.
	/// </summary>
	public class OptionsThingy : System.Windows.Forms.Form
	{
		private Hashtable m_tagHash;
		//NamedButton[] buttons;
		//private PHNOptions options;
		private Ayende.Configuration config;
		private PocketHTMLEditor maineditor;
		private string[] currentButtonValues;
		private string[] boolnames;
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private CheckBox cbAutoIndent;
		private CheckBox cbIndentHTML;
		private CheckBox cbClearType;
		private CheckBox cbXHTML;
		private CheckBox cbPageWrap;
		private CheckBox cbTextWrap;
		private ComboBox comboBox2;
		private Label label5;
		private Label label4;
		private Label label2;
		private Label label1;
		private ComboBox comboBox1;
		private CheckBox cbMonospacedFont;
		private Label label6;
		private Label label3;
		private NumericUpDown nudHardwareButton;
		private CheckBox cbHardwareButtons;
		private CheckBox[] boxes;

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
	
		public OptionsThingy(Control parent, 
							PocketHTMLEditor mainform)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			
			//IterSort iters = new IterSort(parent.TagHash);
			this.Parent = parent;
			this.config = mainform.Config;
			maineditor = mainform;


			currentButtonValues = new string[16];			

			


			m_tagHash = mainform.TagHash;
			ICollection ickeys = m_tagHash.Keys;
			string[] keys = new string[ickeys.Count];
			ickeys.CopyTo(keys, 0);
			Array.Sort(keys);

			//for(int i = 0; i < keys.Length; i++)
			foreach(string s in keys)
			{
				comboBox1.Items.Add(s);
			}

			ResetValues();

			//udButtons.Value = 1;
			// TODO This is hardcoded.  Fix it somehow?
			boolnames = new string[]{"XHTMLTags", "TextWrap", "PageWrap", "ClearType",
										"IndentHTML", "AutoIndent", "HardwareButtonShowsMenu",
										"MonospacedFont"};
			boxes = new CheckBox[]{cbXHTML, cbTextWrap, cbPageWrap, cbClearType, 
									cbIndentHTML, cbAutoIndent, cbHardwareButtons,
									cbMonospacedFont};

			ResetCheckboxes();
			
		}

		internal void ResetCheckboxes()
		{
			for(int i = 0; i < boolnames.Length; i++)
			{
				
				string name = boolnames[i];
				bool chk = config.GetBool("Options", name);
				boxes[i].Checked = chk;
			}
			comboBox2.SelectedIndex = 0;
			//comboBox2_SelectedIndexChanged(this, new System.EventArgs());
		}

		internal void ResetValues()
		{
			Hashtable buttonTags = maineditor.ButtonTags;
			NamedButton[] buttons = maineditor.Buttons;
			for(int i = 0; i < currentButtonValues.Length; i++)
			{
				currentButtonValues[i] = (string)buttonTags[buttons[i].Name];
			}
			comboBox2.SelectedIndex = 0;
			//comboBox2_SelectedIndexChanged(this, new System.EventArgs());
			//comboBox1_SelectedIndexChanged(this, new System.EventArgs());

			string hardwareButtonName = config.GetValue("Options", "HardwareButton");
			string sButtonNumber = hardwareButtonName.Substring(hardwareButtonName.Length - 1);
			int buttonNumber = Int32.Parse(sButtonNumber);

			nudHardwareButton.Value = buttonNumber;
			nudHardwareButton.Enabled = cbHardwareButtons.Checked;

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
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(240, 200);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.cbMonospacedFont);
			this.tabPage1.Controls.Add(this.cbAutoIndent);
			this.tabPage1.Controls.Add(this.cbIndentHTML);
			this.tabPage1.Controls.Add(this.cbClearType);
			this.tabPage1.Controls.Add(this.cbXHTML);
			this.tabPage1.Controls.Add(this.cbPageWrap);
			this.tabPage1.Controls.Add(this.cbTextWrap);
			this.tabPage1.Location = new System.Drawing.Point(0, 0);
			this.tabPage1.Size = new System.Drawing.Size(240, 177);
			this.tabPage1.Text = "Options";
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
			this.tabPage2.Location = new System.Drawing.Point(0, 0);
			this.tabPage2.Size = new System.Drawing.Size(232, 174);
			this.tabPage2.Text = "QuickTags";
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
			this.cbHardwareButtons.Text = "Hardware button shows tag menu\r\n";
			// 
			// comboBox2
			// 
			this.comboBox2.Items.Add("1");
			this.comboBox2.Items.Add("2");
			this.comboBox2.Items.Add("3");
			this.comboBox2.Items.Add("4");
			this.comboBox2.Items.Add("5");
			this.comboBox2.Items.Add("6");
			this.comboBox2.Items.Add("7");
			this.comboBox2.Items.Add("8");
			this.comboBox2.Items.Add("9");
			this.comboBox2.Items.Add("10");
			this.comboBox2.Items.Add("11");
			this.comboBox2.Items.Add("12");
			this.comboBox2.Items.Add("13");
			this.comboBox2.Items.Add("14");
			this.comboBox2.Items.Add("15");
			this.comboBox2.Items.Add("16");
			this.comboBox2.Location = new System.Drawing.Point(49, 27);
			this.comboBox2.Size = new System.Drawing.Size(36, 22);
			this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
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
			this.comboBox1.Location = new System.Drawing.Point(129, 27);
			this.comboBox1.Size = new System.Drawing.Size(108, 22);
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// OptionsThingy
			// 
			this.ClientSize = new System.Drawing.Size(240, 294);
			this.Controls.Add(this.tabControl1);
			this.Text = "OptionsThingy";

		}
		#endregion

		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string text = (string)comboBox1.SelectedItem;
			Tag t = (Tag)m_tagHash[text];
			label2.Text = t.StartTag;
			//int idx = Convert.ToInt32(udButtons.Value - 1);
			int idx = comboBox2.SelectedIndex;
			currentButtonValues[idx] = text;
			//MessageBox.Show(th[text].ToString());
		}

		/*
		private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
		{
			int num = Convert.ToInt32(udButtons.Value - 1);
			string refname = currentButtonValues[num];//maineditor.GetButtonString(num);
			int index = comboBox1.Items.IndexOf(refname);
			comboBox1.SelectedIndex = index;					
		}
		*/

		private void comboBox2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int num = comboBox2.SelectedIndex;
			string refname = currentButtonValues[num];//maineditor.GetButtonString(num);
			int index = comboBox1.Items.IndexOf(refname);
			comboBox1.SelectedIndex = index;
					
		}

		private void cbHardwareButtons_CheckStateChanged(object sender, EventArgs e)
		{
			nudHardwareButton.Enabled = cbHardwareButtons.Checked;
		}
	}
}
