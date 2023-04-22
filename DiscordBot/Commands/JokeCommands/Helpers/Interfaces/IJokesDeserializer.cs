using DiscordBot.Commands.JokeCommands.Models;

namespace DiscordBot.Commands.JokeCommands.Helpers.Interfaces
{
    internal interface IJokesDeserializer
    {
        Task<JokeModel>? DeserializeJokeAsync(HttpResponseMessage response);
    }
}