using System;
using System.ComponentModel;
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

        private readonly ISubject<GenInput> InputStream =
            new BehaviorSubject<GenInput>(GenInput.EmptyYaml);


        public MainWindow()
        {
            InitializeComponent();
            _projectManager = new ProjectManager(this);
            format.ItemsSource = Enum.GetValues(typeof(ModelFormat));
            format.SelectedIndex = 0;
            InputStream
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(NewThreadScheduler.Default)
                .Select(gi => new ApplicationEngine()
                    .WithTemplate(gi.Template)
                    .WithModel(gi.ModelText, gi.Format)
                    .WithEnvironmentVariables()
                    .WithDefinitions(gi.Definitions)
                    .WithHelpers()
                    .Render())
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(HandleNewText);
            SetUI(GenInput.EmptyYaml);
        }


        private void HandleNewText(ApplicationEngine engine)
        {
            OutputText.Text = engine.Output;
            Errors.Text = string.Join(Environment.NewLine, engine.Errors);
        }


        public GenInput CollectInput()
        {
            return new GenInput(TemplateTextBox.Text, ModelTextBox.Text,
                (ModelFormat) format.SelectedItem, DefinitionsTextBox.Text);
        }

        private void OnModelTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                InputStream.OnNext(CollectInput());
            }
            catch (Exception exception)
            {
                Errors.Text = exception.Message;
            }
        }

        private void OnDefinitionsTextChanged(object sender, TextChangedEventArgs e)
        {
            OnModelTextChanged(sender, e);
        }


        private void Format_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnModelTextChanged(null, null);
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
            format.SelectedValue = gi.Format;
            ModelTextBox.Text = gi.ModelText;
            TemplateTextBox.Text = gi.Template;
        }
    }
}