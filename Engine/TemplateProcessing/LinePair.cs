namespace Engine.TemplateProcessing;

/// <summary>
///     Represents a consecutive pair of lines
/// </summary>
/// <remarks>
///     LinePairs are primarily used when hoisting pipes
/// </remarks>
public record LinePair(string First, string Second);
