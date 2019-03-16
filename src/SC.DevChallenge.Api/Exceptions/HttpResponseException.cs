using System;
using System.Net;
using Newtonsoft.Json;

namespace SC.DevChallenge.Api.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode Code { get; }

        public HttpResponseException(HttpStatusCode code)
        {
            Code = code;
        }

        public HttpResponseException(HttpStatusCode code, string message) : base(message)
        {
            Code = code;
        }

        public HttpResponseException(HttpStatusCode code, object reason) : base(JsonConvert.SerializeObject(reason))
        {
            Code = code;
        }
    }
}
