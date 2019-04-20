using System;
using System.Collections.Generic;
using System.Linq;

namespace SC.DevChallenge.Api.Extensions
{
    public static class QuartileExtensions
    {
        public static (decimal, decimal, decimal) TakeQuartiles(this IEnumerable<decimal> collection)
        {
            var sorted = collection.OrderBy(x => x).ToArray();

            if (!sorted.Any())
            {
                throw new ArgumentNullException(nameof(collection),
                    "Provided collection are empty or null");
            }

            var count = sorted.Length;

            var q1 = (int)Math.Ceiling((count - 1.0) / 4.0);
            var q2 = (int) Math.Ceiling((count - 1.0) / 2.0);
            var q3 = (int)Math.Ceiling(3.0* (count - 1.0) / 4.0);

            return (sorted[q1], sorted[q2], sorted[q3]);
        }
    }
}
