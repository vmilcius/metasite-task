namespace Metasite.WeatherApp.Infrastructure.Storage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.EntityFrameworkCore;

    internal class LocationsRepository : ILocationsRepository
    {
        private readonly LocationsDbContext _serversDbContext;

        public LocationsRepository(LocationsDbContext serversDbContext)
        {
            _serversDbContext = serversDbContext;
        }

        public async Task<IEnumerable<Location>> GetAll()
        {
            return await _serversDbContext.Locations.ToListAsync();
        }

        public async Task Store(IEnumerable<Location> servers)
        {
            await _serversDbContext.AddRangeAsync(servers);
            await _serversDbContext.SaveChangesAsync();
        }
    }
}