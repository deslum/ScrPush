using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Hook
	{
		internal class Hook
		{
			private const int WH_KEYBOARD_LL = 13;
			private LowLevelKeyboardProcDelegate m_callback;
			private IntPtr m_hHook;


			[DllImport("user32.dll", SetLastError = true)]
			private static extern IntPtr SetWindowsHookEx(
				int idHook,
				LowLevelKeyboardProcDelegate lpfn,
				IntPtr hMod, int dwThreadId);


			[DllImport("user32.dll", SetLastError = true)]
			private static extern bool UnhookWindowsHookEx(IntPtr hhk);


			[DllImport("Kernel32.dll", SetLastError = true)]
			private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);


			[DllImport("user32.dll", SetLastError = true)]
			private static extern IntPtr CallNextHookEx(
				IntPtr hhk,
				int nCode, IntPtr wParam, IntPtr lParam);


			private IntPtr LowLevelKeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
			{
			if (nCode < 0)
			{
				return CallNextHookEx(m_hHook, nCode, wParam, lParam);
			}
			else
			{
					var khs = (KeyboardHookStruct)Marshal.PtrToStructure (lParam,typeof(KeyboardHookStruct));
					if (khs.VirtualKeyCode == 44 &&
				    wParam.ToInt32() == 257 &&
				    khs.ScanCode == 55)
					{
                        ScrPush.App.ScrPrint();
						IntPtr val= new IntPtr(1);
						return val;                    
					}
					else
					{
						return CallNextHookEx(m_hHook, nCode, wParam, lParam);
					}
				}
			}


			[StructLayout(LayoutKind.Sequential)]
			private struct KeyboardHookStruct
			{
				public readonly int VirtualKeyCode;
				public readonly int ScanCode;
				public readonly int Flags;
				public readonly int Time;
				public readonly IntPtr ExtraInfo;
			}


			private delegate IntPtr LowLevelKeyboardProcDelegate(
				int nCode, IntPtr wParam, IntPtr lParam);


			public void SetHook()
			{
				m_callback =LowLevelKeyboardHookProc;
				m_hHook =SetWindowsHookEx(WH_KEYBOARD_LL,
				                          m_callback,
				                          GetModuleHandle(IntPtr.Zero),0);
			}


			public void Unhook()
			{
				UnhookWindowsHookEx(m_hHook);
			}		
		}
}


