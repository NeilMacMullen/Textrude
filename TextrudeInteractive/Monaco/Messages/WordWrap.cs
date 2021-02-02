namespace TextrudeInteractive.Monaco.Messages
{
    /// <summary>
    ///     Sent TO Monaco tell it to change fontsize
    /// </summary>
    public record WordWrap : MonacoMessages
    {
        public WordWrap(bool onOff) => Enabled = MonacoOptions.OnOff(onOff);

        public string Enabled { get; }
    }
}
