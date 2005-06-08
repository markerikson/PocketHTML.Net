#define TRACE

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;
using Ayende;
using ISquared.Win32Interop;
using ISquared.Win32Interop.WinEnums;
using Microsoft.WindowsCE.Forms;
using System.Diagnostics;
using ISquared.Debugging;
using OpenNETCF;
using OpenNETCF.Windows.Forms;
using HardwareButtons;
using MRUSample;


namespace ISquared.PocketHTML
{	
	public class PocketHTMLEditor : Form, IMRUClient
	{
		static void Main() 
		{
			Application.Run(new PocketHTMLEditor());
			try
			{
				
			}
			catch(Exception e)
			{
				//StreamWriter sw = new StreamWriter(GetCurrentDir(true) + "phnerror.txt", true);
				//sw.WriteLine("Uncaught exception at top level");
				//sw.WriteLine("Exception info: " + e.Message);
				//sw.Close();
				MessageBox.Show("Exception caught at the top level\nException info: " + e.Message 
					+ "\nInnerException: " + e.InnerException);
				
				Application.Exit();
				//throw e;
			}
			
		}

		#region private members

		// TODO is this actually even used?
		private ImageList il;
		private MainMenu m_mainMenu;
		private MRUManager m_mruManager;

		/// The saved settings for PocketHTML
		private Configuration m_config;
		// just used to keep from writing "\r\n" all the time
		private string newline;
		// Name of the current file
		private string m_saveFileName;
		private string m_saveFileDirectory;
		// TODO actually used?
		private ArrayList textList;
		// used by GetLeadingSpaces() to capture the number of beginning spaces
		//Regex reLeadingSpaces;
		// TODO actually used?
		Regex reNewLine;
		private OptionsPanel m_optionsDialog;
		private EditorPanel m_editorPanel;
		private Match findmatch;
		// TODO Not entirely sure if this is actually used
		private int findindex;
		internal FindDialog m_dlgFind;
		private ReplaceDialog m_dlgReplace;
		private MenuItem m_menuFileOpen;
		private MenuItem m_menuEditUndo;
		private MenuItem m_menuEditCopy;
		private MenuItem m_menuEditCut;
		private MenuItem m_menuEditPaste;
		private MenuItem m_menuEditClear;
		private MenuItem m_menuEditSelectAll;
		private MenuItem m_menuFileSave;

		// Holds all the Tag objects, keyed to the short name or the normal name
		private Hashtable m_tagHash;
		// Maps a button name (ie, "button1") to a tag name
		private Hashtable buttonTags;

		private HardwareButtonMessageWindow m_hardwareButtons;

		private bool firstOptions;
		private bool tgetfileExists;
		//private PHNOptions options;
		private bool optionsDialogHidden;
		private MenuItem menuItem4;
		private MenuItem m_menuFileExit;
		private MenuItem m_menuFileSaveAs;
		private MenuItem m_menuFile;
		private MenuItem m_menuEdit;
		private MenuItem m_menuEditSep2;
		private MenuItem m_menuEditSep1;
		private MenuItem m_menuTools;
		private MenuItem m_menuToolsOptions;
		private ImageList imageList1;
		private MenuItem m_menuToolsFind;
		private MenuItem m_menuToolsReplace;
		private MenuItem menuItem2;
		private ToolBar toolBar1;
		private MenuItem m_menuHelp;
		private MenuItem m_menuHelpAbout;
		private MenuItem m_menuHelpContents;
		private MenuItem m_menuFileNew;
		private MenuItem m_menuFileNewBlank;
		private MenuItem m_menuFileNewBasic;
		private System.Windows.Forms.ToolBarButton m_btnPreview;
		private System.Windows.Forms.ToolBarButton m_btnTags;
		private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
		private MenuItem m_menuToolsRefreshTemplates;
		private MenuItem menuItem1;
		private MenuItem menuItem5;
		private MenuItem m_menuFileRecentFiles;
		private MenuItem menuItem8;
		private MenuItem m_menuEditIndent;
		private MenuItem m_menuEditUnindent;
		private MenuItem menuItem7;
		private System.Windows.Forms.MenuItem MenuFileClose;
		
		

		#endregion


		#region properties
		public Hashtable ButtonTags
		{
			get { return buttonTags; }
			set { buttonTags = value; }
		}

		internal TextBox TextBox
		{
			get
			{
				return m_editorPanel.TextBox;
			}			
		}

		internal Hashtable TagHash
		{
			get
			{
				return this.m_tagHash;
			}
		}

		/*
		internal PHNOptions Options
		{
			get
			{
				return options;
			}
		}
		*/

		internal NamedButton[] Buttons
		{
			get
			{
				return m_editorPanel.Buttons;
			}
		}

		internal Configuration Config
		{
			get
			{
				return m_config;
			}
		}
		#endregion


		public PocketHTMLEditor()
		{			
			// TODO Convert to XML config file
			String inifile = Utility.GetCurrentDir(true) + "pockethtml.ini";

			try
			{
				if(!File.Exists(inifile))
				{
					m_config = new Configuration();

					m_config.AddSection("Options");
					m_config.AddSection("Button Tags");

					string[] defaultButtonTags = {"Line break", "Ordered list", "Unordered list", "List item",
													"Heading 1", "Paragraph", "Bold", "Italic",
													"Hyperlink", "Page anchor", "Code block", "Preformatted",
													"HTML", "Ampersand", "Table", "Form"};
					for(int i = 0; i < defaultButtonTags.Length; i++)
					{
						m_config.SetValue("Button Tags", "button" + (i + 1).ToString(), defaultButtonTags[i]);
					}

					m_config.SetValue("Options", "XHTMLTags", "True");
					m_config.SetValue("Options", "TextWrap", "True");
					m_config.SetValue("Options", "PageWrap", "True");
					m_config.SetValue("Options", "ClearType", "True");
					m_config.SetValue("Options", "AutoIndent", "True");
					m_config.SetValue("Options", "IndentHTML", "True");
					//m_config.SetValue("Options", "NumberOfButtons", "16");
					m_config.SetValue("Options", "HardwareButtonShowsMenu", "True");
					m_config.SetValue("Options", "MonospacedFont", "True");
					m_config.SetValue("Options", "HardwareButton", "Hardware1");
					
				}
				else
				{
					StreamReader sr = new StreamReader(inifile);
					m_config = new Configuration(sr);
					sr.Close();
				}
			}
			catch(DirectoryNotFoundException dnfe)
			{
				MessageBox.Show("Exception while loading pockethtml.ini\nException information: " + dnfe.Message);
				Application.Exit();
			}

			if(m_config.SectionExists("Debug"))
			{
				if(m_config.GetBool("Debug", "Text"))
				{
					TextTraceListener.Prefix = "PocketHTML";
					TextTraceListener.InstallListener();
					TextTraceListener.Reset();
				}

				if(m_config.GetBool("Debug", "TCP"))
				{
					TcpTraceListener.InstallListener("PPP_PEER", 6666);
				}
			}

			Debug.WriteLine("Configuration created");

			Debug.WriteLine("Options section exists: " + m_config.SectionExists("Options"));
			Debug.WriteLine("TextWrap exists: " + m_config.ValueExists("Options", "PageWrap"));

			
			
			Debug.WriteLine("Beginning PHE constructor");
			// Generated form setup
			InitializeComponent();

			Debug.WriteLine("InitializeComponent complete");

			RefreshTemplates();
			

			// Retrieve the embedded toolbar graphics.  Needed because
			// the designer-generated ImageList doesn't support transparency.
			Icon iconIE = Utility.GetIcon("Graphics.ie");
			Icon iconTag = Utility.GetIcon("Graphics.Tag");

			imageList1.Images.Add(iconTag);
			imageList1.Images.Add(iconIE);
			
			m_btnTags.ImageIndex = 0;
			m_btnPreview.ImageIndex = 1;
			

			m_editorPanel = new EditorPanel();
			Debug.WriteLine("EditorPanel created");
			m_editorPanel.Parent = this;

			// Hooks up each button with the handler
			// TODO Check if this optimization actually matters
			EventHandler tagClick = new EventHandler(TagButton_Click);
			for(int i = 0; i < m_editorPanel.Buttons.Length; i++)
			{
				m_editorPanel.Buttons[i].Click += tagClick;
			}
			
			// TODO Delay-load this panel
			m_optionsDialog = new OptionsPanel();
			Debug.WriteLine("OptionsDialog created");
			m_optionsDialog.Bounds = new Rectangle(0,0, 240, 270);
			m_optionsDialog.Parent = this;
			m_optionsDialog.Hide();
			m_optionsDialog.SendToBack();
			m_editorPanel.BringToFront();

			
			
			optionsDialogHidden = true;
			
			
			m_saveFileName = String.Empty;
			m_saveFileDirectory = "\\My Documents";

			m_tagHash = new Hashtable();
			buttonTags = new Hashtable();


			m_hardwareButtons = new HardwareButtonMessageWindow();
			m_hardwareButtons.HardwareButtonPressed += new HardwareButtonPressedHandler(HardwareButtonPressed);
			
			//LoadTags();
			LoadTagsXTR();
			Debug.WriteLine("Tags loaded");
			SetupButtons();
			Debug.WriteLine("Buttons created");
			SetupOptions();
			Debug.WriteLine("Options setup complete");

			newline = "\r\n";
			textList = new ArrayList();

			
			reNewLine = new Regex(
				@"\r\n",
				RegexOptions.IgnoreCase
				| RegexOptions.Multiline
				| RegexOptions.IgnorePatternWhitespace);




			// hook up auto-indent
			//m_editorPanel.TextBox.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);


			//firstEnter = true;

			// HACK firstOptions is ugly.  Get rid of it.
			firstOptions = true;

			// Note the constant.  Change this?
			tgetfileExists = File.Exists("\\Windows\\tgetfile.dll");

			findindex = 0;
			m_editorPanel.TagMenuClicked += new EditorPanel.TagMenuClickedHandler(this.InsertTag);
			
			this.ContextMenu = m_editorPanel.TextBox.ContextMenu;
			ContextMenu menuTextBox = m_editorPanel.TextBox.ContextMenu;



			menuTextBox.MenuItems[0].Click += new EventHandler(MenuCopy_Click);
			menuTextBox.MenuItems[1].Click += new EventHandler(MenuCut_Click);
			menuTextBox.MenuItems[2].Click += new EventHandler(MenuPaste_Click);

			
			//
			//RegisterHKeys.RegisterRecordKey(m_hardwareButtons.Hwnd, RegisterButtons.Hardware1);

			m_mruManager = new MRUManager();
			m_mruManager.Initialize(this, m_menuFileRecentFiles);

			int numRecentFiles = m_config.GetSectionCount("Recent Files");

			string[] recentFiles = new string[numRecentFiles];
			for(int i = 0; i < numRecentFiles; i++)
			{
				string keyName = "file" + (i + 1).ToString();
				recentFiles[i] = m_config.GetValue("Recent Files", keyName);
			}

			for(int i = recentFiles.Length - 1; i >= 0; i--)
			{
				m_mruManager.Add(recentFiles[i]);
			}
			//m_mruManager.Add("\\My Documents\\test1.html");
			//m_mruManager.Add("\\My Documents\\test2.html");
			//m_mruManager.Add("\\Program Files\\PocketHTML.Net\\PHN.html");
			
			

			Debug.WriteLine("PHE constructor complete");

			Debug.WriteLine("Test line");
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
			this.m_mainMenu = new System.Windows.Forms.MainMenu();
			this.m_menuFile = new System.Windows.Forms.MenuItem();
			this.m_menuFileNew = new System.Windows.Forms.MenuItem();
			this.m_menuFileNewBlank = new System.Windows.Forms.MenuItem();
			this.m_menuFileNewBasic = new System.Windows.Forms.MenuItem();
			this.m_menuFileOpen = new System.Windows.Forms.MenuItem();
			this.m_menuFileSave = new System.Windows.Forms.MenuItem();
			this.m_menuFileSaveAs = new System.Windows.Forms.MenuItem();
			this.MenuFileClose = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.m_menuFileRecentFiles = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.m_menuFileExit = new System.Windows.Forms.MenuItem();
			this.m_menuEdit = new System.Windows.Forms.MenuItem();
			this.m_menuEditUndo = new System.Windows.Forms.MenuItem();
			this.m_menuEditSep2 = new System.Windows.Forms.MenuItem();
			this.m_menuEditCopy = new System.Windows.Forms.MenuItem();
			this.m_menuEditCut = new System.Windows.Forms.MenuItem();
			this.m_menuEditPaste = new System.Windows.Forms.MenuItem();
			this.m_menuEditSep1 = new System.Windows.Forms.MenuItem();
			this.m_menuEditClear = new System.Windows.Forms.MenuItem();
			this.m_menuEditSelectAll = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.m_menuEditIndent = new System.Windows.Forms.MenuItem();
			this.m_menuEditUnindent = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.m_menuTools = new System.Windows.Forms.MenuItem();
			this.m_menuToolsOptions = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.m_menuToolsRefreshTemplates = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.m_menuToolsFind = new System.Windows.Forms.MenuItem();
			this.m_menuToolsReplace = new System.Windows.Forms.MenuItem();
			this.m_menuHelp = new System.Windows.Forms.MenuItem();
			this.m_menuHelpAbout = new System.Windows.Forms.MenuItem();
			this.m_menuHelpContents = new System.Windows.Forms.MenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList();
			this.il = new System.Windows.Forms.ImageList();
			this.m_btnTags = new System.Windows.Forms.ToolBarButton();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.m_btnPreview = new System.Windows.Forms.ToolBarButton();
			this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
			// 
			// m_mainMenu
			// 
			this.m_mainMenu.MenuItems.Add(this.m_menuFile);
			this.m_mainMenu.MenuItems.Add(this.m_menuEdit);
			this.m_mainMenu.MenuItems.Add(this.m_menuTools);
			this.m_mainMenu.MenuItems.Add(this.m_menuHelp);
			// 
			// m_menuFile
			// 
			this.m_menuFile.MenuItems.Add(this.m_menuFileNew);
			this.m_menuFile.MenuItems.Add(this.m_menuFileOpen);
			this.m_menuFile.MenuItems.Add(this.m_menuFileSave);
			this.m_menuFile.MenuItems.Add(this.m_menuFileSaveAs);
			this.m_menuFile.MenuItems.Add(this.MenuFileClose);
			this.m_menuFile.MenuItems.Add(this.menuItem5);
			this.m_menuFile.MenuItems.Add(this.m_menuFileRecentFiles);
			this.m_menuFile.MenuItems.Add(this.menuItem4);
			this.m_menuFile.MenuItems.Add(this.m_menuFileExit);
			this.m_menuFile.Text = "File";
			// 
			// m_menuFileNew
			// 
			this.m_menuFileNew.MenuItems.Add(this.m_menuFileNewBlank);
			this.m_menuFileNew.MenuItems.Add(this.m_menuFileNewBasic);
			this.m_menuFileNew.Text = "New";
			// 
			// m_menuFileNewBlank
			// 
			this.m_menuFileNewBlank.Text = "Blank";
			this.m_menuFileNewBlank.Click += new System.EventHandler(this.MenuFileNewBlank_Click);
			// 
			// m_menuFileNewBasic
			// 
			this.m_menuFileNewBasic.Text = "Basic";
			this.m_menuFileNewBasic.Click += new System.EventHandler(this.MenuFileNewBasic_Click);
			// 
			// m_menuFileOpen
			// 
			this.m_menuFileOpen.Text = "Open";
			this.m_menuFileOpen.Click += new System.EventHandler(this.MenuFileOpen_Click);
			// 
			// m_menuFileSave
			// 
			this.m_menuFileSave.Text = "Save";
			this.m_menuFileSave.Click += new System.EventHandler(this.MenuFileSave_Click);
			// 
			// m_menuFileSaveAs
			// 
			this.m_menuFileSaveAs.Text = "Save As";
			this.m_menuFileSaveAs.Click += new System.EventHandler(this.MenuFileSaveAs_Click);
			// 
			// MenuFileClose
			// 
			this.MenuFileClose.Text = "Close";
			// 
			// menuItem5
			// 
			this.menuItem5.Text = "-";
			// 
			// m_menuFileRecentFiles
			// 
			this.m_menuFileRecentFiles.Text = "Recent Files";
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "-";
			// 
			// m_menuFileExit
			// 
			this.m_menuFileExit.Text = "Exit";
			this.m_menuFileExit.Click += new System.EventHandler(this.MenuFileExit_Click);
			// 
			// m_menuEdit
			// 
			this.m_menuEdit.MenuItems.Add(this.m_menuEditUndo);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditSep2);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditCopy);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditCut);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditPaste);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditSep1);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditClear);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditSelectAll);
			this.m_menuEdit.MenuItems.Add(this.menuItem8);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditIndent);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditUnindent);
			this.m_menuEdit.MenuItems.Add(this.menuItem7);
			this.m_menuEdit.Text = "Edit";
			this.m_menuEdit.Popup += new System.EventHandler(this.m_menuEdit_Popup);
			// 
			// m_menuEditUndo
			// 
			this.m_menuEditUndo.Text = "Undo";
			this.m_menuEditUndo.Click += new System.EventHandler(this.MenuUndo_Click);
			// 
			// m_menuEditSep2
			// 
			this.m_menuEditSep2.Text = "-";
			// 
			// m_menuEditCopy
			// 
			this.m_menuEditCopy.Text = "Copy";
			this.m_menuEditCopy.Click += new System.EventHandler(this.MenuCopy_Click);
			// 
			// m_menuEditCut
			// 
			this.m_menuEditCut.Text = "Cut";
			this.m_menuEditCut.Click += new System.EventHandler(this.MenuCut_Click);
			// 
			// m_menuEditPaste
			// 
			this.m_menuEditPaste.Text = "Paste";
			this.m_menuEditPaste.Click += new System.EventHandler(this.MenuPaste_Click);
			// 
			// m_menuEditSep1
			// 
			this.m_menuEditSep1.Text = "-";
			// 
			// m_menuEditClear
			// 
			this.m_menuEditClear.Text = "Clear";
			this.m_menuEditClear.Click += new System.EventHandler(this.MenuClear_Click);
			// 
			// m_menuEditSelectAll
			// 
			this.m_menuEditSelectAll.Text = "Select All";
			this.m_menuEditSelectAll.Click += new System.EventHandler(this.MenuSelectAll_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Text = "-";
			// 
			// m_menuEditIndent
			// 
			this.m_menuEditIndent.Text = "Indent Text";
			this.m_menuEditIndent.Click += new System.EventHandler(this.m_menuEditIndent_Click);
			// 
			// m_menuEditUnindent
			// 
			this.m_menuEditUnindent.Text = "Unindent Text";
			this.m_menuEditUnindent.Click += new System.EventHandler(this.m_menuEditUnindent_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Text = "-";
			// 
			// m_menuTools
			// 
			this.m_menuTools.MenuItems.Add(this.m_menuToolsOptions);
			this.m_menuTools.MenuItems.Add(this.menuItem2);
			this.m_menuTools.MenuItems.Add(this.m_menuToolsRefreshTemplates);
			this.m_menuTools.MenuItems.Add(this.menuItem1);
			this.m_menuTools.MenuItems.Add(this.m_menuToolsFind);
			this.m_menuTools.MenuItems.Add(this.m_menuToolsReplace);
			this.m_menuTools.Text = "Tools";
			// 
			// m_menuToolsOptions
			// 
			this.m_menuToolsOptions.Text = "Options";
			this.m_menuToolsOptions.Click += new System.EventHandler(this.MenuToolsOptions_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "-";
			// 
			// m_menuToolsRefreshTemplates
			// 
			this.m_menuToolsRefreshTemplates.Text = "Refresh Templates";
			this.m_menuToolsRefreshTemplates.Click += new System.EventHandler(this.m_menuToolsRefreshTemplates_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Text = "-";
			// 
			// m_menuToolsFind
			// 
			this.m_menuToolsFind.Text = "Find";
			this.m_menuToolsFind.Click += new System.EventHandler(this.MenuToolsFind_Click);
			// 
			// m_menuToolsReplace
			// 
			this.m_menuToolsReplace.Text = "Replace";
			this.m_menuToolsReplace.Click += new System.EventHandler(this.MenuToolsReplace_Click);
			// 
			// m_menuHelp
			// 
			this.m_menuHelp.MenuItems.Add(this.m_menuHelpAbout);
			this.m_menuHelp.MenuItems.Add(this.m_menuHelpContents);
			this.m_menuHelp.Text = "Help";
			// 
			// m_menuHelpAbout
			// 
			this.m_menuHelpAbout.Text = "About";
			this.m_menuHelpAbout.Click += new System.EventHandler(this.MenuHelpAbout_Click);
			// 
			// m_menuHelpContents
			// 
			this.m_menuHelpContents.Text = "Contents";
			this.m_menuHelpContents.Click += new System.EventHandler(this.MenuHelpContents_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.Images.Clear();
			// 
			// il
			// 
			this.il.ImageSize = new System.Drawing.Size(16, 16);
			this.il.Images.Clear();
			// 
			// m_btnTags
			// 
			this.m_btnTags.ImageIndex = 0;
			// 
			// toolBar1
			// 
			this.toolBar1.Buttons.Add(this.m_btnTags);
			this.toolBar1.Buttons.Add(this.m_btnPreview);
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarButton1_ButtonClick);
			// 
			// m_btnPreview
			// 
			this.m_btnPreview.ImageIndex = 1;
			this.m_btnPreview.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			// 
			// inputPanel1
			// 
			this.inputPanel1.EnabledChanged += new System.EventHandler(this.inputPanel1_EnabledChanged);
			// 
			// PocketHTMLEditor
			// 
			this.ClientSize = new System.Drawing.Size(240, 268);
			this.Controls.Add(this.toolBar1);
			this.Menu = this.m_mainMenu;
			this.Text = "PocketHTML.Net";
			this.Deactivate += new System.EventHandler(this.PocketHTMLEditor_Deactivate);
			this.Activated += new System.EventHandler(this.PocketHTMLEditor_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PocketHTMLEditor_Closing);
			this.GotFocus += new System.EventHandler(this.inputPanel1_EnabledChanged);

		}
		#endregion


		private void RefreshTemplates()
		{
			m_menuFileNew.MenuItems.Clear();
			m_menuFileNew.MenuItems.Add(m_menuFileNewBlank);
			m_menuFileNew.MenuItems.Add(m_menuFileNewBasic);

			string templatePath = Utility.GetCurrentDir(true) + "templates";

			if (Directory.Exists(templatePath))
			{
				string[] templates = Directory.GetFiles(templatePath, "*.htm*");

				Array.Sort(templates);

				foreach (string template in templates)
				{
					MenuItem item = new MenuItem();
					int index = template.LastIndexOf("\\");
					string filename = template.Substring(index + 1);
					item.Text = filename;
					item.Click += new EventHandler(MenuFileNewOther_Click);
					m_menuFileNew.MenuItems.Add(item);
				}
			}
		}
		
		/// <summary>
		/// Called whenever the Input Panel is enabled or disabled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void inputPanel1_EnabledChanged(object sender, EventArgs e)
		{			
			Rectangle visible;
			visible = inputPanel1.VisibleDesktop;

			if(optionsDialogHidden)
			{
				m_editorPanel.ResizePanel(inputPanel1);
			}
				/*
			else
			{
				optionsDialog.ResizePanel(inputPanel1);
			}
			*/
		}

		/// <summary>
		/// Called whenever the user clicks one of the QuickTag buttons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TagButton_Click(object sender, EventArgs e)
		{
			NamedButton nb = sender as NamedButton;
			
			String tagName = (String)buttonTags[nb.Name];
			Tag tag = (Tag)m_tagHash[tagName];

			if(tag == null)
			{
				return;
			}
			InsertTag(tag);
		}

		// TODO Not currently used
		internal string GetButtonString(int buttonnum)
		{
			String name = m_editorPanel.Buttons[buttonnum].Name;
			return (string)buttonTags[name];
		}

		// TODO Can this be moved to the Utility class?
		/// <summary>
		/// Utility function to determine the leading spaces on a line
		/// </summary>
		/// <returns></returns>
		

		/// <summary>
		/// Overload that takes the name or short name of the tag to insert.
		/// </summary>
		/// <param name="onlyTag"></param>
		/// <returns></returns>
		private bool InsertTag(string onlyTag)
		{
			
			if(!m_tagHash.ContainsKey(onlyTag))
			{
				return false;
			}
			Tag t = (Tag)m_tagHash[onlyTag];
			InsertTag(t);
			return true;
		}
		
		/// <summary>
		/// The main tag insertion function.  
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		private bool InsertTag(Tag tag)
		{
			TextBoxEx tb = m_editorPanel.TextBox;
			//IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
			StringBuilder sb = new StringBuilder();
			String spaces = tb.GetLeadingSpaces();
			// basically, an additional level of indentation
			// TODO make this an option?
			String spacesPlus = spaces + "    ";

			int currentLineNum = tb.CurrentLine;//CoreDLL.SendMessage(pTB, (int)EM.LINEFROMCHAR, -1, 0); 

			int selstart = tb.SelectionStart;

			int newLineNum = currentLineNum;

			string seltext = tb.SelectedText;

			bool indentHTML = m_config.GetBool("Options", "IndentHTML");

			// We'll need a start tag AND an end tag
			if(tag.ClosingTag)
			{
				sb.Append(tag.StartTag);
				if(tag.MultiLineTag)
				{
					sb.Append(newline);
				}
				// 
				if(tag.InnerTags)
				{
					Tag innerTag = (Tag)m_tagHash[tag.DefaultInnerTag];
					if(indentHTML && tag.MultiLineTag)
					{
						sb.Append(spacesPlus);
					}
					sb.Append(innerTag.StartTag);
					if(tag.MultiLineTag)
					{
						sb.Append(newline);
						if(indentHTML)
						{
							sb.Append(spacesPlus);
						}
						if(seltext != String.Empty)
						{
							sb.Append(seltext);
						}
						sb.Append(newline);
						if(indentHTML)
						{
							sb.Append(spacesPlus);
						}					
					}
					sb.Append(innerTag.EndTag);					
				}

				if(tag.MultiLineTag)
				{
					if(indentHTML)
					{
						sb.Append(spaces);
					}
				}

				if( (seltext != String.Empty) && 
					(!tag.InnerTags))
				{
					sb.Append(seltext);
				}

				if(tag.MultiLineTag)
				{
					if(tag.ClosingTag)
					{
						sb.Append(newline);
					}
					sb.Append(spaces);
				}

				sb.Append(tag.EndTag);
				if(tag.MultiLineTag)
				{
					sb.Append(newline);
				}
			}
			else
			{
				sb.Append(tag.StartTag);
				if(tag.MultiLineTag)
				{
					sb.Append(newline);
					if(indentHTML)
					{
						sb.Append(spaces);
					}
				}				
			}

			//CoreDLL.SendMessageString(pTB, (int)EM.REPLACESEL, 1, sb.ToString());		
			tb.ReplaceSelection(sb.ToString());

			int charIndex = 0; 
			int cursorLocationIndex = 0;
				
			if(tag.DefaultAttributes.Length > 0)
			{
				charIndex = tb.GetLineIndex(currentLineNum);
				
				int firstUninitializedAttribute = -1;
				int temp = -1;
				for(int i = 0; i < tag.DefaultAttributes.Length; i++)
				{
					temp = tag.DefaultAttributes[i].IndexOf("\"");
					if(temp == -1)
					{
						firstUninitializedAttribute = i;
						break;
					}
				}
				
				// TODO What was I doing here?
				//int idx = tag.Name.IndexOf("\"");
				int idx = tag.Value.IndexOf("\"");
				if(firstUninitializedAttribute != -1)
				{
					string fua = tag.DefaultAttributes[firstUninitializedAttribute];
					idx = tag.StartTag.IndexOf(fua) + fua.Length + 2;
				}
				else
				{
					int t1 = tag.StartTag.IndexOf("\"") + 1;
					int t2 = tag.StartTag.IndexOf("\"", t1);
					if(t2 == -1)
					{
						idx = tag.StartTag.Length;
					}
					else
					{
						idx = t2;
					}
				}
				charIndex += idx;
				charIndex += spaces.Length;
			}
			// TODO What was I doing here?
			//else if(tag.NormalTag)
			else if(tag.AngleBrackets)
			{
				if(tag.MultiLineTag)
				{
					newLineNum += 1;

					if(tag.InnerTags)
					{
						newLineNum += 1;
						if(indentHTML)
						{
							cursorLocationIndex = spacesPlus.Length;
						}
					}
					else
					{
						if(indentHTML)
						{
							cursorLocationIndex = spaces.Length;
						}
					}

					charIndex = tb.GetLineIndex(newLineNum);//CoreDLL.SendMessage(pTB, (int)EM.LINEINDEX, newLineNum, 0);
				}

				else
				{					
					if(tag.ClosingTag)
					{		
						charIndex = m_editorPanel.TextBox.SelectionStart;
						cursorLocationIndex -= tag.EndTag.Length;//tag.StartTag.Length;

						if(tag.InnerTags)
						{
							Tag innerTag = (Tag)m_tagHash[tag.DefaultInnerTag];
							cursorLocationIndex -= innerTag.EndTag.Length;//StartTag.Length;
						}
					}
				}				
				
				charIndex += cursorLocationIndex;							
			}
			
			// TODO What was I doing here?
			//if(tag.NormalTag)
			if(tag.AngleBrackets)
			{
				//int sel = CoreDLL.SendMessage(pTB, (int)EM.SETSEL, charIndex, charIndex);
				tb.Focus();
				tb.SelectionStart = charIndex;
				tb.SelectionLength = 0;
			}

			m_editorPanel.TextBox.Modified = true;

			return true;			
		}

		/*
		void textBox1_KeyPress(Object o, KeyPressEventArgs e)
		{
			// The keypressed method uses the KeyChar property to check 
			// whether the ENTER key is pressed. 

			// If the ENTER key is pressed, the Handled property is set to true, 
			// to indicate the event is handled.
			switch(e.KeyChar)
			{
				// Enter
				case (char)13:
				{
					if (firstEnter)
					{
						firstEnter = false;

						if (m_config.GetBool("Options", "AutoIndent"))
						{
							e.Handled = true;
							m_editorPanel.TextBox.AutoIndent();
						}

						firstEnter = true;
					}
					break;
				}
				// Tab
				case (char)9:
				{

					break;
				}
			}		
		}
		 */ 

		

		private void LoadTagsXTR()
		{		
			string filename = Utility.GetCurrentDir(true) + "tags.xml";
			XmlTextReader xtr = new XmlTextReader(filename);
			xtr.WhitespaceHandling = WhitespaceHandling.None;
			xtr.MoveToContent();

			Tag currentTag = null;

			while(xtr.Read())
			{
				switch(xtr.NodeType)
				{
					case XmlNodeType.Element:
					{
						switch(xtr.Name)
						{
							case "Tag":
							{
								Tag t = new Tag();
								currentTag = t;

								xtr.MoveToFirstAttribute();
						
								t.DisplayName = xtr.Value;
								xtr.MoveToNextAttribute();
								t.Value = xtr.Value;
								xtr.MoveToNextAttribute();	
								t.AngleBrackets = Convert.ToBoolean(xtr.Value);
								xtr.MoveToNextAttribute();
								t.DefaultInnerTag = xtr.Value;
								t.InnerTags = (t.DefaultInnerTag != String.Empty);
								xtr.MoveToNextAttribute();						
								t.ShortName = xtr.Value;
								xtr.MoveToNextAttribute();	
								t.MultiLineTag = Convert.ToBoolean(xtr.Value);
								xtr.MoveToNextAttribute();
								t.ClosingTag = Convert.ToBoolean(xtr.Value);
																	
								xtr.MoveToElement();
								m_tagHash[t.DisplayName] = t;

								break;
							}
							case "Attributes":
							{
								ArrayList al = new ArrayList();
								while(xtr.Read())
								{
									if(xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "Attributes")
									{
										break;
									}
									if(xtr.NodeType == XmlNodeType.Text)
									{
										al.Add(xtr.Value);
									}
								}
								string[] attributes = (string[])al.ToArray(typeof(string));
								currentTag.DefaultAttributes = attributes;
								break;
							}
						}
						break;						
					}
				}
			}
		}
		/*
		// TODO Benchmark this sucker
		private void LoadTags()
		{
			string name = String.Empty;
			StringBuilder sberror = new StringBuilder();
			try
			{
				StreamReader sr = null;
				
				name = Utility.GetCurrentDir(true);
				name += "tags.csv";
				sr = new StreamReader(name);	
				
				ArrayList al = new ArrayList();
				string inline = null;
				string[] items;
				char[] itemSep = {','};
				char[] defattSep = {'/'};
				string[] cols = {"Name", "Individual", "InnerTag", "ShortName", "DefaultAttributes"};
							
				if(sr == null)
				{
					string err = "StreamReader was left null in LoadTags()\nFilename: " + name;

					//DebugMessage(err, new Exception("Oops :)"));
					Debug.WriteLine(err);
					Application.Exit();
				}

				while( (inline = sr.ReadLine()) != null)
				{
					al.Add(inline);
				}

				foreach(String l in al)
				{
					String line = l.Trim();
					if( (line.Length == 0) || (line[0] == '#'))
					{
						continue;
					}
					else
					{

						items = line.Split(itemSep);	
						for(int i = 0; i < items.Length; i++)
						{
							items[i] = items[i].Trim();
						}
					
						Tag t = new Tag();
						t.Name = items[0];
						t.NormalTag = Convert.ToBoolean(items[1]);
						t.IndividualTag = Convert.ToBoolean(items[2]);
						t.MultiLineTag = Convert.ToBoolean(items[3]);
						t.DefaultInnerTag = items[4];
						if(t.DefaultInnerTag != String.Empty)
						{
							t.InnerTags = true;
						}
						t.ShortName = items[5];
						if(items[6] != String.Empty)
						{					
							t.DefaultAttributes = items[6].Split(defattSep);
							for(int i = 0; i < t.DefaultAttributes.Length; i++)
							{
								t.DefaultAttributes[i] = t.DefaultAttributes[i].Trim();
							}
						}
						else
						{
							t.DefaultAttributes = new String[0];						
						}
						
						if(t.ShortName != String.Empty)
						{
							tagHash[t.ShortName] = t;
						}
						else
						{
							tagHash[t.Name] = t;
						}

						t.Config = this.config;
					}
				}
			}
			catch(DirectoryNotFoundException)
			{
				//StreamWriter sw = new StreamWriter(GetCurrentDir(true) + "phnerror.txt", true);
				sberror.Append("Exception while loading tags.csv\n");
				sberror.Append("Filename: " + name);
				//DebugMessage(sberror.ToString(), dnfe);
				Debug.WriteLine(sberror.ToString());
				
				Application.Exit();
			}
			catch(Exception e)
			{
				sberror.Append("Caught generic exception in LoadTags()\n");
				sberror.Append("Filename: " + name + "\n");
				Debug.WriteLine(sberror.ToString() + e);
				Application.Exit();
			}
		}
		*/

		internal void SetupButtons()
		{
			for(int i = 0; i < m_editorPanel.Buttons.Length; i++)
			{
				NamedButton nb = m_editorPanel.Buttons[i];
				
				if(m_config.ValueExists("Button Tags", nb.Name))
				{				
					string tagName = m_config.GetValue("Button Tags", nb.Name);

					Tag t = (Tag)m_tagHash[tagName];

					if(t == null)
					{
						continue;
					}

					if(t.ShortName != String.Empty)
					{
						nb.Text = t.ShortName;
					}
					else
					{
						nb.Text = t.Value;
					}
					
					
					buttonTags[nb.Name] = t.DisplayName;
				}
				else
				{
					nb.Text = "N/A";
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetString"></param>
		/// <param name="index"></param>
		/// <param name="findregex"></param>
		/// <param name="scroll"></param>
		/// <returns></returns>
		// HACK This is horribly intertwined with FloatingDialog.  There's gotta be a better way
		internal bool Find(string targetString, int index,
			Regex findregex, bool scroll)
		{
			StringBuilder sb = new StringBuilder();
			//this.findindex = ed.TextBox.SelectionStart + ed.TextBox.SelectionLength;

			findmatch = findregex.Match(targetString, index);			

			bool bRet;
			if(findmatch.Success)
			{				
				m_editorPanel.TextBox.Select(findmatch.Index, findmatch.Length);
				findindex = findmatch.Index;

				if(scroll)
				{
					IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
					CoreDLL.SendMessage(pTB, (int)EM.SCROLLCARET, 0, 0);
				}

				bRet = true;
			}
			else
			{
				findmatch = null;
				findindex = 0;
				bRet = false;
			}
			
			return bRet;
		}



		private void SetupOptions()
		{
			try
			{
				Debug.WriteLine("Starting SetupOptions");
				bool tw = m_config.GetBool("Options", "TextWrap");
				Debug.WriteLine("TextWrap: " + tw);
				Debug.WriteLine("ed: " + m_editorPanel.ToString());
				Debug.WriteLine("ed.TextBox: " + m_editorPanel.TextBox.ToString());

				m_editorPanel.TextBox.WordWrap = tw;
				Debug.WriteLine("Set TextWrap");
				//ed.HtmlControl.WordWrap = config.GetBool("Options", "PageWrap");

				m_editorPanel.HtmlControl.ShrinkMode = m_config.GetBool("Options", "PageWrap");
				Debug.WriteLine("Set PageWrap");

				if(m_editorPanel.TextBox.WordWrap)
				{
					m_editorPanel.TextBox.ScrollBars = ScrollBars.Vertical;
				}
				else
				{
					m_editorPanel.TextBox.ScrollBars = ScrollBars.Both;
				}

				Debug.WriteLine("Set scrollbars");

				m_editorPanel.TextBox.AutoIndent = m_config.GetBool("Options", "AutoIndent");
				// Update the textbox to use 4-space tabs (4 dialog units per character)
				Win32Interop.Utility.SetTabStop(m_editorPanel.TextBox);

				/*
				bool useHardwareButton = m_config.GetBool("Options", "HardwareButtonShowsMenu");

				if(useHardwareButton)
				{
					string buttonName = m_config.GetValue("Options", "HardwareButton");
					RegisterButtons buttons = (RegisterButtons)EnumEx.Parse(typeof(RegisterButtons), buttonName);

					if(buttons != RegisterHKeys.RegisteredButtons)
					{
						RegisterHKeys.UnregisterRecordKey(m_hardwareButtons.Hwnd, RegisterHKeys.RegisteredButtons);
						RegisterHKeys.RegisterRecordKey(m_hardwareButtons.Hwnd, buttons);
					}					
				}
				else
				{
					RegisterHKeys.UnregisterRecordKey(m_hardwareButtons.Hwnd, RegisterHKeys.RegisteredButtons);			
				}
				*/

				UpdateHardwareButton();


				bool useMonospaceFont = Boolean.Parse(m_config.GetValue("Options", "MonospacedFont"));

				Font f = null;
				if(useMonospaceFont)
				{
					f = new Font("Courier New", 8, FontStyle.Regular);
				}
				else
				{
					f = new Font("Tahoma", 8, FontStyle.Regular);
				}

				m_editorPanel.TextBox.Font = f;

				
			}
			catch(Exception e)
			{
				Debug.WriteLine("Caught exception: " + e.ToString());
			}
			
		}

		private void UpdateHardwareButton()
		{
			UpdateHardwareButton(true);
		}

		private void UpdateHardwareButton(bool activate)
		{
			bool useHardwareButton = m_config.GetBool("Options", "HardwareButtonShowsMenu");

			if (useHardwareButton && activate)
			{
				string buttonName = m_config.GetValue("Options", "HardwareButton");
				RegisterButtons buttons = (RegisterButtons)EnumEx.Parse(typeof(RegisterButtons), buttonName);

				if (buttons != RegisterHKeys.RegisteredButtons)
				{
					RegisterHKeys.UnregisterRecordKey(m_hardwareButtons.Hwnd, RegisterHKeys.RegisteredButtons);
					RegisterHKeys.RegisterRecordKey(m_hardwareButtons.Hwnd, buttons);
				}
			}
			else
			{
				RegisterHKeys.UnregisterRecordKey(m_hardwareButtons.Hwnd, RegisterHKeys.RegisteredButtons);
			}
		}


		// TODO Either move these to a new TextBox class, or use OpenNetCF's TextBoxEx
		/*
		private void Copy()
		{
			IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
			CoreDLL.SendMessage(pTB, (int)WM.COPY, 0, 0);
		}

		private void Undo()
		{
			IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
			CoreDLL.SendMessage(pTB, (int)WM.UNDO, 0, 0);
		}

		private void Paste()
		{
			IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
			CoreDLL.SendMessage(pTB, (int)WM.PASTE, 0, 0);
			m_editorPanel.TextBox.Modified = true;
		}

		private void Clear()
		{
			IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
			CoreDLL.SendMessage(pTB, (int)EM.SETSEL, 0, -1);
			CoreDLL.SendMessage(pTB, (int)WM.CLEAR, 0, 0);
		}

		private void Cut()
		{
			IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
			CoreDLL.SendMessage(pTB, (int)WM.CUT, 0, 0);
		}
		*/ 

		private void MenuUndo_Click(object sender, EventArgs e)
		{
			//Undo();
			m_editorPanel.TextBox.Undo();
		}

		private void MenuCopy_Click(object sender, EventArgs e)
		{
			//Copy();
			m_editorPanel.TextBox.Copy();
		}

		private void MenuCut_Click(object sender, EventArgs e)
		{
			//Cut();
			IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
			m_editorPanel.TextBox.Cut();
		}

		private void MenuPaste_Click(object sender, EventArgs e)
		{
			//Paste();
			m_editorPanel.TextBox.Paste();
		}

		private void MenuClear_Click(object sender, EventArgs e)
		{
			//Clear();
			m_editorPanel.TextBox.Clear();
		}

		private void MenuSelectAll_Click(object sender, EventArgs e)
		{
			//IntPtr pTB1 = CoreDLL.GetHandle(m_editorPanel.TextBox);
			//CoreDLL.SendMessage(pTB1, (int)EM.SETSEL, 0, -1);
			m_editorPanel.TextBox.SelectAll();
		}

		private void MenuFileOpen_Click(object sender, EventArgs e)
		{
			OpenFile();
		}


		private void OpenFile()
		{
			string filename = GetFileName(false);

			if(filename != String.Empty)
			{
				LoadFile(filename);	
			}
		}

		private string GetFileName(bool save)
		{
			string filename = String.Empty;			

			StringBuilder sb = new StringBuilder();

			sb.Append("HTML files (*.html, *.htm)\0*.html;*.htm\0");
			sb.Append("ASP files (*.asp, *.aspx)\0*.asp;*.aspx\0");
			sb.Append("PHP files (*.php)\0*.php\0");
			sb.Append("XML files (*.xml)\0*.xml\0");
			sb.Append("Text files (*.txt)\0*.txt\0");
			sb.Append("All files (*.*)\0*.*\0\0");

			string fileFilter = sb.ToString();
			int filterIndex = 1;
			string originalFileName = null;

			if(save && m_saveFileName != String.Empty)
			{
				originalFileName = Path.GetFileName(m_saveFileName);

				string extension = Path.GetExtension(m_saveFileName);//m_saveFileName.Substring(dotIndex + 1);

				switch(extension)
				{
					case ".htm":
					case ".html":
					{
						filterIndex = 1;
						break;
					}
					case ".asp":
					case ".aspx":
					{
						filterIndex = 2;
						break;
					}
					case ".php":
					{
						filterIndex = 3;
						break;
					}
					case ".xml":
					{
						filterIndex = 4;
						break;
					}
					case ".txt":
					{
						filterIndex = 5;
						break;
					}
					default:
					{
						filterIndex = 6;
						break;
					}
			
				}			
			}

			if(!tgetfileExists)
			{
			
				FileDialog fd;

				if(save)
				{
					fd = new SaveFileDialog();
					fd.FileName = originalFileName;
				}
				else
				{
					fd = new OpenFileDialog();
				}
				//OpenFileDialog ofd = new OpenFileDialog();
				fd.Filter = fileFilter;//"HTML files (*.html)|*.html;|All files (*.*)|*.*";
				fd.InitialDirectory = m_saveFileDirectory;
				fd.FilterIndex = filterIndex;
				DialogResult dr = fd.ShowDialog();

				if(dr == DialogResult.OK)
				{
					filename = fd.FileName;
				}
			}
			else
			{
				filename = TGetFile.TGetFileName(save, originalFileName, filterIndex, fileFilter, m_saveFileDirectory);

			}

			return filename;
		}

		private void LoadFile(string filename)
		{
			StreamReader sr = new StreamReader(filename);
			String s =  sr.ReadToEnd();		
			sr.Close();
			//CoreDLL.SendMessageString(CoreDLL.GetHandle(m_editorPanel.TextBox), 
			//	(int)WM.SETTEXT, 0, s);
			m_editorPanel.TextBox.Text = s;
			this.m_saveFileName = filename;
			m_saveFileDirectory = Path.GetDirectoryName(filename);
			m_editorPanel.TextBox.Modified = false;

			Win32Interop.Utility.SetTabStop(m_editorPanel.TextBox);

			m_mruManager.Add(filename);
		}

		private void MenuFileSave_Click(object sender, EventArgs e)
		{
			SaveFile(false);
		}

		private void SaveFile(bool saveas)
		{
			string filename = String.Empty;
			Debug.WriteLine("Saving file.  saveas: " + saveas);
			Debug.WriteLine("Current file name: " + m_saveFileName);

			if( (m_saveFileName == String.Empty) ||
				(saveas) )
			{
				filename = GetFileName(true);
				/*
				if(!tgetfileExists)
				{
					SaveFileDialog sfd = new SaveFileDialog();
					sfd.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
					DialogResult dr = sfd.ShowDialog();

					if(dr == DialogResult.OK)
					{	
						name  = sfd.FileName;

						if(sfd.FilterIndex == 1)
						{
							int idx = name.LastIndexOf(".");
							string justname = name.Substring(0, idx);
							name = justname + ".html";
						}	
					}
					else
					{
						return;
					}
				}
				else
				{
					name = TGetFile.TGetFileName(false);
					if(name == String.Empty)
					{
						return;
					}					
				}				
				*/

				m_saveFileName = filename;
			}
			else
			{
				filename = m_saveFileName;
			}			
				
			if(filename == String.Empty)
			{
				return;
			}

			m_saveFileDirectory = Path.GetDirectoryName(filename);
			StreamWriter sw = new StreamWriter(filename);
			sw.WriteLine(m_editorPanel.TextBox.Text);			
			sw.Close();
			m_editorPanel.TextBox.Modified = false;
			m_mruManager.Add(filename);
		}

		private DialogResult CloseFile()
		{
			DialogResult dr = DialogResult.No;
			if(m_editorPanel.TextBox.Modified)
			{
				dr = MessageBox.Show("You have unsaved changes.  Do you want to save before you quit?",
					"Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1);
				if(dr == DialogResult.Yes)
				{
					SaveFile(false);	
				}
			}
			else
			{
				dr = DialogResult.Yes;
			}
			return dr;
		}

		private void PocketHTMLEditor_Closing(object sender, CancelEventArgs e)
		{
			DialogResult dr = CloseFile();
			if(dr == DialogResult.Cancel)
			{
				e.Cancel = true;
			}
			else
			{
				//ed.HtmlControl.DestroyHTMLControl();
				m_editorPanel.HtmlControl.Dispose();

				string[] recentFiles = m_mruManager.FileNames;

				for(int i = 0; i < recentFiles.Length; i++)
				{
					m_config.SetValue("Recent Files", "file" + (i + 1).ToString(), recentFiles[i]);
				}

				String inifile = Utility.GetCurrentDir(true) + "pockethtml.ini";
				StreamWriter sw = new StreamWriter(inifile);
				m_config.Save(sw);
				sw.Close();
			}			
		} 

		private void PocketHTMLEditor_GotFocus(object sender, EventArgs e)
		{
			inputPanel1_EnabledChanged(sender, e);
		}

		internal void MenuToolsOptions_Click(object sender, EventArgs e)
		{
			if(optionsDialogHidden)
			{
				m_editorPanel.Hide();
				// HACK Ugly hack.  Gotta be a better way to do it.
				if(firstOptions)
				{
					m_optionsDialog.Load(this);
					
				}
				else
				{
					m_optionsDialog.Reset();					
				}
				m_optionsDialog.Show();

				firstOptions = false;
				optionsDialogHidden = false;
				foreach(MenuItem m in m_mainMenu.MenuItems)
				{
					m.Enabled = false;
				}
			}
			else
			{
				if(m_optionsDialog.OK)
				{
					SetupButtons();
					SetupOptions();
				}
				
				m_optionsDialog.Hide();
				m_editorPanel.Show();
				optionsDialogHidden = true;
				foreach(MenuItem m in m_mainMenu.MenuItems)
				{
					m.Enabled = true;
				}
				
				inputPanel1_EnabledChanged(sender, e);
				m_editorPanel.TextBox.Focus();				
			}
		}

		private void MenuFileSaveAs_Click(object sender, EventArgs e)
		{
			SaveFile(true);
		}

		private void MenuFileClose_Click(object sender, EventArgs e)
		{
			DialogResult dr = CloseFile();
			if(dr != DialogResult.Cancel)
			{
				m_editorPanel.TextBox.Text = String.Empty;
				m_editorPanel.TextBox.Modified = false;
				this.m_saveFileName = String.Empty;
			}
		}

		private void MenuFileExit_Click(object sender, EventArgs e)
		{
			this.Close();			
		}

		/*
		private void PocketHTMLEditor_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Point p = new Point(e.X, e.Y);
			Size s = ed.TextBox.Size;
			if( (p.X <= s.Width) &&
				(p.Y <= s.Height ))
			{
				MessageBox.Show(p.X.ToString() + ", " + p.Y.ToString());
			}
		}
		*/

		// TODO Remove 240x320 specific code
		private void toolBarButton1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if(e.Button == this.m_btnTags)
			{
				this.ContextMenu.Show(this, new Point(80, 160));;
			}
				
			else if(e.Button == this.m_btnPreview)
			{
				m_editorPanel.SwapPanels(e.Button.Pushed);
			}		
		}

		public void HardwareButtonPressed(int buttonNumber)
		{
			ContextMenu.Show(this, new Point(80, 160));
		}

		// TODO Remove 240x320 specific code
		private void MenuToolsFind_Click(object sender, EventArgs e)
		{
			if(this.m_dlgFind == null)
			{				
				m_dlgFind = new FindDialog(this);
				m_dlgFind.FormBorderStyle = FormBorderStyle.None;
				int x = (240 - m_dlgFind.Width) / 2;
				int y = 100;
				m_dlgFind.Location = new Point(x, y);
				m_dlgFind.MinimizeBox = false;
				m_dlgFind.MaximizeBox = false;
				m_dlgFind.ControlBox = false;
			}

			if(m_dlgReplace != null && m_dlgReplace.Visible)
			{
				m_dlgReplace.Hide();
			}
			m_dlgFind.Show();
			m_dlgFind.BringToFront();
			m_dlgFind.FindTextBox.Text = m_editorPanel.TextBox.SelectedText;
			m_dlgFind.FindTextBox.Focus();			
		}

		// TODO Remove 240x320 specific code
		private void MenuToolsReplace_Click(object sender, EventArgs e)
		{
			if(this.m_dlgReplace == null)
			{				
				m_dlgReplace = new ReplaceDialog(this);
				m_dlgReplace.FormBorderStyle = FormBorderStyle.None;
				int x = (240 - m_dlgReplace.Width) / 2;
				int y = 100;
				m_dlgReplace.Location = new Point(x, y);
				m_dlgReplace.MinimizeBox = false;
				m_dlgReplace.MaximizeBox = false;
				m_dlgReplace.ControlBox = false;
			}

			if(m_dlgFind != null && m_dlgFind.Visible)
			{
				m_dlgFind.Hide();
			}

			m_dlgReplace.FindTextBox.Text = m_editorPanel.TextBox.SelectedText;
			m_dlgReplace.Show();
			m_dlgReplace.BringToFront();
			m_dlgReplace.FindTextBox.Focus();	
		}

		private void MenuHelpContents_Click(object sender, EventArgs e)
		{
			CoreDLL.CreateProcess("peghelp.exe", @"\Windows\PHN.html#contents", IntPtr.Zero,
				IntPtr.Zero, 0, 0, IntPtr.Zero, IntPtr.Zero, null, null);
		}

		private void MenuHelpAbout_Click(object sender, EventArgs e)
		{
			m_editorPanel.ShowAbout();
		}

		private void MenuFileNewBlank_Click(object sender, EventArgs e)
		{
			NewFile("blank");
		}

		private void MenuFileNewBasic_Click(object sender, EventArgs e)
		{
			NewFile("basic");
		}

		private void MenuFileNewOther_Click(object sender, EventArgs e)
		{
			MenuItem item = sender as MenuItem;
			NewFile(item.Text);
			
		}

		private void NewFile(string template)
		{
			DialogResult dr = CloseFile();
			if(dr != DialogResult.Cancel)
			{
				string templateText = String.Empty;
				switch(template)
				{
					case "blank":
						templateText = String.Empty;
						break;
					case "basic":
						templateText = "<html>\r\n<body>\r\n\r\n\r\n</body>\r\n</html>";
						m_editorPanel.TextBox.Modified = true;
						break;
					default:
						string filename = Utility.GetCurrentDir(true) + "templates\\" + template;
						if(File.Exists(filename))
						{
							string originalDirectory = m_saveFileDirectory;
							LoadFile(filename);
							m_mruManager.Remove(filename);
							m_saveFileName = String.Empty;
							m_saveFileDirectory = originalDirectory;
							m_editorPanel.TextBox.Modified = true;
							return;
						}				
						break;
				}
				m_editorPanel.TextBox.Text = templateText;
				m_editorPanel.TextBox.Modified = false;
				this.m_saveFileName = String.Empty;
				
			}
		}

		private void m_menuToolsRefreshTemplates_Click(object sender, EventArgs e)
		{
			RefreshTemplates();
		}


		#region IMRUClient Members

		void IMRUClient.OpenMRUFile(string fileName)
		{
			if(CloseFile() == DialogResult.Yes)
			{
				LoadFile(fileName);
			}
			
		}

		#endregion

		private void PocketHTMLEditor_Activated(object sender, EventArgs e)
		{
			UpdateHardwareButton(true);
		}

		private void PocketHTMLEditor_Deactivate(object sender, EventArgs e)
		{
			UpdateHardwareButton(false);
		}

		private void m_menuEditIndent_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.IndentSelection();
		}

		private void m_menuEditUnindent_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.UnindentSelection();
		}

		private void m_menuEdit_Click(object sender, EventArgs e)
		{

		}

		private void m_menuEdit_Popup(object sender, EventArgs e)
		{
			m_menuEditUndo.Enabled = m_editorPanel.TextBox.CanUndo;
		}


} // end of PocketHTMLEditor


	class NamedButton : Button
	{
		private string m_name;

		public NamedButton() {}

		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		} 
	}		
}