using System;
using System.Xml;
using System.Diagnostics;
//using System.Environment;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Reflection;
namespace ForestSoftware.NetCF.UserSettings
{
	public interface UserSettings
	{

		Int32 ReadSettingInt(string section, string entry, Int32 defaultVal);

		string ReadSettingString(string section, string entry, string defaultVal);

		bool WriteSettingString(string section, string entry, string dataValue);

		bool WriteSettingInt(string section, string entry, Int32 dataValue);

		string[] GetSectionNames();

		void DeleteKey(string SectionName, string KeyName);
	}
	public class SettingSupport
	{

		public static string CallingAppPath()
		{
			return System.IO.Path.GetDirectoryName(Assembly.GetCallingAssembly().GetName().CodeBase.ToString());
		}

		public static string CallingAppName()
		{
			return Assembly.GetCallingAssembly().GetName().Name;
		}

		public static string AppPath()
		{
			return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.ToString());
		}

		public static string AppName()
		{
			return Assembly.GetExecutingAssembly().GetName().Name;
		}
	}

	public class XmlSettings : UserSettings
	{
		private string m_sXMLFileName;

		private XmlSettings()
		{
		}

		public XmlSettings(string AppName)
		{
			m_sXMLFileName = SettingSupport.CallingAppPath() + "\\" + AppName + ".config";
		}

		private string ReadSettingValue(string section, string key, string defaultVal)
		{
			XmlTextReader dr = null;
			string strRetValue = defaultVal;
			try {
				dr = new XmlTextReader(m_sXMLFileName);
				dr.WhitespaceHandling = WhitespaceHandling.None;
				while (dr.Read()) {
					if (dr.Name == "configuration") {
						while (dr.Read()) {
							if (dr.NodeType == XmlNodeType.Element && dr.HasAttributes && dr.Name == "Section") {
								dr.MoveToFirstAttribute();
								if (dr.Name == "Name" & dr.Value == section) {
									while (dr.Read()) {
										if (dr.NodeType == XmlNodeType.Element && dr.Name.ToString() == "Key" && dr.HasAttributes) {
											dr.MoveToFirstAttribute();
											if (dr.Name == "Name" && dr.Value == key && dr.HasAttributes) {
												dr.MoveToNextAttribute();
												if (dr.Name == "Value") {
													return dr.Value;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return defaultVal;
			} finally {
				dr.Close();
			}
		}

		public Int32 ReadSettingInt(string section, string key, Int32 defaultVal)
		{
			return Convert.ToInt32(ReadSettingValue(section, key, defaultVal.ToString()));
		}

		public string ReadSettingString(string section, string key, string defaultVal)
		{
			return ReadSettingValue(section, key, defaultVal);
		}

		public bool WriteSettingValue(string section, string key, string dataValue)
		{
			bool result = true;
			try {
				XmlDocument xd = new XmlDocument();
				xd.Load(m_sXMLFileName);
				XmlElement xe = xd.DocumentElement;
				if (xe.Name == "configuration") {
					foreach (XmlElement xce in xe.ChildNodes) {
						if (xce.Name == "Section") {
							if (xce.Attributes[0].Name == "Name" & xce.Attributes[0].Value == section) {
								XmlElement xe2 = xd.CreateElement("Key");
								xe2.SetAttribute("Name", key);
								xe2.SetAttribute("Value", dataValue);
								xce.AppendChild(xe2);
								goto exitForStatement3;
							}
						}
					}
					exitForStatement3: ;
				}
				xd.Save(m_sXMLFileName);
			} catch (FileNotFoundException ex) {
				XmlDocument newDoc = new XmlDocument();
				XmlElement xe;
				xe = newDoc.CreateElement("configuration");
				XmlElement sectionElement = newDoc.CreateElement("Section");
				sectionElement.SetAttribute("Name", section);
				XmlElement keyElement = newDoc.CreateElement("Key");
				keyElement.SetAttribute("Name", section);
				keyElement.SetAttribute("Value", dataValue);
				sectionElement.AppendChild(keyElement);
				newDoc.AppendChild(sectionElement);
				newDoc.Save(m_sXMLFileName);
			} finally {
			}
			return result;
		}

		public bool WriteSettingString(string section, string key, string dataValue)
		{
			return WriteSettingValue(section, key, dataValue);
		}

		public bool WriteSettingInt(string section, string key, Int32 dataValue)
		{
			return WriteSettingValue(section, key, dataValue.ToString());
		}

		public string[] GetSectionNames()
		{
			return null;
		}

		public void DeleteKey(string SectionName, string KeyName)
		{
		}
	}
	
}
