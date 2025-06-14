using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentRating.Web.Contracts.ContentPartyEstimationRoom;

namespace ContentRating.Web.UI.Services
{
    public class ContentPartyEstimationService : IContentPartyEstimationService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ContentPartyEstimationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<IEnumerable<PartyEstimationTitle>?> GetRoomsAsync(bool includeEstimated = true, bool includeNotEstimated = true)
        {
            try
            {
                var query = $"?includeEstimated={includeEstimated}&includeNotEstimated={includeNotEstimated}";
                var response = await _httpClient.GetAsync($"api/content-party-estimation-room{query}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<IEnumerable<PartyEstimationTitle>>(jsonString, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> CreateRoomAsync(CreatePartyEstimationRoomRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/content-party-estimation-room", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<PartyEstimationRoomResponse?> GetRoomAsync(Guid roomId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/content-party-estimation-room/{roomId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PartyEstimationRoomResponse>(jsonString, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> InviteRaterAsync(Guid roomId, InviteRaterRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"api/content-party-estimation-room/{roomId}/rater", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> KickRaterAsync(Guid roomId, Guid raterId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/content-party-estimation-room/{roomId}/rater/{raterId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> CompleteEstimationAsync(Guid roomId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/content-party-estimation-room/{roomId}/complete-estimation", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> RemoveContentAsync(Guid roomId, Guid contentId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/content-party-estimation-room/{roomId}/content/{contentId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> ChangeRatingRangeAsync(Guid roomId, ChangeRatingRangeRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/content-party-estimation-room/{roomId}/rating-range", content);
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