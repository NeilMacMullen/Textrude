using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Engine.Application;
using MaterialDesignExtensions.Controls;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow
    {
        private readonly ProjectManager _projectManager;
        private readonly bool _uiIsReady;

        private readonly ComboBox[] formats;

        private readonly ISubject<EngineInputSet> InputStream =
            new BehaviorSubject<EngineInputSet>(EngineInputSet.EmptyYaml);

        private readonly TextBox[] modelBoxes;
        private readonly TextBox[] OutputBoxes;

        public MainWindow()
        {
            InitializeComponent();
            SetTitle(string.Empty);
            formats = new[] {format0, format1, format2};
            modelBoxes = new[] {ModelTextBox0, ModelTextBox1, ModelTextBox2};
            OutputBoxes = new[] {OutputText0, OutputText1, OutputText2};
            _projectManager = new ProjectManager(this);
            foreach (var comboBox in formats)
            {
                comboBox.ItemsSource = Enum.GetValues(typeof(ModelFormat));
            }

            SetUI(EngineInputSet.EmptyYaml);


            InputStream
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(NewThreadScheduler.Default)
                .Select(Render)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(HandleNewText);
            _uiIsReady = true;
        }

        public void SetTitle(string path)
        {
#if HASGITVERSION

            var file = Path.GetFileNameWithoutExtension(path);
            var title =
                $"Textrude Interactive {GitVersionInformation.SemVer} ({GitVersionInformation.CommitDate}) {file}";
            Title = title;
#endif
        }

        private ApplicationEngine Render(EngineInputSet gi)
        {
            var engine = new ApplicationEngine(new FileSystemOperations());
            foreach (var m in gi.Models)
                engine = engine.WithModel(m.Text, m.Format);
            engine = engine
                .WithEnvironmentVariables()
                .WithDefinitions(gi.Definitions)
                .WithIncludePaths(gi.IncludePaths)
                .WithHelpers()
                .WithTemplate(gi.Template);

            return engine.Render();
        }

        private void HandleNewText(ApplicationEngine engine)
        {
            var outputs = engine.GetOutput(OutputBoxes.Length);
            for (var i = 0; i < outputs.Length; i++)
            {
                OutputBoxes[i].Text = outputs[i];
            }

            Errors.Text = string.Join(Environment.NewLine, engine.Errors);
        }


        public EngineInputSet CollectInput()
        {
            var models = Enumerable.Range(0, formats.Length)
                .Select(i => new ModelText(modelBoxes[i].Text, (ModelFormat) formats[i].SelectedValue))
                .ToArray();

            return new EngineInputSet(TemplateTextBox.Text,
                models,
                DefinitionsTextBox.Text,
                IncludesTextBox.Text);
        }

        private void OnModelChanged()
        {
            if (!_uiIsReady)
                return;
            try
            {
                InputStream.OnNext(CollectInput());
            }
            catch (Exception exception)
            {
                Errors.Text = exception.Message;
            }
        }

        private void OnModelTextChanged(object sender, TextChangedEventArgs e)
        {
            OnModelChanged();
        }

        private void OnModelFormatChanged(object sender, SelectionChangedEventArgs e)
        {
            OnModelChanged();
        }

        private void LoadProject(object sender, RoutedEventArgs e)
        {
            _projectManager.LoadProject();
        }


        private void SaveProject(object sender, RoutedEventArgs e)
        {
            _projectManager.SaveProject();
        }


        private void SaveProjectAs(object sender, RoutedEventArgs e)
        {
            _projectManager.SaveProjectAs();
        }


        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            InputStream.OnCompleted();
        }


        public void SetUI(EngineInputSet gi)
        {
            DefinitionsTextBox.Text = string.Join(Environment.NewLine, gi.Definitions);

            for (var i = 0; i < formats.Length; i++)
            {
                var model = (i < gi.Models.Length) ? gi.Models[i] : ModelText.EmptyYaml;
                formats[i].SelectedValue = model.Format;
                modelBoxes[i].Text = model.Text;
            }

            TemplateTextBox.Text = gi.Template;

            IncludesTextBox.Text = string.Join(Environment.NewLine, gi.IncludePaths);
        }

        private void OpenBrowserTo(Uri uri)
        {
            var ps = new ProcessStartInfo(uri.AbsoluteUri)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }


        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            OpenBrowserTo(new Uri("https://github.com/NeilMacMullen/Textrude"));
        }

        private void ShowLanguageRef(object sender, RoutedEventArgs e)
        {
            OpenBrowserTo(new Uri("https://github.com/scriban/scriban/blob/master/doc/language.md"));
        }
    }
}