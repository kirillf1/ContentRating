using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using ContentRating.Web.Contracts.Identity;
using Microsoft.AspNetCore.Components;

namespace ContentRating.Web.UI.Services
{
    public class AuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NavigationManager _navigationManager;
        private readonly SecureTokenStorage _tokenStorage;

        public event Action? AuthStateChanged;
        public bool IsAuthenticated { get; private set; }
        public string? UserName { get; private set; }
        public string? UserEmail { get; private set; }
        public Guid? UserId { get; private set; }

        private string? _accessToken;
        private string? _refreshToken;
        private UserData? _userData;
        private bool _isInitialized = false;
        private bool _isInitializing = false;
        private bool _isRefreshing = false;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            NavigationManager navigationManager,
            SecureTokenStorage tokenStorage
        )
        {
            _httpClientFactory = httpClientFactory;
            _navigationManager = navigationManager;
            _tokenStorage = tokenStorage;
        }

        private HttpClient CreateHttpClient()
        {
            return _httpClientFactory.CreateClient("AuthHttpClient");
        }

        public async Task InitializeAsync()
        {
            // Если уже инициализировано или инициализируется, не делаем повторную инициализацию
            if (_isInitialized || _isInitializing)
            {
                return;
            }

            _isInitializing = true;
            try
            {
                await CheckAuthStateAsync();
                _isInitialized = true;
            }
            catch
            {
                // Игнорируем ошибки инициализации
            }
            finally
            {
                _isInitializing = false;
            }
        }

        public Task LoginWithGoogle()
        {
            var returnUrl = Uri.EscapeDataString(
                _navigationManager.ToAbsoluteUri("/login-callback").ToString()
            );

            using var httpClient = CreateHttpClient();
            var redirectUrl =
                $"{httpClient.BaseAddress}accounts/login-google?returnUrl={returnUrl}";
            _navigationManager.NavigateTo(redirectUrl, forceLoad: true);
            return Task.CompletedTask;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            // Если уже происходит обновление токена, ждем его завершения
            if (_isRefreshing)
            {
                // Ждем максимум 10 секунд для завершения обновления
                var timeout = TimeSpan.FromSeconds(10);
                var start = DateTime.UtcNow;

                while (_isRefreshing && DateTime.UtcNow - start < timeout)
                {
                    await Task.Delay(100);
                }

                // Если обновление завершилось успешно, возвращаем true
                return IsAuthenticated && !string.IsNullOrEmpty(_accessToken);
            }

            _isRefreshing = true;
            try
            {
                var refreshToken = await GetRefreshTokenAsync();
                var accessToken = await GetAccessTokenAsync();

                if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
                {
                    // Если токенов нет, очищаем состояние
                    await ClearAuthStateAsync();
                    return false;
                }

                var refreshRequest = new RefreshTokenRequest
                {
                    RefreshToken = refreshToken,
                    ExpiredAccessToken = accessToken,
                };

                using var httpClient = CreateHttpClient();
                var response = await httpClient.PostAsJsonAsync(
                    "accounts/refresh-token",
                    refreshRequest
                );

                if (response.IsSuccessStatusCode)
                {
                    var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();
                    if (loginResult != null)
                    {
                        await SetTokensAsync(loginResult.Token, loginResult.RefreshToken);
                        UpdateUserInfo(loginResult.Token);
                        return true;
                    }
                }

                // Если обновление не удалось, очищаем состояние
                await ClearAuthStateAsync();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // При ошибке очищаем состояние
                await ClearAuthStateAsync();
                return false;
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        public async Task SetTokensAsync(string accessToken, string refreshToken)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;

            // Получаем данные из токена
            var tokenData = ExtractUserDataFromToken(accessToken);
            _userData = tokenData;

            // Сохраняем в безопасное хранилище
            await _tokenStorage.SetTokensAsync(accessToken, refreshToken, tokenData);

            UpdateUserInfo(accessToken);
            AuthStateChanged?.Invoke();
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            if (_accessToken != null)
            {
                return _accessToken;
            }

            _accessToken = await _tokenStorage.GetAccessTokenAsync();
            return _accessToken;
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            if (_refreshToken != null)
            {
                return _refreshToken;
            }

            _refreshToken = await _tokenStorage.GetRefreshTokenAsync();
            return _refreshToken;
        }

        public async Task LogoutAsync()
        {
            await ClearAuthStateAsync();
        }

        private async Task ClearAuthStateAsync()
        {
            // Очищаем токены и состояние
            _accessToken = null;
            _refreshToken = null;
            _userData = null;
            IsAuthenticated = false;
            UserName = null;
            UserEmail = null;
            UserId = null;
            _isInitialized = false;
            _isRefreshing = false;

            // Очищаем безопасное хранилище
            await _tokenStorage.ClearAllTokensAsync();

            AuthStateChanged?.Invoke();
        }

        private async Task CheckAuthStateAsync()
        {
            try
            {
                // Если токен уже обновляется, не делаем ничего
                if (_isRefreshing)
                {
                    return;
                }

                // Проверяем нужно ли обновить токен
                if (await _tokenStorage.NeedsRefreshAsync())
                {
                    var refreshToken = await _tokenStorage.GetRefreshTokenAsync();
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        // Пытаемся восстановить сессию через refresh token
                        var refreshSuccessful = await RefreshTokenAsync();
                        if (refreshSuccessful)
                        {
                            // Сессия восстановлена, загружаем данные пользователя
                            _userData = await _tokenStorage.GetUserDataAsync();
                            if (_userData != null)
                            {
                                UpdateUserInfoFromUserData(_userData);
                                AuthStateChanged?.Invoke();
                                return;
                            }
                        }
                        else
                        {
                            // Не удалось обновить токен, выходим
                            await LogoutAsync();
                            return;
                        }
                    }
                }
                else
                {
                    // У нас есть валидная сессия
                    _userData = await _tokenStorage.GetUserDataAsync();
                    if (_userData != null)
                    {
                        UpdateUserInfoFromUserData(_userData);
                        AuthStateChanged?.Invoke();
                        return;
                    }
                }

                // Если дошли до сюда, проверяем есть ли токен в памяти
                var token = await GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token) && IsTokenValid(token))
                {
                    UpdateUserInfo(token);
                }
                else
                {
                    // Нет валидного токена, очищаем состояние
                    await ClearAuthStateAsync();
                    return; // Выходим, так как ClearAuthStateAsync уже вызовет AuthStateChanged
                }
            }
            catch
            {
                // Если что-то пошло не так, очищаем состояние
                try
                {
                    await ClearAuthStateAsync();
                }
                catch
                {
                    // Если не удалось очистить состояние, устанавливаем минимально необходимое
                    IsAuthenticated = false;
                    UserName = null;
                    UserEmail = null;
                    UserId = null;
                    AuthStateChanged?.Invoke();
                }
                return; // Выходим, так как состояние уже очищено
            }

            AuthStateChanged?.Invoke();
        }

        private bool IsTokenValid(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                return jsonToken.ValidTo > DateTime.UtcNow.AddMinutes(5); // Проверяем, что токен действителен еще 5 минут
            }
            catch
            {
                return false;
            }
        }

        private void UpdateUserInfo(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                UserName = jsonToken
                    .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)
                    ?.Value;
                UserEmail = jsonToken
                    .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)
                    ?.Value;
                var userIdClaim = jsonToken
                    .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)
                    ?.Value;

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    UserId = userId;
                }

                IsAuthenticated = !string.IsNullOrEmpty(UserName);
            }
            catch
            {
                IsAuthenticated = false;
                UserName = null;
                UserEmail = null;
                UserId = null;
            }
        }

        private void UpdateUserInfoFromUserData(UserData userData)
        {
            UserName = userData.UserName;
            UserEmail = userData.UserEmail;
            if (Guid.TryParse(userData.UserId, out var userId))
            {
                UserId = userId;
            }
            IsAuthenticated = true;
        }

        private static UserData ExtractUserDataFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                var userName =
                    jsonToken
                        .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)
                        ?.Value ?? "";
                var userEmail =
                    jsonToken
                        .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)
                        ?.Value ?? "";
                var userIdClaim =
                    jsonToken
                        .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)
                        ?.Value ?? "";

                return new UserData(userIdClaim, userName, userEmail, jsonToken.ValidTo);
            }
            catch
            {
                return new UserData("", "", "", DateTime.UtcNow);
            }
        }
    }
}
