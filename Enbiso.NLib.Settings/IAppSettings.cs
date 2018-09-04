namespace Enbiso.NLib.Settings
{
    /// <summary>
    /// App Settings interface
    /// </summary>
    public interface IAppSettings
    {
        string Id { get; }
        string Version { get; }
        string Description { get; }
        string ConnectionString { get; set; }
        string BasePath { get; set; }
        string AuthUrl { get; set; }
    }
}
