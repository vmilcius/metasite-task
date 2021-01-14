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
        private readonly CancellationToken? _cancellationToken;

        public RecurringWeatherFetchService(
            IWeatherConditionsClient client,
            IWeatherConditionsRepository repository,
            IConsole console,
            IEnumerable<string> cities,
            CancellationToken? cancellationToken = null)
        {
            _client = client;
            _repository = repository;
            _console = console;
            _cities = cities;
            _cancellationToken = cancellationToken;
        }

        public virtual async Task StartAsync(CancellationToken ct)
        {
            var token = _cancellationToken ?? ct;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var conditions = (await _client.GetFor(_cities, token)).ToList();
                    await _repository.Store(conditions, token);
                    PrintWeatherConditions(conditions);

                    await Task.Delay(TimeSpan.FromSeconds(30), token);
                }
                catch (TaskCanceledException)
                {
                    _console.WriteLine("Stopping.");
                    return;
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