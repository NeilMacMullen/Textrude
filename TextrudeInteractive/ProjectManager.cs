using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using Engine.Application;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using SharedApplication;

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

        public void ExportProject()
        {
            var dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog(_owner) == false)
                return;
            try
            {
                var folder = dlg.SelectedPath;

                var engine = CreateProject().EngineInput;
                var options = new RenderOptions {Definitions = engine.Definitions, Include = engine.IncludePaths};


                void WriteToFile(string fileName, string content)
                    => File.WriteAllText(Path.Combine(folder, fileName), content);

                for (var i = 0; i < engine.Models.Length; i++)
                {
                    var m = engine.Models[i];
                    if (m.Text.Trim().Length == 0)
                    {
                        continue;
                    }

                    var mName = Path.ChangeExtension($"model{i}", m.Format.ToString());
                    WriteToFile(mName, m.Text);
                    options.Models = options.Models.Append(mName).ToArray();
                }

                var templateName = "template.sbn";
                WriteToFile(templateName, engine.Template);
                options.Template = templateName;

                var exe =
                    Path.Combine(new RunTimeEnvironment(new FileSystemOperations()).ApplicationFolder(),
                        "textrude.exe");

                var builder = new CommandLineBuilder(options).WithExe(exe);

                WriteToFile("render.bat", $"{builder.BuildRenderInvocation()}");


                //write yaml invocation

                var jsonArgs = "args.json";
                var (json, jsoncmd) = builder.BuildJson(jsonArgs);
                WriteToFile(jsonArgs, json);
                WriteToFile("jsonrender.bat", jsoncmd);


                var yamlArgs = "args.yaml";
                var (yaml, yamlCmd) = builder.BuildYaml(yamlArgs);
                WriteToFile(yamlArgs, yaml);
                WriteToFile("yamlrender.bat", yamlCmd);
            }
            catch (Exception e)
            {
                MessageBox.Show("Sorry - couldn't export invocation");
            }
        }
    }
}