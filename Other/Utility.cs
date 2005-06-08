using System;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace ISquared
{
	/// <summary>
	/// Summary description for Utility.
	/// </summary>
	public class Utility
	{
		public Utility()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static String GetCurrentDir(bool closingSlash)
		{
			String dir =  Path.GetDirectoryName( 
				Assembly.GetExecutingAssembly().GetName().CodeBase );
		
			if(closingSlash)
			{
				dir += "\\";
			}

			return dir;		
		}

		public static Icon GetIcon(String name)
		{
			Assembly execAssembly = Assembly.GetExecutingAssembly();
			string fullName = execAssembly.GetName().Name + "." + name + ".ico";

			Stream stream  = execAssembly.GetManifestResourceStream(fullName);

			Debug.WriteLine("Loading icon: " + fullName);
			Icon icon = new Icon(stream);
			return icon;
		}

		public static string AssemblyVersion()
		{
			Assembly execAssembly = Assembly.GetExecutingAssembly();
			AssemblyName name = execAssembly.GetName();
			Version version = name.Version;
			return version.ToString();
		}
	}
}
