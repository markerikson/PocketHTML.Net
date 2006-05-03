#region using directives
using System;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Text;
#endregion

namespace ISquared.PocketHTML
{
	public class PocketHTMLUtility
	{
		#region Utility functions
		public static Icon GetIcon(String name, int size)
		{
			Assembly execAssembly = Assembly.GetExecutingAssembly();
			string fullName = execAssembly.GetName().Name + "." + name + ".ico";

			Stream stream  = execAssembly.GetManifestResourceStream(fullName);

			Debug.WriteLine("Loading icon: " + fullName);
			Icon icon = new Icon(stream, size, size);
			return icon;
		}

		public static string AssemblyVersion()
		{
			Assembly execAssembly = Assembly.GetExecutingAssembly();
			AssemblyName name = execAssembly.GetName();
			Version version = name.Version;
			return version.ToString();
		}

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
		#endregion
	}
}
