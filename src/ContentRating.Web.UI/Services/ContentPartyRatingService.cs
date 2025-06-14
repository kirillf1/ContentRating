using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentRating.Web.Contracts.ContentPartyRating;

namespace ContentRating.Web.UI.Services
{
    public class ContentPartyRatingService : IContentPartyRatingService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ContentPartyRatingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<ContentPartyRatingResponse?> GetContentRatingAsync(Guid contentRatingId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/content-party-rating/{contentRatingId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ContentPartyRatingResponse>(jsonString, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> EstimateContentAsync(Guid contentRatingId, EstimateContentRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/content-party-rating/{contentRatingId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
} 