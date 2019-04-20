using System;
using System.Globalization;
using SC.DevChallenge.Api.Converters;

namespace SC.DevChallenge.Api.Extensions
{
    internal static class DateExtensions
    {
        public static DateTime Parse(this string strDate)
        {
            if (!DateTime.TryParseExact(strDate, DateTimeFormatConverter.DefaultFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                throw new FormatException($"Provided date {strDate} is in incorrect format." +
                                            $"Expected format is {DateTimeFormatConverter.DefaultFormat}");
            }

            return date;
        }
    }
}
