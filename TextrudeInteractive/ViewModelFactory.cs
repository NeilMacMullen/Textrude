using System;
using System.Linq;
using Engine.Application;
using Engine.Model;
using SharedApplication;

namespace TextrudeInteractive
{
    public static class ViewModelFactory
    {
        public static EditPaneViewModel CreateOutput(OutputPaneModel f, int n)
        {
            var outputName =
                f.Name.Length != 0
                    ? f.Name
                    : NameProvider.IndexedOutput(n);
            return new EditPaneViewModel
            {
                Format = f.Format,
                LinkedPath = f.Path,
                ScribanName = outputName,
                AvailableFormats = MonacoResourceFetcher.Instance.GetSupportedFormats().ToArray(),
                PaneType = PaneType.Output,
                FileLinkage = FileLinkageTypes.SaveAndClipboard
            };
        }

        public static EditPaneViewModel CreateModel(ModelText model, int n)
        {
            var modelName =
                model.Name.Length != 0
                    ? model.Name
                    : NameProvider.IndexedModel(n);
            return new EditPaneViewModel
            {
                Format = model.Format.ToString(),
                Text = model.Text,
                ScribanName = modelName,
                LinkedPath = model.Path,
                AvailableFormats = Enum.GetNames(typeof(ModelFormat))
                    .Where(f => f != ModelFormat.Unknown.ToString()).ToArray(),
                PaneType = PaneType.Model,
                FileLinkage = FileLinkageTypes.LoadSave
            };
        }

        public static EditPaneViewModel CreateTemplate(string text, string linkedPath)
        {
            var format = "scriban";
            return new EditPaneViewModel
            {
                Format = format,
                Text = text,
                ScribanName = "template",
                LinkedPath = linkedPath,
                AvailableFormats = new[] {format},
                PaneType = PaneType.Template,
                FileLinkage = FileLinkageTypes.LoadSave
            };
        }


        public static EditPaneViewModel CreateDefinitions(string[] defs)
        {
            var defsd = DefinitionParser.CreateDefinitions(defs);

            var text =
                    @"
# Definitions can be entered one-per-line
# E.g.
# DEF1=def1-value
# NAME=value
".Trim() + (Environment.NewLine)
         + string.Join(Environment.NewLine, defsd.Select(dd => $"{dd.Key}={dd.Value}"))
                ;
            var format = ModelFormat.Yaml.ToString();
            return new EditPaneViewModel
            {
                Format = format,
                Text = text,
                ScribanName = ScribanNamespaces.DefinitionsNamespace,
                LinkedPath = string.Empty,
                AvailableFormats = new[] {format},
                PaneType = PaneType.Definitions,
                FileLinkage = FileLinkageTypes.None
            };
        }


        public static EditPaneViewModel CreateIncludePaths(string[] incs)
        {
            var text =
                @"
# Additional include paths can be entered one-per-line
# E.g.
# d:\work\mylib\usefulfuncs.sban        
".Trim() + (Environment.NewLine)
         + string.Join(Environment.NewLine, incs);
            //use yaml for colouring
            var format = ModelFormat.Yaml.ToString();
            return new EditPaneViewModel
            {
                Format = format,
                Text = text,
                ScribanName = "inc",
                LinkedPath = string.Empty,
                AvailableFormats = new[] {format},
                PaneType = PaneType.IncludePaths,
                FileLinkage = FileLinkageTypes.None
            };
        }
    }
}
