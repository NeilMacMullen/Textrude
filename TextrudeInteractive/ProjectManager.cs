using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace TextrudeInteractive
{
    public class ProjectManager
    {
        private const string Filter = "project files (*.texproj)|*.texproj|All files (*.*)|*.*";
        private readonly MainWindow _owner;

        private string _currentProjectPath = string.Empty;

        public ProjectManager(MainWindow owner) => _owner = owner;


        private TemplatazorProject CreateProject()
        {
            var g = _owner.CollectInput();
            var proj = new TemplatazorProject
            {
                EngineInput = g
            };

            return proj;
        }

        public void LoadProject()
        {
            var dlg = new OpenFileDialog {Filter = Filter};

            if (dlg.ShowDialog(_owner) == true)
            {
                var text = File.ReadAllText(dlg.FileName);
                var proj = JsonConvert.DeserializeObject<TemplatazorProject>(text);
                _owner.SetUI(proj.EngineInput);
                _currentProjectPath = dlg.FileName;
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
            var dlg = new SaveFileDialog {Filter = Filter};
            if (dlg.ShowDialog(_owner) == true)
            {
                _currentProjectPath = dlg.FileName;
                SaveProject();
            }
        }
    }
}