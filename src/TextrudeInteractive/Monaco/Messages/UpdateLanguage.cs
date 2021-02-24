﻿// ReSharper disable UnusedMember.Global

namespace TextrudeInteractive.Monaco.Messages
{
    /// <summary>
    ///     Sent TO Monaco tell it to change syntax highlighting
    /// </summary>
    public record UpdateLanguage : MonacoMessages
    {
        public UpdateLanguage(string language) => Language = language.ToLowerInvariant();

        public string Language { get; }
    }


    public record CompletionNode(string name, string description, CompletionType kind)
    {
    }

    public record Completions(CompletionNode[] helpers);

    public enum CompletionType
    {
        Method = 0,
        Function = 1,
        Constructor = 2,
        Field = 3,
        Variable = 4,
        Class = 5,
        Struct = 6,
        Interface = 7,
        Module = 8,
        Property = 9,
        Event = 10,
        Operator = 11,
        Unit = 12,
        Value = 13,
        Constant = 14,
        Enum = 15,
        EnumMember = 16,
        Keyword = 17,
        Text = 18,
        Color = 19,
        File = 20,
        Reference = 21,
        Customcolor = 22,
        Folder = 23,
        TypeParameter = 24,
        User = 25,
        Issue = 26,
        Snippet = 27
    }
}
