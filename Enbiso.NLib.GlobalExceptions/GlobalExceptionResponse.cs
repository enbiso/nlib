namespace Enbiso.NLib.Exceptions
{
    /// <summary>
    /// Exception handler response
    /// </summary>
    public class GlobalExceptionResponse
    {
        public GlobalExceptionResponse(object result, int statusCode)
        {
            Result = result;
            StatusCode = statusCode;
        }
        public object Result { get; set; }
        public int StatusCode { get; set; }
    }
}