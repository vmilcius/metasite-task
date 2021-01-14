namespace Metasite.WeatherApp.IntegrationTests
{
    using System.Threading.Tasks;
    using Xunit;

    [Collection("integration")]
    public class ArgumentValidationTests
    {
        [Theory]
        [InlineData("test")]
        [InlineData("definitely not")]
        [InlineData("bla bla bla")]
        public async Task DoesNotExecuteOnInvalidSubCommand(string args)
        {
            const string ExpectedMessage = "Specify --help for a list of available options and commands.\r\n";

            using var application = new CommandLineApplication(args.Split(' '));
            var returnCode = await application.RunAsync();

            Assert.Equal(ExpectedMessage, application.StandardOutput);
            Assert.Equal(255, returnCode);
        }

        [Fact]
        public async Task DoesNotExecuteSubCommandWithoutRequiredOptions()
        {
            using var application = new CommandLineApplication("weather");
            var returnCode = await application.RunAsync();

            Assert.Equal("The --city field is required.\r\n", application.StandardError);
            Assert.Equal(1, returnCode);
        }
    }
}
