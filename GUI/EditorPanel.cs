#region using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using OpenNETCF.Windows.Forms;
using ISquared.Win32Interop;
using OpenNETCF.Win32;
#endregion

namespace ISquared.PocketHTML
{
	class EditorPanel : System.Windows.Forms.Panel
	{
		#region Delegates
		public delegate bool TagMenuClickedHandler (string tagname);
		public TagMenuClickedHandler TagMenuClicked;
		#endregion

		#region private members
		private System.Windows.Forms.Panel m_pnlButtons;
		private System.Windows.Forms.Panel m_pnlViewer;
		private AboutPanel m_pnlAbout;
		private System.Windows.Forms.Panel m_pnlTextbox;

		private HTMLViewer m_htmlControl;
		private OpenNETCF.Windows.Forms.TextBoxEx m_textbox;
		
		private System.Windows.Forms.ContextMenu m_mnuContextTags;
		private Menu m_currentMenu;
		private NamedButton[] m_buttons;

		private System.Collections.Hashtable m_htMenuTags;
		#endregion

		#region properties
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

		public HTMLViewer HtmlControl
		{
			get
			{
				return this.m_htmlControl;
			}
			set
			{
				this.m_htmlControl = value;
			}
		}		
		
		// TODO Is there a better way to handle this than exposing the buttons as a property?

		public NamedButton[] Buttons
		{
			get
			{
				return this.m_buttons;
			}
			set
			{
				this.m_buttons = value;
			}
		}
		#endregion

		#region Constructor
		public EditorPanel()
		{
			Initialize();

			this.m_pnlViewer.Hide();
		}
		#endregion

		#region Setup methods
		public void Initialize()
		{
			m_htMenuTags = new Hashtable();

			#region form setup
			
			m_pnlButtons = new Panel();
			m_pnlButtons.Parent = this;

			m_pnlViewer = new Panel();
			m_pnlViewer.Parent = this;

			m_pnlTextbox = new Panel();
			m_pnlTextbox.Parent = this;

			m_textbox = new TextBoxEx();
			this.m_textbox.AcceptsTab = true;
			this.m_textbox.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.m_textbox.Location = new System.Drawing.Point(0, 0);
			this.m_textbox.MaxLength = 16777215;
			this.m_textbox.Multiline = true;
			this.m_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_textbox.Text = "";
			m_textbox.Parent = m_pnlTextbox;

			// TODO Can I delay-load this, too?
			m_htmlControl = new HTMLViewer();
			m_htmlControl.Bounds = m_pnlViewer.Bounds;
			m_htmlControl.Parent = m_pnlViewer;

			this.Buttons = new NamedButton[16];

			for (int i = 0; i < 16; i++)
			{
				NamedButton nb = new NamedButton();
				nb.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
				nb.Name = "button" + Convert.ToString(i + 1);
				nb.Parent = m_pnlButtons;
				this.m_buttons[i] = nb;
			}

			IntPtr pTB = CoreDLL.GetHandle(m_textbox);
			CoreDLL.SendMessage(pTB, (int)EM.LIMITTEXT, 0xFFFFFFF, 0);

			#endregion

			SetupTagMenu();
			
		}

		public void SetupTagMenu()
		{
			m_mnuContextTags = new ContextMenu();
			m_textbox.ContextMenu = m_mnuContextTags;
			m_currentMenu = m_mnuContextTags;

			MakeMenuXTR();
		}

		public void UpdateLayout()
		{
            if (PocketHTML.PocketHTMLEditor.DoLayout)
            {

                if (Screen.PrimaryScreen.Bounds.Width > Screen.PrimaryScreen.Bounds.Height)
                {
                    LayoutLandscape();
                }
                else
                {
                    LayoutPortrait();
                }

                m_htmlControl.Bounds = m_pnlViewer.ClientRectangle;
                m_htmlControl.ResizeHTMLControl();

                //DpiHelper.AdjustAllControls(this);
            }
		}

		private void LayoutPortrait()
		{
            /*
			this.Bounds = new Rectangle(0, 0, 240, 270);
			m_pnlButtons.Bounds = new Rectangle(0, 238, 240, 32);
			m_pnlViewer.Bounds = new Rectangle(0, 0, 240, 238);
			m_pnlTextbox.Bounds = new Rectangle(0, 0, 240, 238);
            */

            int scale = DpiHelper.Scale(1);
            int scaled240 = scale * 240;
            int scaled238 = scale * 238;
            int scaled32 = scale * 32;

            this.Bounds = Parent.ClientRectangle;
            //m_pnlButtons.Bounds = new Rectangle(0, scaled238, scaled240, scale * 32);
            //m_pnlViewer.Bounds = new Rectangle(0, 0, scaled240, scaled238);
            //m_pnlTextbox.Bounds = new Rectangle(0, 0, scaled240, scaled238);


            m_pnlButtons.Bounds = new Rectangle(0, ClientSize.Height - scaled32, this.Width, scaled32);
            m_pnlViewer.Bounds = new Rectangle(0, 0, this.Width, this.Height);
            m_pnlTextbox.Bounds = new Rectangle(0, 0, this.Width, this.Height);

            this.m_textbox.Size = new System.Drawing.Size(this.Width, this.Height - scaled32);
        

			int width = scale * 30;
			int height = scale * 16;
			int y = 0;
			int x = 0;

			for (int i = 0; i < 16; i++)
			{
				NamedButton nb = this.Buttons[i];
				x = width * (i % 8);
				if (i > 7)
				{
                    y = height;
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

            int scale = DpiHelper.Scale(1);
            int scaled32 = DpiHelper.Scale(32);

			this.Bounds = new Rectangle(0, 0, availableWidth, scale * 188);
            m_pnlButtons.Bounds = new Rectangle(0, this.Height - scaled32, this.Width, scaled32);
			m_pnlViewer.Bounds = new Rectangle(0, 0, this.Width, this.Height);
			m_pnlTextbox.Bounds = new Rectangle(0, 0, this.Width, this.Height);


            this.m_textbox.Size = new System.Drawing.Size(this.Width, this.Height - scaled32);

			int width = scale * 40;
			int height = scale * 16;
			int y = 0;
			int x = 0;

			for (int i = 0; i < 16; i++)
			{
				NamedButton nb = this.Buttons[i];
				x = width * (i % 8);
				if (i > 7)
				{
					y = height;
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

			// If menu.xml doesn't exist, we're pretty much dead anyway.
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

			m_currentMenu.MenuItems.Add(copy);
			m_currentMenu.MenuItems.Add(cut);
			m_currentMenu.MenuItems.Add(paste);
			m_currentMenu.MenuItems.Add(sep);


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
							m_currentMenu.MenuItems.Add(newItem);

							if(!xtr.IsEmptyElement)
							{
								stack.Push(m_currentMenu);
								m_currentMenu = newItem;
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

						m_currentMenu = (Menu)stack.Pop();
						break;		
					}
				}
			}
			xtr.Close();
		}
		#endregion

		#region Tag insertion methods
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

		private void TagMenuItem_Click(object sender, EventArgs e)
		{
			TagMenuItem tmi = sender as TagMenuItem;

			if (TagMenuClicked != null)
			{
				TagMenuClicked(tmi.Tag);
			}
		}
		#endregion

		#region Panel management
		internal void SwapPanels(bool showHTML)
		{
			if(showHTML)
			{
				m_pnlTextbox.Hide();
                m_pnlButtons.Hide();
                m_pnlButtons.SendToBack();
				m_pnlViewer.Show();
				m_pnlViewer.BringToFront();

				/* TODO Do I really need to check this every time it gets swapped, or can it be 
						just when the options are changed?
				*/
				bool ct = PocketHTMLEditor.Config.GetBool("Options", "ClearType");
				
				if (m_htmlControl.Source != m_textbox.Text ||
					(ct != m_htmlControl.EnableClearType))
									
				{
					m_htmlControl.EnableClearType = ct;
					m_htmlControl.Source = m_textbox.Text;
				}	
			}
			else
			{
				m_pnlViewer.Hide();
				m_pnlTextbox.Show();
				m_pnlTextbox.BringToFront();
                m_pnlButtons.Show();
                m_pnlButtons.BringToFront();
			}
		}

		internal void ShowAbout()
		{
			if(this.m_pnlAbout == null)
			{
				m_pnlAbout = new AboutPanel();
				m_pnlAbout.Parent = this;
				//m_pnlAbout.Bounds = new Rectangle(0, 0, 240, 236);					
			}
            m_pnlAbout.Bounds = this.ClientRectangle;
            m_pnlAbout.UpdateLayout();
			m_pnlAbout.Show();
			m_pnlAbout.BringToFront();
		}

		internal void ResizePanel(Microsoft.WindowsCE.Forms.InputPanel inputPanel1)
		{
			Rectangle visible;
			visible = inputPanel1.VisibleDesktop;
			int scaledSize = DpiHelper.Scale(32);

			if (inputPanel1.Enabled == true)
			{
				m_pnlViewer.Height = this.ClientSize.Height - inputPanel1.Bounds.Height;
				m_pnlTextbox.Height = this.ClientSize.Height - scaledSize - inputPanel1.Bounds.Height;
				m_pnlButtons.Top = visible.Height - scaledSize;
			}
			else
			{
				m_pnlViewer.Height = this.ClientSize.Height;
				m_pnlTextbox.Height = this.ClientSize.Height - scaledSize;
				m_pnlButtons.Top = this.ClientSize.Height - scaledSize;
			}

			m_textbox.Height = m_pnlTextbox.Height;
			m_htmlControl.Bounds = m_pnlViewer.ClientRectangle;
		}
		#endregion

		
	} 

	internal class TagMenuItem : MenuItem
	{
		private string tag;
		
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
