using System;
using System.Reflection;
using System.IO;
using System.Drawing;

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
			Icon icon = new Icon(stream);
			return icon;
		}
	}
}
