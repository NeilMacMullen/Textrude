using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for FileBar.xaml
    /// </summary>
    public partial class FileBar : UserControl
    {
        private string _pathName = string.Empty;

        public Action<string, bool> OnLoad = (_, _) => { };

        public Func<string> OnSave = () => string.Empty;

        public FileBar()
        {
            InitializeComponent();
        }

        public bool IsSaveOnly { get; set; }

        /// <summary>
        ///     Path to file that the model is connected to
        /// </summary>
        public string PathName
        {
            get => _pathName;
            set
            {
                _pathName = value;
                FilePath.Content = _pathName;
            }
        }

        private void UnlinkFile(object sender, RoutedEventArgs e)
        {
            PathName = string.Empty;
        }


        private void LoadFromFile()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter =
                "csv files (*.csv)|*.csv|" +
                "yaml files (*.yaml)|*.yaml|" +
                "json files (*.json)|*.json|" +
                "txt files (*.txt)|*.txt|" +
                "All files (*.*)|*.*";
            dlg.FileName = PathName;
            if (dlg.ShowDialog() != true) return;
            //only change the format if loading from file the first time since
            //the user might override subsequently
            TryLoadFile(dlg.FileName, true);
        }

        private void TryLoadFile(string path, bool isNewFile)
        {
            try
            {
                var text = File.ReadAllText(path);
                PathName = path;
                OnLoad(text, isNewFile);
            }
            catch
            {
            }
        }


        private bool TrySave(string path)
        {
            try
            {
                var text = OnSave();
                File.WriteAllText(path, text);
                PathName = path;
                return true;
            }
            catch
            {
            }

            return false;
        }

        private void SaveToFile()
        {
            var dlg = new SaveFileDialog {FileName = PathName};
            if (dlg.ShowDialog() != true) return;
            TrySave(dlg.FileName);
        }

        private void FileBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsSaveOnly)
                LoadButton.Visibility = Visibility.Collapsed;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathName))
                LoadFromFile();
            else TryLoadFile(PathName, false);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathName))
                SaveToFile();
            else
                TrySave(PathName);
        }

        public void SaveIfLinked() => TrySave(PathName);

        public void LoadIfLinked() => TryLoadFile(PathName, false);
    }
}