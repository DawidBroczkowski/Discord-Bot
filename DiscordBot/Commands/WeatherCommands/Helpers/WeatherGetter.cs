using DiscordBot.Commands.WeatherCommands.Helpers.Interfaces;
using DiscordBot.Commands.WeatherCommands.Models;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Nodes;

namespace DiscordBot.Commands.WeatherCommands.Helpers
{
    internal class WeatherGetter : IWeatherGetter
    {
        private IServiceProvider _serviceProvider;
        private IParseService _parseService;
        private readonly IWeatherCallHandler _weatherCallHandler;
        private readonly IWeatherDeserializer _weatherDeserializer;

        public WeatherGetter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _parseService = _serviceProvider.GetRequiredService<IParseService>();
            _weatherCallHandler = new WeatherCallHandler();
            _weatherDeserializer = new WeatherDeserializer();
        }

        public WeatherGetter()
        {
            _weatherCallHandler = new WeatherCallHandler();
            _weatherDeserializer = new WeatherDeserializer();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<JsonNode> GetCurrentWeatherByPlaceAsync(string place)
        {
            Configure();

            HttpResponseMessage response = await _weatherCallHandler.GetCoordinatesByPlaceAsync(place);
            if (response is null)
            {
                return null;
            }

            Coordinates coordinates = await _weatherDeserializer.DeserializeCoordinatesByPlaceAsync(response);
            if (coordinates is null)
            {
                return null;
            }

            response = await _weatherCallHandler.GetCurrentWeatherAsync(coordinates);
            if (response is null)
            {
                return null;
            }

            JsonNode forecastNode = await _parseService.ParseResponseAsync(response);
            if (forecastNode is null)
            {
                return null;
            }

            return forecastNode;
        }

        public async Task<JsonNode> GetCurrentWeatherByZipAsync(string place)
        {
            Configure();

            HttpResponseMessage response = await _weatherCallHandler.GetCoordinatesByZipAsync(place);
            if (response is null)
            {
                return null;
            }

            Coordinates coordinates = await _weatherDeserializer.DeserializeCoordinatesByZipAsync(response);
            if (coordinates is null)
            {
                return null;
            }

            response = await _weatherCallHandler.GetCurrentWeatherAsync(coordinates);
            if (response is null)
            {
                return null;
            }

            JsonNode forecastNode = await _parseService.ParseResponseAsync(response);
            if (forecastNode is null)
            {
                return null;
            }

            return forecastNode;
        }

        private void Configure()
        {
            _parseService = _serviceProvider.GetRequiredService<IParseService>();
            _weatherCallHandler.SetServiceProvider(_serviceProvider);
            _weatherCallHandler.ConfigureClient();
        }
    }
}
