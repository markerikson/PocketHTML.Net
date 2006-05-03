#region using directives
using System;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Text;
#endregion

namespace ISquared
{
	public class Utility
	{
		#region General utility functions
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

		public static string AssemblyVersion()
		{
			Assembly execAssembly = Assembly.GetExecutingAssembly();
			AssemblyName name = execAssembly.GetName();
			Version version = name.Version;
			return version.ToString();
		}
		#endregion
	}
}
