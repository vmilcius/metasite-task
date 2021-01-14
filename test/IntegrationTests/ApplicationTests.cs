namespace Metasite.WeatherApp.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Infrastructure.Storage;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    [Collection("integration")]
    public class ApplicationTests
    {
        [Fact]
        public async Task PrintsWeatherConditionsAndSavesToStorage()
        {
            var args = "weather --city Vilnius,Athens,Tbilisi,Yerevan".Split(' ');
            using var application = new CommandLineApplication(args);

            var weatherData = new WeatherConditions[]
            {
                new("Vilnius", -10.5, 11, "Cloudy"),
                new("Athens", 18.7, 2, "Sunny"),
                new("Tbilisi", 15.1, 4, "Sunny"),
                new("Yerevan", 25.9, 25, "Rainy")
            };

            var handler = application.FakeHttpClientHandler;

            var callToAuthorize = A.CallTo(() => handler.SendAsyncOverride(
                A<HttpRequestMessage>.That.Matches(message =>
                    message.Method == HttpMethod.Post
                    && message.RequestUri!.ToString().EndsWith("api/authorize")),
                A<CancellationToken>._));

            callToAuthorize.ReturnsLazily((HttpRequestMessage _, CancellationToken _) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{""bearer"":""1234567890""}")
                };
            });

            var callToGetWeatherData = A.CallTo(() => handler.SendAsyncOverride(
                A<HttpRequestMessage>.That.Matches(message =>
                    message.Method == HttpMethod.Get
                    && message.Headers.Authorization!.Scheme == "bearer"
                    && message.Headers.Authorization!.Parameter == "1234567890"),
                A<CancellationToken>._));

            callToGetWeatherData.ReturnsLazily((HttpRequestMessage m, CancellationToken _) =>
            {
                var city = m.RequestUri!.PathAndQuery.Split('/').Last();
                var data = weatherData.Single(w => w.City == city);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(data))
                };
            });

            // Let's give enough time to complete the work of the background service.
            application.CancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2));

            var returnCode = await application.RunAsync();
            Assert.Equal(0, returnCode);

            callToAuthorize.MustHaveHappened(4, Times.Exactly);
            callToGetWeatherData.MustHaveHappened(4, Times.Exactly);

            await AssertDatabase(weatherData);
            AssertConsoleOutput(application.StandardOutput);
        }

        [AssertionMethod]
        private static async Task AssertDatabase(IList<WeatherConditions> expectedConditions)
        {
            await using var dbContext = new WeatherConditionsDbContext();
            var conditions = await dbContext.WeatherConditions.ToListAsync();

            foreach (var c in conditions)
            {
                var expectedData = expectedConditions.Single(w => w.City == c.Location);
                Assert.Equal(expectedData.Precipitation, c.Precipitation);
                Assert.Equal(expectedData.Temperature, c.Temperature);
                Assert.Equal(expectedData.Weather, c.Type);
            }
        }

        private static void AssertConsoleOutput(string standardOutput)
        {
            var consoleLines = standardOutput
                .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            Assert.Contains("Weather at", consoleLines[0]);
            Assert.Equal("Vilnius, -10,5°C, 11mm, Cloudy", consoleLines[1]);
            Assert.Equal("Athens, 18,7°C, 2mm, Sunny", consoleLines[2]);
            Assert.Equal("Tbilisi, 15,1°C, 4mm, Sunny", consoleLines[3]);
            Assert.Equal("Yerevan, 25,9°C, 25mm, Rainy", consoleLines[4]);
            Assert.Equal("Stopping.", consoleLines[5]);
        }

        private record WeatherConditions(string City, double Temperature, double Precipitation, string Weather);
    }
}