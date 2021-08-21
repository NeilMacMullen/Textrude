using System;

namespace Engine.Extensions.TimeRange
{
    /// <summary>
    ///     Provides some helpful functions for templates
    /// </summary>
    public static class TimeRangeMethods
    {
        public static bool After(object a, object b) => Recognise(a) > Recognise(b);

        public static bool Before(object a, object b) => Recognise(a) < Recognise(b);

        public static bool Within(object a, string b)
        {
            if (TimeRangeRecogniser.Recognise(b) is TimeRange range)
            {
                var da = Recognise(a);
                return da >= range.Start && da <= range.End;
            }

            throw new ArgumentException($"Unable to interpret '{b}' as a range of time");
        }


        public static DateTime Recognise(object o) =>
            MyRecogniser.RecogniseFromObject(o);
    }
}
