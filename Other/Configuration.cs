using System;
using System.IO;
using System.Collections;

namespace Ayende
{
	namespace Exceptions
	{
		class InvalidInputException : System.Exception { }
		class ValueDoesntExistException : System.Exception {}
		class SectionDoesntExistException : System.Exception {}
	}
	/// <summary>
	/// 
	/// </summary>
	public class Configuration
	{
		/// <summary>
		/// The key is a string, the value is another Hashtable, which contains the values.
		/// </summary>
		System.Collections.Hashtable Sections;
		/// <summary>
		/// This contains values without Section
		/// </summary>
		System.Collections.Hashtable Values;

		/// <summary>
		/// Construct an empty configuration
		/// </summary>
		public Configuration()
		{
			Sections = new System.Collections.Hashtable();
			Values = new System.Collections.Hashtable();
		}

		/// <summary>
		/// Construct the class from data got through the text reader
		/// </summary>
		/// <param name="tr">The source for constructing the class</param>
		public Configuration(System.IO.TextReader tr)
		{
			Sections = new System.Collections.Hashtable();
			Values = new System.Collections.Hashtable();
			Parse(tr);
		}
	
		/// <summary>
		/// Get a value from the class
		/// </summary>
		/// <param name="Section">The section from which to get the value</param>
		/// <param name="ValueName">The value name in the section</param>
		/// <returns>The value of the value name inside the section</returns>
		public string GetValue(string Section, string ValueName)
		{
			System.Collections.Hashtable SectionTable = (System.Collections.Hashtable)Sections[Section];
			if(SectionTable!=null)
			{
				if(SectionTable.ContainsKey(ValueName))
				{
					return (string)SectionTable[ValueName];
				}
				else
				{
					throw new Exceptions.ValueDoesntExistException();
				}

			}
			else
			{
				throw new Exceptions.SectionDoesntExistException();
			}
		}

		/// <summary>
		/// Get a value from ValueName without a section
		/// </summary>
		/// <param name="ValueName">The valuename to get from</param>
		/// <returns>The value of the value name</returns>
		public string GetValue(string ValueName)
		{
			if(Values.ContainsKey(ValueName))
			{
				return (string)Values[ValueName];
			}
			else
			{
				throw new Exceptions.ValueDoesntExistException();
			}
		}

		/// <summary>
		/// Checks if a spesified section is in the class
		/// </summary>
		/// <param name="SectionName">The name of the section</param>
		/// <returns>Whatever the section exists or not</returns>
		public bool SectionExists(string SectionName)
		{
			return Sections.Contains(SectionName);
		}

		/// <summary>
		/// Checks if a value exists in the class
		/// </summary>
		/// <param name="SectionName">The name of the section, the section must exists</param>
		/// <param name="ValueName">The name of the value to check</param>
		/// <returns>Whatever the value exists in the section or not</returns>
		public bool ValueExists(string SectionName,string ValueName)
		{
			System.Collections.Hashtable SectionTable = (System.Collections.Hashtable)Sections[SectionName];
			if(SectionTable!=null)
			{
				return SectionTable.Contains(ValueName);
			}
			else
			{
				//throw new Exceptions.SectionDoesntExistException();
				return false;
			}
		}

		/// <summary>
		/// Checks if a value exists in the class
		/// </summary>
		/// <param name="ValueName">Name of the kye to check</param>
		/// <returns>Whatever the value exists or not</returns>
		public bool ValueExists(string ValueName)
		{
			return Values.Contains(ValueName);
		}

		/// <summary>
		/// Set a value in the configuration data
		/// </summary>
		/// <param name="Section">Must already exist</param>
		/// <param name="ValueName">Doesn't have to exist before calling this function</param>
		/// <param name="Value">The value that will be stored in ValueName</param>
		public void SetValue(string Section, string ValueName, string Value)
		{
			System.Collections.Hashtable SectionTable = (System.Collections.Hashtable)Sections[Section];
			if(SectionTable!=null)
			{
				SectionTable[ValueName] = Value;
			}
			else
			{
				throw new Exceptions.SectionDoesntExistException();
			}
		}

		/// <summary>
		/// Set a value in the configuration data without the need for a section
		/// </summary>
		/// <param name="ValueName">Doesn't have to exist before-hand</param>
		/// <param name="Value">The value that will be stored in ValueName</param>
		public void SetValue(string ValueName, string Value)
		{
			Values[ValueName] = Value;
		}


		/// <summary>
		/// Add a section to the class, attempts to add a section already existing are ignored.
		/// </summary>
		/// <param name="NewSection">The new section name</param>
		public void AddSection(string NewSection)
		{
			System.Collections.Hashtable New = new System.Collections.Hashtable();
			Sections[NewSection] = New;
		}

		/// <summary>
		/// Saves the class data in the form of INI file into the TextWriter
		/// </summary>
		/// <param name="sw">The destination for the output</param>
		public void Save(System.IO.TextWriter sw)
		{
			CaseInsensitiveComparer cic = new CaseInsensitiveComparer();
			System.Collections.IDictionaryEnumerator Enumerator = Values.GetEnumerator();
			//Print values
			sw.WriteLine("; The values in this group");
			while(Enumerator.MoveNext())
			{
				sw.WriteLine("{0} = {1}",Enumerator.Key,Enumerator.Value);
			}
			sw.WriteLine("; This is where the sections begins");
			Enumerator = Sections.GetEnumerator();
			while(Enumerator.MoveNext())
			{
				Hashtable section = (Hashtable)Enumerator.Value;
				//ICollection ic = ((System.Collections.Hashtable)Enumerator.Value).Keys;
				string[] keys = new string[section.Count];
				int counter = 0;
				foreach(string s in section.Keys)
				{
					keys[counter] = s;
					counter++;
				}

				Array.Sort(keys, 0, keys.Length, cic);

				//System.Collections.IDictionaryEnumerator Enumerator2nd = ((System.Collections.Hashtable)Enumerator.Value).GetEnumerator();
				sw.WriteLine("[{0}]",Enumerator.Key);

				//while(Enumerator2nd.MoveNext())
				foreach(string key in keys)
				{
					//sw.WriteLine("{0} = {1}",Enumerator2nd.Key,Enumerator2nd.Value);
					sw.WriteLine("{0} = {1}", key, (string)section[key]);
				}
			}
		}

		/// <summary>
		/// This private method Parse the input and sort it properly in the class
		/// for later retrival.
		/// </summary>
		/// <param name="sr">an already initialize StreamReader</param>
		private void Parse(System.IO.TextReader sr)
		{
			System.Collections.Hashtable CurrentSection=null;
			string Line,ValueName,Value;
			while (null != (Line = sr.ReadLine()))
			{
				
				int j,i=0;
				while((Line.Length>i) && (Char.IsWhiteSpace(Line[i]))) i++;//skip white space in beginning of line
				if(Line.Length<=i)
					continue;
				if(Line[i] == ';')//Comment
					continue;
				if(Line[i] == '[')//Start new Section
				{
					string SectionName;
					j = Line.IndexOf(']',i);
					if(j==-1)//last ']' not found
						throw new Exceptions.InvalidInputException();

					SectionName = Line.Substring(i+1,j-i-1).Trim();

					if(!Sections.ContainsKey(SectionName))
					{
						this.AddSection(SectionName);
					}
					CurrentSection = (System.Collections.Hashtable)Sections[SectionName];
					while(Line.Length>++j && Char.IsWhiteSpace(Line[i]));//skip white space in beginning of line
					if(Line.Length>j)
					{
						if (Line[j]!=';')//Anything but a comment is unacceptable after a section name
							throw new Exceptions.InvalidInputException();
					}
					continue;
				}
				//Start of a value name, ends with a '='
				j = Line.IndexOf('=',i);
				if(j==-1)
					throw new Exceptions.InvalidInputException();
                ValueName = Line.Substring(i,j-i).Trim();
				if((i = Line.IndexOf(';',j+1))!=-1)//remove comments from end of line
					Value = Line.Substring(j+1,i-(j+1)).Trim();
				else 
					Value = Line.Substring(j+1).Trim();
				if(CurrentSection != null)
				{
					CurrentSection[ValueName] = Value;
				}
				else
				{
					Values[ValueName] = Value;
				}
			}
		}

		public void RemoveValue(string Section, string ValueName)
		{
			System.Collections.Hashtable SectionTable = (System.Collections.Hashtable)Sections[Section];
			if(SectionTable!=null)
				SectionTable.Remove(ValueName);
			else
				throw new Exceptions.SectionDoesntExistException();
		}

		public void RemoveValue(string ValueName)
		{
			Values.Remove(ValueName);
		}

		public void RemoveKey(string Section)
		{
			System.Collections.Hashtable SectionTable = (System.Collections.Hashtable)Sections[Section];
			if(SectionTable!=null)
				Sections.Remove(Section);
			else
				throw new Exceptions.SectionDoesntExistException();
		}

		public bool GetBool(string section, string valuename)
		{
			string stored = GetValue(section, valuename);
			bool result = Convert.ToBoolean(stored);
			return result;
		}

		public int GetSectionCount(string section)
		{
			System.Collections.Hashtable sectionTable = (System.Collections.Hashtable)Sections[section];

			if(sectionTable != null)
			{
				return sectionTable.Count;
			}

			return 0;
		}

	}
}
