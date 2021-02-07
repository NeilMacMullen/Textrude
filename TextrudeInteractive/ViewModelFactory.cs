using System;
using System.Linq;
using Engine.Application;

namespace TextrudeInteractive
{
    public static class ViewModelFactory
    {
        public static EditPaneViewModel CreateOutput(OutputPaneModel f, int n) =>
            new EditPaneViewModel
            {
                Format = f.Format,
                LinkedPath = f.Path,
                ScribanName = $"output{(n == 0 ? "" : n)}",
                AvailableFormats = new MonacoResourceFetcher().GetSupportedFormats().ToArray(),
                PaneType = MonacoPaneType.PaneOutput,
                FileLinkage = FileLinkageTypes.Save
            };

        public static EditPaneViewModel CreateModel(ModelText model, int n)
        {
            var outputFormats = new MonacoResourceFetcher().GetSupportedFormats();
            var modelName = $"model{(n == 0 ? "" : n)}";
            return new EditPaneViewModel
            {
                Format = model.Format.ToString(),
                Text = model.Text,
                ScribanName = modelName,
                LinkedPath = model.Path,
                AvailableFormats = Enum.GetNames(typeof(ModelFormat)),
                PaneType = MonacoPaneType.PaneModel,
                FileLinkage = FileLinkageTypes.LoadSave
            };
        }

        public static EditPaneViewModel CreateDefinitions(string[] defs)
        {
            var defsd = DefinitionParser.CreateDefinitions(defs);
            var text =
                    @"
# Definitions can be entered one-per-line
# using yaml syntax.  E.g.
# DEF1:   def1-value        
".Trim() + (Environment.NewLine)
         + string.Join(Environment.NewLine, defsd.Select(dd => $"{dd.Key}: {dd.Value}"))
                ;
            var format = ModelFormat.Yaml.ToString();
            return new EditPaneViewModel
            {
                Format = format,
                Text = text,
                ScribanName = ScribanNamespaces.DefinitionsNamespace,
                LinkedPath = string.Empty,
                AvailableFormats = new[] {format},
                PaneType = MonacoPaneType.Definitions,
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
                PaneType = MonacoPaneType.IncludePaths,
                FileLinkage = FileLinkageTypes.None
            };
        }
    }

    [Flags]
    public enum FileLinkageTypes
    {
        None = 0,
        Load = (1 << 0),
        Save = (1 << 1),
        LoadSave = Load | Save
    }
}
