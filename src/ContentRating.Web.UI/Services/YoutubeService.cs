using System.Text.Json;
using System.Text.Json.Serialization;
using ContentRating.Web.Contracts.YoutubeContent;

namespace ContentRating.Web.UI.Services
{
    public interface IYoutubeService
    {
        Task<YoutubeServiceResult<IEnumerable<YoutubePlaylist>>> GetPlaylistsAsync();
        Task<YoutubeServiceResult<IEnumerable<YoutubeVideo>>> GetPlaylistVideosAsync(
            string playlistId
        );
    }

    public class YoutubeService : IYoutubeService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public YoutubeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<YoutubeServiceResult<IEnumerable<YoutubePlaylist>>> GetPlaylistsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/youtube-content");

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return YoutubeServiceResult<IEnumerable<YoutubePlaylist>>.Forbidden();
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var playlists = JsonSerializer.Deserialize<IEnumerable<YoutubePlaylist>>(
                        jsonString,
                        _jsonOptions
                    );
                    return YoutubeServiceResult<IEnumerable<YoutubePlaylist>>.Success(
                        playlists ?? Enumerable.Empty<YoutubePlaylist>()
                    );
                }

                return YoutubeServiceResult<IEnumerable<YoutubePlaylist>>.Error(
                    "Ошибка получения плейлистов"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return YoutubeServiceResult<IEnumerable<YoutubePlaylist>>.Error(
                    "Произошла ошибка при загрузке плейлистов"
                );
            }
        }

        public async Task<YoutubeServiceResult<IEnumerable<YoutubeVideo>>> GetPlaylistVideosAsync(
            string playlistId
        )
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/youtube-content/{playlistId}");

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return YoutubeServiceResult<IEnumerable<YoutubeVideo>>.Forbidden();
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var videos = JsonSerializer.Deserialize<List<YoutubeVideo>>(
                        jsonString,
                        _jsonOptions
                    );
                    return YoutubeServiceResult<IEnumerable<YoutubeVideo>>.Success(
                        videos ?? Enumerable.Empty<YoutubeVideo>()
                    );
                }

                return YoutubeServiceResult<IEnumerable<YoutubeVideo>>.Error(
                    "Ошибка получения видео из плейлиста"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return YoutubeServiceResult<IEnumerable<YoutubeVideo>>.Error(
                    "Произошла ошибка при загрузке видео"
                );
            }
        }
    }

    public class YoutubeServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public bool IsForbidden { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public T? Data { get; private set; }

        private YoutubeServiceResult() { }

        public static YoutubeServiceResult<T> Success(T data)
        {
            return new YoutubeServiceResult<T> { IsSuccess = true, Data = data };
        }

        public static YoutubeServiceResult<T> Forbidden()
        {
            return new YoutubeServiceResult<T>
            {
                IsForbidden = true,
                ErrorMessage =
                    "У вас нет возможности получить видео, так как вы не авторизировались через Google",
            };
        }

        public static YoutubeServiceResult<T> Error(string errorMessage)
        {
            return new YoutubeServiceResult<T> { ErrorMessage = errorMessage };
        }
    }
}
