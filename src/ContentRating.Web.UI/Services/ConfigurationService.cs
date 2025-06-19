using System.Text.Json;
using ContentRating.Web.UI.Models;

namespace ContentRating.Web.UI.Services;

public interface IConfigurationService
{
    Task<ApiSettings> GetConfigurationAsync();
}

public class ConfigurationService : IConfigurationService
{
    private readonly HttpClient _httpClient;

    public ConfigurationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiSettings> GetConfigurationAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/configuration");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var config = JsonSerializer.Deserialize<ApiSettings>(
                jsonString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return config ?? new ApiSettings();
        }
        catch (Exception ex)
        {
            // Логирование ошибки или fallback конфигурация
            Console.WriteLine($"Ошибка получения конфигурации: {ex.Message}");
            return new ApiSettings
            {
                BaseUrl = "https://localhost:7247",
                SignalRHubUrl = "https://localhost:7247",
            };
        }
    }
}
