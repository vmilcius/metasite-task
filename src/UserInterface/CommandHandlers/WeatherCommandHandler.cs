namespace Metasite.WeatherApp.UserInterface.CommandHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application;
    using Infrastructure;
    using JetBrains.Annotations;
    using McMaster.Extensions.CommandLineUtils;

    [Command(Name = "weather", Description = "Prints weather data.")]
    public class WeatherCommandHandler : BaseCommandHandler
    {
        private readonly WeatherDataList _serversList;

        [Option("-c|--city", "Cities", CommandOptionType.MultipleValue)]
        [UsedImplicitly]
        private bool LoadCachedServers { get; }

        public WeatherCommandHandler(WeatherDataList serversList)
        {
            _serversList = serversList;
        }

        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            IEnumerable<Weather> servers;

            if (LoadCachedServers)
            {
                servers = await _serversList.GetCached();
            }
            else
            {
                servers = await _serversList.GetLatest();
            }
        }
    }
}