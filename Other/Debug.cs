/************************************************
 * 
 * @author Mike Borromeo
 * @version 1.0.0
 * 
 ************************************************/ 

// usings
using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

// Phosl
namespace Debugging
{
	/// <summary>
	/// The debugging class. Contains declarations for a 'log' and an 'error' delegate that can be 
	/// implemented and when the log and error methods are called from inside debugging the delegates are alerted.
	/// </summary>
	public class Debug
	{
		/// <summary>
		/// The top level namespaces that can be singled out for debugging.
		/// </summary>
		protected static Hashtable namespaces;

		/// <summary>
		/// The error handling delegate.
		/// </summary>
		public delegate void ErrorHandler( string msg, string file, int lineNumber, string method );

		/// <summary>
		/// The log handling delegate.
		/// </summary>
		public delegate void LogHandler( string msg, string file, int lineNumber, string method );

		/// <summary>
		/// The log event.
		/// </summary>
		public static event LogHandler OnLog;

		/// <summary>
		/// The error event.
		/// </summary>
		public static event ErrorHandler OnError;


		/// <summary>
		/// The static constructor.
		/// </summary>
		static Debug()
		{
			// create the namespaces hashtable
			namespaces = new Hashtable();

			// get the assembly of this class
			Assembly a = Assembly.GetAssembly( new Debug().GetType() );

			// now cycle through each type and gather up all the namespaces
			foreach( Type type in a.GetTypes() )
			{
				// check if the namespace is already in our table and by default theyre all turned on
				if( ! namespaces.Contains( type.Namespace ) )
					namespaces.Add( type.Namespace, true );
			}
		}

		/// <summary>
		/// Saftey dance.
		/// </summary>
		protected Debug()
		{
			// just so no one goes creatin an instance of debug
		}

		/// <summary>
		/// Set debugging to on or off for a specific namespace.
		/// </summary>
		/// <param name="ns"></param>
		/// <param name="b"></param>
		[Conditional("DEBUG")]
		public static void DebugNamespace( string ns, bool b )
		{
			// check if the namespace exists
			if( namespaces.Contains( ns ) )
				namespaces[ ns ] = b;
		}

		/// <summary>
		/// The namespaces and their debugging status hashtable.
		/// </summary>
		public static Hashtable Namespaces
		{
			get
			{
				// return the namespaces
				return namespaces;
			}
		}

		/// <summary>
		/// The log function that triggers all the registered delegates.
		/// </summary>
		/// <param name="msg"></param>
		[Conditional("DEBUG")]
		public static void Log( string msg )
		{
			// check if theres any registered log handlers
			if( null != OnLog )
			{
				// create the stack frame for the function that called this function
				StackFrame sf = new StackFrame( 1, true );
				StackFrame sf1 = new StackFrame();

				// only proceed if the namespace in question is being debugged
				if( (bool) namespaces[ sf.GetMethod().DeclaringType.Namespace ] )
					OnLog( msg, sf.GetFileName(), sf.GetFileLineNumber(), sf.GetMethod().ToString() );
			}
		}

		/// <summary>
		/// The error function that triggers all the registered delegates.
		/// </summary>
		/// <param name="msg"></param>
		[Conditional("DEBUG")]
		public static void Error( string msg )
		{
			// check if theres any registered error handlers
			if( null != OnError )
			{
				// create the stack frame for the function that called this function
				StackFrame sf = new StackFrame( 1, true );

				// trigger an error event
				if( (bool) namespaces[ sf.GetMethod().DeclaringType.Namespace ] )
					OnError( msg, sf.GetFileName(), sf.GetFileLineNumber(), sf.GetMethod().ToString() );
			}
		}
	}
}
