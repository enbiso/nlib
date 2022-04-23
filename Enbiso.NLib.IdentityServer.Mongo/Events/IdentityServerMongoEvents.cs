namespace Enbiso.NLib.IdentityServer.Mongo.Events;

public interface IIdentityServerMongoEvent
{
}

public interface IIdentityServerMongoEventHandler<in TEvent> where TEvent: IIdentityServerMongoEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken);
}