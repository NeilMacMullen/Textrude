using System.IO;

namespace TextrudeInteractive;

public static class FileManager
{
    public static bool TryLoadFile(string path, out string text)
    {
        text = string.Empty;
        try
        {
            text = File.ReadAllText(path);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public static bool TrySave(string path, string text)
    {
        try
        {
            File.WriteAllText(path, text);

            return true;
        }
        catch
        {
        }

        return false;
    }
}
