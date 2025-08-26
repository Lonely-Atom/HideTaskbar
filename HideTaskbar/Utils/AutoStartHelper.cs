using Microsoft.Win32;

namespace HideTaskbar.Utils
{
    /// <summary>
    /// 设置开机自动启动帮助类
    /// </summary>
    public class AutoStartHelper
    {
        private const string _REGISTRY_KEY_STRING = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        #region 设置开机自启动
        /// <summary>
        /// 设置开机自启动
        /// </summary>
        /// <param name="appName">软件名称</param>
        public static void SetStartup(string appName)
        {
            string appPath = Application.ExecutablePath;

            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(_REGISTRY_KEY_STRING, true);

            if (registryKey != null)
            {
                // 判断是否已经设置开机自启动
                if (registryKey.GetValue(appName) == null)
                {
                    // 设置开机自启动
                    registryKey.SetValue(appName, appPath);
                }

                registryKey.Close();
            }
        }
        #endregion

        #region 取消开机自启动
        /// <summary>
        /// 取消开机自启动
        /// </summary>
        /// <param name="appName">软件名称</param>
        public static void UnsetStartup(string appName)
        {
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(_REGISTRY_KEY_STRING, true);

            if (registryKey != null)
            {
                // 判断是否已经设置了开机自启动
                if (registryKey.GetValue(appName) != null)
                {
                    // 取消开机自启动
                    registryKey.DeleteValue(appName);
                }

                registryKey.Close();
            }
        }
        #endregion
    }
}
