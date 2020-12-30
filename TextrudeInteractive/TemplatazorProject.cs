using Engine.Application;

namespace TextrudeInteractive
{
    public class TemplatazorProject
    {
        public string ModelText { get; set; } = string.Empty;
        public string TemplateText { get; set; } = string.Empty;
        public string[] Definitions { get; set; } = new string[0];
        public ModelFormat DataFormat { get; set; } = ModelFormat.Auto;
    }
}