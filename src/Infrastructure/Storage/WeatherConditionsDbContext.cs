#nullable disable
namespace Metasite.WeatherApp.Infrastructure.Storage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Domain;
    using JetBrains.Annotations;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public class WeatherConditionsDbContext : DbContext
    {
        private readonly string _connectionString;
        private readonly ILogger<WeatherConditionsDbContext> _logger;

        [UsedImplicitly]
        public DbSet<WeatherConditions> WeatherConditions { get; set; }

        public WeatherConditionsDbContext()
        {
            _logger = NullLogger<WeatherConditionsDbContext>.Instance;
        }

        public WeatherConditionsDbContext(ILogger<WeatherConditionsDbContext> logger)
        {
            _logger = logger;
        }

        public WeatherConditionsDbContext(
            string connectionString,
            ILogger<WeatherConditionsDbContext> logger = null)
        {
            _connectionString = connectionString;
            _logger = logger ?? NullLogger<WeatherConditionsDbContext>.Instance;
        }

        /// <summary>
        /// Checks if any pending migrations exist and applies them, if necessary.
        /// </summary>
        /// <returns></returns>
        public async Task MigrateDatabase()
        {
            var databaseFile = Path.Combine(AppContext.BaseDirectory, "weather.db");

            if (!File.Exists(databaseFile))
            {
                await using var fs = File.Create(databaseFile);
            }

            await Database.MigrateAsync();
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
                    .UseSqlite(_connectionString ?? "Data Source=weather.db")
                    .LogTo(message => _logger.LogInformation(message), LogLevel.Information);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherConditions>().ToTable("WeatherConditions");
            base.OnModelCreating(modelBuilder);
        }
    }
}