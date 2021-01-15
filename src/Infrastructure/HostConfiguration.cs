namespace Metasite.WeatherApp.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Domain;
    using Http;
    using McMaster.Extensions.CommandLineUtils;
    using McMaster.Extensions.Hosting.CommandLine;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Storage;

    public static class HostConfiguration
    {
        public static void Container(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<GlobalExceptionHandler>()
                .As<IUnhandledExceptionHandler>();

            containerBuilder.RegisterModule<WeatherConditionsClientRegistrationModule>();

            containerBuilder
                .RegisterType<WeatherConditionsRepository>()
                .As<IWeatherConditionsRepository>();

            containerBuilder.Register<Func<IEnumerable<string>, RecurringWeatherFetchService>>(ctx =>
            {
                var client = ctx.Resolve<IWeatherConditionsClient>();
                var repository = ctx.Resolve<IWeatherConditionsRepository>();
                var console = ctx.Resolve<IConsole>();

                return cities => new RecurringWeatherFetchService(client, repository, console, cities);
            });
        }

        public static void Services(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<WeatherConditionsDbContext>();
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