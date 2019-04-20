using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                var startDate = request.StartDate.Parse();
                var endDate = request.EndDate.Parse();

                var isPortfolioEmpty = string.IsNullOrEmpty(request.Portfolio);

                if (isPortfolioEmpty)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest,
                        new
                        {
                            message = "Provide portfolio name"
                        });
                }

                if (request.ResultPoints <= 0)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest,
                        new
                        {
                            message = "Result point should be positive",
                            actual = request.ResultPoints
                        });
                }

                if (endDate < startDate)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest,
                        new
                        {
                            message = "Start date should be before end date",
                            startDate = request.StartDate,
                            endDate = request.EndDate
                        });
                }

                var startTimeSlot = _converter.DateTimeToTimeSlot(startDate);
                var endTimeSlot = _converter.DateTimeToTimeSlot(endDate);

                var timeSlotsCount = endTimeSlot - startTimeSlot + 1;

                if (timeSlotsCount < request.ResultPoints)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest,
                        new
                        {
                            message = "Time slots between start and end date less than " +
                                      "result points requested",
                            timeSlotsCount,
                            resultPoints = request.ResultPoints
                        });
                }

                var result = new List<ApiPriceModel>();

                for (var i = 0; i < request.ResultPoints; i++)
                {
                    var leftTimeSlot = startTimeSlot + i * timeSlotsCount / request.ResultPoints;
                    var rightTimeSlot = startTimeSlot + (i + 1) * timeSlotsCount / request.ResultPoints;

                    var averages = new List<decimal>();
                    for (var j = leftTimeSlot; j < rightTimeSlot; j++)
                    {
                        var start = _converter.GetTimeSlotStartDate(j);
                        var end = _converter.GetTimeSlotStartDate(j + 1);

                        var average = await _priceModelService.GetAverage(start, end,
                            portfolio: request.Portfolio);

                        if (average.HasValue)
                        {
                            averages.Add(average.Value);
                        }
                    }

                    var date = _converter.GetTimeSlotStartDate(rightTimeSlot - 1);

                    if (!averages.Any())
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound,
                            new
                            {
                                message = "No price models",
                                date
                            });
                    }

                    var priceModel = new ApiPriceModel(date,
                        averages.Average());

                    result.Add(priceModel);
                }

                return result.ToArray();
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
