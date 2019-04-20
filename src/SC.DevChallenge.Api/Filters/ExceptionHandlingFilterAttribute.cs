using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using SC.DevChallenge.Api.ActionResults;
using SC.DevChallenge.Core.Exceptions;

namespace SC.DevChallenge.Api.Filters
{
    public class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is FormatException fex)
            {
                context.Result = new StatusCodeResult(HttpStatusCode.BadRequest, new
                {
                    message = fex.Message
                });
            }
            else if (exception is ArgumentOutOfRangeException aex)
            {
                context.Result = new StatusCodeResult(HttpStatusCode.BadRequest, new
                {
                    message = aex.Message,
                    actual = aex.ActualValue
                });
            }
            else if (exception is ArgumentNullException ane)
            {
                context.Result = new StatusCodeResult(HttpStatusCode.BadRequest, new
                {
                    message = ane.Message,
                });
            }
            else if (exception is ArgumentException ae)
            {
                context.Result = new StatusCodeResult(HttpStatusCode.BadRequest, new
                {
                    message = ae.Message
                });
            }
            else if (exception is PriceModelAbsentException pmae)
            {
                context.Result = new StatusCodeResult(HttpStatusCode.NotFound, new
                {
                    message = "No price models",
                    date = pmae.Date
                });
            }
        }
    }
}
