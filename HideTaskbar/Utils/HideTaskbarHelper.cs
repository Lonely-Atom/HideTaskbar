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
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string className, string windowText);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChild, string className, string? windowText);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int ShowWindow(IntPtr hwnd, int command);
        #endregion

        #region 非阻塞向指定窗口的消息队列中投递一条消息
        private const int BM_CLICK = 0x00F5;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool PostMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);
        #endregion

        #region 判断窗口是否为可见状态的 Windows API 函数
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        #endregion

        #region 获取窗口位置
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        #endregion

        #region 设置窗口位置
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Enum for SetWindowPos flags
        [Flags]
        private enum SetWindowPosFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040
        }
        #endregion

        #region 设置窗口焦点
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern uint AllowSetForegroundWindow(uint dwProcessId);
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
            IntPtr hwndPrimary = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwndPrimary, status ? SW_HIDE : SW_SHOWNORMAL);

            // 隐藏其他显示器上的任务栏
            foreach (var screen in Screen.AllScreens)
            {
                if (!screen.Primary)
                {
                    IntPtr hwndSecondary = FindWindow("Shell_SecondaryTrayWnd", "");
                    ShowWindow(hwndSecondary, status ? SW_HIDE : SW_SHOWNORMAL);
                }
            }
        }

        /// <summary>
        /// 获取系统托盘状态
        /// </summary>
        public static Boolean GetTaryStatus()
        {
            IntPtr hwndPrimary = FindWindow("NotifyIconOverflowWindow", "");
            return IsWindowVisible(hwndPrimary);
        }

        #region 【已弃用】此方法直接通过系统托盘窗口的句柄来显示，会导致托盘内新增图标时不会自动扩大系统托盘窗口的大小，导致部分图标不可见，以及开机自启动时自动隐藏任务栏，显示系统托盘会失效。
        /// <summary>
        /// 显示/隐藏系统托盘（已使用 WinSpy++ 查看，目前无法操作 Win11 的系统托盘）
        /// </summary>
        /// <param name="status">状态</param>
        public static void ChangeTray(bool status)
        {
            IntPtr hwndPrimary = FindWindow("NotifyIconOverflowWindow", "");
            ShowWindow(hwndPrimary, status ? SW_HIDE : SW_SHOWNORMAL);
        }
        #endregion

        #region 【新方法】使用找到任务栏句柄上的系统托盘按钮句柄上的按钮，再通过向按钮发送点击消息模拟点击系统托盘按钮（解决了系统托盘窗口大小不自动扩大的问题以及开机自启动时自动隐藏任务栏显示系统托盘失效的问题，但出现了新的问题：系统托盘在一些刷新情况下第一次会默认显示在屏幕左上角)
        /// <summary>
        /// 显示/隐藏系统托盘（已使用 WinSpy++ 查看，目前无法操作 Win11 的系统托盘）
        /// </summary>
        /// <param name="status">状态</param>
        public static void ChangeTray()
        {
            IntPtr hwndPrimary = FindWindow("Shell_TrayWnd", "");
            IntPtr hwndSecond = FindWindowEx(hwndPrimary, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr buttonHandle = FindWindowEx(hwndSecond, IntPtr.Zero, "Button", null);

            if (buttonHandle != IntPtr.Zero)
            {
                // 在此显示任务栏的目的是为了防止无任务栏的情况下通过模拟点击系统托盘按钮打开系统托盘，系统托盘会显示在屏幕的左上角
                ChangeTaskbar(false);
                // 向按钮发送点击消息
                PostMessage(buttonHandle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                // 显示系统托盘后给予其焦点，解决无法通过点击其他位置来隐藏系统托盘的问题
                IntPtr hwndTray = FindWindow("NotifyIconOverflowWindow", "");
                FocusExternalWindow(hwndTray);
            }
        }
        #endregion

        /// <summary>
        /// 将指定窗口句柄设为焦点
        /// </summary>
        /// <param name="handle">需设置焦点的窗口句柄</param>
        /// <exception cref="Exception">抛出异常</exception>
        public static void FocusExternalWindow(IntPtr handle)
        {
            try
            {
                // 允许所有进程设置前台窗口。0xFFFFFFFF表示允许所有进程。
                // 如果你知道目标窗口所属的具体进程ID，可以传入那个进程ID以增加安全性。
                AllowSetForegroundWindow(0xFFFFFFFF);
                // 将目标窗口设置为前台并赋予焦点
                SetForegroundWindow(handle);
            }
            catch (Exception ex)
            {
                throw new Exception($"无法将焦点转移到目标窗口: {ex.Message}");
            }
        }
    }
}
