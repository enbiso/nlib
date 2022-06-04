using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Enbiso.NLib.HttpContextService;

public enum HttpContextSource
{
    Query,
    Route,
    JsonBody
}

public record HttpContextFetchRequest(HttpContextSource Source, string Name)
{
    public static HttpContextFetchRequest FromQuery(string name) => new(HttpContextSource.Query, name);
    public static HttpContextFetchRequest FromRoute(string name) => new(HttpContextSource.Route, name);
    public static HttpContextFetchRequest FromJsonBody(string name) => new(HttpContextSource.JsonBody, name);
}

public interface IHttpContextFetcher
{
    Task<string> FromRoute(string name);
    Task<string> FromJsonBody(string name);
    Task<string> FromQuery(string name);
}

public class HttpContextFetcher : IHttpContextFetcher
{
    private readonly IHttpContextAccessor _contextAccessor;

    public HttpContextFetcher(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Task<string> FromRoute(string name)
    {
        var httpContext = _contextAccessor.HttpContext;
        return Task.FromResult(httpContext?.GetRouteValue(name) as string);
    }

    public async Task<string> FromJsonBody(string name)
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null) return null;
        httpContext.Request.EnableBuffering();
        var jsonDocument = await JsonDocument.ParseAsync(httpContext.Request.Body);
        httpContext.Request.Body.Position = 0;
        return jsonDocument.RootElement.TryGetProperty(name, out var jsonProp) ? jsonProp.GetString() : null;
    }

    public Task<string> FromQuery(string name)
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null) return null;
        var value = httpContext.Request.Query.TryGetValue(name, out var queryValue)
            ? queryValue.ToString()
            : null;
        return Task.FromResult(value);
    }
}