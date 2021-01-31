using System;

namespace TextrudeInteractive
{
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
    }
}
