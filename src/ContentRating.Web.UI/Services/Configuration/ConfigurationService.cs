using System.Text.Json;
using ContentRating.Web.UI.Models;
using Microsoft.Extensions.Logging;

namespace ContentRating.Web.UI.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(HttpClient httpClient, ILogger<ConfigurationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiSettings?> GetApiSettingsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/configuration");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var config = JsonSerializer.Deserialize<ApiSettings>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return config;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения конфигурации API");
            return null;
        }
    }
}
