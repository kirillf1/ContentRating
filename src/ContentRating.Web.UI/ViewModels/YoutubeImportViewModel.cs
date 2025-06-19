using ContentRating.Domain.Shared.Content;
using ContentRating.Web.Contracts.ContentEstimationListEditor;
using ContentRating.Web.Contracts.YoutubeContent;
using ContentRating.Web.UI.Services;
using MudBlazor;

namespace ContentRating.Web.UI.ViewModels
{
    public class YoutubeImportViewModel : IAsyncDisposable
    {
        private readonly IYoutubeService _youtubeService;
        private readonly IContentEstimationListService _contentService;
        private readonly ISnackbar _snackbar;

        public YoutubeImportViewModel(
            IYoutubeService youtubeService,
            IContentEstimationListService contentService,
            ISnackbar snackbar
        )
        {
            _youtubeService = youtubeService;
            _contentService = contentService;
            _snackbar = snackbar;
        }

        public Guid RoomId { get; private set; }
        
        // YouTube функциональность
        public List<YoutubePlaylist> YoutubePlaylists { get; set; } = new();
        public bool IsLoadingPlaylists { get; set; } = false;
        public bool IsLoadingVideos { get; set; } = false;
        public string? SelectedPlaylistId { get; set; }
        public bool ShowYoutubeDialog { get; set; } = false;
        public string YoutubeErrorMessage { get; set; } = string.Empty;
        public bool HasYoutubeError { get; set; } = false;

        // События
        public event Action? StateChanged;
        public event Action<ContentItemViewModel>? ContentImported;

        public void Initialize(Guid roomId)
        {
            RoomId = roomId;
        }

        public async Task OpenYoutubeDialogAsync()
        {
            ShowYoutubeDialog = true;
            HasYoutubeError = false;
            YoutubeErrorMessage = string.Empty;
            await LoadYoutubePlaylistsAsync();
            StateChanged?.Invoke();
        }

        public void CloseYoutubeDialog()
        {
            ShowYoutubeDialog = false;
            SelectedPlaylistId = null;
            HasYoutubeError = false;
            YoutubeErrorMessage = string.Empty;
            StateChanged?.Invoke();
        }

        private async Task LoadYoutubePlaylistsAsync()
        {
            try
            {
                IsLoadingPlaylists = true;
                StateChanged?.Invoke();

                var result = await _youtubeService.GetPlaylistsAsync();

                if (result.IsForbidden)
                {
                    HasYoutubeError = true;
                    YoutubeErrorMessage = result.ErrorMessage;
                    _snackbar.Add(result.ErrorMessage, Severity.Warning);
                }
                else if (result.IsSuccess && result.Data != null)
                {
                    YoutubePlaylists = result.Data.ToList();
                    if (!YoutubePlaylists.Any())
                    {
                        HasYoutubeError = true;
                        YoutubeErrorMessage = "У вас нет доступных плейлистов YouTube";
                    }
                }
                else
                {
                    HasYoutubeError = true;
                    YoutubeErrorMessage = result.ErrorMessage;
                    _snackbar.Add(result.ErrorMessage, Severity.Error);
                }
            }
            finally
            {
                IsLoadingPlaylists = false;
                StateChanged?.Invoke();
            }
        }

        public async Task ImportFromPlaylistAsync(List<ContentItemViewModel> existingContent)
        {
            if (string.IsNullOrEmpty(SelectedPlaylistId))
            {
                _snackbar.Add("Выберите плейлист для импорта", Severity.Warning);
                return;
            }

            try
            {
                IsLoadingVideos = true;
                StateChanged?.Invoke();

                var result = await _youtubeService.GetPlaylistVideosAsync(SelectedPlaylistId);

                if (result.IsForbidden)
                {
                    _snackbar.Add(result.ErrorMessage, Severity.Warning);
                    return;
                }

                if (!result.IsSuccess || result.Data == null)
                {
                    _snackbar.Add(result.ErrorMessage, Severity.Error);
                    return;
                }

                var videos = result.Data.ToList();
                if (!videos.Any())
                {
                    _snackbar.Add("В выбранном плейлисте нет видео", Severity.Info);
                    return;
                }

                int addedCount = 0;
                int duplicateCount = 0;

                foreach (var video in videos)
                {
                    // Проверяем, что видео еще не добавлено
                    if (IsUrlAlreadyExists(video.Url, existingContent))
                    {
                        duplicateCount++;
                        continue;
                    }

                    var request = new CreateContentRequest
                    {
                        Id = Guid.NewGuid(),
                        Name = video.Name,
                        Url = video.Url,
                        ContentType = ContentType.Video,
                    };

                    var success = await _contentService.CreateContentAsync(RoomId, request);
                    if (success)
                    {
                        // Создаем объект для уведомления
                        var newContent = new ContentItemViewModel
                        {
                            Id = request.Id,
                            Name = request.Name,
                            Url = request.Url,
                            ContentType = request.ContentType,
                            LastModificationDate = DateTime.Now,
                            IsEditing = false
                        };

                        ContentImported?.Invoke(newContent);
                        addedCount++;
                    }

                    // Небольшая задержка между запросами
                    await Task.Delay(100);
                }

                if (addedCount > 0)
                {
                    _snackbar.Add($"Добавлено {addedCount} видео из плейлиста", Severity.Success);
                }

                if (duplicateCount > 0)
                {
                    _snackbar.Add($"Пропущено {duplicateCount} дубликатов", Severity.Info);
                }

                CloseYoutubeDialog();
            }
            finally
            {
                IsLoadingVideos = false;
                StateChanged?.Invoke();
            }
        }

        private bool IsUrlAlreadyExists(string url, List<ContentItemViewModel> existingContent)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            var normalizedUrl = NormalizeUrl(url);

            return existingContent.Any(item =>
                string.Equals(
                    NormalizeUrl(item.Url),
                    normalizedUrl,
                    StringComparison.OrdinalIgnoreCase
                )
            );
        }

        private string NormalizeUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
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
                return url.ToLowerInvariant();
            }
        }

        private bool IsYouTubeUrl(string url)
        {
            return url.Contains("youtube.com") || url.Contains("youtu.be");
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

        public async ValueTask DisposeAsync()
        {
            // Очистка ресурсов
            await Task.CompletedTask;
        }
    }
} 