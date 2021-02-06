using System.Linq;
using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace SharedApplication
{
    public class CommandLineBuilder
    {
        private readonly StringBuilder builder = new();
        private readonly RenderOptions options;
        private string _exe = string.Empty;

        public CommandLineBuilder(RenderOptions theoptions) => options = theoptions;

        public CommandLineBuilder WithExe(string exe)
        {
            _exe = exe;
            return this;
        }

        private void AppendFlag(string flag) => builder.Append($" --{flag.ToLowerInvariant()}");
        private void AppendItem(string item) => builder.Append($" \"{item}\"");
        private void AppendFileName(string file) => AppendItem(file);

        public string BuildRenderInvocation()
        {
            StartInvocation("render");
            if (options.Lazy)
                AppendFlag(nameof(RenderOptions.Lazy));

            if (options.Models.Any())
            {
                AppendFlag(nameof(RenderOptions.Models));
                foreach (var model in options.Models)
                {
                    AppendFileName(model);
                }
            }

            AppendFlag(nameof(options.Template));
            AppendFileName("template.sbn");


            if (options.Definitions.Any())
            {
                AppendFlag(nameof(RenderOptions.Definitions));
                foreach (var d in options.Definitions)
                    AppendItem(d);
            }

            if (options.Include.Any())
            {
                AppendFlag(nameof(RenderOptions.Include));
                foreach (var inc in options.Include)
                    AppendFileName(inc);
            }


            if (options.Output.Any())
            {
                AppendFlag(nameof(RenderOptions.Output));
                foreach (var outfile in options.Output)
                    AppendFileName(outfile);
            }

            return builder.ToString();
        }

        private void StartInvocation(string cmd)
        {
            builder.Clear();
            if (_exe.Length != 0)
                AppendFileName(_exe);
            builder.Append($" {cmd}");
        }

        public (string argsContent, string invocation) BuildJson(string argsFile)
        {
            StartInvocation("renderFromFile ");
            AppendFlag(nameof(RenderFromFileOptions.Arguments));
            AppendFileName(argsFile);
            var invocation = builder.ToString();

            return (
                JsonSerializer.Serialize(options, new JsonSerializerOptions {WriteIndented = true}),
                invocation
            );
        }

        public (string argsContent, string invocation) BuildYaml(string argsFile)
        {
            StartInvocation("renderFromFile ");
            AppendFlag(nameof(RenderFromFileOptions.Arguments));
            AppendFileName(argsFile);
            var invocation = builder.ToString();

            return (new Serializer().Serialize(options),
                invocation);
        }
    }
}
