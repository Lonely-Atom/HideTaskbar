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
            // ��ȡ��ǰ���̵����� 
            string processName = Process.GetCurrentProcess().ProcessName;
            Process[] runningProcesses = Process.GetProcessesByName(processName);

            // ����Ѿ�������ͬ�Ľ��̣�����ʾ��Ϣ���˳�
            if (runningProcesses.Length > 1)
            {
                MessageBox.Show("Ӧ�ó����Ѿ��������С�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}