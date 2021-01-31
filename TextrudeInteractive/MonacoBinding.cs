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
        private bool _isReady;
        private bool _isReadOnly;
        private CoreWebView2Environment _webEnv;
        private WebView2 _webView;
        private Queue<Messages> _messagesToBeDelvivered;

        public MonacoBinding(WebView2 webView, bool isReadOnly)
        {
            _webView = webView;
            _isReadOnly = isReadOnly;
            _isReady = false;
            _messagesToBeDelvivered = new Queue<Messages>();
        }

        public Action<MonacoBinding> OnUserInput = _ => { };

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    PostMessage(new Messages.UpdateText(Text));
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
                    PostMessage(new Messages.UpdateLanguage(Format));
                }
            }
        }


        public async Task Initialize()
        {
            _webEnv = await CoreWebView2Environment.CreateAsync();
            await _webView.EnsureCoreWebView2Async(_webEnv);

            _webView.CoreWebView2.AddWebResourceRequestedFilter("http://*", CoreWebView2WebResourceContext.All);
            _webView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
            _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            _webView.CoreWebView2.Navigate(monacoBaseUri);
        }

        public void OpenDevTools()
            => _webView.CoreWebView2.OpenDevToolsWindow();

        public static bool IsWebView2RuntimeAvailable()
        {
            try
            {
                var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                return !string.IsNullOrWhiteSpace(version);
            }
            catch (Exception)
            {
                return false;
            }
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

        private void PostMessage(Messages msg)
        {
            if (_isReady)
                _webView.CoreWebView2.PostWebMessageAsJson(msg.ToJson());
            else
                _messagesToBeDelvivered.Enqueue(msg);
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = JsonDocument.Parse(e.WebMessageAsJson);
            var type = json.RootElement.GetProperty("type").GetString();
            switch (type)
            {
                case nameof(Messages.Ready):
                    _isReady = true;
                    PostMessage(new Messages.Setup(_isReadOnly, Format));
                    while (_messagesToBeDelvivered.TryDequeue(out var delayedMsg))
                    {
                        PostMessage(delayedMsg);
                    }
                    break;
                case nameof(Messages.UpdatedText):
                    var msg = Messages.UpdatedText.FromJson(e.WebMessageAsJson);
                    _text = msg.Text;
                    OnUserInput(this);
                    break;
                default:
                    break;
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

        private abstract record Messages
        {
            protected Messages()
            {
                Type = GetType().Name;
            }

            [JsonPropertyName("type")]
            public string Type { get; }

            public string ToJson() => JsonSerializer.Serialize(this, GetType());

            public record Ready : Messages
            {
                public static Ready FromJson(string json)
                    => JsonSerializer.Deserialize<Ready>(json);
            }

            public record Setup : Messages
            {
                public Setup(bool isReadOnly, string format)
                {
                    IsReadOnly = isReadOnly;
                    Language = format;
                }

                [JsonPropertyName("isReadOnly")]
                public bool IsReadOnly { get; }
                [JsonPropertyName("language")]
                public string Language { get; }
            }

            public record UpdatedText : Messages
            {
                public UpdatedText(string text)
                {
                    Text = text;
                }

                [JsonPropertyName("text")]
                public string Text { get; }

                public static UpdatedText FromJson(string json)
                    => JsonSerializer.Deserialize<UpdatedText>(json);
            }

            public record UpdateText : Messages
            {
                public UpdateText(string text)
                {
                    Text = text;
                }

                [JsonPropertyName("text")]
                public string Text { get; }
            }

            public record UpdateLanguage : Messages
            {
                public UpdateLanguage(string language)
                {
                    Language = language;
                }

                [JsonPropertyName("language")]
                public string Language { get; }
            }
        }
    }
}
