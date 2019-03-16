using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SC.DevChallenge.Api.Converters;
using SC.DevChallenge.Api.MediatorRequests;

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

        [HttpGet]
        [Route("average")]
        [ProducesResponseType(typeof(AveragePriceRequest), 200)]
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
