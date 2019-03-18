using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SC.DevChallenge.Api.MediatorRequests;
using SC.DevChallenge.Api.Models;

namespace SC.DevChallenge.Api.Controllers
{
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
		/// <param name="date">Required. Time slot</param>
		/// <remarks>At least one filter parameter should be provided (portfolio, owner or instrument)</remarks>
		/// <response code="200">Average price for this period</response>
		/// <response code="400">Invalid date/no filters provided</response>
		/// <response code="404">No data for this period</response>
		[HttpGet]
        [Route("average")]
        [ProducesResponseType(typeof(AveragePriceModel), 200)]
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
    }
}
