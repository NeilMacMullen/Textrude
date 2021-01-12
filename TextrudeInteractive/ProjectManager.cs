using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;

namespace TextrudeInteractive
{
    public class ProjectManager
    {
        private const string Filter = "project files (*.texproj)|*.texproj|All files (*.*)|*.*";
        private readonly MainWindow _owner;

        private string _currentProjectPath = string.Empty;

        public ProjectManager(MainWindow owner) => _owner = owner;


        private TextrudeProject CreateProject()
        {
            var g = _owner.CollectInput();
            var proj = new TextrudeProject
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
                try
                {
                    var text = File.ReadAllText(dlg.FileName);
                    var proj = JsonSerializer.Deserialize<TextrudeProject>(text);
                    _currentProjectPath = dlg.FileName;
                    UpdateUI(proj);
                }
                catch (Exception e)
                {
                    MessageBox.Show(_owner, "Error - unable to open project");
                }
            }
        }

        public void SaveProject()
        {
            if (string.IsNullOrWhiteSpace(_currentProjectPath))
                SaveProjectAs();
            else
            {
                try
                {
                    var o = new JsonSerializerOptions {WriteIndented = true};
                    var text = JsonSerializer.Serialize(CreateProject(), o);

                    File.WriteAllText(_currentProjectPath, text);
                }
                catch
                {
                    MessageBox.Show(_owner, "Error - unable to save project");
                }
            }
        }

        public void SaveProjectAs()
        {
            var dlg = new SaveFileDialog {Filter = Filter};
            if (dlg.ShowDialog(_owner) == true)
            {
                _currentProjectPath = dlg.FileName;
                _owner.SetTitle(_currentProjectPath);
                SaveProject();
            }
        }

        public void NewProject()
        {
            _currentProjectPath = string.Empty;
            var proj = new TextrudeProject();
            UpdateUI(proj);
        }

        private void UpdateUI(TextrudeProject project)
        {
            _owner.SetUI(project.EngineInput);

            _owner.SetTitle(_currentProjectPath);
        }
    }
}