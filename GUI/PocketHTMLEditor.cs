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
using System.Windows.Forms;
using Ayende;
using ISquared.Win32Interop;
using ISquared.Win32Interop.WinEnums;
using Microsoft.WindowsCE.Forms;
using System.Diagnostics;
//using Oxbow.Tools;
using ISquared.Debugging;



namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	/// 
	
	public class PocketHTMLEditor : Form
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		static void Main() 
		{
			try
			{
				Application.Run(new PocketHTMLEditor());
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
		private MainMenu mainMenu1;

		/// The saved settings for PocketHTML
		private Configuration config;
		// just used to keep from writing "\r\n" all the time
		private string newline;
		// Name of the current file
		private string saveFileName;
		// TODO actually used?
		private ArrayList textList;
		// used by GetLeadingSpaces() to capture the number of beginning spaces
		Regex reLeadingSpaces;
		// TODO actually used?
		Regex reNewLine;
		private OptionsPanel optionsDialog;
		private EditorPanel ed;
		private Match findmatch;
		// TODO Not entirely sure if this is actually used
		private int findindex;
		internal FindDialog find;
		private ReplaceDialog replace;
		private MenuItem MenuFileOpen;
		private MenuItem MenuEditUndo;
		private MenuItem MenuEditCopy;
		private MenuItem MenuEditCut;
		private MenuItem MenuEditPaste;
		private MenuItem MenuEditClear;
		private MenuItem MenuEditSelectAll;
		private MenuItem MenuFileSave;
		// Keeps the auto-indent function from recursing
		private bool firstEnter;
		// Holds all the Tag objects, keyed to the short name or the normal name
		private Hashtable tagHash;
		// Maps a button name (ie, "button1") to a tag name
		private Hashtable buttonTags;
		private bool firstOptions;
		private bool tgetfileExists;
		//private PHNOptions options;
		private bool optionsDialogHidden;
		private MenuItem menuItem4;
		private MenuItem MenuFileExit;
		private MenuItem MenuFileSaveAs;
		private MenuItem MenuFile;
		private MenuItem MenuEdit;
		private MenuItem MenuEditSep2;
		private MenuItem MenuEditSep1;
		private MenuItem MenuTools;
		private MenuItem MenuToolsOptions;
		private ImageList imageList1;
		private MenuItem MenuToolsFind;
		private MenuItem MenuToolsReplace;
		private MenuItem menuItem2;
		private ToolBar toolBar1;
		private MenuItem MenuHelp;
		private MenuItem MenuHelpAbout;
		private MenuItem MenuHelpContents;
		private MenuItem MenuFileNew;
		private MenuItem MenuFileNewBlank;
		private System.Windows.Forms.ToolBarButton m_btnPreview;
		private System.Windows.Forms.ToolBarButton m_btnTags;
		private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
		private System.Windows.Forms.MenuItem MenuFileClose;
		private MenuItem MenuFileNewBasic;
		

		#endregion


		#region properties
		internal TextBox TextBox
		{
			get
			{
				return ed.TextBox;
			}			
		}

		internal Hashtable TagHash
		{
			get
			{
				return this.tagHash;
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
				return ed.Buttons;
			}
		}

		internal Configuration Config
		{
			get
			{
				return config;
			}
		}
		#endregion


		public PocketHTMLEditor()
		{			
			// TODO Convert to XML config file
			String inifile = Utility.GetCurrentDir(true) + "pockethtml.ini";

			// TODO Set up some nice defaults
			try
			{
				if(!File.Exists(inifile))
				{
					config = new Configuration();				
				}
				else
				{
					StreamReader sr = new StreamReader(inifile);
					config = new Configuration(sr);
					sr.Close();
				}
			}
			catch(DirectoryNotFoundException dnfe)
			{
				MessageBox.Show("Exception while loading pockethtml.ini\nException information: " + dnfe.Message);
				Application.Exit();
			}

			if(config.SectionExists("Debug"))
			{
				if(config.GetBool("Debug", "Text"))
				{
					TextTraceListener.Prefix = "PocketHTML";
					TextTraceListener.InstallListener();
					TextTraceListener.Reset();
				}

				if(config.GetBool("Debug", "TCP"))
				{
					TcpTraceListener.InstallListener("PPP_PEER", 6666);
				}
			}

			Debug.WriteLine("Configuration created");

			Debug.WriteLine("Options section exists: " + config.SectionExists("Options"));
			Debug.WriteLine("TextWrap exists: " + config.ValueExists("Options", "PageWrap"));

			
			
			Debug.WriteLine("Beginning PHE constructor");
			// Generated form setup
			InitializeComponent();
			Debug.WriteLine("InitializeComponent complete");

			string templatePath = Utility.GetCurrentDir(true) + "templates";
			string[] templates = Directory.GetFiles(templatePath, "*.htm*");

			Array.Sort(templates);

			foreach(string template in templates)
			{
				MenuItem item = new MenuItem();
				int index = template.LastIndexOf("\\");
				string filename = template.Substring(index + 1);
				item.Text = filename;
				item.Click += new EventHandler(MenuFileNewOther_Click);
				MenuFileNew.MenuItems.Add(item);
			}

			// Retrieve the embedded toolbar graphics.  Needed because
			// the designer-generated ImageList doesn't support transparency.
			Icon iconIE = Utility.GetIcon("Graphics.ie");
			Icon iconTag = Utility.GetIcon("Graphics.Tag");

			imageList1.Images.Add(iconTag);
			imageList1.Images.Add(iconIE);
			
			m_btnTags.ImageIndex = 0;
			m_btnPreview.ImageIndex = 1;
			

			ed = new EditorPanel();
			Debug.WriteLine("EditorPanel created");
			ed.Parent = this;

			// Hooks up each button with the handler
			// TODO Check if this optimization actually matters
			EventHandler tagClick = new EventHandler(TagButton_Click);
			for(int i = 0; i < ed.Buttons.Length; i++)
			{
				ed.Buttons[i].Click += tagClick;
			}
			
			// TODO Delay-load this panel
			optionsDialog = new OptionsPanel();
			Debug.WriteLine("OptionsDialog created");
			optionsDialog.Bounds = new Rectangle(0,0, 240, 270);
			optionsDialog.Parent = this;
			optionsDialog.Hide();
			optionsDialog.SendToBack();
			ed.BringToFront();

			
			
			optionsDialogHidden = true;
			
			
			saveFileName = String.Empty;

			tagHash = new Hashtable();
			buttonTags = new Hashtable();
			
			

			

			
			LoadTags();
			Debug.WriteLine("Tags loaded");
			SetupButtons();
			Debug.WriteLine("Buttons created");
			SetupOptions();
			Debug.WriteLine("Options setup complete");

			newline = "\r\n";
			textList = new ArrayList();

			reLeadingSpaces = new Regex(
				@"^\s+",
				RegexOptions.IgnoreCase | 
				RegexOptions.Multiline |
				RegexOptions.IgnorePatternWhitespace);
			reNewLine = new Regex(
				@"\r\n",
				RegexOptions.IgnoreCase
				| RegexOptions.Multiline
				| RegexOptions.IgnorePatternWhitespace);


			// hook up auto-indent
			ed.TextBox.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);


			firstEnter = true;

			// HACK firstOptions is ugly.  Get rid of it.
			firstOptions = true;

			// Note the constant.  Change this?
			tgetfileExists = File.Exists("\\Windows\\tgetfile.dll");

			findindex = 0;
			ed.TagMenuClicked += new EditorPanel.TagMenuClickedHandler(this.InsertTag);
			
			this.ContextMenu = ed.TextBox.ContextMenu;
			
			
			Debug.WriteLine("PHE constructor complete");
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.MenuFile = new System.Windows.Forms.MenuItem();
			this.MenuFileNew = new System.Windows.Forms.MenuItem();
			this.MenuFileNewBlank = new System.Windows.Forms.MenuItem();
			this.MenuFileNewBasic = new System.Windows.Forms.MenuItem();
			this.MenuFileOpen = new System.Windows.Forms.MenuItem();
			this.MenuFileSave = new System.Windows.Forms.MenuItem();
			this.MenuFileSaveAs = new System.Windows.Forms.MenuItem();
			this.MenuFileClose = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.MenuFileExit = new System.Windows.Forms.MenuItem();
			this.MenuEdit = new System.Windows.Forms.MenuItem();
			this.MenuEditUndo = new System.Windows.Forms.MenuItem();
			this.MenuEditSep2 = new System.Windows.Forms.MenuItem();
			this.MenuEditCopy = new System.Windows.Forms.MenuItem();
			this.MenuEditCut = new System.Windows.Forms.MenuItem();
			this.MenuEditPaste = new System.Windows.Forms.MenuItem();
			this.MenuEditSep1 = new System.Windows.Forms.MenuItem();
			this.MenuEditClear = new System.Windows.Forms.MenuItem();
			this.MenuEditSelectAll = new System.Windows.Forms.MenuItem();
			this.MenuTools = new System.Windows.Forms.MenuItem();
			this.MenuToolsOptions = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.MenuToolsReplace = new System.Windows.Forms.MenuItem();
			this.MenuToolsFind = new System.Windows.Forms.MenuItem();
			this.MenuHelp = new System.Windows.Forms.MenuItem();
			this.MenuHelpAbout = new System.Windows.Forms.MenuItem();
			this.MenuHelpContents = new System.Windows.Forms.MenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList();
			this.il = new System.Windows.Forms.ImageList();
			this.m_btnTags = new System.Windows.Forms.ToolBarButton();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.m_btnPreview = new System.Windows.Forms.ToolBarButton();
			this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.Add(this.MenuFile);
			this.mainMenu1.MenuItems.Add(this.MenuEdit);
			this.mainMenu1.MenuItems.Add(this.MenuTools);
			this.mainMenu1.MenuItems.Add(this.MenuHelp);
			// 
			// MenuFile
			// 
			this.MenuFile.MenuItems.Add(this.MenuFileNew);
			this.MenuFile.MenuItems.Add(this.MenuFileOpen);
			this.MenuFile.MenuItems.Add(this.MenuFileSave);
			this.MenuFile.MenuItems.Add(this.MenuFileSaveAs);
			this.MenuFile.MenuItems.Add(this.MenuFileClose);
			this.MenuFile.MenuItems.Add(this.menuItem4);
			this.MenuFile.MenuItems.Add(this.MenuFileExit);
			this.MenuFile.Text = "File";
			// 
			// MenuFileNew
			// 
			this.MenuFileNew.MenuItems.Add(this.MenuFileNewBlank);
			this.MenuFileNew.MenuItems.Add(this.MenuFileNewBasic);
			this.MenuFileNew.Text = "New";
			// 
			// MenuFileNewBlank
			// 
			this.MenuFileNewBlank.Text = "Blank";
			this.MenuFileNewBlank.Click += new System.EventHandler(this.MenuFileNewBlank_Click);
			// 
			// MenuFileNewBasic
			// 
			this.MenuFileNewBasic.Text = "Basic";
			this.MenuFileNewBasic.Click += new System.EventHandler(this.MenuFileNewBasic_Click);
			// 
			// MenuFileOpen
			// 
			this.MenuFileOpen.Text = "Open";
			this.MenuFileOpen.Click += new System.EventHandler(this.MenuFileOpen_Click);
			// 
			// MenuFileSave
			// 
			this.MenuFileSave.Text = "Save";
			this.MenuFileSave.Click += new System.EventHandler(this.MenuFileSave_Click);
			// 
			// MenuFileSaveAs
			// 
			this.MenuFileSaveAs.Text = "Save As";
			this.MenuFileSaveAs.Click += new System.EventHandler(this.MenuFileSaveAs_Click);
			// 
			// MenuFileClose
			// 
			this.MenuFileClose.Text = "Close";
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "-";
			// 
			// MenuFileExit
			// 
			this.MenuFileExit.Text = "Exit";
			this.MenuFileExit.Click += new System.EventHandler(this.MenuFileExit_Click);
			// 
			// MenuEdit
			// 
			this.MenuEdit.MenuItems.Add(this.MenuEditUndo);
			this.MenuEdit.MenuItems.Add(this.MenuEditSep2);
			this.MenuEdit.MenuItems.Add(this.MenuEditCopy);
			this.MenuEdit.MenuItems.Add(this.MenuEditCut);
			this.MenuEdit.MenuItems.Add(this.MenuEditPaste);
			this.MenuEdit.MenuItems.Add(this.MenuEditSep1);
			this.MenuEdit.MenuItems.Add(this.MenuEditClear);
			this.MenuEdit.MenuItems.Add(this.MenuEditSelectAll);
			this.MenuEdit.Text = "Edit";
			// 
			// MenuEditUndo
			// 
			this.MenuEditUndo.Text = "Undo";
			this.MenuEditUndo.Click += new System.EventHandler(this.MenuUndo_Click);
			// 
			// MenuEditSep2
			// 
			this.MenuEditSep2.Text = "-";
			// 
			// MenuEditCopy
			// 
			this.MenuEditCopy.Text = "Copy";
			this.MenuEditCopy.Click += new System.EventHandler(this.MenuCopy_Click);
			// 
			// MenuEditCut
			// 
			this.MenuEditCut.Text = "Cut";
			this.MenuEditCut.Click += new System.EventHandler(this.MenuCut_Click);
			// 
			// MenuEditPaste
			// 
			this.MenuEditPaste.Text = "Paste";
			this.MenuEditPaste.Click += new System.EventHandler(this.MenuPaste_Click);
			// 
			// MenuEditSep1
			// 
			this.MenuEditSep1.Text = "-";
			// 
			// MenuEditClear
			// 
			this.MenuEditClear.Text = "Clear";
			this.MenuEditClear.Click += new System.EventHandler(this.MenuClear_Click);
			// 
			// MenuEditSelectAll
			// 
			this.MenuEditSelectAll.Text = "Select All";
			this.MenuEditSelectAll.Click += new System.EventHandler(this.MenuSelectAll_Click);
			// 
			// MenuTools
			// 
			this.MenuTools.MenuItems.Add(this.MenuToolsOptions);
			this.MenuTools.MenuItems.Add(this.menuItem2);
			this.MenuTools.MenuItems.Add(this.MenuToolsReplace);
			this.MenuTools.MenuItems.Add(this.MenuToolsFind);
			this.MenuTools.Text = "Tools";
			// 
			// MenuToolsOptions
			// 
			this.MenuToolsOptions.Text = "Options";
			this.MenuToolsOptions.Click += new System.EventHandler(this.MenuToolsOptions_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "-";
			// 
			// MenuToolsReplace
			// 
			this.MenuToolsReplace.Text = "Replace";
			this.MenuToolsReplace.Click += new System.EventHandler(this.MenuToolsReplace_Click);
			// 
			// MenuToolsFind
			// 
			this.MenuToolsFind.Text = "Find";
			this.MenuToolsFind.Click += new System.EventHandler(this.MenuToolsFind_Click);
			// 
			// MenuHelp
			// 
			this.MenuHelp.MenuItems.Add(this.MenuHelpAbout);
			this.MenuHelp.MenuItems.Add(this.MenuHelpContents);
			this.MenuHelp.Text = "Help";
			// 
			// MenuHelpAbout
			// 
			this.MenuHelpAbout.Text = "About";
			this.MenuHelpAbout.Click += new System.EventHandler(this.MenuHelpAbout_Click);
			// 
			// MenuHelpContents
			// 
			this.MenuHelpContents.Text = "Contents";
			this.MenuHelpContents.Click += new System.EventHandler(this.MenuHelpContents_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			// 
			// il
			// 
			this.il.ImageSize = new System.Drawing.Size(16, 16);
			// 
			// m_btnTags
			// 
			this.m_btnTags.ImageIndex = 0;
			// 
			// toolBar1
			// 
			this.toolBar1.BackColor = System.Drawing.Color.White;
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
			this.Controls.Add(this.toolBar1);
			this.Menu = this.mainMenu1;
			this.Text = "PocketHTML.Net";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PocketHTMLEditor_Closing);
			this.GotFocus += new System.EventHandler(this.inputPanel1_EnabledChanged);

		}
		#endregion

		
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
				ed.ResizePanel(inputPanel1);
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
			Tag tag = (Tag)tagHash[tagName];
			InsertTag(tag);
		}

		// TODO Not currently used
		internal string GetButtonString(int buttonnum)
		{
			String name = ed.Buttons[buttonnum].Name;
			return (string)buttonTags[name];
		}

		// TODO Can this be moved to the Utility class?
		/// <summary>
		/// Utility function to determine the leading spaces on a line
		/// </summary>
		/// <returns></returns>
		private String GetLeadingSpaces()
		{
			IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
			int currentLine = CoreDLL.SendMessage(pTB, (int)EM.LINEFROMCHAR, -1, 0); 

			StringBuilder lineText;
			int charIndex = CoreDLL.SendMessage(pTB, (int)EM.LINEINDEX, currentLine, 0); 
			int lineLength = CoreDLL.SendMessage(pTB, (int)EM.LINELENGTH, 
				charIndex, 0);


			lineText = new StringBuilder(" ", lineLength + 1);
			lineText[0] = (char)(lineLength + 1);

			int numChars = CoreDLL.SendMessageGetLine(pTB, (int)EM.GETLINE,
				currentLine, lineText);

			string line = lineText.ToString();
			String spaces;
			
			Match lws = reLeadingSpaces.Match(line);

			if(lws.Success)
			{
				spaces = lws.Captures[0].Value;
			}
			else
			{
				spaces = String.Empty;
			}
			return spaces;
		}

		/// <summary>
		/// Overload that takes the name or short name of the tag to insert.
		/// </summary>
		/// <param name="onlyTag"></param>
		/// <returns></returns>
		private bool InsertTag(string onlyTag)
		{
			
			if(!tagHash.ContainsKey(onlyTag))
			{
				return false;
			}
			Tag t = (Tag)tagHash[onlyTag];
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
			IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
			StringBuilder sb = new StringBuilder();
			String spaces = GetLeadingSpaces();
			// basically, an additional level of indentation
			// TODO make this an option?
			String spacesPlus = spaces + "    ";

			int currentLineNum = CoreDLL.SendMessage(pTB, (int)EM.LINEFROMCHAR, -1, 0); 

			int selstart = ed.TextBox.SelectionStart;

			int newLineNum = currentLineNum;

			string seltext = ed.TextBox.SelectedText;

			bool indentHTML = config.GetBool("Options", "IndentHTML");

			// We'll need a start tag AND an end tag
			if(!tag.IndividualTag)
			{
				sb.Append(tag.StartTag);
				if(tag.MultiLineTag)
				{
					sb.Append(newline);
				}
				// 
				if(tag.InnerTags)
				{
					Tag innerTag = (Tag)tagHash[tag.DefaultInnerTag];
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
					if(!tag.IndividualTag)
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

			CoreDLL.SendMessageString(pTB, (int)EM.REPLACESEL, 1, sb.ToString());			

			int charIndex = 0; 
			int cursorLocationIndex = 0;
				
			if(tag.DefaultAttributes.Length > 0)
			{
				charIndex = CoreDLL.SendMessage(pTB, (int)EM.LINEINDEX, currentLineNum, 0);
				
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
				
				int idx = tag.Name.IndexOf("\"");
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
			else if(tag.NormalTag)
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

					charIndex = CoreDLL.SendMessage(pTB, (int)EM.LINEINDEX, newLineNum, 0);
				}

				else
				{					
					if(!tag.IndividualTag)
					{		
						charIndex = ed.TextBox.SelectionStart;
						cursorLocationIndex -= tag.EndTag.Length;//tag.StartTag.Length;

						if(tag.InnerTags)
						{
							Tag innerTag = (Tag)tagHash[tag.DefaultInnerTag];
							cursorLocationIndex -= innerTag.EndTag.Length;//StartTag.Length;
						}
					}
				}				
				
				charIndex += cursorLocationIndex;							
			}
			
			if(tag.NormalTag)
			{
				int sel = CoreDLL.SendMessage(pTB, (int)EM.SETSEL, charIndex, charIndex);
			}

			ed.TextBox.Modified = true;

			return true;			
		}

		void textBox1_KeyPress(Object o, KeyPressEventArgs e)
		{
			// The keypressed method uses the KeyChar property to check 
			// whether the ENTER key is pressed. 

			// If the ENTER key is pressed, the Handled property is set to true, 
			// to indicate the event is handled.
			if(e.KeyChar == (char)13)
			{
				if(firstEnter)
				{
					firstEnter = false;
					
					if(config.GetBool("Options", "AutoIndent"))
					{
						e.Handled=true;
						AutoIndent();
					}
					
					firstEnter = true;
				}
			}			
		}

		private void AutoIndent()
		{
			string spaces = GetLeadingSpaces();
			IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
			CoreDLL.SendMessageString(pTB, (int)EM.REPLACESEL, 0, "\r\n" + spaces);
		}

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

		internal void SetupButtons()
		{
			for(int i = 0; i < ed.Buttons.Length; i++)
			{
				NamedButton nb = ed.Buttons[i];
				
				if(config.ValueExists("Button Tags", nb.Name))
				{				
					string tagName = config.GetValue("Button Tags", nb.Name);

					Tag t = (Tag)tagHash[tagName];					
					
					if(t.ShortName != String.Empty)
					{
						nb.Text = t.ShortName;
					}
					else
					{
						nb.Text = t.Name;
					}
					
					buttonTags[nb.Name] = nb.Text;
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
				ed.TextBox.Select(findmatch.Index, findmatch.Length);
				findindex = findmatch.Index;

				if(scroll)
				{
					IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
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
				bool tw = config.GetBool("Options", "TextWrap");
				Debug.WriteLine("TextWrap: " + tw);
				Debug.WriteLine("ed: " + ed.ToString());
				Debug.WriteLine("ed.TextBox: " + ed.TextBox.ToString());
				//ed.TextBox.WordWrap = tw;
				Debug.WriteLine("Set TextWrap");
				//ed.HtmlControl.WordWrap = config.GetBool("Options", "PageWrap");

				//ed.HtmlControl.ShrinkMode = config.GetBool("Options", "PageWrap");
				Debug.WriteLine("Set PageWrap");

				if(ed.TextBox.WordWrap)
				{
					ed.TextBox.ScrollBars = ScrollBars.Vertical;
				}
				else
				{
					ed.TextBox.ScrollBars = ScrollBars.Both;
				}
				Debug.WriteLine("Set scrollbars");
			}
			catch(Exception e)
			{
				Debug.WriteLine("Caught exception: " + e.ToString());
			}
			
		}

		// TODO Either move these to a new TextBox class, or use OpenNetCF's TextBoxEx
		private void Copy()
		{
			IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
			CoreDLL.SendMessage(pTB, (int)WM.COPY, 0, 0);
		}

		private void Undo()
		{
			IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
			CoreDLL.SendMessage(pTB, (int)WM.UNDO, 0, 0);
		}

		private void Paste()
		{
			IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
			CoreDLL.SendMessage(pTB, (int)WM.PASTE, 0, 0);
			ed.TextBox.Modified = true;
		}

		private void Clear()
		{
			IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
			CoreDLL.SendMessage(pTB, (int)EM.SETSEL, 0, -1);
			CoreDLL.SendMessage(pTB, (int)WM.CLEAR, 0, 0);
		}

		private void Cut()
		{
			IntPtr pTB = CoreDLL.GetHandle(ed.TextBox);
			CoreDLL.SendMessage(pTB, (int)WM.CUT, 0, 0);
		}

		private void MenuUndo_Click(object sender, EventArgs e)
		{
			Undo();
		}

		private void MenuCopy_Click(object sender, EventArgs e)
		{
			Copy();
		}

		private void MenuCut_Click(object sender, EventArgs e)
		{
			Cut();
		}

		private void MenuPaste_Click(object sender, EventArgs e)
		{
			Paste();
		}

		private void MenuClear_Click(object sender, EventArgs e)
		{
			Clear();
		}

		private void MenuSelectAll_Click(object sender, EventArgs e)
		{
		
			IntPtr pTB1 = CoreDLL.GetHandle(ed.TextBox);
			CoreDLL.SendMessage(pTB1, (int)EM.SETSEL, 0, -1);
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

			if(!tgetfileExists)
			{
			
				FileDialog fd;

				if(save)
				{
					fd = new SaveFileDialog();
				}
				else
				{
					fd = new OpenFileDialog();
				}
				//OpenFileDialog ofd = new OpenFileDialog();
				fd.Filter = "HTML files (*.html)|*.html;|All files (*.*)|*.*";
				DialogResult dr = fd.ShowDialog();

				if(dr == DialogResult.OK)
				{
					filename = fd.FileName;
				}
			}
			else
			{
				filename = TGetFile.TGetFileName(save);

			}

			return filename;
		}

		private void LoadFile(string filename)
		{
			StreamReader sr = new StreamReader(filename);
			String s =  sr.ReadToEnd();		
			sr.Close();
			CoreDLL.SendMessageString(CoreDLL.GetHandle(ed.TextBox), 
				(int)WM.SETTEXT, 0, s);
			this.saveFileName = filename;
			ed.TextBox.Modified = false;
		}

		private void MenuFileSave_Click(object sender, EventArgs e)
		{
			SaveFile(false);
		}

		private void SaveFile(bool saveas)
		{
			string filename = String.Empty;
			Debug.WriteLine("Saving file.  saveas: " + saveas);
			Debug.WriteLine("Current file name: " + saveFileName);

			if( (saveFileName == String.Empty) ||
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

				saveFileName = filename;
			}
			else
			{
				filename = saveFileName;
			}			
				
			StreamWriter sw = new StreamWriter(filename);
			sw.WriteLine(ed.TextBox.Text);			
			sw.Close();
			ed.TextBox.Modified = false;			
		}

		private DialogResult CloseFile()
		{
			DialogResult dr = DialogResult.No;
			if(ed.TextBox.Modified)
			{
				dr = MessageBox.Show("You have unsaved changes.  Do you want to save before you quit?",
					"Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1);
				if(dr == DialogResult.Yes)
				{
					SaveFile(false);	
				}
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
				ed.HtmlControl.Dispose();
				String inifile = Utility.GetCurrentDir(true) + "pockethtml.ini";
				StreamWriter sw = new StreamWriter(inifile);
				config.Save(sw);
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
				ed.Hide();
				// HACK Ugly hack.  Gotta be a better way to do it.
				if(firstOptions)
				{
					optionsDialog.Load(this);
					
				}
				else
				{
					optionsDialog.Reset();					
				}
				optionsDialog.Show();

				firstOptions = false;
				optionsDialogHidden = false;
				foreach(MenuItem m in mainMenu1.MenuItems)
				{
					m.Enabled = false;
				}
			}
			else
			{
				if(optionsDialog.OK)
				{
					SetupButtons();
					SetupOptions();
				}
				
				optionsDialog.Hide();
				ed.Show();
				optionsDialogHidden = true;
				foreach(MenuItem m in mainMenu1.MenuItems)
				{
					m.Enabled = true;
				}
				
				inputPanel1_EnabledChanged(sender, e);
				ed.TextBox.Focus();				
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
				ed.TextBox.Text = String.Empty;
				ed.TextBox.Modified = false;
				this.saveFileName = String.Empty;
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
				ed.SwapPanels(e.Button.Pushed);
			}		
		}

		// TODO Remove 240x320 specific code
		private void MenuToolsFind_Click(object sender, EventArgs e)
		{
			if(this.find == null)
			{				
				find = new FindDialog(this);
				find.FormBorderStyle = FormBorderStyle.None;
				int x = (240 - find.Width) / 2;
				int y = 100;
				find.Location = new Point(x, y);
				find.MinimizeBox = false;
				find.MaximizeBox = false;
				find.ControlBox = false;
			}

			find.Show();
			find.BringToFront();
			find.TextBox.Focus();			
		}

		// TODO Remove 240x320 specific code
		private void MenuToolsReplace_Click(object sender, EventArgs e)
		{
			if(this.replace == null)
			{				
				replace = new ReplaceDialog(this);
				replace.FormBorderStyle = FormBorderStyle.None;
				int x = (240 - replace.Width) / 2;
				int y = 100;
				replace.Location = new Point(x, y);
				replace.MinimizeBox = false;
				replace.MaximizeBox = false;
				replace.ControlBox = false;
			}
			

			replace.Show();
			replace.BringToFront();		
		}

		private void MenuHelpContents_Click(object sender, EventArgs e)
		{
			CoreDLL.CreateProcess("peghelp.exe", @"\Windows\PHN.html#contents", IntPtr.Zero,
				IntPtr.Zero, 0, 0, IntPtr.Zero, IntPtr.Zero, null, null);
		}

		private void MenuHelpAbout_Click(object sender, EventArgs e)
		{
			ed.ShowAbout();
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
						break;
					default:
						string filename = Utility.GetCurrentDir(true) + "templates\\" + template;
						if(File.Exists(filename))
						{
							LoadFile(filename);
							return;
						}				
						break;
				}
				ed.TextBox.Text = templateText;
				ed.TextBox.Modified = false;
				this.saveFileName = String.Empty;
				
			}
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