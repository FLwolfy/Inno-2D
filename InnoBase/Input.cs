namespace InnoBase;

public static class Input
{
    // 鼠标按键
    public enum MouseButton : int
    {
        Left = 0,
        Right = 1,
        Middle = 2,
        XButton1 = 3,
        XButton2 = 4
    }

    // 常用键盘按键，参考Windows虚拟键码或者MonoGame Keys枚举
    public enum KeyCode : int
    {
        // 字母
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,

        // 数字键（主键盘区）
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,

        // 功能键
        Escape = 27,
        Space = 32,
        Enter = 13,
        Tab = 9,
        Backspace = 8,

        // 箭头键
        LeftArrow = 37,
        UpArrow = 38,
        RightArrow = 39,
        DownArrow = 40,

        // 修饰键
        LeftShift = 160,
        RightShift = 161,
        LeftCtrl = 162,
        RightCtrl = 163,
        LeftAlt = 164,
        RightAlt = 165,

        // 其他常用键
        CapsLock = 20,
        Insert = 45,
        Delete = 46,
        Home = 36,
        End = 35,
        PageUp = 33,
        PageDown = 34,

        // 小键盘数字键
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,

        NumLock = 144,
        ScrollLock = 145,

        // 功能键F1-F12
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,

        // 其他符号键
        Plus = 187,          // '+' key
        Minus = 189,         // '-' key
        Equals = 187,        // '=' key
        Comma = 188,         // ',' key
        Period = 190,        // '.' key
        Slash = 191,         // '/' key
        Backslash = 220,     // '\' key
        Semicolon = 186,     // ';' key
        Quote = 222,         // ''' key
        LeftBracket = 219,   // '[' key
        RightBracket = 221,  // ']' key
        Tilde = 192          // '`' key
    }
}