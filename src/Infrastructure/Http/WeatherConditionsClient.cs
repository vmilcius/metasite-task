namespace Metasite.WeatherApp.Infrastructure.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using JetBrains.Annotations;

    public class WeatherConditionsClient : IWeatherConditionsClient
    {
        private readonly HttpClient _client;

        public WeatherConditionsClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<WeatherConditions>> GetFor(
            IEnumerable<string> cities,
            CancellationToken ct = default)
        {
            var tasks = cities.Select(city => GetWeatherConditions(city, ct));
            var responses = await Task.WhenAll(tasks);

            var conditions = responses.Select(r =>
            {
                var obj = new WeatherConditions(
                    Guid.NewGuid(),
                    r.City,
                    DateTime.UtcNow,
                    r.Temperature,
                    r.Precipitation,
                    r.Weather);

                return obj;
            });

            return conditions;
        }

        private async Task<Response> GetWeatherConditions(string city, CancellationToken ct)
        {
            var response = await _client.GetAsync($"weather/{city}", ct);

            var obj = await response
                .Content
                .ReadFromJsonAsync<Response>(JsonOptions.Default, ct);

            return obj ?? throw new ArgumentNullException(null, "Response is null.");
        }

        [UsedImplicitly]
        private record Response(string City, double Temperature, double Precipitation, string Weather);
    }
}