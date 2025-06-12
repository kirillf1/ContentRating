using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ContentRating.Web.UI.Services
{
    public class SecureTokenStorage
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly string _encryptionKey;
        private const string ACCESS_TOKEN_KEY = "cr_at";
        private const string REFRESH_TOKEN_KEY = "cr_rt";
        private const string USER_DATA_KEY = "cr_ud";

        public SecureTokenStorage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _encryptionKey = GenerateOrGetEncryptionKey();
        }

        public async Task SetTokensAsync(string accessToken, string refreshToken, UserData? userData = null)
        {
            try
            {
                // Access token в sessionStorage (очищается при закрытии вкладки)
                var encryptedAccessToken = EncryptString(accessToken);
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", ACCESS_TOKEN_KEY, encryptedAccessToken);

                // Refresh token в localStorage с шифрованием (остается между сессиями)
                var encryptedRefreshToken = EncryptString(refreshToken);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", REFRESH_TOKEN_KEY, encryptedRefreshToken);

                // Данные пользователя в sessionStorage
                if (userData != null)
                {
                    var userDataJson = JsonSerializer.Serialize(userData);
                    var encryptedUserData = EncryptString(userDataJson);
                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", USER_DATA_KEY, encryptedUserData);
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не пробрасываем её
                Console.WriteLine($"Error storing tokens: {ex.Message}");
            }
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                var encryptedToken = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", ACCESS_TOKEN_KEY);
                return string.IsNullOrEmpty(encryptedToken) ? null : DecryptString(encryptedToken);
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
                var encryptedToken = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", REFRESH_TOKEN_KEY);
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
                var encryptedData = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", USER_DATA_KEY);
                if (string.IsNullOrEmpty(encryptedData))
                    return null;

                var userDataJson = DecryptString(encryptedData);
                return JsonSerializer.Deserialize<UserData>(userDataJson);
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
                await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", ACCESS_TOKEN_KEY);
                await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", USER_DATA_KEY);
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", REFRESH_TOKEN_KEY);
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
                var userData = await GetUserDataAsync();
                return !string.IsNullOrEmpty(accessToken) && userData != null;
            }
            catch
            {
                return false;
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

    public record UserData(
        string UserId,
        string UserName,
        string UserEmail,
        DateTime TokenExpiry
    );
} 