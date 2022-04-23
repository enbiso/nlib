namespace Enbiso.NLib.IdentityServer.Mongo.Models;

public class RecoveryCode
{
    public string Code { get; set; }
    public bool Used { get; set; }
}