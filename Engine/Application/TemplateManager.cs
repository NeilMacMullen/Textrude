using System.Collections.Immutable;
using System.Linq;
using Scriban;
using Scriban.Runtime;

namespace Engine.Application
{
    public class TemplateManager
    {
        private readonly TemplateContext _context = new();
        private readonly ScriptObject _top = new();

        private Template _compiledTemplate;

        public TemplateManager()
        {
            _context.StrictVariables = true;
            _context.PushGlobal(_top);
        }

        public ImmutableArray<string> ErrorList { get; private set; } = ImmutableArray<string>.Empty;

        public string Render()
        {
            if (_compiledTemplate.HasErrors) return string.Empty;

            var ret = _compiledTemplate.Render(_context);
            if (_compiledTemplate.HasErrors)
                ErrorList = ErrorList
                    .Concat(_compiledTemplate.Messages.Select(e => $"ERROR: {e}"))
                    .ToImmutableArray();

            return ret;
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
    }
}