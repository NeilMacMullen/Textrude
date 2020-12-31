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
        private readonly ScriptObject _top = new();

        private Template _compiledTemplate;
        private string _output = string.Empty;

        public TemplateManager()
        {
            _context.StrictVariables = true;
            _context.PushGlobal(_top);
        }

        public ImmutableArray<string> ErrorList { get; private set; } = ImmutableArray<string>.Empty;

        public string Render()
        {
            if (_compiledTemplate.HasErrors) return string.Empty;

            _output = _compiledTemplate.Render(_context);
            if (_compiledTemplate.HasErrors)
                ErrorList = ErrorList
                    .Concat(_compiledTemplate.Messages.Select(e => $"ERROR: {e}"))
                    .ToImmutableArray();

            return _output;
        }

        public void SetTemplate(string templateText)
        {
            _compiledTemplate = Template.Parse(templateText);
            ErrorList = _compiledTemplate
                .Messages
                .Select(e => $"ERROR: {e}")
                .ToImmutableArray();
        }


        public void AddVariable(string env, object val)
        {
            _top.Add(env, val);
        }

        public string[] GetOutput(int n)
        {
            var r = new string[n];
            for (var i = 0; i < n; i++)
            {
                if (i == 0)
                    r[i] = _output;
                else
                {
                    var t = _context.GetValue(new ScriptVariableGlobal($"file{i}"));
                    r[i] = t?.ToString() ?? string.Empty;
                }
            }

            return r;
        }
    }
}