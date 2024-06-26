using System.Runtime.InteropServices;

namespace HideTaskbar.Utils
{
    public static class HideTaskbarHelper
    {
        #region 切换任务栏设置中的“在桌面模式下自动隐藏任务栏”选项 Windows API 函数
        private const uint ABM_SETSTATE = 0x0000000A;
        private const uint ABS_AUTOHIDE = 0x0000001;

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        #endregion

        #region 查找和显示/隐藏 Windows 窗口的 Windows API 函数
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);
        #endregion

        #region 判断窗口是否为可见状态的 Windows API 函数
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(int hWnd);
        #endregion

        /// <summary>
        /// 切换任务栏设置中的“在桌面模式下自动隐藏任务栏”选项
        /// </summary>
        /// <param name="status">状态</param>
        public static void ChangeAutoHideTaskbar(bool status)
        {
            APPBARDATA appBarData = new()
            {
                cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA)),
                hWnd = IntPtr.Zero,
                uEdge = 0xFFFFFFFF,
                lParam = status ? (IntPtr)ABS_AUTOHIDE : IntPtr.Zero
            };

            SHAppBarMessage(ABM_SETSTATE, ref appBarData);
        }

        /// <summary>
        /// 显示/隐藏任务栏
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

        /// <summary>
        /// 获取系统托盘状态
        /// </summary>
        public static Boolean GetTaryStatus()
        {
            int hwndPrimary = FindWindow("NotifyIconOverflowWindow", "");
            return IsWindowVisible(hwndPrimary);
        }

        /// <summary>
        /// 显示/隐藏系统托盘（已使用 WinSpy++ 查看，目前无法操作 Win11 的系统托盘）
        /// </summary>
        /// <param name="status">状态</param>
        public static void ChangeTray(bool status)
        {
            int hwndPrimary = FindWindow("NotifyIconOverflowWindow", "");
            ShowWindow(hwndPrimary, status ? SW_HIDE : SW_SHOW);
        }
    }
}
