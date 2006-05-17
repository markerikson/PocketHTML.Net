#region Using directives
using System;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OpenNETCF.Windows.Forms;

#endregion

namespace ISquared.PocketHTML
{
	public class PocketHTMLShared
	{
		private static bool m_indentHTML = true;

		public static bool IndentHTML
		{
			set
			{
				m_indentHTML = value;
			}
			get
			{
				return m_indentHTML;
			}
		}

		#region PocketHTML utility functions
		public static string DecodeData(Stream stream, Encoding defaultEncoding)
		{
			string charset = null;
			MemoryStream rawdata = new MemoryStream();
			byte[] buffer = new byte[1024];
			int read = stream.Read(buffer, 0, buffer.Length);
			while (read > 0)
			{
				rawdata.Write(buffer, 0, read);
				read = stream.Read(buffer, 0, buffer.Length);
			}

			stream.Close();

			//
			// if ContentType is null, or did not contain charset, we search in body
			//

			MemoryStream ms = rawdata;
			ms.Seek(0, SeekOrigin.Begin);

			StreamReader srr = new StreamReader(ms, Encoding.ASCII);
			String meta = srr.ReadToEnd();

			if (meta != null)
			{
				int start_ind = meta.IndexOf("charset=");
				int end_ind = -1;
				if (start_ind != -1)
				{
					end_ind = meta.IndexOf("\"", start_ind);
					if (end_ind != -1)
					{
						int start = start_ind + 8;
						charset = meta.Substring(start, end_ind - start + 1);
						charset = charset.TrimEnd(new Char[] { '>', '"' });
						Console.WriteLine("META: charset=" + charset);
					}
				}
			}


			Encoding e = null;
			if (charset == null)
			{
				e = defaultEncoding;//Encoding.UTF8; //default encoding
			}
			else
			{
				try
				{
					e = Encoding.GetEncoding(charset);
				}
				catch (Exception ee)
				{
					Console.WriteLine("Exception: GetEncoding: " + charset);
					Console.WriteLine(ee.ToString());
					e = new UTF8Encoding(false); //Encoding.UTF8;
				}
			}

			rawdata.Seek(0, SeekOrigin.Begin);

			StreamReader sr = new StreamReader(rawdata, e);

			String s = sr.ReadToEnd();

			return s;
		}

		public static Hashtable LoadTagsXTR()
		{
			Hashtable tagHash = new Hashtable();
			string filename = Utility.GetCurrentDir(true) + "tags.xml";
			XmlTextReader xtr = new XmlTextReader(filename);
			xtr.WhitespaceHandling = WhitespaceHandling.None;
			xtr.MoveToContent();

			Tag currentTag = null;

			while (xtr.Read())
			{
				switch (xtr.NodeType)
				{
					case XmlNodeType.Element:
					{
						switch (xtr.Name)
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
								tagHash[t.DisplayName] = t;

								break;
							}
							case "Attributes":
							{
								ArrayList al = new ArrayList();
								while (xtr.Read())
								{
									if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "Attributes")
									{
										break;
									}
									if (xtr.NodeType == XmlNodeType.Text)
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

			xtr.Close();
			return tagHash;
		}

		public static bool InsertTag(Tag tag, TextBoxEx tb, Hashtable htTags)
		{
			//TextBoxEx tb = m_editorPanel.TextBox;
			StringBuilder sb = new StringBuilder();
			String spaces = tb.GetLeadingSpaces();
			string newline = "\r\n";

			// basically, an additional level of indentation
			// TODO make this an option?
			String spacesPlus = spaces + "\t";

			int currentLineNum = tb.CurrentLine;

			int selstart = tb.SelectionStart;

			int newLineNum = currentLineNum;

			string seltext = tb.SelectedText;

			bool indentHTML = IndentHTML;//m_config.GetBool("Options", "IndentHTML");

			// We'll need a start tag AND an end tag
			if (tag.ClosingTag)
			{
				sb.Append(tag.StartTag);
				if (tag.MultiLineTag)
				{
					sb.Append("\r\n");
				}
				// 
				if (tag.InnerTags)
				{
					Tag innerTag = (Tag)htTags[tag.DefaultInnerTag];
					if (indentHTML && tag.MultiLineTag)
					{
						sb.Append(spacesPlus);
					}
					sb.Append(innerTag.StartTag);
					if (tag.MultiLineTag)
					{
						sb.Append(newline);
						if (indentHTML)
						{
							sb.Append(spacesPlus);
						}
						if (seltext != String.Empty)
						{
							sb.Append(seltext);
						}
						sb.Append(newline);
						if (indentHTML)
						{
							sb.Append(spacesPlus);
						}
					}
					sb.Append(innerTag.EndTag);
				}

				if (tag.MultiLineTag)
				{
					if (indentHTML)
					{
						sb.Append(spaces);
					}
				}

				if ((seltext != String.Empty) &&
					(!tag.InnerTags))
				{
					sb.Append(seltext);
				}

				if (tag.MultiLineTag)
				{
					if (tag.ClosingTag)
					{
						sb.Append(newline);
					}
					sb.Append(spaces);
				}

				sb.Append(tag.EndTag);
				if (tag.MultiLineTag)
				{
					sb.Append(newline);
				}
			}
			else
			{
				sb.Append(tag.StartTag);
				if (tag.MultiLineTag)
				{
					sb.Append(newline);
					if (indentHTML)
					{
						sb.Append(spaces);
					}
				}
			}

			tb.ReplaceSelection(sb.ToString());

			int charIndex = 0;
			int cursorLocationIndex = 0;

			if (tag.DefaultAttributes.Length > 0)
			{
				charIndex = tb.GetLineIndex(currentLineNum);

				int firstUninitializedAttribute = -1;
				int temp = -1;
				for (int i = 0; i < tag.DefaultAttributes.Length; i++)
				{
					temp = tag.DefaultAttributes[i].IndexOf("\"");
					if (temp == -1)
					{
						firstUninitializedAttribute = i;
						break;
					}
				}

				// TODO What was I doing here?
				//int idx = tag.Name.IndexOf("\"");
				int idx = tag.Value.IndexOf("\"");
				if (firstUninitializedAttribute != -1)
				{
					string fua = tag.DefaultAttributes[firstUninitializedAttribute];
					idx = tag.StartTag.IndexOf(fua) + fua.Length + 2;
				}
				else
				{
					int t1 = tag.StartTag.IndexOf("\"") + 1;
					int t2 = tag.StartTag.IndexOf("\"", t1);
					if (t2 == -1)
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
			else if (tag.AngleBrackets)
			{
				if (tag.MultiLineTag)
				{
					newLineNum += 1;

					if (tag.InnerTags)
					{
						newLineNum += 1;
						if (indentHTML)
						{
							cursorLocationIndex = spacesPlus.Length;
						}
					}
					else
					{
						if (indentHTML)
						{
							cursorLocationIndex = spaces.Length;
						}
					}

					charIndex = tb.GetLineIndex(newLineNum);
				}

				else
				{
					if (tag.ClosingTag)
					{
						charIndex = tb.SelectionStart;
						cursorLocationIndex -= tag.EndTag.Length;

						if (tag.InnerTags)
						{
							Tag innerTag = (Tag)htTags[tag.DefaultInnerTag];
							cursorLocationIndex -= innerTag.EndTag.Length;
						}
					}
				}

				charIndex += cursorLocationIndex;
			}

			// TODO What was I doing here?
			//if(tag.NormalTag)
			if (tag.AngleBrackets)
			{
				tb.Focus();
				tb.SelectionStart = charIndex;
				tb.SelectionLength = 0;
			}

			tb.Modified = true;

			return true;
		}


		#endregion
	}
}
