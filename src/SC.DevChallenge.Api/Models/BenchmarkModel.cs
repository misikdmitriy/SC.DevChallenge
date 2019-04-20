using System;
using Newtonsoft.Json;
using SC.DevChallenge.Api.Converters;

namespace SC.DevChallenge.Api.Models
{
    public class BenchmarkModel
    {
        [JsonConverter(typeof(DecimalFormatConverter), 2)]
        public decimal Price { get; }

        [JsonConverter(typeof(DateTimeFormatConverter), DateTimeFormatConverter.DefaultFormat)]
        public DateTime Date { get; }

        public BenchmarkModel(DateTime date, decimal price)
        {
            Date = date;
            Price = price;
        }
    }
}
