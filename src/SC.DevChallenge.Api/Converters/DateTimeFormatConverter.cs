using System.Globalization;
using Newtonsoft.Json.Converters;

namespace SC.DevChallenge.Api.Converters
{
    public class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public const string DefaultFormat = "dd/MM/yyyy HH:mm:ss";

        public DateTimeFormatConverter(string format)
        {
            DateTimeFormat = format;
            Culture = CultureInfo.InvariantCulture;
        }
    }
}
