using ContentRating.Domain.Shared.Content;
using ContentRating.Web.Contracts.ContentEstimationListEditor;
using ContentRating.Web.UI.Services.Content;
using ContentRating.Web.UI.ViewModels.Content;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ContentRating.Web.UI.ViewModels.Import
{
    public class FileUploadViewModel : IAsyncDisposable
    {
        private readonly IContentEstimationListService _contentService;
        private readonly IContentFileService _contentFileService;
        private readonly ISnackbar _snackbar;

        public FileUploadViewModel(
            IContentEstimationListService contentService,
            IContentFileService contentFileService,
            ISnackbar snackbar
        )
        {
            _contentService = contentService;
            _contentFileService = contentFileService;
            _snackbar = snackbar;
        }

        public Guid RoomId { get; private set; }

        // Загрузка файлов
        public bool ShowFileUploadDialog { get; set; } = false;
        public bool IsUploadingFile { get; set; } = false;
        public IBrowserFile? SelectedFile { get; set; }
        public ContentType SelectedFileContentType { get; set; } = ContentType.Video;
        public string FileUploadName { get; set; } = string.Empty;

        // События
        public event Action? StateChanged;
        public event Action<ContentItemViewModel>? FileUploaded;

        public void Initialize(Guid roomId)
        {
            RoomId = roomId;
        }

        public void OpenFileUploadDialog()
        {
            ShowFileUploadDialog = true;
            ResetFileUploadState();
            StateChanged?.Invoke();
        }

        public void CloseFileUploadDialog()
        {
            ShowFileUploadDialog = false;
            ResetFileUploadState();
            StateChanged?.Invoke();
        }

        public void OnFileSelected(IBrowserFile file)
        {
            SelectedFile = file;
            if (string.IsNullOrEmpty(FileUploadName))
            {
                FileUploadName = Path.GetFileNameWithoutExtension(file.Name);
            }

            // Автоматически определяем тип контента
            SelectedFileContentType = DetectContentTypeFromFile(file);
            StateChanged?.Invoke();
        }

        public async Task<bool> UploadFileAsync()
        {
            if (SelectedFile == null || string.IsNullOrWhiteSpace(FileUploadName))
            {
                _snackbar.Add("Выберите файл и введите название", Severity.Warning);
                return false;
            }

            try
            {
                IsUploadingFile = true;
                StateChanged?.Invoke();

                var uploadResult = await _contentFileService.UploadFileAsync(SelectedFile);
                if (uploadResult == null)
                {
                    _snackbar.Add("Не удалось загрузить файл", Severity.Error);
                    return false;
                }

                // Создаем контент с полученной ссылкой
                var request = new CreateContentRequest
                {
                    Id = Guid.NewGuid(),
                    Name = FileUploadName,
                    Url = uploadResult.FileRoute,
                    ContentType = SelectedFileContentType,
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
                        IsEditing = false,
                    };

                    FileUploaded?.Invoke(newContent);
                    _snackbar.Add("Файл успешно загружен и добавлен", Severity.Success);
                    CloseFileUploadDialog();
                    return true;
                }
                else
                {
                    _snackbar.Add("Файл загружен, но не удалось добавить контент", Severity.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Ошибка при загрузке файла: {ex.Message}", Severity.Error);
                return false;
            }
            finally
            {
                IsUploadingFile = false;
                StateChanged?.Invoke();
            }
        }

        private void ResetFileUploadState()
        {
            SelectedFile = null;
            FileUploadName = string.Empty;
            SelectedFileContentType = ContentType.Video;
            IsUploadingFile = false;
        }

        private ContentType DetectContentTypeFromFile(IBrowserFile file)
        {
            var contentType = file.ContentType.ToLowerInvariant();

            if (contentType.StartsWith("video/"))
                return ContentType.Video;

            if (contentType.StartsWith("audio/"))
                return ContentType.Audio;

            if (contentType.StartsWith("image/"))
                return ContentType.Image;

            // Попробуем определить по расширению
            var extension = Path.GetExtension(file.Name).ToLowerInvariant();

            if (IsVideoExtension(extension))
                return ContentType.Video;

            if (IsAudioExtension(extension))
                return ContentType.Audio;

            if (IsImageExtension(extension))
                return ContentType.Image;

            return ContentType.Video; // По умолчанию
        }

        private bool IsVideoExtension(string extension)
        {
            var videoExtensions = new[]
            {
                ".mp4",
                ".avi",
                ".mkv",
                ".mov",
                ".wmv",
                ".flv",
                ".webm",
                ".m4v",
                ".3gp",
                ".ogv",
            };
            return videoExtensions.Contains(extension);
        }

        private bool IsAudioExtension(string extension)
        {
            var audioExtensions = new[]
            {
                ".mp3",
                ".wav",
                ".flac",
                ".aac",
                ".ogg",
                ".wma",
                ".m4a",
                ".opus",
                ".aiff",
            };
            return audioExtensions.Contains(extension);
        }

        private bool IsImageExtension(string extension)
        {
            var imageExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".bmp",
                ".webp",
                ".svg",
                ".tiff",
                ".ico",
            };
            return imageExtensions.Contains(extension);
        }

        public async ValueTask DisposeAsync()
        {
            // Очистка ресурсов
            await Task.CompletedTask;
        }
    }
}
