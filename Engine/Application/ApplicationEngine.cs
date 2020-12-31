using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Scriban.Runtime;

namespace Engine.Application
{
    public class ApplicationEngine
    {
        private readonly TemplateManager _templateManager = new();
        private int _modelCount;

        public ImmutableArray<string> Errors = ImmutableArray<string>.Empty;

        public string Output { get; private set; } = string.Empty;
        public bool HasErrors => Errors.Any();

        public ApplicationEngine WithModel(string modelText, ModelFormat format)
        {
            try
            {
                var serializer = ModelDeserializerFactory.Fetch(format);
                var model = serializer.Deserialize(modelText);

                //Note that models are added as model0, model1 etc but for
                //convenience, "model0" is also available as "model"
                if (_modelCount == 0)
                    _templateManager.AddVariable("model", model.Untyped);
                _templateManager.AddVariable($"model{_modelCount}", model.Untyped);
                _modelCount++;
            }
            catch (Exception e)
            {
                Errors = Errors.Add($"Model error: {e.Message}");
            }

            return this;
        }

        public ApplicationEngine WithEnvironmentVariables()
        {
            var environmentVariables =
                Environment.GetEnvironmentVariables().Cast<DictionaryEntry>()
                    .ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
            _templateManager.AddVariable("env", environmentVariables);
            return this;
        }

        public ApplicationEngine WithDefinitions(IEnumerable<string> rawDefinitions)
        {
            try
            {
                var definitions = DefinitionParser.CreateDefinitions(rawDefinitions);
                _templateManager.AddVariable("def", definitions);
            }
            catch (ArgumentException e)
            {
                Errors = Errors.Add($"Definition error: {e.Message}");
            }

            return this;
        }

        public ApplicationEngine WithTemplate(string templateText)
        {
            _templateManager.SetTemplate(templateText);
            Errors = Errors.AddRange(_templateManager.ErrorList);
            return this;
        }

        public ApplicationEngine Render()
        {
            try
            {
                Output = _templateManager.Render();
            }
            catch (Exception e)
            {
                Errors = Errors.Add($"Render error: {e.Message}");
            }

            return this;
        }

        public ApplicationEngine WithHelpers()
        {
            var scriptObject1 = new ScriptObject();
            scriptObject1.Import(typeof(MyFunctions));
            _templateManager.AddVariable("helpers", scriptObject1);
            return this;
        }

        public string[] GetOutput(int i)
        {
            return _templateManager.GetOutput(i);
        }
    }


    public static class MyFunctions
    {
        public static string Dump(object o)
        {
            var serialiser = ModelDeserializerFactory.Fetch(ModelFormat.Yaml);
            var text = serialiser.Serialize(o);
            return text;
        }
    }
}