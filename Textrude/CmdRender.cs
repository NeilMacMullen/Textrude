using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Application;
using SharedApplication;

namespace Textrude
{
    /// <summary>
    ///     Renders a template based on the supplied parameters
    /// </summary>
    public class CmdRender
    {
        private readonly RenderOptions _options;
        private readonly RunTimeEnvironment _runtime;
        private readonly Helpers _sys;

        public CmdRender(RenderOptions options, RunTimeEnvironment rte, Helpers sys)
        {
            _options = options;
            _runtime = rte;
            _sys = sys;
        }

        private DateTime[] GetLastWrittenDates(IEnumerable<string> files, DateTime fallback, bool allowNotExist)
        {
            DateTime GetLastWriteTime(string file)
            {
                if (allowNotExist && !_runtime.FileSystem.Exists(file))
                    return fallback;

                return _runtime.FileSystem.GetLastWriteTimeUtc(file);
            }

            var f = files
                .Select(f =>
                    _sys.GetOrQuit(() => GetLastWriteTime(f), $"Missing/inaccessible file '{f}'")
                )
                .ToArray();
            //ensure we always have at least one date in the set
            if (!f.Any())
                f = f.Append(fallback).ToArray();
            return f;
        }

        private string TryReadFile(string path)
        {
            return _sys.GetOrQuit(() => _runtime.FileSystem.ReadAllText(path),
                $"Unable to read file {_options.Template}");
        }

        public void Run()
        {
            var lastModelDate = GetLastWrittenDates(_options.Models, DateTime.MinValue, false).Max();
            var earliestOutputDate = GetLastWrittenDates(_options.Output, DateTime.MinValue, true).Min();
            var templateDate = GetLastWrittenDates(new[] {_options.Template}, DateTime.MinValue, false).Max();

            var lastInputDate = lastModelDate > templateDate ? lastModelDate : templateDate;


            //check if there is nothing to do...
            if (_options.Lazy && (lastInputDate < earliestOutputDate))
                return;


            var template = TryReadFile(_options.Template);

            var engine = new ApplicationEngine(_runtime)
                .WithHelpers()
                .WithDefinitions(_options.Definitions)
                .WithTemplate(template);

            foreach (var modelPath in _options.Models)
            {
                var modelText = TryReadFile(modelPath);
                var format = ModelDeserializerFactory.FormatFromExtension(modelPath);
                engine = engine.WithModel(modelText, format);
            }

            engine = engine.WithIncludePaths(_options.Include);

            if (!_options.HideEnvironment)
                engine = engine.WithEnvironmentVariables();

            engine.Render();

            foreach (var engineError in engine.Errors)
            {
                Console.Error.WriteLine(engineError);
            }

            if (engine.HasErrors)
                _sys.GetOrQuit<int>(() => throw new ApplicationException(), "");

            var outputFiles = _options.Output.ToArray();
            var outputFileCount = outputFiles.Length;
            if (outputFileCount != 0)
            {
                var outputs = engine.GetOutput(outputFileCount);
                for (var i = 0; i < outputFileCount; i++)
                {
                    var outputFile = outputFiles[i];
                    var text = outputs[i];
                    _sys.TryOrQuit(() => _runtime.FileSystem.WriteAllText(outputFile, text),
                        $"Unable to write output to {outputFile}");
                }
            }
            else
            {
                var outputs = engine.GetOutput(10);
                for (var i = 0; i < outputs.Length; i++)
                {
                    var text = outputs[i];
                    if (text.Length == 0)
                        continue;
                    Console.WriteLine($"----------------- output{i} -------------------");
                    Console.WriteLine(text);
                }
            }
        }

        public static void Run(RenderOptions options, RunTimeEnvironment rte, Helpers sys)
        {
            var cmd = new CmdRender(options, rte, sys);
            cmd.Run();
        }
    }
}