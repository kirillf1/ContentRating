using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using ContentRating.Web.Contracts.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ContentRating.Web.UI.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
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

        public AuthService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            SecureTokenStorage tokenStorage
        )
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _tokenStorage = tokenStorage;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await CheckAuthStateAsync();
            }
            catch
            {
                // Игнорируем ошибки инициализации
            }
        }

        public Task LoginWithGoogle()
        {
            var returnUrl = Uri.EscapeDataString(
                _navigationManager.ToAbsoluteUri("/login-callback").ToString()
            );
            var redirectUrl =
                $"{_httpClient.BaseAddress}accounts/login-google?returnUrl={returnUrl}";
            _navigationManager.NavigateTo(redirectUrl, forceLoad: true);
            return Task.CompletedTask;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var refreshToken = await GetRefreshTokenAsync();
            var accessToken = await GetAccessTokenAsync();

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
                return false;

            try
            {
                var refreshRequest = new RefreshTokenRequest
                {
                    RefreshToken = refreshToken,
                    ExpiredAccessToken = accessToken,
                };
                var response = await _httpClient.PostAsJsonAsync(
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
            }
            catch
            {
                // Игнорируем ошибки рефреша токена
            }

            return false;
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
                return _accessToken;

            _accessToken = await _tokenStorage.GetAccessTokenAsync();
            return _accessToken;
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            if (_refreshToken != null)
                return _refreshToken;

            _refreshToken = await _tokenStorage.GetRefreshTokenAsync();
            return _refreshToken;
        }

        public async Task LogoutAsync()
        {
            // Очищаем токены и состояние
            _accessToken = null;
            _refreshToken = null;
            _userData = null;
            IsAuthenticated = false;
            UserName = null;
            UserEmail = null;
            UserId = null;

            // Очищаем безопасное хранилище
            await _tokenStorage.ClearAllTokensAsync();

            AuthStateChanged?.Invoke();
        }

        private async Task CheckAuthStateAsync()
        {
            try
            {
                // Сначала проверяем есть ли активная сессия в памяти
                if (await _tokenStorage.HasValidSessionAsync())
                {
                    _userData = await _tokenStorage.GetUserDataAsync();
                    if (_userData != null)
                    {
                        UpdateUserInfoFromUserData(_userData);
                        AuthStateChanged?.Invoke();
                        return;
                    }
                }

                var token = await GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    if (IsTokenValid(token))
                    {
                        UpdateUserInfo(token);
                    }
                    else
                    {
                        // Пытаемся обновить токен
                        var refreshSuccessful = await RefreshTokenAsync();
                        if (!refreshSuccessful)
                        {
                            await LogoutAsync();
                        }
                    }
                }
            }
            catch
            {
                // Если что-то пошло не так, просто устанавливаем неавторизованное состояние
                IsAuthenticated = false;
                UserName = null;
                UserEmail = null;
                UserId = null;
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
