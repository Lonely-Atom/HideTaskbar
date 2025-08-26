using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace HideTaskbar.Utils
{
    #region 配置文件帮助类
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public class ConfigHelper
    {
        private static readonly ConfigHelper _instance = new();

        public static ConfigHelper Instance => _instance;

        public AppConfigModel AppConfig { get; set; } = default!;

        #region 读取配置内容并绑定到实体
        /// <summary>
        /// 读取配置内容并绑定到实体
        /// </summary>
        private ConfigHelper()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            // 将配置绑定到实体
            AppConfig = configuration.GetSection("AppConfig").Get<AppConfigModel>() ?? default!;
        }
        #endregion

        #region 更新配置文件
        /// <summary>
        /// 更新配置文件
        /// </summary>
        public void UpdateConfig()
        {
            string jsonContent = JsonConvert.SerializeObject(new { AppConfig }, Formatting.Indented);
            File.WriteAllText("appsettings.json", jsonContent);
        }
        #endregion
    }
    #endregion

    #region 配置项实体
    public class AppConfigModel
    {
        public ConfigsModel Configs { get; set; } = default!;
        public HotkeysModel Hotkeys { get; set; } = default!;
    }

    public class ConfigsModel
    {
        public bool FirstShowAbout { get; set; } = true;
        public bool AutoHideTaskbar { get; set; } = false;
        public bool AutoHideDesktopIcon { get; set; } = false;
        public bool AutoHideDesktop { get; set; } = false;
        public bool AutoStart { get; set; } = false;
        public bool CloseNotice { get; set; } = false;
    }

    public class HotkeysModel
    {
        public string TaskBar { get; set; } = "Ctrl+Shift+~";
        public string Tray { get; set; } = "Ctrl+Alt+~";
        public string DesktopIcon { get; set; } = "Ctrl+Shift+1";
        public string Desktop { get; set; } = "Ctrl+Alt+1";
    }
    #endregion
}
