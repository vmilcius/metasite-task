namespace Metasite.WeatherApp.UserInterface
{
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using CommandHandlers;
    using Infrastructure;
    using Infrastructure.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            using var host = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(HostConfiguration.Container)
                .ConfigureServices(HostConfiguration.Services)
                .ConfigureLogging(HostConfiguration.Logging)
                .ConfigureCommandLineApplication<RootCommandHandler>(args, out var applicationState)
                .Build();

            var dbContext = host.Services.GetRequiredService<WeatherConditionsDbContext>();
            await dbContext.MigrateDatabase();

            await host.RunAsync();
            return applicationState.ExitCode;
        }
    }
}