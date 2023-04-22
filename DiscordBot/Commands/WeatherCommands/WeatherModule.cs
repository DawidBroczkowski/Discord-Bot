using Discord.Interactions;
using DiscordBot.Commands.WeatherCommands.Interfaces;

namespace DiscordBot.Commands
{
    public class WeatherModule : InteractionModuleBase<SocketInteractionContext>
    {
        // Dependency injection for the use of config class:
        private readonly IServiceProvider _serviceProvider;
        private IWeatherService _weatherService;

        public WeatherModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _weatherService = new WeatherService();
        }
        // End of dependency injection.

        [CheckPermission(DataAccess.Enums.Commands.weather)]
        [SlashCommand("weather", "Gets current weather in a given place from OpenWeatherAPI")]
        public async Task WeatherAsync([Summary(description: "City/Town/Place name")] string place)
        {
            Configure();
            await _weatherService.SendCurrentWeatherByPlaceAsync(place);
        }

        [CheckPermission(DataAccess.Enums.Commands.weatherzip)]
        [SlashCommand("weatherzip", "Gets current weather in a place given by ZIP code from OpenWeatherAPI")]
        public async Task WeatherZipAsync([Summary(description: "ZIP code and country, ex: 00-001,PL / W1,GB etc.")] string zip)
        {
            Configure();
            await _weatherService.SendCurrentWeatherByZipAsync(zip);
        }

        private void Configure()
        {
            _weatherService.SetContext(Context);
            _weatherService.SetServiceProvider(_serviceProvider);  
        }
    }
}