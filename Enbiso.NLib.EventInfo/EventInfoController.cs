using Microsoft.AspNetCore.Mvc;

namespace Enbiso.NLib.EventInfo;

public class EventInfoController: ControllerBase
{
    private readonly IEventInfoService _eventInfoService;

    public EventInfoController(IEventInfoService eventInfoService)
    {
        _eventInfoService = eventInfoService;
    }

    [HttpGet]
    public EventInfoListResponse ListEvents() => _eventInfoService.List();
}

