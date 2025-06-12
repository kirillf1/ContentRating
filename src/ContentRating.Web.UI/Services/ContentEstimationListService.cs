using System.Text.Json;
using System.Text;
using ContentRating.Web.Contracts.ContentEstimationListEditor;

namespace ContentRating.Web.UI.Services
{
    public interface IContentEstimationListService
    {
        Task<IEnumerable<ContentEstimationListEditorTitle>?> GetRoomsAsync();
        Task<bool> CreateRoomAsync(string roomName);
    }

    public class ContentEstimationListService : IContentEstimationListService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ContentEstimationListService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
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
                    RoomName = roomName
                };

                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/content-estimation-list-editor", content);
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
