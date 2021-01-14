namespace Metasite.WeatherApp.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Domain;
    using FakeItEasy;
    using Infrastructure;
    using Infrastructure.Http;
    using Infrastructure.Storage;
    using McMaster.Extensions.CommandLineUtils;
    using McMaster.Extensions.Hosting.CommandLine.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using UserInterface.CommandHandlers;

    public class CommandLineApplication : IDisposable
    {
        private readonly IConsole _console;
        private readonly IHost _host;
        private readonly CommandLineState _state;
        private readonly WeatherConditionsDbContext _databaseContext;

        public string StandardOutput => _console.Out.ToString() ?? string.Empty;

        public string StandardError => _console.Error.ToString() ?? string.Empty;

        internal WeatherConditionsClientHandler FakeHttpClientHandler { get; }

        internal CancellationTokenSource CancellationTokenSource { get; } = new();

        public CommandLineApplication(params string[] args)
        {
            _console = new StringOutputConsole();
            FakeHttpClientHandler = A.Fake<WeatherConditionsClientHandler>(o => o.CallsBaseMethods());

            _host = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(HostConfiguration.Container)
                .ConfigureServices(HostConfiguration.Services)
                .ConfigureLogging(HostConfiguration.Logging)
                .ConfigureCommandLineApplication<RootCommandHandler>(args, out var state)
                .ConfigureContainer<ContainerBuilder>(containerBuilder =>
                {
                    containerBuilder.RegisterInstance(_console);
                    containerBuilder.RegisterInstance(FakeHttpClientHandler);

                    containerBuilder.Register<Func<IEnumerable<string>, RecurringWeatherFetchService>>(ctx =>
                    {
                        var client = ctx.Resolve<IWeatherConditionsClient>();
                        var repository = ctx.Resolve<IWeatherConditionsRepository>();
                        var console = ctx.Resolve<IConsole>();

                        return cities => new RecurringWeatherFetchService(
                            client,
                            repository,
                            console,
                            cities,
                            CancellationTokenSource.Token);
                    });
                })
                .Build();

            _state = state;
            _databaseContext = _host.Services.GetRequiredService<WeatherConditionsDbContext>();
        }

        public async Task<int> RunAsync(Func<WeatherConditionsDbContext, Task>? databaseContextFunc = null)
        {
            await _databaseContext.MigrateDatabase();
            _databaseContext.WeatherConditions.RemoveRange(_databaseContext.WeatherConditions);
            await _databaseContext.SaveChangesAsync();

            if (databaseContextFunc != null)
            {
                await databaseContextFunc(_databaseContext);
            }

            await _host.RunAsync();

            return _state.ExitCode;
        }

        public void Dispose()
        {
            _host.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}