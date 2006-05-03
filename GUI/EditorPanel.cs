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
using OpenNETCF.Win32;
//using ISquared.Win32Interop.WinEnums;



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
		//private System.Windows.Forms.TextBox textBox1;
		private OpenNETCF.Windows.Forms.TextBoxEx m_textbox;
		private IntPtr pTB;
		private System.Collections.Hashtable menuTags;
		private System.Windows.Forms.ContextMenu contextTop;
		private NamedButton[] buttons;
		private bool showAbout;

		private HTMLViewer htmlControl;

		private Menu currentMenu;		
		
		/// <summary>
		/// Property Tb (TextBox)
		/// </summary>
		public TextBoxEx TextBox
		{
			get
			{
				return this.m_textbox;
			}
			set
			{
				this.m_textbox = value;
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
			
			buttonPanel = new Panel();
			buttonPanel.Parent = this;

			viewerPanel = new Panel();
			viewerPanel.Parent = this;

			tbPanel = new Panel();
			tbPanel.Parent = this;

			m_textbox = new TextBoxEx();
			this.m_textbox.AcceptsTab = true;
			this.m_textbox.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_textbox.Location = new System.Drawing.Point(0, 0);
			this.m_textbox.MaxLength = 16777215;
			this.m_textbox.Multiline = true;
			this.m_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_textbox.Text = "";
			m_textbox.Parent = tbPanel;

			// TODO Can I delay-load this, too?
			htmlControl = new HTMLViewer();
			htmlControl.Bounds = viewerPanel.Bounds;
			htmlControl.Parent = viewerPanel;
			/*
			m_browser = new WebBrowser();
			m_browser.Bounds = viewerPanel.Bounds;
			m_browser.Parent = viewerPanel;
			m_browser.BorderStyle = BorderStyle.None;
			*/

			//htmlControl.CreateHTMLControl();

			int width = 30;
			int height = 16;
			int y = 0;
			int x = 0;

			this.Buttons = new NamedButton[16];

			for (int i = 0; i < 16; i++)
			{
				NamedButton nb = new NamedButton();
				nb.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
				nb.Name = "button" + Convert.ToString(i + 1);
				nb.Parent = buttonPanel;
				this.buttons[i] = nb;
			}

			pTB = CoreDLL.GetHandle(m_textbox);
			CoreDLL.SendMessage(pTB, (int)EM.LIMITTEXT, 0xFFFFFFF, 0);

			#endregion

			contextTop = new ContextMenu();
			m_textbox.ContextMenu = contextTop;
			currentMenu = contextTop;

			MakeMenuXTR();

			//UpdateLayout();

		}

		//protected override void OnResize(EventArgs e)
		//{
		//	base.OnResize(e);
		public void UpdateLayout()
		{
			if (Screen.PrimaryScreen.Bounds.Width > Screen.PrimaryScreen.Bounds.Height)
			{
				LayoutLandscape();
			}
			else
			{
				LayoutPortrait();
			}

			DpiHelper.AdjustAllControls(this);
		}

		private void LayoutPortrait()
		{
			int availableWidth = Screen.PrimaryScreen.WorkingArea.Width;
			int availableHeight = Screen.PrimaryScreen.WorkingArea.Height;

			this.Bounds = new Rectangle(0, 0, 240, 270);
			buttonPanel.Bounds = new Rectangle(0, 238, 240, 32);
			viewerPanel.Bounds = new Rectangle(0, 0, 240, 237);
			tbPanel.Bounds = new Rectangle(0, 0, 240, 237);


			this.m_textbox.Size = new System.Drawing.Size(240, 237);

			int width = 30;
			int height = 16;
			int y = 0;
			int x = 0;

			for (int i = 0; i < 16; i++)
			{
				NamedButton nb = this.Buttons[i];
				x = width * (i % 8);
				if (i > 7)
				{
					y = 16;
				}
				else
				{
					y = 0;
				}

				nb.Bounds = new Rectangle(x, y, width, height);
			}
		}

		private void LayoutLandscape()
		{
			int availableWidth = Screen.PrimaryScreen.WorkingArea.Width;
			int availableHeight = Screen.PrimaryScreen.WorkingArea.Height;

			this.Bounds = new Rectangle(0, 0, availableWidth, 188);
			buttonPanel.Bounds = new Rectangle(0, this.Height - 32, this.Width, 32);
			viewerPanel.Bounds = new Rectangle(0, 0, this.Width, this.Height);
			tbPanel.Bounds = new Rectangle(0, 0, this.Width, this.Height);


			this.m_textbox.Size = new System.Drawing.Size(this.Width, this.Height - 32);

			int width = 40;
			int height = 16;
			int y = 0;
			int x = 0;

			for (int i = 0; i < 16; i++)
			{
				NamedButton nb = this.Buttons[i];
				x = width * (i % 8);
				if (i > 7)
				{
					y = 16;
				}
				else
				{
					y = 0;
				}

				nb.Bounds = new Rectangle(x, y, width, height);
			}
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

			MenuItem copy = new MenuItem();
			copy.Text = "Copy";
			MenuItem cut = new MenuItem();
			cut.Text = "Cut";
			MenuItem paste = new MenuItem();
			paste.Text = "Paste";
			MenuItem sep = new MenuItem();
			sep.Text = "-";

			currentMenu.MenuItems.Add(copy);
			currentMenu.MenuItems.Add(cut);
			currentMenu.MenuItems.Add(paste);
			currentMenu.MenuItems.Add(sep);


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
				bool ct = PocketHTMLEditor.Config.GetBool("Options", "ClearType");
				
				if (htmlControl.Source != m_textbox.Text ||//(htmlControl.HtmlString != tb.Text) ||
					(ct != htmlControl.EnableClearType))
									
				{
					//htmlControl.ClearType = ct;
					//htmlControl.HtmlString = tb.Text;
					htmlControl.EnableClearType = ct;
					htmlControl.Source = m_textbox.Text;
				}	
				 
				/*
				if(ct != m_browser.EnableClearType)
				{
					m_browser.EnableClearType = ct;
				}
				
				m_browser.DocumentText = m_textbox.Text;
				*/
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
			int scaledSize = DpiHelper.Scale(32);
			if (inputPanel1.Enabled == true)
			{
				viewerPanel.Height = this.ClientSize.Height - scaledSize - inputPanel1.Bounds.Height;
				tbPanel.Height = this.ClientSize.Height - scaledSize - inputPanel1.Bounds.Height;
				buttonPanel.Top = visible.Height - scaledSize;
			}
			else
			{
				viewerPanel.Height = this.ClientSize.Height - scaledSize;
				tbPanel.Height = this.ClientSize.Height - scaledSize;
				buttonPanel.Top = this.ClientSize.Height - scaledSize;
			}

			m_textbox.Height = tbPanel.Height;
			htmlControl.Bounds = viewerPanel.ClientRectangle;
			//m_browser.Bounds = viewerPanel.ClientRectangle;
			//m_browser.BorderStyle = BorderStyle.Fixed3D;
			//m_browser.Height -= 2;
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
