using System;
using System.Net;
using Newtonsoft.Json;

namespace SC.DevChallenge.Api.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode Code { get; }

        public HttpResponseException(HttpStatusCode code, object reason) : base(JsonConvert.SerializeObject(reason))
        {
            Code = code;
        }
    }
}
