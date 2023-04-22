using DiscordBot.Commands.JokeCommands.Helpers.Interfaces;
using DiscordBot.Commands.JokeCommands.Models;
using System.Text.Json;

namespace DiscordBot.Commands.JokeCommands.Helpers
{
    internal class JokesDeserializer : IJokesDeserializer
    {
        public async Task<JokeModel>? DeserializeJokeAsync(HttpResponseMessage response)
        {
            // Deserialize JSON to joke:
            string jsonString = await response.Content.ReadAsStringAsync();
            JokeModel joke = JsonSerializer.Deserialize<JokeModel>(jsonString)!;
            return joke;
        }
    }
}
