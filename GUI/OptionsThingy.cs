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
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private Hashtable m_tagHash;
		//NamedButton[] buttons;
		//private PHNOptions options;
		private Ayende.Configuration config;
		private System.Windows.Forms.CheckBox cbTextWrap;
		private System.Windows.Forms.CheckBox cbPageWrap;
		private System.Windows.Forms.CheckBox cbXHTML;
		private System.Windows.Forms.CheckBox cbClearType;
		private PocketHTMLEditor maineditor;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private string[] currentButtonValues;
		private string[] boolnames;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.CheckBox cbIndentHTML;
		private System.Windows.Forms.CheckBox cbAutoIndent;
		private CheckBox[] boxes;
		

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
			boolnames = new string[]{"XHTMLTags", "TextWrap", "PageWrap",
									"ClearType", "IndentHTML", "AutoIndent"};
			boxes = new CheckBox[]{cbXHTML, cbTextWrap, cbPageWrap, 
									cbClearType, cbIndentHTML, cbAutoIndent};

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
			this.cbTextWrap = new System.Windows.Forms.CheckBox();
			this.cbPageWrap = new System.Windows.Forms.CheckBox();
			this.cbXHTML = new System.Windows.Forms.CheckBox();
			this.cbClearType = new System.Windows.Forms.CheckBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cbIndentHTML = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.cbAutoIndent = new System.Windows.Forms.CheckBox();
			// 
			// cbTextWrap
			// 
			this.cbTextWrap.Location = new System.Drawing.Point(4, 4);
			this.cbTextWrap.Size = new System.Drawing.Size(128, 16);
			this.cbTextWrap.Text = "Wrap editor text";
			// 
			// cbPageWrap
			// 
			this.cbPageWrap.Location = new System.Drawing.Point(4, 24);
			this.cbPageWrap.Size = new System.Drawing.Size(128, 16);
			this.cbPageWrap.Text = "Wrap page preview";
			// 
			// cbXHTML
			// 
			this.cbXHTML.Location = new System.Drawing.Point(4, 84);
			this.cbXHTML.Size = new System.Drawing.Size(160, 16);
			this.cbXHTML.Text = "XHTML-style single tags";
			// 
			// cbClearType
			// 
			this.cbClearType.Location = new System.Drawing.Point(4, 104);
			this.cbClearType.Size = new System.Drawing.Size(164, 16);
			this.cbClearType.Text = "Use ClearType in preview";
			// 
			// comboBox1
			// 
			this.comboBox1.Location = new System.Drawing.Point(128, 160);
			this.comboBox1.Size = new System.Drawing.Size(112, 22);
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label1.Location = new System.Drawing.Point(4, 188);
			this.label1.Size = new System.Drawing.Size(32, 20);
			this.label1.Text = "Tag:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label2.Location = new System.Drawing.Point(40, 188);
			this.label2.Size = new System.Drawing.Size(196, 52);
			this.label2.Text = "label2";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.label3.Location = new System.Drawing.Point(84, 136);
			this.label3.Size = new System.Drawing.Size(76, 20);
			this.label3.Text = "QuickTags";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// cbIndentHTML
			// 
			this.cbIndentHTML.Location = new System.Drawing.Point(4, 44);
			this.cbIndentHTML.Size = new System.Drawing.Size(128, 16);
			this.cbIndentHTML.Text = "Indent HTML tags";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label4.Location = new System.Drawing.Point(4, 160);
			this.label4.Size = new System.Drawing.Size(44, 20);
			this.label4.Text = "Button:";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			this.label5.Location = new System.Drawing.Point(92, 160);
			this.label5.Size = new System.Drawing.Size(36, 20);
			this.label5.Text = "Label:";
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
			this.comboBox2.Location = new System.Drawing.Point(48, 160);
			this.comboBox2.Size = new System.Drawing.Size(36, 22);
			this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
			// 
			// cbAutoIndent
			// 
			this.cbAutoIndent.Location = new System.Drawing.Point(4, 64);
			this.cbAutoIndent.Size = new System.Drawing.Size(136, 16);
			this.cbAutoIndent.Text = "Autoindent on Enter";
			// 
			// OptionsThingy
			// 
			this.ClientSize = new System.Drawing.Size(240, 294);
			this.Controls.Add(this.cbAutoIndent);
			this.Controls.Add(this.comboBox2);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbIndentHTML);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.cbClearType);
			this.Controls.Add(this.cbXHTML);
			this.Controls.Add(this.cbPageWrap);
			this.Controls.Add(this.cbTextWrap);
			this.Text = "OptionsThingy";

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			
		}

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
	}
}
