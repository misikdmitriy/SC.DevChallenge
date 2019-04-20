using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SC.DevChallenge.Api.MediatorRequests;
using SC.DevChallenge.Api.Models;

namespace SC.DevChallenge.Api.Controllers
{
    /// <summary>
    /// Price API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PricesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Return average price model
        /// </summary>
        /// <param name="portfolio">Optional. Portfolio name</param>
        /// <param name="owner">Optional. Owner name</param>
        /// <param name="instrument">Optional. Instrument name</param>
        /// <param name="date">Required. Time slot. Format - dd/MM/yyyy HH:mm:ss</param>
        /// <remarks>At least one filter parameter should be provided (portfolio, owner or instrument)</remarks>
        /// <response code="200">Average price for this period</response>
        /// <response code="400">Invalid date/No filters provided</response>
        /// <response code="404">No data for this period</response>
        [HttpGet]
        [Route("average")]
        [ProducesResponseType(typeof(ApiPriceModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAveragePrice(string portfolio, string owner, 
            string instrument, 
            string date)
        {
            var result = await _mediator.Send(new AveragePriceRequest(instrument, owner, 
                portfolio, date));

            return Ok(result);
        }

        /// <summary>
        /// Return benchmark
        /// </summary>
        /// <param name="portfolio">Required. Portfolio name</param>
        /// <param name="date">Required. Time slot. Format - dd/MM/yyyy HH:mm:ss</param>
        /// <response code="200">Benchmark price for this period</response>
        /// <response code="400">Invalid date/Portfolio not provided</response>
        /// <response code="404">No data for this period</response>
        [HttpGet]
        [Route("benchmark")]
        [ProducesResponseType(typeof(ApiPriceModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBenchmark(string portfolio, string date)
        {
            var result = await _mediator.Send(new BenchmarkRequest(portfolio, date));

            return Ok(result);
        }

        /// <summary>
        /// Return aggregate
        /// </summary>
        /// <param name="portfolio">Required. Portfolio name</param>
        /// <param name="startdate">Required. Start date of time slot. Format - dd/MM/yyyy HH:mm:ss</param>
        /// <param name="enddate">Required. End date of time slot. Format - dd/MM/yyyy HH:mm:ss</param>
        /// <param name="resultpoints">Required. Results point</param>
        /// <response code="200">Aggregated price models for this period</response>
        /// <response code="400">Invalid date/Portfolio not provided/End date is earlier than start date
        /// Result points is not positive/Short time period between start and end date</response>
        /// <response code="404">No data for this period</response>
        [HttpGet]
        [Route("aggregate")]
        [ProducesResponseType(typeof(ApiPriceModel[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAggregate(string portfolio, string startdate,
            string enddate, int resultpoints)
        {
            var result = await _mediator.Send(new AggregateRequest(portfolio, startdate,
                enddate, resultpoints));

            return Ok(result);
        }
    }
}
