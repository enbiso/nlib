using System;
using System.Net;

namespace Enbiso.NLib.RestClient
{
    public class RestRequestFailedException: Exception
    {
        public HttpStatusCode StatusCode { get; }

        public RestRequestFailedException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}