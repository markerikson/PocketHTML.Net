using System;
using Ayende;
using System.Text;

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for Tag.
	/// </summary>
	class Tag
	{
		private string name;
		private bool individualTag;
		private string defaultInnerTag;
		private string shortName;
		private string[] defaultAttributes;
		private string startTag;
		private string endTag;
		private bool innerTags;
		private bool multiLineTag;
		private bool normalTag;
		private Configuration config;
		

		public Configuration Config
		{
			set
			{
				this.config = value;
			}
		}


		/// <summary>
		/// Property NormalTag (bool)
		/// </summary>
		public bool NormalTag
		{
			get
			{
				return this.normalTag;
			}
			set
			{
				this.normalTag = value;
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

		public Tag()
		{
			name = String.Empty;
			individualTag = false;
			defaultInnerTag = String.Empty;
			shortName = String.Empty;
			defaultAttributes = new String[0];
			startTag = String.Empty;
			endTag = String.Empty;
			innerTags = false;
			normalTag = true;
			config = null;
		}
		
		/// <summary>
		/// Property EndTag (string)
		/// </summary>
		public string EndTag
		{
			get
			{
				if(endTag == String.Empty)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("</");
					sb.Append(name);
					sb.Append(">");
					endTag = sb.ToString();
				}
				return this.endTag;
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

				if(this.NormalTag)
				{
					sb.Append("<");
				}
					
				sb.Append(this.Name);

				foreach(string s in defaultAttributes)
				{
					sb.Append(" ");
					sb.Append(s);
					sb.Append("=\"\"");
				}

					

				if(this.normalTag)
				{
					// for XHTML compatibility
					if(config.GetBool("Options", "XHTMLTags") && 
						this.IndividualTag)
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
				return this.defaultAttributes ;
			}
			set
			{
				this.defaultAttributes  = value;
			}
		}
		/// <summary>
		/// Property ShortName  (string)
		/// </summary>
		public string ShortName 
		{
			get
			{
				return this.shortName ;
			}
			set
			{
				this.shortName  = value;
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
		/// Property IndividualTag (bool)
		/// </summary>
		public bool IndividualTag
		{
			get
			{
				return this.individualTag;
			}
			set
			{
				this.individualTag = value;
			}
		}	


		/// <summary>
		/// Property Name (string)
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				if(name.IndexOf("$C", 0, name.Length) != -1)
				{
					name = name.Replace("$C", ",");
				}

				if(name.IndexOf("$N", 0, name.Length) != -1)
				{
					name = name.Replace("$N", "\r\n");
				}

			}
		}
	}
}
