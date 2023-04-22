using DiscordBot.Commands.WeatherCommands.Models;

namespace DiscordBot.Commands.WeatherCommands.Helpers.Interfaces
{
    internal interface IWeatherDeserializer
    {
        Task<Coordinates> DeserializeCoordinatesByPlaceAsync(HttpResponseMessage response);
        Task<Coordinates> DeserializeCoordinatesByZipAsync(HttpResponseMessage response);
    }
}