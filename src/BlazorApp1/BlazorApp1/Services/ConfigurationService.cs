using BlazorApp1.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace BlazorApp1.Services
{
    public interface IConfigurationService
    {
        Task<AppConfiguration> GetConfigurationAsync();
        Task<string> GetApiBaseUrlAsync();
        Task<string> GetSignalRHubUrlAsync();
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly HttpClient _httpClient;
        private AppConfiguration? _cachedConfiguration;

        public ConfigurationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AppConfiguration> GetConfigurationAsync()
        {
            if (_cachedConfiguration != null)
                return _cachedConfiguration;

            try
            {
                // Sprawdź środowisko z zmiennej ASPNETCORE_ENVIRONMENT
                var environment = await GetEnvironmentAsync();
                var configFileName = $"appsettings.{environment}.json";
                
                Console.WriteLine($"Loading configuration for environment: {environment}");
                Console.WriteLine($"Trying to load: {configFileName}");
                
                try
                {
                    _cachedConfiguration = await _httpClient.GetFromJsonAsync<AppConfiguration>(configFileName);
                    Console.WriteLine($"Successfully loaded {configFileName}");
                }
                catch (Exception envEx)
                {
                    Console.WriteLine($"Failed to load {configFileName}: {envEx.Message}");
                    // Jeśli nie znajdzie pliku dla środowiska, użyj domyślnego
                    _cachedConfiguration = await _httpClient.GetFromJsonAsync<AppConfiguration>("appsettings.json");
                    Console.WriteLine("Loaded default appsettings.json");
                }

                return _cachedConfiguration ?? new AppConfiguration();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                return new AppConfiguration();
            }
        }

        public async Task<string> GetApiBaseUrlAsync()
        {
            var config = await GetConfigurationAsync();
            return config.ApiSettings.BaseUrl;
        }

        public async Task<string> GetSignalRHubUrlAsync()
        {
            var config = await GetConfigurationAsync();
            return config.ApiSettings.SignalRHubUrl;
        }

        private Task<string> GetEnvironmentAsync()
        {
            try
            {
                // Sprawdź zmienną środowiskową ASPNETCORE_ENVIRONMENT
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                
                // Jeśli nie ma zmiennej środowiskowej, sprawdź inne sposoby
                if (string.IsNullOrEmpty(environment))
                {
                    // Na Azure Static Web Apps możesz użyć innych zmiennych
                    environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");
                }
                
                // Domyślnie Production dla Azure, Development dla localhost
                return Task.FromResult(environment ?? "Development");
            }
            catch
            {
                return Task.FromResult("Development");
            }
        }
    }
}
