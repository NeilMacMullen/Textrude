using System;
using System.Collections.Immutable;
using System.Linq;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace Engine.Application
{
    public class TemplateManager
    {
        private readonly TemplateContext _context = new();
        private readonly ScriptLoader _scriptLoader;
        private readonly ScriptObject _top = new();

        private Template _compiledTemplate;
        private string _output = string.Empty;

        public TemplateManager(IFileSystemOperations ops)
        {
            //TODO - disabled until bug-fix in Scriban
            //_context.StrictVariables = true;
            _scriptLoader = new ScriptLoader(ops);
            _context.TemplateLoader = _scriptLoader;
            _context.PushGlobal(_top);
        }

        public ImmutableArray<string> ErrorList { get; private set; } = ImmutableArray<string>.Empty;

        public string Render()
        {
            if (ErrorList.Any())
                return string.Empty;


            _output = _compiledTemplate.Render(_context);
            if (_compiledTemplate.HasErrors)
                ErrorList = ErrorList
                    .Concat(_compiledTemplate.Messages.Select(e => $"ERROR: {e}"))
                    .ToImmutableArray();

            return _output;
        }

        public void SetTemplate(string templateText)
        {
            try
            {
                _compiledTemplate = Template.Parse(templateText);

                ErrorList = _compiledTemplate
                    .Messages
                    .Select(e => $"ERROR: {e}")
                    .ToImmutableArray();
            }
            catch (Exception e)
            {
                ErrorList = ErrorList.Add($"SCRIBAN INTERNAL EXCEPTION: {e}");
            }
        }


        public void AddVariable(string env, object val)
        {
            _top.Add(env, val);
        }

        public void AddIncludePath(string path)
        {
            _scriptLoader.AddIncludePath(path);
        }

        public string TryGetString(string variableName)
        {
            var scribanVariable = new ScriptVariableGlobal(variableName);
            try
            {
                return _context.GetValue(scribanVariable)?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}