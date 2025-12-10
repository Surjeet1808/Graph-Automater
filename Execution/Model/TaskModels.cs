using System.Runtime.InteropServices;

namespace operation.Model
{

    [StructLayout(LayoutKind.Sequential)]
    struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public int mouseData;
        public int dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
    }

     [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        public int type;
        public MOUSEINPUT mi;
    }
}