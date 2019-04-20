using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SC.DevChallenge.Api.Exceptions;
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
            try
            {
                var date = request.Date.Parse();

                var timeSlot = _converter.DateTimeToTimeSlot(date);

                var isPortfolioEmpty = string.IsNullOrEmpty(request.Portfolio);

                if (isPortfolioEmpty)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest,
                        new
                        {
                            message = "Provide portfolio name"
                        });
                }

                var benchmark = await _priceModelService.GetBenchmark(timeSlot,
                    portfolio: request.Portfolio);

                if (!benchmark.Price.HasValue)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound,
                        new
                        {
                            message = "No price models",
                            date = benchmark.Start
                        });
                }

                return new ApiPriceModel(benchmark.Start, benchmark.Price.Value);
            }
            catch (FormatException fex)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, new
                {
                    message = fex.Message
                });
            }
            catch (ArgumentOutOfRangeException aex)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, new
                {
                    message = aex.Message,
                    actual = aex.ActualValue
                });
            }
        }
    }
}
