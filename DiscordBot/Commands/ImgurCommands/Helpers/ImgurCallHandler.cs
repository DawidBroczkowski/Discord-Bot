using DiscordBot.Commands.ImgurCommands.Helpers.Interfaces;
using DiscordBot.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands.ImgurCommands.Helpers
{
    internal class ImgurCallHandler : IImgurCallHandler
    {
        private IServiceProvider _serviceProvider;
        private IConfigRepository _configRepository;
        private HttpClient _httpClient;

        public ImgurCallHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _httpClient = new();
        }

        public ImgurCallHandler()
        {
            _httpClient = new();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ConfigureClient()
        {
            // Client configuration:
            _configRepository = _serviceProvider.GetRequiredService<IConfigRepository>();
            _httpClient.Timeout = TimeSpan.FromSeconds(3);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_configRepository.Config.ImgurClientId}");
        }

        public async Task<HttpResponseMessage> GetImagesAsync(string content)
        {
            // Get images from API:
            HttpResponseMessage response = new();

            response = await _httpClient.GetAsync($"https://api.imgur.com/3/gallery/search/top/all/1?q={content}");
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
