namespace Metasite.WeatherApp.Application
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWeatherConditionsClient
    {
        Task<IReadOnlyCollection<Domain.WeatherConditions>> GetAll();
    }
}