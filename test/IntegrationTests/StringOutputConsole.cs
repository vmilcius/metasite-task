namespace Metasite.WeatherApp.IntegrationTests
{
    using System;
    using System.IO;
    using McMaster.Extensions.CommandLineUtils;

    public class StringOutputConsole : IConsole
    {
#pragma warning disable CS0067
        public event ConsoleCancelEventHandler? CancelKeyPress;
#pragma warning restore CS0067

        public TextWriter Out { get; } = new StringWriter();

        public TextWriter Error { get; } = new StringWriter();

        public TextReader In { get; } = new StringReader(string.Empty);

        public bool IsInputRedirected { get; } = false;

        public bool IsOutputRedirected { get; } = false;

        public bool IsErrorRedirected { get; } = false;

        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;

        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Standard output.
        /// </summary>
        /// <returns></returns>
        public string StandardOutput => Out.ToString() ?? string.Empty;

        /// <summary>
        /// Standard error output.
        /// </summary>
        /// <returns></returns>
        public string StandardError => Error.ToString() ?? string.Empty;

        public void ResetColor()
        {
        }
    }
}