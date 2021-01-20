using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;

namespace AvalonTest
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AvalonEditCompletionHelper _mainEditWindow;

        public MainWindow()
        {
            InitializeComponent();
            _mainEditWindow = new AvalonEditCompletionHelper(textEditor);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainEditWindow.Register();
            var all =
                string.Join(Environment.NewLine,
                    HighlightingManager.Instance.HighlightingDefinitions.Select(hl => hl.Name));
            textEditor.Text = all;
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("asdfasdf");
        }
    }

    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class CompletionData : ICompletionData
    {
        public CompletionData(string text) => Text = text;

        public ImageSource Image => null;

        public string Text { get; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content => Text;

        public object Description => Text;
        public double Priority { get; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }

    public class CompletionTreeNode
    {
        public static readonly CompletionTreeNode Empty =
            new CompletionTreeNode(string.Empty, Array.Empty<CompletionTreeNode>());

        public CompletionTreeNode[] Children = new CompletionTreeNode [0];
        public string Stem = string.Empty;


        public CompletionTreeNode(string stem, IEnumerable<CompletionTreeNode> children)
        {
            Stem = stem;
            Children = children.ToArray();
        }

        public CompletionTreeNode Find(TokenisedPath tokens)
        {
            if (tokens.Length > 0)
            {
                if (tokens.Root == Stem)
                {
                    if (tokens.HasChildren)
                        foreach (var child in Children)
                        {
                            var c = child.Find(tokens.Child());
                            if (c != Empty)
                                return c;
                        }
                    else return this;
                }
            }

            return Empty;
        }

        public static TokenisedPath TokenizePath(string path) => new TokenisedPath(path.Split("."));
        public CompletionTreeNode Find(string path) => Find(TokenizePath(path));

        public static CompletionTreeNode[] Build(IEnumerable<TokenisedPath> paths)
        {
            var nodes = paths
                .Where(p => !p.IsEmpty)
                .GroupBy(p => p.Root)
                .ToArray();
            return nodes.Select(n =>
                new CompletionTreeNode(n.Key, Build(n.Select(p => p.Child())))
            ).ToArray();
        }

        public static CompletionTreeNode[] Build(IEnumerable<string> paths)
            => Build(paths.Select(TokenizePath));
    }

    public class TokenisedPath
    {
        public string[] Tokens;

        public TokenisedPath(string[] tokens) =>
            Tokens = tokens.ToArray();

        public bool IsEmpty => Length == 0;
        public string Root => Tokens[0];
        public bool HasChildren => Tokens.Length > 1;

        public int Length => Tokens.Length;
        public TokenisedPath Child() => new TokenisedPath(Tokens.Skip(1).ToArray());
    }

    public class AvalonEditCompletionHelper
    {
        private readonly TextEditor _textEditor;
        private CompletionWindow completionWindow;

        public AvalonEditCompletionHelper(TextEditor editor) => _textEditor = editor;

        public void Register()
        {
            _textEditor.TextArea.TextEntering += TextEntering;
            _textEditor.TextArea.TextEntered += TextEntered;
        }


        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            var area = sender as TextArea;
            if (e.Text == ".")
            {
                var prevText = LeadingString(area);
                completionWindow = new CompletionWindow(area);
                var data = completionWindow.CompletionList.CompletionData;
                var nodes = CompletionTreeNode.Build(new[]
                {
                    "date.now",
                    "date.add_days",
                    "date.add_months",
                    "date.add_years",
                    "date.add_hours",
                    "date.add_minutes",
                    "date.add_seconds",
                    "date.add_milliseconds",
                    "date.parse",
                    "date.to_string",
                    "string.replace",
                    "string.cat"
                });

                var matches = nodes.Select(n => n.Find(prevText)).ToArray();
                foreach (var child in matches.SelectMany(m => m.Children))
                {
                    data.Add(new CompletionData(child.Stem));
                }

                completionWindow.Show();
                completionWindow.Closed += delegate { completionWindow = null; };
            }
        }

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }

            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private string LeadingString(TextArea area)
        {
            // Open code completion after the user has pressed dot:
            var co = area.Caret.Offset;
            var currentLine = area.Document.GetLineByOffset(co);

            return area.Document.GetText(currentLine.Offset, co - currentLine.Offset - 1);
        }
    }
}