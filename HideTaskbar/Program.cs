namespace HideTaskbar
{
    internal static class Program
    {
        // ���廥�����
        static readonly Mutex mutex = new(true, "BBFAB829-3487-44AD-B80C-C28F10E4B39F");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ����Ƿ��Ѿ���һ��ʵ��������
            if (!mutex.WaitOne(TimeSpan.Zero))
            {
                MessageBox.Show("Ӧ�ó����Ѿ��������С�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());

            // �ͷŻ������
            mutex.ReleaseMutex();
        }
    }
}