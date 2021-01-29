using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
		// TODO Use Uri instead of string
		private static readonly string monacoBaseUri = "http://monaco-editor/";

		private string _format = string.Empty;
		private string _text = string.Empty;
		private CoreWebView2Environment _webEnv;

		public OutputMonacoPane()
		{
			InitializeComponent();
			InitWebView();

			FormatSelection.ItemsSource = GetMonacoSupportedFormats();
			FormatSelection.SelectedIndex = 0;
		}

		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				SetText(_text);
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
					FormatSelection.SelectedItem = _format;
				}
			}
		}

		private async void InitWebView()
		{
			_webEnv = await CoreWebView2Environment.CreateAsync();
			await WebView.EnsureCoreWebView2Async(_webEnv);

			WebView.CoreWebView2.AddWebResourceRequestedFilter("http://*", CoreWebView2WebResourceContext.All);
			WebView.CoreWebView2.WebResourceRequested += OnWebResourceRequested; ;

			WebView.CoreWebView2.Navigate(monacoBaseUri);
#if DEBUG
			WebView.CoreWebView2.OpenDevToolsWindow();
#endif
		}

		private void OnWebResourceRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs e)
		{
			if (e.Request.Uri == monacoBaseUri)
			{
				e.Response = _webEnv.CreateWebResourceResponse(
					new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Properties.Resources.monaco)),
					200,
					"OK",
					""
				);
			}
			else if (e.Request.Uri.StartsWith($"{monacoBaseUri}vs"))
			{
				var path = e.Request.Uri.Replace(monacoBaseUri, "");
				using (var zipStream = new MemoryStream(Properties.Resources.monaco_editor_0_21_2))
				{
					using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Read))
					{
						var file = zip.GetEntry(path);
						var response = new MemoryStream(); // cache into local stream so is not disposed
						file.Open().CopyTo(response);
						response.Position = 0;

						string mimeType;
						switch (Path.GetExtension(path))
						{
							case ".js":
								mimeType = "text/javascript";
								break;
							case ".css":
								mimeType = "text/css";
								break;
							case ".ttf":
								mimeType = "font/ttf";
								break;
							default:
								mimeType = "application/octet-stream";
								break;
						}

						e.Response = _webEnv.CreateWebResourceResponse(
							response,
							200,
							"OK",
							$"Content-Type: {mimeType}\nContent-Length: {response.Length}"
						);
					}
				}
			}
		}

		private void SetText(string str)
		{
			if (WebView != null && WebView.CoreWebView2 != null)
			{
				WebView.CoreWebView2.PostWebMessageAsJson(
					new UpdateMonacoTextMessage(str, Format).ToJson()
				);
			}
		}

		private void FormatSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Format = FormatSelection.SelectedItem as string;
			if (WebView != null && WebView.CoreWebView2 != null)
			{
				WebView.CoreWebView2.PostWebMessageAsJson(
					new UpdateMonacoLanguageMessage(Format).ToJson()
				);
			}
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

		private static List<string> GetMonacoSupportedFormats()
		{
			var monacoLangRegex = new System.Text.RegularExpressions.Regex(@"vs/basic-languages/(?<name>.+)/$");
			using (var zipStream = new MemoryStream(Properties.Resources.monaco_editor_0_21_2))
			{
				using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Read))
				{
					var langs = zip.Entries
						.Select(e => monacoLangRegex.Match(e.FullName))
						.Where(m => m.Success)
						.Select(m => m.Groups["name"].Value)
						.ToList();
					return langs;
				}
			}
		}

		private record UpdateMonacoTextMessage
		{
			public UpdateMonacoTextMessage(string text, string language)
			{
				Text = text;
				Language = language;
			}

			[JsonPropertyName("type")]
			public string Type { get; } = "UpdateText";
			[JsonPropertyName("text")]
			public string Text { get; }
			[JsonPropertyName("language")]
			public string Language { get; }

			public string ToJson() => JsonSerializer.Serialize(this);
		}

		private record UpdateMonacoLanguageMessage
		{
			public UpdateMonacoLanguageMessage(string language)
			{
				Language = language;
			}

			[JsonPropertyName("type")]
			public string Type { get; } = "UpdateLanguage";
			[JsonPropertyName("language")]
			public string Language { get; }

			public string ToJson() => JsonSerializer.Serialize(this);
		}
	}
}
