using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SC.DevChallenge.Api.Exceptions;
using SC.DevChallenge.Api.Models;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Models;
using SC.DevChallenge.Db.Repositories.Contracts;

namespace SC.DevChallenge.Api.MediatorRequests
{
    public class AveragePriceRequest : IRequest<AveragePriceModel>
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

    public class AveragePriceResultHandler : IRequestHandler<AveragePriceRequest, AveragePriceModel>
    {
        private readonly IDbRepository<PriceModel> _repository;
        private readonly IDateTimeConverter _converter;

        public AveragePriceResultHandler(IDbRepository<PriceModel> repository, 
            IDateTimeConverter converter)
        {
            _repository = repository;
            _converter = converter;
        }

        public async Task<AveragePriceModel> Handle(AveragePriceRequest request, CancellationToken cancellationToken)
        {
            DateTime date;
            if (!DateTime.TryParse(request.Date, out date))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, 
                    "Provided date is in incorrect format");
            }

            var timeSlot = _converter.DateTimeToTimeSlot(date);

            var start = _converter.GetTimeSlotStartDate(timeSlot);
            var end = _converter.GetTimeSlotStartDate(timeSlot + 1);

            var isInstrumentOwnerEmpty = string.IsNullOrEmpty(request.InstrumentOwner);
            var isInstrumentEmpty = string.IsNullOrEmpty(request.Instrument);
            var isPortfolioEmpty = string.IsNullOrEmpty(request.Portfolio);

            if (isInstrumentOwnerEmpty && isInstrumentEmpty && isPortfolioEmpty)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, 
                    "Provide at least one filter");
            }

            var priceModels = await _repository.FindAsync(x => (isInstrumentOwnerEmpty || x.InstrumentOwner.Name == request.InstrumentOwner) &&
                                                               (isInstrumentEmpty || x.Instrument.Name == request.Instrument) &&
                                                               (isPortfolioEmpty || x.Portfolio.Name == request.Portfolio) &&
                                                               x.Date >= start && x.Date < end);
            if (!priceModels.Any())
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, 
                    "No price models");
            }

            return new AveragePriceModel(start, priceModels.Select(x => x.Price).Average());
        }
    }
}
