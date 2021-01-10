namespace Metasite.WeatherApp.Infrastructure.Http
{
    using System;
    using System.Net.Http;
    using Autofac;
    using Microsoft.Extensions.Logging;

    public class WeatherConditionsClientRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(ctx =>
                {
                    var logger = ctx.Resolve<ILogger<WeatherConditionsClientHandler>>();
                    return new WeatherConditionsClientHandler("meta", "site", logger)
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