namespace Enbiso.NLib.IdentityServer.Mongo.Events;

public class IdentityServerMongoUserCreateEvent<TUser>: IIdentityServerMongoEvent
{
    public IdentityServerMongoUserCreateEvent(TUser user)
    {
        User = user;
    }

    public TUser User { get; }
}

public class IdentityServerMongoUserDeleteEvent<TUser>: IIdentityServerMongoEvent
{
    public IdentityServerMongoUserDeleteEvent(TUser user)
    {
        User = user;
    }

    public TUser User { get; }
}

public class IdentityServerMongoUserUpdateEvent<TUser>: IIdentityServerMongoEvent
{
    public IdentityServerMongoUserUpdateEvent(TUser user)
    {
        User = user;
    }

    public TUser User { get; }
}