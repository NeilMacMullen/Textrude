using System;

namespace Textrude
{
    public static class Helpers
    {
        public static T TryOrQuit<T>(Func<T> a, string message)
        {
            try
            {
                return a();
            }
            catch
            {
                Console.Error.WriteLine(message);
                Environment.Exit(1);
            }

            //keep the compiler happy since it doesn't recognize Environment.Exit
            throw new ApplicationException();
        }
    }
}