using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.GlobalExceptions
{
    /// <summary>
    /// Global exception handler interface
    /// </summary>
    public interface IGlobalExceptionHandler
    {
        Type[] ValidTypes { get; }

        Task<GlobalExceptionResponse> HandleException(Exception exception);
    }

    /// <summary>
    /// Abstract Global exception handler
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    public abstract class GlobalExceptionHandler<TException> : IGlobalExceptionHandler
        where TException : Exception
    {
        public Type[] ValidTypes => new [] { typeof(TException) };
        protected abstract Task<GlobalExceptionResponse> Handle(TException ex);

        public Task<GlobalExceptionResponse> HandleException(Exception exception)
        {
            if (exception is TException ex)
            {
                return Handle(ex);
            }
            return null;
        }

    }
}