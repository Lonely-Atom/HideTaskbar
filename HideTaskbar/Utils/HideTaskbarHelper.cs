﻿using System.Runtime.InteropServices;

namespace HideTaskbar.Utils
{
    public static class HideTaskbarHelper
    {
        #region 切换任务栏设置中的“在桌面模式下自动隐藏任务栏”选项 Windows API 函数
        const uint ABM_SETSTATE = 0x0000000A;
        const uint ABS_AUTOHIDE = 0x0000001;

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        #endregion

        #region 完全隐藏任务栏 Windows API 函数
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);
        #endregion

        /// <summary>
        /// 切换任务栏设置中的“在桌面模式下自动隐藏任务栏”选项
        /// </summary>
        /// <param name="status">状态</param>
        public static void ChangeAutoHideTaskbar(bool status)
        {
            APPBARDATA appBarData = new APPBARDATA
            {
                cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA)),
                hWnd = IntPtr.Zero,
                uEdge = 0xFFFFFFFF,
                lParam = status ? (IntPtr)ABS_AUTOHIDE : IntPtr.Zero
            };

            SHAppBarMessage(ABM_SETSTATE, ref appBarData);
        }

        /// <summary>
        /// 完全隐藏任务栏
        /// </summary>
        /// <param name="status">状态</param>
        public static void ChangeTaskbar(bool status)
        {
            int hwndPrimary = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwndPrimary, status ? SW_HIDE : SW_SHOW);

            // 隐藏其他显示器上的任务栏
            foreach (var screen in Screen.AllScreens)
            {
                if (!screen.Primary)
                {
                    int hwndSecondary = FindWindow("Shell_SecondaryTrayWnd", "");
                    ShowWindow(hwndSecondary, status ? SW_HIDE : SW_SHOW);
                }
            }
        }
    }
}
