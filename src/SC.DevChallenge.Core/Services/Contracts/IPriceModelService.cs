using System;
using System.Threading.Tasks;
using SC.DevChallenge.Core.Models;

namespace SC.DevChallenge.Core.Services.Contracts
{
    public interface IPriceModelService
    {
        Task<PriceModelResult> GetAverage(int timeSlot,
            string instrumentOwner = null, string instrument = null, string portfolio = null);

        Task<decimal?> GetAverage(DateTime start, DateTime end,
            string instrumentOwner = null, string instrument = null, string portfolio = null);

        Task<PriceModelResult> GetBenchmark(int timeSlot,
            string instrumentOwner = null, string instrument = null, string portfolio = null);

        Task<decimal?> GetBenchmark(DateTime start, DateTime end,
                string instrumentOwner = null, string instrument = null, string portfolio = null);
    }
}
