using ContentRating.Domain.Shared.Content;
using ContentRating.Web.Contracts.ContentEstimationListEditor;
using ContentRating.Web.Contracts.YoutubeContent;
using ContentRating.Web.UI.Services;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ContentRating.Web.UI.ViewModels
{
    public class ContentEstimationListEditorViewModel : IAsyncDisposable
    {
        private readonly IContentEstimationListService _contentService;
        private readonly IContentEstimationListEditorHubService _hubService;
        private readonly ISnackbar _snackbar;

        // Специализированные ViewModels
        public YoutubeImportViewModel YoutubeImport { get; }
        public FileUploadViewModel FileUpload { get; }
        public ContentValidationService ContentValidation { get; }

        public ContentEstimationListEditorViewModel(
            IContentEstimationListService contentService,
            IContentEstimationListEditorHubService hubService,
            ISnackbar snackbar,
            YoutubeImportViewModel youtubeImport,
            FileUploadViewModel fileUpload,
            ContentValidationService contentValidation
        )
        {
            _contentService = contentService;
            _hubService = hubService;
            _snackbar = snackbar;

            YoutubeImport = youtubeImport;
            FileUpload = fileUpload;
            ContentValidation = contentValidation;

            // Подписываемся на события
            SubscribeToHubEvents();
            SubscribeToChildViewModelEvents();
        }

        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public Guid CreatorId { get; set; }
        public List<ContentItemViewModel> ContentItems { get; set; } = new();
        public List<InvitedEditorResponse> InvitedEditors { get; set; } = new();

        public bool IsLoading { get; set; } = true;
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        // Состояние для нового контента
        public ContentItemViewModel NewContent { get; set; } = new();
        public bool IsAddingContent { get; set; } = false;

        // YouTube функциональность
        public List<YoutubePlaylist> YoutubePlaylists { get; set; } = new();
        public bool IsLoadingPlaylists { get; set; } = false;
        public bool IsLoadingVideos { get; set; } = false;
        public string? SelectedPlaylistId { get; set; }
        public bool ShowYoutubeDialog { get; set; } = false;
        public string YoutubeErrorMessage { get; set; } = string.Empty;
        public bool HasYoutubeError { get; set; } = false;

        // Загрузка файлов
        public bool ShowFileUploadDialog { get; set; } = false;
        public bool IsUploadingFile { get; set; } = false;
        public IBrowserFile? SelectedFile { get; set; }
        public ContentType SelectedFileContentType { get; set; } = ContentType.Video;
        public string FileUploadName { get; set; } = string.Empty;

        // Событие для уведомления UI об изменениях
        public event Action? StateChanged;

        private ContentItemViewModel? _currentEditingItem;

        private void SubscribeToChildViewModelEvents()
        {
            YoutubeImport.StateChanged += OnStateChanged;
            FileUpload.StateChanged += OnStateChanged;
            YoutubeImport.ContentImported += OnContentImported;
            FileUpload.FileUploaded += OnContentImported;
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke();
        }

        private void OnContentImported(ContentItemViewModel content)
        {
            // Проверяем, что контент еще не добавлен (избегаем дубликатов из SignalR)
            var existingItem = ContentItems.FirstOrDefault(x => x.Id == content.Id);
            if (existingItem == null)
            {
                ContentItems.Add(content);
                ContentValidation.Initialize(ContentItems);
                StateChanged?.Invoke();
            }
        }

        public async Task LoadRoomDataAsync(Guid roomId)
        {
            try
            {
                RoomId = roomId;
                IsLoading = true;
                HasError = false;

                var roomData = await _contentService.GetRoomEditorAsync(roomId);
                if (roomData == null)
                {
                    HasError = true;
                    ErrorMessage = "Топ не найден";
                    return;
                }

                RoomName = roomData.Name;
                CreatorName = roomData.CreatorName;
                CreatorId = roomData.CreatorId;

                ContentItems = roomData
                    .Content.Select(c => new ContentItemViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Url = c.Path,
                        ContentType = c.ContentType,
                        CreatorId = c.CreatorId,
                        LastModificationDate = c.LastModificationDate,
                        IsEditing = false,
                    })
                    .ToList();

                InvitedEditors = roomData.InvitedEditors.ToList();

                // Инициализируем дочерние ViewModels
                YoutubeImport.Initialize(roomId);
                FileUpload.Initialize(roomId);
                ContentValidation.Initialize(ContentItems);

                await _hubService.ConnectAsync(roomId);
            }
            catch (Exception)
            {
                HasError = true;
                ErrorMessage = "Произошла ошибка при загрузке данных";
                _snackbar.Add("Ошибка загрузки данных", Severity.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> AddContentAsync()
        {
            if (!ContentValidation.ValidateNewContent(NewContent))
            {
                return false;
            }

            try
            {
                IsAddingContent = true;

                var request = new CreateContentRequest
                {
                    Id = Guid.NewGuid(),
                    Name = NewContent.Name,
                    Url = NewContent.Url,
                    ContentType = NewContent.ContentType,
                };

                var success = await _contentService.CreateContentAsync(RoomId, request);

                if (success)
                {
                    NewContent = new ContentItemViewModel();
                    _snackbar.Add("Контент добавлен", Severity.Success);
                    return true;
                }
                else
                {
                    _snackbar.Add("Не удалось добавить контент", Severity.Error);
                    return false;
                }
            }
            catch (Exception)
            {
                _snackbar.Add("Произошла ошибка при добавлении контента", Severity.Error);
                return false;
            }
            finally
            {
                IsAddingContent = false;
            }
        }

        public async Task<bool> UpdateContentAsync(ContentItemViewModel item)
        {
            if (!ContentValidation.ValidateContentItem(item))
            {
                return false;
            }

            try
            {
                var request = new UpdateContentRequest
                {
                    Name = item.Name,
                    Url = item.Url,
                    ContentType = item.ContentType,
                };

                var success = await _contentService.UpdateContentAsync(RoomId, item.Id, request);

                if (success)
                {
                    item.LastModificationDate = DateTime.Now;
                    item.IsEditing = false;
                    _currentEditingItem = null;
                    _snackbar.Add("Контент обновлен", Severity.Success);
                    return true;
                }
                else
                {
                    _snackbar.Add("Не удалось обновить контент", Severity.Error);
                    return false;
                }
            }
            catch (Exception)
            {
                _snackbar.Add("Произошла ошибка при обновлении контента", Severity.Error);
                return false;
            }
        }

        public async Task<bool> DeleteContentAsync(ContentItemViewModel item)
        {
            try
            {
                var success = await _contentService.DeleteContentAsync(RoomId, item.Id);

                if (success)
                {
                    _snackbar.Add("Контент удален", Severity.Success);
                    return true;
                }
                else
                {
                    _snackbar.Add("Не удалось удалить контент", Severity.Error);
                    return false;
                }
            }
            catch (Exception)
            {
                _snackbar.Add("Произошла ошибка при удалении контента", Severity.Error);
                return false;
            }
        }

        public void StartEditing(ContentItemViewModel item)
        {
            // Отменяем редактирование предыдущего элемента (если есть)
            if (_currentEditingItem != null && _currentEditingItem.Id != item.Id)
            {
                _currentEditingItem.IsEditing = false;
                _currentEditingItem.RestoreOriginalValues();
            }

            // Сохраняем оригинальные значения для отмены
            item.OriginalName = item.Name;
            item.OriginalUrl = item.Url;
            item.OriginalContentType = item.ContentType;

            item.IsEditing = true;
            _currentEditingItem = item;
        }

        public void CancelEditing(ContentItemViewModel item)
        {
            item.RestoreOriginalValues();
            item.IsEditing = false;
            _currentEditingItem = null;
        }

        public void OnUrlChanged(string newUrl)
        {
            if (string.IsNullOrWhiteSpace(newUrl))
            {
                NewContent.ContentType = ContentType.Video;
                return;
            }

            var detectedType = ContentValidation.DetectContentType(newUrl);
            if (detectedType.HasValue)
            {
                NewContent.ContentType = detectedType.Value;
            }

            if (string.IsNullOrWhiteSpace(NewContent.Name))
            {
                NewContent.Name = ContentValidation.ExtractNameFromUrl(newUrl);
            }
        }

        private void SubscribeToHubEvents()
        {
            _hubService.ContentCreated += OnContentCreated;
            _hubService.ContentUpdated += OnContentUpdated;
            _hubService.ContentDeleted += OnContentDeleted;
            _hubService.EditorInvited += OnEditorInvited;
            _hubService.EditorKicked += OnEditorKicked;
        }

        private void OnContentCreated(Guid editorId, ContentNotificationData content)
        {
            // Проверяем, что контент еще не добавлен (избегаем дубликатов)
            var existingItem = ContentItems.FirstOrDefault(x => x.Id == content.Id);
            if (existingItem != null)
            {
                return; // Контент уже существует, игнорируем
            }

            var newItem = new ContentItemViewModel
                    {
                        Id = content.Id,
                        Name = content.Name,
                        Url = content.Path,
                        ContentType = content.ContentType,
                        CreatorId = editorId,
                        LastModificationDate = content.LastModificationDate,
                        IsEditing = false,
            };

            ContentItems.Add(newItem);
            ContentValidation.Initialize(ContentItems);
            
            // Уведомление о добавлении контента
            _snackbar.Add($"➕ Добавлен новый контент: {content.Name}", Severity.Info);
            
            StateChanged?.Invoke();
        }

        private void OnContentUpdated(Guid editorId, ContentNotificationData content)
        {
            var existingItem = ContentItems.FirstOrDefault(x => x.Id == content.Id);
            if (existingItem != null)
            {
                existingItem.Name = content.Name;
                existingItem.Url = content.Path;
                existingItem.ContentType = content.ContentType;
                existingItem.LastModificationDate = content.LastModificationDate;
                existingItem.IsEditing = false;

                // Уведомление об изменении контента
                _snackbar.Add($"✏️ Контент обновлен: {content.Name}", Severity.Info);

                StateChanged?.Invoke();
            }
        }

        private void OnContentDeleted(Guid contentId)
        {
            var itemToRemove = ContentItems.FirstOrDefault(x => x.Id == contentId);
            if (itemToRemove != null)
            {
                var contentName = itemToRemove.Name;
                ContentItems.Remove(itemToRemove);
                ContentValidation.Initialize(ContentItems);

                // Уведомление об удалении контента
                _snackbar.Add($"🗑️ Контент удален: {contentName}", Severity.Warning);

                StateChanged?.Invoke();
            }
        }

        private void OnEditorInvited(Guid newEditorId, string editorName, Guid inviterId)
        {
            var newEditor = new InvitedEditorResponse(newEditorId, editorName);
            InvitedEditors.Add(newEditor);

            // Уведомление о добавлении нового редактора
            _snackbar.Add($"👥 Новый редактор присоединился: {editorName}", Severity.Success);

            StateChanged?.Invoke();
        }

        private void OnEditorKicked(Guid kickedEditorId, Guid kickInitiatorId)
        {
            var editorToRemove = InvitedEditors.FirstOrDefault(x => x.Id == kickedEditorId);
            if (editorToRemove != null)
            {
                var editorName = editorToRemove.Name;
                InvitedEditors.Remove(editorToRemove);

                // Уведомление об удалении редактора
                _snackbar.Add($"👤 Редактор покинул комнату: {editorName}", Severity.Warning);

                StateChanged?.Invoke();
            }
        }

        public async ValueTask DisposeAsync()
        {
            // Отписываемся от событий
            if (YoutubeImport != null)
            {
                YoutubeImport.StateChanged -= OnStateChanged;
                YoutubeImport.ContentImported -= OnContentImported;
                await YoutubeImport.DisposeAsync();
            }

            if (FileUpload != null)
            {
                FileUpload.StateChanged -= OnStateChanged;
                FileUpload.FileUploaded -= OnContentImported;
                await FileUpload.DisposeAsync();
            }

            _hubService.ContentCreated -= OnContentCreated;
            _hubService.ContentUpdated -= OnContentUpdated;
            _hubService.ContentDeleted -= OnContentDeleted;
            _hubService.EditorInvited -= OnEditorInvited;
            _hubService.EditorKicked -= OnEditorKicked;

            await _hubService.DisconnectAsync();
        }
    }
}
