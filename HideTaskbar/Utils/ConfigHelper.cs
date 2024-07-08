using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HideTaskbar.Utils
{
    public class ConfigHelper
    {
        private static readonly ConfigHelper _instance = new();

        public static ConfigHelper Instance => _instance;

        public AppConfigModel AppConfig { get; set; } = default!;

        private ConfigHelper()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            // 将配置绑定到实体
            AppConfig = configuration.GetSection("AppConfig").Get<AppConfigModel>() ?? default!;
        }

        public void UpdateConfig()
        {
            string jsonContent = JsonConvert.SerializeObject(new { AppConfig }, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("appsettings.json", jsonContent);
        }
    }

    public class AppConfigModel
    {
        public ConfigsModel Configs { get; set; } = default!;
        public HotkeysModel Hotkeys { get; set; } = default!;
    }

    public class ConfigsModel
    {
        public bool FirstShowAbout { get; set; } = true;
        public bool AutoHide { get; set; } = false;
        public bool AutoStart { get; set; } = false;
        public bool CloseNotice { get; set; } = false;
    }

    public class HotkeysModel
    {
        public string TaskBar { get; set; } = "Ctrl+Shift+~";
        public string Tray { get; set; } = "Ctrl+Alt+~";
    }
}
