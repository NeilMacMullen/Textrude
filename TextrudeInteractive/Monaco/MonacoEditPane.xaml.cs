using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive.Monaco
{
    /// <summary>
    ///     Interaction logic for MonacoEditPane.xaml
    /// </summary>
    public partial class MonacoEditPane : UserControl
    {
        private readonly MonacoBinding _monacoBinding;

        public MonacoEditPane()
        {
            InitializeComponent();
            _monacoBinding = new MonacoBinding(WebView, false) {OnUserInput = _ => OnUserInput()};
            _monacoBinding.Initialize().ConfigureAwait(false);
        }


        private void OnUserInput()
        {
        }

        #region Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MonacoEditPane), new
                PropertyMetadata("", OnTextChanged));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject dp,
            DependencyPropertyChangedEventArgs e)
        {
            var pane = dp as MonacoEditPane;
            if (e.NewValue == null)
                return;
            pane._monacoBinding.Text = e.NewValue.ToString();
        }

        #endregion

        #region Font

        public static readonly DependencyProperty TextSizeProperty =
            DependencyProperty.Register("TextSize", typeof(double), typeof(MonacoEditPane), new
                PropertyMetadata(12.0, OnTextSizeChanged));

        public double TextSize
        {
            get => (double) GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        private static void OnTextSizeChanged(DependencyObject dp,
            DependencyPropertyChangedEventArgs e)
        {
            var pane = dp as MonacoEditPane;
            if (e.NewValue == null)
                return;
            pane._monacoBinding.SetTextSize((double) e.NewValue);
        }

        #endregion

        #region LineNumbers

        public static readonly DependencyProperty LineNumbersProperty =
            DependencyProperty.Register("LineNumbers", typeof(bool), typeof(MonacoEditPane), new
                PropertyMetadata(true, OnLineNumbersChanged));

        public bool LineNumbers
        {
            get => (bool) GetValue(LineNumbersProperty);
            set => SetValue(LineNumbersProperty, value);
        }

        private static void OnLineNumbersChanged(DependencyObject dp,
            DependencyPropertyChangedEventArgs e)
        {
            var pane = dp as MonacoEditPane;
            if (e.NewValue == null)
                return;
            pane._monacoBinding.SetLineNumbers((bool) e.NewValue);
        }

        #endregion


        #region WordWrap

        public static readonly DependencyProperty WordWrapProperty =
            DependencyProperty.Register("WordWrap", typeof(bool), typeof(MonacoEditPane), new
                PropertyMetadata(true, OnWordWrapChanged));

        public bool WordWrap
        {
            get => (bool) GetValue(WordWrapProperty);
            set => SetValue(WordWrapProperty, value);
        }

        private static void OnWordWrapChanged(DependencyObject dp,
            DependencyPropertyChangedEventArgs e)
        {
            var pane = dp as MonacoEditPane;
            if (e.NewValue == null)
                return;
            pane._monacoBinding.SetWordWrap((bool) e.NewValue);
        }

        #endregion

        #region Format

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(MonacoEditPane), new
                PropertyMetadata("", OnFormatChanged));

        public string Format
        {
            get => (string) GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }

        private static void OnFormatChanged(DependencyObject dp,
            DependencyPropertyChangedEventArgs e)
        {
            var pane = dp as MonacoEditPane;
            if (e.NewValue == null)
                return;
            pane._monacoBinding.Format = e.NewValue.ToString();
        }

        #endregion
    }
}
