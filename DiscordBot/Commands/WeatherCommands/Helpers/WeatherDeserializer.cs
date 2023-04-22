using DiscordBot.Commands.WeatherCommands.Helpers.Interfaces;
using DiscordBot.Commands.WeatherCommands.Models;
using System.Text.Json;

namespace DiscordBot.Commands.WeatherCommands.Helpers
{
    internal class WeatherDeserializer : IWeatherDeserializer
    {
        public async Task<Coordinates> DeserializeCoordinatesByPlaceAsync(HttpResponseMessage response)
        {
            // Convert response to string.
            var stringResult = await response.Content.ReadAsStringAsync();

            // Deserialize JSON coordinates:
            List<Coordinates> coordinates = JsonSerializer.Deserialize<List<Coordinates>>(stringResult);
            return coordinates[0];
        }

        public async Task<Coordinates> DeserializeCoordinatesByZipAsync(HttpResponseMessage response)
        {
            // Convert response to string.
            var stringResult = await response.Content.ReadAsStringAsync();

            // Deserialize JSON coordinates:
            Coordinates coordinates = JsonSerializer.Deserialize<Coordinates>(stringResult);
            return coordinates;
        }
    }
}
