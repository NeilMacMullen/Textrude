using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Engine.Application;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProjectManager _projectManager;
        private readonly bool _uiIsReady;

        private readonly ComboBox[] formats;

        private readonly ISubject<GenInput> InputStream =
            new BehaviorSubject<GenInput>(GenInput.EmptyYaml);

        private readonly TextBox[] modelBoxes;
        private readonly TextBox[] OutputBoxes;

        public MainWindow()
        {
            InitializeComponent();
            formats = new[] {format0, format1, format2};
            modelBoxes = new[] {ModelTextBox0, ModelTextBox1, ModelTextBox2};
            OutputBoxes = new[] {OutputText0, OutputText1, OutputText2};
            _projectManager = new ProjectManager(this);
            foreach (var comboBox in formats)
            {
                comboBox.ItemsSource = Enum.GetValues(typeof(ModelFormat));
            }

            SetUI(GenInput.EmptyYaml);


            InputStream
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(NewThreadScheduler.Default)
                .Select(Render)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(HandleNewText);
            _uiIsReady = true;
        }

        private ApplicationEngine Render(GenInput gi)
        {
            var engine = new ApplicationEngine()
                .WithTemplate(gi.Template)
                .WithEnvironmentVariables()
                .WithDefinitions(gi.Definitions)
                .WithHelpers();

            foreach (var m in gi.Models)
                engine = engine.WithModel(m.Text, m.Format);

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


        public GenInput CollectInput()
        {
            var models = Enumerable.Range(0, formats.Length)
                .Select(i => new ModelText(modelBoxes[i].Text, (ModelFormat) formats[i].SelectedValue))
                .ToArray();

            return new GenInput(TemplateTextBox.Text, models, DefinitionsTextBox.Text);
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


        public void SetUI(GenInput gi)
        {
            DefinitionsTextBox.Text = string.Join(Environment.NewLine, gi.Definitions);

            for (var i = 0; i < formats.Length; i++)
            {
                var model = (i < gi.Models.Length) ? gi.Models[i] : ModelText.EmptyYaml;
                formats[i].SelectedValue = model.Format;
                modelBoxes[i].Text = model.Text;
            }

            TemplateTextBox.Text = gi.Template;
        }
    }
}