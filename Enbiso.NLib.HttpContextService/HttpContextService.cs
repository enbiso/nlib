using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Enbiso.NLib.HttpContextService;

public interface IHttpContextService
{
    Task<string> Fetch(HttpContextFetchRequest request);
    string ClaimValue(string name);
    string UserId();
    bool UserInRole(string role);
}

public class HttpContextService: IHttpContextService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IHttpContextFetcher _fetcher;

    public HttpContextService(IHttpContextAccessor contextAccessor, IHttpContextFetcher fetcher)
    {
        _contextAccessor = contextAccessor;
        _fetcher = fetcher;
    }
    
    public Task<string> Fetch(HttpContextFetchRequest request)
    {
        var (httpContextDataSource, name) = request;
        return httpContextDataSource switch
        {
            HttpContextSource.Query => _fetcher.FromQuery(name),
            HttpContextSource.Route => _fetcher.FromRoute(name),
            HttpContextSource.JsonBody => _fetcher.FromJsonBody(name),
            _ => null
        };
    }

    public string ClaimValue(string name)
    {
        var user = _contextAccessor.HttpContext?.User;
        return user.FindFirstValue(name);
    }

    public string UserId()
    {
        return ClaimValue("sub") ?? ClaimValue(ClaimTypes.NameIdentifier);
    }

    public bool UserInRole(string role)
    {
        return _contextAccessor.HttpContext?.User.IsInRole(role) ?? false;
    }
}