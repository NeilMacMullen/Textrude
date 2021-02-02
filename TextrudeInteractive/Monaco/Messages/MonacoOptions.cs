namespace TextrudeInteractive.Monaco.Messages
{
    public static class MonacoOptions
    {
        public const string On = "on";
        public const string Off = "off";

        public static string OnOff(in bool onOff) => onOff ? On : Off;
    }
}
