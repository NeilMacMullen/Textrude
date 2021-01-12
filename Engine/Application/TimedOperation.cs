using System.Diagnostics;

namespace Engine.Application
{
    public class TimedOperation<T>
    {
        public readonly Stopwatch Timer = Stopwatch.StartNew();
        public readonly T Value;

        public TimedOperation(T value) => Value = value;
    }
}