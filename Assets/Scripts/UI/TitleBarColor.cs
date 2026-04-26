using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TitleBarColor : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetActiveWindow();

    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref uint attrValue, int attrSize);

    const int DWMWA_CAPTION_COLOR = 35;       // Windows 11+
    const int DWMWA_TEXT_COLOR = 36;          // Optional (title text color)
    const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20; // For Windows 10 dark mode support

    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    void Awake()
    {
        CenterOnScreen();
    }

    private void CenterOnScreen()
    {
        System.IntPtr hWnd = GetActiveWindow();
        if (hWnd == System.IntPtr.Zero) return;

        GetWindowRect(hWnd, out RECT rect);

        int windowWidth = rect.Right - rect.Left;
        int windowHeight = rect.Bottom - rect.Top;

        int screenWidth = Display.main.systemWidth;
        int screenHeight = Display.main.systemHeight;

        int posX = (screenWidth - windowWidth) / 2;
        int posY = (screenHeight - windowHeight) / 2;

        MoveWindow(hWnd, posX, posY, windowWidth, windowHeight, true);

        uint darkMode = 1;
        DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(uint));

        //// Set custom title bar color (ABGR)
        //uint color = 0x00000000; // Dark gray (Blender-like)
        //DwmSetWindowAttribute(hwnd, DWMWA_CAPTION_COLOR, ref color, sizeof(uint));

        //// Optional: Set title text color (white)
        //uint textColor = 0xFFFFFFFF;
        //DwmSetWindowAttribute(hwnd, DWMWA_TEXT_COLOR, ref textColor, sizeof(uint));
    }
#endif
}
