using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;
using System.Runtime.InteropServices;

namespace HardwareButtons
{
	/// <summary>
	/// Summary description for HButtons.
	/// </summary>
	public enum KeyModifiers
	{
		None = 0,
		Alt = 1,
		Control = 2,
		Shift = 4,
		Windows = 8,
		Modkeyup = 0x1000,
	}

	public enum KeysHardware : int
	{
		Hardware1 = 193,
		Hardware2 = 194,
		Hardware3 = 195,
		Hardware4 = 196,
		Hardware5 = 197
	}

	public enum RegisterButtons
	{
		None	  = 0,
		Hardware1 = 1,
		Hardware2 = 2,
		Hardware3 = 4,
		Hardware4 = 8,
		Hardware5 = 16,
	}
	/*
	public class HButtons : System.Windows.Forms.Form
	{
		myMessageWindow messageWindow;
		public HButtons()
		{
			this.messageWindow = new myMessageWindow(this); RegisterHKeys.RegisterRecordKey(this.messageWindow.Hwnd);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		public static void Main()
		{
			Application.Run(new HButtons());
		}
		public void ButtonPressed(int button)
		{
			switch (button)
			{
				case (int)KeysHardware.Hardware1:
					MessageBox.Show("Button 1 pressed!");
					break;
				case (int)KeysHardware.Hardware2:
					MessageBox.Show("Button 2 pressed!");
					break;
				case (int)KeysHardware.Hardware3:
					MessageBox.Show("Button 3 pressed!");
					break;
				case (int)KeysHardware.Hardware4:
					MessageBox.Show("Button 4 pressed!");
					break;
			}

		}

	}
	*/

	public delegate void HardwareButtonPressedHandler(int button);

	public class HardwareButtonMessageWindow : MessageWindow
	{
		public event HardwareButtonPressedHandler HardwareButtonPressed;


		public const int WM_HOTKEY = 0x0312;
		//HButtons example;
		public HardwareButtonMessageWindow()//HButtons example)
		{
			//this.example = example;
		}

		protected override void WndProc(ref Message msg)
		{
			switch (msg.Msg)
			{
				case WM_HOTKEY:
					if(HardwareButtonPressed != null)
					{
						HardwareButtonPressed(msg.WParam.ToInt32());
					}
					//example.ButtonPressed(msg.WParam.ToInt32());
					return;
			}
			base.WndProc(ref msg);
		}
	}
	public class RegisterHKeys
	{
		[DllImport("coredll.dll", SetLastError = true)]
		public static extern bool RegisterHotKey(
		IntPtr hWnd, // handle to window
		int id, // hot key identifier
		KeyModifiers Modifiers, // key-modifier options
		int key //virtual-key code
		);

		[DllImport("coredll.dll", SetLastError = true)]
		private static extern bool UnregisterHotKey(
			IntPtr hWnd, // handle to window 
			int id // hot key identifier 			
			);

		private static RegisterButtons m_registeredButtons = RegisterButtons.None;

		public static RegisterButtons RegisteredButtons
		{
			get { return m_registeredButtons; }
			set { m_registeredButtons = value; }
		}


		[DllImport("coredll.dll")]
		private static extern bool UnregisterFunc1(KeyModifiers
		modifiers, int keyID);

		public static void UnregisterRecordKey(IntPtr hWnd, RegisterButtons buttons)
		{
			if ((buttons & RegisterButtons.Hardware1) == RegisterButtons.Hardware1)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware1);
				UnregisterHotKey(hWnd, (int)KeysHardware.Hardware1);
			}

			if ((buttons & RegisterButtons.Hardware2) == RegisterButtons.Hardware2)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware2);
				UnregisterHotKey(hWnd, (int)KeysHardware.Hardware2);
			}

			if ((buttons & RegisterButtons.Hardware3) == RegisterButtons.Hardware3)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware3);
				UnregisterHotKey(hWnd, (int)KeysHardware.Hardware3);
			}

			if ((buttons & RegisterButtons.Hardware4) == RegisterButtons.Hardware4)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware4);
				UnregisterHotKey(hWnd, (int)KeysHardware.Hardware4);
			}

			if ((buttons & RegisterButtons.Hardware5) == RegisterButtons.Hardware5)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware5);
				UnregisterHotKey(hWnd, (int)KeysHardware.Hardware5);
			}

			m_registeredButtons &= ~buttons;
			
		}

		public static void RegisterRecordKey(IntPtr hWnd, RegisterButtons buttons)
		{
			if( (buttons & RegisterButtons.Hardware1) == RegisterButtons.Hardware1)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware1);
				RegisterHotKey(hWnd, (int)KeysHardware.Hardware1, KeyModifiers.Windows, (int)KeysHardware.Hardware1);
			}

			if ((buttons & RegisterButtons.Hardware2) == RegisterButtons.Hardware2)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware2);
				RegisterHotKey(hWnd, (int)KeysHardware.Hardware2, KeyModifiers.Windows, (int)KeysHardware.Hardware2);
			}

			if ((buttons & RegisterButtons.Hardware3) == RegisterButtons.Hardware3)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware3);
				RegisterHotKey(hWnd, (int)KeysHardware.Hardware3, KeyModifiers.Windows, (int)KeysHardware.Hardware3);
			}

			if ((buttons & RegisterButtons.Hardware4) == RegisterButtons.Hardware4)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware4);
				RegisterHotKey(hWnd, (int)KeysHardware.Hardware4, KeyModifiers.Windows, (int)KeysHardware.Hardware4);
			}

			if ((buttons & RegisterButtons.Hardware5) == RegisterButtons.Hardware5)
			{
				UnregisterFunc1(KeyModifiers.Windows, (int)KeysHardware.Hardware5);
				RegisterHotKey(hWnd, (int)KeysHardware.Hardware5, KeyModifiers.Windows, (int)KeysHardware.Hardware5);
			}

			RegisteredButtons = buttons;
		}


	}
}