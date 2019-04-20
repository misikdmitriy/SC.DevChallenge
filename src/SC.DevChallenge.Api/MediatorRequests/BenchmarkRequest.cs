using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SC.DevChallenge.Api.Extensions;
using SC.DevChallenge.Api.Models;
using SC.DevChallenge.Core.Services.Contracts;

namespace SC.DevChallenge.Api.MediatorRequests
{
    public class BenchmarkRequest : IRequest<ApiPriceModel>
    {
        public string Portfolio { get; }
        public string Date { get; }

        public BenchmarkRequest(string portfolio, string date)
        {
            Portfolio = portfolio;
            Date = date;
        }
    }

    public class BenchmarkResultHandler : IRequestHandler<BenchmarkRequest, ApiPriceModel>
    {
        private readonly IDateTimeConverter _converter;
        private readonly IPriceModelService _priceModelService;

        public BenchmarkResultHandler(IDateTimeConverter converter,
            IPriceModelService priceModelService)
        {
            _converter = converter;
            _priceModelService = priceModelService;
        }

        public async Task<ApiPriceModel> Handle(BenchmarkRequest request, CancellationToken cancellationToken)
        {
            var date = request.Date.Parse();

            var timeSlot = _converter.DateTimeToTimeSlot(date);

            var isPortfolioEmpty = string.IsNullOrEmpty(request.Portfolio);

            if (isPortfolioEmpty)
            {
                throw new ArgumentNullException(nameof(request.Portfolio));
            }

            var benchmark = await _priceModelService.GetBenchmark(timeSlot,
                portfolio: request.Portfolio);

            return new ApiPriceModel(benchmark.Start, benchmark.Price);
        }
    }
}
