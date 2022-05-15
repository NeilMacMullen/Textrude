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
    public static string SettingsPath { get; } = GetPath("settings");
    public static string SnippetsPath { get; } = GetPath("snippets");


    public static string GetPath(string t)
        => Path.Combine(GetTextrudeAppDataFolder(), $"{t}.json");

    public static string GetTextrudeAppDataFolder()
    {
        var exe = Assembly.GetExecutingAssembly().GetName().Name;
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var textrudeData = Path.Combine(appData, exe);
        if (!Directory.Exists(textrudeData))
            Directory.CreateDirectory(textrudeData);
        return textrudeData;
    }

    private static T ReadFile<T>(string path)
        where T : new()
    {
        try
        {
            var text = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var o = JsonSerializer.Deserialize<T>(text, options);
            return o;
        }
        catch
        {
            return new T();
        }
    }

    public static ApplicationSettings ReadSettings()
        => ReadFile<ApplicationSettings>(SettingsPath);

    public static SnippetFile ReadSnippets()
        => ReadFile<SnippetFile>(SnippetsPath);


    private static void WriteFile<T>(string path, T obj)
    {
        try
        {
            var text = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, text);
        }
        catch
        {
        }
    }

    public static void WriteSettings(ApplicationSettings settings)
        => WriteFile(SettingsPath, settings);
}
