using System.Diagnostics;

namespace Engine.Application
{
    /// <summary>
    ///     Simple Wrapper to allow us to time operations
    /// </summary>
    public class TimedOperation<T>
    {
        public readonly Stopwatch Timer = Stopwatch.StartNew();
        public readonly T Value;

        public TimedOperation(T value) => Value = value;
    }
}