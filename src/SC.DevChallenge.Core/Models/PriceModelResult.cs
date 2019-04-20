using System;

namespace SC.DevChallenge.Core.Models
{
    public class PriceModelResult
    {
        public DateTime Start { get; }
        public decimal Price { get; }

        public PriceModelResult(DateTime start, decimal price)
        {
            Price = price;
            Start = start;
        }
    }
}
