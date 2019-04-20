using System;
using System.Collections.Generic;
using System.Linq;

namespace SC.DevChallenge.Core.Extensions
{
    public static class StatisticExtensions
    {
        public static decimal CountBenchmark(this IEnumerable<decimal> collection)
        {
            var array = collection.ToArray();

            var (q1, q3) = array.GetQuartiles();

            var iqr = q3 - q1;

            var lowest = q1 - 1.5m * iqr;
            var highest = q3 + 1.5m * iqr;

            return array
                .Where(x => x >= lowest && x <= highest)
                .Average();
        }

        private static (decimal, decimal) GetQuartiles(this IEnumerable<decimal> collection)
        {
            var sorted = collection.OrderBy(x => x).ToArray();

            if (!sorted.Any())
            {
                throw new ArgumentNullException(nameof(collection),
                    "Provided collection are empty or null");
            }

            var count = sorted.Length;

            var q1 = (int)Math.Ceiling((count - 1.0) / 4.0);
            var q3 = (int)Math.Ceiling((3.0 * count - 3.0) / 4.0);

            return (sorted[q1], sorted[q3]);
        }
    }
}
