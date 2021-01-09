#nullable disable
namespace Metasite.WeatherApp.Infrastructure.Storage
{
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using JetBrains.Annotations;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public class LocationsDbContext : DbContext
    {
        private readonly string _connectionString;
        private readonly ILogger<LocationsDbContext> _logger;

        [UsedImplicitly]
        public DbSet<Location> Locations { get; set; }

        public LocationsDbContext()
        {
            _logger = NullLogger<LocationsDbContext>.Instance;
        }

        public LocationsDbContext(ILogger<LocationsDbContext> logger)
        {
            _logger = logger;
        }

        public LocationsDbContext(string connectionString, ILogger<LocationsDbContext> logger = null)
        {
            _connectionString = connectionString;
            _logger = logger ?? NullLogger<LocationsDbContext>.Instance;
        }

        /// <summary>
        /// Checks if any pending migrations exist and applies them, if necessary.
        /// </summary>
        /// <returns></returns>
        public async Task MigrateDatabase()
        {
            var pendingMigrations = (await Database.GetPendingMigrationsAsync()).ToList();

            if (pendingMigrations.Any())
            {
                var message = $"Pending migrations: {string.Join(',', pendingMigrations)}";
                _logger.LogInformation(message);
                await Database.MigrateAsync();
            }
        }

        /// <summary>
        /// Executes TRUNCATE against the provided database table.
        /// Use with extreme caution.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public async Task Truncate<TEntity>() where TEntity : class
        {
            var tableName = Set<TEntity>().EntityType.GetTableName();
            await Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString?.Contains("Mode=Memory") ?? false)
            {
                var inMemoryConnection = new SqliteConnection(_connectionString);
                inMemoryConnection.Open();
                optionsBuilder.UseSqlite(inMemoryConnection);
            }
            else
            {
                optionsBuilder
                    .UseSqlite(_connectionString ?? "Data Source=servers.db")
                    .LogTo(message => _logger.LogInformation(message), LogLevel.Information);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO : set the db model
            base.OnModelCreating(modelBuilder);
        }
    }
}