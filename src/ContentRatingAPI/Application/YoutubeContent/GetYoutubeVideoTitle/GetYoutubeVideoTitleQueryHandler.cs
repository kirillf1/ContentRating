// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using ContentRating.Web.Contracts.YoutubeContent;

namespace ContentRatingAPI.Application.YoutubeContent.GetYoutubeVideoTitle
{
    public partial class GetYoutubeVideoTitleQueryHandler
        : IRequestHandler<GetYoutubeVideoTitleQuery, Result<YoutubeVideoTitle>>
    {
        private readonly HttpClient _httpClient;

        public GetYoutubeVideoTitleQueryHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<YoutubeVideoTitle>> Handle(
            GetYoutubeVideoTitleQuery request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var videoId = ExtractYouTubeVideoId(request.Url);
                if (string.IsNullOrEmpty(videoId))
                {
                    return Result.Error("Неверный URL YouTube видео");
                }

                var title = await GetYouTubeVideoTitleAsync(videoId, cancellationToken);
                if (string.IsNullOrEmpty(title))
                {
                    return Result.Error("Не удалось получить название видео");
                }

                return new YoutubeVideoTitle() { Id = videoId, Name = title };
            }
            catch (Exception ex)
            {
                return Result.Error($"Произошла ошибка при получении названия видео: {ex.Message}");
            }
        }

        private async Task<string> GetYouTubeVideoTitleAsync(
            string videoId,
            CancellationToken cancellationToken
        )
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
                );

                var url = $"https://www.youtube.com/watch?v={videoId}";
                var response = await _httpClient.GetStringAsync(url, cancellationToken);

                // Ищем тег <title> в HTML
                var titleMatch = YoutubeRegexTitle().Match(response);

                if (titleMatch.Success)
                {
                    var title = titleMatch.Groups[1].Value;
                    title = title.Replace(" - YouTube", "").Trim();
                    title = System.Net.WebUtility.HtmlDecode(title);
                    return title;
                }

                var ogTitleMatch = Regex.Match(
                    response,
                    @"<meta\s+property=[""']og:title[""']\s+content=[""']([^""']+)[""']",
                    RegexOptions.IgnoreCase
                );

                if (ogTitleMatch.Success)
                {
                    var title = ogTitleMatch.Groups[1].Value;
                    return System.Net.WebUtility.HtmlDecode(title);
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ExtractYouTubeVideoId(string url)
        {
            try
            {
                var uri = new Uri(url);

                if (uri.Host.Contains("youtu.be"))
                {
                    return uri.LocalPath.TrimStart('/');
                }

                if (uri.Host.Contains("youtube.com"))
                {
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    return query["v"] ?? string.Empty;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        [GeneratedRegex(@"<title[^>]*>([^<]+)</title>", RegexOptions.IgnoreCase, "ru-RU")]
        private static partial Regex YoutubeRegexTitle();
    }
}
