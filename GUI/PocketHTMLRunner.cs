#region Using directives

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

#endregion

namespace ISquared.PocketHTML
{
	/// <summary>
	/// Summary description for PocketHTMLRunner.
	/// </summary>
	sealed public class PocketHTMLRunner
	{
		private PocketHTMLRunner()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		internal static void LaunchPocketHTML(string[] args)
		{
			PocketHTMLEditor mainForm = null;

			try
			{
				mainForm = new PocketHTMLEditor(args);
				Application.Run(mainForm);
				Cursor.Current = Cursors.Default;
			}
			catch(System.Exception exception)
			{
				Cursor.Current = Cursors.Default;
				//Name.Forms.ErrorForm errorForm = new Name.Forms.ErrorForm(exception);
				//errorForm.ShowDialog();
				//errorForm = null;

				string errorLog = Path.Combine(Path.GetTempPath(), "phnerror.txt");
				


				MessageBox.Show(exception.Message);
				

				StringBuilder sb = new StringBuilder();
				sb.Append("Uncaught exception at top level");
				sb.Append("\r\n");
				sb.Append("Exception info: ");
				sb.Append(exception.ToString());
				sb.Append("\r\n");

				if(exception.InnerException != null)
				{
					sb.Append("Inner exception: ");
					sb.Append(exception.InnerException.Message);
				}
				
				string errorOutput = sb.ToString();

				

				StreamWriter sw = new StreamWriter(errorLog, false);
				sw.Write(errorOutput);
				//sw.WriteLine();
				//sw.WriteLine(" + exception.Message);
				//sw.WriteLine("Inner exception: " + exception.InnerException.Message);
				sw.Close();

				MessageBox.Show(errorOutput);

				Debug.WriteLine(errorOutput);

				string errorMessage = string.Format("PocketHTML has had an unexpected problem.  Please open {0} for further details", errorLog);
				MessageBox.Show(errorMessage);

				mainForm.Close();
			}
			finally
			{
				mainForm = null;
				Application.DoEvents();
				Application.Exit();
			}
		}
	}
}
