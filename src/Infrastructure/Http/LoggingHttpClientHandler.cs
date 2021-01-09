namespace Metasite.WeatherApp.Infrastructure.Http
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    internal class LoggingHttpClientHandler : HttpClientHandler
    {
        private readonly ILogger _logger;

        protected LoggingHttpClientHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();

            sw.Start();
            var response = await base.SendAsync(request, cancellationToken);
            sw.Stop();

            var uri = response.RequestMessage?.RequestUri;
            var method = response.RequestMessage?.Method;
            var statusCode = response.StatusCode;

            // ReSharper disable once UseStringInterpolation
            var message = string.Format(
                "HTTP request. Uri: {0}, method: {1}, status: {2}. Elapsed: {3} ms.",
                uri,
                method,
                statusCode,
                sw.ElapsedMilliseconds);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(message);
            }
            else
            {
                _logger.LogError(message);
            }

            return response;
        }
    }
}
