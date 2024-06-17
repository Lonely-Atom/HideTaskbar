namespace HideTaskbar
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            notifyIcon = new NotifyIcon(components);
            timer = new System.Windows.Forms.Timer(components);
            contextMenuStrip = new ContextMenuStrip(components);
            tsm_hideOrShow = new ToolStripMenuItem();
            tsm_hideOrShowTray = new ToolStripMenuItem();
            tsm_autoHide = new ToolStripMenuItem();
            tsm_autoStart = new ToolStripMenuItem();
            tsm_closeNotice = new ToolStripMenuItem();
            tsm_about = new ToolStripMenuItem();
            tsm_exit = new ToolStripMenuItem();
            contextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "隐藏任务栏小工具";
            notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            // 
            // timer
            // 
            timer.Enabled = true;
            timer.Interval = 300;
            timer.Tick += timer_Tick;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { tsm_hideOrShow, tsm_hideOrShowTray, tsm_autoHide, tsm_autoStart, tsm_closeNotice, tsm_about, tsm_exit });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(197, 114);
            // 
            // tsm_hideOrShow
            // 
            tsm_hideOrShow.Name = "tsm_hideOrShow";
            tsm_hideOrShow.Size = new Size(196, 22);
            tsm_hideOrShow.Text = "隐藏任务栏 (Ctrl + ~)";
            tsm_hideOrShow.Click += tsm_hideOrShow_Click;
            // 
            // tsm_hideOrShowTray
            // 
            tsm_hideOrShowTray.Name = "tsm_hideOrShowTray";
            tsm_hideOrShowTray.Size = new Size(196, 22);
            tsm_hideOrShowTray.Text = "隐藏系统托盘 (Alt + 1)";
            tsm_hideOrShowTray.Click += tsm_hideOrShowTray_Click;
            // 
            // tsm_autoHide
            // 
            tsm_autoHide.Name = "tsm_autoHide";
            tsm_autoHide.Size = new Size(196, 22);
            tsm_autoHide.Text = "启动后自动隐藏任务栏";
            tsm_autoHide.Click += tsm_autoHide_Click;
            // 
            // tsm_autoStart
            // 
            tsm_autoStart.Name = "tsm_autoStart";
            tsm_autoStart.Size = new Size(196, 22);
            tsm_autoStart.Text = "开机自启动";
            tsm_autoStart.Click += tsm_autoStart_Click;
            // 
            // tsm_closeNotice
            // 
            tsm_closeNotice.Name = "tsm_closeNotice";
            tsm_closeNotice.Size = new Size(196, 22);
            tsm_closeNotice.Text = "关闭通知";
            tsm_closeNotice.Click += tsm_closeNotice_Click;
            // 
            // tsm_about
            // 
            tsm_about.Name = "tsm_about";
            tsm_about.Size = new Size(196, 22);
            tsm_about.Text = "关于";
            tsm_about.Click += tsm_about_Click;
            // 
            // tsm_exit
            // 
            tsm_exit.Name = "tsm_exit";
            tsm_exit.Size = new Size(196, 22);
            tsm_exit.Text = "退出";
            tsm_exit.Click += tsm_exit_Click;
            // 
            // MainForm
            // 
            ClientSize = new Size(0, 0);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            ShowInTaskbar = false;
            Text = "隐藏任务栏小工具";
            WindowState = FormWindowState.Minimized;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer timer;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem tsm_hideOrShow;
        private ToolStripMenuItem tsm_hideOrShowTray;
        private ToolStripMenuItem tsm_autoHide;
        private ToolStripMenuItem tsm_autoStart;
        private ToolStripMenuItem tsm_closeNotice;
        private ToolStripMenuItem tsm_about;
        private ToolStripMenuItem tsm_exit;
    }
}
