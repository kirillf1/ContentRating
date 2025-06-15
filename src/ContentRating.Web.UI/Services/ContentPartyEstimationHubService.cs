using ContentRating.Web.Contracts.ContentPartyRating;
using ContentRating.Web.UI.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ContentRating.Web.UI.Services
{
    public class ContentPartyEstimationHubService : IContentPartyEstimationHubService, IAsyncDisposable
    {
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<ContentPartyEstimationHubService> _logger;
        private readonly AuthService _authService;
        private HubConnection? _hubConnection;
        private Guid? _currentRoomId;

        public ContentPartyEstimationHubService(
            ApiSettings apiSettings,
            ILogger<ContentPartyEstimationHubService> logger,
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

        public event Action<Guid, Guid, double>? RatingChanged;
        public event Action<Guid, string, double>? RaterInvited;
        public event Action<Guid>? RaterKicked;
        public event Action<Guid>? ContentDeleted;
        public event Action? EstimationCompleted;
        public event Action<double, double>? RatingRangeChanged;
        public event Action? ConnectionLost;
        public event Action? ConnectionRestored;

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
                        throw new UnauthorizedAccessException("Failed to refresh authentication token");
                    }
                }

                // Отключаемся от предыдущего подключения
                await DisconnectAsync();

                _currentRoomId = roomId;

                // Создаем новое подключение
                var hubUrl = $"{_apiSettings.BaseUrl.TrimEnd('/')}/partyEstimationHub";

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
                    .WithAutomaticReconnect(new InfiniteRetryPolicy())
                    .Build();

                // Подписываемся на события
                SetupEventHandlers();

                // Подключаемся
                await _hubConnection.StartAsync();

                // Присоединяемся к группе комнаты
                await _hubConnection.InvokeAsync("JoinEstimationRoom", roomId);

                _logger.LogInformation("Connected to SignalR hub for estimation room {RoomId}", roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to SignalR hub for estimation room {RoomId}", roomId);
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
                        await _hubConnection.InvokeAsync("ExitEstimationRoom", _currentRoomId.Value);
                    }

                    await _hubConnection.DisposeAsync();
                    _logger.LogInformation("Disconnected from SignalR hub for estimation room {RoomId}", _currentRoomId);
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

        public async Task EstimateContentAsync(Guid contentRatingId, double newRating)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected to SignalR hub");
            }

            if (!_authService.IsAuthenticated || _authService.UserId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            try
            {
                var request = new EstimateContentRequest
                {
                    NewScore = newRating,
                    RaterForChangeScoreId = _authService.UserId.Value,
                };

                await _hubConnection!.InvokeAsync("EstimateContent", contentRatingId, request);
                
                _logger.LogDebug("Sent rating {Rating} for content {ContentRatingId}", newRating, contentRatingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send rating through SignalR");
                throw;
            }
        }

        private void SetupEventHandlers()
        {
            if (_hubConnection == null)
                return;

            _hubConnection.On<Guid, Guid, double>(
                "RatingChanged",
                (raterId, ratingId, score) =>
                {
                    _logger.LogDebug("Received RatingChanged event: Rater {RaterId}, Rating {RatingId}, Score {Score}", 
                        raterId, ratingId, score);
                    
                    // Проверяем, что это не наша собственная оценка (избегаем дублирования)
                    if (raterId != _authService.UserId)
                    {
                        RatingChanged?.Invoke(raterId, ratingId, score);
                    }
                }
            );

            _hubConnection.On<Guid, string, double>(
                "RaterInvited",
                (newRaterId, raterName, baseScore) =>
                {
                    _logger.LogDebug("Received RaterInvited event: {RaterId} - {RaterName}", newRaterId, raterName);
                    RaterInvited?.Invoke(newRaterId, raterName, baseScore);
                }
            );

            _hubConnection.On<Guid>(
                "RaterKicked",
                (kickedRaterId) =>
                {
                    _logger.LogDebug("Received RaterKicked event: {RaterId}", kickedRaterId);
                    RaterKicked?.Invoke(kickedRaterId);
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

            _hubConnection.On(
                "EstimationCompleted",
                () =>
                {
                    _logger.LogDebug("Received EstimationCompleted event");
                    EstimationCompleted?.Invoke();
                }
            );

            _hubConnection.On<double, double>(
                "RatingRangeChanged",
                (minRating, maxRating) =>
                {
                    _logger.LogDebug("Received RatingRangeChanged event: {MinRating} - {MaxRating}", minRating, maxRating);
                    RatingRangeChanged?.Invoke(minRating, maxRating);
                }
            );

            _hubConnection.Reconnecting += async (error) =>
            {
                _logger.LogWarning("SignalR connection lost, attempting to reconnect: {Error}", error?.Message);
                
                // Уведомляем UI о потере соединения
                ConnectionLost?.Invoke();

                // Если ошибка связана с аутентификацией, пытаемся обновить токен
                if (error?.Message?.Contains("401") == true || error?.Message?.Contains("Unauthorized") == true)
                {
                    _logger.LogInformation("Attempting to refresh token before reconnection");
                    try
                    {
                        var refreshSuccess = await _authService.RefreshTokenAsync();
                        if (!refreshSuccess)
                        {
                            _logger.LogWarning("Failed to refresh token, user may need to re-authenticate");
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
                _logger.LogInformation("SignalR reconnected with connection ID {ConnectionId}", connectionId);
                
                // Переподключаемся к группе комнаты
                if (_currentRoomId.HasValue)
                {
                    try
                    {
                        await _hubConnection.InvokeAsync("JoinEstimationRoom", _currentRoomId.Value);
                        
                        // Уведомляем UI о восстановлении соединения для перезагрузки данных
                        ConnectionRestored?.Invoke();
                        
                        _logger.LogInformation("Successfully rejoined estimation room {RoomId} after reconnection", _currentRoomId.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to rejoin estimation room after reconnection");
                    }
                }
            };

            _hubConnection.Closed += async (error) =>
            {
                _logger.LogWarning("SignalR connection closed: {Error}", error?.Message);
                
                // Уведомляем UI о потере соединения
                ConnectionLost?.Invoke();
                
                // Если соединение закрыто не по нашей инициативе, пытаемся переподключиться
                if (error != null && _currentRoomId.HasValue)
                {
                    _logger.LogInformation("Attempting to reconnect after connection closed unexpectedly");
                    
                    // Ждем немного перед попыткой переподключения
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    
                    try
                    {
                        await ConnectAsync(_currentRoomId.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to reconnect after connection was closed");
                    }
                }
            };
        }

        private async void OnAuthStateChanged()
        {
            if (!_authService.IsAuthenticated && IsConnected)
            {
                _logger.LogInformation("User signed out, disconnecting from SignalR hub");
                await DisconnectAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_authService != null)
            {
                _authService.AuthStateChanged -= OnAuthStateChanged;
            }

            await DisconnectAsync();
        }
    }
} 