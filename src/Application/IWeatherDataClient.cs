namespace Metasite.WeatherApp.Application
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWeatherDataClient
    {
        Task<IReadOnlyCollection<Domain.Location>> GetAll();
    }
}