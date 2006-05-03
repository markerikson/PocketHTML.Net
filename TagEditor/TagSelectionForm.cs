#region Using directives

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using ISquared.Windows.Forms;

#endregion

namespace ISquared.PocketHTML
{
	#region Enums
	public enum SelectedTagType
	{
		ExistingTag = 0,
		Header = 1,
		Separator = 2,
	}
	#endregion

	public class TagSelectionForm : FloatingDialog
	{
		#region Private members
		private ComboBox m_comboTagList;
		private Button m_btnCancel;
		private Button m_btnOK;
		private System.Windows.Forms.MainMenu mainMenu1;
		private RadioButton m_rbTag;
		private RadioButton m_rbSeparator;
		private RadioButton m_rbHeading;
		private TextBox textBox1;
		private ArrayList m_tagList;
		#endregion

		#region Properties
		public ArrayList TagList
		{
			get
			{
				return m_tagList;
			}
			set
			{
				m_tagList = value;
				m_comboTagList.DataSource = m_tagList;
			}
		}

		public SelectedTagType SelectedType
		{
			get
			{
				if(m_rbTag.Checked)
				{
					return SelectedTagType.ExistingTag;
				}
				else if(m_rbHeading.Checked)
				{
					return SelectedTagType.Header;
				}
				else if(m_rbSeparator.Checked)
				{
					return SelectedTagType.Separator;
				}
				else
				{
					throw new Exception("Unexpected selection type in TagSelectionForm");
				}

			}
		}

		public string SelectedTagName
		{
			get
			{
				string result = string.Empty;

				if(m_rbTag.Checked)
				{
					result = (string)m_comboTagList.SelectedItem;;
				}
				else if(m_rbHeading.Checked)
				{
					result = textBox1.Text;
				}
				return result;
			}
		}
		#endregion

		#region Constructor
		public TagSelectionForm()
		{
			InitializeComponent();

			m_comboTagList.BindingContext = new BindingContext();
			m_rbTag.Checked = true;

			DpiHelper.AdjustAllControls(this);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.m_comboTagList = new System.Windows.Forms.ComboBox();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_rbTag = new System.Windows.Forms.RadioButton();
			this.m_rbSeparator = new System.Windows.Forms.RadioButton();
			this.m_rbHeading = new System.Windows.Forms.RadioButton();
			this.textBox1 = new System.Windows.Forms.TextBox();
			// 
			// m_comboTagList
			// 
			this.m_comboTagList.Location = new System.Drawing.Point(60, 3);
			this.m_comboTagList.Size = new System.Drawing.Size(140, 22);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.Location = new System.Drawing.Point(69, 77);
			this.m_btnCancel.Size = new System.Drawing.Size(60, 20);
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
			// 
			// m_btnOK
			// 
			this.m_btnOK.Location = new System.Drawing.Point(3, 77);
			this.m_btnOK.Size = new System.Drawing.Size(60, 20);
			this.m_btnOK.Text = "OK";
			this.m_btnOK.Click += new System.EventHandler(this.m_btnOK_Click);
			// 
			// m_rbTag
			// 
			this.m_rbTag.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_rbTag.Location = new System.Drawing.Point(3, 3);
			this.m_rbTag.Size = new System.Drawing.Size(51, 20);
			this.m_rbTag.Text = "Tag:";
			// 
			// m_rbSeparator
			// 
			this.m_rbSeparator.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_rbSeparator.Location = new System.Drawing.Point(3, 51);
			this.m_rbSeparator.Size = new System.Drawing.Size(91, 20);
			this.m_rbSeparator.Text = "Separator";
			// 
			// m_rbHeading
			// 
			this.m_rbHeading.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_rbHeading.Location = new System.Drawing.Point(3, 29);
			this.m_rbHeading.Size = new System.Drawing.Size(91, 20);
			this.m_rbHeading.Text = "Heading:";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(80, 31);
			this.textBox1.Size = new System.Drawing.Size(120, 21);
			// 
			// TagSelectionForm
			// 
			this.ClientSize = new System.Drawing.Size(240, 268);
			/*
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.m_rbHeading);
			this.Controls.Add(this.m_rbSeparator);
			this.Controls.Add(this.m_rbTag);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_comboTagList);
			*/
			this.Menu = this.mainMenu1;

			textBox1.Parent = this.ContentPanel;
			m_rbHeading.Parent = this.ContentPanel;
			m_rbSeparator.Parent = this.ContentPanel;
			m_rbTag.Parent = this.ContentPanel;
			m_btnCancel.Parent = this.ContentPanel;
			m_btnOK.Parent = this.ContentPanel;
			m_comboTagList.Parent = this.ContentPanel;

		}

		#endregion
		#endregion

		#region Event handlers
		private void m_btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void m_btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}
		#endregion
	}
}
