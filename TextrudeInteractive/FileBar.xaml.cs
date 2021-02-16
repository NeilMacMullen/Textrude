using System;
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
        private readonly PathManipulator _pathMangler;
        private string _pathName = string.Empty;

        public Func<string> ObtainText = () => string.Empty;

        public Action<string, string> OnLoad = (_, _) => { };

        public FileBar()
        {
            InitializeComponent();

            _pathMangler = PathManipulator.FromExe();
        }

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
                UpdateAbsRelButton();
            }
        }

        public void SetSaveOnly(FileLinkageTypes type)
        {
            LoadButton.Visibility = type.HasFlag(FileLinkageTypes.Load) ? Visibility.Visible : Visibility.Collapsed;
            SaveButton.Visibility = type.HasFlag(FileLinkageTypes.Save) ? Visibility.Visible : Visibility.Collapsed;
            CopyToClipboardButton.Visibility =
                type.HasFlag(FileLinkageTypes.Clipboard) ? Visibility.Visible : Visibility.Collapsed;


            FilePath.Visibility = type != FileLinkageTypes.None ? Visibility.Visible : Visibility.Collapsed;
            UnlinkButton.Visibility = type != FileLinkageTypes.None ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UnlinkFile(object sender, RoutedEventArgs e)
        {
            OnLoad(string.Empty, ObtainText());
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
            if (FileManager.TryLoadFile(dlg.FileName, out var text))

                OnLoad(dlg.FileName, text);
        }


        private void SaveToFile()
        {
            var dlg = new SaveFileDialog {FileName = PathName};
            if (dlg.ShowDialog() != true) return;
            if (FileManager.TrySave(dlg.FileName, ObtainText()))
                OnLoad(dlg.FileName, ObtainText());
        }

        private void FileBar_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathName))
                LoadFromFile();
            else if (FileManager.TryLoadFile(PathName, out var text))
            {
                OnLoad(PathName, text);
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathName))
                SaveToFile();
            else

                FileManager.TrySave(PathName, ObtainText());
        }

        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            ClipboardHelper.CopyToClipboard(ObtainText());
        }

        private void UpdateAbsRelButton()
        {
            AbsRelButton.Content = PathManipulator.IsAbsolute(PathName) ? "C:" : "..";
        }


        private void ToggleAbsRel(object sender, RoutedEventArgs e)
        {
            if (PathName.Length == 0)
                return;
            PathName = PathManipulator.IsAbsolute(PathName)
                ? _pathMangler.ToRelative(PathName)
                : _pathMangler.ToAbsolute(PathName);

            OnLoad(PathName, ObtainText());
        }
    }
}
