namespace Metasite.WeatherApp.Domain
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IWeatherConditionsRepository
    {
        Task Store(IEnumerable<WeatherConditions> weatherConditions, CancellationToken ct = default);
    }
}