namespace BlazorApp1.Configuration
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string SignalRHubUrl { get; set; } = string.Empty;
    }

    public class AppConfiguration
    {
        public ApiSettings ApiSettings { get; set; } = new();
        public string Environment { get; set; } = "Development";
    }
}
