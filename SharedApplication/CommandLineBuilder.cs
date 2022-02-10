using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace SharedApplication;

/// <summary>
///     Builds command lines for the CLI version of textrude
/// </summary>
public class CommandLineBuilder
{
    private readonly StringBuilder _builder = new();
    private readonly RenderOptions _options;
    private string _exe = string.Empty;

    public CommandLineBuilder(RenderOptions op) => _options = op;

    public CommandLineBuilder WithExe(string exe)
    {
        _exe = exe;
        return this;
    }

    private string QuoteItem(string item)
        => item.Contains(" ") ? $"\"{item}\"" : item;

    private void AppendFlag(string flag) => _builder.Append($" --{flag.ToLowerInvariant()}");
    private void AppendItem(string item) => _builder.Append($" {QuoteItem(item)}");

    private void AppendIfPresent(string option, IEnumerable<string> items)
    {
        var clean = items.Clean();
        if (clean.Any())
        {
            AppendFlag(option);
            foreach (var i in clean)
            {
                AppendItem(i);
            }
        }
    }

    public string BuildRenderInvocation()
    {
        StartInvocation("render");
        if (_options.Lazy)
            AppendFlag(nameof(RenderOptions.Lazy));

        AppendIfPresent(nameof(RenderOptions.Models), _options.Models);

        AppendFlag(nameof(_options.Template));
        AppendItem(_options.Template);

        AppendIfPresent(nameof(RenderOptions.Definitions), _options.Definitions);
        AppendIfPresent(nameof(RenderOptions.Include), _options.Include);
        AppendIfPresent(nameof(RenderOptions.Output), _options.Output);
        return _builder.ToString();
    }

    private void StartInvocation(string cmd)
    {
        _builder.Clear();
        if (_exe.Length != 0)
            AppendItem(_exe);
        _builder.Append($" {cmd}");
    }

    public (string argsContent, string invocation) BuildJson(string argsFile)
    {
        StartInvocation("renderFromFile ");
        AppendFlag(nameof(RenderFromFileOptions.Arguments));
        AppendItem(argsFile);
        var invocation = _builder.ToString();

        return (
            JsonSerializer.Serialize(_options, new JsonSerializerOptions { WriteIndented = true }),
            invocation
        );
    }

    public (string argsContent, string invocation) BuildYaml(string argsFile)
    {
        StartInvocation("renderFromFile ");
        AppendFlag(nameof(RenderFromFileOptions.Arguments));
        AppendItem(argsFile);
        var invocation = _builder.ToString();

        return (new Serializer().Serialize(_options),
            invocation);
    }
}
