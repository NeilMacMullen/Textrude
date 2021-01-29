using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TextrudeInteractive
{
	public class MonacoBinding
	{
		// TODO Use Uri instead of string
		private static readonly string monacoBaseUri = "http://monaco-editor/";

		private string _text;
		private string _format;
		private CoreWebView2Environment _webEnv;
		private WebView2 _webView;

		public MonacoBinding(WebView2 webView)
		{
			_webView = webView;
		}

		public string Text
		{
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					if (_webView != null && _webView.CoreWebView2 != null)
					{
						_webView.CoreWebView2.PostWebMessageAsJson(
							new UpdateMonacoTextMessage(Text, Format).ToJson()
						);
					}
				}
			}
		}

		public string Format
		{
			get { return _format; }
			set
			{
				if (_format != value)
				{
					_format = value;
					if (_webView != null && _webView.CoreWebView2 != null)
					{
						_webView.CoreWebView2.PostWebMessageAsJson(
							new UpdateMonacoLanguageMessage(Format).ToJson()
						);
					}
				}
			}
		}


		public async Task Initialize()
		{
			_webEnv = await CoreWebView2Environment.CreateAsync();
			await _webView.EnsureCoreWebView2Async(_webEnv);

			_webView.CoreWebView2.AddWebResourceRequestedFilter("http://*", CoreWebView2WebResourceContext.All);
			_webView.CoreWebView2.WebResourceRequested += OnWebResourceRequested; ;

			_webView.CoreWebView2.Navigate(monacoBaseUri);
#if DEBUG
			_webView.CoreWebView2.OpenDevToolsWindow();
#endif
		}

		public static List<string> GetSupportedFormats()
		{
			var monacoLangRegex = new System.Text.RegularExpressions.Regex(@"vs/(basic-languages|language)/(?<name>.+)/$");
			using (var zipStream = new MemoryStream(Properties.Resources.monaco_editor_0_21_2))
			{
				using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Read))
				{
					var langs = zip.Entries
						.Select(e => monacoLangRegex.Match(e.FullName))
						.Where(m => m.Success)
						.Select(m => m.Groups["name"].Value)
						.Concat(new[] { "text" })
						.OrderBy(l => l)
						.Distinct()
						.ToList();
					return langs;
				}
			}
		}

		private void OnWebResourceRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs e)
		{
			if (e.Request.Uri == monacoBaseUri)
			{
				e.Response = _webEnv.CreateWebResourceResponse(
					new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.monaco)),
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
