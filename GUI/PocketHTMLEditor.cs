#define TRACE

#region Using directives
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
using ISquared.Windows.Forms;
using Microsoft.WindowsCE.Forms;
using System.Diagnostics;
using ISquared.Debugging;
using OpenNETCF;
using OpenNETCF.Windows.Forms;
using OpenNETCF.Win32;
using HardwareButtons;
using MRUSample;
using ISquared.PocketHTML;
#endregion

namespace ISquared.PocketHTML
{	
	public class PocketHTMLEditor : Form, IMRUClient
	{
		#region Main
		public static void Main() 
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
		#endregion

		#region private members
		private MainMenu m_mainMenu;
		private MRUManager m_mruManager;

		/// The saved settings for PocketHTML
		private static Configuration m_config;
		private static string m_versionText = "1.2 Beta 7";

		// Name of the current file
		private string m_saveFileName;
		private string m_saveFileDirectory;

		private OptionsPanel m_optionsPanel;
		private EditorPanel m_editorPanel;
		internal FindDialog m_dlgFind;
		private ReplaceDialog m_dlgReplace;

		#region Menu items
		private MenuItem m_menuFileOpen;
		private MenuItem m_menuEditUndo;
		private MenuItem m_menuEditCopy;
		private MenuItem m_menuEditCut;
		private MenuItem m_menuEditPaste;
		private MenuItem m_menuEditClear;
		private MenuItem m_menuEditSelectAll;
		private MenuItem m_menuFileSave;
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
		private MenuItem m_menuEditFind;
		private MenuItem m_menuEditReplace;
		private MenuItem menuItem2;		
		private MenuItem m_menuHelp;
		private MenuItem m_menuHelpAbout;
		private MenuItem m_menuHelpContents;
		private MenuItem m_menuFileNew;
		private MenuItem m_menuFileNewBlank;
		private MenuItem m_menuFileNewBasic;
		private MenuItem m_menuToolsRefreshTemplates;
		private MenuItem menuItem1;
		private MenuItem menuItem5;
		private MenuItem m_menuFileRecentFiles;
		private MenuItem menuItem8;
		private MenuItem m_menuToolsIndent;
		private MenuItem m_menuToolsUnindent;
		private System.Windows.Forms.MenuItem MenuFileClose;
		#endregion

		private ToolBar toolBar1;

		// Holds all the Tag objects, keyed to the short name or the normal name
		private Hashtable m_htTags;
		// Maps a button name (ie, "button1") to a tag name
		private Hashtable buttonTags;

		private HardwareButtonMessageWindow m_hardwareButtons;

		private bool tgetfileExists;
		//private PHNOptions options;
		private bool m_optionsDialogHidden;
		
		private System.Windows.Forms.ToolBarButton m_btnPreview;
		private System.Windows.Forms.ToolBarButton m_btnTags;
		private MenuItem m_menuToolsRefreshTags;
		private MenuItem m_menuToolsLaunchTagEditor;
		private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;

		
		
		

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
				return this.m_htTags;
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

		internal static Configuration Config
		{
			get
			{
				return m_config;
			}
		}

		public static string VersionText
		{
			get { return PocketHTMLEditor.m_versionText; }
		}
		#endregion

		#region Constructor

		public PocketHTMLEditor()
		{			
			String inifile = Utility.GetCurrentDir(true) + "pockethtml.ini";

			try
			{
				// If there's no ini file, set up default options
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
					m_config.SetValue("Options", "ZoomLevel", "1");
					m_config.SetValue("Options", "DefaultEncoding", "utf-8");
					
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

			#if DEBUG
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
			#endif
			Debug.WriteLine("Configuration created");

			Debug.WriteLine("Options section exists: " + m_config.SectionExists("Options"));
			Debug.WriteLine("TextWrap exists: " + m_config.ValueExists("Options", "PageWrap"));
						
			Debug.WriteLine("Beginning PHE construction");

			// Generated form setup
			InitializeComponent();

			Debug.WriteLine("InitializeComponent complete");
			
			RefreshTemplates();

			Debug.WriteLine("Loading EditorPanel");
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

			m_editorPanel.BringToFront();
			m_optionsDialogHidden = true;
			
			m_saveFileName = String.Empty;
			m_saveFileDirectory = "\\My Documents";

			m_htTags = new Hashtable();
			buttonTags = new Hashtable();

			m_hardwareButtons = new HardwareButtonMessageWindow();
			m_hardwareButtons.HardwareButtonPressed += new HardwareButtonPressedHandler(HardwareButtonPressed);
			
			Debug.WriteLine("Loading tags");
			m_htTags = PocketHTMLShared.LoadTagsXTR();
			Debug.WriteLine("Tags loaded, loading buttons");
			SetupButtons();
			Debug.WriteLine("Buttons created, checking options");
			SetupOptions();
			Debug.WriteLine("Options setup complete");

			string tgetfilePath = CoreDLL.SystemDirectory + "\\tgetfile.dll";
			tgetfileExists = File.Exists(tgetfilePath);

			m_editorPanel.TagMenuClicked += new EditorPanel.TagMenuClickedHandler(this.InsertTag);
			
			this.ContextMenu = m_editorPanel.TextBox.ContextMenu;
			ContextMenu menuTextBox = m_editorPanel.TextBox.ContextMenu;

			menuTextBox.MenuItems[0].Click += new EventHandler(MenuCopy_Click);
			menuTextBox.MenuItems[1].Click += new EventHandler(MenuCut_Click);
			menuTextBox.MenuItems[2].Click += new EventHandler(MenuPaste_Click);

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

			DpiHelper.AdjustAllControls(this);

			Debug.WriteLine("PHE constructor complete");

		}

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
			this.m_menuEditFind = new System.Windows.Forms.MenuItem();
			this.m_menuEditReplace = new System.Windows.Forms.MenuItem();
			this.m_menuTools = new System.Windows.Forms.MenuItem();
			this.m_menuToolsOptions = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.m_menuToolsLaunchTagEditor = new System.Windows.Forms.MenuItem();
			this.m_menuToolsRefreshTags = new System.Windows.Forms.MenuItem();
			this.m_menuToolsRefreshTemplates = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.m_menuToolsIndent = new System.Windows.Forms.MenuItem();
			this.m_menuToolsUnindent = new System.Windows.Forms.MenuItem();
			this.m_menuHelp = new System.Windows.Forms.MenuItem();
			this.m_menuHelpAbout = new System.Windows.Forms.MenuItem();
			this.m_menuHelpContents = new System.Windows.Forms.MenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList();
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
			this.MenuFileClose.Click += new System.EventHandler(this.MenuFileClose_Click);
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
			this.m_menuEdit.MenuItems.Add(this.m_menuEditFind);
			this.m_menuEdit.MenuItems.Add(this.m_menuEditReplace);
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
			// m_menuEditFind
			// 
			this.m_menuEditFind.Text = "Find";
			this.m_menuEditFind.Click += new System.EventHandler(this.MenuEditFind_Click);
			// 
			// m_menuEditReplace
			// 
			this.m_menuEditReplace.Text = "Replace";
			this.m_menuEditReplace.Click += new System.EventHandler(this.MenuEditReplace_Click);
			// 
			// m_menuTools
			// 
			this.m_menuTools.MenuItems.Add(this.m_menuToolsOptions);
			this.m_menuTools.MenuItems.Add(this.menuItem2);
			this.m_menuTools.MenuItems.Add(this.m_menuToolsLaunchTagEditor);
			this.m_menuTools.MenuItems.Add(this.m_menuToolsRefreshTags);
			this.m_menuTools.MenuItems.Add(this.m_menuToolsRefreshTemplates);
			this.m_menuTools.MenuItems.Add(this.menuItem1);
			this.m_menuTools.MenuItems.Add(this.m_menuToolsIndent);
			this.m_menuTools.MenuItems.Add(this.m_menuToolsUnindent);
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
			// m_menuToolsLaunchTagEditor
			// 
			this.m_menuToolsLaunchTagEditor.Text = "Launch Tag Editor";
			this.m_menuToolsLaunchTagEditor.Click += new System.EventHandler(this.m_menuToolsLaunchTagEditor_Click);
			// 
			// m_menuToolsRefreshTags
			// 
			this.m_menuToolsRefreshTags.Text = "Refresh Tags";
			this.m_menuToolsRefreshTags.Click += new System.EventHandler(this.m_menuToolsRefreshTags_Click);
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
			// m_menuToolsIndent
			// 
			this.m_menuToolsIndent.Text = "Indent Text";
			this.m_menuToolsIndent.Click += new System.EventHandler(this.m_menuToolsIndent_Click);
			// 
			// m_menuToolsUnindent
			// 
			this.m_menuToolsUnindent.Text = "Unindent Text";
			this.m_menuToolsUnindent.Click += new System.EventHandler(this.m_menuToolsUnindent_Click);
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
			this.Closed += new System.EventHandler(this.PocketHTMLEditor_Closed);
			this.Activated += new System.EventHandler(this.PocketHTMLEditor_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PocketHTMLEditor_Closing);
			this.GotFocus += new System.EventHandler(this.inputPanel1_EnabledChanged);
			this.Load += new System.EventHandler(this.PocketHTMLEditor_Load);

		}
		#endregion
#endregion
		
		#region Tag insertion functions
		/// <summary>
		/// Called whenever the user clicks one of the QuickTag buttons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TagButton_Click(object sender, EventArgs e)
		{
			NamedButton nb = sender as NamedButton;
			
			String tagName = (String)buttonTags[nb.Name];
			InsertTag(tagName);
			/*
			Tag tag = (Tag)m_htTags[tagName];

			if(tag == null)
			{
				return;
			}
			PocketHTMLShared.InsertTag(tag);
			*/
		}

		/// <summary>
		/// Overload that takes the name or short name of the tag to insert.
		/// </summary>
		/// <param name="onlyTag"></param>
		/// <returns></returns>
		private bool InsertTag(string tagName)
		{

			if (!m_htTags.ContainsKey(tagName))
			{
				return false;
			}
			Tag t = (Tag)m_htTags[tagName];
			//InsertTag(t);
			PocketHTMLShared.InsertTag(t, m_editorPanel.TextBox, m_htTags);
			return true;
		}
		
		#endregion

		#region Setup functions
		/*
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
		*/

		internal void SetupButtons()
		{
			for(int i = 0; i < m_editorPanel.Buttons.Length; i++)
			{
				NamedButton nb = m_editorPanel.Buttons[i];
				
				if(m_config.ValueExists("Button Tags", nb.Name))
				{				
					string tagName = m_config.GetValue("Button Tags", nb.Name);

					Tag t = (Tag)m_htTags[tagName];

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

		private void SetupOptions()
		{
			try
			{
				bool tw = m_config.GetBool("Options", "TextWrap");
				Debug.WriteLine("TextWrap: " + tw);

				m_editorPanel.TextBox.WordWrap = tw;
				Debug.WriteLine("Set TextWrap");

				m_editorPanel.HtmlControl.ShrinkMode = m_config.GetBool("Options", "PageWrap");
				Debug.WriteLine("Set PageWrap");

				int zoomLevel = int.Parse(m_config.GetValue("Options", "ZoomLevel"));
				m_editorPanel.HtmlControl.ZoomLevel = zoomLevel;

				bool xhtmlTags = m_config.GetBool("Options", "XHTMLTags");
				Tag.XHTMLTags = xhtmlTags;

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
				InteropUtility.SetTabStop(m_editorPanel.TextBox);

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
		#endregion

		#region Hardware button functions
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

		public void HardwareButtonPressed(int buttonNumber)
		{
			ContextMenu.Show(this, new Point(80, 160));
		}
		#endregion

		#region File menu handlers
		private void MenuFileSave_Click(object sender, EventArgs e)
		{
			SaveFile(false);
		}

		private void MenuFileOpen_Click(object sender, EventArgs e)
		{
			OpenFile();
		}

		private void MenuFileSaveAs_Click(object sender, EventArgs e)
		{
			SaveFile(true);
		}

		private void MenuFileClose_Click(object sender, EventArgs e)
		{
			DialogResult dr = CloseFile();
			if (dr != DialogResult.Cancel)
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
		#endregion

		#region Edit menu handlers
		private void MenuUndo_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.Undo();
		}

		private void MenuCopy_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.Copy();
		}

		private void MenuCut_Click(object sender, EventArgs e)
		{
			IntPtr pTB = CoreDLL.GetHandle(m_editorPanel.TextBox);
			m_editorPanel.TextBox.Cut();
		}

		private void MenuPaste_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.Paste();
		}

		private void MenuClear_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.Clear();
		}

		private void MenuSelectAll_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.SelectAll();
		}

		private void m_menuEdit_Popup(object sender, EventArgs e)
		{
			m_menuEditUndo.Enabled = m_editorPanel.TextBox.CanUndo;
		}

		private void MenuEditFind_Click(object sender, EventArgs e)
		{
			if (this.m_dlgFind == null)
			{
				m_dlgFind = new FindDialog(m_editorPanel.TextBox);
				m_dlgFind.ParentForm = this;
				m_dlgFind.Parent = this;
				int x = (inputPanel1.VisibleDesktop.Width - m_dlgFind.Width) / 2;
				int y = (inputPanel1.VisibleDesktop.Height - m_dlgFind.Height) / 2;
				m_dlgFind.Location = new Point(x, y);
			}

			if (m_dlgReplace != null && m_dlgReplace.Visible)
			{
				m_dlgReplace.Hide();
			}
			m_dlgFind.Show();
			m_dlgFind.BringToFront();
			m_dlgFind.SearchText = m_editorPanel.TextBox.SelectedText;
		}

		private void MenuEditReplace_Click(object sender, EventArgs e)
		{
			if (this.m_dlgReplace == null)
			{
				m_dlgReplace = new ReplaceDialog(m_editorPanel.TextBox);
				m_dlgReplace.ParentForm = this;
				m_dlgReplace.Parent = this;
				int x = (inputPanel1.VisibleDesktop.Width - m_dlgReplace.Width) / 2;
				int y = (inputPanel1.VisibleDesktop.Height - m_dlgReplace.Height) / 2;
				m_dlgReplace.Location = new Point(x, y);
			}

			if (m_dlgFind != null && m_dlgFind.Visible)
			{
				m_dlgFind.Hide();
			}

			m_dlgReplace.Show();
			m_dlgReplace.BringToFront();
			m_dlgReplace.SearchText = m_editorPanel.TextBox.SelectedText;
		}
		#endregion

		#region Tools menu handlers
		internal void MenuToolsOptions_Click(object sender, EventArgs e)
		{
			if (m_optionsDialogHidden)
			{
				m_editorPanel.Hide();

				if (m_optionsPanel == null)
				{
					Debug.WriteLine("Loading OptionsPanel");
					m_optionsPanel = new OptionsPanel(this);
					Debug.WriteLine("OptionsDialog created");

					//m_optionsPanel.Bounds = new Rectangle(0, 0, DpiHelper.Scale(240), DpiHelper.Scale(270));
					
					m_optionsPanel.Parent = this;
				}
				else
				{
					m_optionsPanel.Reset();
				}
				m_optionsPanel.Bounds = new Rectangle(0, 0, this.Width, this.Height);
				m_optionsPanel.Show();

				m_optionsDialogHidden = false;
				foreach (MenuItem m in m_mainMenu.MenuItems)
				{
					m.Enabled = false;
				}
			}
			else
			{
				if (m_optionsPanel.Result == DialogResult.OK)
				{
					SetupButtons();
					SetupOptions();
				}

				m_optionsPanel.Hide();
				m_editorPanel.Show();
				m_optionsDialogHidden = true;
				foreach (MenuItem m in m_mainMenu.MenuItems)
				{
					m.Enabled = true;
				}

				inputPanel1_EnabledChanged(sender, e);
				m_editorPanel.TextBox.Focus();
			}
		}

		private void m_menuToolsIndent_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.IndentSelection();
		}

		private void m_menuToolsUnindent_Click(object sender, EventArgs e)
		{
			m_editorPanel.TextBox.UnindentSelection();
		}

		private void m_menuToolsRefreshTemplates_Click(object sender, EventArgs e)
		{
			RefreshTemplates();
		}

		private void m_menuToolsRefreshTags_Click(object sender, EventArgs e)
		{
			m_htTags = PocketHTMLShared.LoadTagsXTR();

			m_editorPanel.SetupTagMenu();
			this.ContextMenu = m_editorPanel.TextBox.ContextMenu;
		}

		private void m_menuToolsLaunchTagEditor_Click(object sender, EventArgs e)
		{
			string editorPath = Utility.GetCurrentDir(true) + "TagEditor.exe";
			CoreDLL.CreateProcess(editorPath, string.Empty, IntPtr.Zero,
				IntPtr.Zero, 0, 0, IntPtr.Zero, IntPtr.Zero, null, null);
		}
		#endregion

		#region Help menu handlers
		private void MenuHelpContents_Click(object sender, EventArgs e)
		{
			CoreDLL.CreateProcess("peghelp.exe", @"\Windows\PHN.html#contents", IntPtr.Zero,
				IntPtr.Zero, 0, 0, IntPtr.Zero, IntPtr.Zero, null, null);
		}

		private void MenuHelpAbout_Click(object sender, EventArgs e)
		{
			m_editorPanel.ShowAbout();
		}
		#endregion

		#region New/open/save functions
		private void OpenFile()
		{
			string filename = GetFileName(false);

			if (filename != String.Empty)
			{
				if (CloseFile() != DialogResult.Cancel)
				{
					LoadFile(filename);
				}
			}
		}

		private string GetFileName(bool save)
		{
			string filename = String.Empty;			

			StringBuilder sb = new StringBuilder();
			
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
				sb.Append("HTML files (*.html, *.htm)|*.html;*.htm|");
				sb.Append("ASP files (*.asp, *.aspx)|*.asp;*.aspx|");
				sb.Append("PHP files (*.php)|*.php|");
				sb.Append("XML files (*.xml)|*.xml|");
				sb.Append("Text files (*.txt)|*.txt|");
				sb.Append("All files (*.*)|*.*");
				string fileFilter = sb.ToString();
			
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

				fd.Filter = fileFilter;
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
				sb.Append("HTML files (*.html, *.htm)\0*.html;*.htm\0");
				sb.Append("ASP files (*.asp, *.aspx)\0*.asp;*.aspx\0");
				sb.Append("PHP files (*.php)\0*.php\0");
				sb.Append("XML files (*.xml)\0*.xml\0");
				sb.Append("Text files (*.txt)\0*.txt\0");
				sb.Append("All files (*.*)\0*.*\0\0");
				string fileFilter = sb.ToString();

				filename = TGetFile.TGetFileName(save, originalFileName, filterIndex, fileFilter, m_saveFileDirectory);

			}

			return filename;
		}

		private void LoadFile(string filename)
		{
			FileStream fs = new FileStream(filename, FileMode.Open);
			string encodingName = Config.GetValue("Options", "DefaultEncoding");
			Encoding defaultEncoding = new UTF8Encoding(false);//Encoding.UTF8;

			try
			{
				if (encodingName == "utf-8")
				{
					defaultEncoding = new UTF8Encoding(false);
				}
				else
				{
					defaultEncoding = Encoding.GetEncoding(encodingName);
				}
			}
			catch(PlatformNotSupportedException)
			{}
			String contents = PocketHTMLShared.DecodeData(fs, defaultEncoding);
			m_editorPanel.TextBox.Text = contents;

			this.m_saveFileName = filename;
			m_editorPanel.HtmlControl.CurrentFilename = filename;
			m_saveFileDirectory = Path.GetDirectoryName(filename);
			m_editorPanel.TextBox.Modified = false;

			InteropUtility.SetTabStop(m_editorPanel.TextBox);

			m_mruManager.Add(filename);
		}

		private bool SaveFile(bool saveas)
		{
			string filename = String.Empty;
			Debug.WriteLine("Saving file.  saveas: " + saveas);
			Debug.WriteLine("Current file name: " + m_saveFileName);

			if ((m_saveFileName == String.Empty) ||
				(saveas))
			{
				filename = GetFileName(true);


			}
			else
			{
				filename = m_saveFileName;
			}

			if (filename == String.Empty)
			{
				return false;
			}

			m_saveFileName = filename;
			m_editorPanel.HtmlControl.CurrentFilename = filename;

			m_saveFileDirectory = Path.GetDirectoryName(filename);

			string encodingName = Config.GetValue("Options", "DefaultEncoding");

			Encoding defaultEncoding = new UTF8Encoding(false);

			try
			{
				if (encodingName == "utf-8")
				{
					defaultEncoding = new UTF8Encoding(false);
				}
				else
				{
					defaultEncoding = Encoding.GetEncoding(encodingName);
				}

			}
			catch (PlatformNotSupportedException)
			{ }

			StreamWriter sw = new StreamWriter(filename, false, defaultEncoding);
			sw.WriteLine(m_editorPanel.TextBox.Text);
			sw.Close();
			m_editorPanel.TextBox.Modified = false;
			m_mruManager.Add(filename);

			return true;
		}

		private DialogResult CloseFile()
		{
			DialogResult dr = DialogResult.No;
			if (m_editorPanel.TextBox.Modified)
			{
				dr = MessageBox.Show("You have unsaved changes.  Do you want to save before you quit?",
					"Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1);
				if (dr == DialogResult.Yes)
				{
					if (!SaveFile(false))
					{
						dr = DialogResult.Cancel;
					}
				}
			}
			else
			{
				dr = DialogResult.Yes;
			}
			return dr;
		}

		private void NewFile(string template)
		{
			DialogResult dr = CloseFile();
			if (dr != DialogResult.Cancel)
			{
				string templateText = String.Empty;
				switch (template)
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
						if (File.Exists(filename))
						{
							string originalDirectory = m_saveFileDirectory;
							LoadFile(filename);

							// LoadFile adds the name of the template to the MRU list, but
							// we don't want templates listed there
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
		#endregion

		#region Event handlers
		/// <summary>
		/// Called whenever the Input Panel is enabled or disabled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void inputPanel1_EnabledChanged(object sender, EventArgs e)
		{
			Rectangle visible;
			visible = inputPanel1.VisibleDesktop;

			if (m_optionsDialogHidden)
			{
				m_editorPanel.ResizePanel(inputPanel1);
			}
		}

		private void PocketHTMLEditor_Closing(object sender, CancelEventArgs e)
		{
			Debug.WriteLine("Program close requested");
			DialogResult dr = CloseFile();
			if(dr == DialogResult.Cancel)
			{
				e.Cancel = true;
				Debug.WriteLine("Close canceled");
			}
			else
			{
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
				Debug.WriteLine("Finishing close");
			}
			
		} 

		private void PocketHTMLEditor_GotFocus(object sender, EventArgs e)
		{
			inputPanel1_EnabledChanged(sender, e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if(m_editorPanel != null)
			{
				m_editorPanel.UpdateLayout();
			}			
		}

		private void PocketHTMLEditor_Activated(object sender, EventArgs e)
		{
			Debug.WriteLine("Main form activated");
			UpdateHardwareButton(true);
		}

		private void PocketHTMLEditor_Deactivate(object sender, EventArgs e)
		{
			Debug.WriteLine("Main form deactivated");
			UpdateHardwareButton(false);
		}

		private void PocketHTMLEditor_Load(object sender, EventArgs e)
		{
			// Retrieve the embedded toolbar graphics.  Needed because
			// the designer-generated ImageList doesn't support transparency.

			Debug.WriteLine("Loading icons");
			int iconSize = DpiHelper.Scale(16);

			Icon iconIE = ISquared.Utility.GetIcon("Graphics.ie", iconSize);
			Icon iconTag = ISquared.Utility.GetIcon("Graphics.Tag", iconSize);

			Debug.WriteLine("Icons loaded, adding to imagelist");

			StringBuilder sb = new StringBuilder();
			sb.Append("iconIE: ");
			sb.Append(iconIE);
			Debug.WriteLine(sb.ToString());

			sb = new StringBuilder();
			sb.Append("iconTag: ");
			sb.Append(iconTag);
			Debug.WriteLine(sb.ToString());

			Debug.WriteLine("Icons added, setting indices");

			Size size1 = new Size(iconSize, iconSize);
			imageList1.ImageSize = size1;

			imageList1.Images.Add(iconTag);
			imageList1.Images.Add(iconIE);
			Debug.WriteLine("Set image size");
			m_btnTags.ImageIndex = 0;
			m_btnPreview.ImageIndex = 1;
			Debug.WriteLine("Set indices");

			Debug.WriteLine("Creating HTML control");
			m_editorPanel.HtmlControl.CreateHTMLControl();

			Debug.WriteLine("Loading complete");
		}

		private void PocketHTMLEditor_Closed(object sender, EventArgs e)
		{
			Debug.WriteLine("PocketHTML closed");
		}

		// TODO Remove 240x320 specific code
		private void toolBarButton1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.m_btnTags)
			{
				this.ContextMenu.Show(this, new Point(80, 160)); ;
			}

			else if (e.Button == this.m_btnPreview)
			{
				m_editorPanel.SwapPanels(e.Button.Pushed);
			}
		}
		#endregion

		#region IMRUClient Members

		void IMRUClient.OpenMRUFile(string fileName)
		{
			if(CloseFile() != DialogResult.Cancel)
			{
				LoadFile(fileName);
			}
			
		}

		#endregion

		#region Other functions
		private void RefreshTemplates()
		{
			Debug.WriteLine("Checking templates");
			m_menuFileNew.MenuItems.Clear();
			m_menuFileNew.MenuItems.Add(m_menuFileNewBlank);
			m_menuFileNew.MenuItems.Add(m_menuFileNewBasic);

			string templatePath = Utility.GetCurrentDir(true) + "templates";

			if (Directory.Exists(templatePath))
			{
				Debug.WriteLine("Templates exist");
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
			else
			{
				Debug.WriteLine("Templates directory not found");
			}
			Debug.WriteLine("Templates check complete");
		}
		#endregion







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