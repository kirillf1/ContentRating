using ContentRating.Web.Contracts.ContentPartyEstimationRoom;
using ContentRating.Web.Contracts.ContentPartyRating;
using ContentRating.Web.UI.Services;
using MudBlazor;

namespace ContentRating.Web.UI.ViewModels
{
    public class ContentPartyEstimationRoomViewModel : IAsyncDisposable
    {
        private readonly IContentPartyEstimationService _estimationService;
        private readonly IContentPartyRatingService _ratingService;
        private readonly IContentPartyEstimationHubService _hubService;
        private readonly ISnackbar _snackbar;
        private readonly AuthService _authService;

        public ContentPartyEstimationRoomViewModel(
            IContentPartyEstimationService estimationService,
            IContentPartyRatingService ratingService,
            IContentPartyEstimationHubService hubService,
            ISnackbar snackbar,
            AuthService authService
        )
        {
            _estimationService = estimationService;
            _ratingService = ratingService;
            _hubService = hubService;
            _snackbar = snackbar;
            _authService = authService;

            // Подписываемся на события SignalR
            SubscribeToHubEvents();
        }

        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public Guid CreatorId { get; set; }
        public double MinRating { get; set; } = 0.0;
        public double MaxRating { get; set; } = 10.0;
        public bool IsEstimationCompleted { get; set; } = false;

        public List<ContentPartyRatingViewModel> ContentRatings { get; set; } = new();
        public List<RaterViewModel> Raters { get; set; } = new();

        public bool IsLoading { get; set; } = true;
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        // Текущий пользователь
        public Guid? CurrentUserId => _authService.UserId;
        public RaterViewModel? CurrentRater => Raters.FirstOrDefault(r => r.Id == CurrentUserId);

        // Событие для уведомления UI об изменениях
        public event Action? StateChanged;

        public async Task LoadRoomDataAsync(Guid roomId)
        {
            try
            {
                RoomId = roomId;
                IsLoading = true;
                HasError = false;

                var roomData = await _estimationService.GetRoomAsync(roomId);
                if (roomData == null)
                {
                    HasError = true;
                    ErrorMessage = "Комната не найдена";
                    return;
                }

                RoomName = roomData.Name;
                CreatorName = roomData.CreatorName;
                CreatorId = roomData.CreatorId;
                MinRating = roomData.MinRating;
                MaxRating = roomData.MaxRating;

                // Загружаем рейтеров
                Raters = roomData
                    .Raters.Select(r => new RaterViewModel { Id = r.Id, Name = r.Name })
                    .ToList();

                // Загружаем контент с оценками из ответа API
                ContentRatings = roomData
                    .ContentRatings.Select(cr =>
                    {
                        var viewModel = ContentPartyRatingViewModel.FromResponse(cr);

                        // Заполняем имена рейтеров
                        foreach (var rating in viewModel.Ratings)
                        {
                            var rater = Raters.FirstOrDefault(r => r.Id == rating.RaterId);
                            if (rater != null)
                            {
                                rating.RaterName = rater.Name;
                            }
                        }

                        return viewModel;
                    })
                    .ToList();

                // Подключаемся к SignalR для получения обновлений в реальном времени
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
                StateChanged?.Invoke();
            }
        }

        public async Task<bool> EstimateContentAsync(Guid contentRatingId, double newRating)
        {
            if (CurrentUserId == null)
            {
                _snackbar.Add("Пользователь не авторизован", Severity.Error);
                return false;
            }

            var content = ContentRatings.FirstOrDefault(c => c.RatingId == contentRatingId);
            if (content == null)
            {
                _snackbar.Add("Контент не найден", Severity.Error);
                return false;
            }

            // Валидация оценки
            if (newRating < MinRating || newRating > MaxRating)
            {
                _snackbar.Add(
                    $"Оценка должна быть от {MinRating:F2} до {MaxRating:F2}",
                    Severity.Error
                );
                return false;
            }

            try
            {
                // Отправляем оценку через SignalR
                if (_hubService.IsConnected)
                {
                    await _hubService.EstimateContentAsync(contentRatingId, newRating);

                    // Обновляем локальную оценку сразу для лучшего UX
                    UpdateLocalRating(contentRatingId, CurrentUserId.Value, newRating);

                    _snackbar.Add("Оценка сохранена", Severity.Success);
                    StateChanged?.Invoke();
                    return true;
                }
                else
                {
                    // Fallback на HTTP API если SignalR недоступен
                    var request = new EstimateContentRequest
                    {
                        NewScore = newRating,
                        RaterForChangeScoreId = CurrentUserId.Value,
                    };

                    var success = await _ratingService.EstimateContentAsync(
                        contentRatingId,
                        request
                    );

                    if (success)
                    {
                        UpdateLocalRating(contentRatingId, CurrentUserId.Value, newRating);
                        _snackbar.Add("Оценка сохранена", Severity.Success);
                        StateChanged?.Invoke();
                        return true;
                    }
                    else
                    {
                        _snackbar.Add("Не удалось сохранить оценку", Severity.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _snackbar.Add("Произошла ошибка при сохранении оценки", Severity.Error);
                return false;
            }
        }

        public double? GetCurrentUserRating(Guid contentRatingId)
        {
            if (CurrentUserId == null)
                return null;

            var content = ContentRatings.FirstOrDefault(c => c.RatingId == contentRatingId);
            var rating = content?.Ratings.FirstOrDefault(r => r.RaterId == CurrentUserId.Value);

            return rating?.Rating;
        }

        public bool CanRate(Guid contentRatingId)
        {
            return CurrentUserId != null && !IsEstimationCompleted;
        }

        public bool CanRateForMockUser(Guid mockUserId, Guid contentRatingId)
        {
            // Проверяем, что пользователь - mock и текущий пользователь может управлять оценками
            var mockUser = Raters.FirstOrDefault(r => r.Id == mockUserId);
            return mockUser?.IsMock == true && CurrentUserId != null && !IsEstimationCompleted;
        }

        public bool CanManageParticipants()
        {
            return CurrentUserId == CreatorId;
        }

        public bool CanCompleteEstimation()
        {
            return CurrentUserId == CreatorId && !IsEstimationCompleted;
        }

        public bool CanDeleteContent()
        {
            return CurrentUserId == CreatorId && !IsEstimationCompleted;
        }

        public async Task<bool> CompleteEstimationAsync()
        {
            if (!CanCompleteEstimation())
                return false;

            try
            {
                var success = await _estimationService.CompleteEstimationAsync(RoomId);
                if (success)
                {
                    _snackbar.Add("Оценка завершена", Severity.Success);
                    // Состояние обновится через SignalR
                    return true;
                }
                else
                {
                    _snackbar.Add("Не удалось завершить оценку", Severity.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Ошибка при завершении оценки: {ex.Message}", Severity.Error);
                return false;
            }
        }

        public async Task<bool> DeleteContentAsync(Guid contentId)
        {
            if (!CanDeleteContent())
            {
                _snackbar.Add("У вас нет прав для удаления контента", Severity.Error);
                return false;
            }

            var content = ContentRatings.FirstOrDefault(c => c.ContentId == contentId);
            if (content == null)
            {
                _snackbar.Add("Контент не найден", Severity.Error);
                return false;
            }

            try
            {
                var success = await _estimationService.RemoveContentAsync(RoomId, contentId);
                if (success)
                {
                    // Удаляем локально сразу для лучшего UX
                    // SignalR обновит остальных участников
                    ContentRatings.Remove(content);
                    _snackbar.Add($"Контент «{content.Name}» удален", Severity.Success);
                    StateChanged?.Invoke();
                    return true;
                }
                else
                {
                    _snackbar.Add("Не удалось удалить контент", Severity.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _snackbar.Add("Произошла ошибка при удалении контента", Severity.Error);
                return false;
            }
        }

        public async Task<bool> EstimateContentForMockUserAsync(
            Guid mockUserId,
            Guid contentRatingId,
            double newRating
        )
        {
            if (!CanRateForMockUser(mockUserId, contentRatingId))
            {
                _snackbar.Add("Вы не можете управлять оценками этого пользователя", Severity.Error);
                return false;
            }

            if (newRating < MinRating || newRating > MaxRating)
            {
                _snackbar.Add(
                    $"Оценка должна быть от {MinRating:F2} до {MaxRating:F2}",
                    Severity.Error
                );
                return false;
            }

            try
            {
                // Для mock пользователей используем HTTP API (они не подключены к SignalR)
                var request = new EstimateContentRequest
                {
                    NewScore = newRating,
                    RaterForChangeScoreId = mockUserId,
                };

                var success = await _ratingService.EstimateContentAsync(contentRatingId, request);

                if (success)
                {
                    UpdateLocalRating(contentRatingId, mockUserId, newRating);
                    var mockUser = Raters.FirstOrDefault(r => r.Id == mockUserId);
                    _snackbar.Add(
                        $"Оценка для {mockUser?.DisplayName} сохранена",
                        Severity.Success
                    );
                    StateChanged?.Invoke();
                    return true;
                }
                else
                {
                    _snackbar.Add("Не удалось сохранить оценку", Severity.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _snackbar.Add("Произошла ошибка при сохранении оценки", Severity.Error);
                return false;
            }
        }

        public double? GetMockUserRating(Guid mockUserId, Guid contentRatingId)
        {
            var content = ContentRatings.FirstOrDefault(c => c.RatingId == contentRatingId);
            var rating = content?.Ratings.FirstOrDefault(r => r.RaterId == mockUserId);
            return rating?.Rating;
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke();
        }

        private void SubscribeToHubEvents()
        {
            _hubService.RatingChanged += OnRatingChanged;
            _hubService.RaterInvited += OnRaterInvited;
            _hubService.RaterKicked += OnRaterKicked;
            _hubService.ContentDeleted += OnContentDeleted;
            _hubService.EstimationCompleted += OnEstimationCompleted;
            _hubService.RatingRangeChanged += OnRatingRangeChanged;
            _hubService.ConnectionLost += OnConnectionLost;
            _hubService.ConnectionRestored += OnConnectionRestored;
        }

        private void OnRatingChanged(Guid raterId, Guid ratingId, double score)
        {
            // Находим контент по ratingId
            var content = ContentRatings.FirstOrDefault(c => c.RatingId == ratingId);
            if (content != null)
            {
                UpdateLocalRating(ratingId, raterId, score);
                
                // Показываем уведомление о том, что пользователь оценил контент
                var rater = Raters.FirstOrDefault(r => r.Id == raterId);
                if (rater != null)
                {
                    var raterName = rater.DisplayName;
                    var contentName = content.Name;
                    var scoreFormatted = score.ToString("F1");
                    
                    _snackbar.Add(
                        $"{raterName} оценил «{contentName}» на {scoreFormatted}",
                        Severity.Info,
                        config =>
                        {
                            config.ShowCloseIcon = true;
                            config.VisibleStateDuration = 4000; // Показываем 4 секунды
                            config.HideTransitionDuration = 300;
                            config.ShowTransitionDuration = 300;
                        }
                    );
                }
                
                StateChanged?.Invoke();
            }
        }

        private void OnRaterInvited(Guid newRaterId, string raterName, double baseScore)
        {
            // Добавляем нового рейтера
            if (!Raters.Any(r => r.Id == newRaterId))
            {
                var rater = new RaterViewModel { Id = newRaterId, Name = raterName };
                Raters.Add(rater);

                // Добавляем базовую оценку для нового рейтера ко всему контенту
                foreach (var content in ContentRatings)
                {
                    // Проверяем, что у рейтера еще нет оценки для этого контента
                    var existingRating = content.Ratings.FirstOrDefault(r =>
                        r.RaterId == newRaterId
                    );
                    if (existingRating == null)
                    {
                        // Создаем новую оценку с базовым значением
                        var newRating = new RaterRatingViewModel
                        {
                            RaterId = newRaterId,
                            Rating = baseScore,
                            RaterName = raterName,
                        };

                        content.Ratings.Add(newRating);

                        // Пересчитываем среднюю оценку
                        content.AverageRating = content.Ratings.Any()
                            ? content.Ratings.Average(r => r.Rating)
                            : 0;
                    }
                }

                StateChanged?.Invoke();
            }
        }

        private void OnRaterKicked(Guid kickedRaterId)
        {
            // Удаляем рейтера
            var rater = Raters.FirstOrDefault(r => r.Id == kickedRaterId);
            if (rater != null)
            {
                Raters.Remove(rater);

                // Удаляем его оценки
                foreach (var content in ContentRatings)
                {
                    var ratingToRemove = content.Ratings.FirstOrDefault(r =>
                        r.RaterId == kickedRaterId
                    );
                    if (ratingToRemove != null)
                    {
                        content.Ratings.Remove(ratingToRemove);
                        // Пересчитываем среднюю оценку
                        content.AverageRating = content.Ratings.Any()
                            ? content.Ratings.Average(r => r.Rating)
                            : 0;
                    }
                }

                StateChanged?.Invoke();
            }
        }

        private void OnContentDeleted(Guid contentId)
        {
            // Удаляем контент
            var contentToRemove = ContentRatings.FirstOrDefault(c => c.ContentId == contentId);
            if (contentToRemove != null)
            {
                ContentRatings.Remove(contentToRemove);
                StateChanged?.Invoke();
            }
        }

        private void OnEstimationCompleted()
        {
            IsEstimationCompleted = true;
            StateChanged?.Invoke();
        }

        private void OnRatingRangeChanged(double minRating, double maxRating)
        {
            MinRating = minRating;
            MaxRating = maxRating;
            StateChanged?.Invoke();
        }

        private void OnConnectionLost()
        {
            _snackbar.Add("Соединение потеряно, попытка переподключения...", Severity.Warning);
            StateChanged?.Invoke();
        }

        private async void OnConnectionRestored()
        {
            _snackbar.Add("Соединение восстановлено, обновление данных...", Severity.Success);

            // Перезагружаем данные комнаты
            if (RoomId != Guid.Empty)
            {
                try
                {
                    await LoadRoomDataAsync(RoomId);
                }
                catch (Exception ex)
                {
                    _snackbar.Add(
                        "Ошибка при обновлении данных после переподключения",
                        Severity.Error
                    );
                }
            }

            StateChanged?.Invoke();
        }

        private void UpdateLocalRating(Guid contentRatingId, Guid raterId, double newRating)
        {
            var content = ContentRatings.FirstOrDefault(c => c.RatingId == contentRatingId);
            if (content == null)
                return;

            // Обновляем или добавляем оценку
            var existingRating = content.Ratings.FirstOrDefault(r => r.RaterId == raterId);
            if (existingRating != null)
            {
                existingRating.Rating = newRating;
            }
            else
            {
                var rater = Raters.FirstOrDefault(r => r.Id == raterId);
                content.Ratings.Add(
                    new RaterRatingViewModel
                    {
                        RaterId = raterId,
                        Rating = newRating,
                        RaterName =
                            rater?.Name ?? (raterId == CurrentUserId ? "Вы" : "Неизвестный"),
                    }
                );
            }

            // Пересчитываем среднюю оценку
            content.AverageRating = content.Ratings.Any()
                ? content.Ratings.Average(r => r.Rating)
                : 0;
        }

        public async ValueTask DisposeAsync()
        {
            // Отписываемся от событий SignalR
            _hubService.RatingChanged -= OnRatingChanged;
            _hubService.RaterInvited -= OnRaterInvited;
            _hubService.RaterKicked -= OnRaterKicked;
            _hubService.ContentDeleted -= OnContentDeleted;
            _hubService.EstimationCompleted -= OnEstimationCompleted;
            _hubService.RatingRangeChanged -= OnRatingRangeChanged;
            _hubService.ConnectionLost -= OnConnectionLost;
            _hubService.ConnectionRestored -= OnConnectionRestored;

            // Отключаемся от SignalR
            await _hubService.DisconnectAsync();
        }
    }

    public class RaterViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsMock => Name.StartsWith("Mock:");
        public string DisplayName => IsMock ? Name.Substring(5) : Name; // Убираем префикс "Mock:" для отображения
    }
}
