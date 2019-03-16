using System.Globalization;
using Newtonsoft.Json.Converters;

namespace SC.DevChallenge.Api.Converters
{
    public class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public const string DefaultFormat = "MM/dd/yyyy HH:mm:ss";

        public DateTimeFormatConverter(string format)
        {
            DateTimeFormat = format;
            Culture = CultureInfo.InvariantCulture;
        }
    }
}
