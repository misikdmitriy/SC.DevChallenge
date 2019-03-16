using System;
using Newtonsoft.Json;
using SC.DevChallenge.Api.Converters;

namespace SC.DevChallenge.Api.Models
{
    public class AveragePriceModel
    {
        public decimal Price { get; }

        [JsonConverter(typeof(DateFormatConverter), "MM/dd/yyyy HH:mm:ss")]
        public DateTime DateTime { get; }

        public AveragePriceModel(DateTime dateTime, decimal price)
        {
            DateTime = dateTime;
            Price = price;
        }
    }
}
