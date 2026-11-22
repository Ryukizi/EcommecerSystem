using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Validacao.Web.Services
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private const string RefreshHeaderName = "X-Token-Refreshed";
        private static readonly SemaphoreSlim RefreshLock = new(1, 1);

        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _refreshHttpClient;

        public JwtAuthorizationHandler(IJSRuntime jsRuntime, HttpClient refreshHttpClient)
        {
            _jsRuntime = jsRuntime;
            _refreshHttpClient = refreshHttpClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await TryGetStorageAsync("authToken");
            if (!string.IsNullOrWhiteSpace(token) && request.Headers.Authorization is null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized || request.Headers.Contains(RefreshHeaderName))
            {
                return response;
            }

            try
            {
                var novoToken = await RenovarTokenComLockAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(novoToken))
                {
                    return response;
                }

                response.Dispose();
                var retry = await ClonarComTokenAsync(request, novoToken);
                return await base.SendAsync(retry, cancellationToken);
            }
            catch
            {
                return response;
            }
        }

        private async Task<string?> RenovarTokenComLockAsync(CancellationToken cancellationToken)
        {
            await RefreshLock.WaitAsync(cancellationToken);
            try
            {
                var refreshToken = await TryGetStorageAsync("refreshToken");
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return null;
                }

                var res = await _refreshHttpClient.PostAsJsonAsync("api/usuario/refresh", new RefreshRequest(refreshToken), cancellationToken);
                if (!res.IsSuccessStatusCode)
                {
                    await LimparTokensAsync();
                    return null;
                }

                var body = await res.Content.ReadFromJsonAsync<RefreshResponse>(cancellationToken: cancellationToken);
                if (string.IsNullOrWhiteSpace(body?.Token))
                {
                    await LimparTokensAsync();
                    return null;
                }

                await TrySetStorageAsync("authToken", body.Token);
                if (!string.IsNullOrWhiteSpace(body.RefreshToken))
                {
                    await TrySetStorageAsync("refreshToken", body.RefreshToken);
                }

                return body.Token;
            }
            finally
            {
                RefreshLock.Release();
            }
        }


        private async Task<string?> TryGetStorageAsync(string key)
        {
            try { return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key); }
            catch (Exception ex) when (ex is InvalidOperationException or JSException) { return null; }
        }

        private async Task TrySetStorageAsync(string key, string value)
        {
            try { await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value); }
            catch (Exception ex) when (ex is InvalidOperationException or JSException) { }
        }

        private async Task LimparTokensAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
            }
            catch { }
        }

        private static async Task<HttpRequestMessage> ClonarComTokenAsync(HttpRequestMessage original, string novoToken)
        {
            var clone = new HttpRequestMessage(original.Method, original.RequestUri);
            clone.Headers.TryAddWithoutValidation(RefreshHeaderName, "true");
            clone.Headers.Authorization = new AuthenticationHeaderValue("Bearer", novoToken);

            if (original.Content is not null)
            {
                clone.Content = new ByteArrayContent(await original.Content.ReadAsByteArrayAsync());
                foreach (var h in original.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }
            }

            foreach (var h in original.Headers)
            {
                if (!h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                {
                    clone.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }
            }

            return clone;
        }

        private sealed record RefreshRequest([property: JsonPropertyName("refreshToken")] string RefreshToken);
        private sealed record RefreshResponse([property: JsonPropertyName("token")] string Token, [property: JsonPropertyName("refreshToken")] string? RefreshToken);
    }
}