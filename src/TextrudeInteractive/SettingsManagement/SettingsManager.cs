using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Simple class to persist and retrieve application settings
    /// </summary>
    public static class SettingsManager
    {
        public static string GetPath()
        {
            var exe = Assembly.GetExecutingAssembly().GetName().Name;
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var settingsFile = Path.Combine(appData, exe, "settings.json");
            if (!File.Exists(settingsFile))
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFile));
            return settingsFile;
        }

        public static ApplicationSettings ReadSettings()
        {
            try
            {
                var text = File.ReadAllText(GetPath());
                return JsonSerializer.Deserialize<ApplicationSettings>(text);
            }
            catch
            {
                return new ApplicationSettings();
            }
        }

        public static void WriteSettings(ApplicationSettings settings)
        {
            try
            {
                var text = JsonSerializer.Serialize(settings, new JsonSerializerOptions {WriteIndented = true});
                File.WriteAllText(GetPath(), text);
            }
            catch
            {
            }
        }
    }
}
