// using System;
// using System.Runtime.InteropServices;
// using System.Threading;
// using System.Threading.Tasks;
// using System.Windows;
// using System.Windows.Forms; // Add reference to System.Windows.Forms
// using System.Windows.Input;

// namespace operation.Tasks
// {
//     /// <summary>
//     /// WPF-compatible input automation class
//     /// Handles mouse and keyboard operations without conflicts
//     /// </summary>
//     public class WpfInputHelper
//     {
//         #region Win32 API Declarations

//         [DllImport("user32.dll", SetLastError = true)]
//         private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

//         [DllImport("user32.dll", SetLastError = true)]
//         private static extern bool SetCursorPos(int X, int Y);

//         [DllImport("user32.dll", SetLastError = true)]
//         private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

//         #endregion

//         #region Structures

//         [StructLayout(LayoutKind.Sequential)]
//         private struct INPUT
//         {
//             public uint type;
//             public INPUTUNION union;
//         }

//         [StructLayout(LayoutKind.Explicit)]
//         private struct INPUTUNION
//         {
//             [FieldOffset(0)]
//             public MOUSEINPUT mi;
//             [FieldOffset(0)]
//             public KEYBDINPUT ki;
//         }

//         [StructLayout(LayoutKind.Sequential)]
//         private struct MOUSEINPUT
//         {
//             public int dx;
//             public int dy;
//             public uint mouseData;
//             public uint dwFlags;
//             public uint time;
//             public IntPtr dwExtraInfo;
//         }

//         [StructLayout(LayoutKind.Sequential)]
//         private struct KEYBDINPUT
//         {
//             public ushort wVk;
//             public ushort wScan;
//             public uint dwFlags;
//             public uint time;
//             public IntPtr dwExtraInfo;
//         }

//         #endregion

//         #region Constants

//         private const uint INPUT_MOUSE = 0;
//         private const uint INPUT_KEYBOARD = 1;

//         // Mouse event flags
//         public const uint MOUSEEVENTF_MOVE = 0x0001;
//         public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
//         public const uint MOUSEEVENTF_LEFTUP = 0x0004;
//         public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
//         public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
//         public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
//         public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
//         public const uint MOUSEEVENTF_WHEEL = 0x0800;
//         public const uint MOUSEEVENTF_HWHEEL = 0x01000;
//         public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

//         // Keyboard event flags
//         public const uint KEYEVENTF_KEYDOWN = 0x0000;
//         public const uint KEYEVENTF_KEYUP = 0x0002;

//         #endregion

//         #region Public Methods - Compatible with your existing API

//         /// <summary>
//         /// Sets the cursor position (compatible with your PerformSetCursorPosition)
//         /// </summary>
//         public void PerformSetCursorPosition(int fixedX, int fixedY, int waitTime = 0)
//         {
//             if (waitTime > 0)
//                 Thread.Sleep(waitTime);

//             SetCursorPos(fixedX, fixedY);
//         }

//         /// <summary>
//         /// Performs a mouse click (compatible with your PerformClick)
//         /// </summary>
//         public void PerformClick(int fixedX, int fixedY, uint mouseEventDown, uint mouseEventUp, int waitTime = 0)
//         {
//             if (waitTime > 0)
//                 Thread.Sleep(waitTime);

//             var primaryScreen = Screen.PrimaryScreen;
//             if (primaryScreen == null)
//                 throw new InvalidOperationException("Primary screen is not available.");

//             int normalizedX = (int)(fixedX * (65535.0 / primaryScreen.Bounds.Width));
//             int normalizedY = (int)(fixedY * (65535.0 / primaryScreen.Bounds.Height));

//             INPUT[] inputs = new INPUT[3];

//             // Move mouse
//             inputs[0].type = INPUT_MOUSE;
//             inputs[0].union.mi.dx = normalizedX;
//             inputs[0].union.mi.dy = normalizedY;
//             inputs[0].union.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE;
//             inputs[0].union.mi.mouseData = 0;
//             inputs[0].union.mi.time = 0;
//             inputs[0].union.mi.dwExtraInfo = IntPtr.Zero;

//             // Mouse down
//             inputs[1].type = INPUT_MOUSE;
//             inputs[1].union.mi.dx = normalizedX;
//             inputs[1].union.mi.dy = normalizedY;
//             inputs[1].union.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | mouseEventDown;
//             inputs[1].union.mi.mouseData = 0;
//             inputs[1].union.mi.time = 0;
//             inputs[1].union.mi.dwExtraInfo = IntPtr.Zero;

//             // Mouse up
//             inputs[2].type = INPUT_MOUSE;
//             inputs[2].union.mi.dx = normalizedX;
//             inputs[2].union.mi.dy = normalizedY;
//             inputs[2].union.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | mouseEventUp;
//             inputs[2].union.mi.mouseData = 0;
//             inputs[2].union.mi.time = 0;
//             inputs[2].union.mi.dwExtraInfo = IntPtr.Zero;

//             SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
//         }

//         /// <summary>
//         /// Performs a scroll operation (compatible with your PerformScroll)
//         /// </summary>
//         public void PerformScroll(int fixedX, int fixedY, int scrollValue, uint mouseEventWheel, int waitTime = 0)
//         {
//             if (waitTime > 0)
//                 Thread.Sleep(waitTime);

//             var primaryScreen = Screen.PrimaryScreen;
//             if (primaryScreen == null)
//                 throw new InvalidOperationException("Primary screen is not available.");

//             int normalizedX = (int)(fixedX * (65535.0 / primaryScreen.Bounds.Width));
//             int normalizedY = (int)(fixedY * (65535.0 / primaryScreen.Bounds.Height));

//             INPUT[] inputs = new INPUT[1];
//             inputs[0].type = INPUT_MOUSE;
//             inputs[0].union.mi.dx = normalizedX;
//             inputs[0].union.mi.dy = normalizedY;
//             inputs[0].union.mi.dwFlags = mouseEventWheel | MOUSEEVENTF_ABSOLUTE;
//             inputs[0].union.mi.mouseData = (uint)scrollValue;
//             inputs[0].union.mi.time = 0;
//             inputs[0].union.mi.dwExtraInfo = IntPtr.Zero;

//             SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
//         }

//         /// <summary>
//         /// Performs a keyboard event (compatible with your PerformKeyEvent)
//         /// </summary>
//         public void PerformKeyEvent(int val, uint keyFlag, int waitTime = 0)
//         {
//             if (waitTime > 0)
//                 Thread.Sleep(waitTime);

//             keybd_event((byte)val, 0, keyFlag, 0);
//         }

//         /// <summary>
//         /// Types text (compatible with your PerformTypeText)
//         /// </summary>
//         public void PerformTypeText(string text, int waitTime = 0)
//         {
//             if (waitTime > 0)
//                 Thread.Sleep(waitTime);

//             // Use Unicode input for better WPF compatibility
//             foreach (char c in text)
//             {
//                 SendCharacter(c);
//                 Thread.Sleep(5); // Small delay between characters
//             }
//         }

//         /// <summary>
//         /// Alternative: Use SendKeys (may have conflicts in some WPF scenarios)
//         /// </summary>
//         public void PerformTypeTextWithSendKeys(string text, int waitTime = 0)
//         {
//             if (waitTime > 0)
//                 Thread.Sleep(waitTime);

//             SendKeys.SendWait(text);
//         }

//         #endregion

//         #region Private Helper Methods

//         private void SendCharacter(char c)
//         {
//             INPUT[] inputs = new INPUT[2];

//             // Key down
//             inputs[0].type = INPUT_KEYBOARD;
//             inputs[0].union.ki.wVk = 0;
//             inputs[0].union.ki.wScan = c;
//             inputs[0].union.ki.dwFlags = 0x0004; // KEYEVENTF_UNICODE
//             inputs[0].union.ki.time = 0;
//             inputs[0].union.ki.dwExtraInfo = IntPtr.Zero;

//             // Key up
//             inputs[1].type = INPUT_KEYBOARD;
//             inputs[1].union.ki.wVk = 0;
//             inputs[1].union.ki.wScan = c;
//             inputs[1].union.ki.dwFlags = 0x0004 | 0x0002; // KEYEVENTF_UNICODE | KEYEVENTF_KEYUP
//             inputs[1].union.ki.time = 0;
//             inputs[1].union.ki.dwExtraInfo = IntPtr.Zero;

//             SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
//         }

//         #endregion
//     }
// }