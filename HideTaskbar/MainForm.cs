using HideTaskbar.Utils;
using System.Text;

namespace HideTaskbar
{
    public partial class MainForm : Form
    {
        // 全局快捷键对象
        private readonly GlobalHotkeyHelper globalHotkeyHelper;

        public MainForm()
        {
            InitializeComponent();

            // 注册全局快捷键
            globalHotkeyHelper = new GlobalHotkeyHelper(
                GetHashCode(),
                HideOrShowTaskbar,
                GlobalHotkeyHelper.KeyModifiers.Ctrl,
                Keys.Oemtilde
            );
        }

        // 窗口加载事件
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 虽然窗口大小已经设置为 0，但还是会有个灰窗口，需使用 Hide 方法隐藏
            Hide();

            // 读取 ini 文件中的配置
            string closeNoticeConfig = IniFileHelper.Select("CloseNotice");
            string autoStartConfig = IniFileHelper.Select("AutoStart");
            string autoHideConfig = IniFileHelper.Select("AutoHide");
            string firstShowAbout = IniFileHelper.Select("FirstShowAbout");

            // 关闭通知配置
            if (!string.IsNullOrEmpty(closeNoticeConfig))
            {
                EnableCloseNotice(Convert.ToBoolean(closeNoticeConfig), true);
            }
            // 开机自启动配置
            if (!string.IsNullOrEmpty(autoStartConfig))
            {
                EnableAutoStart(Convert.ToBoolean(autoStartConfig), true);
            }
            // 启动后自动隐藏任务栏配置
            if (!string.IsNullOrEmpty(autoHideConfig))
            {
                EnableAutoHide(Convert.ToBoolean(autoHideConfig), true);
            }
            // 第一次显示关于配置
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
            sb_msg.AppendLine("此软件无窗口，已运行在任务栏托盘中，可右键托盘中的图标打开菜单。\n");
            sb_msg.AppendLine("注意：请先详细阅读【关于】中的信息后再使用。");
            SendNotification(Text, sb_msg.ToString());
        }

        // 窗口关闭事件
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 释放全局快捷键
            globalHotkeyHelper.Dispose();
        }

        // 托盘图标鼠标双击事件
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                HideOrShowTaskbar();
        }

        // 菜单【隐藏/显示任务栏】选项点击事件
        private void tsm_hideOrShow_Click(object sender, EventArgs e)
        {
            HideOrShowTaskbar();
        }

        // 菜单【启动后自动隐藏任务栏】选项点击事件
        private void tsm_autoHide_Click(object sender, EventArgs e)
        {
            EnableAutoHide(!tsm_autoHide.Checked, false);
        }

        // 菜单【开机自启动】选项点击事件
        private void tsm_autoStart_Click(object sender, EventArgs e)
        {
            EnableAutoStart(!tsm_autoStart.Checked, false);
        }

        // 菜单【关闭通知】选项点击事件
        private void tsm_closeNotice_Click(object sender, EventArgs e)
        {
            EnableCloseNotice(!tsm_closeNotice.Checked, false);
        }

        // 菜单【关于】选项点击事件
        private void tsm_about_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        // 菜单【退出】选项点击事件
        private void tsm_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // 计时器触发事件
        private void timer_Tick(object sender, EventArgs e)
        {
            if (tsm_hideOrShow.Checked)
                // 完全隐藏任务栏
                HideTaskbarHelper.ChangeTaskbar(true);
        }

        // 隐藏/显示任务栏
        private void HideOrShowTaskbar()
        {
            tsm_hideOrShow.Checked = !tsm_hideOrShow.Checked;

            if (tsm_hideOrShow.Checked)
            {
                // 打开任务栏设置中的“在桌面模式下自动隐藏任务栏”选项
                HideTaskbarHelper.ChangeAutoHideTaskbar(true);
                tsm_hideOrShow.Text = "显示任务栏 (Ctrl + ~)";
                SendNotification(Text, "已隐藏任务栏。");
            }
            else
            {
                // 显示任务栏
                HideTaskbarHelper.ChangeTaskbar(false);
                // 关闭任务栏设置中的“在桌面模式下自动隐藏任务栏”选项
                HideTaskbarHelper.ChangeAutoHideTaskbar(false);
                tsm_hideOrShow.Text = "隐藏任务栏 (Ctrl + ~)";
                SendNotification(Text, "已显示任务栏。");
            }
        }

        // 设置是否启动后自动隐藏任务栏
        private void EnableAutoHide(bool enable, bool isInit)
        {
            tsm_autoHide.Checked = enable;

            if (enable)
            {
                tsm_autoHide.Text = "取消启动后自动隐藏任务栏";
                if (!isInit)
                    SendNotification(Text, "已设置启动后自动隐藏任务栏！");
                else
                    HideOrShowTaskbar();
            }
            else
            {
                tsm_autoHide.Text = "启动后自动隐藏任务栏";
                if (!isInit)
                    SendNotification(Text, "已取消启动后自动隐藏任务栏！");
            }

            if (!isInit)
                IniFileHelper.Update("AutoHide", enable.ToString());
        }

        // 设置是否开机自启动
        private void EnableAutoStart(bool enable, bool isInit)
        {
            tsm_autoStart.Checked = enable;

            if (enable)
            {
                AutoStartHelper.SetStartup(Text);
                tsm_autoStart.Text = "取消开机自启动";
                if (!isInit)
                    SendNotification(Text, "已设置开机自启动！");
            }
            else
            {
                AutoStartHelper.UnsetStartup(Text);
                tsm_autoStart.Text = "开机自启动";
                if (!isInit)
                    SendNotification(Text, "已取消开机自启动！");
            }

            if (!isInit)
                IniFileHelper.Update("AutoStart", enable.ToString());
        }

        // 设置是否关闭通知
        private void EnableCloseNotice(bool enable, bool isInit)
        {
            tsm_closeNotice.Checked = enable;

            if (enable)
            {
                tsm_closeNotice.Text = "开启通知";
            }
            else
            {
                tsm_closeNotice.Text = "关闭通知";
            }

            if (!isInit)
                IniFileHelper.Update("CloseNotice", enable.ToString());
        }

        // 显示关于
        private void ShowAbout()
        {
            StringBuilder sb_msg = new StringBuilder();
            sb_msg.AppendLine("【软件名】：隐藏任务栏小工具\n");

            sb_msg.AppendLine("【重要提醒】：");
            sb_msg.AppendLine("    1.本软件为免费提供，作者为「LonelyAtom」。任何索要付费购买此软件的行为均为欺诈。请勿向任何第三方支付费用，以免受到欺骗。");
            sb_msg.AppendLine("    2.本软件受到版权保护，并且仅限于合法获得许可的用户使用。未经授权的复制、分发或盗版行为将依法追究其法律责任。\n");

            sb_msg.AppendLine("【重要说明】：");
            sb_msg.AppendLine("    1.此软件无窗口，运行后将自动收到任务栏托盘中。");
            sb_msg.AppendLine("    2.隐藏任务栏后托盘也会被隐藏，所以请熟记以下快捷键，防止隐藏后无法恢复。");
            sb_msg.AppendLine("    3.双击任务栏图标可以【隐藏/显示任务栏】。\n");

            sb_msg.AppendLine("【快捷键】：");
            sb_msg.AppendLine("    1.隐藏/显示任务栏：【Ctrl + ~】\n");

            sb_msg.AppendLine("【作者】：LonelyAtom");

            MessageBox.Show(sb_msg.ToString(), "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 发送通知
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
