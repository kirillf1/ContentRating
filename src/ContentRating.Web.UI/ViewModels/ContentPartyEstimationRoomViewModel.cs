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
        private readonly ISnackbar _snackbar;
        private readonly AuthService _authService;

        public ContentPartyEstimationRoomViewModel(
            IContentPartyEstimationService estimationService,
            IContentPartyRatingService ratingService,
            ISnackbar snackbar,
            AuthService authService
        )
        {
            _estimationService = estimationService;
            _ratingService = ratingService;
            _snackbar = snackbar;
            _authService = authService;
        }

        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
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

                // TODO: Подключиться к SignalR для получения обновлений в реальном времени
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
                _snackbar.Add($"Оценка должна быть от {MinRating} до {MaxRating}", Severity.Error);
                return false;
            }

            try
            {
                var request = new EstimateContentRequest
                {
                    NewScore = newRating,
                    RaterForChangeScoreId = CurrentUserId.Value,
                };

                var success = await _ratingService.EstimateContentAsync(contentRatingId, request);

                if (success)
                {
                    // Обновляем локальную оценку, убирая дублирование
                    var existingRating = content.Ratings.FirstOrDefault(r =>
                        r.RaterId == CurrentUserId.Value
                    );
                    if (existingRating != null)
                    {
                        existingRating.Rating = newRating;
                    }
                    else
                    {
                        content.Ratings.Add(
                            new RaterRatingViewModel
                            {
                                RaterId = CurrentUserId.Value,
                                Rating = newRating,
                                RaterName = CurrentRater?.Name ?? _authService.UserName ?? "Вы",
                            }
                        );
                    }

                    // Пересчитываем среднюю оценку
                    content.AverageRating = content.Ratings.Any()
                        ? content.Ratings.Average(r => r.Rating)
                        : 0;

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

        private void OnStateChanged()
        {
            StateChanged?.Invoke();
        }

        public ValueTask DisposeAsync()
        {
            // TODO: Отключиться от SignalR
            // Освобождение других ресурсов
            return ValueTask.CompletedTask;
        }
    }

    public class RaterViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
