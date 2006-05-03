#region using directives
using System;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using ISquared.Win32Interop;
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

		public static Icon GetIcon(String name, int size)
		{
			Assembly execAssembly = Assembly.GetCallingAssembly();
			string fullName = execAssembly.GetName().Name + "." + name + ".ico";

			Stream stream = execAssembly.GetManifestResourceStream(fullName);

			Debug.WriteLine("Loading icon: " + fullName);
			Icon icon = new Icon(stream, size, size);
			return icon;
		}

		public static IntPtr GetHandle(Control c)
		{
			c.Capture = true;
			IntPtr hWnd = CoreDLL.GetCapture();
			c.Capture = false;
			return hWnd;
		}

		public static Control GetFocusedControl(Control parent)
		{
			if (parent.Focused)
			{
				return parent;
			}
			else
			{
				foreach (Control c in parent.Controls)
				{
					if (c.Focused)
					{
						return c;
					}
				}

				return null;
			}
		}

		#endregion

	}
}
