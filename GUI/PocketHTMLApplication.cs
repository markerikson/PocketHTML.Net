#region Using directives

using System;
using System.Windows.Forms;

#endregion

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for PocketHTMLApplication.
	/// </summary>
	sealed public class PocketHTMLApplication
	{
		private PocketHTMLApplication()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void Main()
		{
			try
			{
				PocketHTMLRunner.LaunchPocketHTML();
				Cursor.Current = Cursors.Default;
			}
			catch(System.TypeLoadException)
			{
				Cursor.Current = Cursors.Default;
				// Code
			}
			catch(System.MissingMethodException)
			{
				Cursor.Current = Cursors.Default;
				// Code
			}
			catch(System.Exception)
			{
				Cursor.Current = Cursors.Default;
				// Code
			}
		}
	}
}
