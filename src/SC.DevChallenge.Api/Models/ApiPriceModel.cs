using System;
using Newtonsoft.Json;
using SC.DevChallenge.Api.Converters;

namespace SC.DevChallenge.Api.Models
{
    public class ApiPriceModel
    {
		[JsonConverter(typeof(DecimalFormatConverter), 2)]
        public decimal Price { get; }

        [JsonConverter(typeof(DateTimeFormatConverter), DateTimeFormatConverter.DefaultFormat)]
        public DateTime Date { get; }

        public ApiPriceModel(DateTime date, decimal price)
        {
            Date = date;
            Price = price;
        }
    }
}
