using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Shell;
using Engine.Application;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using SharedApplication;

namespace TextrudeInteractive
{
    public class ProjectManager
    {
        private const string Extension = ".texproj";
        private static readonly string Filter = $"project files (*{Extension})|*{Extension}|All files (*.*)|*.*";
        private readonly MainWindow _owner;

        public ProjectManager(MainWindow owner) => _owner = owner;

        public string CurrentProjectPath { get; private set; } = string.Empty;
        public bool IsDirty { get; set; }

        private TextrudeProject CreateProject()
        {
            var proj = new TextrudeProject
            {
                Version = 1,
                EngineInput = _owner.CollectInput(),
                OutputControl = _owner.CollectOutput()
            };

            return proj;
        }


        public void LoadProject(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            try
            {
                var text = File.ReadAllText(path);
                var proj = JsonSerializer.Deserialize<TextrudeProject>(text);

                CurrentProjectPath = path;
                UpdateUi(proj);
                IsDirty = false;
                AddCurrentToJumpList();
            }
            catch (Exception e)
            {
                MessageBox.Show(_owner, $"Error - unable to open project '{path}'");
            }
        }


        public void LoadProject()
        {
            var dlg = new OpenFileDialog {Filter = Filter};

            if (dlg.ShowDialog(_owner) == true)
            {
                LoadProject(dlg.FileName);
            }
        }

        public void SaveProject()
        {
            if (string.IsNullOrWhiteSpace(CurrentProjectPath))
                SaveProjectAs();
            else
            {
                try
                {
                    var o = new JsonSerializerOptions {WriteIndented = true};
                    var text = JsonSerializer.Serialize(CreateProject(), o);

                    File.WriteAllText(CurrentProjectPath, text);
                    IsDirty = false;
                    AddCurrentToJumpList();
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
                CurrentProjectPath = dlg.FileName;
                _owner.SetTitle(CurrentProjectPath);
                SaveProject();
            }
        }

        public void NewProject()
        {
            CurrentProjectPath = string.Empty;
            var proj = new TextrudeProject();
            UpdateUi(proj);
            IsDirty = false;
        }

        private void UpdateUi(TextrudeProject project)
        {
            //TODO nasty hack to cope with old style projects where we always had 3 inputs and
            //outputs.  Can be removed in a few months from Feb 01 2021
            var trim = project.Version == 0;


            //Set up the OUTPUT first, otherwise they will not
            //be available when the input is set so the rendered output
            //won't appear!


            _owner.SetOutputPanes(project.OutputControl, trim);
            _owner.SetUi(project.EngineInput, trim);
            _owner.SetTitle(CurrentProjectPath);
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
            catch
            {
                MessageBox.Show("Sorry - couldn't export invocation");
            }
        }

        public static bool IsProject(string s) => Path.GetExtension(s) == Extension;


        public void AddCurrentToJumpList()
        {
            if (string.IsNullOrWhiteSpace(CurrentProjectPath))
                return;
            var exe = Process.GetCurrentProcess().MainModule.FileName;
            var task =
                new JumpTask
                {
                    ApplicationPath = exe,
                    Arguments = CurrentProjectPath,
                    Title = Path.GetFileNameWithoutExtension(CurrentProjectPath),
                    Description = CurrentProjectPath,
                };
            JumpList.AddToRecentCategory(task);
        }
    }
}
