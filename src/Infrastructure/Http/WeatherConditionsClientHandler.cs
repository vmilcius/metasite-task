namespace Metasite.WeatherApp.Infrastructure.Http
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Net.Mime;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    internal class WeatherConditionsClientHandler : LoggingHttpClientHandler
    {
        private readonly string _username;
        private readonly string _password;

        public WeatherConditionsClientHandler(
            string username,
            string password,
            ILogger logger)
            : base(logger)
        {
            _username = username;
            _password = password;
        }

        protected virtual Task<HttpResponseMessage> SendAsyncOverride(
            HttpRequestMessage message,
            CancellationToken ct)
        {
            return base.SendAsync(message, ct);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken ct)
        {
            var baseAddress = request.RequestUri!.GetLeftPart(UriPartial.Authority);

            var token = await Authorize(baseAddress, ct);
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

            return await SendAsyncOverride(request, ct);
        }

        private async Task<string> Authorize(string baseAddress, CancellationToken ct)
        {
            var credentials = new
            {
                Username = _username,
                Password = _password
            };

            using var content = new StringContent(
                JsonSerializer.Serialize(credentials, JsonOptions.Default),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            using var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(new Uri(baseAddress), "api/authorize"),
                Content = content
            };

            using var response = await SendAsyncOverride(requestMessage, ct);
            response.EnsureSuccessStatusCode();

            var result = await response
                .Content
                .ReadFromJsonAsync<AuthorizationResponse>(JsonOptions.Default, ct);

            return result!.Bearer;
        }

        [UsedImplicitly]
        private record AuthorizationResponse(string Bearer);
    }
}