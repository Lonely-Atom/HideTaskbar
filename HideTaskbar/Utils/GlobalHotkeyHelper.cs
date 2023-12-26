using System.Runtime.InteropServices;

namespace HideTaskbar.Utils
{
    public class GlobalHotkeyHelper : NativeWindow
    {
        // 快捷键 id
        private readonly int id;
        // 回调方法
        private readonly Action onHotkeyPressed;

        #region 全局快捷键 Windows API 函数
        private const int WM_HOTKEY = 0x0312;

        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        /// <summary>
        /// 注册全局快捷键
        /// </summary>
        /// <param name="id">快捷键 id</param>
        /// <param name="onHotkeyPressed">回调方法</param>
        /// <param name="modifiers">修饰键</param>
        /// <param name="key">按键</param>
        public GlobalHotkeyHelper(int id, Action onHotkeyPressed, KeyModifiers modifiers, Keys key)
        {
            this.id = id;
            this.onHotkeyPressed = onHotkeyPressed;
            CreateHandle(new CreateParams());
            RegisterHotKey(Handle, id, (int)modifiers, (int)key);
        }

        /// <summary>
        /// 释放全局快捷键
        /// </summary>
        public void Dispose()
        {
            UnregisterHotKey(Handle, id);
            DestroyHandle();
        }

        /// <summary>
        /// 重写窗口过程函数，捕获到快捷键则触发回调方法
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 捕获到注册的快捷键则触发回调方法
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == id)
                onHotkeyPressed?.Invoke();
        }
    }
}