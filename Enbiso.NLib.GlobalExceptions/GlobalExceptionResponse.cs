using System;

namespace Enbiso.NLib.GlobalExceptions
{
    /// <summary>
    /// Exception handler response with type
    /// </summary>
    public class GlobalExceptionResponse
    {
        public object Content { get; set; }
        public int StatusCode { get; set; }
        public GlobalExceptionResponse(object content, int statusCode)
        {
            Content = content;
            StatusCode = statusCode;
        }
        
    }
}