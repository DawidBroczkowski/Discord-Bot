using Discord.Interactions;
using DiscordBot.Commands.WeatherCommands.Helpers;
using DiscordBot.Commands.WeatherCommands.Helpers.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace DiscordBot.Commands.WeatherCommands.Interfaces
{
    internal class WeatherService : IWeatherService
    {
        private IServiceProvider _serviceProvider;
        private IEmbedService _embedService;
        private SocketInteractionContext _context;
        private readonly IWeatherGetter _weatherGetter;
        private ZipCodeRepository _zipCodeRepository;

        public WeatherService(IServiceProvider serviceProvider, SocketInteractionContext context)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _weatherGetter = new WeatherGetter();
        }

        public WeatherService()
        {
            _weatherGetter = new WeatherGetter();
        }

        public void SetContext(SocketInteractionContext context)
        {
            _context = context;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SendCurrentWeatherByPlaceAsync(string place)
        {
            Configure();
            JsonNode forecastNode;
            try
            {
                forecastNode = await _weatherGetter.GetCurrentWeatherByPlaceAsync(place);
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("weather", ex.Message);
                return;
            }
            if (forecastNode == null)
            {
                await _embedService.ReplyErrorAsync("weather", "Couldn't get the weather. Maybe check the place name?");
                return;
            }

            await _embedService.SendCurrentWeatherDataAsync(forecastNode);
        }

        public async Task SendCurrentWeatherByZipAsync(string zip)
        {
            Configure();
            JsonNode forecastNode;

            string countryCode = zip.Substring(zip.Length - 2, 2);
            string givenZipCode = zip.Substring(0, zip.Length - 3);
            string? zipCodePattern = _zipCodeRepository.GetZipCode(countryCode);
            zipCodePattern = $"^({zipCodePattern})$";

            if (zipCodePattern is null)
            {
                await _embedService.ReplyErrorAsync("weather", "Country code not found.");
                return;
            }
            if (!Regex.IsMatch(givenZipCode, zipCodePattern))
            {
                await _embedService.ReplyErrorAsync("weather", "Zip code doesn't match the country's format.");
                return;
            }

            try
            {
                forecastNode = await _weatherGetter.GetCurrentWeatherByZipAsync(zip);
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("weather", ex.Message);
                return;
            }
            if (forecastNode == null)
            {
                await _embedService.ReplyErrorAsync("weather", "Couldn't get the weather. Maybe check the Zip formatting?");
                return;
            }

            await _embedService.SendCurrentWeatherDataAsync(forecastNode);
        }

        private void Configure()
        {
            _weatherGetter.SetServiceProvider(_serviceProvider);
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(_context);
            _zipCodeRepository = _serviceProvider.GetRequiredService<ZipCodeRepository>();
        }
    }
}
