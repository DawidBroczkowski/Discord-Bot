using System.Text.Json.Nodes;

namespace DiscordBot.Commands.WeatherCommands.Helpers.Interfaces
{
    internal interface IWeatherGetter
    {
        Task<JsonNode> GetCurrentWeatherByPlaceAsync(string place);
        Task<JsonNode> GetCurrentWeatherByZipAsync(string place);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}