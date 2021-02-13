using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using TextrudeInteractive.Monaco.Messages;

namespace TextrudeInteractive
{
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
        private readonly Queue<MonacoMessages> _messagesToBeDelivered;
        private readonly MonacoResourceFetcher _resources;
        private readonly WebView2 _webView;
        private string _format = string.Empty;
        private bool _isReady;

        private string _text = string.Empty;
        private CoreWebView2Environment _webEnv;

        public Action<MonacoBinding> OnUserInput = _ => { };

        public MonacoBinding(WebView2 webView, bool isReadOnly)
        {
            _resources = MonacoResourceFetcher.Instance;
            _webView = webView;
            _isReadOnly = isReadOnly;
            _isReady = false;
            _messagesToBeDelivered = new Queue<MonacoMessages>();
        }

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    PostMessage(new UpdateText(Text));
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
                PostMessage(new UpdateLanguage(Format));
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


        public ImmutableArray<string> GetSupportedFormats() => _resources.GetSupportedFormats();

        private void PostMessage(MonacoMessages msg)
        {
            if (_isReady)
                _webView.CoreWebView2.PostWebMessageAsJson(msg.ToJson());
            else
                _messagesToBeDelivered.Enqueue(msg);
        }


        private void OnReady()
        {
            _isReady = true;
            PostMessage(new Setup(_isReadOnly, Format));
            while (_messagesToBeDelivered.TryDequeue(out var delayedMsg))
            {
                PostMessage(delayedMsg);
            }

            //reset control size to fill window to try and avoid white flash
            _webView.Width = double.NaN;
            _webView.Height = double.NaN;
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = JsonDocument.Parse(e.WebMessageAsJson);
            var type = json.RootElement.GetProperty(nameof(MonacoMessages.Type)).GetString();
            switch (type)
            {
                case nameof(Ready):
                    OnReady();
                    break;

                case nameof(UpdatedText):
                    var msg = MonacoMessages.FromJson<UpdateText>(e.WebMessageAsJson);
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
                return;
            }


            if (e.Request.Uri.StartsWith($"{MonacoBaseUri}vs"))
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

        public void SetTextSize(double textSize)
        {
            PostMessage(new FontSize(textSize));

            var msg = new UpdateCompletions(
                new Completions(
                    new[]
                    {
                        new CompletionNode($"abcd {textSize}", "def"),
                        new CompletionNode($"hhhh {textSize}", "xyz"),
                    }));
            var json = JsonSerializer.Serialize(msg, new JsonSerializerOptions {WriteIndented = true});
            PostMessage(msg);
        }

        public void SetLineNumbers(bool onOff)
        {
            PostMessage(new LineNumbers(onOff));
        }

        public void SetWordWrap(bool onOff)
        {
            PostMessage(new WordWrap(onOff));
        }


        public void Setup(string format, bool onOff)
        {
            PostMessage(new Setup(onOff, format));
        }

        public void SetWhitespace(bool onOff)
        {
            PostMessage(new RenderWhitespace(onOff
                ? RenderWhitespace.MonacoWhitespaceType.Boundary
                : RenderWhitespace.MonacoWhitespaceType.None));
        }
    }
}
