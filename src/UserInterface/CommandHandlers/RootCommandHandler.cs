namespace Metasite.WeatherApp.UserInterface.CommandHandlers
{
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using McMaster.Extensions.CommandLineUtils;

    [Command(Name = "metaapp.exe", Description = "Metasite .NET application")]
    [Subcommand(typeof(WeatherCommandHandler))]
    [UsedImplicitly]
    public class RootCommandHandler : BaseCommandHandler
    {
        protected override Task OnExecuteAsync(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.CompletedTask;
        }
    }
}