namespace Metasite.WeatherApp.Infrastructure.Http
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IWeatherConditionsClient
    {
        Task<IEnumerable<Domain.WeatherConditions>> GetFor(
            IEnumerable<string> cities,
            CancellationToken ct = default);
    }
}