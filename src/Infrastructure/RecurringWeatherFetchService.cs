namespace Metasite.WeatherApp.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Http;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Hosting;

    public class RecurringWeatherFetchService : IHostedService
    {
        private readonly IWeatherConditionsClient _client;
        private readonly IWeatherConditionsRepository _repository;
        private readonly IConsole _console;
        private readonly IEnumerable<string> _cities;

        public RecurringWeatherFetchService(
            IWeatherConditionsClient client,
            IWeatherConditionsRepository repository,
            IConsole console,
            IEnumerable<string> cities)
        {
            _client = client;
            _repository = repository;
            _console = console;
            _cities = cities;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var conditions = (await _client.GetFor(_cities, ct)).ToList();
                    await _repository.Store(conditions, ct);
                    PrintWeatherConditions(conditions);

                    await Task.Delay(TimeSpan.FromSeconds(30), ct);
                }
                catch (Exception e)
                {
                    _console.WriteLine($"Exception occurred: {e.Message}\nStopping.");
                    return;
                }
            }

            _console.WriteLine("Stopping.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void PrintWeatherConditions(IEnumerable<WeatherConditions> conditions)
        {
            var oldColor = _console.ForegroundColor;

            _console.ForegroundColor = ConsoleColor.Green;
            _console.WriteLine("Weather at {0:yyyy-MM-dd HH:mm:ss}:", DateTimeOffset.UtcNow);
            _console.ForegroundColor = ConsoleColor.White;

            foreach (var weatherConditions in conditions)
            {
                _console.WriteLine(weatherConditions);
            }

            _console.ForegroundColor = oldColor;
        }
    }
}