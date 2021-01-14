namespace Metasite.WeatherApp.UserInterface.CommandHandlers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using JetBrains.Annotations;
    using McMaster.Extensions.CommandLineUtils;
    using static System.StringSplitOptions;

    [Command(Name = "weather", Description = "Prints weather data.")]
    public class WeatherCommandHandler : BaseCommandHandler
    {
        private readonly Func<IEnumerable<string>, RecurringWeatherFetchService> _serviceFactory;

        [Required]
        [Option("-c|--city", "Cities", CommandOptionType.MultipleValue)]
        [UsedImplicitly]
        private string[] Cities { get; } = Array.Empty<string>();

        public WeatherCommandHandler(
            Func<IEnumerable<string>, RecurringWeatherFetchService> serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        protected override async Task OnExecuteAsync(CommandLineApplication app, CancellationToken ct)
        {
            var cities = Cities;

            // Allow comma-separated input:
            if (Cities.Length == 1 && Cities[0].Contains(','))
            {
                cities = Cities[0].Split(',', RemoveEmptyEntries | TrimEntries);
            }

            await _serviceFactory(cities).StartAsync(ct);
        }
    }
}