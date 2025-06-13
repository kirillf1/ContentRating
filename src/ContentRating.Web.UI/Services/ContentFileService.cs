using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentRating.Web.Contracts.ContentFileManager;
using Microsoft.AspNetCore.Components.Forms;

namespace ContentRating.Web.UI.Services
{
    public interface IContentFileService
    {
        Task<SavedFileResponse?> UploadFileAsync(IBrowserFile file);
        Task<bool> DeleteFileAsync(Guid fileId);
    }

    public class ContentFileService : IContentFileService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ContentFileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<SavedFileResponse?> UploadFileAsync(IBrowserFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var stream = file.OpenReadStream(maxAllowedSize: 200_000_000);
                using var streamContent = new StreamContent(stream);

                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", file.Name);

                var response = await _httpClient.PostAsync("api/content-files", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SavedFileResponse>(jsonString, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки файла: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteFileAsync(Guid fileId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/content-files/{fileId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления файла: {ex.Message}");
                return false;
            }
        }
    }
} 