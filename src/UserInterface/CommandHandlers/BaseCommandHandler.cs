namespace Metasite.WeatherApp.UserInterface.CommandHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using McMaster.Extensions.CommandLineUtils;

    [HelpOption("-h|--help|-?")]
    public abstract class BaseCommandHandler
    {
        [UsedImplicitly]
        protected abstract Task OnExecuteAsync(CommandLineApplication app, CancellationToken ct);
    }
}