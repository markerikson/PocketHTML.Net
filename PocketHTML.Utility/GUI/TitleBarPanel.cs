#region Using directives

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

#endregion

namespace ISquared.Windows.Forms
{
	public class TitleBarPanel : System.Windows.Forms.Panel
	{
		#region members
		internal bool mouseDown = false;
		internal Point oldPoint = Point.Empty;
		private Point startingPoint;

		private Label m_lblCaption;
		private FloatingDialog m_dlgParent;

		private IntPtr hPen, hOldPen;
		private IntPtr hDC, hwndForm;
		private Rectangle rcDraw;
		private bool bDrawing;
		#endregion

		#region properties
		public string Caption
		{
			get
			{
				return m_lblCaption.Text;
			}
			set
			{
				m_lblCaption.Text = value;
			}
		}

		public FloatingDialog ParentDialog
		{
			get
			{
				return m_dlgParent;
			}
			set
			{
				m_dlgParent = value;
			}
		}

		/// <summary>
		/// Property OldPoint (Point)
		/// </summary>
		public Point OldPoint
		{
			get
			{
				return this.oldPoint;
			}
			set
			{
				this.oldPoint = value;
			}
		}
		#endregion

		#region Constructor
		public TitleBarPanel()
		{
			m_lblCaption = new Label();
			m_lblCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			m_lblCaption.ForeColor = System.Drawing.Color.White;
			m_lblCaption.Bounds = new Rectangle(4, 0, 100, 16);
			m_lblCaption.Text = "Caption";

			this.BackColor = SystemColors.ActiveCaption;

			m_lblCaption.Parent = this;
		}
		#endregion

		#region Event handlers
		override protected void OnMouseDown(MouseEventArgs e)
		{
			mouseDown = true;
			oldPoint = new Point(e.X, e.Y);
			startingPoint = new Point(e.X, e.Y);

			// Direct all mouse events to canvas
			ParentDialog.ParentForm.Capture = true;
			hwndForm = GetCapture();

			this.Capture = true;

			// Create "rubber-band" pen
			hPen = CreatePen(PenStyle.PS_DASH, 1, ColorToWin32(Color.Blue));

			// Get Device Context
			hDC = GetDC(IntPtr.Zero);

			// Select our pen into context, save the old pen
			hOldPen = SelectObject(hDC, hPen);

			// Set current drawing mode to XOR, so that drawing again will restore the image
			SetROP2(hDC, ROP.R2_XORPEN);

			Point currentLocation = new Point(ParentDialog.Left, ParentDialog.Top);

			Point finalLocation = new Point();

			// Handle differences between Show() / ShowDialog()
			if(ParentDialog.Parent != null)
			{
				finalLocation = ParentDialog.ParentForm.PointToScreen(currentLocation);
			}
			else
			{
				finalLocation = currentLocation;
				
			}

			// stretch it to the current mouse position
			rcDraw = new Rectangle(finalLocation.X, finalLocation.Y, ParentDialog.Width, ParentDialog.Height);

			// Draw rectangle
			DrawRectangle(hDC, rcDraw);
			bDrawing = true;
		}

		override protected void OnMouseMove(MouseEventArgs e)
		{
			if (bDrawing)
			{
				int dx, dy;
				dx = e.X - oldPoint.X;
				dy = e.Y - oldPoint.Y;

				// Erase rectangle - restore the original graphics
				DrawRectangle(hDC, rcDraw);

				if (ParentDialog.ParentForm != null)
				{
					ParentDialog.ParentForm.Refresh();
				}

				// Update rectangle location
				rcDraw.X += dx;
				rcDraw.Y += dy;

				oldPoint.X = e.X;
				oldPoint.Y = e.Y;

				// Draw it again
				DrawRectangle(hDC, rcDraw);
			}
		}

		override protected void OnMouseUp(MouseEventArgs e)
		{
			if (bDrawing)
			{
				int dx, dy;
				dx = e.X - startingPoint.X;
				dy = e.Y - startingPoint.Y;

				// Erase rectangle
				DrawRectangle(hDC, rcDraw);

				// Restore everything
				this.Capture = false;
				bDrawing = false;
				DeleteObject(SelectObject(hDC, hOldPen));
				ReleaseDC(hwndForm, hDC);

				Point currentLocation = ParentDialog.Location;
				currentLocation.X += dx;
				currentLocation.Y += dy;
				Point revisedLocation = ParentDialog.ParentForm.PointToScreen(currentLocation);

				ParentDialog.Location = currentLocation;
			}
		}
		#endregion

		#region Utility functions
		private int ColorToWin32(Color clr)
		{
			return clr.B << 16 + clr.G << 8 + clr.R;
		}

		private void DrawRectangle(IntPtr hDC, Rectangle r)
		{
			int[] points = new int[10];

			points[0] = r.Left;
			points[1] = r.Top;
			points[2] = r.Right;
			points[3] = r.Top;
			points[4] = r.Right;
			points[5] = r.Bottom;
			points[6] = r.Left;
			points[7] = r.Bottom;
			points[8] = r.Left;
			points[9] = r.Top;

			Polyline(hDC, points, 5);
		}
		#endregion

		#region P/Invoke Definitions
		public enum PenStyle
		{
			PS_SOLID = 0,
			PS_DASH = 1,
			PS_NULL = 5
		}

		public enum ROP
		{
			R2_BLACK = 1,   /*  0       */
			R2_NOTMERGEPEN = 2,   /* DPon     */
			R2_MASKNOTPEN = 3,   /* DPna     */
			R2_NOTCOPYPEN = 4,   /* PN       */
			R2_MASKPENNOT = 5,   /* PDna     */
			R2_NOT = 6,   /* Dn       */
			R2_XORPEN = 7,   /* DPx      */
			R2_NOTMASKPEN = 8,   /* DPan     */
			R2_MASKPEN = 9,   /* DPa      */
			R2_NOTXORPEN = 10,  /* DPxn     */
			R2_NOP = 11,  /* D        */
			R2_MERGENOTPEN = 12,  /* DPno     */
			R2_COPYPEN = 13,  /* P        */
			R2_MERGEPENNOT = 14,  /* PDno     */
			R2_MERGEPEN = 15,  /* DPo      */
			R2_WHITE = 16,  /*  1       */
			R2_LAST = 16,
		}

		[DllImport("coredll")]
		public static extern IntPtr GetCapture();

		[DllImport("coredll")]
		public static extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("coredll")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("coredll")]
		public static extern IntPtr CreatePen(PenStyle fnPenStyle, int nWidth, int crColor);

		[DllImport("coredll")]
		public static extern IntPtr SetROP2(IntPtr hDC, ROP rop);

		[DllImport("coredll")]
		public static extern bool Polyline(IntPtr hdc, int[] Points, int cPoints);

		[DllImport("coredll")]
		public static extern bool Polygon(IntPtr hdc, int[] Points, int cPoints);

		[DllImport("coredll")]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport("coredll")]
		public static extern IntPtr DeleteObject(IntPtr hObject);

		#endregion
	}
}
