using System.Threading.Tasks;

namespace Enbiso.NLib.Exceptions
{
    public static class GlobalExceptionHandlerExtensions
    {
        public static Task<GlobalExceptionResponse> BadRequest(this IGlobalExceptionHandler handler, object result)
        {
            return Task.FromResult(new GlobalExceptionResponse(result, 400));
        }

        public static Task<GlobalExceptionResponse> UnProcessableEntity(this IGlobalExceptionHandler handler, object result)
        {
            return Task.FromResult(new GlobalExceptionResponse(result, 422));
        }

        public static Task<GlobalExceptionResponse> NotFound(this IGlobalExceptionHandler handler, object result)
        {
            return Task.FromResult(new GlobalExceptionResponse(result, 404));
        }

        public static Task<GlobalExceptionResponse> Ok(this IGlobalExceptionHandler handler, object result)
        {
            return Task.FromResult(new GlobalExceptionResponse(result, 200));
        }

        public static Task<GlobalExceptionResponse> InternalServerError(this IGlobalExceptionHandler handler, object result)
        {
            return Task.FromResult(new GlobalExceptionResponse(result, 500));
        }
    }
}
