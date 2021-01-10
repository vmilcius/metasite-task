namespace Metasite.WeatherApp.Domain
{
    using System;

    public class WeatherConditions
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Location { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public double Temperature { get; private set; }

        public double Precipitation { get; private set; }

        public string Type { get; private set; } = string.Empty;

        public WeatherConditions()
        {
        }

        public WeatherConditions(
            Guid id,
            string location,
            DateTime createdAt,
            double temperature,
            double precipitation,
            string type)
        {
            Id = id;
            Location = location;
            CreatedAt = createdAt;
            Temperature = temperature;
            Precipitation = precipitation;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Location}, {Temperature}°C, {Precipitation}mm, {Type}";
        }
    }
}