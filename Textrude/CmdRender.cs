using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Engine.Application;

namespace Textrude
{
    public class CmdRender
    {
        public static void Run(Options options, IFileSystemOperations filesystem)
        {
            var outputShouldBeChecked = !string.IsNullOrWhiteSpace(options.Output) && filesystem.Exists(options.Output);
            if (outputShouldBeChecked)
            {
                var modelDates = options.Models
                    .Select(m => Helpers.TryOrQuit(() => filesystem.GetLastWriteTimeUtc(m),
                        $"Unable to open model file {m}"))
                    .ToArray();
                var modelLastWritten = modelDates.Any() ? modelDates.Max() : DateTime.MaxValue;

                var templateLastWritten = Helpers.TryOrQuit(() => filesystem.GetLastWriteTimeUtc(options.Template),
                    $"Unable to open template file {options.Template}");
                var outputLastWritten = Helpers.TryOrQuit(() => filesystem.GetLastWriteTimeUtc(options.Output),
                    $"Unable to open output file {options.Output}");

                //check if there is nothing to do...
                if (outputLastWritten > templateLastWritten && outputLastWritten > modelLastWritten)
                    return;
            }

            var template = Helpers.TryOrQuit(() => filesystem.ReadAllText(options.Template),
                $"Unable to read template file {options.Template}");

            var engine = new ApplicationEngine(filesystem)
                .WithDefinitions(options.Definitions)
                .WithTemplate(template);

            foreach (var modelPath in options.Models)
            {
                var modelText = Helpers.TryOrQuit(() => filesystem.ReadAllText(modelPath),
                    $"Unable to read model file {modelPath}");

                var format = ModelDeserializerFactory.FormatFromExtension(modelPath);
                engine = engine.WithModel(modelText, format);
            }

            if (!options.HideEnvironment)
                engine = engine.WithEnvironmentVariables();

            engine.Render();

            foreach (var engineError in engine.Errors)
            {
                Console.Error.WriteLine(engineError);
            }

            if (engine.HasErrors)
                Helpers.TryOrQuit<int>(() => throw new ApplicationException(), "");

            var output = engine.Output;
            if (!string.IsNullOrWhiteSpace(options.Output))
                filesystem.WriteAllText(options.Output, output);
            else Console.WriteLine(output);
        }

        [Verb("render")]
        public class Options
        {
            [Option(Required = true)] public IEnumerable<string> Models { get; set; } = Enumerable.Empty<string>();

            [Option(Required = true)] public string Template { get; set; } = string.Empty;

            [Option] public string Output { get; set; } = string.Empty;

            [Option] public IEnumerable<string> Definitions { get; set; } = Enumerable.Empty<string>();

            [Option] public bool HideEnvironment { get; set; }
        }
    }
}