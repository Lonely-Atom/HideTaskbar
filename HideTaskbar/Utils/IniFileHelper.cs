namespace HideTaskbar.Utils
{
    public class IniFileHelper
    {
        private const string _FILE_PATH = "Settings.ini";

        public static string Select(string key)
        {
            string startPath = Application.StartupPath;

            List<string> lines = new(File.ReadAllLines(startPath + _FILE_PATH));

            foreach (string line in lines)
            {
                string data = line.Replace(" ", string.Empty);

                if (data.StartsWith($"{key}="))
                {
                    return data.Split('=')[1];
                }
            }

            return string.Empty;
        }

        public static void Update(string key, string value)
        {
            string startPath = Application.StartupPath;

            List<string> lines = new(File.ReadAllLines(startPath + _FILE_PATH));

            for(int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Replace(" ", string.Empty);

                if (line.StartsWith($"{key}="))
                {
                    lines[i] = $"{key}={value}";
                }
            }

            File.WriteAllLines(_FILE_PATH, lines);
        }
    }
}
