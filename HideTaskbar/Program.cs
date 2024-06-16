using System.Diagnostics;

namespace HideTaskbar
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 获取当前进程的名称 
            string processName = Process.GetCurrentProcess().ProcessName;
            Process[] runningProcesses = Process.GetProcessesByName(processName);

            // 如果已经存在相同的进程，则显示信息并退出
            if (runningProcesses.Length > 1)
            {
                MessageBox.Show("应用程序已经在运行中。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}