using System;

namespace TextrudeInteractive.SettingsManagement;

public class RecentlyUsedProject
{
    public string Path { get; set; } = string.Empty;
    public DateTime LastLoaded { get; set; }
}
