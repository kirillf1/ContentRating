using ContentRating.Domain.Shared.Content;
using ContentRating.Web.UI.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace ContentRating.Web.UI.Services
{
    public class ContentEstimationListEditorHubService
        : IContentEstimationListEditorHubService,
            IAsyncDisposable
    {
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<ContentEstimationListEditorHubService> _logger;
        private readonly AuthService _authService;
        private HubConnection? _hubConnection;
        private Guid? _currentRoomId;

        public ContentEstimationListEditorHubService(
            ApiSettings apiSettings,
            ILogger<ContentEstimationListEditorHubService> logger,
            AuthService authService
        )
        {
            _apiSettings = apiSettings;
            _logger = logger;
            _authService = authService;

            // Подписываемся на изменения состояния аутентификации
            _authService.AuthStateChanged += OnAuthStateChanged;
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public event Action<Guid, ContentNotificationData>? ContentCreated;
        public event Action<Guid, ContentNotificationData>? ContentUpdated;
        public event Action<Guid>? ContentDeleted;
        public event Action<Guid, string, Guid>? EditorInvited;
        public event Action<Guid, Guid>? EditorKicked;

        public async Task ConnectAsync(Guid roomId)
        {
            try
            {
                // Если уже подключены к этой комнате, ничего не делаем
                if (_currentRoomId == roomId && IsConnected)
                    return;

                // Проверяем аутентификацию перед подключением
                if (!_authService.IsAuthenticated)
                {
                    _logger.LogWarning("User is not authenticated, cannot connect to SignalR hub");
                    throw new UnauthorizedAccessException("User is not authenticated");
                }

                // Проверяем и обновляем токен если нужно
                var token = await _authService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No access token available, attempting to refresh");
                    var refreshSuccess = await _authService.RefreshTokenAsync();
                    if (!refreshSuccess)
                    {
                        _logger.LogError("Failed to refresh token, cannot connect to SignalR hub");
                        throw new UnauthorizedAccessException(
                            "Failed to refresh authentication token"
                        );
                    }
                }

                // Отключаемся от предыдущего подключения
                await DisconnectAsync();

                _currentRoomId = roomId;

                // Создаем новое подключение
                var hubUrl = $"{_apiSettings.BaseUrl.TrimEnd('/')}/contentListEditor";

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(
                        hubUrl,
                        options =>
                        {
                            // Добавляем авторизацию через токены
                            options.AccessTokenProvider = async () =>
                            {
                                var currentToken = await _authService.GetAccessTokenAsync();
                                _logger.LogDebug("Providing access token for SignalR connection");
                                return currentToken;
                            };
                        }
                    )
                    .WithAutomaticReconnect(
                        new[]
                        {
                            TimeSpan.Zero,
                            TimeSpan.FromSeconds(2),
                            TimeSpan.FromSeconds(10),
                            TimeSpan.FromSeconds(30),
                        }
                    )
                    .Build();

                // Подписываемся на события
                SetupEventHandlers();

                // Подключаемся
                await _hubConnection.StartAsync();

                // Присоединяемся к группе комнаты
                await _hubConnection.InvokeAsync("JoinContentEditing", roomId);

                _logger.LogInformation("Connected to SignalR hub for room {RoomId}", roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to SignalR hub for room {RoomId}", roomId);
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                try
                {
                    if (_currentRoomId.HasValue && IsConnected)
                    {
                        await _hubConnection.InvokeAsync(
                            "ExitContentEditing",
                            _currentRoomId.Value
                        );
                    }

                    await _hubConnection.DisposeAsync();
                    _logger.LogInformation(
                        "Disconnected from SignalR hub for room {RoomId}",
                        _currentRoomId
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disconnecting from SignalR hub");
                }
                finally
                {
                    _hubConnection = null;
                    _currentRoomId = null;
                }
            }
        }

        private void SetupEventHandlers()
        {
            if (_hubConnection == null)
                return;

            _hubConnection.On<Guid, ContentNotificationData>(
                "ContentCreated",
                (editorId, content) =>
                {
                    _logger.LogDebug(
                        "Received ContentCreated event: {ContentId} by {EditorId}",
                        content.Id,
                        editorId
                    );
                    ContentCreated?.Invoke(editorId, content);
                }
            );

            _hubConnection.On<Guid, ContentNotificationData>(
                "ContentUpdated",
                (editorId, content) =>
                {
                    _logger.LogDebug(
                        "Received ContentUpdated event: {ContentId} by {EditorId}",
                        content.Id,
                        editorId
                    );
                    ContentUpdated?.Invoke(editorId, content);
                }
            );

            _hubConnection.On<Guid>(
                "ContentDeleted",
                (contentId) =>
                {
                    _logger.LogDebug("Received ContentDeleted event: {ContentId}", contentId);
                    ContentDeleted?.Invoke(contentId);
                }
            );

            _hubConnection.On<Guid, string, Guid>(
                "EditorInvited",
                (newEditorId, editorName, inviterId) =>
                {
                    _logger.LogDebug(
                        "Received EditorInvited event: {EditorId} invited by {InviterId}",
                        newEditorId,
                        inviterId
                    );
                    EditorInvited?.Invoke(newEditorId, editorName, inviterId);
                }
            );

            _hubConnection.On<Guid, Guid>(
                "EditorKicked",
                (kickedEditorId, kickInitiatorId) =>
                {
                    _logger.LogDebug(
                        "Received EditorKicked event: {EditorId} kicked by {InitiatorId}",
                        kickedEditorId,
                        kickInitiatorId
                    );
                    EditorKicked?.Invoke(kickedEditorId, kickInitiatorId);
                }
            );

            _hubConnection.Reconnecting += async (error) =>
            {
                _logger.LogWarning(
                    "SignalR connection lost, attempting to reconnect: {Error}",
                    error?.Message
                );

                // Если ошибка связана с аутентификацией, пытаемся обновить токен
                if (
                    error?.Message?.Contains("401") == true
                    || error?.Message?.Contains("Unauthorized") == true
                )
                {
                    _logger.LogInformation("Attempting to refresh token before reconnection");
                    try
                    {
                        var refreshSuccess = await _authService.RefreshTokenAsync();
                        if (!refreshSuccess)
                        {
                            _logger.LogWarning(
                                "Failed to refresh token, user may need to re-authenticate"
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error refreshing token during reconnection");
                    }
                }
            };

            _hubConnection.Reconnected += async (connectionId) =>
            {
                _logger.LogInformation(
                    "SignalR reconnected with connection ID: {ConnectionId}",
                    connectionId
                );

                // Повторно присоединяемся к группе после переподключения
                if (_currentRoomId.HasValue)
                {
                    try
                    {
                        await _hubConnection.InvokeAsync(
                            "JoinContentEditing",
                            _currentRoomId.Value
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Failed to rejoin room {RoomId} after reconnection",
                            _currentRoomId.Value
                        );
                    }
                }
            };

            _hubConnection.Closed += (error) =>
            {
                _logger.LogWarning("SignalR connection closed: {Error}", error?.Message);
                return Task.CompletedTask;
            };
        }

        private async void OnAuthStateChanged()
        {
            // Если пользователь вышел из системы, отключаемся от Hub
            if (!_authService.IsAuthenticated && IsConnected)
            {
                _logger.LogInformation("User logged out, disconnecting from SignalR hub");
                await DisconnectAsync();
            }
            // Если пользователь вошел в систему и у нас есть активная комната, переподключаемся
            else if (_authService.IsAuthenticated && _currentRoomId.HasValue && !IsConnected)
            {
                _logger.LogInformation(
                    "User authenticated, reconnecting to SignalR hub for room {RoomId}",
                    _currentRoomId.Value
                );
                try
                {
                    await ConnectAsync(_currentRoomId.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reconnect to SignalR hub after authentication");
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            // Отписываемся от событий аутентификации
            _authService.AuthStateChanged -= OnAuthStateChanged;

            await DisconnectAsync();
        }
    }
}
