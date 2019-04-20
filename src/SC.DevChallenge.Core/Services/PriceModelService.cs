using System;
using System.Linq;
using System.Threading.Tasks;
using SC.DevChallenge.Core.Extensions;
using SC.DevChallenge.Core.Models;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Factories.Contracts;
using SC.DevChallenge.Db.Models;
using SC.DevChallenge.Db.Repositories;

namespace SC.DevChallenge.Core.Services
{
    public class PriceModelService : IPriceModelService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly IDateTimeConverter _converter;

        public PriceModelService(IDbContextFactory<AppDbContext> dbContextFactory,
            IDateTimeConverter converter)
        {
            _dbContextFactory = dbContextFactory;
            _converter = converter;
        }

        public async Task<PriceModelResult> GetAverage(int timeSlot, 
            string instrumentOwner, string instrument, string portfolio)
        {
            var (start, end) = GetTimeSlotScope(timeSlot);

            var average = await GetAverage(start, end, instrumentOwner,
                instrument, portfolio);

            return new PriceModelResult(start, average);
        }

        public async Task<PriceModelResult> GetBenchmark(int timeSlot, 
            string instrumentOwner, string instrument, string portfolio)
        {
            var (start, end) = GetTimeSlotScope(timeSlot);

            var average = await GetBenchmark(start, end, instrumentOwner,
                instrument, portfolio);

            return new PriceModelResult(start, average);
        }

        public async Task<decimal?> GetAverage(DateTime start, DateTime end,
            string instrumentOwner, string instrument, string portfolio)
        {
            var priceModels = await GetPriceModels(start, end,
                instrumentOwner, instrument, portfolio);

            if (!priceModels.Any())
            {
                return null;
            }

            return priceModels.Select(x => x.Price).Average();
        }

        public async Task<decimal?> GetBenchmark(DateTime start, DateTime end,
            string instrumentOwner, string instrument, string portfolio)
        {
            var priceModels = await GetPriceModels(start, end,
                instrumentOwner, instrument, portfolio);

            if (!priceModels.Any())
            {
                return null;
            }

            return priceModels.Select(x => x.Price).CountBenchmark();
        }

        private (DateTime, DateTime) GetTimeSlotScope(int timeSlot)
        {
            return (_converter.GetTimeSlotStartDate(timeSlot),
                _converter.GetTimeSlotStartDate(timeSlot + 1));
        }

        private async Task<PriceModel[]> GetPriceModels(DateTime start, DateTime end,
            string instrumentOwner, string instrument, string portfolio)
        {
            using (var context = _dbContextFactory.CreateContext())
            {
                var repository = new DbRepository<PriceModel>(context);

                var isInstrumentOwnerEmpty = string.IsNullOrEmpty(instrumentOwner);
                var isInstrumentEmpty = string.IsNullOrEmpty(instrument);
                var isPortfolioEmpty = string.IsNullOrEmpty(portfolio);

                var priceModels = await repository.FindAsync(x =>
                    (isInstrumentOwnerEmpty || x.InstrumentOwner.Name == instrumentOwner) &&
                    (isInstrumentEmpty || x.Instrument.Name == instrument) &&
                    (isPortfolioEmpty || x.Portfolio.Name == portfolio) &&
                    x.Date >= start && x.Date < end);

                return priceModels;
            }
        }
    }
}
