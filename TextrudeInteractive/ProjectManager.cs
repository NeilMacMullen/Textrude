using System;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace TextrudeInteractive
{
    public class ProjectManager
    {
        private readonly MainWindow _owner;

        private string _currentProjectPath = string.Empty;

        public ProjectManager(MainWindow owner) => _owner = owner;


        private TemplatazorProject CreateProject()
        {
            var g = _owner.CollectInput();
            var proj = new TemplatazorProject
            {
                DataFormat = g.Format,
                Definitions = g.Definitions,
                ModelText = g.ModelText,
                TemplateText = g.Template
            };
            return proj;
        }

        public void LoadProject()
        {
            var dlg = new OpenFileDialog();

            if (dlg.ShowDialog(_owner) == true)
            {
                var text = File.ReadAllText(dlg.FileName);
                var proj = JsonConvert.DeserializeObject<TemplatazorProject>(text);
                var gi = new GenInput(proj.TemplateText, proj.ModelText, proj.DataFormat,
                    string.Join(Environment.NewLine, proj.Definitions));
                _owner.SetUI(gi);
            }
        }

        public void SaveProject()
        {
            if (string.IsNullOrWhiteSpace(_currentProjectPath))
                SaveProjectAs();
            else
            {
                var text = JsonConvert.SerializeObject(CreateProject(), Formatting.Indented);
                File.WriteAllText(_currentProjectPath, text);
            }
        }

        public void SaveProjectAs()
        {
            var dlg = new SaveFileDialog();
            if (dlg.ShowDialog(_owner) == true)
            {
                _currentProjectPath = dlg.FileName;
                SaveProject();
            }
        }
    }
}