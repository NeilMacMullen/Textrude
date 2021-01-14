using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Tests
{
    /// <summary>
    ///     Simple TemplateLoader that returns a constant string no matter what file is requested
    /// </summary>
    internal class MockTemplateLoader : ITemplateLoader
    {
        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
            => templateName;

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
            => "some text";

        public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
            => ValueTask.FromResult(Load(context, callerSpan, templatePath));
    }
}