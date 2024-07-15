using ContentRatingAPI.Application.YoutubeContent;
using ContentRatingAPI.Infrastructure.YoutubeClient.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ContentRatingAPI.Infrastructure.YoutubeClient
{
    public class HttpYoutubeClient : IYoutubeClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<HttpYoutubeClient> logger;

        public HttpYoutubeClient(IHttpClientFactory httpClientFactory, ILogger<HttpYoutubeClient> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }
        public async Task<Result<IEnumerable<YoutubePlaylist>>> GetAvailablePlayLists(string accessToken)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("https://www.googleapis.com/youtube/v3/playlists?mine=true&part=snippet");

            if (response.IsSuccessStatusCode)
            {
                var stringResult = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<PlaylistModel>(stringResult, options);
                if (result is null)
                {
                    logger.LogWarning("{Playlist} can't be deserialized", nameof(List<YoutubePlaylist>));
                    return Result.Error("Unknown response");
                }

                return Result.Success(result.Items!.Select(c => new YoutubePlaylist(c.Snippet!.Title!, c.Id!)));
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                return Result.Forbidden();
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return Result.NotFound();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Result.Unauthorized();
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                return Result.Error(await response.Content.ReadAsStringAsync());

            logger.LogWarning("Unknown response status code for request while getting youtube playlist, status code: {status code}, message {message}", response.StatusCode,
                await response.Content.ReadAsStringAsync());
            return Result.Error("Unknown error try later");
        }

        public async Task<Result<IEnumerable<YoutubeVideo>>> GetVideosFromPlayList(string playListId, string accessToken)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync($"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&playlistId={playListId}");

            if (response.IsSuccessStatusCode)
            {
                var videos = new List<YoutubeVideo>();
                var stringResult = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var playlist = JsonSerializer.Deserialize<PlaylistModel>(stringResult, options);
                if (playlist is null)
                {
                    logger.LogWarning("{Playlist} can't be deserialized", nameof(List<YoutubePlaylist>));
                    return Result.Error("Unknown response");
                }
                videos.AddRange(playlist!.Items!.Select(c => MapYoutubeVideo(c.Snippet?.ResourceId?.VideoId, c?.Snippet?.Title)));
                while (playlist != null && !string.IsNullOrEmpty(playlist?.NextPageToken))
                {
                    playlist = await client.GetFromJsonAsync<PlaylistModel>($"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&playlistId={playListId}&pageToken={playlist.NextPageToken}");
                    if (playlist != null)
                        videos.AddRange(playlist!.Items!.Select(c => MapYoutubeVideo(c.Snippet?.ResourceId?.VideoId, c?.Snippet?.Title)));
                }
                return Result.Success(videos.AsEnumerable());
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                return Result.Forbidden();
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return Result.NotFound();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Result.Unauthorized();
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                return Result.Error(await response.Content.ReadAsStringAsync());

            logger.LogWarning("Unknown response status code for request while getting youtube videos, status code: {status code}, message {message}", response.StatusCode,
                await response.Content.ReadAsStringAsync());
            return Result.Error("Unknown error try later");
        }
        private static YoutubeVideo MapYoutubeVideo(string? id, string? name)
        {
            name ??= "deleted video";
            if (string.IsNullOrWhiteSpace(id))
            {
                return new YoutubeVideo(string.Empty, string.Empty, name);
            }
            var videoUrl = $"https://www.youtube.com/watch?v={id}";
            var embedUrl = $"https://www.youtube.com/embed/{id}";
            return new YoutubeVideo(videoUrl, embedUrl, name);
        }
    }
}
