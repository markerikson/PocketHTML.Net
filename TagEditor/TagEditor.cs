#region Using directives

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using ISquared;
using ISquared.PocketHTML;
using CustomControls;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;
using ISquared.Win32Interop;
using ISquared.Win32Interop.WinEnums;
using OpenNETCF.Windows.Forms;

#endregion

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for form.
	/// </summary>
	public class TagEditor : System.Windows.Forms.Form
	{
		#region Members
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TextBoxEx m_tbOutput;
		private CheckBox m_cbAngleBrackets;
		private CheckBox m_cbMultilineTag;
		private CheckBox m_cbClosingTag;
		private ComboBox m_comboInnerTag;
		private Label label5;
		private Label label4;
		private TextBox m_tbShortName;
		private TextBox m_tbDisplayName;
		private Label label3;
		private TextBox m_tbTagValue;
		private Label label2;
		private ComboBox m_comboAvailableTags;
		private Label label1;
		private Button button8;
		private Button m_btnMoveRight;
		private Button m_btnMoveDown;
		private Button m_btnMoveUp;
		private Button m_btnMoveLeft;
		private Button m_btnPreviewTag;
		private TreeView treeView1;
		private Button button9;
		private TabPage tabPage2;

		private Hashtable m_htTags;
		private ArrayList m_sortedTags;
		private MainMenu mainMenu1;
		private MenuItem menuItem1;
		private MenuItem m_miFileExit;
		private MenuItem m_miFileSave;
		private MenuItem m_miTags;
		private MenuItem m_miTagsNew;
		private MenuItem m_miTagsSave;
		private MenuItem m_miTagsDelete;
		private Tag m_emptyTag;
		private Tag m_currentTag;
		private Label label7;
		private TextBox m_tbAttributes;
		private MenuItem m_miTagMenu;
		private MenuItem m_miTMPreview;
		private MenuItem m_miTMNew;
		private MenuItem m_miTMDelete;
		private MenuItem menuItem3;
		private MenuItem m_miEditCopy;
		private MenuItem m_miEditCut;
		private MenuItem m_miEditPaste;
		private Button m_btnInputValue;
		private ScrollablePanel m_panelTagInfo;
		private MenuItem m_miFileReload;
		private MenuItem m_miTMValidateMenu;
		private bool m_treeviewFocused;

		#endregion

		#region Constructor
		public TagEditor()
		{
			// TODO Perhaps move the edited setup code out of InitializeComponent?
			InitializeComponent();

			this.Text = "Tag Editor";

			m_sortedTags = new ArrayList();
			m_emptyTag = new Tag();

			BindingContext bc1 = new BindingContext();
			BindingContext bc2 = new BindingContext();

			ContextMenu ct = new ContextMenu();
			treeView1.ContextMenu = ct;

			MenuItem miSelectTag = new MenuItem();
			miSelectTag.Text = "Select tag";
			miSelectTag.Click += new EventHandler(miSelectTag_Click);
			ct.MenuItems.Add(miSelectTag);

			MenuItem miDeleteMenuItem = new MenuItem();
			miDeleteMenuItem.Text = "Delete Menu Item";
			miDeleteMenuItem.Click += new EventHandler(miDeleteMenuItem_Click);
			ct.MenuItems.Add(miDeleteMenuItem);			

			m_comboAvailableTags.BindingContext = bc1;
			m_comboInnerTag.BindingContext = bc2;
			m_comboAvailableTags.DataSource = m_sortedTags;
			m_comboInnerTag.DataSource = m_sortedTags;

			InteropUtility.SetTabStop(m_tbOutput);

			ReloadDataFiles();
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.m_btnInputValue = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.m_tbAttributes = new System.Windows.Forms.TextBox();
			this.m_tbOutput = new TextBoxEx();
			this.m_cbAngleBrackets = new System.Windows.Forms.CheckBox();
			this.m_cbMultilineTag = new System.Windows.Forms.CheckBox();
			this.m_cbClosingTag = new System.Windows.Forms.CheckBox();
			this.m_comboInnerTag = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.m_tbShortName = new System.Windows.Forms.TextBox();
			this.m_tbDisplayName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.m_tbTagValue = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.m_comboAvailableTags = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.button9 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.m_btnMoveRight = new System.Windows.Forms.Button();
			this.m_btnMoveDown = new System.Windows.Forms.Button();
			this.m_btnMoveUp = new System.Windows.Forms.Button();
			this.m_btnMoveLeft = new System.Windows.Forms.Button();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.m_miFileExit = new System.Windows.Forms.MenuItem();
			this.m_miFileSave = new System.Windows.Forms.MenuItem();
			this.m_miFileReload = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.m_miEditCopy = new System.Windows.Forms.MenuItem();
			this.m_miEditCut = new System.Windows.Forms.MenuItem();
			this.m_miEditPaste = new System.Windows.Forms.MenuItem();
			this.m_miTags = new System.Windows.Forms.MenuItem();
			this.m_miTagsNew = new System.Windows.Forms.MenuItem();
			this.m_miTagsSave = new System.Windows.Forms.MenuItem();
			this.m_miTagsDelete = new System.Windows.Forms.MenuItem();
			this.m_miTagMenu = new System.Windows.Forms.MenuItem();
			this.m_miTMPreview = new System.Windows.Forms.MenuItem();
			this.m_miTMNew = new System.Windows.Forms.MenuItem();
			this.m_miTMDelete = new System.Windows.Forms.MenuItem();
			this.m_panelTagInfo = new CustomControls.ScrollablePanel();
			this.m_miTMValidateMenu = new System.Windows.Forms.MenuItem();
			m_btnPreviewTag = new Button();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(240, 268);
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 

			/*
			this.tabPage1.Controls.Add(this.m_btnInputValue);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.m_tbAttributes);
			this.tabPage1.Controls.Add(this.m_tbOutput);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.m_cbAngleBrackets);
			this.tabPage1.Controls.Add(this.m_cbMultilineTag);
			this.tabPage1.Controls.Add(this.m_cbClosingTag);
			this.tabPage1.Controls.Add(this.m_comboInnerTag);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.m_tbShortName);
			this.tabPage1.Controls.Add(this.m_tbDisplayName);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.m_tbTagValue);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.m_comboAvailableTags);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(0, 0);
			this.tabPage1.Size = new System.Drawing.Size(240, 245);
			this.tabPage1.Text = "Tags";
			 
			*/

			this.tabPage1.Location = new System.Drawing.Point(0, 0);
			this.tabPage1.Size = new System.Drawing.Size(240, 245);
			this.tabPage1.Text = "Tags";

			m_panelTagInfo.Parent = tabPage1;
			m_panelTagInfo.Bounds = new Rectangle(0, 0, 240, 245);
			m_panelTagInfo.SetScrollHeight(350);
			

			this.m_panelTagInfo.Contents.Controls.Add(this.m_btnInputValue);
			this.m_panelTagInfo.Contents.Controls.Add(this.label7);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_tbAttributes);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_tbOutput);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_cbAngleBrackets);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_cbMultilineTag);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_cbClosingTag);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_comboInnerTag);
			this.m_panelTagInfo.Contents.Controls.Add(this.label5);
			this.m_panelTagInfo.Contents.Controls.Add(this.label4);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_tbShortName);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_tbDisplayName);
			this.m_panelTagInfo.Contents.Controls.Add(this.label3);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_tbTagValue);
			this.m_panelTagInfo.Contents.Controls.Add(this.label2);
			this.m_panelTagInfo.Contents.Controls.Add(this.m_comboAvailableTags);
			this.m_panelTagInfo.Contents.Controls.Add(this.label1);
			m_panelTagInfo.Contents.Controls.Add(m_btnPreviewTag);
			


			// 
			// m_btnInputValue
			// 
			this.m_btnInputValue.Location = new System.Drawing.Point(199, 57);
			this.m_btnInputValue.Size = new System.Drawing.Size(17, 19);
			this.m_btnInputValue.Text = "...";
			this.m_btnInputValue.Click += new System.EventHandler(this.m_btnInputValue_Click);
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.label7.Location = new System.Drawing.Point(3, 181);
			this.label7.Size = new System.Drawing.Size(60, 16);
			this.label7.Text = "Attributes:";
			// 
			// m_tbAttributes
			// 
			this.m_tbAttributes.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_tbAttributes.Location = new System.Drawing.Point(69, 181);
			this.m_tbAttributes.Multiline = true;
			this.m_tbAttributes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_tbAttributes.Size = new System.Drawing.Size(147, 58);
			this.m_tbAttributes.Text = "line1\r\nline2\r\nline3\r\nline4";
			this.m_tbAttributes.WordWrap = false;
			// 
			// m_tbOutput
			// 
			this.m_tbOutput.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_tbOutput.Location = new System.Drawing.Point(69, 245);
			this.m_tbOutput.Multiline = true;
			this.m_tbOutput.Size = new System.Drawing.Size(147, 100);
			this.m_tbOutput.ScrollBars = ScrollBars.Both;
			this.m_tbOutput.WordWrap = false;



			m_btnPreviewTag.Font = new Font("Tahoma", 8F, FontStyle.Regular);
			m_btnPreviewTag.Bounds = new Rectangle(3, 245, 64, 20);
			m_btnPreviewTag.Text = "Preview Tag";
			m_btnPreviewTag.Click += new EventHandler(m_btnPreviewTag_Click);
			

			// 
			// m_cbAngleBrackets
			// 
			this.m_cbAngleBrackets.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_cbAngleBrackets.Location = new System.Drawing.Point(3, 155);
			this.m_cbAngleBrackets.Size = new System.Drawing.Size(132, 20);
			this.m_cbAngleBrackets.Text = "Needs angle brackets";
			// 
			// m_cbMultilineTag
			// 
			this.m_cbMultilineTag.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_cbMultilineTag.Location = new System.Drawing.Point(137, 133);
			this.m_cbMultilineTag.Size = new System.Drawing.Size(79, 20);
			this.m_cbMultilineTag.Text = "Multi-line";
			// 
			// m_cbClosingTag
			// 
			this.m_cbClosingTag.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_cbClosingTag.Location = new System.Drawing.Point(3, 133);
			this.m_cbClosingTag.Size = new System.Drawing.Size(132, 20);
			this.m_cbClosingTag.Text = "Closing tag required";
			// 
			// m_comboInnerTag
			// 
			this.m_comboInnerTag.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_comboInnerTag.Location = new System.Drawing.Point(85, 107);
			this.m_comboInnerTag.Size = new System.Drawing.Size(131, 20);
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.label5.Location = new System.Drawing.Point(3, 111);
			this.label5.Size = new System.Drawing.Size(76, 16);
			this.label5.Text = "Inner tag:";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.label4.Location = new System.Drawing.Point(3, 82);
			this.label4.Size = new System.Drawing.Size(76, 16);
			this.label4.Text = "Short name:";
			// 
			// m_tbShortName
			// 
			this.m_tbShortName.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_tbShortName.Location = new System.Drawing.Point(85, 82);
			this.m_tbShortName.Size = new System.Drawing.Size(131, 19);
			this.m_tbShortName.Text = "textBox3";
			// 
			// m_tbDisplayName
			// 
			this.m_tbDisplayName.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_tbDisplayName.Location = new System.Drawing.Point(85, 32);
			this.m_tbDisplayName.Size = new System.Drawing.Size(131, 19);
			this.m_tbDisplayName.Text = "textBox2";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.label3.Location = new System.Drawing.Point(3, 32);
			this.label3.Size = new System.Drawing.Size(76, 16);
			this.label3.Text = "Display name:";
			// 
			// m_tbTagValue
			// 
			this.m_tbTagValue.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_tbTagValue.Location = new System.Drawing.Point(85, 57);
			this.m_tbTagValue.Size = new System.Drawing.Size(108, 19);
			this.m_tbTagValue.Text = "textBox1";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.label2.Location = new System.Drawing.Point(3, 57);
			this.label2.Size = new System.Drawing.Size(76, 16);
			this.label2.Text = "Tag value:";
			// 
			// m_comboAvailableTags
			// 
			this.m_comboAvailableTags.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_comboAvailableTags.Location = new System.Drawing.Point(85, 6);
			this.m_comboAvailableTags.Size = new System.Drawing.Size(131, 20);
			this.m_comboAvailableTags.SelectedIndexChanged += new System.EventHandler(this.m_comboAvailableTags_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.label1.Location = new System.Drawing.Point(3, 10);
			this.label1.Size = new System.Drawing.Size(76, 16);
			this.label1.Text = "Available tags:";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.button9);
			this.tabPage2.Controls.Add(this.button8);
			this.tabPage2.Controls.Add(this.m_btnMoveRight);
			this.tabPage2.Controls.Add(this.m_btnMoveDown);
			this.tabPage2.Controls.Add(this.m_btnMoveUp);
			this.tabPage2.Controls.Add(this.m_btnMoveLeft);
			this.tabPage2.Controls.Add(this.treeView1);
			this.tabPage2.Location = new System.Drawing.Point(0, 0);
			this.tabPage2.Size = new System.Drawing.Size(240, 245);
			this.tabPage2.Text = "Tag Menu";
			// 
			// button9
			// 
			this.button9.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.button9.Location = new System.Drawing.Point(89, 245);
			this.button9.Size = new System.Drawing.Size(76, 20);
			this.button9.Text = "Delete Item";
			// 
			// button8
			// 
			this.button8.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.button8.Location = new System.Drawing.Point(7, 245);
			this.button8.Size = new System.Drawing.Size(76, 20);
			this.button8.Text = "Add Item";
			// 
			// m_btnMoveRight
			// 
			this.m_btnMoveRight.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.m_btnMoveRight.Location = new System.Drawing.Point(141, 219);
			this.m_btnMoveRight.Size = new System.Drawing.Size(40, 20);
			this.m_btnMoveRight.Text = "Right";
			this.m_btnMoveRight.Click += new System.EventHandler(this.m_btnMoveRight_Click);
			// 
			// m_btnMoveDown
			// 
			this.m_btnMoveDown.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.m_btnMoveDown.Location = new System.Drawing.Point(95, 219);
			this.m_btnMoveDown.Size = new System.Drawing.Size(40, 20);
			this.m_btnMoveDown.Text = "Down";
			this.m_btnMoveDown.Click += new System.EventHandler(this.m_btnMoveDown_Click);
			// 
			// m_btnMoveUp
			// 
			this.m_btnMoveUp.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.m_btnMoveUp.Location = new System.Drawing.Point(49, 219);
			this.m_btnMoveUp.Size = new System.Drawing.Size(40, 20);
			this.m_btnMoveUp.Text = "Up";
			this.m_btnMoveUp.Click += new System.EventHandler(this.m_btnMoveUp_Click);
			// 
			// m_btnMoveLeft
			// 
			this.m_btnMoveLeft.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.m_btnMoveLeft.Location = new System.Drawing.Point(3, 219);
			this.m_btnMoveLeft.Size = new System.Drawing.Size(40, 20);
			this.m_btnMoveLeft.Text = "Left";
			this.m_btnMoveLeft.Click += new System.EventHandler(this.m_btnMoveLeft_Click);
			// 
			// treeView1
			// 
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Size = new System.Drawing.Size(240, 213);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.Add(this.menuItem1);
			this.mainMenu1.MenuItems.Add(this.menuItem3);
			this.mainMenu1.MenuItems.Add(this.m_miTags);
			this.mainMenu1.MenuItems.Add(this.m_miTagMenu);
			// 
			// menuItem1
			// 
			this.menuItem1.MenuItems.Add(this.m_miFileSave);
			this.menuItem1.MenuItems.Add(this.m_miFileReload);
			this.menuItem1.MenuItems.Add(this.m_miFileExit);
			this.menuItem1.Text = "File";
			// 
			// m_miFileExit
			// 
			this.m_miFileExit.Text = "Exit";
			// 
			// m_miFileSave
			// 
			this.m_miFileSave.Text = "Save Files";
			this.m_miFileSave.Click += new System.EventHandler(this.m_miFileSave_Click);
			// 
			// m_miFileReload
			// 
			this.m_miFileReload.Text = "Reload Data";
			this.m_miFileReload.Click += new System.EventHandler(this.m_miFileReload_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.MenuItems.Add(this.m_miEditCopy);
			this.menuItem3.MenuItems.Add(this.m_miEditCut);
			this.menuItem3.MenuItems.Add(this.m_miEditPaste);
			this.menuItem3.Text = "Edit";
			// 
			// m_miEditCopy
			// 
			this.m_miEditCopy.Text = "Copy";
			this.m_miEditCopy.Click += new System.EventHandler(this.HandleEditMenu);
			// 
			// m_miEditCut
			// 
			this.m_miEditCut.Text = "Cut";
			this.m_miEditCut.Click += new System.EventHandler(this.HandleEditMenu);
			// 
			// m_miEditPaste
			// 
			this.m_miEditPaste.Text = "Paste";
			this.m_miEditPaste.Click += new System.EventHandler(this.HandleEditMenu);
			// 
			// m_miTags
			// 
			this.m_miTags.MenuItems.Add(this.m_miTagsNew);
			this.m_miTags.MenuItems.Add(this.m_miTagsSave);
			this.m_miTags.MenuItems.Add(this.m_miTagsDelete);
			this.m_miTags.Text = "Tags";
			// 
			// m_miTagsNew
			// 
			this.m_miTagsNew.Text = "New Tag";
			this.m_miTagsNew.Click += new System.EventHandler(this.m_miTagsNew_Click);
			// 
			// m_miTagsSave
			// 
			this.m_miTagsSave.Text = "Save Tag";
			this.m_miTagsSave.Click += new System.EventHandler(this.m_miTagsSave_Click);
			// 
			// m_miTagsDelete
			// 
			this.m_miTagsDelete.Text = "Delete Tag";
			this.m_miTagsDelete.Click += new System.EventHandler(this.m_miTagsDelete_Click);
			// 
			// m_miTagMenu
			// 
			this.m_miTagMenu.MenuItems.Add(this.m_miTMNew);
			this.m_miTagMenu.MenuItems.Add(this.m_miTMDelete);
			this.m_miTagMenu.MenuItems.Add(this.m_miTMPreview);
			this.m_miTagMenu.MenuItems.Add(this.m_miTMValidateMenu);
			this.m_miTagMenu.Text = "Tag Menu";
			// 
			// m_miTMPreview
			// 
			this.m_miTMPreview.Text = "Preview Menu";
			this.m_miTMPreview.Click += new System.EventHandler(this.m_miTMPreview_Click);
			// 
			// m_miTMNew
			// 
			this.m_miTMNew.Text = "New Menu Item";
			this.m_miTMNew.Click += new System.EventHandler(this.m_miTMNew_Click);
			// 
			// m_miTMDelete
			// 
			this.m_miTMDelete.Text = "Delete Menu Item";
			this.m_miTMDelete.Click += new System.EventHandler(this.miDeleteMenuItem_Click);
			// 
			// m_panelTagInfo
			// 
			// 
			// m_miTMValidateMenu
			// 
			this.m_miTMValidateMenu.Text = "Validate Menu";
			this.m_miTMValidateMenu.Click += new System.EventHandler(this.m_miTMValidateMenu_Click);
			// 
			// Form1
			// 
			this.ClientSize = new System.Drawing.Size(240, 268);
			this.Controls.Add(this.tabControl1);
			this.Menu = this.mainMenu1;
			this.Text = "Form1";

		}

		void m_btnPreviewTag_Click(object sender, EventArgs e)
		{
			PreviewTag();
		}

		#endregion


		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			Application.Run(new TagEditor());
		}
		#endregion

		#region Load / Save code
		private void ReloadDataFiles()
		{
			m_sortedTags.Clear();
			treeView1.Nodes.Clear();

			m_htTags = PocketHTMLShared.LoadTagsXTR();

			m_htTags[string.Empty] = m_emptyTag;

			foreach (DictionaryEntry de in m_htTags)
			{
				Tag t = (Tag)de.Value;
				m_sortedTags.Add(t.DisplayName);
			}

			m_sortedTags.Sort();

			LoadMenuTree();

			RefreshTagLists();

			m_comboAvailableTags.SelectedIndex = 0;
			m_comboInnerTag.SelectedIndex = -1;
			m_comboInnerTag.SelectedIndex = -1;

			tabControl1.SelectedIndex = 0;
			treeView1.SelectedNode = treeView1.Nodes[0];
		}

		/*
		 * Scenarios:
		 * 1) Existing tag, DisplayName is unchanged
		 *		- Save tag info
		 * 2) Existing tag, DisplayName has been made blank
		 *		- Warn user about blank name and return
		 * 3) Existing tag, DisplayName has been changed, name is unique
		 *		- Remove old tag, save new one
		 * 4) Existing tag, Displayname has been changed, name is a duplicate
		 *		- Warn user about duplicate name and return
		 * 5) New tag, DisplayName is blank
		 *		- Warn user about blank name and return
		 * 6) New tag, DisplayName is filled, name is unique
		 *		- Save new tag, add to hashtable
		 * 7) New tag, DisplayName is filled, name is duplicate
		 *		- Warn user about duplicate name and return
		 */

		private bool CheckTagValidity()
		{
			if(m_tbTagValue.Text == string.Empty)
			{
				MessageBox.Show("Please give this tag a value.", "Missing Information",
					MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				return false;
			}

			// Handles cases 2 and 5
			if (m_tbDisplayName.Text == String.Empty)
			{
				MessageBox.Show("Please give this tag a Display Name.", "Missing Information",
					MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				return false;
			}

			// Handles cases 4 and 7
			if (m_tbDisplayName.Text != m_currentTag.DisplayName)
			{
				if (m_htTags.ContainsKey(m_tbDisplayName.Text))
				{
					MessageBox.Show("A tag with this DisplayName already exists.  Please give this tag a different DisplayName and save again.",
									"Duplicate DisplayName", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
			}

			// Handles case 3
			if (m_tbDisplayName.Text != m_currentTag.DisplayName
				&& m_currentTag.DisplayName != string.Empty)
			{
				string message = "Rename tag from \"" + m_currentTag.DisplayName + "\" to \"" + m_tbDisplayName.Text + "\"?";
				DialogResult dr = MessageBox.Show(message, "Rename tag?", MessageBoxButtons.YesNo,
												MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dr == DialogResult.Yes)
				{
					m_htTags.Remove(m_currentTag.DisplayName);
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		private void SaveCurrentTag()
		{
			// Handles cases 1 and 6
			m_currentTag.DisplayName = m_tbDisplayName.Text;
			m_currentTag.Value = m_tbTagValue.Text;
			m_currentTag.ShortName = m_tbShortName.Text;
			m_currentTag.MultiLineTag = m_cbMultilineTag.Checked;
			m_currentTag.AngleBrackets = m_cbAngleBrackets.Checked;
			m_currentTag.ClosingTag = m_cbClosingTag.Checked;
			m_currentTag.DefaultInnerTag = (string)m_comboInnerTag.SelectedItem;
			m_currentTag.InnerTags = m_currentTag.DefaultInnerTag != string.Empty;

			string attributeString = m_tbAttributes.Text;

			if (attributeString != string.Empty)
			{
				string[] attributes = Regex.Split(attributeString, "\r\n");
				m_currentTag.DefaultAttributes = attributes;
			}
			else
			{
				m_currentTag.DefaultAttributes = new string[0];
			}
		}

		private void SaveTagInformation()
		{
			if(!CheckTagValidity())
			{
				return;
			}

			if (m_currentTag == m_emptyTag)
			{
				m_currentTag = new Tag();
			}

			SaveCurrentTag();

			m_htTags[m_currentTag.DisplayName] = m_currentTag;

			if (!m_sortedTags.Contains(m_currentTag.DisplayName))
			{
				m_sortedTags.Add(m_currentTag.DisplayName);
				m_sortedTags.Sort();
			}

			// Cached because m_currentTag gets overwritten in SelectedIndex handler
			string currentDisplayName = m_currentTag.DisplayName;

			RefreshTagLists();

			m_comboAvailableTags.SelectedItem = currentDisplayName;

		}

		private void SaveTagFile()
		{
			string tagFile = Utility.GetCurrentDir(true) + "tags.xml";
			XmlTextWriter xtw = new XmlTextWriter(tagFile, Encoding.ASCII);
			xtw.Formatting = Formatting.Indented;

			xtw.WriteStartDocument();
			xtw.WriteStartElement("TagList");


			foreach (string s in m_sortedTags)
			{
				if (s == string.Empty)
				{
					continue;
				}

				Tag t = (Tag)m_htTags[s];

				if (t == null)
				{
					continue;
				}

				xtw.WriteStartElement("Tag");

				xtw.WriteAttributeString("DisplayName", t.DisplayName);
				xtw.WriteAttributeString("Value", t.Value);
				xtw.WriteAttributeString("AngleBrackets", t.AngleBrackets.ToString());
				xtw.WriteAttributeString("InnerTag", t.DefaultInnerTag);
				xtw.WriteAttributeString("ShortName", t.ShortName);
				xtw.WriteAttributeString("MultiLine", t.MultiLineTag.ToString());
				xtw.WriteAttributeString("ClosingTag", t.ClosingTag.ToString());

				if (t.DefaultAttributes.Length > 0)
				{
					xtw.WriteStartElement("Attributes");

					foreach (string attribute in t.DefaultAttributes)
					{
						xtw.WriteStartElement("Attribute");
						xtw.WriteString(attribute);
						xtw.WriteEndElement();
					}

					xtw.WriteEndElement();
				}

				xtw.WriteEndElement();

			}

			xtw.WriteEndElement();
			xtw.Close();
		}

		private void SaveMenuFile()
		{
			string menuFile = Utility.GetCurrentDir(true) + "menu.xml";

			XmlTextWriter xtw = new XmlTextWriter(menuFile, Encoding.ASCII);
			xtw.Formatting = Formatting.Indented;

			xtw.WriteStartDocument();
			xtw.WriteStartElement("menuitems");

			WriteMenuItems(xtw, treeView1.Nodes);

			xtw.WriteEndElement();

			xtw.Close();
		}

		private void WriteMenuItems(XmlTextWriter xtw, TreeNodeCollection tnc)
		{
			foreach (TreeNode tn in tnc)
			{
				xtw.WriteStartElement("menuitem");
				xtw.WriteAttributeString("name", tn.Text);

				if (tn.Nodes.Count > 0)
				{
					WriteMenuItems(xtw, tn.Nodes);
				}

				xtw.WriteEndElement();
			}
		}

		private void LoadMenuTree()
		{
			string filename = Utility.GetCurrentDir(true) + "menu.xml";

			XmlTextReader xtr = new XmlTextReader(filename);
			xtr.WhitespaceHandling = WhitespaceHandling.None;
			xtr.MoveToContent();
			Stack stack = new Stack();

			TreeNodeCollection currentCollection = treeView1.Nodes;

			while (xtr.Read())
			{
				switch (xtr.NodeType)
				{
					case XmlNodeType.Element:
					{
						if (xtr.Name == "menuitems")
						{
							continue;
						}
						if (xtr.IsStartElement())
						{
							string name = xtr.GetAttribute("name");
							TreeNode newNode = new TreeNode();
							newNode.Text = name;
							currentCollection.Add(newNode);

							if (!xtr.IsEmptyElement)
							{
								stack.Push(currentCollection);
								currentCollection = newNode.Nodes;
							}
						}
						break;
					}
					case XmlNodeType.EndElement:
					{
						if (xtr.Name == "menuitems")
						{
							continue;
						}

						currentCollection = (TreeNodeCollection)stack.Pop();
						break;
					}
				}
			}
			xtr.Close();
		}
		#endregion

		#region Event handlers
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			tabControl1.Bounds = this.ClientRectangle;
			m_panelTagInfo.Bounds = tabPage1.ClientRectangle;

			DpiHelper.AdjustAllControls(this);
		}


		private void m_comboAvailableTags_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(m_comboAvailableTags.SelectedItem == null)
			{
				return;
			}

			string itemName = (string)m_comboAvailableTags.SelectedItem;

			Tag t = (Tag)m_htTags[itemName];

			if (t == null)
			{
				return;
			}
			DisplayTagDetails(t);

			m_currentTag = t;
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl1.SelectedIndex == 0)
			{
				m_miTags.Enabled = true;
				m_miTagMenu.Enabled = false;
			}
			else if (tabControl1.SelectedIndex == 1)
			{
				m_miTags.Enabled = false;
				m_miTagMenu.Enabled = true;
				treeView1.Focus();
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			if (treeView1 != null)
			{
				treeView1.Focus();
				if (m_treeviewFocused == false)
				{
					m_treeviewFocused = true;
					IntPtr hWnd = GetFocus();
					Int32 lS = GetWindowLong(hWnd, -16);
					lS = lS | 0x20;
					SetWindowLong(hWnd, -16, lS);
				}
			}
		}
		#endregion

		#region Tag / menu interface functions

		private void PreviewTag()
		{
			Tag backupTag = m_currentTag;
			m_currentTag = new Tag();

			SaveCurrentTag();
			m_tbOutput.Text = "";
			m_tbOutput.SelectionStart = 0;
			PocketHTMLShared.InsertTag(m_currentTag, m_tbOutput, m_htTags);

			m_currentTag = backupTag;
		}

		private void RefreshTagLists()
		{
			ComboBox[] boxes = new ComboBox[] { m_comboAvailableTags, m_comboInnerTag };

			foreach (ComboBox cb in boxes)
			{
				BindingManagerBase bm = cb.BindingContext[m_sortedTags];
				CurrencyManager cm = (CurrencyManager)bm;
				if (cm != null)
					cm.Refresh();
			}
		}

		private void DisplayTagDetails(Tag t)
		{
			m_tbDisplayName.Text = t.DisplayName;
			m_tbShortName.Text = t.ShortName;
			m_tbTagValue.Text = t.Value;
			m_cbMultilineTag.Checked = t.MultiLineTag;
			m_cbAngleBrackets.Checked = t.AngleBrackets;
			m_cbClosingTag.Checked = t.ClosingTag;

			string attributeText = string.Empty;

			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < t.DefaultAttributes.Length; i++)
			{
				if (i > 0)
				{
					sb.Append("\r\n");
				}

				sb.Append(t.DefaultAttributes[i]);
			}

			attributeText = sb.ToString();

			m_tbAttributes.Text = attributeText;

			if (t.InnerTags)
			{
				m_comboInnerTag.SelectedItem = t.DefaultInnerTag;
			}
			else
			{
				// Odd hack to ensure nothing's selected
				m_comboInnerTag.SelectedIndex = -1;
				m_comboInnerTag.SelectedIndex = -1;
			}
		}

		private void SelectNodeText(TreeNode tn)
		{			
			TagSelectionForm tsf = new TagSelectionForm();
			tsf.Caption = "Select Tag";
			tsf.Size = new Size(210, 130);
			tsf.Location = new Point(15, 100);
			tsf.TagList = m_sortedTags;
			tsf.ParentForm = this;

			//tsf.ShowDialog();

			DialogResult dr = tsf.ShowDialog();//DialogResult.Cancel;

			if (dr == DialogResult.OK)
			{
				switch (tsf.SelectedType)
				{
					case SelectedTagType.ExistingTag:
					case SelectedTagType.Header:
					{
						tn.Text = tsf.SelectedTagName;
						break;
					}
					case SelectedTagType.Separator:
					{
						tn.Text = "-";
						break;
					}
				}
			}
			
		}

		private void ValidateMenu()
		{
			Queue q = new Queue();
			TreeNode badNode = null;
			string message = string.Empty;
			bool isOkay = true;

			foreach (TreeNode tn in treeView1.Nodes)
			{
				q.Enqueue(tn);
			}

			while (q.Count > 0)
			{
				TreeNode tn = (TreeNode)q.Dequeue();

				// Must have text
				if (tn.Text == string.Empty)
				{
					isOkay = false;
					badNode = tn;
					message = "This node cannot have an empty name";
					break;
				}

				/* Must be either:
				 * 1) A valid tag
				 * 2) A separator (with no children)
				 * 3) A parent menu item
				 */
				if (!m_sortedTags.Contains(tn.Text))
				{
					if (tn.Text == "-")
					{
						if (tn.Nodes.Count > 0)
						{
							isOkay = false;
							badNode = tn;
							message = "A separator cannot have children";
							break;
						}
					}
					else if (tn.Nodes.Count == 0)
					{
						isOkay = false;
						badNode = tn;
						message = "This node is not a valid tag name, and is not a parent menu item";
						break;
					}
				}

				foreach (TreeNode child in tn.Nodes)
				{
					q.Enqueue(child);
				}

			}

			if (!isOkay)
			{
				treeView1.SelectedNode = badNode;
				MessageBox.Show(message, "Invalid Menu Item", MessageBoxButtons.OK,
								MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
			else
			{
				MessageBox.Show("This menu is valid", "Valid Menu", MessageBoxButtons.OK,
								MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
			}
		}

		#endregion

		#region Menu handlers
		private void m_miTagsSave_Click(object sender, EventArgs e)
		{
			SaveTagInformation();
		}

		void miDeleteMenuItem_Click(object sender, EventArgs e)
		{
			string itemText = treeView1.SelectedNode.Text;
			DialogResult dr = MessageBox.Show("Delete \"" + itemText + "\" ?", "Delete Item", MessageBoxButtons.YesNo,
												 MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

			if (dr == DialogResult.Yes)
			{
				TreeNodeCollection tnc = null;

				if (treeView1.SelectedNode.Parent != null)
				{
					tnc = treeView1.SelectedNode.Parent.Nodes;
				}
				else
				{
					tnc = treeView1.Nodes;
				}
				tnc.Remove(treeView1.SelectedNode);
			}
		}

		void miSelectTag_Click(object sender, EventArgs e)
		{
			SelectNodeText(treeView1.SelectedNode);
		}

		private void m_miTagsNew_Click(object sender, EventArgs e)
		{
			m_comboAvailableTags.SelectedIndex = -1;
			m_comboAvailableTags.SelectedIndex = -1;

			m_currentTag = m_emptyTag;//new Tag();

			DisplayTagDetails(m_currentTag);
		}

		private void m_miTagsDelete_Click(object sender, EventArgs e)
		{
			if(m_currentTag == m_emptyTag)
			{
				MessageBox.Show("Can't delete an empty/unsaved tag!", "Warning", MessageBoxButtons.OK,
											MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				return;
			}

			string message = "Delete tag \"" + m_currentTag.DisplayName + "\"?";
			DialogResult dr = MessageBox.Show(message, "Delete tag?", MessageBoxButtons.YesNo,
											MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

			if (dr == DialogResult.Yes)
			{
				m_htTags.Remove(m_currentTag.DisplayName);
				m_sortedTags.Remove(m_currentTag.DisplayName);

				RefreshTagLists();
				m_comboAvailableTags.SelectedIndex = 0;
			}
			else
			{
				return;
			}
		}

		private void m_miFileSave_Click(object sender, EventArgs e)
		{
			SaveTagFile();
			SaveMenuFile();
		}

		private void m_miTMNew_Click(object sender, EventArgs e)
		{
			TreeNode selectedNode = treeView1.SelectedNode;
			TreeNodeCollection tnc = GetParentCollection(selectedNode);

			TreeNode newNode = new TreeNode();
			newNode.Text = "New menu item";

			tnc.Add(newNode);
			treeView1.SelectedNode = newNode;

			SelectNodeText(newNode);

		}

		private void m_miTMPreview_Click(object sender, EventArgs e)
		{
			ContextMenu ct = new ContextMenu();

			Queue qNodes = new Queue();
			Queue qItems = new Queue();

			qNodes.Enqueue(treeView1.Nodes);
			qItems.Enqueue(ct.MenuItems);

			while(qNodes.Count > 0)
			{
				TreeNodeCollection tnc = (TreeNodeCollection)qNodes.Dequeue();
				Menu.MenuItemCollection mic = (Menu.MenuItemCollection)qItems.Dequeue();

				foreach(TreeNode tn in tnc)
				{
					MenuItem mi = new MenuItem();
					mi.Text = tn.Text;

					mic.Add(mi);

					qNodes.Enqueue(tn.Nodes);
					qItems.Enqueue(mi.MenuItems);
				}
			}

			ct.Show(tabControl1, new Point(10, 10));			
		}

		private void m_btnInputValue_Click(object sender, EventArgs e)
		{
			InputDialog id = new InputDialog(true);
			id.Caption = "Tag Value";
			id.InputText = m_tbTagValue.Text;
			id.Size = new Size(210, 88);
			id.Location = new Point(15, 100);
			id.ParentForm = this;

			DialogResult dr = id.ShowDialog();

			if(dr == DialogResult.OK)
			{
				m_tbTagValue.Text = id.InputText;
			}
			id.Dispose();			
		}

		private void m_miFileReload_Click(object sender, EventArgs e)
		{
			ReloadDataFiles();
		}

		private void HandleEditMenu(object sender, EventArgs e)
		{
			Control c = Utility.GetFocusedControl(tabPage1);

			if (c == null)
			{
				return;
			}

			if (!(c is TextBox))
			{
				return;
			}

			IntPtr hWnd = Utility.GetHandle(c);

			int message = 0;

			if (sender == m_miEditCopy)
			{
				message = (int)WM.COPY;
			}
			else if (sender == m_miEditCut)
			{
				message = (int)WM.CUT;
			}
			else if (sender == m_miEditPaste)
			{
				message = (int)WM.PASTE;
			}

			CoreDLL.SendMessage(hWnd, message, 0, 0);
		}

		private void m_miTMValidateMenu_Click(object sender, EventArgs e)
		{
			ValidateMenu();
		}

		#endregion

		#region Treeview manipulation
		private TreeNodeCollection GetParentCollection(TreeNode node)
		{
			TreeNode parent = node.Parent;
			if (parent == null)
			{
				return treeView1.Nodes;
			}
			else
			{
				return parent.Nodes;
			}
		}

		private void UpdateButtons(TreeNode node)
		{
			bool hasParent = (node.Parent != null);

			m_btnMoveLeft.Enabled = hasParent;

			TreeNode prevNode = PrevNode(node);
			bool prevSibling = (prevNode != null);

			m_btnMoveRight.Enabled = prevSibling;
			m_btnMoveUp.Enabled = prevSibling;

			TreeNode nextNode = NextNode(node);
			bool nextSibling = (nextNode != null);

			m_btnMoveDown.Enabled = nextSibling;
		}

		private TreeNode NextNode(TreeNode node)
		{
			TreeNodeCollection tnc = GetParentCollection(node);
			int currentIndex = tnc.IndexOf(node);

			if ((currentIndex + 1) < tnc.Count)
			{
				return tnc[currentIndex + 1];
			}
			return null;
		}

		private TreeNode PrevNode(TreeNode node)
		{
			TreeNodeCollection tnc = GetParentCollection(node);
			int currentIndex = tnc.IndexOf(node);

			if ((currentIndex > 0) && (currentIndex <= tnc.Count))
			{
				return tnc[currentIndex - 1];
			}
			return null;
		}

		private void m_btnMoveLeft_Click(object sender, EventArgs e)
		{
			TreeNode selected = treeView1.SelectedNode;

			TreeNode newSibling = selected.Parent;
			TreeNodeCollection tnc = GetParentCollection(newSibling);

			newSibling.Nodes.Remove(selected);
			tnc.Add(selected);
			treeView1.SelectedNode = selected;		
		}

		private void m_btnMoveUp_Click(object sender, EventArgs e)
		{
			TreeNode selected = treeView1.SelectedNode;

			TreeNodeCollection tnc = GetParentCollection(selected);
			int currentIndex = tnc.IndexOf(selected);

			tnc.Remove(selected);
			tnc.Insert(currentIndex - 1, selected);
			treeView1.SelectedNode = selected;
		}

		private void m_btnMoveDown_Click(object sender, EventArgs e)
		{
			TreeNode selected = treeView1.SelectedNode;

			TreeNodeCollection tnc = GetParentCollection(selected);
			int currentIndex = tnc.IndexOf(selected);

			tnc.Remove(selected);
			tnc.Insert(currentIndex + 1, selected);
			treeView1.SelectedNode = selected;
		}

		private void m_btnMoveRight_Click(object sender, EventArgs e)
		{
			TreeNode selected = treeView1.SelectedNode;

			TreeNode newParent = PrevNode(selected);
			TreeNodeCollection tnc = GetParentCollection(selected);

			tnc.Remove(selected);
			newParent.Nodes.Add(selected);
			treeView1.SelectedNode = selected;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			UpdateButtons(e.Node);
		}
		#endregion

		#region P/Invoke
		[DllImport("coredll")]
		private static extern int SetWindowLong(IntPtr hwnd, int message, int style);

		[DllImport("coredll")]
		private static extern int GetWindowLong(IntPtr hwnd, int message);

		[DllImport("coredll")]
		private static extern IntPtr GetFocus();
		#endregion
	}
}

