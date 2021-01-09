namespace Metasite.WeatherApp.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;

    public class WeatherDataList
    {
        private readonly IWeatherDataClient _serversListClient;
        private readonly ILocationsRepository _repository;

        public WeatherDataList(IWeatherDataClient serversListClient, ILocationsRepository repository)
        {
            _serversListClient = serversListClient;
            _repository = repository;
        }

        public async Task<IEnumerable<Weather>> GetLatest()
        {
            var servers = await _serversListClient.GetAll();
            await _repository.Store(servers);

            var ordered = servers
                .Select(s => new Weather(s.Name, 234))
                .OrderBy(s => s.Name);

            return ordered;
        }

        public async Task<IEnumerable<Weather>> GetCached()
        {
            var servers = await _repository.GetAll();

            var ordered = servers
                .Select(s => new Weather(s.Name, 123))
                .OrderBy(s => s.Name);

            return ordered;
        }
    }
}