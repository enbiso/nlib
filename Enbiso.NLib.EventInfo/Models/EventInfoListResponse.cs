using System.Collections.Generic;

namespace Enbiso.NLib.EventInfo.Models;

public class EventInfoListResponse
{
    public List<EventRecord> Records { get; set; } = new();
    public int Count { get; set; }
}