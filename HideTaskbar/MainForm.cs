using HideTaskbar.Utils;
using System.Text;

namespace HideTaskbar
{
    public partial class MainForm : Form
    {
        // ȫ�ֿ�ݼ�����
        private readonly GlobalHotkeyHelper globalHotkeyHelper;

        public MainForm()
        {
            InitializeComponent();

            // ע��ȫ�ֿ�ݼ�
            globalHotkeyHelper = new GlobalHotkeyHelper(
                GetHashCode(),
                HideOrShowTaskbar,
                GlobalHotkeyHelper.KeyModifiers.Ctrl,
                Keys.Oemtilde
            );
        }

        // ���ڼ����¼�
        private void MainForm_Load(object sender, EventArgs e)
        {
            // ��Ȼ���ڴ�С�Ѿ�����Ϊ 0�������ǻ��и��Ҵ��ڣ���ʹ�� Hide ��������
            Hide();

            // ��ȡ ini �ļ��е�����
            string closeNoticeConfig = IniFileHelper.Select("CloseNotice");
            string autoStartConfig = IniFileHelper.Select("AutoStart");
            string autoHideConfig = IniFileHelper.Select("AutoHide");
            string firstShowAbout = IniFileHelper.Select("FirstShowAbout");

            // �ر�֪ͨ����
            if (!string.IsNullOrEmpty(closeNoticeConfig))
            {
                EnableCloseNotice(Convert.ToBoolean(closeNoticeConfig), true);
            }
            // ��������������
            if (!string.IsNullOrEmpty(autoStartConfig))
            {
                EnableAutoStart(Convert.ToBoolean(autoStartConfig), true);
            }
            // �������Զ���������������
            if (!string.IsNullOrEmpty(autoHideConfig))
            {
                EnableAutoHide(Convert.ToBoolean(autoHideConfig), true);
            }
            // ��һ����ʾ��������
            if (!string.IsNullOrEmpty(firstShowAbout))
            {
                bool isFirstShowAbout = Convert.ToBoolean(firstShowAbout);
                if (isFirstShowAbout)
                {
                    ShowAbout();
                    IniFileHelper.Update("FirstShowAbout", "False");
                }
            }

            StringBuilder sb_msg = new StringBuilder();
            sb_msg.AppendLine("������޴��ڣ��������������������У����Ҽ������е�ͼ��򿪲˵���\n");
            sb_msg.AppendLine("ע�⣺������ϸ�Ķ������ڡ��е���Ϣ����ʹ�á�");
            SendNotification(Text, sb_msg.ToString());
        }

        // ���ڹر��¼�
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // �ͷ�ȫ�ֿ�ݼ�
            globalHotkeyHelper.Dispose();
        }

        // ����ͼ�����˫���¼�
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                HideOrShowTaskbar();
        }

        // �˵�������/��ʾ��������ѡ�����¼�
        private void tsm_hideOrShow_Click(object sender, EventArgs e)
        {
            HideOrShowTaskbar();
        }

        // �˵����������Զ�������������ѡ�����¼�
        private void tsm_autoHide_Click(object sender, EventArgs e)
        {
            EnableAutoHide(!tsm_autoHide.Checked, false);
        }

        // �˵���������������ѡ�����¼�
        private void tsm_autoStart_Click(object sender, EventArgs e)
        {
            EnableAutoStart(!tsm_autoStart.Checked, false);
        }

        // �˵����ر�֪ͨ��ѡ�����¼�
        private void tsm_closeNotice_Click(object sender, EventArgs e)
        {
            EnableCloseNotice(!tsm_closeNotice.Checked, false);
        }

        // �˵������ڡ�ѡ�����¼�
        private void tsm_about_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        // �˵����˳���ѡ�����¼�
        private void tsm_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // ��ʱ�������¼�
        private void timer_Tick(object sender, EventArgs e)
        {
            if (tsm_hideOrShow.Checked)
                // ��ȫ����������
                HideTaskbarHelper.ChangeTaskbar(true);
        }

        // ����/��ʾ������
        private void HideOrShowTaskbar()
        {
            tsm_hideOrShow.Checked = !tsm_hideOrShow.Checked;

            if (tsm_hideOrShow.Checked)
            {
                // �������������еġ�������ģʽ���Զ�������������ѡ��
                HideTaskbarHelper.ChangeAutoHideTaskbar(true);
                tsm_hideOrShow.Text = "��ʾ������ (Ctrl + ~)";
                SendNotification(Text, "��������������");
            }
            else
            {
                // ��ʾ������
                HideTaskbarHelper.ChangeTaskbar(false);
                // �ر������������еġ�������ģʽ���Զ�������������ѡ��
                HideTaskbarHelper.ChangeAutoHideTaskbar(false);
                tsm_hideOrShow.Text = "���������� (Ctrl + ~)";
                SendNotification(Text, "����ʾ��������");
            }
        }

        // �����Ƿ��������Զ�����������
        private void EnableAutoHide(bool enable, bool isInit)
        {
            tsm_autoHide.Checked = enable;

            if (enable)
            {
                tsm_autoHide.Text = "ȡ���������Զ�����������";
                if (!isInit)
                    SendNotification(Text, "�������������Զ�������������");
                else
                    HideOrShowTaskbar();
            }
            else
            {
                tsm_autoHide.Text = "�������Զ�����������";
                if (!isInit)
                    SendNotification(Text, "��ȡ���������Զ�������������");
            }

            if (!isInit)
                IniFileHelper.Update("AutoHide", enable.ToString());
        }

        // �����Ƿ񿪻�������
        private void EnableAutoStart(bool enable, bool isInit)
        {
            tsm_autoStart.Checked = enable;

            if (enable)
            {
                AutoStartHelper.SetStartup(Text);
                tsm_autoStart.Text = "ȡ������������";
                if (!isInit)
                    SendNotification(Text, "�����ÿ�����������");
            }
            else
            {
                AutoStartHelper.UnsetStartup(Text);
                tsm_autoStart.Text = "����������";
                if (!isInit)
                    SendNotification(Text, "��ȡ��������������");
            }

            if (!isInit)
                IniFileHelper.Update("AutoStart", enable.ToString());
        }

        // �����Ƿ�ر�֪ͨ
        private void EnableCloseNotice(bool enable, bool isInit)
        {
            tsm_closeNotice.Checked = enable;

            if (enable)
            {
                tsm_closeNotice.Text = "����֪ͨ";
            }
            else
            {
                tsm_closeNotice.Text = "�ر�֪ͨ";
            }

            if (!isInit)
                IniFileHelper.Update("CloseNotice", enable.ToString());
        }

        // ��ʾ����
        private void ShowAbout()
        {
            StringBuilder sb_msg = new StringBuilder();
            sb_msg.AppendLine("���������������������С����\n");

            sb_msg.AppendLine("����Ҫ���ѡ���");
            sb_msg.AppendLine("    1.�����Ϊ����ṩ������Ϊ��LonelyAtom�����κ���Ҫ���ѹ�����������Ϊ��Ϊ��թ���������κε�����֧�����ã������ܵ���ƭ��");
            sb_msg.AppendLine("    2.������ܵ���Ȩ���������ҽ����ںϷ������ɵ��û�ʹ�á�δ����Ȩ�ĸ��ơ��ַ��������Ϊ������׷���䷨�����Ρ�\n");

            sb_msg.AppendLine("����Ҫ˵������");
            sb_msg.AppendLine("    1.������޴��ڣ����к��Զ��յ������������С�");
            sb_msg.AppendLine("    2.����������������Ҳ�ᱻ���أ�������������¿�ݼ�����ֹ���غ��޷��ָ���");
            sb_msg.AppendLine("    3.˫��������ͼ����ԡ�����/��ʾ����������\n");

            sb_msg.AppendLine("����ݼ�����");
            sb_msg.AppendLine("    1.����/��ʾ����������Ctrl + ~��\n");

            sb_msg.AppendLine("�����ߡ���LonelyAtom");

            MessageBox.Show(sb_msg.ToString(), "����", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ����֪ͨ
        private void SendNotification(string title, string text)
        {
            if (!tsm_closeNotice.Checked)
            {
                notifyIcon.BalloonTipTitle = title;
                notifyIcon.BalloonTipText = text;
                notifyIcon.ShowBalloonTip(2000);
            }
        }
    }
}
