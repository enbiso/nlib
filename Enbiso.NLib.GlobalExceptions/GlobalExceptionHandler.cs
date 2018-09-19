using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.Exceptions
{
    /// <summary>
    /// Global exception handler interface
    /// </summary>
    public interface IGlobalExceptionHandler
    {
        Type ValidType { get; }

        Task<GlobalExceptionResponse> HandleException(Exception exception);
    }

    /// <summary>
    /// Abstract Global exception handler
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    public abstract class AbstractGlobalExceptionHandler<TException> : IGlobalExceptionHandler
        where TException : Exception
    {
        public Type ValidType => typeof(TException);
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