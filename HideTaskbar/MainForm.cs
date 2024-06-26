using HideTaskbar.Utils;
using System.Text;

namespace HideTaskbar
{
    public partial class MainForm : Form
    {
        // ȫ�ֿ�ݼ�����
        private readonly GlobalHotkeyHelper? globalHotkeyTaskBar;
        private readonly GlobalHotkeyHelper? globalHotkeyTray;

        public MainForm()
        {
            InitializeComponent();

            #region ��ʼ��ȡ�����ļ��������������
            // �ر�֪ͨ����
            EnableCloseNotice(ConfigHelper.Instance.AppConfig.Configs.CloseNotice, true);
            // ��������������
            EnableAutoStart(ConfigHelper.Instance.AppConfig.Configs.AutoStart, true);
            // �������Զ���������������
            EnableAutoHide(ConfigHelper.Instance.AppConfig.Configs.AutoHide, true);
            // ��һ����ʾ��������
            if (ConfigHelper.Instance.AppConfig.Configs.FirstShowAbout)
            {
                ShowAbout();
                ConfigHelper.Instance.AppConfig.Configs.FirstShowAbout = false;
                ConfigHelper.Instance.UpdateConfig();
            }
            #endregion

            #region ע��ȫ�ֿ�ݼ�������/��ʾ������
            globalHotkeyTaskBar = new GlobalHotkeyHelper(
                GetHashCode(),
                HideOrShowTaskbar
            );

            if(!globalHotkeyTaskBar.RegisterHotKey(ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar))
                SendNotification(Text, $"��{ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar}���ȼ�ע��ʧ��!");
            #endregion

            #region ע��ȫ�ֿ�ݼ�������/��ʾ����
            globalHotkeyTray = new GlobalHotkeyHelper(
                GetHashCode(),
                HideOrShowTray
            );

            if (!globalHotkeyTray.RegisterHotKey(ConfigHelper.Instance.AppConfig.Hotkeys.Tray))
                SendNotification(Text, $"��{ConfigHelper.Instance.AppConfig.Hotkeys.Tray}���ȼ�ע��ʧ��!");
            #endregion

            #region ���г�����ʾ
            StringBuilder sb_msg = new();
            sb_msg.AppendLine("������޴��ڣ��������������������У����Ҽ������е�ͼ��򿪲˵���\n");
            sb_msg.AppendLine("ע�⣺������ϸ�Ķ������ڡ��е���Ϣ����ʹ�á�");
            SendNotification(Text, sb_msg.ToString());
            #endregion

            #region ��ʼ������/��ʾ�������˵����ı�����ֹ�� Designer �����ñ������±���
            tsm_hideOrShowTaskbar.Text = $"���������� ({ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar})";
            #endregion
        }

        // ���ڼ����¼�
        private void MainForm_Load(object sender, EventArgs e)
        {
            // ��Ȼ���ڴ�С�Ѿ�����Ϊ 0�������ǻ��и��Ҵ��ڣ���ʹ�� Hide ��������
            Hide();
        }

        // ���ڹر��¼�
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // �ͷ�ȫ�ֿ�ݼ�
            globalHotkeyTaskBar?.Dispose();
            globalHotkeyTray?.Dispose();
        }

        // ����ͼ�����˫���¼�
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                HideOrShowTaskbar();
        }

        // �˵�������/��ʾ��������ѡ�����¼�
        private void Tsm_hideOrShow_Click(object sender, EventArgs e)
        {
            HideOrShowTaskbar();
        }

        // �˵�������/��ʾϵͳ���̡�ѡ�����¼�
        private void Tsm_hideOrShowTray_Click(object sender, EventArgs e)
        {
            HideOrShowTray();
        }

        // �˵����������Զ�������������ѡ�����¼�
        private void Tsm_autoHide_Click(object sender, EventArgs e)
        {
            EnableAutoHide(!tsm_autoHide.Checked, false);
        }

        // �˵���������������ѡ�����¼�
        private void Tsm_autoStart_Click(object sender, EventArgs e)
        {
            EnableAutoStart(!tsm_autoStart.Checked, false);
        }

        // �˵����ر�֪ͨ��ѡ�����¼�
        private void Tsm_closeNotice_Click(object sender, EventArgs e)
        {
            EnableCloseNotice(!tsm_closeNotice.Checked, false);
        }

        // �˵������ڡ�ѡ�����¼�
        private void Tsm_about_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        // �˵����˳���ѡ�����¼�
        private void Tsm_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // ��ʱ�������¼�
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (tsm_hideOrShowTaskbar.Checked)
                // ��ȫ����������
                HideTaskbarHelper.ChangeTaskbar(true);
        }

        // ����/��ʾ������
        private void HideOrShowTaskbar()
        {
            tsm_hideOrShowTaskbar.Checked = !tsm_hideOrShowTaskbar.Checked;

            if (tsm_hideOrShowTaskbar.Checked)
            {
                // �������������еġ�������ģʽ���Զ�������������ѡ��
                HideTaskbarHelper.ChangeAutoHideTaskbar(true);
                tsm_hideOrShowTaskbar.Text = $"��ʾ������ ({ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar})";
                //SendNotification(Text, "��������������");
            }
            else
            {
                // ��ʾ������
                HideTaskbarHelper.ChangeTaskbar(false);
                // �ر������������еġ�������ģʽ���Զ�������������ѡ��
                HideTaskbarHelper.ChangeAutoHideTaskbar(false);
                tsm_hideOrShowTaskbar.Text = $"���������� ({ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar})";
                //SendNotification(Text, "����ʾ��������");
            }
        }

        // ����/��ʾϵͳ����
        private void HideOrShowTray()
        {
            if (HideTaskbarHelper.GetTaryStatus())
                HideTaskbarHelper.ChangeTray(true);
            else
                HideTaskbarHelper.ChangeTray(false);
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
            {
                ConfigHelper.Instance.AppConfig.Configs.AutoHide = enable;
                ConfigHelper.Instance.UpdateConfig();
            }
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
            {
                ConfigHelper.Instance.AppConfig.Configs.AutoStart = enable;
                ConfigHelper.Instance.UpdateConfig();
            }
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
            {
                ConfigHelper.Instance.AppConfig.Configs.CloseNotice = enable;
                ConfigHelper.Instance.UpdateConfig();
            }
        }

        // ��ʾ����
        private static void ShowAbout()
        {
            StringBuilder sb_msg = new();
            sb_msg.AppendLine("���������������������С����\n");

            sb_msg.AppendLine("����Ҫ���ѡ���");
            sb_msg.AppendLine("    1. �����Ϊ����ṩ������Ϊ��LonelyAtom�����κ���Ҫ���ѹ�����������Ϊ��Ϊ��թ���������κε�����֧�����ã������ܵ���ƭ��");
            sb_msg.AppendLine("    2. ������ܵ���Ȩ���������ҽ����ںϷ������ɵ��û�ʹ�á�δ����Ȩ�ĸ��ơ��ַ��������Ϊ������׷���䷨�����Ρ�\n");

            sb_msg.AppendLine("����Ҫ˵������");
            sb_msg.AppendLine("    1. ������޴��ڣ����к��Զ��յ������������С�");
            sb_msg.AppendLine("    2. ����������������Ҳ�ᱻ���أ�������������¿�ݼ�����ֹ���غ��޷��ָ���");
            sb_msg.AppendLine("    3. ˫��������ͼ����ԡ�����/��ʾ����������\n");

            sb_msg.AppendLine("����ݼ��������� appsettings.json �ļ����޸ģ���");
            sb_msg.AppendLine($"    1.����/��ʾ����������{ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar}��");
            sb_msg.AppendLine($"    2.����/��ʾϵͳ���̣���{ConfigHelper.Instance.AppConfig.Hotkeys.Tray}����Ŀǰ��֪ Win11 ��Ч��\n");

            sb_msg.AppendLine("�����ߡ���LonelyAtom\n");
            sb_msg.AppendLine("�������ߡ���Wwwwtgd");

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
