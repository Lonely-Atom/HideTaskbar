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
            Win = 8
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        /// <summary>
        /// 创建快捷键帮助类对象
        /// </summary>
        /// <param name="id">快捷键 id</param>
        /// <param name="onHotkeyPressed">回调方法</param>
        public GlobalHotkeyHelper(int id, Action onHotkeyPressed)
        {
            this.id = id;
            this.onHotkeyPressed = onHotkeyPressed;
            CreateHandle(new CreateParams());
        }

        /// <summary>
        /// 注册全局快捷键
        /// </summary>
        /// <param name="hotkeyString">快捷键字符串</param>
        /// <returns>是否注册成功</returns>
        public bool RegisterHotKey(string hotkeyString)
        {
            if (TryParseHotKey(hotkeyString, out KeyModifiers modifiers, out Keys key))
                return RegisterHotKey(Handle, id, (int)modifiers, (int)key);
            else
                return false;
        }

        /// <summary>
        /// 尝试解析快捷键字符串为 KeyModifiers 和 Keys 返回
        /// </summary>
        /// <param name="hotkeyString">快捷键字符串</param>
        /// <param name="modifiers">功能键</param>
        /// <param name="key">按键</param>
        /// <returns>是否成功解析</returns>
        static bool TryParseHotKey(string hotkeyString, out KeyModifiers modifiers, out Keys key)
        {
            modifiers = KeyModifiers.None;
            key = Keys.None;

            if (string.IsNullOrWhiteSpace(hotkeyString))
                return false;

            string[] parts = hotkeyString.Split('+', StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.Trim())
                                .ToArray();

            foreach (string part in parts)
                switch (part.ToLower())
                {
                    case "ctrl":
                    case "control":
                        modifiers |= KeyModifiers.Ctrl;
                        break;
                    case "alt":
                        modifiers |= KeyModifiers.Alt;
                        break;
                    case "shift":
                        modifiers |= KeyModifiers.Shift;
                        break;
                    case "win":
                    case "windows":
                        modifiers |= KeyModifiers.Win;
                        break;
                    default:
                        if (part.Length == 1)
                        {
                            // 处理单个字符
                            char k = part[0];
                            if (char.IsLetterOrDigit(k))
                                key = (Keys)char.ToUpper(k);
                            else if (k == '~')
                                key = Keys.Oemtilde;
                            else
                                return false;
                        }
                        else
                            // 尝试解析为 Keys 枚举
                            if (!Enum.TryParse(part, true, out key))
                            return false;
                        break;
                }

            return modifiers != KeyModifiers.None && key != Keys.None;
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
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 捕获到注册的快捷键则触发回调方法
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == id)
                onHotkeyPressed?.Invoke();
        }
    }
}