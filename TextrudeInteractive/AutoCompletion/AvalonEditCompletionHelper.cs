using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Input;
using Engine.Application;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;

namespace TextrudeInteractive.AutoCompletion
{
    /// <summary>
    ///     Helps to add the code-completion behavior to the AvalonEdit control
    /// </summary>
    public class AvalonEditCompletionHelper
    {
        private readonly TextEditor _textEditor;

        private ImmutableArray<CompletionTreeNode> _completionNodes = ImmutableArray<CompletionTreeNode>.Empty;
        private CompletionWindow _completionWindow;

        public AvalonEditCompletionHelper(TextEditor editor) => _textEditor = editor;

        public void Register()
        {
            _textEditor.TextArea.TextEntering += TextEntering;
            _textEditor.TextArea.TextEntered += TextEntered;
        }


        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            var area = sender as TextArea;
            if (e.Text != ".") return;
            var prevText = LeadingString(area);


            var matches = _completionNodes
                .Select(n => n.Find(prevText))
                .SelectMany(m => m.Children)
                .ToArray();

            //only show the popup if there are matches
            if (!matches.Any()) return;

            _completionWindow = new CompletionWindow(area);
            var data = _completionWindow.CompletionList.CompletionData;
            foreach (var child in matches)
            {
                data.Add(new CompletionData(child.Stem));
            }

            _completionWindow.Show();
            _completionWindow.Closed += delegate { _completionWindow = null; };
        }

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length <= 0 || _completionWindow == null) return;

            if (!char.IsLetterOrDigit(e.Text[0]))
            {
                // Whenever a non-letter is typed while the completion window is open,
                // insert the currently selected element.
                _completionWindow.CompletionList.RequestInsertion(e);
            }

            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        /// <summary>
        ///     Get the bit of valid path just before the dot
        /// </summary>
        private static string LeadingString(TextArea area)
        {
            // Open code completion after the user has pressed dot:
            var offset = area.Caret.Offset;
            var currentLine = area.Document.GetLineByOffset(offset);

            var leadingText = area.Document.GetText(currentLine.Offset, offset - currentLine.Offset - 1);

            static bool IsValidChar(char c)
                => char.IsLetterOrDigit(c) || "._".Contains(c);

            var badChars = leadingText.Select((c, i) => !IsValidChar(c) ? i : -1).ToArray();
            if (badChars.Any())
                leadingText = leadingText.Substring(badChars.Max() + 1);
            return leadingText;
        }

        public void SetCompletion(IEnumerable<ModelPath> paths)
        {
            _completionNodes = CompletionTreeNode.Build(paths);
        }
    }
}