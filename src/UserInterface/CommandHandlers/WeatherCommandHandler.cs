namespace Metasite.WeatherApp.UserInterface.CommandHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using JetBrains.Annotations;
    using McMaster.Extensions.CommandLineUtils;

    [Command(Name = "weather", Description = "Prints weather data.")]
    public class WeatherCommandHandler : BaseCommandHandler
    {
        private readonly Func<IEnumerable<string>, RecurringWeatherFetchService> _serviceFactory;

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
            await _serviceFactory(Cities).StartAsync(ct);
        }
    }
}