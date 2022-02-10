using System;

namespace Textrude;

/// <summary>
///     Miscellaneous operations for the application
/// </summary>
public class Helpers
{
    public Action<string> ExitHandler { get; set; } = ExitApplication;

    private static void ExitApplication(string message)
    {
        Console.Error.WriteLine(message);

        Environment.Exit(1);
    }

    /// <summary>
    ///     Attempt to perform an operation or quit if error detected
    /// </summary>
    public T GetOrQuit<T>(Func<T> a, string message)
    {
        try
        {
            return a();
        }
        catch
        {
            ExitHandler(message);
        }

        //keep the compiler happy since it doesn't recognize Environment.Exit
        throw new ApplicationException();
    }

    public void TryOrQuit(Action a, string message)
    {
        GetOrQuit(() =>
        {
            a();
            return 1;
        }, message);
    }
}
