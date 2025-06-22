using ContentRating.Web.UI.Models;

namespace ContentRating.Web.UI.Services.Configuration;

public interface IConfigurationService
{
    Task<ApiSettings?> GetApiSettingsAsync();
}
