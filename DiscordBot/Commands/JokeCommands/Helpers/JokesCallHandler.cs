using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static DiscordBot.Commands.JokeCommands.Models.Enums;
using DiscordBot.Commands.JokeCommands.Helpers.Interfaces;

namespace DiscordBot.Commands.JokeCommands.Helpers
{
    internal class JokesCallHandler : IJokesCallHandler
    {
        private HttpClient _httpClient;

        public JokesCallHandler()
        {
            _httpClient = new HttpClient();
        }
        public void ConfigureClient()
        {
            // Client configuration:
            _httpClient.Timeout = TimeSpan.FromSeconds(3);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> GetJokeAsync(JokeType type)
        {
            // Get a joke from API:
            HttpResponseMessage response = new();

            response = await _httpClient.GetAsync($"https://v2.jokeapi.dev/joke/{type}");
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
