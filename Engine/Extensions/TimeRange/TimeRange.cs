using System;
using System.Linq;

namespace Engine.Extensions.TimeRange;

/// <summary>
///     Represents a range of time... Start to End
/// </summary>
public class TimeRange
{
    public static readonly TimeRange Zero = new(DateTime.MinValue, DateTime.MinValue);

    public TimeRange(DateTime t, TimeSpan span) : this(t, t + span)
    {
    }

    public TimeRange(DateTime t, DateTime end)
    {
        Start = t;
        End = end;
    }

    public DateTime End { get; set; }

    public DateTime Start { get; set; }

    public TimeSpan Duration => End - Start;

    public static TimeRange CreateEnclosingRange(DateTime[] startDates) => new(startDates.Min(), startDates.Max());
}
