using System;

namespace SC.DevChallenge.Core.Exceptions
{
    public class PriceModelAbsentException : Exception
    {
        public DateTime Date { get; }

        public PriceModelAbsentException(DateTime date)
        {
            Date = date;
        }
    }
}
