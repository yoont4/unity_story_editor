/* SCRIPT INSPECTOR 3
 * version 3.0.17, December 2016
 * Copyright © 2012-2016, Flipbook Games
 * 
 * Unity's legendary editor for C#, UnityScript, Boo, Shaders, and text,
 * now transformed into an advanced C# IDE!!!
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join discussion in Unity forums http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */


namespace ScriptInspector
{	
	using UnityEngine;
	using UnityEditor;
	
	using System;
	using System.Runtime.InteropServices;
	
	
	[InitializeOnLoad]
	internal static class FGKeyboardHook
	{
#if !UNITY_EDITOR_OSX
		const int WH_KEYBOARD = 2;
		const int WH_MOUSE = 7;
		
		const ushort XBUTTON1 = 0x0001;
		const ushort XBUTTON2 = 0x0002;
		
		const int VK_SHIFT = 0x10;
		const int VK_CONTROL = 0x11;
		
		const int WM_XBUTTONDOWN = 0x020B;
		const int WM_XBUTTONDBLCLK = 0x020D;
		const int WM_NCXBUTTONDOWN = 0x00AB;
		const int WM_NCXBUTTONDBLCLK = 0x00AD;
		
		static readonly KeyboardProc _keyboardProc = KeyboardHookCallback;
		static IntPtr _keyboardHookID = IntPtr.Zero;
		
		static readonly MouseProc _mouseProc = MouseHookCallback;
		static IntPtr _mouseHookID = IntPtr.Zero;
		
		delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
		delegate IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr SetWindowsHookEx(int idHook, MouseProc lpfn, IntPtr hMod, uint dwThreadId);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool UnhookWindowsHookEx(IntPtr hhk);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
		static extern short GetKeyState(int keyCode);
		
		[StructLayout(LayoutKind.Sequential)]
		public class Point
		{
			public int x;
			public int y;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct MOUSEHOOKSTRUCT
		{	
			public Point pt;
			public IntPtr hwnd;
			public uint wHitTestCode;
			public IntPtr dwExtraInfo;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct MouseHookStructEx
		{
			public MOUSEHOOKSTRUCT mouseHookStruct;
			public int MouseData;
		}
		
		static FGKeyboardHook()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				EditorApplication.update += SetHookOnFirstUpdate;
				EditorApplication.update += OnUpdate;
				AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
			}
		}
		
		static void SetHookOnFirstUpdate()
		{
			EditorApplication.update -= SetHookOnFirstUpdate;
			_keyboardHookID = SetHook(_keyboardProc);
			_mouseHookID = SetHook(_mouseProc);
		}
		
		static IntPtr SetHook(KeyboardProc proc)
		{
#pragma warning disable 618
			return SetWindowsHookEx(WH_KEYBOARD, proc, IntPtr.Zero, (uint) AppDomain.GetCurrentThreadId());
#pragma warning restore 618
		}
		
		static IntPtr SetHook(MouseProc proc)
		{
#pragma warning disable 618
			return SetWindowsHookEx(WH_MOUSE, proc, IntPtr.Zero, (uint) AppDomain.GetCurrentThreadId());
#pragma warning restore 618
		}
		
		static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			//Marshall the data from the callback.
			MouseHookStructEx info = (MouseHookStructEx) Marshal.PtrToStructure(lParam, typeof(MouseHookStructEx));
			
			if (nCode >= 0)
			{
				var msg = wParam.ToInt32();
				
				var wnd = EditorWindow.focusedWindow;
				if (wnd != null &&
				(FGTextBuffer.activeEditor != null &&
					FGTextBuffer.activeEditor.hasCodeViewFocus &&
					wnd == FGTextBuffer.activeEditor.OwnerWindow
					|| (wnd is FGConsole || wnd is FindResultsWindow)))
				{
					switch (msg)
					{
					case WM_XBUTTONDOWN:
					case WM_XBUTTONDBLCLK:
					case WM_NCXBUTTONDOWN:
					case WM_NCXBUTTONDBLCLK:
						var xButton = info.MouseData >> 16;
						if (xButton == XBUTTON1)
						{
							// 4th Mouse Button (Back) -> (Ctrl+)Alt+Left
							sendToWindow = wnd;
							delayedKeyEvent = Event.KeyboardEvent(SISettings.wordBreak_UseBothModifiers ? "&^left" : "&left");
							return (IntPtr) 1;
						}
						else if (xButton == XBUTTON2)
						{
							// 5th Mouse Button (Forward) -> (Ctrl+)Alt+Right
							sendToWindow = wnd;
							delayedKeyEvent = Event.KeyboardEvent(SISettings.wordBreak_UseBothModifiers ? "&^right" : "&right");
							return (IntPtr) 1;
						}
						break;
					}
				}
			}
			return CallNextHookEx(_mouseHookID, nCode, wParam, lParam); 
		}
		
		static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0)
			{
				if ((lParam.ToInt32() & 0xA0000000) == 0)
				{
					var vkCode = wParam.ToInt32();
					if (vkCode == 'F' && (GetKeyState(VK_CONTROL) & 0x8000) != 0 && (GetKeyState(VK_SHIFT) & 0x8000) != 0)
					{
						EditorApplication.delayCall += FindReplaceWindow.ShowFindInFilesWindow;
						return (IntPtr) 1;
					}
					
					var wnd = EditorWindow.focusedWindow;
					if (wnd != null &&
					(FGTextBuffer.activeEditor != null &&
						FGTextBuffer.activeEditor.hasCodeViewFocus &&
						wnd == FGTextBuffer.activeEditor.OwnerWindow
						|| vkCode == '\t' && (wnd is FGConsole || wnd is TabSwitcher || wnd is FindResultsWindow)))
					{
						if ((GetKeyState(VK_CONTROL) & 0x8000) != 0)
						{
							if ((GetKeyState(VK_SHIFT) & 0x8000) == 0)
							{
								if (vkCode == 'S')
								{
									// Ctrl+S
									sendToWindow = wnd;
									delayedKeyEvent = Event.KeyboardEvent("^&s");
									return (IntPtr) 1;
								}
								if (vkCode == 'Z')
								{
									// Ctrl+Z
									sendToWindow = wnd;
									delayedKeyEvent = Event.KeyboardEvent("#^z");
									return (IntPtr) 1;
								}
								if (vkCode == 'Y')
								{
									// Ctrl+Y
									sendToWindow = wnd;
									delayedKeyEvent = Event.KeyboardEvent("#^y");
									return (IntPtr) 1;
								}
								if (vkCode == 'R')
								{
									// Ctrl+R
									sendToWindow = wnd;
									delayedKeyEvent = Event.KeyboardEvent("#^r");
									return (IntPtr) 1;
								}
								if (vkCode == '\t')
								{
									// Ctrl+Tab
									sendToWindow = wnd;
									delayedKeyEvent = Event.KeyboardEvent("^\t");
									return (IntPtr) 1;
								}
							}
							else
							{
								if (vkCode == 'Z')
								{
									// Shift+Ctrl+Z
									sendToWindow = wnd;
									delayedKeyEvent = Event.KeyboardEvent("#^y");
									return (IntPtr) 1;
								}
								if (vkCode == '\t')
								{
									// Shift+Ctrl+Tab
									sendToWindow = wnd;
									delayedKeyEvent = Event.KeyboardEvent("#^\t");
									return (IntPtr) 1;
								}
							}
						}
					}
				}
			}
			return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
		}
		
		static EditorWindow sendToWindow;
		static Event delayedKeyEvent;
		static void OnUpdate()
		{
			if (delayedKeyEvent != null)
			{
				var temp = delayedKeyEvent;
				delayedKeyEvent = null;
				if (sendToWindow && sendToWindow == EditorWindow.focusedWindow)
				{
					//Debug.Log("Forwarding " + temp);
					sendToWindow.SendEvent(temp);
				}
			}
		}
		
		static void OnDomainUnload(object sender, EventArgs e)
		{
			if (_keyboardHookID != IntPtr.Zero)
				UnhookWindowsHookEx(_keyboardHookID);
			_keyboardHookID = IntPtr.Zero;
			
			if (_mouseHookID != IntPtr.Zero)
				UnhookWindowsHookEx(_mouseHookID);
			_mouseHookID = IntPtr.Zero;
		}
#endif
	}
	
}
