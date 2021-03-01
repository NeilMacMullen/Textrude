using System.IO;
using System.Linq;
using System.Windows;
using Engine.Application;
using MaterialDesignExtensions.Controls;
using Ookii.Dialogs.Wpf;
using SharedApplication;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : MaterialWindow
    {
        private readonly PathManipulator exePath = PathManipulator.FromExe();

        private readonly TextrudeProject proj;
        private string _homeFolder;

        public ExportDialog(TextrudeProject project)
        {
            proj = project;
            _homeFolder = new RunTimeEnvironment(new FileSystem()).ApplicationFolder();
            InitializeComponent();
            RenderCli.IsChecked = true;
            UseAbsolutePaths.IsChecked = true;
            RootFolder.Text = _homeFolder;
            IgnoreUnlinked.IsChecked = true;
            UpdateUi();
        }

        public void UpdateHomePath()
        {
            var dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog(this) == false)
                return;
            _homeFolder = dlg.SelectedPath;
            RootFolder.Text = _homeFolder;
            UpdateUi();
        }

        public void UpdateUi()
        {
            var engine = proj.EngineInput;

            WorkingDirectorySection.IsEnabled = UseAbsolutePaths.IsChecked == false;

            string RelAbsPath(string path)
            {
                path = exePath.ToAbsolute(path);

                var rootManipulator = new PathManipulator(_homeFolder);
                return UseAbsolutePaths.IsChecked == true
                    ? rootManipulator.ToAbsolute(path)
                    : rootManipulator.ToRelative(path);
            }

            NamedFile GetFile(string name, string path, ModelFormat format) =>
                new NamedFile(name, RelAbsPath(path), format);

            var options = new RenderOptions
            {
                Definitions = engine.Definitions.Clean().ToArray(),
                Include = engine.IncludePaths.Clean()
                    .ToArray(),
                Models = NamedFileFactory.Squash(
                        engine.Models
                            .Select(m => GetFile(m.Name, m.Path, m.Format))
                            .Where(CheckLink)
                    )
                    .ToArray(),
                Output = NamedFileFactory.Squash(
                        proj.OutputControl.Outputs.Select(m => GetFile(m.Name, m.Path, ModelFormat.Unknown))
                            .Where(CheckLink))
                    .ToArray(),
                Lazy = IsLazy.IsChecked == true,
                Template = RelAbsPath(engine.TemplatePath)
            };

            const string exeName = "textrude.exe";
            var exe = UseFullyQualifiedExe.IsChecked == true
                ? Path.Combine(exePath.Root,
                    exeName)
                : exeName;

            var builder = new CommandLineBuilder(options).WithExe(exe);
            ArgsFileSection.Visibility = RenderCli.IsChecked == true
                ? Visibility.Collapsed
                : Visibility.Visible;
            if (RenderCli.IsChecked == true)
            {
                CommandText.Text = builder.BuildRenderInvocation();
                JsonYaml.Text = string.Empty;
            }

            if (RenderJson.IsChecked == true)
            {
                var (json, jsoncmd) = builder.BuildJson("args.json");
                CommandText.Text = jsoncmd;
                JsonYaml.Text = json;
            }

            if (RenderYaml.IsChecked == true)
            {
                var (yaml, yamlcmd) = builder.BuildYaml("args.yaml");
                CommandText.Text = yamlcmd;
                JsonYaml.Text = yaml;
            }
        }

        private bool CheckLink(NamedFile f) =>
            IgnoreUnlinked.IsChecked != true ||
            f.Path.Length != 0;

        private void OnUpdateUI(object sender, RoutedEventArgs e)
        {
            UpdateUi();
        }

        private void OnUpdateHomeFolder(object sender, RoutedEventArgs e)
        {
            UpdateHomePath();
        }

        private void CopyCmdToClipboard(object sender, RoutedEventArgs e)
        {
            ClipboardHelper.CopyToClipboard(CommandText.Text);
        }

        private void CopyArgsToClipboard(object sender, RoutedEventArgs e)
        {
            ClipboardHelper.CopyToClipboard(JsonYaml.Text);
        }
    }
}
