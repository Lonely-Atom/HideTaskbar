namespace HideTaskbar
{
    internal static class Program
    {
        // 定义互斥对象
        static readonly Mutex mutex = new(true, "BBFAB829-3487-44AD-B80C-C28F10E4B39F");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 检查是否已经有一个实例在运行
            if (!mutex.WaitOne(TimeSpan.Zero))
            {
                MessageBox.Show("应用程序已经在运行中。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // 监听 Application.ApplicationExit 事件
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            Application.Run(new MainForm());
        }

        // 程序退出回调方法
        private static void OnApplicationExit(object? sender, EventArgs e)
        {
            // 释放互斥对象
            mutex.ReleaseMutex();
        }
    }
}