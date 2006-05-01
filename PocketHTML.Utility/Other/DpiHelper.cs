#region Using directives

using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

#endregion

namespace ISquared
{
	/// <summary>
	/// Summary description for DpiHelper.
	/// </summary>
	public class DpiHelper
	{
		/// <summary>The real dpi of the device.</summary>
		private static int dpi = SafeNativeMethods.GetDeviceCaps(IntPtr.Zero, /*LOGPIXELSX*/88);

		/// <summary>Adjust the sizes of controls to account for the DPI of the device.</summary>
		/// <param name="parent">The parent node of the tree of controls to adjust.</param>
		public static void AdjustAllControls(Control parent)
		{
			if (dpi == 96)
				return;
			foreach (Control child in parent.Controls)
			{
				AdjustControl(child);
				AdjustAllControls(child);
			}
		}

		public static void AdjustControl(Control control)
		{
			control.Bounds = new Rectangle(
				  control.Left * dpi / 96,
				  control.Top * dpi / 96,
				  control.Width * dpi / 96,
				  control.Height * dpi / 96);
		}

		/// <summary>Scale a coordinate to account for the dpi.</summary>
		/// <param name="x">The number of pixels at 96dpi.</param>
		public static int Scale(int x)
		{
			return x * dpi / 96;
		}

		private class SafeNativeMethods
		{
			[DllImport("coredll.dll")]
			static internal extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		}
	}
}
