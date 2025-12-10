namespace GraphSimulator.Execution.Common
{
    public static class Constants
    {
        // Virtual Key Codes - Define them directly here
        public const byte VK_RETURN = 0x0D;
        public const byte VK_SHIFT = 0x10;
        public const byte VK_CONTROL = 0x11;
        public const byte VK_MENU = 0x12;  // ALT key
        public const byte VK_ESCAPE = 0x1B;
        public const byte VK_SPACE = 0x20;
        public const byte VK_LEFT = 0x25;
        public const byte VK_UP = 0x26;
        public const byte VK_RIGHT = 0x27;
        public const byte VK_DOWN = 0x28;
        public const byte VK_DELETE = 0x2E;
        public const byte VK_TAB = 0x09;
        
        // Mouse event flags
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MOUSEEVENTF_WHEEL = 0x0800;
    }
}