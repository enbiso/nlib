using Enbiso.NLib.IdentityServer.Mongo.Models;

namespace Enbiso.NLib.IdentityServer.Mongo.Events;

public class IdentityServerMongoUserCreateEvent: IIdentityServerMongoEvent
{
    public IdentityServerMongoUserCreateEvent(User user)
    {
        User = user;
    }

    public User User { get; }
}

public class IdentityServerMongoUserDeleteEvent: IIdentityServerMongoEvent
{
    public IdentityServerMongoUserDeleteEvent(User user)
    {
        User = user;
    }

    public User User { get; }
}

public class IdentityServerMongoUserUpdateEvent: IIdentityServerMongoEvent
{
    public IdentityServerMongoUserUpdateEvent(User user)
    {
        User = user;
    }

    public User User { get; }
}