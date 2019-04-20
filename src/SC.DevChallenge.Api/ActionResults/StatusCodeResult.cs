using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SC.DevChallenge.Api.ActionResults
{
    public class StatusCodeResult : IActionResult
    {
        private readonly object _reason;
        private readonly HttpStatusCode _code;

        public StatusCodeResult(HttpStatusCode code, object reason)
        {
            _reason = reason;
            _code = code;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)_code;
            context.HttpContext.Response.ContentType = MediaTypeNames.Application.Json;
            return context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(_reason));
        }
    }
}
