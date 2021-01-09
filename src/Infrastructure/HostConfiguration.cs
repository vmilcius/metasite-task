namespace Metasite.WeatherApp.Infrastructure
{
    using Application;
    using Autofac;
    using Domain;
    using McMaster.Extensions.Hosting.CommandLine;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Storage;

    public static class HostConfiguration
    {
        public static void Container(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<LocationsRepository>()
                .As<ILocationsRepository>();

            containerBuilder.RegisterType<WeatherDataList>();
        }

        public static void Services(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IUnhandledExceptionHandler, GlobalExceptionHandler>();
            serviceCollection.AddDbContext<LocationsDbContext>();
        }

        public static void Logging(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder
                .ClearProviders()
                .AddFile("activity.log")
                .SetMinimumLevel(LogLevel.Information);
        }
    }
}