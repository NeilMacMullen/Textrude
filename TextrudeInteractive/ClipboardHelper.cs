using System.Windows;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Simple class to wrap Windows Clipboard flakiness
    /// </summary>
    public static class ClipboardHelper
    {
        public static void CopyToClipboard(string text)
        {
            var maxAttempts = 3;
            for (var i = 0; i < maxAttempts; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    return;
                }
                catch
                {
                }
            }
        }
    }
}
