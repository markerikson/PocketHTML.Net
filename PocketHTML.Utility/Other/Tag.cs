#region using directives
using System;
using System.Text;
#endregion

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for Tag.
	/// </summary>
	public class Tag : IComparable
	{
		#region private members
		private string val;
		private string displayName;
		private bool closingTag;
		private string defaultInnerTag;
		private string shortName;
		private string[] defaultAttributes;
		private string startTag;
		private string endTag;
		private bool innerTags;
		private bool multiLineTag;
		private bool angleBrackets;

		private static bool m_xhtmlTags = true;
		#endregion

		#region properties
		static public bool XHTMLTags
		{
			get
			{
				return m_xhtmlTags;
			}
			set
			{
				m_xhtmlTags = value;
			}
		}

		/// <summary>
		/// Property NormalTag (bool)
		/// </summary>
		public bool AngleBrackets
		{
			get
			{
				return this.angleBrackets;
			}
			set
			{
				this.angleBrackets = value;
			}
		}
		/// <summary>
		/// Property MultiLineTag (bool)
		/// </summary>
		public bool MultiLineTag
		{
			get
			{
				return this.multiLineTag;
			}
			set
			{
				this.multiLineTag = value;
			}
		}

		/// <summary>
		/// Property InnerTags (bool)
		/// </summary>
		public bool InnerTags
		{
			get
			{
				return this.innerTags;
			}
			set
			{
				this.innerTags = value;
			}
		}

		/// <summary>
		/// Property EndTag (string)
		/// </summary>
		public string EndTag
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if(angleBrackets)
				{
					sb.Append("</");
				}
				
				sb.Append(val);

				if(angleBrackets)
				{
					sb.Append(">");
				}
				
				return sb.ToString();
			}

		}

		/// <summary>
		/// Property Start (string)
		/// </summary>
		public string StartTag
		{
			get
			{
				StringBuilder sb = new StringBuilder();

				if (angleBrackets)
				{
					sb.Append("<");
				}

				sb.Append(this.Value);

				foreach (string s in defaultAttributes)
				{
					sb.Append(" ");
					sb.Append(s);

					if (s.IndexOf("=") == -1)
					{
						sb.Append("=\"\"");
					}
				}

				if (angleBrackets)
				{
					// for XHTML compatibility

					//if(PocketHTMLEditor.Config.GetBool("Options", "XHTMLTags") 
					if (XHTMLTags
						&& !this.closingTag
						&& this.shortName != "comment")
					{
						sb.Append(" /");
					}

					sb.Append(">");
				}

				startTag = sb.ToString();

				return this.startTag;
			}
		}
		/// <summary>
		/// Property DefaultAttributes  (string[])
		/// </summary>
		public string[] DefaultAttributes
		{
			get
			{
				return this.defaultAttributes;
			}
			set
			{
				this.defaultAttributes = value;
			}
		}
		/// <summary>
		/// Property ShortName  (string)
		/// </summary>
		public string ShortName
		{
			get
			{
				return this.shortName;
			}
			set
			{
				this.shortName = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}
		/// <summary>
		/// Property DefaultInnerTag (string)
		/// </summary>
		public string DefaultInnerTag
		{
			get
			{
				return this.defaultInnerTag;
			}
			set
			{
				this.defaultInnerTag = value;
			}
		}
		/// <summary>
		/// Property closingTag (bool)
		/// </summary>
		public bool ClosingTag
		{
			get
			{
				return this.closingTag;
			}
			set
			{
				this.closingTag = value;
			}
		}


		/// <summary>
		/// Property Name (string)
		/// </summary>
		public string Value
		{
			get
			{
				return this.val;
			}
			set
			{
				this.val = value;
				if (val.IndexOf("$C", 0, val.Length) != -1)
				{
					val = val.Replace("$C", ",");
				}

				if (val.IndexOf("$N", 0, val.Length) != -1)
				{
					val = val.Replace("$N", "\r\n");
				}

			}
		}
		#endregion

		#region Constructor
		public Tag()
		{
			displayName = String.Empty;
			val = String.Empty;
			closingTag = false;
			defaultInnerTag = String.Empty;
			shortName = String.Empty;
			defaultAttributes = new String[0];
			startTag = String.Empty;
			endTag = String.Empty;
			innerTags = false;
			angleBrackets = true;
			//config = null;
		}
		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is Tag)
			{
				Tag t = (Tag)obj;
				return this.DisplayName.CompareTo(t.DisplayName);
			}

			throw new ArgumentException("Object is not a Tag");

		}

		#endregion

		#region Other functions
		public Tag Clone()
		{
			return (Tag)this.MemberwiseClone();
		}
		#endregion
	}
}
