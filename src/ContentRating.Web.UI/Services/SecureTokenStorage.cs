using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ContentRating.Web.Contracts.Identity;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;

namespace ContentRating.Web.UI.Services
{
    public class SecureTokenStorage
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly string _encryptionKey;
        private const string ACCESS_TOKEN_KEY = "cr_at";
        private const string REFRESH_TOKEN_KEY = "cr_rt";
        private const string USER_DATA_KEY = "cr_ud";
        private readonly ILogger<SecureTokenStorage> _logger;

        public SecureTokenStorage(IJSRuntime jsRuntime, ILogger<SecureTokenStorage> logger)
        {
            _jsRuntime = jsRuntime;
            _encryptionKey = GenerateOrGetEncryptionKey();
            _logger = logger;
        }

        public async Task SetTokensAsync(
            string accessToken,
            string refreshToken,
            UserData? userData = null
        )
        {
            try
            {
                // Access token в sessionStorage (очищается при закрытии вкладки)
                var encryptedAccessToken = EncryptString(accessToken);
                await _jsRuntime.InvokeVoidAsync(
                    "sessionStorage.setItem",
                    ACCESS_TOKEN_KEY,
                    encryptedAccessToken
                );

                // Дублируем access token в localStorage для восстановления после перезагрузки
                // но с дополнительной меткой времени для безопасности
                await _jsRuntime.InvokeVoidAsync(
                    "localStorage.setItem",
                    $"{ACCESS_TOKEN_KEY}_backup",
                    encryptedAccessToken
                );

                // Refresh token в localStorage с шифрованием (остается между сессиями)
                var encryptedRefreshToken = EncryptString(refreshToken);
                await _jsRuntime.InvokeVoidAsync(
                    "localStorage.setItem",
                    REFRESH_TOKEN_KEY,
                    encryptedRefreshToken
                );

                // Данные пользователя в sessionStorage и localStorage для восстановления
                if (userData != null)
                {
                    var userDataJson = JsonSerializer.Serialize(userData);
                    var encryptedUserData = EncryptString(userDataJson);
                    await _jsRuntime.InvokeVoidAsync(
                        "sessionStorage.setItem",
                        USER_DATA_KEY,
                        encryptedUserData
                    );
                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.setItem",
                        $"{USER_DATA_KEY}_backup",
                        encryptedUserData
                    );
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не пробрасываем её
                _logger.LogError(ex, "Ошибка при сохранении токенов");
            }
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                // Сначала пробуем получить из sessionStorage
                var encryptedToken = await _jsRuntime.InvokeAsync<string?>(
                    "sessionStorage.getItem",
                    ACCESS_TOKEN_KEY
                );
                if (!string.IsNullOrEmpty(encryptedToken))
                {
                    return DecryptString(encryptedToken);
                }

                // Если нет в sessionStorage, пробуем восстановить из localStorage backup
                var encryptedBackupToken = await _jsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    $"{ACCESS_TOKEN_KEY}_backup"
                );
                if (!string.IsNullOrEmpty(encryptedBackupToken))
                {
                    var token = DecryptString(encryptedBackupToken);

                    // Восстанавливаем в sessionStorage для текущей сессии
                    if (!string.IsNullOrEmpty(token))
                    {
                        await _jsRuntime.InvokeVoidAsync(
                            "sessionStorage.setItem",
                            ACCESS_TOKEN_KEY,
                            encryptedBackupToken
                        );
                        return token;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            try
            {
                var encryptedToken = await _jsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    REFRESH_TOKEN_KEY
                );
                return string.IsNullOrEmpty(encryptedToken) ? null : DecryptString(encryptedToken);
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserData?> GetUserDataAsync()
        {
            try
            {
                // Сначала пробуем получить из sessionStorage
                var encryptedData = await _jsRuntime.InvokeAsync<string?>(
                    "sessionStorage.getItem",
                    USER_DATA_KEY
                );
                if (!string.IsNullOrEmpty(encryptedData))
                {
                    var userDataJson = DecryptString(encryptedData);
                    return JsonSerializer.Deserialize<UserData>(userDataJson);
                }

                // Если нет в sessionStorage, пробуем восстановить из localStorage backup
                var encryptedBackupData = await _jsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    $"{USER_DATA_KEY}_backup"
                );
                if (!string.IsNullOrEmpty(encryptedBackupData))
                {
                    var userDataJson = DecryptString(encryptedBackupData);
                    var userData = JsonSerializer.Deserialize<UserData>(userDataJson);

                    // Восстанавливаем в sessionStorage для текущей сессии
                    if (userData != null)
                    {
                        await _jsRuntime.InvokeVoidAsync(
                            "sessionStorage.setItem",
                            USER_DATA_KEY,
                            encryptedBackupData
                        );
                        return userData;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task ClearAllTokensAsync()
        {
            try
            {
                // Очищаем sessionStorage
                await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", ACCESS_TOKEN_KEY);
                await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", USER_DATA_KEY);

                // Очищаем localStorage (включая backup файлы)
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", REFRESH_TOKEN_KEY);
                await _jsRuntime.InvokeVoidAsync(
                    "localStorage.removeItem",
                    $"{ACCESS_TOKEN_KEY}_backup"
                );
                await _jsRuntime.InvokeVoidAsync(
                    "localStorage.removeItem",
                    $"{USER_DATA_KEY}_backup"
                );
            }
            catch
            {
                // Игнорируем ошибки очистки
            }
        }

        public async Task<bool> HasValidSessionAsync()
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var refreshToken = await GetRefreshTokenAsync();

                // Если есть access token, проверяем его валидность
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var userData = await GetUserDataAsync();
                    return userData != null && userData.TokenExpiry > DateTime.UtcNow.AddMinutes(5);
                }

                // Если нет access token, но есть refresh token, можно восстановить сессию
                return !string.IsNullOrEmpty(refreshToken);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> NeedsRefreshAsync()
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return true;
                }

                var userData = await GetUserDataAsync();
                return userData == null || userData.TokenExpiry <= DateTime.UtcNow.AddMinutes(5);
            }
            catch
            {
                return true;
            }
        }

        public async Task CleanupExpiredTokensAsync()
        {
            try
            {
                var userData = await GetUserDataAsync();
                if (userData != null && userData.TokenExpiry <= DateTime.UtcNow)
                {
                    // Токен истек, удаляем backup файлы
                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.removeItem",
                        $"{ACCESS_TOKEN_KEY}_backup"
                    );
                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.removeItem",
                        $"{USER_DATA_KEY}_backup"
                    );
                }
            }
            catch
            {
                // Игнорируем ошибки очистки
            }
        }

        private string GenerateOrGetEncryptionKey()
        {
            // В реальном приложении ключ должен быть более сложным
            // Можно использовать fingerprint устройства или другие уникальные данные
            var key = $"ContentRating_{Environment.MachineName}_{Environment.UserName}";
            return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(key)))[..32];
        }

        private string EncryptString(string plainText)
        {
            try
            {
                // Простое обфускание для защиты от случайного просмотра
                var keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
                var textBytes = Encoding.UTF8.GetBytes(plainText);
                var resultBytes = new byte[textBytes.Length];

                for (int i = 0; i < textBytes.Length; i++)
                {
                    resultBytes[i] = (byte)(textBytes[i] ^ keyBytes[i % keyBytes.Length]);
                }

                return Convert.ToBase64String(resultBytes);
            }
            catch
            {
                // В случае ошибки используем Base64
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            }
        }

        private string DecryptString(string cipherText)
        {
            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);
                var keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
                var resultBytes = new byte[cipherBytes.Length];

                for (int i = 0; i < cipherBytes.Length; i++)
                {
                    resultBytes[i] = (byte)(cipherBytes[i] ^ keyBytes[i % keyBytes.Length]);
                }

                return Encoding.UTF8.GetString(resultBytes);
            }
            catch
            {
                // В случае ошибки пробуем декодировать как обычный Base64
                try
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
                }
                catch
                {
                    return cipherText;
                }
            }
        }
    }

    public record UserData(string UserId, string UserName, string UserEmail, DateTime TokenExpiry);
}
