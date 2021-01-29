using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive
{
	/// <summary>
	/// Interaction logic for OutputMonacoPane.xaml
	/// </summary>
	// TODO OutputMonacoPane is extended-copy of OutputPane -> maybe use inheritance?
	public partial class OutputMonacoPane : UserControl
	{
		private string _format = string.Empty;
		private string _text = string.Empty;
		private MonacoBinding _monacoBinding;

		public OutputMonacoPane()
		{
			InitializeComponent();
			_monacoBinding = new MonacoBinding(WebView, isReadOnly: true, _ => { });
			_monacoBinding.Initialize().ConfigureAwait(false);

			var formats = MonacoBinding.GetSupportedFormats();
			FormatSelection.ItemsSource = formats;
			FormatSelection.SelectedIndex = formats.IndexOf("text");
		}

		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				_monacoBinding.Text = value;
			}
		}

		/// <summary>
		///     Serialisable Format to be persisted in project
		/// </summary>
		public string Format
		{
			get => _format;
			set
			{
				if (_format != value)
				{
					_format = value;
					_monacoBinding.Format = Format;
					FormatSelection.SelectedItem = _format;
				}
			}
		}

		private void FormatSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Format = FormatSelection.SelectedItem as string;
		}

		private void OutputPane_OnLoaded(object sender, RoutedEventArgs e)
		{
			//Beware - if this is in a TabItem, this occurs every time a tab is switched
		}

		private void SaveToFile(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog();

			if (dlg.ShowDialog() != true) return;
			try
			{
				File.WriteAllText(dlg.FileName, Text);
			}
			catch
			{
			}
		}
	}
}
