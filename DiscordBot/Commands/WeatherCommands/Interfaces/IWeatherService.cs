using Discord.Interactions;

namespace DiscordBot.Commands.WeatherCommands.Interfaces
{
    internal interface IWeatherService
    {
        Task SendCurrentWeatherByPlaceAsync(string place);
        Task SendCurrentWeatherByZipAsync(string zip);
        void SetContext(SocketInteractionContext context);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}