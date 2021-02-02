using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace TextrudeInteractive
{
    //Supress messages about unused accessors in records 
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    public class MonacoBinding
    {
        // TODO Use Uri instead of string
        private const string MonacoBaseUri = "http://monaco-editor/";


        private static readonly Dictionary<string, string> KnownFileTypes = new()
        {
            [".js"] = "text/javascript",
            [".css"] = "text/css",
            [".ttf"] = "font/ttf",
        };

        private readonly bool _isReadOnly;
        private readonly Queue<Messages> _messagesToBeDelivered;
        private readonly MonacoResourceFetcher _resources = new();
        private readonly WebView2 _webView;
        private string _format = string.Empty;
        private bool _isReady;

        private string _text = string.Empty;
        private CoreWebView2Environment _webEnv;

        public Action<MonacoBinding> OnUserInput = _ => { };

        public MonacoBinding(WebView2 webView, bool isReadOnly)
        {
            _webView = webView;
            _isReadOnly = isReadOnly;
            _isReady = false;
            _messagesToBeDelivered = new Queue<Messages>();
        }

        public string Text
        {
            get => _text;
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
            get => _format;
            set
            {
                if (_format == value) return;
                _format = value;
                PostMessage(new Messages.UpdateLanguage(Format));
            }
        }


        public async Task Initialize()
        {
            _webEnv = await CoreWebView2Environment.CreateAsync();
            await _webView.EnsureCoreWebView2Async(_webEnv);

            _webView.CoreWebView2.AddWebResourceRequestedFilter("http://*", CoreWebView2WebResourceContext.All);
            _webView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
            _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            _webView.CoreWebView2.Navigate(MonacoBaseUri);
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

        public ImmutableArray<string> GetSupportedFormats() => _resources.GetSupportedFormats();

        private void PostMessage(Messages msg)
        {
            if (_isReady)
                _webView.CoreWebView2.PostWebMessageAsJson(msg.ToJson());
            else
                _messagesToBeDelivered.Enqueue(msg);
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
                    while (_messagesToBeDelivered.TryDequeue(out var delayedMsg))
                    {
                        PostMessage(delayedMsg);
                    }

                    //reset control size to fill window to try and avoid white flash
                    _webView.Width = double.NaN;
                    _webView.Height = double.NaN;
                    break;
                case nameof(Messages.UpdatedText):
                    var msg = Messages.UpdatedText.FromJson(e.WebMessageAsJson);
                    _text = msg.Text;
                    OnUserInput(this);
                    break;
            }
        }

        private CoreWebView2WebResourceResponse OkResponse(Stream stream, string type) =>
            _webEnv.CreateWebResourceResponse(
                stream,
                200,
                "OK",
                type
            );

        private void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (e.Request.Uri == MonacoBaseUri)
            {
                e.Response = OkResponse(_resources.Monaco(), string.Empty);
            }
            else if (e.Request.Uri.StartsWith($"{MonacoBaseUri}vs"))
            {
                var path = e.Request.Uri.Replace(MonacoBaseUri, "");

                var response = _resources.FetchPath(path);
                var mimeType = MimeTypeForExtension(Path.GetExtension(path));
                e.Response = OkResponse(response, ContentResponse(response, mimeType));
            }
        }

        private static string MimeTypeForExtension(string extension)
            => KnownFileTypes.TryGetValue(extension, out var known) ? known : "application/octet-stream";

        private static string ContentResponse(Stream content, string mimeType)
            => $"Content-Type: {mimeType}\nContent-Length: {content.Length}";

        private abstract record Messages
        {
            protected Messages() => Type = GetType().Name;

            [JsonPropertyName("type")] public string Type { get; }

            public string ToJson() => JsonSerializer.Serialize(this, GetType());

            // ReSharper disable once ClassNeverInstantiated.Local - message generated by webView
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

                [JsonPropertyName("isReadOnly")] public bool IsReadOnly { get; }

                [JsonPropertyName("language")] public string Language { get; }
            }

            // ReSharper disable once ClassNeverInstantiated.Local - message generated by webView
            public record UpdatedText : Messages
            {
                public UpdatedText(string text) => Text = text;

                [JsonPropertyName("text")] public string Text { get; }

                public static UpdatedText FromJson(string json)
                    => JsonSerializer.Deserialize<UpdatedText>(json);
            }

            public record UpdateText : Messages
            {
                public UpdateText(string text) => Text = text;

                [JsonPropertyName("text")] public string Text { get; }
            }

            public record UpdateLanguage : Messages
            {
                public UpdateLanguage(string language) => Language = language;

                [JsonPropertyName("language")] public string Language { get; }
            }
        }
    }
}
