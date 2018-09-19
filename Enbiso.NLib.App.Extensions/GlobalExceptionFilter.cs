using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enbiso.NLib.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Enbiso.NLib.App.Extensions
{
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IGlobalExceptionHandler> _handlers;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IEnumerable<IGlobalExceptionHandler> handlers)
        {
            _logger = logger;
            _handlers = handlers;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var handler = _handlers.FirstOrDefault(h => h.ValidTypes.Contains(context.Exception.GetType()));
            if (handler != null)
            {
                var result = await handler.HandleException(context.Exception);
                context.Result = new ObjectResult(result.Result);
                context.HttpContext.Response.StatusCode = result.StatusCode;
                context.ExceptionHandled = true;
            }
        }
    }
}
