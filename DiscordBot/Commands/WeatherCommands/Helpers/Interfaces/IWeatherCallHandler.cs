using DiscordBot.Commands.WeatherCommands.Models;

namespace DiscordBot.Commands.WeatherCommands.Helpers.Interfaces
{
    internal interface IWeatherCallHandler
    {
        void ConfigureClient();
        Task<HttpResponseMessage> GetCoordinatesByPlaceAsync(string place);
        Task<HttpResponseMessage> GetCoordinatesByZipAsync(string zip);
        Task<HttpResponseMessage> GetCurrentWeatherAsync(Coordinates coordinates);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}