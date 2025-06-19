using System.Net.Http.Headers;

namespace ContentRating.Web.UI.Services
{
    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly AuthService _authService;

        public AuthenticatedHttpClientHandler(AuthService authService)
            : base(new HttpClientHandler())
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            var token = await _authService.GetAccessTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Если получили 401, пытаемся обновить токен
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshSuccessful = await _authService.RefreshTokenAsync();
                if (refreshSuccessful)
                {
                    // Повторяем запрос с новым токеном
                    var newToken = await _authService.GetAccessTokenAsync();
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        // Клонируем запрос для повторной отправки
                        var clonedRequest = await CloneHttpRequestMessageAsync(request);
                        clonedRequest.Headers.Authorization = new AuthenticationHeaderValue(
                            "Bearer",
                            newToken
                        );
                        response = await base.SendAsync(clonedRequest, cancellationToken);
                    }
                }
                else
                {
                    await _authService.LogoutAsync();
                }
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(
            HttpRequestMessage request
        )
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version,
            };

            // Копируем заголовки
            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Копируем содержимое
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(content);

                // Копируем заголовки содержимого
                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return clone;
        }
    }
}
