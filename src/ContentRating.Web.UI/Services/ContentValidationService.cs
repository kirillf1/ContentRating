using System.Text.Json;
using ContentRating.Domain.Shared.Content;
using ContentRating.Web.Contracts.YoutubeContent;
using ContentRating.Web.Contracts.ContentEstimationListEditor;
using ContentRating.Web.UI.ViewModels;
using MudBlazor;
using Microsoft.Extensions.Logging;

namespace ContentRating.Web.UI.Services
{
    public class ContentValidationService
    {
        private readonly ISnackbar _snackbar;
        private readonly HttpClient _httpClient;
        private List<ContentItemViewModel> _existingContent = new();
        private readonly ILogger<ContentValidationService> _logger;

        public ContentValidationService(ISnackbar snackbar, HttpClient httpClient, ILogger<ContentValidationService> logger)
        {
            _snackbar = snackbar;
            _httpClient = httpClient;
            _logger = logger;
        }

        public void Initialize(List<ContentItemViewModel> existingContent)
        {
            _existingContent = existingContent;
        }

        public bool ValidateNewContent(ContentItemViewModel content)
        {
            if (!ValidateContentItem(content))
            {
                return false;
            }

            // Проверка на дублирование URL
            if (IsUrlAlreadyExists(content.Url))
            {
                _snackbar.Add("Контент с таким URL уже добавлен в топ", Severity.Warning);
                return false;
            }

            return true;
        }

        public bool ValidateContentItem(ContentItemViewModel item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                _snackbar.Add("Название контента не может быть пустым", Severity.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.Url))
            {
                _snackbar.Add("URL контента не может быть пустым", Severity.Warning);
                return false;
            }

            if (!Uri.TryCreate(item.Url, UriKind.Absolute, out _))
            {
                _snackbar.Add("Введите корректный URL", Severity.Warning);
                return false;
            }

            return true;
        }

        public bool IsUrlAlreadyExists(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            // Нормализуем URL для сравнения
            var normalizedUrl = NormalizeUrl(url);

            return _existingContent.Any(item =>
                string.Equals(
                    NormalizeUrl(item.Url),
                    normalizedUrl,
                    StringComparison.OrdinalIgnoreCase
                )
            );
        }

        public string NormalizeUrl(string url)
        {
            try
            {
                var uri = new Uri(url);

                // Убираем trailing slash и приводим к нижнему регистру
                var normalized = uri.ToString().TrimEnd('/').ToLowerInvariant();

                // Для YouTube URL нормализуем формат
                if (IsYouTubeUrl(url))
                {
                    var videoId = ExtractYouTubeVideoId(url);
                    if (!string.IsNullOrEmpty(videoId))
                    {
                        return $"https://www.youtube.com/watch?v={videoId}";
                    }
                }

                return normalized;
            }
            catch
            {
                // Если не удалось распарсить URL, возвращаем как есть в нижнем регистре
                return url.ToLowerInvariant();
            }
        }

        public string ExtractNameFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            try
            {
                var uri = new Uri(url);

                // YouTube специальная обработка
                if (IsYouTubeUrl(url))
                {
                    return ExtractYouTubeName(url);
                }

                // Для других URL пытаемся извлечь имя файла
                var fileName = Path.GetFileNameWithoutExtension(uri.LocalPath);
                if (!string.IsNullOrEmpty(fileName))
                {
                    // Убираем символы подчеркивания и дефисы, заменяем на пробелы
                    fileName = fileName.Replace("_", " ").Replace("-", " ");

                    // Убираем лишние пробелы
                    fileName = System
                        .Text.RegularExpressions.Regex.Replace(fileName, @"\s+", " ")
                        .Trim();

                    // Делаем первую букву заглавной
                    if (fileName.Length > 0)
                    {
                        fileName = char.ToUpper(fileName[0]) + fileName.Substring(1);
                    }

                    return fileName;
                }

                // Если не удалось извлечь имя файла, используем домен
                return $"Контент с {uri.Host}";
            }
            catch
            {
                // Если URL некорректный, возвращаем пустую строку
                return string.Empty;
            }
        }

        public ContentType? DetectContentType(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            try
            {
                var uri = new Uri(url);
                var host = uri.Host.ToLowerInvariant();
                var path = uri.LocalPath.ToLowerInvariant();
                var extension = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();

                // Видео платформы
                if (IsVideoUrl(url, host, path))
                {
                    return ContentType.Video;
                }

                // Аудио форматы
                if (IsAudioUrl(extension))
                {
                    return ContentType.Audio;
                }

                // Изображения
                if (IsImageUrl(extension))
                {
                    return ContentType.Image;
                }

                // Если не удалось определить, возвращаем null
                return null;
            }
            catch
            {
                return null;
            }
        }

        public bool IsYouTubeUrl(string url)
        {
            return url.Contains("youtube.com") || url.Contains("youtu.be");
        }

        private string ExtractYouTubeName(string url)
        {
            try
            {
                var uri = new Uri(url);
                var videoId = ExtractYouTubeVideoId(url);

                if (!string.IsNullOrEmpty(videoId))
                {
                    return $"YouTube видео ({videoId})";
                }

                return "YouTube видео";
            }
            catch
            {
                return "YouTube видео";
            }
        }

        public async Task<string> ExtractYouTubeNameAsync(string url)
        {
            try
            {
                var request = new { Url = url };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(
                    json,
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    "api/youtube-content/video-title",
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var title = JsonSerializer.Deserialize<YoutubeVideoTitle>(
                        responseJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (title is not null)
                    {
                        return title.Name;
                    }
                }

                // Fallback к базовому названию
                var videoId = ExtractYouTubeVideoId(url);
                if (!string.IsNullOrEmpty(videoId))
                {
                    return $"YouTube видео ({videoId})";
                }

                return "YouTube видео";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении названия YouTube видео по URL {Url}", url);
                // Fallback к базовому названию
                var videoId = ExtractYouTubeVideoId(url);
                if (!string.IsNullOrEmpty(videoId))
                {
                    return $"YouTube видео ({videoId})";
                }

                return "YouTube видео";
            }
        }

        private string ExtractYouTubeVideoId(string url)
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

        private bool IsVideoUrl(string url, string host, string path)
        {
            // YouTube
            if (host.Contains("youtube.com") || host.Contains("youtu.be"))
                return true;

            // Другие видео платформы
            if (
                host.Contains("vimeo.com")
                || host.Contains("dailymotion.com")
                || host.Contains("twitch.tv")
                || host.Contains("rutube.ru")
                || host.Contains("ok.ru") && path.Contains("video")
                || host.Contains("vk.com") && path.Contains("video")
            )
                return true;

            // Видео файлы по расширению
            var videoExtensions = new[]
            {
                "mp4",
                "avi",
                "mkv",
                "mov",
                "wmv",
                "flv",
                "webm",
                "m4v",
                "3gp",
                "ogv",
            };
            var extension = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();

            return videoExtensions.Contains(extension);
        }

        private bool IsAudioUrl(string extension)
        {
            var audioExtensions = new[]
            {
                "mp3",
                "wav",
                "ogg",
                "m4a",
                "aac",
                "flac",
                "wma",
                "opus",
                "aiff",
            };
            return audioExtensions.Contains(extension);
        }

        private bool IsImageUrl(string extension)
        {
            var imageExtensions = new[]
            {
                "jpg",
                "jpeg",
                "png",
                "gif",
                "bmp",
                "webp",
                "svg",
                "ico",
                "tiff",
                "tif",
            };
            return imageExtensions.Contains(extension);
        }
    }
}
