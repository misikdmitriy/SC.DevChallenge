using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SC.DevChallenge.Api.Exceptions;
using SC.DevChallenge.Api.Extensions;
using SC.DevChallenge.Api.Models;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Factories.Contracts;
using SC.DevChallenge.Db.Models;
using SC.DevChallenge.Db.Repositories;

namespace SC.DevChallenge.Api.MediatorRequests
{
    public class BenchmarkRequest : IRequest<BenchmarkModel>
    {
        public string Portfolio { get; }
        public string Date { get; }

        public BenchmarkRequest(string portfolio, string date)
        {
            Portfolio = portfolio;
            Date = date;
        }
    }

    public class BenchmarkResultHandler : IRequestHandler<BenchmarkRequest, BenchmarkModel>
    {
        private IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly IDateTimeConverter _converter;

        public BenchmarkResultHandler(IDbContextFactory<AppDbContext> dbContextFactory, 
            IDateTimeConverter converter)
        {
            _dbContextFactory = dbContextFactory;
            _converter = converter;
        }

        public async Task<BenchmarkModel> Handle(BenchmarkRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var date = request.Date.Parse();

                var timeSlot = _converter.DateTimeToTimeSlot(date);

                var start = _converter.GetTimeSlotStartDate(timeSlot);
                var end = _converter.GetTimeSlotStartDate(timeSlot + 1);

                var isPortfolioEmpty = string.IsNullOrEmpty(request.Portfolio);

                if (isPortfolioEmpty)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest,
                        new
                        {
                            message = "Provide portfolio name"
                        });
                }

                using (var context = _dbContextFactory.CreateContext())
                {
                    var repository = new DbRepository<PriceModel>(context);

                    var priceModels = await repository.FindAsync(
                        x => x.Portfolio.Name == request.Portfolio &&
                             x.Date >= start && x.Date < end);

                    if (!priceModels.Any())
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound,
                            new
                            {
                                message = "No price models",
                                date = start
                            });
                    }

                    var (q1, _, q3) = priceModels.Select(x => x.Price).TakeQuartiles();

                    var iqr = q3 - q1;

                    var lowest = q1 - 1.5m * iqr;
                    var highest = q3 + 1.5m * iqr;

                    var average = priceModels
                        .Where(x => x.Price >= lowest && x.Price <= highest)
                        .Average(x => x.Price);

                    return new BenchmarkModel(start, average);
                }
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
