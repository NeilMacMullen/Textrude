using System;

namespace TextrudeInteractive.SettingsManagement;

/// <summary>
///     Simple DTO to allow serialisation of project settings
/// </summary>
public class ApplicationSettings
{
    /// <summary>
    ///     Format version
    /// </summary>
    public int Version { get; set; } = 1;

    public RecentlyUsedProject[] RecentProjects { get; set; } = Array.Empty<RecentlyUsedProject>();
    public double FontSize { get; set; } = 12;
    public bool LineNumbersOn { get; set; } = true;
    public bool WrapText { get; set; }

    /// <summary>
    ///     Time between last keystroke and render
    /// </summary>
    public int ResponseTime { get; set; } = 50;

    public bool ShowWhitespace { get; set; }
}
