using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Engine.Application;
using MaterialDesignExtensions.Controls;
using TextrudeInteractive.Monaco.Messages;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow
    {
        private const string HomePage = @"https://github.com/NeilMacMullen/Textrude";

        private readonly MonacoVisualSettings _editorVisualSettings = new();

        private readonly ISubject<EngineInputSet> _inputStream =
            new BehaviorSubject<EngineInputSet>(EngineInputSet.EmptyYaml);

        //private readonly AvalonEditCompletionHelper _mainEditWindow;

        private readonly TabControlManager _modelManager;
        private readonly TabControlManager _outputManager;
        private readonly ProjectManager _projectManager;
        private readonly TabControlManager _templateManager;

        private UpgradeManager.VersionInfo _latestVersion = UpgradeManager.VersionInfo.Default;
        private int _responseTimeMs = 50;
        private int _uiLockCount;

        public MainWindow()
        {
            InitializeComponent();

            if (!MonacoResourceFetcher.IsWebView2RuntimeAvailable())
            {
                MessageBox.Show(
                    "The WebView2 runtime or Edge (non-stable channel) must be installed for the editor to work!\n" +
                    @"See the Textrude main site for further information https://github.com/NeilMacMullen/Textrude \n" +
                    "Textrude will now exit.",
                    "Textrude: WebView2 runtime must be installed!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Application.Current.Shutdown();
            }

            LockRender();


            SetTitle(string.Empty);

            TemplateEditPane.SetDirection(PaneType.Model);
            TemplateEditPane.OnUserInput = OnModelChanged;
            _templateManager = new TabControlManager(Templates, TemplateEditPane);

            SharedInput.SetDirection(PaneType.Model);
            SharedInput.OnUserInput = OnModelChanged;
            _modelManager = new TabControlManager(InputModels, SharedInput);

            SharedOutput.SetDirection(PaneType.Output);
            _outputManager = new TabControlManager(OutputTab, SharedOutput);
            _projectManager = new ProjectManager(this);


            SetUi(EngineInputSet.EmptyYaml, false);
            SetOutputPanes(EngineOutputSet.Empty, false);

            //do this before setting up the input stream so we can change the responsiveness
            var settings = LoadSettings();


            //check to see if the application was invoked with arguments
            //if so open that project, otherwise open the last used
            var args = Environment.GetCommandLineArgs();
            if (args.Length == 2 && ProjectManager.IsProject(args[1]))
            {
                _projectManager.LoadProject(args[1]);
            }
            else if (settings.RecentProjects.Any())
            {
                var mostRecentProject = settings.RecentProjects.OrderByDescending(p => p.LastLoaded).First();
                _projectManager.LoadProject(mostRecentProject.Path);
            }

            _inputStream
                .Throttle(TimeSpan.FromMilliseconds(_responseTimeMs))
                .ObserveOn(NewThreadScheduler.Default)
                .Select(Render)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(HandleRenderResults);
            RunBackgroundUpgradeCheck();
            DataContext = _editorVisualSettings;
            ApplyVisualSettings(null, null);
            _editorVisualSettings.PropertyChanged += ApplyVisualSettings;
            UnlockRender();
        }


        private void ApplyVisualSettings(object sender, PropertyChangedEventArgs args)
        {
            var editors = new[]
            {
                SharedInput,
                SharedOutput,
                TemplateEditPane
            };
            foreach (var e in editors)
            {
                e.MonacoPane.TextSize = _editorVisualSettings.TextSize;
                e.MonacoPane.LineNumbers = _editorVisualSettings.LineNumbers;
                e.MonacoPane.WordWrap = _editorVisualSettings.WordWrap;
                e.MonacoPane.VisibleWhitespace = _editorVisualSettings.ShowWhitespace;
            }
        }


        private void LockRender()
        {
            _uiLockCount++;
        }

        private void UnlockRender()
        {
            _uiLockCount--;
            if (_uiLockCount == 0)
                OnModelChanged(false);
        }

        private void RunBackgroundUpgradeCheck()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    _latestVersion = await UpgradeManager.GetLatestVersion();
                    await Task.Delay(TimeSpan.FromHours(24));
                }
            });
        }


        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (ShouldChangesBeLost())
            {
                PersistSettings();
                _inputStream.OnCompleted();
            }
            else e.Cancel = true;
        }


        private void ToggleWhiteSpace(object sender, RoutedEventArgs e)
        {
            _editorVisualSettings.ShowWhitespace = !_editorVisualSettings.ShowWhitespace;
        }

        private void ToggleDefsAndIncludes(object sender, RoutedEventArgs e)
        {
            static bool IsToggleable(EditPaneViewModel p) =>
                new[] {PaneType.IncludePaths, PaneType.Definitions}.Contains(p.PaneType);

            _modelManager.ToggleVisibility(IsToggleable);
        }

        private void RenameModel(object sender, RoutedEventArgs e)
        {
            Rename(_modelManager, PaneType.Model);
        }

        private void SaveAllExportInvocation(object sender, RoutedEventArgs e)
        {
            SaveAllInputs(null, null);
            SaveAllOutputs(null, null);
            ExportInvocation(null, null);
        }

        #region jumplist

        #endregion

        #region outputs menu

        private void AddOutput(object sender, RoutedEventArgs e)
        {
            _outputManager.AddPane(ViewModelFactory.CreateOutput(OutputPaneModel.Empty, _outputManager.Count));
        }

        private void RemoveOutput(object sender, RoutedEventArgs e) => _outputManager.RemoveSelected(_ => true);


        private void SaveAllOutputs(object sender, RoutedEventArgs e)
        {
            _outputManager.ForAll(p => p.SaveIfLinked());
        }

        private void Rename(TabControlManager mgr, PaneType type)
        {
            var currentModel = mgr.CurrentPane();
            if (currentModel.PaneType != type)
                return;
            var dlg = new RenameItem(currentModel.ScribanName) {Owner = this};
            if (dlg.ShowDialog() == true)
            {
                currentModel.ScribanName = dlg.Name;
                mgr.RepaintHeaders();
                OnModelChanged(true);
            }
        }

        private void RenameOutput(object sender, RoutedEventArgs e)
        {
            Rename(_outputManager, PaneType.Output);
        }

        #endregion

        #region inputs menu

        private void AddModel(object sender, RoutedEventArgs e)
        {
            LockRender();
            _modelManager.AddPane(ViewModelFactory.CreateModel(ModelText.EmptyYaml,
                _modelManager.Panes.Count(p => p.PaneType == PaneType.Model)));
            UnlockRender();
        }

        private void RemoveModel(object sender, RoutedEventArgs e)
        {
            LockRender();
            _modelManager.RemoveSelected(p => p.PaneType == PaneType.Model);
            UnlockRender();
        }

        private void ReloadAllInputs(object sender, RoutedEventArgs e)
        {
            LockRender();
            _modelManager.ForAll(p => p.LoadIfLinked());
            _templateManager.ForAll(p => p.LoadIfLinked());

            UnlockRender();
        }

        private void SaveAllInputs(object sender, RoutedEventArgs e)
        {
            _modelManager.ForAll(p => p.SaveIfLinked());
            _templateManager.ForAll(p => p.SaveIfLinked());
        }

        #endregion

        #region view menu

        private void SmallerFont(object sender, RoutedEventArgs e) =>
            _editorVisualSettings.TextSize = Math.Max(_editorVisualSettings.TextSize - 2, 10);

        private void LargerFont(object sender, RoutedEventArgs e) =>
            _editorVisualSettings.TextSize = Math.Min(_editorVisualSettings.TextSize + 2, 36);

        private void ToggleLineNumbers(object sender, RoutedEventArgs e) =>
            _editorVisualSettings.LineNumbers = !_editorVisualSettings.LineNumbers;

        private void ToggleWordWrap(object sender, RoutedEventArgs e) =>
            _editorVisualSettings.WordWrap = !_editorVisualSettings.WordWrap;

        #endregion

        #region project menu and support

        /// <summary>
        ///     Sets up the output side of the UI when a new project is loaded
        /// </summary>
        public void SetOutputPanes(EngineOutputSet outputControl, bool trim)
        {
            _outputManager.Clear();
            var outputs = outputControl.Outputs;
            if (trim)
                outputs = outputs.Take(1).ToArray();
            foreach (var f in outputs)
            {
                _outputManager.AddPane(ViewModelFactory.CreateOutput(f, _outputManager.Count));
            }

            //ensure there is always at least one output - otherwise things can get confusing for the user
            if (!_outputManager.Panes.Any())
                _outputManager.AddPane(ViewModelFactory.CreateOutput(OutputPaneModel.Empty, 0));

            _outputManager.FocusFirst();
        }

        public void SetTitle(string path)
        {
#if HASGITVERSION

            var file = Path.GetFileNameWithoutExtension(path);
            var title =
                $"Textrude Interactive {GitVersionInformation.SemVer} : {file}";
            Title = title;
#endif
        }

        public EngineOutputSet CollectOutput()
        {
            return new(
                _outputManager.Panes.Select(b => new OutputPaneModel(b.Format, b.ScribanName, b.LinkedPath))
            );
        }

        private bool ShouldChangesBeLost()
        {
            if (_projectManager.IsDirty)
            {
                if (MessageBox.Show(this,
                    "You have unsaved changes in the current project.\nDo you really want to lose them?", "Warning",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        private void NewProject(object sender, RoutedEventArgs e)
        {
            LockRender();
            if (ShouldChangesBeLost())
                _projectManager.NewProject();
            LockRender();
        }

        private void LoadProject(object sender, RoutedEventArgs e)
        {
            LockRender();
            if (ShouldChangesBeLost())
                _projectManager.LoadProject();
            UnlockRender();
        }


        private void SaveProject(object sender, RoutedEventArgs e) => _projectManager.SaveProject();


        private void SaveProjectAs(object sender, RoutedEventArgs e) => _projectManager.SaveProjectAs();

        /// <summary>
        ///     Sets up the input side of the UI when a new project is loaded
        /// </summary>
        public void SetUi(EngineInputSet inputSet, bool trim)
        {
            _modelManager.Clear();
            foreach (var model in inputSet.Models)
            {
                if (trim && string.IsNullOrWhiteSpace(model.Text))
                    continue;
                _modelManager.AddPane(ViewModelFactory.CreateModel(model, _modelManager.Count));
            }

            //ensure we start with at least one model to avoid confusing the user
            if (!_modelManager.Panes.Any())
                _modelManager.AddPane(ViewModelFactory.CreateModel(ModelText.EmptyYaml, _modelManager.Count));

            _modelManager.AddPane(ViewModelFactory.CreateDefinitions(inputSet.Definitions));
            _modelManager.AddPane(ViewModelFactory.CreateIncludePaths(inputSet.IncludePaths));
            _modelManager.FocusFirst();

            _templateManager.Clear();
            _templateManager.AddPane(ViewModelFactory.CreateTemplate(inputSet.Template, inputSet.TemplatePath));
        }


        private void ExportInvocation(object sender, RoutedEventArgs e) => _projectManager.ExportProject();

        #endregion

        #region main loop

        private void OnModelChanged() => OnModelChanged(true);

        private void OnModelChanged(bool markDirty)
        {
            if (_uiLockCount > 0)
                return;
            if (markDirty)
                _projectManager.IsDirty = true;
            try
            {
                _inputStream.OnNext(CollectInput());
            }
            catch (Exception exception)
            {
                Errors.Text = exception.Message;
            }
        }

        private TimedOperation<ApplicationEngine> Render(EngineInputSet gi)
        {
            var rte = new RunTimeEnvironment(new FileSystemOperations());
            var engine = new ApplicationEngine(rte);
            var timer = new TimedOperation<ApplicationEngine>(engine);

            foreach (var m in gi.Models)
                engine = engine.WithModel(m.Name, m.Text, m.Format);
            engine = engine
                .WithEnvironmentVariables()
                .WithIncludePaths(gi.IncludePaths)
                .WithDefinitions(gi.Definitions)
                .WithHelpers()
                .WithTemplate(gi.Template);

            engine.Render();
            return timer;
        }

        private void HandleRenderResults(TimedOperation<ApplicationEngine> timedEngine)
        {
            var elapsedMs = (int) timedEngine.Timer.ElapsedMilliseconds;
            var engine = timedEngine.Value;
            var outputPanes = _outputManager.Panes.ToArray();
            foreach (var o in outputPanes)
            {
                o.Text = engine.GetOutputFromVariable(o.ScribanName);
            }

            Errors.Text = string.Empty;
#if HASGITVERSION
            if (_latestVersion.Supersedes(GitVersionInformation.SemVer))
            {
                Errors.Text =
                    $"Upgrade to {_latestVersion.Version} available - please visit {UpgradeManager.ReleaseSite}" +
                    Environment.NewLine;
            }
#endif
            Errors.Text += $"Completed: {DateTime.Now.ToLongTimeString()}  Render time: {elapsedMs}ms" +
                           Environment.NewLine;
            if (engine.HasErrors)
            {
                Errors.Foreground = Brushes.OrangeRed;
                Errors.Text += string.Join(Environment.NewLine, engine.Errors);
            }
            else
            {
                Errors.Foreground = Brushes.GreenYellow;
                Errors.Text += "No errors";
            }

            CompletionType MapType(ModelPath.PathType type)
            {
                var d = new Dictionary<ModelPath.PathType, CompletionType>
                {
                    [ModelPath.PathType.Method] = CompletionType.Method,
                    [ModelPath.PathType.Property] = CompletionType.Property,
                    [ModelPath.PathType.Keyword] = CompletionType.Keyword
                };
                return d.TryGetValue(type, out var k) ? k : CompletionType.Value;
            }

            var nodes = engine.ModelPaths()
                .Select(r => new CompletionNode(r.Render(), r.Terminal(), MapType(r.ModelType)
                )).ToArray();
            var comp = new Completions(nodes);
            TemplateEditPane.MonacoPane.SetCompletions(comp);
        }


        public EngineInputSet CollectInput()
        {
            ModelFormat TryFormat(string s)
                => Enum.TryParse(typeof(ModelFormat), s, true, out var f) ? (ModelFormat) f : ModelFormat.Line;

            static bool IsInput(EditPaneViewModel p) =>
                new[] {PaneType.Model}.Contains(p.PaneType);


            var models = _modelManager.Panes
                .Where(IsInput)
                .Select(m => new ModelText(m.Text, TryFormat(m.Format), m.ScribanName, m.LinkedPath))
                .ToArray();
            var includeText = _modelManager.Panes.Single(p => p.PaneType == PaneType.IncludePaths)
                .Text;
            var template = _templateManager.Panes.Single();
            var definitions = _modelManager.Panes.Single(p => p.PaneType == PaneType.Definitions).Text;
            return new EngineInputSet(template.Text,
                template.LinkedPath,
                models,
                definitions,
                includeText);
        }

        #endregion

        #region settings

        /// <summary>
        ///     Loads any persisted settings and applies them
        /// </summary>
        private ApplicationSettings LoadSettings()
        {
            var settings = SettingsManager.ReadSettings();
            _editorVisualSettings.LineNumbers = settings.LineNumbersOn;
            _editorVisualSettings.TextSize = settings.FontSize;
            _editorVisualSettings.WordWrap = settings.WrapText;
            _editorVisualSettings.ShowWhitespace = settings.ShowWhitespace;
            _responseTimeMs = settings.ResponseTime;
            return settings;
        }


        /// <summary>
        ///     Persist settings
        /// </summary>
        private void PersistSettings()
        {
            //persist settings
            var settings = new ApplicationSettings
            {
                FontSize = _editorVisualSettings.TextSize,
                LineNumbersOn = _editorVisualSettings.LineNumbers,
                WrapText = _editorVisualSettings.WordWrap,
                ShowWhitespace = _editorVisualSettings.ShowWhitespace,
                ResponseTime = _responseTimeMs,
                //TODO - this is a temporary hack. ProjectManager should actually track the projects that have been used
                //and save them all so we can display them in the menu
                RecentProjects = new[]
                {
                    new RecentlyUsedProject
                    {
                        LastLoaded = DateTime.UtcNow,
                        Path = _projectManager.CurrentProjectPath
                    }
                }
            };
            SettingsManager.WriteSettings(settings);
        }

        #endregion

        #region HelpMenu

        private void OpenBrowserTo(Uri uri)
        {
            var ps = new ProcessStartInfo(uri.AbsoluteUri)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private void OpenScriban(string doc)
        {
            OpenBrowserTo(new Uri($"https://github.com/scriban/scriban/blob/master/doc/{doc}.md"));
        }

        private void ShowLanguageRef(object sender, RoutedEventArgs e) =>
            OpenScriban("language");

        private void ShowBuiltIns(object sender, RoutedEventArgs e) => OpenScriban("builtins");

        private void OpenHome(string path) =>
            OpenBrowserTo(new Uri(HomePage + "/" + path));

        private void ShowAbout(object sender, RoutedEventArgs e) =>
            OpenHome(string.Empty);


        private void NewIssue(object sender, RoutedEventArgs e) =>
            OpenHome("issues/new?assignees=&labels=bug&template=bug_report.md&title=Bug");

        private void NewIdea(object sender, RoutedEventArgs e) =>
            OpenHome("issues/new?assignees=&labels=enhancement&template=feature_request.md&title=Suggestion");

        private void SendASmile(object sender, RoutedEventArgs e) =>
            OpenHome("issues/new?assignees=&labels=smile&template=positive-feedback.md&title=I%20like%20it%21");

        private void GoGitter(object sender, RoutedEventArgs e)
        {
            OpenBrowserTo(new Uri("https://gitter.im/Textrude/community"));
        }

        #endregion
    }
}
