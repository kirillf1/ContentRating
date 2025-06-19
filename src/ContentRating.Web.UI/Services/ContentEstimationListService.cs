using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentRating.Web.Contracts.ContentEstimationListEditor;

namespace ContentRating.Web.UI.Services
{
    public interface IContentEstimationListService
    {
        Task<IEnumerable<ContentEstimationListEditorTitle>?> GetRoomsAsync();
        Task<bool> CreateRoomAsync(string roomName);
        Task<bool> DeleteRoomAsync(Guid roomId);
        Task<ContentEstimationListEditorResponse?> GetRoomEditorAsync(Guid roomId);
        Task<bool> CreateContentAsync(Guid roomId, CreateContentRequest request);
        Task<bool> UpdateContentAsync(Guid roomId, Guid contentId, UpdateContentRequest request);
        Task<bool> DeleteContentAsync(Guid roomId, Guid contentId);
        Task<bool> InviteEditorAsync(Guid roomId, InviteEditorRequest request);
        Task<bool> KickEditorAsync(Guid roomId, Guid editorId);
    }

    public class ContentEstimationListService : IContentEstimationListService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ContentEstimationListService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<IEnumerable<ContentEstimationListEditorTitle>?> GetRoomsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/content-estimation-list-editor");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<
                        IEnumerable<ContentEstimationListEditorTitle>
                    >(jsonString, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> CreateRoomAsync(string roomName)
        {
            try
            {
                var request = new CreateContentEstimationListEditorRequest
                {
                    Id = Guid.NewGuid(),
                    RoomName = roomName,
                };

                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    "api/content-estimation-list-editor",
                    content
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> DeleteRoomAsync(Guid roomId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"api/content-estimation-list-editor/{roomId}"
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<ContentEstimationListEditorResponse?> GetRoomEditorAsync(Guid roomId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"api/content-estimation-list-editor/{roomId}"
                );

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ContentEstimationListEditorResponse>(
                        jsonString,
                        _jsonOptions
                    );
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> CreateContentAsync(Guid roomId, CreateContentRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"api/content-estimation-list-editor/{roomId}/content",
                    content
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> UpdateContentAsync(
            Guid roomId,
            Guid contentId,
            UpdateContentRequest request
        )
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(
                    $"api/content-estimation-list-editor/{roomId}/content/{contentId}",
                    content
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> DeleteContentAsync(Guid roomId, Guid contentId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"api/content-estimation-list-editor/{roomId}/content/{contentId}"
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> InviteEditorAsync(Guid roomId, InviteEditorRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"api/content-estimation-list-editor/{roomId}/editor",
                    content
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> KickEditorAsync(Guid roomId, Guid editorId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"api/content-estimation-list-editor/{roomId}/editor/{editorId}"
                );
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
