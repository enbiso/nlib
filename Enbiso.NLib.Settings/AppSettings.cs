namespace Enbiso.NLib.Settings
{
    /// <inheritdoc />
    /// <summary>
    /// Generic App settings
    /// </summary>
    public class AppSettings : IAppSettings
    {
        public string Id { get; }
        public string Version { get; }
        public string Description { get; }
        public string ConnectionString { get; set; }
        public string BasePath { get; set; }
        public string AuthUrl { get; set; }
    }
}