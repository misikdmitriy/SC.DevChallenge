using System;
using Newtonsoft.Json;
using SC.DevChallenge.Api.Converters;

namespace SC.DevChallenge.Api.Models
{
    public class AveragePriceModel
    {
        public decimal Price { get; }

        [JsonConverter(typeof(DateTimeFormatConverter), DateTimeFormatConverter.DefaultFormat)]
        public DateTime DateTime { get; }

        public AveragePriceModel(DateTime dateTime, decimal price)
        {
            DateTime = dateTime;
            Price = price;
        }
    }
}
