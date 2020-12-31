using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Engine.Application
{
    public class ScriptLoader : ITemplateLoader
    {
        private readonly IFileSystemOperations _filesystem;

        public List<string> IncludePaths = new();

        public ScriptLoader(IFileSystemOperations filesystem) => _filesystem = filesystem;

        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            foreach (var i in IncludePaths)
            {
                var path = Path.Combine(i, templateName);
                if (_filesystem.Exists(path))
                    return path;
            }

            return templateName;
        }


        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
            => _filesystem.ReadAllText(templatePath);


        public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath) =>
            ValueTask.FromResult(Load(context, callerSpan, templatePath));

        public void AddIncludePath(string path)
        {
            IncludePaths.Add(path);
        }
    }
}