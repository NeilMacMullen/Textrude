using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace TextrudeInteractive.SettingsManagement;

/// <summary>
///     Simple class to persist and retrieve application settings
/// </summary>
public static class SettingsManager
{
    public static string GetSettingsPath()
    {
        var app = GetTextrudeAppDataFolder();
        var settingsFile = Path.Combine(app, "settings.json");
        return settingsFile;
    }

    public static string GetTextrudeAppDataFolder()
    {
        var exe = Assembly.GetExecutingAssembly().GetName().Name;
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var textrudeData = Path.Combine(appData, exe);
        if (!Directory.Exists(textrudeData))
            Directory.CreateDirectory(textrudeData);
        return textrudeData;
    }

    public static ApplicationSettings ReadSettings()
    {
        try
        {
            var text = File.ReadAllText(GetSettingsPath());
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
            var text = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(GetSettingsPath(), text);
        }
        catch
        {
        }
    }
}
