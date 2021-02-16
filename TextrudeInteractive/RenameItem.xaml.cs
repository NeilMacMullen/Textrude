using System.Windows;
using MaterialDesignExtensions.Controls;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for RenameItem.xaml
    /// </summary>
    public partial class RenameItem : MaterialWindow
    {
        public RenameItem(string oldName)
        {
            InitializeComponent();
            Name = oldName;
            Title = $"Renaming {oldName}";
            NewName.Text = oldName;
            NewName.Focus();
        }

        public new string Name { get; private set; }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Name = NewName.Text;
            DialogResult = true;
        }
    }
}
