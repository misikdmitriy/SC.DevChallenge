using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SC.DevChallenge.Api.Extensions;
using SC.DevChallenge.Api.Models;
using SC.DevChallenge.Core.Exceptions;
using SC.DevChallenge.Core.Services.Contracts;

namespace SC.DevChallenge.Api.MediatorRequests
{
    public class AggregateRequest : IRequest<ApiPriceModel[]>
    {
        public string Portfolio { get; }
        public string StartDate { get; }
        public string EndDate { get; }
        public int ResultPoints { get; }

        public AggregateRequest(string portfolio, string startDate, string endDate, int resultPoints)
        {
            EndDate = endDate;
            Portfolio = portfolio;
            ResultPoints = resultPoints;
            StartDate = startDate;
        }
    }

    public class AggregateResultHandler : IRequestHandler<AggregateRequest, ApiPriceModel[]>
    {
        private readonly IDateTimeConverter _converter;
        private readonly IPriceModelService _priceModelService;

        public AggregateResultHandler(IDateTimeConverter converter,
            IPriceModelService priceModelService)
        {
            _converter = converter;
            _priceModelService = priceModelService;
        }

        public async Task<ApiPriceModel[]> Handle(AggregateRequest request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate.Parse();
            var endDate = request.EndDate.Parse();

            var isPortfolioEmpty = string.IsNullOrEmpty(request.Portfolio);

            if (isPortfolioEmpty)
            {
                throw new ArgumentNullException(nameof(request.Portfolio));
            }

            if (request.ResultPoints <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.ResultPoints),
                    request.ResultPoints, "Provide positive value");
            }

            if (endDate < startDate)
            {
                throw new ArgumentOutOfRangeException(nameof(request.EndDate),
                    request.EndDate, $"Provide end date after {request.StartDate}");
            }

            var startTimeSlot = _converter.DateTimeToTimeSlot(startDate);
            var endTimeSlot = _converter.DateTimeToTimeSlot(endDate);

            var timeSlotsCount = endTimeSlot - startTimeSlot + 1;

            if (timeSlotsCount < request.ResultPoints)
            {
                throw new ArgumentException("Time slots between start and end date less than " +
                                            "result points requested." +
                                            $"Time slots - {timeSlotsCount}, " +
                                            $"Result points - {request.ResultPoints}");
            }

            if (timeSlotsCount == 1)
            {
                throw new ArgumentException("Time interval is too short. " +
                                            "Use instead 'benchmark' endpoint");
            }

            var frame = timeSlotsCount / request.ResultPoints;
            var rest = timeSlotsCount % request.ResultPoints;

            var result = new List<ApiPriceModel>();

            var leftTimeSlot = startTimeSlot;

            for (var i = 0; i < request.ResultPoints; i++)
            {
                var rightTimeSlot = leftTimeSlot + frame;

                if (rest > 0)
                {
                    ++rightTimeSlot;
                    --rest;
                }

                var benchmarks = new List<decimal>();
                for (var j = leftTimeSlot; j < rightTimeSlot; j++)
                {
                    try
                    {
                        var benchmark = await _priceModelService.GetBenchmark(j,
                            portfolio: request.Portfolio);

                        benchmarks.Add(benchmark.Price);
                    }
                    catch (PriceModelAbsentException)
                    {
                        // just ignore
                    }
                }

                var date = _converter.GetTimeSlotStartDate(rightTimeSlot - 1);

                if (!benchmarks.Any())
                {
                    throw new PriceModelAbsentException(date);
                }

                var priceModel = new ApiPriceModel(date,
                    benchmarks.Average());

                result.Add(priceModel);

                leftTimeSlot = rightTimeSlot;
            }

            return result.ToArray();
        }
    }
}
