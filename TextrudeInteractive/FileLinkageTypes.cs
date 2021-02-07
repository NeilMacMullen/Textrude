using System;

namespace TextrudeInteractive
{
    [Flags]
    public enum FileLinkageTypes
    {
        None = 0,
        Load = (1 << 0),
        Save = (1 << 1),
        Clipboard = (1 << 2),
        LoadSave = Load | Save,
        SaveAndClipboard = Save | Clipboard,
    }
}
