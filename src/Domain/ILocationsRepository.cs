namespace Metasite.WeatherApp.Domain
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILocationsRepository
    {
        Task<IEnumerable<Location>> GetAll();

        Task Store(IEnumerable<Location> servers);
    }
}