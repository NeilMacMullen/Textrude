using System;

namespace TextrudeInteractive.SettingsManagement;

/// <summary>
///     Defines the format of the Snippet file.
/// </summary>
/// <remarks>
///     Note that currently on the Scriban array is read.  In future, it might
///     make sense to allow snippets to be loaded for input models- possibly even on a project-by-project
///     basis
/// </remarks>
public record SnippetFile
{
    public Snippet[] Scriban { get; init; } = Array.Empty<Snippet>();
    public Snippet[] Csv { get; init; } = Array.Empty<Snippet>();
    public Snippet[] Json { get; init; } = Array.Empty<Snippet>();
    public Snippet[] Yaml { get; init; } = Array.Empty<Snippet>();
}
