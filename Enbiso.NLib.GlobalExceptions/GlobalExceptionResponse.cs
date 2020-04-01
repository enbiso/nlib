using System;

namespace Enbiso.NLib.GlobalExceptions
{
    /// <summary>
    /// Exception handler response
    /// </summary>
    public class GlobalExceptionResponse: GlobalExceptionResponse<Exception>
    {
        public GlobalExceptionResponse(object content, int statusCode): base(content, statusCode)
        {
        }
    }

    /// <summary>
    /// Exception handler response with type
    /// </summary>
    /// <typeparam name="TException">TypeOfException</typeparam>
    public class GlobalExceptionResponse<TException> where TException: Exception
    {
        public object Content { get; set; }
        public int StatusCode { get; set; }
        public Type Type { get; set; }
        public GlobalExceptionResponse(object content, int statusCode)
        {
            Content = content;
            StatusCode = statusCode;
            Type = typeof(TException);
        }
        
    }
}