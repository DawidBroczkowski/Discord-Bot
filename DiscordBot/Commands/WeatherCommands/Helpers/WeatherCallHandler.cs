using DiscordBot.Commands.WeatherCommands.Helpers.Interfaces;
using DiscordBot.Commands.WeatherCommands.Models;
using DiscordBot.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace DiscordBot.Commands.WeatherCommands.Helpers
{
    internal class WeatherCallHandler : IWeatherCallHandler
    {
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvdier;
        private IConfigRepository _configRepository;

        public WeatherCallHandler(IServiceProvider serviceProvider)
        {
            _httpClient = new HttpClient();
            _serviceProvdier = serviceProvider;
            _configRepository = _serviceProvdier.GetRequiredService<IConfigRepository>();
        }

        public WeatherCallHandler()
        {
            _httpClient = new HttpClient();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvdier = serviceProvider;
        }

        public void ConfigureClient()
        {
            // Client configuration:
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(3);
            _httpClient.BaseAddress = new Uri("http://api.openweathermap.org");
        }

        public async Task<HttpResponseMessage> GetCoordinatesByPlaceAsync(string place)
        {
            Configure();
            HttpResponseMessage response = await _httpClient.GetAsync($"/geo/1.0/direct?q={place}&limit=1&appid={_configRepository.Config.WeatherApiToken}");
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> GetCoordinatesByZipAsync(string zip)
        {
            Configure();
            HttpResponseMessage response = await _httpClient.GetAsync($"/geo/1.0/zip?zip={zip}&appid={_configRepository.Config.WeatherApiToken}");
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> GetCurrentWeatherAsync(Coordinates coordinates)
        {
            // Get weather data from API:
            Configure();
            HttpResponseMessage response = await _httpClient.GetAsync($"/data/2.5/weather?lat={coordinates.lat}&lon={coordinates.lon}&appid={_configRepository.Config.WeatherApiToken}");
            response.EnsureSuccessStatusCode();
            return response;
        }

        private void Configure()
        {
            _configRepository = _serviceProvdier.GetRequiredService<IConfigRepository>();
        }
    }
}
