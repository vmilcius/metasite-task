namespace Metasite.WeatherApp.Infrastructure.Http
{
    using System;
    using System.IO;
    using System.Net.Http;
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class WeatherConditionsClientRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))
                .Build();

            var username = configuration.GetValue<string>("WeatherConditionsApi:Username");
            var password = configuration.GetValue<string>("WeatherConditionsApi:Password");

            builder
                .Register(ctx =>
                {
                    var logger = ctx.Resolve<ILogger<WeatherConditionsClientHandler>>();

                    return new WeatherConditionsClientHandler(username, password, logger)
                    {
                        UseProxy = false,
                        UseDefaultCredentials = false
                    };
                })
                .SingleInstance();

            builder.Register(ctx =>
            {
                var handler = ctx.Resolve<WeatherConditionsClientHandler>();
                return new HttpClient(handler, false)
                {
                    BaseAddress = new Uri("https://metasite-weather-api.herokuapp.com/api/")
                };
            });

            builder
                .RegisterType<WeatherConditionsClient>()
                .As<IWeatherConditionsClient>();
        }
    }
}