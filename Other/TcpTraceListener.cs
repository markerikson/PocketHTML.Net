#region using directives
using System;
using System.Diagnostics;
using System.Text;
using System.Net.Sockets;
using System.Reflection;
using System.Xml;
using System.Collections;
#endregion

namespace ISquared.Debugging
{
	/// <summary>
	/// TcpTraceListener is a specialized TraceListener that sends the output
	/// of <c>Debug.Write</c> or <c>Debug.WriteLine</c> to a server using
	/// TCP/IP
	/// </summary>
	public class TcpTraceListener : TraceListener, IDisposable
	{
		#region members
		/// <summary>
		/// The message a TcpTraceListener sends when it is no longer needed
		/// </summary>
		public static string end = "*END_DEBUG*";

		private string server = String.Empty;
		private int serverPort = 0;
		private TcpClient tcpClient = null;
		private NetworkStream stream = null;
		#endregion

		#region Constructors / setup
		[Conditional("DEBUG")]
		/// <summary>
		/// Call this method to hook to the Debug infrastructure a 
		/// TcpTraceListener that sends its output to the specified server
		/// and port
		/// </summary>
		/// <param name="server">The IP address or name of the server</param>
		/// <param name="port">The port number the server is listening to</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		/// <exception cref="System.Net.Sockets.SocketException"></exception>
		/// <exception cref="System.ObjectDisposedException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		public static void InstallListener(string server, int port)
		{
			Debug.Listeners.Add(new TcpTraceListener(server, port));
		}

		[Conditional("DEBUG")]
		/// <summary>
		/// Call this method to hook to the Debug infrastructure a 
		/// TcpTraceListener that sends its output to the server and port
		/// specified in the executing assembly's configuration file
		/// </summary>
		public static void InstallListener()
		{
			String server = String.Empty;
			int port = 0;
			StringBuilder strBuild = new StringBuilder();
			String configFile = String.Format("{0}.config",
				Assembly.GetCallingAssembly().GetName().CodeBase);
			ReadParameters(configFile, out server, out port);
			Debug.Listeners.Add(new TcpTraceListener(server, port));
		}
		/// <summary>
		/// Constructor that extracts the server address and port from a string
		/// </summary>
		/// <param name="constructionString">
		/// Specifies the server address and port according to the following
		/// pattern: 123.123.123.123;123
		/// </param>
		protected TcpTraceListener(string constructionString)
		{
			string sep = ";";
			char[] delim = sep.ToCharArray();
			string[] args = constructionString.Split(delim);
			if (args.Length != 2)
				throw new ArgumentException("The server address and port number must be specified");
			
			server = args[0];
			serverPort = Int32.Parse(args[1]);
			ConnectToServer();
		}
        
		/// <summary>
		/// Constructor that takes a server IP address and port
		/// </summary>
		/// <param name="server">The server address or name</param>
		/// <param name="port">The server port number</param>
		public TcpTraceListener(string server, int port)
		{
			this.server = server;
			serverPort = port;
			ConnectToServer();
		}

		/// <summary>
		/// Attempts a connection to the server and obtains a stream to
		/// communicate with it
		/// </summary>
		private void ConnectToServer()
		{
			tcpClient = new TcpClient(server, serverPort);
			stream = tcpClient.GetStream();
		}
		#endregion

		#region Writing functions
		public override void Write(string message)
		{
			SendString(message);
		}
		
		public override void WriteLine(string message)
		{
			string msg = String.Format("{0}\n", message);
			SendString(msg);
		}


		/// <summary>
		/// Sends the argument to the server (Unicode encoded)
		/// </summary>
		/// <param name="msg">The message to send</param>
		private void SendString(string msg)
		{
			if (stream != null)
			{
				try
				{
					Byte[] data = System.Text.Encoding.Unicode.GetBytes(msg);
					stream.Write(data, 0, data.Length);
					stream.Flush();
				}
				catch (Exception)
				{
				}
			}
		}
		#endregion

		#region Reading functions

		/// <summary>
		/// Reads the server name and port from the specified configuration file
		/// </summary>
		/// <param name="fileName">The name of the app configuration file to parse</param>
		private static void ReadParameters(string fileName, out string server, out int port)
		{
			server = String.Empty;
			port = 0;
			XmlTextReader rd = null;
			Hashtable tb = new Hashtable(2);
			try
			{
				rd = new XmlTextReader(fileName);
				rd.WhitespaceHandling = WhitespaceHandling.None;
				// Read document node
				rd.ReadStartElement("configuration");
				// Read appSettings node
				rd.ReadStartElement("appSettings");
				// Read first add element
				if (rd.Name.Equals("add"))
				{
					ReadAddAttributes(rd, tb);
				}
				// Read second add element
				rd.ReadStartElement("add");
				if (rd.Name.Equals("add"))
				{
					ReadAddAttributes(rd, tb);
				}

				server = (String)tb["TcpTraceServer"];
				port = Int32.Parse((String)tb["TcpTracePort"]);
			}
			catch (XmlException xmlEx)
			{
				Console.WriteLine(xmlEx.Message);
			}
			finally
			{
				if (rd != null)
					rd.Close();
			}
		}

		/// <summary>
		/// Extracts the "key" and "value" attributes from the current element in the reader
		/// </summary>
		/// <param name="rd">An XmlReader positioned in an "add" element</param>
		/// <param name="tb">A hashtable to put the values associated to the "key" and "value" attributes</param>
		/// <returns><c>true</c> if the attributes are correctly read, <c>false</c> otherwise</returns>
		private static bool ReadAddAttributes(XmlReader rd, Hashtable tb)
		{
			String keyAtt = null;
			String valueAtt = null;
			bool retVal = false;

			keyAtt = rd.GetAttribute("key");
			valueAtt = rd.GetAttribute("value");
			if (keyAtt != null && valueAtt != null)
			{
				tb.Add(keyAtt, valueAtt);
				retVal = true;
			}

			return retVal;
		}
		#endregion

		#region Other functions
		public override void Close()
		{
			if (stream != null)
			{
				try
				{
					SendString(end);
					stream.Flush();
					stream.Close();
					stream = null;
				}
				catch
				{
				}
			}
			if (tcpClient != null)
			{
				try
				{
					tcpClient.Close();
					tcpClient = null;
				}
				catch(Exception)
				{
				}
			}
		}
	
		public void Dispose()
		{
			Close();
		}
		#endregion

	}
}
