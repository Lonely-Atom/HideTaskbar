using HideTaskbar.Utils;

using System.Text;

namespace HideTaskbar
{
    public partial class MainForm : Form
    {
        #region 全局快捷键对象
        private readonly GlobalHotkeyHelper? globalHotkeyTaskBar;
        private readonly GlobalHotkeyHelper? globalHotkeyTray;
        private readonly GlobalHotkeyHelper? globalHotkeyDesktopIcon;
        private readonly GlobalHotkeyHelper? globalHotkeyDesktop;
        #endregion

        public MainForm()
        {
            InitializeComponent();

            #region 初始读取配置文件并进行相关设置
            // 关闭通知配置
            EnableCloseNotice(ConfigHelper.Instance.AppConfig.Configs.CloseNotice, true);
            // 开机自启动配置
            EnableAutoStart(ConfigHelper.Instance.AppConfig.Configs.AutoStart, true);
            // 启动后自动隐藏任务栏配置
            EnableAutoHide(ConfigHelper.Instance.AppConfig.Configs.AutoHideTaskbar, true);
            // 启动后自动隐藏桌面图标配置
            EnableAutoHideDesktopIcon(ConfigHelper.Instance.AppConfig.Configs.AutoHideDesktopIcon, true);
            // 启动后自动隐藏桌面配置
            EnableAutoHideDesktop(ConfigHelper.Instance.AppConfig.Configs.AutoHideDesktop, true);
            // 第一次显示关于配置
            if (ConfigHelper.Instance.AppConfig.Configs.FirstShowAbout)
            {
                ShowAbout();
                ConfigHelper.Instance.AppConfig.Configs.FirstShowAbout = false;
                ConfigHelper.Instance.UpdateConfig();
            }
            #endregion

            #region 注册全局快捷键：隐藏/显示任务栏
            globalHotkeyTaskBar = new GlobalHotkeyHelper(
                GetHashCode(),
                HideOrShowTaskbar
            );

            if (!globalHotkeyTaskBar.RegisterHotKey(ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar))
                SendNotification(Text, $"【{ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar}】热键注册失败!");
            #endregion

            #region 注册全局快捷键：隐藏/显示托盘
            globalHotkeyTray = new GlobalHotkeyHelper(
                GetHashCode(),
                HideOrShowTray
            );

            if (!globalHotkeyTray.RegisterHotKey(ConfigHelper.Instance.AppConfig.Hotkeys.Tray))
                SendNotification(Text, $"【{ConfigHelper.Instance.AppConfig.Hotkeys.Tray}】热键注册失败!");
            #endregion

            #region 注册全局快捷键：隐藏/显示桌面图标
            globalHotkeyDesktopIcon = new GlobalHotkeyHelper(
                GetHashCode(),
                HideOrShowDesktopIcon
            );

            if (!globalHotkeyDesktopIcon.RegisterHotKey(ConfigHelper.Instance.AppConfig.Hotkeys.DesktopIcon))
                SendNotification(Text, $"【{ConfigHelper.Instance.AppConfig.Hotkeys.DesktopIcon}】热键注册失败!");
            #endregion

            #region 注册全局快捷键：隐藏/显示桌面
            globalHotkeyDesktop = new GlobalHotkeyHelper(
                GetHashCode(),
                HideOrShowDesktop
            );

            if (!globalHotkeyDesktop.RegisterHotKey(ConfigHelper.Instance.AppConfig.Hotkeys.Desktop))
                SendNotification(Text, $"【{ConfigHelper.Instance.AppConfig.Hotkeys.Desktop}】热键注册失败!");
            #endregion

            #region 运行程序提示
            StringBuilder sb_msg = new();
            sb_msg.AppendLine("此软件无窗口，已运行在任务栏托盘中，可右键托盘中的图标打开菜单。\n");
            sb_msg.AppendLine("注意：请先详细阅读【关于】中的信息后再使用。");
            SendNotification(Text, sb_msg.ToString());
            #endregion

            #region 初始化隐藏/显示任务栏菜单项文本，防止在 Designer 中引用变量导致报错
            tsm_hideOrShowTaskbar.Text = $"隐藏任务栏 ({ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar})";
            #endregion
        }

        #region 窗口加载事件
        /// <summary>
        /// 窗口加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 虽然窗口大小已经设置为 0，但还是会有个灰窗口，需使用 Hide 方法隐藏
            Hide();
        }
        #endregion

        #region 窗口关闭事件
        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // TODO: 退出时恢复设置

            // 释放全局快捷键
            globalHotkeyTaskBar?.Dispose();
            globalHotkeyTray?.Dispose();
            globalHotkeyDesktopIcon?.Dispose();
            globalHotkeyDesktop?.Dispose();
        }
        #endregion

        #region 托盘图标鼠标双击事件
        /// <summary>
        /// 托盘图标鼠标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                HideOrShowTaskbar();
        }
        #endregion

        #region 菜单【隐藏/显示任务栏】选项点击事件
        /// <summary>
        /// 菜单【隐藏/显示任务栏】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_hideOrShow_Click(object sender, EventArgs e)
        {
            HideOrShowTaskbar();
        }
        #endregion

        #region 菜单【隐藏/显示系统托盘】选项点击事件
        /// <summary>
        /// 菜单【隐藏/显示系统托盘】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_hideOrShowTray_Click(object sender, EventArgs e)
        {
            HideOrShowTray();
        }
        #endregion

        #region 菜单【启动后自动隐藏任务栏】选项点击事件
        /// <summary>
        /// 菜单【启动后自动隐藏任务栏】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_autoHide_Click(object sender, EventArgs e)
        {
            EnableAutoHide(!tsm_autoHide.Checked, false);
        }
        #endregion

        #region 菜单【启动后自动隐藏桌面图标】选项点击事件
        /// <summary>
        /// 菜单【启动后自动隐藏桌面图标】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_autoHideDesktopIcon_Click(object sender, EventArgs e)
        {
            EnableAutoHideDesktopIcon(!tsm_autoHideDesktopIcon.Checked, false);
        }
        #endregion

        #region 菜单【启动后自动隐藏桌面】选项点击事件
        /// <summary>
        /// 菜单【启动后自动隐藏桌面】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_autoHideDesktop_Click(object sender, EventArgs e)
        {
            EnableAutoHideDesktop(!tsm_autoHideDesktop.Checked, false);
        }
        #endregion

        #region 菜单【开机自启动】选项点击事件
        /// <summary>
        /// 菜单【开机自启动】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_autoStart_Click(object sender, EventArgs e)
        {
            EnableAutoStart(!tsm_autoStart.Checked, false);
        }
        #endregion

        #region 菜单【关闭通知】选项点击事件
        /// <summary>
        /// 菜单【关闭通知】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_closeNotice_Click(object sender, EventArgs e)
        {
            EnableCloseNotice(!tsm_closeNotice.Checked, false);
        }
        #endregion

        #region 菜单【关于】选项点击事件
        /// <summary>
        /// 菜单【关于】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_about_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }
        #endregion

        #region 菜单【退出】选项点击事件
        /// <summary>
        /// 菜单【退出】选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsm_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region 计时器触发事件
        /// <summary>
        /// 计时器触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (tsm_hideOrShowTaskbar.Checked)
                // 完全隐藏任务栏
                HideTaskbarHelper.ChangeHideTaskbar(true);
        }
        #endregion

        #region 隐藏/显示任务栏
        /// <summary>
        /// 隐藏/显示任务栏
        /// </summary>
        private void HideOrShowTaskbar()
        {
            // 在隐藏/显示任务栏时若系统托盘为打开状态，则将其隐藏（防止在打开系统托盘的情况下隐藏任务栏会导致在隐藏任务栏状态下显示系统托盘会同时使任务栏显示的问题）
            if (HideTaskbarHelper.GetTaryStatus())
                HideTaskbarHelper.ChangeTray();

            tsm_hideOrShowTaskbar.Checked = !tsm_hideOrShowTaskbar.Checked;

            if (tsm_hideOrShowTaskbar.Checked)
            {
                // 打开任务栏设置中的“在桌面模式下自动隐藏任务栏”选项
                HideTaskbarHelper.ChangeAutoHideTaskbar(true);
                tsm_hideOrShowTaskbar.Text = $"显示任务栏 ({ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar})";
                //SendNotification(Text, "已隐藏任务栏。");
            }
            else
            {
                // 显示任务栏
                HideTaskbarHelper.ChangeHideTaskbar(false);
                // 关闭任务栏设置中的“在桌面模式下自动隐藏任务栏”选项
                HideTaskbarHelper.ChangeAutoHideTaskbar(false);
                tsm_hideOrShowTaskbar.Text = $"隐藏任务栏 ({ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar})";
                //SendNotification(Text, "已显示任务栏。");
            }
        }
        #endregion

        #region 隐藏/显示系统托盘
        /// <summary>
        /// 隐藏/显示系统托盘
        /// </summary>
        private void HideOrShowTray()
        {
            HideTaskbarHelper.ChangeTray();
        }
        #endregion

        #region 隐藏/显示桌面图标
        /// <summary>
        /// 隐藏/显示桌面图标
        /// </summary>
        private void HideOrShowDesktopIcon()
        {
            HideTaskbarHelper.ChangeDesktopIcon();
        }
        #endregion

        #region 隐藏/显示桌面
        /// <summary>
        /// 隐藏/显示桌面
        /// </summary>
        private void HideOrShowDesktop()
        {
            HideTaskbarHelper.ChangeDesktop();
        }
        #endregion

        #region 设置是否启动后自动隐藏任务栏
        /// <summary>
        /// 设置是否启动后自动隐藏任务栏
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="isInit"></param>
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
            {
                ConfigHelper.Instance.AppConfig.Configs.AutoHideTaskbar = enable;
                ConfigHelper.Instance.UpdateConfig();
            }
        }
        #endregion

        #region 设置是否启动后自动隐藏桌面图标
        /// <summary>
        /// 设置是否启动后自动隐藏桌面图标
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="isInit"></param>
        private void EnableAutoHideDesktopIcon(bool enable, bool isInit)
        {
            tsm_autoHideDesktopIcon.Checked = enable;

            if (enable)
            {
                tsm_autoHideDesktopIcon.Text = "取消启动后自动隐藏桌面图标";
                if (!isInit)
                    SendNotification(Text, "已设置启动后自动隐藏桌面图标！");
                else
                    HideOrShowDesktopIcon();
            }
            else
            {
                tsm_autoHideDesktopIcon.Text = "启动后自动隐藏桌面图标";
                if (!isInit)
                    SendNotification(Text, "已取消启动后自动隐藏桌面图标！");
            }

            if (!isInit)
            {
                ConfigHelper.Instance.AppConfig.Configs.AutoHideDesktopIcon = enable;
                ConfigHelper.Instance.UpdateConfig();
            }
        }
        #endregion

        #region 设置是否启动后自动隐藏桌面
        /// <summary>
        /// 设置是否启动后自动隐藏桌面
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="isInit"></param>
        private void EnableAutoHideDesktop(bool enable, bool isInit)
        {
            tsm_autoHideDesktop.Checked = enable;

            if (enable)
            {
                tsm_autoHideDesktop.Text = "取消启动后自动隐藏桌面";
                if (!isInit)
                    SendNotification(Text, "已设置启动后自动隐藏桌面！");
                else
                    HideOrShowDesktop();
            }
            else
            {
                tsm_autoHideDesktop.Text = "启动后自动隐藏桌面";
                if (!isInit)
                    SendNotification(Text, "已取消启动后自动隐藏桌面！");
            }

            if (!isInit)
            {
                ConfigHelper.Instance.AppConfig.Configs.AutoHideDesktop = enable;
                ConfigHelper.Instance.UpdateConfig();
            }
        }
        #endregion

        #region 设置是否开机自启动
        /// <summary>
        /// 设置是否开机自启动
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="isInit"></param>
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
            {
                ConfigHelper.Instance.AppConfig.Configs.AutoStart = enable;
                ConfigHelper.Instance.UpdateConfig();
            }
        }
        #endregion

        #region 设置是否关闭通知
        /// <summary>
        /// 设置是否关闭通知
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="isInit"></param>
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
            {
                ConfigHelper.Instance.AppConfig.Configs.CloseNotice = enable;
                ConfigHelper.Instance.UpdateConfig();
            }
        }
        #endregion

        #region 显示关于
        /// <summary>
        /// 显示关于
        /// </summary>
        private static void ShowAbout()
        {
            StringBuilder sb_msg = new();
            sb_msg.AppendLine("【软件名】：隐藏任务栏小工具");
            sb_msg.AppendLine();
            sb_msg.AppendLine("【重要提醒】：");
            sb_msg.AppendLine("    1. 本软件为免费提供，作者为「LonelyAtom」。任何索要付费购买此软件的行为均为欺诈。请勿向任何第三方支付费用，以免受到欺骗。");
            sb_msg.AppendLine("    2. 本软件受到版权保护，并且仅限于合法获得许可的用户使用。未经授权的复制、分发或盗版行为将依法追究其法律责任。");
            sb_msg.AppendLine();
            sb_msg.AppendLine("【重要说明】：");
            sb_msg.AppendLine("    1. 此软件无窗口，运行后将自动收到任务栏托盘中。");
            sb_msg.AppendLine("    2. 隐藏任务栏后托盘也会被隐藏，所以请熟记以下快捷键，防止隐藏后无法恢复。");
            sb_msg.AppendLine("    3. 双击任务栏图标可以【隐藏/显示任务栏】。");
            sb_msg.AppendLine();
            sb_msg.AppendLine("【快捷键】（可在 appsettings.json 文件中修改）：");
            sb_msg.AppendLine($"    1.隐藏/显示任务栏：【{ConfigHelper.Instance.AppConfig.Hotkeys.TaskBar}】");
            sb_msg.AppendLine($"    2.隐藏/显示系统托盘：【{ConfigHelper.Instance.AppConfig.Hotkeys.Tray}】");
            sb_msg.AppendLine($"    3.隐藏/显示桌面图标：【{ConfigHelper.Instance.AppConfig.Hotkeys.DesktopIcon}】");
            sb_msg.AppendLine($"    4.隐藏/显示桌面：【{ConfigHelper.Instance.AppConfig.Hotkeys.Desktop}】");
            sb_msg.AppendLine();
            sb_msg.AppendLine("【作者】：LonelyAtom");
            sb_msg.AppendLine();
            sb_msg.AppendLine("【贡献者】：Wwwwtgd");

            MessageBox.Show(sb_msg.ToString(), "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region 发送通知
        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        private void SendNotification(string title, string text)
        {
            if (!tsm_closeNotice.Checked)
            {
                notifyIcon.BalloonTipTitle = title;
                notifyIcon.BalloonTipText = text;
                notifyIcon.ShowBalloonTip(2000);
            }
        }
        #endregion
    }
}
