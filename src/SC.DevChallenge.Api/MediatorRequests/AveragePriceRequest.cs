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
    public class AveragePriceRequest : IRequest<ApiPriceModel>
    {
        public string Portfolio { get; }
        public string Instrument { get; }
        public string InstrumentOwner { get; }
        public string Date { get; }

        public AveragePriceRequest(string instrument, string instrumentOwner, string portfolio,
            string date)
        {
            Instrument = instrument;
            InstrumentOwner = instrumentOwner;
            Portfolio = portfolio;
            Date = date;
        }
    }

    public class AveragePriceResultHandler : IRequestHandler<AveragePriceRequest, ApiPriceModel>
    {
        private readonly IDateTimeConverter _converter;
        private readonly IPriceModelService _priceModelService;

        public AveragePriceResultHandler(IDateTimeConverter converter, 
            IPriceModelService priceModelService)
        {
            _converter = converter;
            _priceModelService = priceModelService;
        }

        public async Task<ApiPriceModel> Handle(AveragePriceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var date = request.Date.Parse();

                var timeSlot = _converter.DateTimeToTimeSlot(date);

                var isInstrumentOwnerEmpty = string.IsNullOrEmpty(request.InstrumentOwner);
                var isInstrumentEmpty = string.IsNullOrEmpty(request.Instrument);
                var isPortfolioEmpty = string.IsNullOrEmpty(request.Portfolio);

                if (isInstrumentOwnerEmpty && isInstrumentEmpty && isPortfolioEmpty)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest,
                        new
                        {
                            message = "Provide at least one filter"
                        });
                }

                var average = await _priceModelService.GetAverage(timeSlot, 
                    request.InstrumentOwner, request.Instrument, request.Portfolio);

                if (!average.Price.HasValue)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound,
                        new
                        {
                            message = "No price models",
                            date = average.Start
                        });
                }

                return new ApiPriceModel(average.Start, average.Price.Value);
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
