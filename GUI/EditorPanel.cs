using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
//using CKIUtil.Controls.HTMLViewer;
using OpenNETCF.Windows.Forms;
using ISquared.Win32Interop;
using ISquared.Win32Interop.WinEnums;



namespace ISquared.PocketHTML
{

	class EditorPanel : System.Windows.Forms.Panel
	{
		public delegate bool TagMenuClickedHandler (string tagname);
		public TagMenuClickedHandler TagMenuClicked;

		private System.Windows.Forms.Panel buttonPanel;
		private System.Windows.Forms.Panel viewerPanel;
		private AboutPanel aboutPanel;
		private System.Windows.Forms.Panel tbPanel;
		private System.Windows.Forms.TextBox tb;
		private IntPtr pTB;
		private System.Collections.Hashtable menuTags;
		private System.Windows.Forms.ContextMenu contextTop;
		private NamedButton[] buttons;
		private bool showAbout;

		private HTMLViewer htmlControl;

		private Menu currentMenu;

		/*
		private MenuItem MenuDocument;
		private MenuItem MenuForms;
		private MenuItem MenuLinks;
		private MenuItem MenuLists;
		private MenuItem MenuTables;
		private MenuItem MenuText;
		private MenuItem MenuOther;

		private MenuItem MenuDocumentDoctype;
		
		private TagMenuItem MenuTextFormat;
		private TagMenuItem MenuTextFont;
		private TagMenuItem MenuTextLayout;
		private TagMenuItem MenuTextChars;			

		private TagMenuItem MenuTextLayoutHeadings;
		private TagMenuItem MenuTextLayoutFrames;
		 */ 


		
		
		/// <summary>
		/// Property Tb (TextBox)
		/// </summary>
		public TextBox TextBox
		{
			get
			{
				return this.tb;
			}
			set
			{
				this.tb = value;
			}
		}

		
		/// <summary>
		/// Property HtmlControl (HTMLViewer)
		/// </summary>
		public HTMLViewer HtmlControl
		{
			get
			{
				return this.htmlControl;
			}
			set
			{
				this.htmlControl = value;
			}
		}		


		// TODO Is there a better way to handle this than exposing the buttons as a property?

		/// <summary>
		/// Property Buttons (NamedButton[])
		/// </summary>
		public NamedButton[] Buttons
		{
			get
			{
				return this.buttons;
			}
			set
			{
				this.buttons = value;
			}
		}

		public EditorPanel()
		{
			Initialize();

			this.viewerPanel.Hide();
			showAbout = true;
		}

		public void Initialize()
		{
			menuTags = new Hashtable();

			#region form setup
			this.Bounds = new Rectangle(0, 0, 240, 270);
			buttonPanel = new Panel();
			buttonPanel.Bounds = new Rectangle(0, 238, 240, 32);
			buttonPanel.Parent = this;

			viewerPanel = new Panel();
			viewerPanel.Parent = this;
			viewerPanel.Bounds = new Rectangle(0, 0, 240, 236);

			tbPanel = new Panel();
			tbPanel.Parent = this;
			tbPanel.Bounds = new Rectangle(0, 0, 240, 236);

			tb = new TextBox();
			this.tb.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular);
			this.tb.MaxLength = 0;
			this.tb.Multiline = true;
			this.tb.Bounds = new Rectangle(0, 0, 240, 205);
			this.tb.Text = "";
			tb.ScrollBars = ScrollBars.Vertical;
			tb.Parent = tbPanel;

			// TODO Can I delay-load this, too?
			htmlControl = new HTMLViewer();
			htmlControl.Bounds = viewerPanel.Bounds;
			htmlControl.Parent = viewerPanel;
			//htmlControl.CreateHTMLControl();

			int width = 30;
			int height = 16;
			int y = 0;
			int x = 0;

			this.Buttons = new NamedButton[16];

			for(int i = 0; i < 16; i++)
			{
				NamedButton nb = new NamedButton();
				nb.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
				
				x = width * (i % 8);
				if(i > 7)
				{
					y = 16;
				}
				else
				{
					y = 0;
				}

				nb.Bounds = new Rectangle(x, y, width, height);
				nb.Name = "button" + Convert.ToString(i + 1);
				nb.Parent = buttonPanel;
				this.buttons[i] = nb;
			}	
		
			LOGFONT lf = new LOGFONT();
			lf.lfFaceName = "Tahoma";
			lf.lfHeight = 13;
			lf.lfWeight = 400;
			IntPtr pLF = CoreDLL.LocalAllocCE(0x40, 256);
			Marshal.StructureToPtr(lf, pLF, false);
			IntPtr hFont = CoreDLL.CreateFontIndirect(pLF);

			pTB = CoreDLL.GetHandle(tb);
			CoreDLL.SendMessage(pTB, (int)WM.SETFONT, (int)hFont, 0);
			CoreDLL.SendMessage(pTB, (int)EM.LIMITTEXT, 0xFFFFFFF, 0);
			CoreDLL.LocalFreeCE(pLF);

			#endregion

			contextTop = new ContextMenu();
			tb.ContextMenu = contextTop;
			currentMenu = contextTop;

			MakeMenuXTR();

			
			#region menu items
			/*
			

			MenuDocument = new MenuItem();
			MenuDocument.Text = "Document";
			contextTop.MenuItems.Add(MenuDocument);

			MenuForms = new MenuItem();
			MenuForms.Text = "Forms";
			contextTop.MenuItems.Add(MenuForms);

			MenuLinks = new MenuItem();
			MenuLinks.Text = "Links";
			contextTop.MenuItems.Add(MenuLinks);

			MenuLists = new MenuItem();
			MenuLists.Text = "Lists";
			contextTop.MenuItems.Add(MenuLists);

			MenuTables = new MenuItem();
			MenuTables.Text = "Tables";
			contextTop.MenuItems.Add(MenuTables);

			MenuText = new MenuItem();
			MenuText.Text = "Text";
			contextTop.MenuItems.Add(MenuText);
			
			MenuOther = new MenuItem();
			MenuOther.Text = "Other";
			contextTop.MenuItems.Add(MenuOther);
			

			

			MenuDocumentDoctype = new MenuItem();
			MenuDocumentDoctype.Text = "Doctypes";
			MenuDocument.MenuItems.Add(MenuDocumentDoctype);


			InsertMenuItem("HTML 2.0", "doc20", true, MenuDocumentDoctype);
			InsertMenuItem("HTML 3.2", "doc32", true, MenuDocumentDoctype);
			InsertMenuItem("HTML 4.0 Strict", "doc40s", true, MenuDocumentDoctype);
			InsertMenuItem("HTML 4.0 Transitional", "doc40t", true, MenuDocumentDoctype);
			InsertMenuItem("HTML 4.0 Frameset", "doc40f", true, MenuDocumentDoctype);


			MenuItem sep;
			sep = new MenuItem();
			sep.Text = "-";
			MenuDocument.MenuItems.Add(sep);

			InsertMenuItem("html", true, MenuDocument);
			InsertMenuItem("head", true, MenuDocument);
			InsertMenuItem("title", true, MenuDocument);
			InsertMenuItem("meta", true, MenuDocument);
			InsertMenuItem("body", true, MenuDocument);

			InsertMenuItem("get", "frmget", true, MenuForms);
			InsertMenuItem("post", "frmpst", true, MenuForms);

			sep = new MenuItem();
			sep.Text = "-";
			MenuForms.MenuItems.Add(sep);

			InsertMenuItem("select", "select", true, MenuForms);
			InsertMenuItem("textarea", "text", true, MenuForms);

			MenuItem MenuFormsInputs = new MenuItem();
			MenuFormsInputs.Text = "Inputs";
			MenuForms.MenuItems.Add(MenuFormsInputs);

			InsertMenuItem("button", "inpbut", true, MenuFormsInputs);
			InsertMenuItem("checkbox", "inpchk", true, MenuFormsInputs);
			InsertMenuItem("file", "inpfil", true, MenuFormsInputs);
			InsertMenuItem("hidden", "inphid", true, MenuFormsInputs);
			InsertMenuItem("password", "inppwd", true, MenuFormsInputs);
			InsertMenuItem("radio", "inprad", true, MenuFormsInputs);
			InsertMenuItem("reset", "inpres", true, MenuFormsInputs);
			InsertMenuItem("submit", "inpsub", true, MenuFormsInputs);
			InsertMenuItem("text", "inptxt", true, MenuFormsInputs);

			InsertMenuItem("a href", "a hr", true, MenuLinks);
			InsertMenuItem("a name", "a nm", true, MenuLinks);
	
			InsertMenuItem("ol", true, MenuLists);
			InsertMenuItem("ul", true, MenuLists);
			InsertMenuItem("li", true, MenuLists);
			InsertMenuItem("dl", true, MenuLists);
			InsertMenuItem("dt", true, MenuLists);
			InsertMenuItem("dd", true, MenuLists);



			
			//MenuTablesCreate = new TagMenuItem();
			//MenuTablesCreate.Text = "Create";
			//MenuTables.MenuItems.Add(MenuTablesCreate);

			//sep = new MenuItem();
			//sep.Text = "-";
			//MenuTables.MenuItems.Add(sep);


			InsertMenuItem("table", true, MenuTables);
			InsertMenuItem("tbody", true, MenuTables);
			InsertMenuItem("th", true, MenuTables);
			InsertMenuItem("tr", true, MenuTables);
			InsertMenuItem("td", true, MenuTables);
			InsertMenuItem("caption", true, MenuTables);


			MenuTextFormat = new TagMenuItem();
			MenuTextFormat.Text = "Text Format";
			MenuText.MenuItems.Add(MenuTextFormat);

			MenuTextFont = new TagMenuItem();
			MenuTextFont.Text = "Text Font";
			MenuText.MenuItems.Add(MenuTextFont);

			MenuTextLayout = new TagMenuItem();
			MenuTextLayout.Text = "Text Layout";
			MenuText.MenuItems.Add(MenuTextLayout);

			MenuTextChars = new TagMenuItem();
			MenuTextChars.Text = "Special Characters";
			MenuText.MenuItems.Add(MenuTextChars);


			InsertMenuItem("font", true, MenuTextFont);
			InsertMenuItem("b", true, MenuTextFont);
			InsertMenuItem("i", true, MenuTextFont);
			InsertMenuItem("u", true, MenuTextFont);
			InsertMenuItem("strike", true, MenuTextFont);


			InsertMenuItem("big", true, MenuTextFormat);
			InsertMenuItem("cite", true, MenuTextFormat);
			InsertMenuItem("code", true, MenuTextFormat);
			InsertMenuItem("em", true, MenuTextFormat);
			InsertMenuItem("small", true, MenuTextFormat);
			InsertMenuItem("span", true, MenuTextFormat);
			InsertMenuItem("strong", true, MenuTextFormat);
			InsertMenuItem("sub", true, MenuTextFormat);
			InsertMenuItem("sup", true, MenuTextFormat);
			InsertMenuItem("tt", true, MenuTextFormat);


			InsertMenuItem("<blockquote>", "block", true, MenuTextLayout);
			InsertMenuItem("center", true, MenuTextLayout);
			InsertMenuItem("div", true, MenuTextLayout);
			InsertMenuItem("p", true, MenuTextLayout);
			InsertMenuItem("pre", true, MenuTextLayout);
		
			sep = new MenuItem();
			sep.Text = "-";
			MenuTextLayout.MenuItems.Add(sep);
		
			MenuTextLayoutFrames = new TagMenuItem();
			MenuTextLayoutFrames.Text = "Frames";
			MenuTextLayout.MenuItems.Add(MenuTextLayoutFrames);
		
			MenuTextLayoutHeadings = new TagMenuItem();
			MenuTextLayoutHeadings.Text = "Headings";			
			MenuTextLayout.MenuItems.Add(MenuTextLayoutHeadings);
		

			InsertMenuItem("h1", true, MenuTextLayoutHeadings);
			InsertMenuItem("h2", true, MenuTextLayoutHeadings);
			InsertMenuItem("h3", true, MenuTextLayoutHeadings);
			InsertMenuItem("h4", true, MenuTextLayoutHeadings);
			InsertMenuItem("h5", true, MenuTextLayoutHeadings);
			InsertMenuItem("h6", true, MenuTextLayoutHeadings);


			InsertMenuItem("<frameset>", "fset", true, MenuTextLayoutFrames);
			InsertMenuItem("frame", true, MenuTextLayoutFrames);
			InsertMenuItem("<noframe>", "nofrm", true, MenuTextLayoutFrames);


			InsertMenuItem("&&amp (&&)", "&", true, MenuTextChars);
			InsertMenuItem("&&gt (>)", ">", true, MenuTextChars);
			InsertMenuItem("&&lt (<)", "<", true, MenuTextChars);
			InsertMenuItem("&&quot (\")", "\"", true, MenuTextChars);
			InsertMenuItem("&&copy (©)", "copy", true, MenuTextChars);					
			InsertMenuItem("&&reg (®)", "reg", true, MenuTextChars);
			InsertMenuItem("&&trade (™)", "trade", true, MenuTextChars);
			InsertMenuItem("&&deg (°)", "deg", true, MenuTextChars);	
			InsertMenuItem("&&nbsp", "nbsp", true, MenuTextChars);


			
			
			InsertMenuItem("<!--  -->", "comment", true, MenuOther);
			InsertMenuItem("br", true, MenuOther);
			InsertMenuItem("hr", true, MenuOther);
			InsertMenuItem("img", true, MenuOther);





			*/
			#endregion
			


		}

		private void MakeMenuXTR()
		{
			string filename = Utility.GetCurrentDir(true) + "menu.xml";

			if(!File.Exists(filename))
			{
				throw new Exception("The file menu.xml could not be found.  Please make sure that menu.xml is in the same directory as pockethtml.exe");
			}

			XmlTextReader xtr = new XmlTextReader(filename);
			xtr.WhitespaceHandling = WhitespaceHandling.None;
			xtr.MoveToContent();
			Stack stack = new Stack();
			EventHandler eh = new EventHandler(TagMenuItem_Click);

			while(xtr.Read())
			{
				switch(xtr.NodeType)
				{
					case XmlNodeType.Element:
					{
						if(xtr.Name == "menuitems")
						{
							continue;
						}
						if(xtr.IsStartElement())
						{
							string name = xtr.GetAttribute("name");
							TagMenuItem newItem = new TagMenuItem();
							newItem.Click += eh;
							newItem.Text = name;
							newItem.Tag = name;
							currentMenu.MenuItems.Add(newItem);

							if(!xtr.IsEmptyElement)
							{
								stack.Push(currentMenu);
								currentMenu = newItem;
							}

						}
						break;
					}
					case XmlNodeType.EndElement:
					{
						if(xtr.Name == "menuitems")
						{
							continue;
						}

						currentMenu = (Menu)stack.Pop();
						break;		
					}
				}
			}
			xtr.Close();
		}

		private TagMenuItem InsertMenuItem(string text, bool handler, MenuItem menu)
		{
			return InsertMenuItem(text, String.Empty, handler, menu);
		}


		private TagMenuItem InsertMenuItem(string text, string tag, bool handler, 
			MenuItem menu)
		{
			TagMenuItem tmi = new TagMenuItem();
			if(tag == String.Empty)
			{
				tmi.Text = "<" + text + ">";
				tmi.Tag = text;
			}
			else
			{
				tmi.Text = text;
				tmi.Tag = tag;
			}			
			
			if(handler)
			{
				tmi.Click += new EventHandler(TagMenuItem_Click);
			}
			
			menu.MenuItems.Add(tmi);

			return tmi;
		}

		internal void SwapPanels(bool showHTML)
		{
			if(showHTML)
			{
				tbPanel.Hide();
				viewerPanel.Show();
				viewerPanel.BringToFront();

				/* TODO Do I really need to check this every time it gets swapped, or can it be 
						just when the options are changed?
				*/
				bool ct = ((PocketHTMLEditor)this.Parent).Config.GetBool("Options", "ClearType");
				if( htmlControl.Source != tb.Text ||//(htmlControl.HtmlString != tb.Text) ||
					(ct != htmlControl.EnableClearType))
									
				{
					//htmlControl.ClearType = ct;
					//htmlControl.HtmlString = tb.Text;
					htmlControl.EnableClearType = ct;
					htmlControl.Source = tb.Text;
				}	
			}
			else
			{
				viewerPanel.Hide();
				tbPanel.Show();
				tbPanel.BringToFront();
			}
		}

		internal void ShowAbout()
		{
			if(showAbout)
			{
				if(this.aboutPanel == null)
				{
					aboutPanel = new AboutPanel();
					aboutPanel.Parent = this;
					aboutPanel.Bounds = new Rectangle(0, 0, 240, 236);					
				}
				aboutPanel.Show();
				aboutPanel.BringToFront();
			}
			else
			{
				aboutPanel.Hide();
			}			
		}

		internal void ResizePanel(Microsoft.WindowsCE.Forms.InputPanel inputPanel1)
		{
			Rectangle visible;
			visible = inputPanel1.VisibleDesktop;
			if (inputPanel1.Enabled == true)
			{
				viewerPanel.Height = this.ClientSize.Height - 32 - inputPanel1.Bounds.Height;
				tbPanel.Height = this.ClientSize.Height - 32 - inputPanel1.Bounds.Height;
				buttonPanel.Top = visible.Height - 32;
			}
			else
			{
				viewerPanel.Height = this.ClientSize.Height - 32;
				tbPanel.Height = this.ClientSize.Height - 32;
				buttonPanel.Top = this.ClientSize.Height - 32;
			}

			tb.Height = tbPanel.Height;
			htmlControl.Bounds = viewerPanel.ClientRectangle;
		}

		private void TagMenuItem_Click(object sender, EventArgs e)
		{
			TagMenuItem tmi = sender as TagMenuItem;			

			if(TagMenuClicked != null)
			{
				TagMenuClicked(tmi.Tag);
			}
		}
	} 

	internal class TagMenuItem : MenuItem
	{
		private string tag;
		
		/// <summary>
		/// Property Tag (string)
		/// </summary>
		public string Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
				
			}
		}
	}
}
