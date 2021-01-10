namespace Metasite.WeatherApp.Infrastructure.Storage
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;

    internal class WeatherConditionsRepository : IWeatherConditionsRepository
    {
        private readonly WeatherConditionsDbContext _dbContext;

        public WeatherConditionsRepository(WeatherConditionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Store(
            IEnumerable<WeatherConditions> weatherConditions,
            CancellationToken ct = default)
        {
            await _dbContext.AddRangeAsync(weatherConditions, ct);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}